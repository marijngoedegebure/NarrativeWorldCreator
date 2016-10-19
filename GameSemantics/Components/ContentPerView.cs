/**************************************************************************
 * 
 * ContentPerView.cs
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
using Semantics.Components;
using Semantics.Data;
using Semantics.Utilities;

namespace GameSemantics.Components
{

    #region Class: ContentPerView
    /// <summary>
    /// A class that defines all content for a particular view.
    /// </summary>
    public sealed class ContentPerView : IdHolder
    {

        #region Properties and Fields

        #region Property: View
        /// <summary>
        /// Gets the view.
        /// </summary>
        public View View
        {
            get
            {
                return GameDatabase.Current.Select<View>(this.ID, GameTables.ContentPerView, GameColumns.View);
            }
            internal set
            {
                GameDatabase.Current.Update(this.ID, GameTables.ContentPerView, GameColumns.View, value);
                NotifyPropertyChanged("View");
            }
        }
        #endregion Property: View

        #region Property: Content
        /// <summary>
        /// Gets all content.
        /// </summary>
        public ReadOnlyCollection<StaticContentValued> Content
        {
            get
            {
                return GameDatabase.Current.SelectAll<StaticContentValued>(this.ID, GameTables.ContentPerViewContentValued, GameColumns.ContentValued).AsReadOnly();
            }
        }
        #endregion Property: Content

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: ContentPerView()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static ContentPerView()
        {
            // View
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(GameColumns.View, new Tuple<Type, EntryType>(typeof(View), EntryType.Intermediate));
            GameDatabase.Current.AddTableDefinition(GameTables.ContentPerView, typeof(ContentPerView), dict);

            // Content
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(GameColumns.ContentValued, new Tuple<Type, EntryType>(typeof(StaticContentValued), EntryType.Unique));
            GameDatabase.Current.AddTableDefinition(GameTables.ContentPerViewContentValued, typeof(ContentPerView), dict);
        }
        #endregion Static Constructor: ContentPerView()

        #region Constructor: ContentPerView()
        /// <summary>
        /// Creates a new content per view.
        /// </summary>
        internal ContentPerView()
            : base()
        {
        }
        #endregion Constructor: ContentPerView()

        #region Constructor: ContentPerView(uint id)
        /// <summary>
        /// Creates a new content per view from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the content per view from.</param>
        private ContentPerView(uint id)
            : base(id)
        {
        }
        #endregion Constructor: ContentPerView(uint id)

        #region Constructor: ContentPerView(ContentPerView contentPerView)
        /// <summary>
        /// Clones a content per view.
        /// </summary>
        /// <param name="contentPerView">The content per view to clone.</param>
        internal ContentPerView(ContentPerView contentPerView)
            : base()
        {
            if (contentPerView != null)
            {
                GameDatabase.Current.StartChange();

                this.View = contentPerView.View;

                foreach (StaticContentValued staticContentValued in contentPerView.Content)
                    AddContent(staticContentValued.Clone());

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: ContentPerView(ContentPerView contentPerView)

        #region Constructor: ContentPerView(View view)
        /// <summary>
        /// Creates a new content per view for the given view.
        /// </summary>
        /// <param name="view">The view to create the content per view for; can be null.</param>
        internal ContentPerView(View view)
            : base()
        {
            GameDatabase.Current.StartChange();

            this.View = view;

            GameDatabase.Current.StopChange();
        }
        #endregion Constructor: ContentPerView(View view)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddContent(StaticContentValued staticContentValued)
        /// <summary>
        /// Adds valued content.
        /// </summary>
        /// <param name="staticContentValued">The valued content to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        internal Message AddContent(StaticContentValued staticContentValued)
        {
            if (staticContentValued != null && staticContentValued.Content != null)
            {
                // If the valued content is already available in all content, there is no use to add it
                if (HasContent(staticContentValued.StaticContent))
                    return Message.RelationExistsAlready;

                // Add the valued content
                GameDatabase.Current.Insert(this.ID, GameTables.ContentPerViewContentValued, GameColumns.ContentValued, staticContentValued);
                NotifyPropertyChanged("Content");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddContent(StaticContentValued staticContentValued)

        #region Method: RemoveContent(StaticContentValued staticContentValued)
        /// <summary>
        /// Removes valued content.
        /// </summary>
        /// <param name="staticContentValued">The valued content to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        internal Message RemoveContent(StaticContentValued staticContentValued)
        {
            if (staticContentValued != null)
            {
                if (HasContent(staticContentValued.StaticContent))
                {
                    // Remove the valued content
                    GameDatabase.Current.Remove(this.ID, GameTables.ContentPerViewContentValued, GameColumns.ContentValued, staticContentValued);
                    NotifyPropertyChanged("Content");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveContent(StaticContentValued staticContentValued)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasContent(StaticContent staticContent)
        /// <summary>
        /// Checks if this content per view has the given content.
        /// </summary>
        /// <param name="staticContent">The content to check.</param>
        /// <returns>Returns true when the content per view has the content.</returns>
        internal bool HasContent(StaticContent staticContent)
        {
            if (staticContent != null)
            {
                foreach (StaticContentValued contentValued in this.Content)
                {
                    if (staticContent.Equals(contentValued.Content))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasContent(StaticContent staticContent)

        #region Method: Remove()
        /// <summary>
        /// Remove the content per view.
        /// </summary>
        public override void Remove()
        {
            GameDatabase.Current.StartChange();

            // Remove the valued content
            foreach (StaticContentValued staticContentValued in this.Content)
                staticContentValued.Remove();
            GameDatabase.Current.Remove(this.ID, GameTables.ContentPerViewContentValued);

            base.Remove();

            GameDatabase.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: ContentPerView

}