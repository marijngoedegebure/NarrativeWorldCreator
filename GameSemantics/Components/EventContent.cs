/**************************************************************************
 * 
 * EventContent.cs
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
using Common;
using GameSemantics.Data;
using GameSemantics.GameContent;
using GameSemantics.Utilities;
using Semantics.Components;
using Semantics.Data;

namespace GameSemantics.Components
{

    #region Class: EventContent
    /// <summary>
    /// A class to keep track of content that should be started or stopped when an event starts/is executed/stops.
    /// </summary>
    public sealed class EventContent : IdHolder
    {

        #region Properties and Fields

        #region Property: Event
        /// <summary>
        /// Gets or sets the event.
        /// </summary>
        public Event Event
        {
            get
            {
                return GameDatabase.Current.Select<Event>(this.ID, GameTables.EventContent, GameColumns.Event);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameTables.EventContent, GameColumns.Event, value);
                NotifyPropertyChanged("Event");
            }
        }
        #endregion Property: Event

        #region Property: EventState
        /// <summary>
        /// Gets or sets the state of the event for the content to trigger.
        /// </summary>
        public EventStateExtended EventState
        {
            get
            {
                return GameDatabase.Current.Select<EventStateExtended>(this.ID, GameTables.EventContent, GameColumns.EventState);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameTables.EventContent, GameColumns.EventState, value);
                NotifyPropertyChanged("EventState");
            }
        }
        #endregion Property: EventState

        #region Property: ContentBehavior
        /// <summary>
        /// Gets or sets the behavior of the content for the state of the event.
        /// </summary>
        public StartStop ContentBehavior
        {
            get
            {
                return GameDatabase.Current.Select<StartStop>(this.ID, GameTables.EventContent, GameColumns.ContentBehavior);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameTables.EventContent, GameColumns.ContentBehavior, value);
                NotifyPropertyChanged("ContentBehavior");
            }
        }
        #endregion Property: ContentBehavior

        #region Property: Content
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        public DynamicContentValued Content
        {
            get
            {
                return GameDatabase.Current.Select<DynamicContentValued>(this.ID, GameTables.EventContent, GameColumns.DynamicContentValued);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameTables.EventContent, GameColumns.DynamicContentValued, value);
                NotifyPropertyChanged("Content");
            }
        }
        #endregion Property: Content

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: EventContent()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static EventContent()
        {
            // Event and content
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(GameColumns.Event, new Tuple<Type, EntryType>(typeof(Event), EntryType.Nullable));
            dict.Add(GameColumns.ContentValued, new Tuple<Type, EntryType>(typeof(ContentValued), EntryType.Nullable));
            GameDatabase.Current.AddTableDefinition(GameTables.EventContent, typeof(EventContent), dict);
        }
        #endregion Static Constructor: EventContent()

        #region Constructor: EventContent()
        /// <summary>
        /// Creates new event content.
        /// </summary>
        public EventContent()
            : base()
        {
        }
        #endregion Constructor: EventContent()

        #region Constructor: EventContent(Event even, EventStateExtended eventState, StartStop contentBehavior, DynamicContentValued content)
        /// <summary>
        /// Creates new event content.
        /// </summary>
        /// <param name="even">The event.</param>
        /// <param name="eventState">The state of the event.</param>
        /// <param name="contentBehavior">The behavior of the content.</param>
        /// <param name="content">The content</param>
        public EventContent(Event even, EventStateExtended eventState, StartStop contentBehavior, DynamicContentValued content)
            : base()
        {
            GameDatabase.Current.StartChange();

            this.Event = even;
            this.EventState = eventState;
            this.ContentBehavior = contentBehavior;
            this.Content = content;

            GameDatabase.Current.StopChange();
        }
        #endregion Constructor: EventContent(Event even, EventStateExtended eventState, StartStop contentBehavior, DynamicContentValued content)

        #region Constructor: EventContent(uint id)
        /// <summary>
        /// Creates new event content from the given ID.
        /// </summary>
        /// <param name="id">The ID to create event content from.</param>
        private EventContent(uint id)
            : base(id)
        {
        }
        #endregion Constructor: EventContent(uint id)

        #region Constructor: EventContent(EventContent eventContent)
        /// <summary>
        /// Clones event content.
        /// </summary>
        /// <param name="eventContent">The event content to clone.</param>
        public EventContent(EventContent eventContent)
            : base()
        {
            if (eventContent != null)
            {
                GameDatabase.Current.StartChange();

                this.Event = eventContent.Event;
                this.EventState = eventContent.EventState;
                this.ContentBehavior = eventContent.ContentBehavior;
                if (eventContent.Content != null)
                    this.Content = eventContent.Content.Clone();

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: EventContent(EventContent eventContent)

        #endregion Method Group: Constructors

    }
    #endregion Class: EventContent
		
}