/**************************************************************************
 * 
 * AbstractGameNode.cs
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

    #region Class: AbstractGameNode
    /// <summary>
    /// An abstract game node.
    /// </summary>
    public abstract class AbstractGameNode : Node
    {

        #region Properties and Fields

        #region Property: ViewsPerContextType
        /// <summary>
        /// Gets all the views per context type.
        /// </summary>
        public ReadOnlyCollection<ViewsPerContextType> ViewsPerContextType
        {
            get
            {
                return GameDatabase.Current.SelectAll<ViewsPerContextType>(this.ID, GameTables.AbstractGameNodeViewsPerContextType, GameColumns.ViewsPerContextType).AsReadOnly();
            }
        }
        #endregion Property: ViewsPerContextType

        #region Property: ContextTypes
        /// <summary>
        /// Gets all context types.
        /// </summary>
        public ReadOnlyCollection<ContextType> ContextTypes
        {
            get
            {
                List<ContextType> contextTypes = new List<ContextType>();
                foreach (ViewsPerContextType viewsPerContextType in this.ViewsPerContextType)
                    contextTypes.Add(viewsPerContextType.ContextType);
                return contextTypes.AsReadOnly();
            }
        }
        #endregion Property: ContextTypes

        #region Property: DynamicContent
        /// <summary>
        /// Gets all the dynamic content, used to be linked to the EventContent.
        /// </summary>
        public ReadOnlyCollection<DynamicContentValued> DynamicContent
        {
            get
            {
                return GameDatabase.Current.SelectAll<DynamicContentValued>(this.ID, GameTables.AbstractGameNodeDynamicContent, GameColumns.DynamicContentValued).AsReadOnly();
            }
        }
        #endregion Property: DynamicContent

        #region Property: EventContent
        /// <summary>
        /// Gets the content that should be started or stopped when an event starts/is being executed/stops.
        /// </summary>
        public ReadOnlyCollection<EventContent> EventContent
        {
            get
            {
                return GameDatabase.Current.SelectAll<EventContent>(this.ID, GameTables.AbstractGameNodeEventContent, GameColumns.EventContent).AsReadOnly();
            }
        }
        #endregion Property: EventContent

        #region Property: EventExtensions
        /// <summary>
        /// Gets the event extensions.
        /// </summary>
        public ReadOnlyCollection<EventExtension> EventExtensions
        {
            get
            {
                return GameDatabase.Current.SelectAll<EventExtension>(this.ID, GameTables.AbstractGameNodeEventExtension, GameColumns.EventExtension).AsReadOnly();
            }
        }
        #endregion Property: EventExtensions

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: AbstractGameNode()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static AbstractGameNode()
        {
            // Views per context type
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(GameColumns.ViewsPerContextType, new Tuple<Type, EntryType>(typeof(ViewsPerContextType), EntryType.Unique));
            GameDatabase.Current.AddTableDefinition(GameTables.AbstractGameNodeViewsPerContextType, typeof(AbstractGameNode), dict);
        }
        #endregion Static Constructor: AbstractGameNode()

        #region Constructor: AbstractGameNode()
        /// <summary>
        /// Creates a new abstract game node.
        /// </summary>
        protected AbstractGameNode()
            : base()
        {
        }
        #endregion Constructor: AbstractGameNode()

        #region Constructor: AbstractGameNode(uint id)
        /// <summary>
        /// Creates a new abstract game node from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the abstract game node from.</param>
        protected AbstractGameNode(uint id)
            : base(id)
        {
        }
        #endregion Constructor: AbstractGameNode(uint id)

        #region Constructor: AbstractGameNode(string name)
        /// <summary>
        /// Creates a new abstract game node with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the abstract game node.</param>
        protected AbstractGameNode(string name)
            : base(name)
        {
        }
        #endregion Constructor: AbstractGameNode(string name)

        #region Constructor: AbstractGameNode(AbstractGameNode abstractGameNode)
        /// <summary>
        /// Clones an abstract game node.
        /// </summary>
        /// <param name="gameNode">The abstract game node to clone.</param>
        protected AbstractGameNode(AbstractGameNode abstractGameNode)
            : base(abstractGameNode)
        {
            if (abstractGameNode != null)
            {
                GameDatabase.Current.StartChange();

                foreach (ViewsPerContextType viewsPerContextType in abstractGameNode.ViewsPerContextType)
                {
                    AddContextType(viewsPerContextType.ContextType);
                    ViewsPerContextType viewsPerContextType2 = GetViewsPerContextType(viewsPerContextType.ContextType);
                    foreach (ContentPerView contentPerView in viewsPerContextType.ContentPerView)
                    {
                        viewsPerContextType2.AddView(contentPerView.View);
                        ContentPerView contentPerView2 = viewsPerContextType2.GetContentPerView(contentPerView.View);
                        foreach (StaticContentValued staticContentValued in contentPerView.Content)
                            contentPerView2.AddContent(staticContentValued.Clone());
                    }
                }

                foreach (EventContent eventContent in this.EventContent)
                    AddEventContent(new EventContent(eventContent));

                foreach (EventExtension eventExtension in this.EventExtensions)
                    AddEventExtension(new EventExtension(eventExtension));

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: AbstractGameNode(AbstractGameNode abstractGameNode)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddContextType(ContextType contextType)
        /// <summary>
        /// Adds the given context type.
        /// </summary>
        /// <param name="contextType">The context type to add; can be null.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddContextType(ContextType contextType)
        {
            if (HasContextType(contextType))
                return Message.RelationExistsAlready;

            // Create a new views per context type and add it
            ViewsPerContextType viewsPerContextType = new ViewsPerContextType(contextType);
            GameDatabase.Current.Insert(this.ID, GameTables.AbstractGameNodeViewsPerContextType, GameColumns.ViewsPerContextType, viewsPerContextType);
            NotifyPropertyChanged("ViewsPerContextType");
            NotifyPropertyChanged("ContextTypes");

            return Message.RelationSuccess;
        }
        #endregion Method: AddContextType(ContextType contextType)

        #region Method: RemoveContextType(ContextType contextType)
        /// <summary>
        /// Removes the given context type, and all its defined views and content.
        /// </summary>
        /// <param name="contextType">The context type to remove; can be null.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveContextType(ContextType contextType)
        {
            if (HasContextType(contextType))
            {
                // Get the corresponding views per context type and remove it
                ViewsPerContextType viewsPerContextType = GetViewsPerContextType(contextType);
                GameDatabase.Current.Remove(this.ID, GameTables.AbstractGameNodeViewsPerContextType, GameColumns.ViewsPerContextType, viewsPerContextType);
                NotifyPropertyChanged("ViewsPerContextType");
                NotifyPropertyChanged("ContextTypes");
                viewsPerContextType.Remove();

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveContextType(ContextType contextType)

        #region Method: AddView(View view, ContextType contextType)
        /// <summary>
        /// Adds the given view for the given context type.
        /// </summary>
        /// <param name="view">The view to add to the context type; can be null.</param>
        /// <param name="contextType">The context type to add the view for; can be null.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddView(View view, ContextType contextType)
        {
            // Get the correct views per context type, or create one if it does not yet exist
            ViewsPerContextType viewsPerContextType = GetViewsPerContextType(contextType);
            if (viewsPerContextType == null)
            {
                AddContextType(contextType);
                viewsPerContextType = GetViewsPerContextType(contextType);
            }

            // Add the view
            if (viewsPerContextType != null)
                return viewsPerContextType.AddView(view);

            return Message.RelationFail;
        }
        #endregion Method: AddView(View view, ContextType contextType)

        #region Method: RemoveView(View view, ContextType contextType)
        /// <summary>
        /// Removes the given view for the given context type.
        /// </summary>
        /// <param name="view">The view to remove from the context type; can be null.</param>
        /// <param name="contextType">The context type to remove the view of; can be null.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveView(View view, ContextType contextType)
        {
            // Get the correct views per context type
            ViewsPerContextType viewsPerContextType = GetViewsPerContextType(contextType);

            // Remove the view
            if (viewsPerContextType != null)
                return viewsPerContextType.RemoveView(view);

            return Message.RelationFail;
        }
        #endregion Method: RemoveView(View view, ContextType contextType)

        #region Method: AddContent(StaticContentValued staticContentValued)
        /// <summary>
        /// Adds the given content to the default view for the default context type.
        /// </summary>
        /// <param name="staticContentValued">The content to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddContent(StaticContentValued staticContentValued)
        {
            return AddContent(staticContentValued, null, null);
        }
        #endregion Method: AddContent(StaticContentValued staticContentValued)

        #region Method: RemoveContent(StaticContentValued staticContentValued)
        /// <summary>
        /// Remove the given content from the default view and context type.
        /// </summary>
        /// <param name="staticContentValued">The content to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveContent(StaticContentValued staticContentValued)
        {
            return RemoveContent(staticContentValued, null, null);
        }
        #endregion Method: RemoveContent(StaticContentValued staticContentValued)

        #region Method: AddContent(StaticContentValued staticContentValued, View view, ContextType contextType)
        /// <summary>
        /// Adds the given content to the given view for the given context type.
        /// </summary>
        /// <param name="staticContentValued">The content to add to the view.</param>
        /// <param name="view">The view of the context type; can be null.</param>
        /// <param name="contextType">The context type; can be null.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddContent(StaticContentValued staticContentValued, View view, ContextType contextType)
        {
            if (staticContentValued != null)
            {
                // Get the correct views per context type, or create one if it does not yet exist
                ViewsPerContextType viewsPerContextType = GetViewsPerContextType(contextType);
                if (viewsPerContextType == null)
                {
                    AddContextType(contextType);
                    viewsPerContextType = GetViewsPerContextType(contextType);
                }

                // Get the correct content per view
                if (viewsPerContextType != null)
                    return viewsPerContextType.AddContent(staticContentValued, view);
            }
            return Message.RelationFail;
        }
        #endregion Method: AddContent(StaticContentValued staticContentValued, View view, ContextType contextType)

        #region Method: RemoveContent(StaticContentValued staticContentValued, View view, ContextType contextType)
        /// <summary>
        /// Remove the given content from the given view for the given context type.
        /// </summary>
        /// <param name="staticContentValued">The content to remove from the view.</param>
        /// <param name="view">The view of the context type; can be null.</param>
        /// <param name="contextType">The context type; can be null.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveContent(StaticContentValued staticContentValued, View view, ContextType contextType)
        {
            if (staticContentValued != null)
            {
                // Get the correct views per context type
                ViewsPerContextType viewsPerContextType = GetViewsPerContextType(contextType);

                if (viewsPerContextType != null)
                    return viewsPerContextType.RemoveContent(staticContentValued, view);
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveContent(StaticContentValued staticContentValued, View view, ContextType contextType)

        #region Method: AddContent(DynamicContentValued dynamicContentValued)
        /// <summary>
        /// Adds the given content.
        /// </summary>
        /// <param name="dynamicContentValued">The content to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddContent(DynamicContentValued dynamicContentValued)
        {
            if (HasContent(dynamicContentValued.DynamicContent))
                return Message.RelationExistsAlready;

            GameDatabase.Current.Insert(this.ID, GameTables.AbstractGameNodeDynamicContent, GameColumns.DynamicContentValued, dynamicContentValued);
            NotifyPropertyChanged("DynamicContent");

            return Message.RelationSuccess;
        }
        #endregion Method: AddContent(DynamicContentValued dynamicContentValued)

        #region Method: RemoveContent(DynamicContentValued dynamicContentValued)
        /// <summary>
        /// Removes the given content.
        /// </summary>
        /// <param name="contextType">The content to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveContent(DynamicContentValued dynamicContentValued)
        {
            if (HasContent(dynamicContentValued.DynamicContent))
            {
                GameDatabase.Current.Remove(this.ID, GameTables.AbstractGameNodeDynamicContent, GameColumns.DynamicContentValued, dynamicContentValued);
                NotifyPropertyChanged("DynamicContent");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveContent(DynamicContentValued dynamicContentValued)

        #region Method: AddEventContent(EventContent eventContent)
        /// <summary>
        /// Adds event content.
        /// </summary>
        /// <param name="eventContent">The event content to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddEventContent(EventContent eventContent)
        {
            if (eventContent != null)
            {
                // If the event content is already available, there is no use to add it
                if (this.EventContent.Contains(eventContent))
                    return Message.RelationExistsAlready;

                // Add the event content
                GameDatabase.Current.Insert(this.ID, GameTables.AbstractGameNodeEventContent, GameColumns.EventContent, eventContent);
                NotifyPropertyChanged("EventContent");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddEventContent(EventContent eventContent)

        #region Method: RemoveEventContent(EventContent eventContent)
        /// <summary>
        /// Removes event content.
        /// </summary>
        /// <param name="eventContent">The event content to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveEventContent(EventContent eventContent)
        {
            if (eventContent != null)
            {
                if (this.EventContent.Contains(eventContent))
                {
                    // Remove the event content
                    GameDatabase.Current.Remove(this.ID, GameTables.AbstractGameNodeEventContent, GameColumns.EventContent, eventContent);
                    NotifyPropertyChanged("EventContent");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveEventContent(EventContent eventContent)

        #region Method: AddEventExtension(EventExtension eventExtension)
        /// <summary>
        /// Adds an event extension.
        /// </summary>
        /// <param name="eventExtension">The event extension to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddEventExtension(EventExtension eventExtension)
        {
            if (eventExtension != null)
            {
                // If the event extension is already available, there is no use to add it
                if (HasEventExtension(eventExtension.Event))
                    return Message.RelationExistsAlready;

                // Add the event extension
                GameDatabase.Current.Insert(this.ID, GameTables.AbstractGameNodeEventExtension, GameColumns.EventExtension, eventExtension);
                NotifyPropertyChanged("EventExtensions");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddEventExtension(EventExtension eventExtension)

        #region Method: RemoveEventExtension(EventExtension eventExtension)
        /// <summary>
        /// Removes an event extension.
        /// </summary>
        /// <param name="eventExtension">The event extension to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveEventExtension(EventExtension eventExtension)
        {
            if (eventExtension != null)
            {
                if (this.EventExtensions.Contains(eventExtension))
                {
                    // Remove the event extension
                    GameDatabase.Current.Remove(this.ID, GameTables.AbstractGameNodeEventExtension, GameColumns.EventExtension, eventExtension);
                    NotifyPropertyChanged("EventExtensions");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveEventExtension(EventExtension eventExtension)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasContextType(ContextType contextType)
        /// <summary>
        /// Returns whether the abstract game node has the given context type.
        /// </summary>
        /// <param name="contextType">The context type to check; can be null.</param>
        /// <returns>Returns whether the abstract game node has the context type.</returns>
        public bool HasContextType(ContextType contextType)
        {
            foreach (ViewsPerContextType viewsPerContextType in this.ViewsPerContextType)
            {
                if ((contextType == null && viewsPerContextType.ContextType == null) || (contextType != null && contextType.Equals(viewsPerContextType.ContextType)))
                    return true;
            }
            return false;
        }
        #endregion Method: HasContextType(ContextType contextType)

        #region Method: HasView(View view, ContextType contextType)
        /// <summary>
        /// Checks whether the given view has been defined for the given context type.
        /// </summary>
        /// <param name="view">The view to check; can be null.</param>
        /// <param name="contextType">The context type; can be null.</param>
        /// <returns>Returns whether the view has been defined for the context type.</returns>
        public bool HasView(View view, ContextType contextType)
        {
            foreach (ViewsPerContextType viewsPerContextType in this.ViewsPerContextType)
            {
                if ((contextType == null && viewsPerContextType.ContextType == null) || (contextType != null && contextType.Equals(viewsPerContextType.ContextType)))
                    return viewsPerContextType.HasView(view);
            }
            return false;
        }
        #endregion Method: HasView(View view, ContextType contextType)

        #region Method: HasContent(StaticContent staticContent, View view, ContextType contextType)
        /// <summary>
        /// Checks whether the given content has been assigned to the given view for the given context type.
        /// </summary>
        /// <param name="staticContent">The content to check.</param>
        /// <param name="view">The view of the context type; can be null.</param>
        /// <param name="contextType">The context type; can be null.</param>
        /// <returns>Returns whether the content has been assigned to the view for the context type.</returns>
        public bool HasContent(StaticContent staticContent, View view, ContextType contextType)
        {
            foreach (ViewsPerContextType viewsPerContextType in this.ViewsPerContextType)
            {
                if ((contextType == null && viewsPerContextType.ContextType == null) || (contextType != null && contextType.Equals(viewsPerContextType.ContextType)))
                    return viewsPerContextType.HasContent(staticContent, view);
            }
            return false;
        }
        #endregion Method: HasContent(StaticContent staticContent, View view, ContextType contextType)

        #region Method: HasContent(DynamicContent dynamicContent)
        /// <summary>
        /// Checks if this abstract game node has the given content.
        /// </summary>
        /// <param name="dynamicContent">The content to check.</param>
        /// <returns>Returns true when the entity has the attribute.</returns>
        public bool HasContent(DynamicContent dynamicContent)
        {
            if (dynamicContent != null)
            {
                foreach (DynamicContentValued dynamicContentValued in this.DynamicContent)
                {
                    if (dynamicContent.Equals(dynamicContentValued.DynamicContent))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasContent(DynamicContent dynamicContent)

        #region Method: HasEventExtension(Event even)
        /// <summary>
        /// Checks if this abstract game node has an event extension for the given event.
        /// </summary>
        /// <param name="even">The event to check.</param>
        /// <returns>Returns true when the abstract game node has an event extension for the event.</returns>
        public bool HasEventExtension(Event even)
        {
            if (even != null)
            {
                foreach (EventExtension eventExtension in this.EventExtensions)
                {
                    if (even.Equals(eventExtension.Event))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasEventExtension(Event even)

        #region Method: GetViewsPerContextType(ContextType contextType)
        /// <summary>
        /// Get the views per context type for the given context type.
        /// </summary>
        /// <param name="contextType">The context type to get the views per context type of.</param>
        /// <returns>The views per context type of the context type.</returns>
        internal ViewsPerContextType GetViewsPerContextType(ContextType contextType)
        {
            foreach (ViewsPerContextType viewsPerContextType in this.ViewsPerContextType)
            {
                if ((contextType == null && viewsPerContextType.ContextType == null) || (contextType != null && contextType.Equals(viewsPerContextType.ContextType)))
                    return viewsPerContextType;
            }
            return null;
        }
        #endregion Method: GetViewsPerContextType(ContextType contextType)

        #region Method: GetViewsOfContextType(ContextType contextType)
        /// <summary>
        /// Get the views that are defined for the given context type.
        /// </summary>
        /// <param name="contextType">The context type to get the views of.</param>
        /// <returns>Returns the views that are defined for the context type.</returns>
        public ReadOnlyCollection<View> GetViewsOfContextType(ContextType contextType)
        {
            ViewsPerContextType viewsPerContextType = GetViewsPerContextType(contextType);
            if (viewsPerContextType != null)
                return viewsPerContextType.Views;

            return new List<View>().AsReadOnly();
        }
        #endregion Method: GetViewsOfContextType(ContextType contextType)

        #region Method: GetContentOfViewOfContextType(View view, ContextType contextType)
        /// <summary>
        /// Get all the content that has been defined for the given view of the given context type.
        /// </summary>
        /// <param name="view">The view of the context type.</param>
        /// <param name="contextType">The context type.</param>
        /// <returns>All the content that has been defined for the view of the context type.</returns>
        public ReadOnlyCollection<StaticContentValued> GetContentOfViewOfContextType(View view, ContextType contextType)
        {
            ViewsPerContextType viewsPerContextType = GetViewsPerContextType(contextType);
            if (viewsPerContextType != null)
                return viewsPerContextType.GetContentOfView(view);

            return new List<StaticContentValued>().AsReadOnly();
        }
        #endregion Method: GetContentOfViewOfContextType(View view, ContextType contextType)

        #region Method: GetAllContent()
        /// <summary>
        /// Get all the associated content of the abstract game node, regardless of context or view.
        /// </summary>
        /// <returns>All the content that has been defined for this game object.</returns>
        public IEnumerable<ContentValued> GetAllContent()
        {
            foreach (ViewsPerContextType viewsPerContextType in this.ViewsPerContextType)
            {
                foreach (StaticContentValued staticContentValued in viewsPerContextType.GetAllContent())
                    yield return staticContentValued;
            }
            foreach (DynamicContentValued dynamicContentValued in this.DynamicContent)
                yield return dynamicContentValued;
        }
        #endregion Method: GetAllContent()

        #region Method: Remove()
        /// <summary>
        /// Remove the abstract game node.
        /// </summary>
        public override void Remove()
        {
            GameDatabase.Current.StartChange();

            // Remove the views per context type
            foreach (ViewsPerContextType viewsPerContextType in this.ViewsPerContextType)
                viewsPerContextType.Remove();
            GameDatabase.Current.Remove(this.ID, GameTables.AbstractGameNodeViewsPerContextType);

            // Remove the dynamic content
            foreach (DynamicContentValued dynamicContentValued in this.DynamicContent)
                dynamicContentValued.Remove();
            GameDatabase.Current.Remove(this.ID, GameTables.AbstractGameNodeDynamicContent);

            // Remove the event content
            foreach (EventContent eventContent in this.EventContent)
                eventContent.Remove();
            GameDatabase.Current.Remove(this.ID, GameTables.AbstractGameNodeEventContent);

            // Remove the event extensions
            foreach (EventExtension eventExtension in this.EventExtensions)
                eventExtension.Remove();
            GameDatabase.Current.Remove(this.ID, GameTables.AbstractGameNodeEventExtension);

            base.Remove();

            GameDatabase.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: Clone()
        /// <summary>
        /// Clones the abstract game node.
        /// </summary>
        /// <returns>A clone of the abstract game node.</returns>
        public new AbstractGameNode Clone()
        {
            return base.Clone() as AbstractGameNode;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: AbstractGameNode

}