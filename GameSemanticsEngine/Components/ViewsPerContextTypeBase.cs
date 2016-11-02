/**************************************************************************
 * 
 * ViewsPerContextTypeBase.cs
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
using GameSemanticsEngine.GameContent;
using GameSemanticsEngine.Tools;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Components;

namespace GameSemanticsEngine.Components
{

    #region Class: ViewsPerContextTypeBase
    /// <summary>
    /// A base of a views per context type.
    /// </summary>
    public sealed class ViewsPerContextTypeBase : Base
    {

        #region Properties and Fields

        #region Property: ViewsPerContextType
        /// <summary>
        /// Gets the views per context type of which this is a views per context type base.
        /// </summary>
        internal ViewsPerContextType ViewsPerContextType
        {
            get
            {
                return this.IdHolder as ViewsPerContextType;
            }
        }
        #endregion Property: ViewsPerContextType

        #region Property: ContextType
        /// <summary>
        /// Gets the context type.
        /// </summary>
        private ContextTypeBase contextType = null;

        /// <summary>
        /// Gets the context type.
        /// </summary>
        public ContextTypeBase ContextType
        {
            get
            {
                return contextType;
            }
        }
        #endregion Property: ContextType

        #region Property: Views
        /// <summary>
        /// Gets all views.
        /// </summary>
        public IEnumerable<ViewBase> Views
        {
            get
            {
                foreach (ContentPerViewBase contentPerViewBase in this.ContentPerView)
                    yield return contentPerViewBase.View;
            }
        }
        #endregion Property: Views

        #region Property: ContentPerView
        /// <summary>
        /// The content per view.
        /// </summary>
        private ContentPerViewBase[] contentPerView = null;

        /// <summary>
        /// Gets the content per view.
        /// </summary>
        public ReadOnlyCollection<ContentPerViewBase> ContentPerView
        {
            get
            {
                if (contentPerView == null)
                {
                    LoadContentPerView();
                    if (contentPerView == null)
                        return new List<ContentPerViewBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<ContentPerViewBase>(contentPerView);
            }
        }

        /// <summary>
        /// Loads the content per view.
        /// </summary>
        private void LoadContentPerView()
        {
            if (this.ViewsPerContextType != null)
            {
                List<ContentPerViewBase> contentPerViewBases = new List<ContentPerViewBase>();
                foreach (ContentPerView contentPerView2 in this.ViewsPerContextType.ContentPerView)
                    contentPerViewBases.Add(GameBaseManager.Current.GetBase<ContentPerViewBase>(contentPerView2));
                contentPerView = contentPerViewBases.ToArray();
            }
        }
        #endregion Property: ContentPerView

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ViewsPerContextTypeBase(ViewsPerContextType viewsPerContextType)
        /// <summary>
        /// Creates a new views per context type base from the given views per context type.
        /// </summary>
        /// <param name="viewsPerContextType">The views per context type to create a views per context type base from.</param>
        internal ViewsPerContextTypeBase(ViewsPerContextType viewsPerContextType)
            : base(viewsPerContextType)
        {
            if (viewsPerContextType != null)
            {
                this.contextType = GameBaseManager.Current.GetBase<ContextTypeBase>(viewsPerContextType.ContextType);

                if (GameBaseManager.PreloadProperties)
                    LoadContentPerView();
            }
        }
        #endregion Constructor: ViewsPerContextTypeBase(ViewsPerContextType viewsPerContextType)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetContentPerView(ViewBase viewBase)
        /// <summary>
        /// Get the content per view for the given view.
        /// </summary>
        /// <param name="viewBase">The view to get the content per view of; can be null.</param>
        /// <returns>The content per view of the view.</returns>
        internal ContentPerViewBase GetContentPerView(ViewBase viewBase)
        {
            foreach (ContentPerViewBase contentPerView in this.ContentPerView)
            {
                if ((viewBase == null && contentPerView.View == null) || (viewBase != null && viewBase.Equals(contentPerView.View)))
                    return contentPerView;
            }
            return null;
        }
        #endregion Method: GetContentPerView(ViewBase viewBase)

        #region Method: GetContentOfView(ViewBase viewBase)
        /// <summary>
        /// Get all the content that is defined for the given view.
        /// </summary>
        /// <param name="viewBase">The view to get the content of; can be null.</param>
        /// <returns>Returns all the content that is defined for the view.</returns>
        internal IEnumerable<StaticContentValuedBase> GetContentOfView(ViewBase viewBase)
        {
            ContentPerViewBase contentPerView = GetContentPerView(viewBase);
            if (contentPerView != null)
            {
                foreach (StaticContentValuedBase staticContentValued in contentPerView.Content)
                    yield return staticContentValued;
            }
        }
        #endregion Method: GetContentOfView(ViewBase viewBase)

        #region Method: GetAllContent()
        /// <summary>
        /// Get all the associated content, regardless of view.
        /// </summary>
        /// <returns>All the content that has been defined.</returns>
        public IEnumerable<StaticContentValuedBase> GetAllContent()
        {
            foreach (ContentPerViewBase contentPerView in this.ContentPerView)
            {
                foreach (StaticContentValuedBase staticContentValued in contentPerView.Content)
                    yield return staticContentValued;
            }
        }
        #endregion Method: GetAllContent()

        #endregion Method Group: Other

    }
    #endregion Class: ViewsPerContextTypeBase

}