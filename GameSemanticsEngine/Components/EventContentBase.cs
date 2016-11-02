/**************************************************************************
 * 
 * EventContentBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using GameSemantics.Utilities;
using GameSemanticsEngine.GameContent;
using GameSemanticsEngine.Tools;
using SemanticsEngine.Components;

namespace GameSemantics.Components
{

    #region Class: EventContentBase
    /// <summary>
    /// A base of event content.
    /// </summary>
    public sealed class EventContentBase : Base
    {

        #region Properties and Fields

        #region Property: EventContent
        /// <summary>
        /// Gets the event content of which this is an event content base.
        /// </summary>
        internal EventContent EventContent
        {
            get
            {
                return this.IdHolder as EventContent;
            }
        }
        #endregion Property: EventContent

        #region Property: Event
        /// <summary>
        /// The event.
        /// </summary>
        private EventBase even = null;
        
        /// <summary>
        /// Gets the event.
        /// </summary>
        public EventBase Event
        {
            get
            {
                return even;
            }
        }
        #endregion Property: Event

        #region Property: EventState
        /// <summary>
        /// The state of the event for the content to trigger.
        /// </summary>
        private EventStateExtended eventState = default(EventStateExtended);
        
        /// <summary>
        /// Gets the state of the event for the content to trigger.
        /// </summary>
        public EventStateExtended EventState
        {
            get
            {
                return eventState;
            }
        }
        #endregion Property: EventState

        #region Property: ContentBehavior
        /// <summary>
        /// The behavior of the content for the state of the event.
        /// </summary>
        private StartStop contentBehavior = default(StartStop);
        
        /// <summary>
        /// Gets the behavior of the content for the state of the event.
        /// </summary>
        public StartStop ContentBehavior
        {
            get
            {
                return contentBehavior;
            }
        }
        #endregion Property: ContentBehavior

        #region Property: Content
        /// <summary>
        /// The content.
        /// </summary>
        private DynamicContentValuedBase content = null;
        
        /// <summary>
        /// Gets the content.
        /// </summary>
        public DynamicContentValuedBase Content
        {
            get
            {
                return content;
            }
        }
        #endregion Property: Content

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: EventContentBase(EventContent eventContent)
        /// <summary>
        /// Creates a new event content base from the given event content.
        /// </summary>
        /// <param name="eventContent">The event content to create an event content base from.</param>
        internal EventContentBase(EventContent eventContent)
            : base(eventContent)
        {
            if (eventContent != null)
            {
                this.even = GameBaseManager.Current.GetBase<EventBase>(eventContent.Event);
                this.eventState = eventContent.EventState;
                this.contentBehavior = eventContent.ContentBehavior;
                this.content = GameBaseManager.Current.GetBase<DynamicContentValuedBase>(eventContent.Content);
            }
        }
        #endregion Constructor: EventContentBase(EventContent eventContent)

        #endregion Method Group: Constructors

    }
    #endregion Class: EventContentBase
		
}