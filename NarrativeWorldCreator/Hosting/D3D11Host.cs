using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using NarrativeWorldCreator.Graphics;
using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace NarrativeWorldCreator.Hosting
{
    /// <summary>
    /// Host a Direct3D 11 scene in WPF. Override to implement.
    /// This class uses the <see cref="Image"/> class to host its content.
    /// The actual game is rendered into a DirectX image (D3D11Image) and then displayed as the source.
    /// </summary>
    public abstract class D3D11Host : System.Windows.Controls.Image
    {
        public const int DefaultWidth = 1920;
        public const int DefaultHeight = 1080;

        private static bool? _IsInDesignMode;

        private readonly WpfGraphicsDeviceManager _GraphicsDeviceManager;
        private readonly GameServiceContainer _Services;
        //private readonly IContentProvider _ContentProvider;
        private readonly Stopwatch _Timer;
        private readonly int _Height;
        private readonly int _Width;

        // Image source:
        private RenderTarget2D _RenderTarget;
        private D3D11Image _D3D11Image;
        private bool _ResetBackBuffer;

        // Render timing:
        private TimeSpan _LastRenderingTime;

        private bool _Loaded;
        private ContentManager _ContentManager;

        protected D3D11Host()
            : this(DefaultWidth, DefaultHeight)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="D3D11Host"/> class.
        /// </summary>
        protected D3D11Host(int width, int height)
        {
            _Height = height;
            _Width = width;

            _Timer = new Stopwatch();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;

            _Services = new GameServiceContainer();
            _GraphicsDeviceManager = new WpfGraphicsDeviceManager(this, width, height);
            _GraphicsDeviceManager.CreateDevice();

            Initialize();

        }

        /// <summary>
        /// Gets a value indicating whether the controls runs in the context of a designer (e.g.
        /// Visual Studio Designer or Expression Blend).
        /// </summary>
        /// <value>
        /// <see langword="true" /> if controls run in design mode; otherwise, 
        /// <see langword="false" />.
        /// </value>
        public static bool IsInDesignMode
        {
            get
            {
                if (!_IsInDesignMode.HasValue)
                    _IsInDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty, typeof(FrameworkElement)).Metadata.DefaultValue;

                return _IsInDesignMode.Value;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs eventArgs)
        {
            if (IsInDesignMode || _Loaded)
                return;
            
            _ContentManager = new ContentManager(_Services);
            _ContentManager.RootDirectory = "Content";
            
            _Loaded = true;
            InitializeImageSource();
            StartRendering();
            Load();
        }


        private void OnUnloaded(object sender, RoutedEventArgs eventArgs)
        {
            if (IsInDesignMode)
                return;

            StopRendering();
            Unload();
            UnInitializeImageSource();

            if (_GraphicsDeviceManager != null)
                _GraphicsDeviceManager.Dispose();

            _Loaded = false;
        }

   

        private void InitializeImageSource()
        {
            _D3D11Image = new D3D11Image();
            _D3D11Image.IsFrontBufferAvailableChanged += OnIsFrontBufferAvailableChanged;
            
            CreateBackBuffer();

            Source = _D3D11Image;
        }

        private void UnInitializeImageSource()
        {
            _D3D11Image.IsFrontBufferAvailableChanged -= OnIsFrontBufferAvailableChanged;
            Source = null;

            if (_D3D11Image != null)
            {
                _D3D11Image.Dispose();
                _D3D11Image = null;
            }
            if (_RenderTarget != null)
            {
                _RenderTarget.Dispose();
                _RenderTarget = null;
            }
        }

        private void CreateBackBuffer()
        {
            _D3D11Image.SetBackBuffer(null);
            if (_RenderTarget != null)
            {
                _RenderTarget.Dispose();
                _RenderTarget = null;
            }
            _RenderTarget = new RenderTarget2D(_GraphicsDeviceManager.GraphicsDevice, _Width, _Height, false, SurfaceFormat.Bgra32, DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.DiscardContents, true);
            
            _D3D11Image.SetBackBuffer(_RenderTarget);

        }

        private void StartRendering()
        {
            if (_Timer.IsRunning)
                return;

            System.Windows.Media.CompositionTarget.Rendering += OnRendering;
            _Timer.Start();
        }

        private void StopRendering()
        {
            if (!_Timer.IsRunning)
                return;

            System.Windows.Media.CompositionTarget.Rendering -= OnRendering;
            _Timer.Stop();
        }

        private void OnRendering(object sender, EventArgs eventArgs)
        {
            if (!_Timer.IsRunning)
                return;

            // Recreate back buffer if necessary.
            if (_ResetBackBuffer)
                CreateBackBuffer();

            // CompositionTarget.Rendering event may be raised multiple times per frame
            // (e.g. during window resizing).
            var renderingEventArgs = (System.Windows.Media.RenderingEventArgs)eventArgs;
            if (_LastRenderingTime != renderingEventArgs.RenderingTime || _ResetBackBuffer)
            {
                _LastRenderingTime = renderingEventArgs.RenderingTime;

                UpdateInternal(_Timer.Elapsed);
                _GraphicsDeviceManager.GraphicsDevice.SetRenderTarget(_RenderTarget);

                Draw(_Timer.Elapsed);
                _GraphicsDeviceManager.GraphicsDevice.Flush();
            }
            
            _D3D11Image.Invalidate(); // Always invalidate D3DImage to reduce flickering
            // during window resizing.

            _ResetBackBuffer = false;
        }

        /// <summary>
        /// Raises the <see cref="FrameworkElement.SizeChanged" /> event, using the specified 
        /// information as part of the eventual event data.
        /// </summary>
        /// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            _ResetBackBuffer = true;
            base.OnRenderSizeChanged(sizeInfo);
        }

        private void OnIsFrontBufferAvailableChanged(object sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            if (_D3D11Image.IsFrontBufferAvailable)
            {
                StartRendering();
                _ResetBackBuffer = true;
            }
            else
            {
                StopRendering();
            }
        }



        private void UpdateInternal(TimeSpan dt)
        {
            Update(dt);
        }

        public D3D11Image ImageSource
        {
            get
            {
                return _D3D11Image;
            }
        }

        public RenderTarget2D RenderTarget
        {
            get 
            {
                return _RenderTarget;
            }
        }

        public int getHeight()
        {
            return _Height;
        }

        #region Game members


        /// <summary>
        /// Gets the graphics device.
        /// </summary>
        /// <value>The graphics device.</value>
        public WpfGraphicsDeviceManager GraphicsDeviceManager
        {
            get 
            { 
                return _GraphicsDeviceManager; 
            }
        }

        /// <summary>
        /// Gets the content manager - use in <see cref="Load"/> to load your content and in <see cref="Unload"/> to unload your content!
        /// </summary>
        public ContentManager Content
        {
            get 
            { 
                return _ContentManager; 
            }
        }

        public GameServiceContainer Services
        {
            get
            {
                return _Services;
            }
        }

        // These methods mimick the XNA Game class
        public abstract void Initialize();

        public abstract void Load();

        public abstract void Unload();

        public abstract void Update(TimeSpan gameTime);

        public abstract void Draw(TimeSpan gameTime);

        #endregion
    }
}
