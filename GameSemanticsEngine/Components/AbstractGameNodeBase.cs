/**************************************************************************
 * 
 * AbstractGameNodeBase.cs
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
using SemanticsEngine.Abstractions;
using SemanticsEngine.Components;

namespace GameSemanticsEngine.Components
{

    #region Class: AbstractGameNodeBase
    /// <summary>
    /// A base of an abstract game node.
    /// </summary>
    public abstract class AbstractGameNodeBase : NodeBase
    {

        #region Properties and Fields

        #region Property: AbstractGameNode
        /// <summary>
        /// Gets the abstract game node of which this is an abstract game node base.
        /// </summary>
        protected internal AbstractGameNode AbstractGameNode
        {
            get
            {
                return this.Node as AbstractGameNode;
            }
        }
        #endregion Property: AbstractGameNode

        #region Property: ViewsPerContextType
        /// <summary>
        /// The views per context type.
        /// </summary>
        private ViewsPerContextTypeBase[] viewsPerContextType = null;

        /// <summary>
        /// Gets the views per context type.
        /// </summary>
        public ReadOnlyCollection<ViewsPerContextTypeBase> ViewsPerContextType
        {
            get
            {
                if (viewsPerContextType == null)
                {
                    LoadViewsPerContextType();
                    if (viewsPerContextType == null)
                        return new List<ViewsPerContextTypeBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<ViewsPerContextTypeBase>(viewsPerContextType);
            }
        }

        /// <summary>
        /// Loads the views per context type.
        /// </summary>
        private void LoadViewsPerContextType()
        {
            if (this.AbstractGameNode != null)
            {
                List<ViewsPerContextTypeBase> viewsPerContextTypeBases = new List<ViewsPerContextTypeBase>();
                foreach (ViewsPerContextType viewsPerContextType2 in this.AbstractGameNode.ViewsPerContextType)
                    viewsPerContextTypeBases.Add(GameBaseManager.Current.GetBase<ViewsPerContextTypeBase>(viewsPerContextType2));
                viewsPerContextType = viewsPerContextTypeBases.ToArray();
            }
        }
        #endregion Property: ViewsPerContextType

        #region Property: ContextTypes
        /// <summary>
        /// Gets all context types.
        /// </summary>
        public ReadOnlyCollection<ContextTypeBase> ContextTypes
        {
            get
            {
                List<ContextTypeBase> contextTypes = new List<ContextTypeBase>();
                foreach (ViewsPerContextTypeBase viewsPerContextTypeBase in this.ViewsPerContextType)
                    contextTypes.Add(viewsPerContextTypeBase.ContextType);
                return contextTypes.AsReadOnly();
            }
        }
        #endregion Property: ContextTypes

        #region Property: DynamicContent
        /// <summary>
        /// All the dynamic content, used to be linked to the EventContent.
        /// </summary>
        private DynamicContentValuedBase[] dynamicContent = null;
        
        /// <summary>
        /// Gets all the dynamic content, used to be linked to the EventContent.
        /// </summary>
        public ReadOnlyCollection<DynamicContentValuedBase> DynamicContent
        {
            get
            {
                if (dynamicContent == null)
                {
                    LoadDynamicContent();
                    if (dynamicContent == null)
                        return new List<DynamicContentValuedBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<DynamicContentValuedBase>(dynamicContent);
            }
        }

        /// <summary>
        /// Loads the dynamic content.
        /// </summary>
        private void LoadDynamicContent()
        {
            if (this.AbstractGameNode != null)
            {
                List<DynamicContentValuedBase> dynamicContentValuedBases = new List<DynamicContentValuedBase>();
                foreach (DynamicContentValued dynamicContentValued in this.AbstractGameNode.DynamicContent)
                    dynamicContentValuedBases.Add(GameBaseManager.Current.GetBase<DynamicContentValuedBase>(dynamicContentValued));
                dynamicContent = dynamicContentValuedBases.ToArray();
            }
        }
        #endregion Property: DynamicContent

        #region Property: EventContent
        /// <summary>
        /// The content that should be started or stopped when an event starts/is being executed/stops.
        /// </summary>
        private EventContentBase[] eventContent = null;
        
        /// <summary>
        /// Gets the content that should be started or stopped when an event starts/is being executed/stops.
        /// </summary>
        public ReadOnlyCollection<EventContentBase> EventContent
        {
            get
            {
                if (eventContent == null)
                {
                    LoadEventContent();
                    if (eventContent == null)
                        return new List<EventContentBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<EventContentBase>(eventContent);
            }
        }

        /// <summary>
        /// Loads the event content.
        /// </summary>
        private void LoadEventContent()
        {
            if (this.AbstractGameNode != null)
            {
                List<EventContentBase> eventContentBases = new List<EventContentBase>();
                foreach (EventContent eventContent2 in this.AbstractGameNode.EventContent)
                    eventContentBases.Add(GameBaseManager.Current.GetBase<EventContentBase>(eventContent2));
                eventContent = eventContentBases.ToArray();
            }
        }
        #endregion Property: EventContent

        #region Property: EventExtension
        /// <summary>
        /// The event extensions.
        /// </summary>
        private EventExtensionBase[] eventExtensions = null;

        /// <summary>
        /// Gets the event extensions.
        /// </summary>
        public ReadOnlyCollection<EventExtensionBase> EventExtension
        {
            get
            {
                if (eventExtensions != null)
                    return new ReadOnlyCollection<EventExtensionBase>(eventExtensions);
                return new List<EventExtensionBase>(0).AsReadOnly();
            }
        }
        #endregion Property: EventExtension

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: AbstractGameNodeBase(AbstractGameNode abstractGameNode)
        /// <summary>
        /// Creates a new abstract game node base from the given abstract game node.
        /// </summary>
        /// <param name="abstractGameNode">The abstract game node to create an abstract game node base from.</param>
        protected AbstractGameNodeBase(AbstractGameNode abstractGameNode)
            : base(abstractGameNode)
        {
            if (abstractGameNode != null)
            {
                // Always load the event extensions, so they are registered at the event
                List<EventExtensionBase> eventExtensionBases = new List<EventExtensionBase>();
                foreach (EventExtension eventExtension in abstractGameNode.EventExtensions)
                    eventExtensionBases.Add(GameBaseManager.Current.GetBase<EventExtensionBase>(eventExtension));
                this.eventExtensions = eventExtensionBases.ToArray();

                if (GameBaseManager.PreloadProperties)
                {
                    LoadViewsPerContextType();
                    LoadEventContent();
                    LoadDynamicContent();
                }
            }
        }
        #endregion Constructor: AbstractGameNodeBase(AbstractGameNode abstractGameNode)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetViewsPerContextType(ContextTypeBase contextTypeBase)
        /// <summary>
        /// Get the views per context type for the given context type.
        /// </summary>
        /// <param name="contextType">The context type to get the views per context type of; can be null.</param>
        /// <returns>The views per context type of the context type.</returns>
        internal ViewsPerContextTypeBase GetViewsPerContextType(ContextTypeBase contextTypeBase)
        {
            foreach (ViewsPerContextTypeBase viewsPerContextType in this.ViewsPerContextType)
            {
                if ((contextTypeBase == null && viewsPerContextType.ContextType == null) || (contextTypeBase != null && contextTypeBase.Equals(viewsPerContextType.ContextType)))
                    return viewsPerContextType;
            }
            return null;
        }
        #endregion Method: GetViewsPerContextType(ContextTypeBase contextTypeBase)

        #region Method: GetViewsOfContextType(ContextTypeBase contextTypeBase)
        /// <summary>
        /// Get the views that are defined for the given context type.
        /// </summary>
        /// <param name="contextTypeBase">The context type to get the views of; can be null.</param>
        /// <returns>Returns the views that are defined for the context type.</returns>
        public IEnumerable<ViewBase> GetViewsOfContextType(ContextTypeBase contextTypeBase)
        {
            ViewsPerContextTypeBase viewsPerContextType = GetViewsPerContextType(contextTypeBase);
            if (viewsPerContextType != null)
            {
                foreach (ViewBase viewBase in viewsPerContextType.Views)
                    yield return viewBase;
            }
        }
        #endregion Method: GetViewsOfContextType(ContextTypeBase contextTypeBase)

        #region Method: GetContentOfViewOfContextType(ViewBase viewBase, ContextTypeBase contextTypeBase)
        /// <summary>
        /// Get all the content that has been defined for the given view of the given context type.
        /// </summary>
        /// <param name="viewBase">The view of the context type; can be null.</param>
        /// <param name="contextTypeBase">The context type; can be null.</param>
        /// <returns>All the content that has been defined for the view of the context type.</returns>
        public IEnumerable<StaticContentValuedBase> GetContentOfViewOfContextType(ViewBase viewBase, ContextTypeBase contextTypeBase)
        {
            ViewsPerContextTypeBase viewsPerContextType = GetViewsPerContextType(contextTypeBase);
            if (viewsPerContextType != null)
            {
                foreach (StaticContentValuedBase staticContentValued in viewsPerContextType.GetContentOfView(viewBase))
                    yield return staticContentValued;
            }
        }
        #endregion Method: GetContentOfViewOfContextType(ViewBase viewBase, ContextTypeBase contextTypeBase)

        #region Method: GetAllContent()
        /// <summary>
        /// Get all the associated content of the abstract game node, regardless of context or view.
        /// </summary>
        /// <returns>All the content that has been defined for this game object.</returns>
        public IEnumerable<ContentValuedBase> GetAllContent()
        {
            foreach (ViewsPerContextTypeBase viewsPerContextType in this.ViewsPerContextType)
            {
                foreach (StaticContentValuedBase staticContentValued in viewsPerContextType.GetAllContent())
                    yield return staticContentValued;
            }
            foreach (DynamicContentValuedBase dynamicContentValuedBase in this.DynamicContent)
                yield return dynamicContentValuedBase;
        }
        #endregion Method: GetAllContent()

        #endregion Method Group: Other

    }
    #endregion Class: AbstractGameNodeBase

}