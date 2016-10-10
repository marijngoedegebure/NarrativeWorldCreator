using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NarrativeWorldCreator.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorldCreator.Graphics
{
    public class WpfGraphicsDeviceManager : IGraphicsDeviceService, IGraphicsDeviceManager, IDisposable
    {

        public event EventHandler<EventArgs> DeviceCreated;
        public event EventHandler<EventArgs> DeviceDisposing;
        public event EventHandler<EventArgs> DeviceReset;
        public event EventHandler<EventArgs> DeviceResetting;
       
        private readonly D3D11Host _Game;
        private readonly int _Width;
        private readonly int _Height;

        private SurfaceFormat _PreferredBackBufferFormat;
        private DepthFormat _PreferredDepthStencilFormat;
        private GraphicsDevice _GraphicsDevice;
        private int _PreferredBackBufferHeight;
        private int _PreferredBackBufferWidth;
        private bool _Disposed;
        private bool _DrawBegun;

        public WpfGraphicsDeviceManager(D3D11Host game, int width, int height)
        {
            _Game = game;
            _Game.Services.AddService(typeof(IGraphicsDeviceManager), this);
            _Game.Services.AddService(typeof(IGraphicsDeviceService), this);

            _Width = width;
            _Height = height;
        }

        ~WpfGraphicsDeviceManager()
        {
            this.Dispose(false);
        }
        
        public void CreateDevice()
        {
            this.Initialize();
            this.RaiseDeviceCreated(this, EventArgs.Empty);
        }

        //public void ApplyChanges()
        //{
        //    if (this._GraphicsDevice == null)
        //    {
        //        return;
        //    }

        //    PresentationParameters parameters = new PresentationParameters() 
        //    {
        //       BackBufferWidth = this._PreferredBackBufferWidth,
        //       BackBufferHeight = this._PreferredBackBufferHeight,
        //       BackBufferFormat = this._PreferredBackBufferFormat,
        //       DepthStencilFormat = this._PreferredDepthStencilFormat,
        //       IsFullScreen = false
        //    };

        //    this._GraphicsDevice.Reset(parameters);
        //}

        private void Initialize() 
        {
            var presentationParameters = new PresentationParameters
            {
                // Do not associate graphics device with window.
                DeviceWindowHandle = IntPtr.Zero,
                BackBufferHeight = _Height,
                BackBufferWidth = _Width,
                DisplayOrientation = DisplayOrientation.Default
            };

            _GraphicsDevice = new GraphicsDevice(GraphicsAdapter.DefaultAdapter, GraphicsProfile.HiDef, presentationParameters);
            
            _GraphicsDevice.DeviceReset += RaiseDeviceReset;
            _GraphicsDevice.DeviceResetting += RaiseDeviceResetting;
        }

        public bool BeginDraw()
        {
            if (_GraphicsDevice == null)
            {
                return false;
            }

            _DrawBegun = true;
            return true;
        }

        public void EndDraw()
        {
            if (_GraphicsDevice != null && _DrawBegun)
            {
                _DrawBegun = false;
                _GraphicsDevice.Present();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._Disposed)
            {
                if (disposing && _GraphicsDevice != null)
                {
                    _GraphicsDevice.DeviceReset -= RaiseDeviceReset;
                    _GraphicsDevice.DeviceResetting -= RaiseDeviceResetting;
                    _GraphicsDevice.Dispose();
                }
                    

                _Disposed = true;
            }
        }

        public GraphicsDevice GraphicsDevice
        {
            get
            {
                return _GraphicsDevice;
            }
        }

        public DepthFormat PreferredDepthStencilFormat
        {
            get
            {
                return this._PreferredDepthStencilFormat;
            }
            set
            {
                this._PreferredDepthStencilFormat = value;
            }
        }

        public SurfaceFormat PreferredBackBufferFormat
        {
            get
            {
                return this._PreferredBackBufferFormat;
            }
            set
            {
                this._PreferredBackBufferFormat = value;
            }
        }

        public int PreferredBackBufferWidth
        {
            get
            {
                return this._PreferredBackBufferWidth;
            }
            set
            {
                this._PreferredBackBufferWidth = value;
            }
        }
        
        public int PreferredBackBufferHeight
        {
            get
            {
                return this._PreferredBackBufferHeight;
            }
            set
            {
                this._PreferredBackBufferHeight = value;
            }
        }

        private void RaiseDeviceCreated(object sender, EventArgs e)
        {
            EventHandler<EventArgs> handler = DeviceCreated;
            if (handler != null)
                handler(sender, e);
        }

        private void RaiseDeviceDisposing(object sender, EventArgs e)
        {
            EventHandler<EventArgs> handler = DeviceDisposing;
            if (handler != null)
                handler(sender, e);
        }

        private void RaiseDeviceReset(object sender, EventArgs e)
        {
            EventHandler<EventArgs> handler = DeviceReset;
            if (handler != null)
                handler(sender, e);
        }

        private void RaiseDeviceResetting(object sender, EventArgs e)
        {
            EventHandler<EventArgs> handler = DeviceResetting;
            if (handler != null)
                handler(sender, e);
        }

    }
}
