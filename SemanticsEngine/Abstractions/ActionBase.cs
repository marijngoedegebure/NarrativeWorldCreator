/**************************************************************************
 * 
 * ActionBase.cs
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
using Semantics.Abstractions;
using Semantics.Components;
using SemanticsEngine.Components;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Abstractions
{

    #region Class: ActionBase
    /// <summary>
    /// A base of an action.
    /// </summary>
    public class ActionBase : AbstractionBase
    {

        #region Properties and Fields

        #region Property: Action
        /// <summary>
        /// Gets the action of which this is an action base.
        /// </summary>
        protected internal Action Action
        {
            get
            {
                return this.IdHolder as Action;
            }
        }
        #endregion Property: Action

        #region Property: Events
        /// <summary>
        /// Gets all the events of the action.
        /// </summary>
        public ReadOnlyCollection<EventBase> Events
        {
            get
            {
                List<EventBase> events = new List<EventBase>();
                events.AddRange(this.AutomaticEvents);
                events.AddRange(this.ManualEvents);
                return events.AsReadOnly();
            }
        }
        #endregion Property: Events

        #region Property: AutomaticEvents
        /// <summary>
        /// The automatic events of the action.
        /// </summary>
        private EventBase[] automaticEvents = null;

        /// <summary>
        /// Gets the automatic events of the action.
        /// </summary>
        public ReadOnlyCollection<EventBase> AutomaticEvents
        {
            get
            {
                if (automaticEvents == null)
                {
                    LoadAutomaticEvents();
                    if (automaticEvents == null)
                        return new List<EventBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<EventBase>(automaticEvents);
            }
        }

        /// <summary>
        /// Loads the automatic events.
        /// </summary>
        private void LoadAutomaticEvents()
        {
            if (this.Action != null)
            {
                List<EventBase> eventBases = new List<EventBase>();
                foreach (Event even in this.Action.AutomaticEvents)
                    eventBases.Add(BaseManager.Current.GetBase<EventBase>(even));
                automaticEvents = eventBases.ToArray();
            }
        }
        #endregion Property: AutomaticEvents

        #region Property: ManualEvents
        /// <summary>
        /// The manual events of the action.
        /// </summary>
        private EventBase[] manualEvents = null;

        /// <summary>
        /// Gets the manual events of the action.
        /// </summary>
        public ReadOnlyCollection<EventBase> ManualEvents
        {
            get
            {
                if (manualEvents == null)
                {
                    LoadManualEvents();
                    if (manualEvents == null)
                        return new List<EventBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<EventBase>(manualEvents);
            }
        }

        /// <summary>
        /// Loads the manual events.
        /// </summary>
        private void LoadManualEvents()
        {
            if (this.Action != null)
            {
                List<EventBase> eventBases = new List<EventBase>();
                foreach (Event even in this.Action.ManualEvents)
                    eventBases.Add(BaseManager.Current.GetBase<EventBase>(even));
                manualEvents = eventBases.ToArray();
            }
        }
        #endregion Property: ManualEvents

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ActionBase(Action action)
        /// <summary>
        /// Create an action base from the given action.
        /// </summary>
        /// <param name="action">The action to create an action base from.</param>
        protected internal ActionBase(Action action)
            : base(action)
        {
            if (action != null)
            {
                if (BaseManager.PreloadProperties)
                {
                    LoadAutomaticEvents();
                    LoadManualEvents();
                }
            }
        }
        #endregion Constructor: ActionBase(Action action)

        #endregion Method Group: Constructors

    }
    #endregion Class: ActionBase

}
