/**************************************************************************
 * 
 * ViewsPerContextType.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Common;
using GameSemantics.Data;
using GameSemantics.GameContent;
using Semantics.Abstractions;
using Semantics.Components;
using Semantics.Data;
using Semantics.Utilities;

namespace GameSemantics.Components
{

    #region Class: ViewsPerContextType
    /// <summary>
    /// A class that defines all views for a particular context type.
    /// </summary>
    public sealed class ViewsPerContextType : IdHolder
    {

        #region Properties and Fields

        #region Property: ContextType
        /// <summary>
        /// Gets the context type.
        /// </summary>
        public ContextType ContextType
        {
            get
            {
                return GameDatabase.Current.Select<ContextType>(this.ID, GameTables.ViewsPerContextType, GameColumns.ContextType);
            }
            internal set
            {
                GameDatabase.Current.Update(this.ID, GameTables.ViewsPerContextType, GameColumns.ContextType, value);
                NotifyPropertyChanged("ContextType");
            }
        }
        #endregion Property: ContextType

        #region Property: Views
        /// <summary>
        /// Gets all views.
        /// </summary>
        public ReadOnlyCollection<View> Views
        {
            get
            {
                List<View> views = new List<View>();
                foreach (ContentPerView contentPerView in this.ContentPerView)
                    views.Add(contentPerView.View);
                return views.AsReadOnly();
            }
        }
        #endregion Property: Views

        #region Property: ContentPerView
        /// <summary>
        /// Gets all content per view.
        /// </summary>
        public ReadOnlyCollection<ContentPerView> ContentPerView
        {
            get
            {
                return GameDatabase.Current.SelectAll<ContentPerView>(this.ID, GameTables.ViewsPerContextTypeContentPerView, GameColumns.ContentPerView).AsReadOnly();
            }
        }
        #endregion Property: ContentPerView

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: ViewsPerContextType()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static ViewsPerContextType()
        {
            // Context type
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(GameColumns.ContextType, new Tuple<Type, EntryType>(typeof(ContextType), EntryType.Intermediate));
            GameDatabase.Current.AddTableDefinition(GameTables.ViewsPerContextType, typeof(ViewsPerContextType), dict);

            // Content per view
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(GameColumns.ContentPerView, new Tuple<Type, EntryType>(typeof(ContentPerView), EntryType.Unique));
            GameDatabase.Current.AddTableDefinition(GameTables.ViewsPerContextTypeContentPerView, typeof(ViewsPerContextType), dict);
        }
        #endregion Static Constructor: ViewsPerContextType()

        #region Constructor: ViewsPerContextType()
        /// <summary>
        /// Creates a new views per context type.
        /// </summary>
        internal ViewsPerContextType()
            : base()
        {
        }
        #endregion Constructor: ViewsPerContextType()

        #region Constructor: ViewsPerContextType(uint id)
        /// <summary>
        /// Creates a new views per context type from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the views per context type from.</param>
        private ViewsPerContextType(uint id)
            : base(id)
        {
        }
        #endregion Constructor: ViewsPerContextType(uint id)

        #region Constructor: ViewsPerContextType(ViewsPerContextType viewsPerContextType)
        /// <summary>
        /// Clones a views per context type.
        /// </summary>
        /// <param name="contentPerView">The views per context type to clone.</param>
        internal ViewsPerContextType(ViewsPerContextType viewsPerContextType)
            : base()
        {
            if (viewsPerContextType != null)
            {
                GameDatabase.Current.StartChange();

                this.ContextType = viewsPerContextType.ContextType;

                foreach (ContentPerView contentPerView in viewsPerContextType.ContentPerView)
                {
                    AddView(contentPerView.View);
                    ContentPerView contentPerView2 = GetContentPerView(contentPerView.View);
                    if (contentPerView2 != null)
                    {
                        foreach (StaticContentValued staticContentValued in contentPerView.Content)
                            contentPerView2.AddContent(staticContentValued.Clone());
                    }
                }

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: ViewsPerContextType(ViewsPerContextType viewsPerContextType)

        #region Constructor: ViewsPerContextType(ContextType contextType)
        /// <summary>
        /// Creates a new views per context type for the given context type.
        /// </summary>
        /// <param name="contextType">The context type to create the views per context type for; can be null.</param>
        internal ViewsPerContextType(ContextType contextType)
            : base()
        {
            GameDatabase.Current.StartChange();

            this.ContextType = contextType;

            GameDatabase.Current.StopChange();
        }
        #endregion Constructor: ViewsPerContextType(ContextType contextType)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddView(View view)
        /// <summary>
        /// Adds a view.
        /// </summary>
        /// <param name="view">The view to add; can be null.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        internal Message AddView(View view)
        {
            // If the view is already available in all views, there is no use to add it
            if (HasView(view))
                return Message.RelationExistsAlready;

            // Create a new content per view and add it
            ContentPerView contentPerView = new ContentPerView(view);
            GameDatabase.Current.Insert(this.ID, GameTables.ViewsPerContextTypeContentPerView, GameColumns.ContentPerView, contentPerView);
            NotifyPropertyChanged("ContentPerView");
            NotifyPropertyChanged("Views");

            return Message.RelationSuccess;
        }
        #endregion Method: AddView(View view)

        #region Method: RemoveView(View view)
        /// <summary>
        /// Removes a view, and all its defined content.
        /// </summary>
        /// <param name="view">The view to remove; can be null.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        internal Message RemoveView(View view)
        {
            if (HasView(view))
            {
                // Get the corresponding content per view and remove it
                ContentPerView contentPerView = GetContentPerView(view);
                GameDatabase.Current.Remove(this.ID, GameTables.ViewsPerContextTypeContentPerView, GameColumns.ContentPerView, contentPerView);
                NotifyPropertyChanged("ContentPerView");
                NotifyPropertyChanged("Views");
                contentPerView.Remove();

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveView(View view)

        #region Method: AddContent(StaticContentValued staticContentValued, View view)
        /// <summary>
        /// Adds the given content to the given view.
        /// </summary>
        /// <param name="staticContentValued">The content to add to the view.</param>
        /// <param name="view">The view; can be null.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        internal Message AddContent(StaticContentValued staticContentValued, View view)
        {
            if (staticContentValued != null)
            {
                // Get the correct content per view, or create one if it does not yet exist
                ContentPerView contentPerView = GetContentPerView(view);
                if (contentPerView == null)
                {
                    AddView(view);
                    contentPerView = GetContentPerView(view);
                }

                // Add the content
                if (contentPerView != null)
                    return contentPerView.AddContent(staticContentValued);
            }
            return Message.RelationFail;
        }
        #endregion Method: AddContent(StaticContentValued staticContentValued, View view)

        #region Method: RemoveContent(StaticContentValued staticContentValued, View view)
        /// <summary>
        /// Remove the given content from the given view for the given context type.
        /// </summary>
        /// <param name="staticContentValued">The content to remove from the view.</param>
        /// <param name="view">The view of the context type; can be null.</param>
        /// <param name="contextType">The context type; can be null.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        internal Message RemoveContent(StaticContentValued staticContentValued, View view)
        {
            if (staticContentValued != null)
            {
                // Get the correct content per view
                ContentPerView contentPerView = GetContentPerView(view);

                // Remove the content
                if (contentPerView != null)
                    return contentPerView.RemoveContent(staticContentValued);
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveContent(StaticContentValued staticContentValued, View view)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasView(View view)
        /// <summary>
        /// Checks if this views per context type has the given view.
        /// </summary>
        /// <param name="view">The view to check; can be null.</param>
        /// <returns>Returns true when the views per context type has the view.</returns>
        internal bool HasView(View view)
        {
            foreach (ContentPerView contentPerView in this.ContentPerView)
            {
                if ((view == null && contentPerView.View == null) || (view != null && view.Equals(contentPerView.View)))
                    return true;
            }
            return false;
        }
        #endregion Method: HasView(View view)

        #region Method: HasContent(StaticContent staticContent, View view)
        /// <summary>
        /// Checks whether the given content has been defined for the given view.
        /// </summary>
        /// <param name="staticContent">The content to check.</param>
        /// <param name="view">The view; can be null.</param>
        /// <returns>Returns whether the content has been defined for the content.</returns>
        internal bool HasContent(StaticContent staticContent, View view)
        {
            if (staticContent != null)
            {
                foreach (ContentPerView contentPerView in this.ContentPerView)
                {
                    if ((view == null && contentPerView.View == null) || (view != null && view.Equals(contentPerView.View)))
                        return contentPerView.HasContent(staticContent);
                }
            }
            return false;
        }
        #endregion Method: HasContent(StaticContent staticContent, View view)

        #region Method: GetContentPerView(View view)
        /// <summary>
        /// Get the content per view for the given view.
        /// </summary>
        /// <param name="view">The view to get the content per view of.</param>
        /// <returns>The content per view of the view.</returns>
        internal ContentPerView GetContentPerView(View view)
        {
            foreach (ContentPerView contentPerView in this.ContentPerView)
            {
                if ((view == null && contentPerView.View == null) || (view != null && view.Equals(contentPerView.View)))
                    return contentPerView;
            }
            return null;
        }
        #endregion Method: GetContentPerView(View view)

        #region Method: GetContentOfView(View view)
        /// <summary>
        /// Get all the content that is defined for the given view.
        /// </summary>
        /// <param name="view">The view to get the content of.</param>
        /// <returns>Returns all the content that is defined for the view.</returns>
        internal ReadOnlyCollection<StaticContentValued> GetContentOfView(View view)
        {
            ContentPerView contentPerView = GetContentPerView(view);
            if (contentPerView != null)
                return contentPerView.Content;

            return new List<StaticContentValued>().AsReadOnly();
        }
        #endregion Method: GetContentOfView(View view)

        #region Method: GetAllContent()
        /// <summary>
        /// Get all the associated content, regardless of view.
        /// </summary>
        /// <returns>All the content that has been defined.</returns>
        public IEnumerable<ContentValued> GetAllContent()
        {
            foreach (ContentPerView contentPerView in this.ContentPerView)
            {
                foreach (StaticContentValued staticContentValued in contentPerView.Content)
                    yield return staticContentValued;
            }
        }
        #endregion Method: GetAllContent()

        #region Method: Remove()
        /// <summary>
        /// Remove the views per context type.
        /// </summary>
        public override void Remove()
        {
            GameDatabase.Current.StartChange();

            // Remove the content per view
            foreach (ContentPerView contentPerView in this.ContentPerView)
                contentPerView.Remove();
            GameDatabase.Current.Remove(this.ID, GameTables.ViewsPerContextTypeContentPerView);

            base.Remove();

            GameDatabase.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: ViewsPerContextType

}