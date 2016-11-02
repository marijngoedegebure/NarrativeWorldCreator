/**************************************************************************
 * 
 * ContentPerViewBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using GameSemantics.Components;
using GameSemantics.GameContent;
using GameSemanticsEngine.GameContent;
using GameSemanticsEngine.Tools;
using SemanticsEngine.Components;

namespace GameSemanticsEngine.Components
{

    #region Class: ContentPerViewBase
    /// <summary>
    /// A base of a content per view.
    /// </summary>
    public sealed class ContentPerViewBase : Base
    {

        #region Properties and Fields

        #region Property: ContentPerView
        /// <summary>
        /// Gets the content per view of which this is a content per view base.
        /// </summary>
        internal ContentPerView ContentPerView
        {
            get
            {
                return this.IdHolder as ContentPerView;
            }
        }
        #endregion Property: ContentPerView

        #region Property: View
        /// <summary>
        /// The view.
        /// </summary>
        private ViewBase view = null;
        
        /// <summary>
        /// Gets the view.
        /// </summary>
        public ViewBase View
        {
            get
            {
                return view;
            }
        }
        #endregion Property: View

        #region Property: Content
        /// <summary>
        /// The content.
        /// </summary>
        private StaticContentValuedBase[] content = null;

        /// <summary>
        /// Gets the content.
        /// </summary>
        public ReadOnlyCollection<StaticContentValuedBase> Content
        {
            get
            {
                if (content == null)
                {
                    LoadContent();
                    if (content == null)
                        return new List<StaticContentValuedBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<StaticContentValuedBase>(content);
            }
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        private void LoadContent()
        {
            if (this.ContentPerView != null)
            {
                List<StaticContentValuedBase> contentValuedBases = new List<StaticContentValuedBase>();
                foreach (StaticContentValued staticContentValued in this.ContentPerView.Content)
                    contentValuedBases.Add(GameBaseManager.Current.GetBase<StaticContentValuedBase>(staticContentValued));
                content = contentValuedBases.ToArray();
            }
        }
        #endregion Property: Content

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ContentPerViewBase(ContentPerView contentPerView)
        /// <summary>
        /// Creates a new content per view base from the given content per view.
        /// </summary>
        /// <param name="contentPerView">The content per view to create a content per view base from.</param>
        internal ContentPerViewBase(ContentPerView contentPerView)
            : base(contentPerView)
        {
            if (contentPerView != null)
            {
                this.view = GameBaseManager.Current.GetBase<ViewBase>(contentPerView.View);

                if (GameBaseManager.PreloadProperties)
                    LoadContent();
            }
        }
        #endregion Constructor: ContentPerViewBase(ContentPerView contentPerView)

        #endregion Method Group: Constructors

    }
    #endregion Class: ContentPerViewBase

}