/**************************************************************************
 * 
 * EventExtension.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System.Collections.ObjectModel;
using GameSemantics.Data;
using GameSemantics.GameContent;
using Semantics.Components;
using Semantics.Utilities;

namespace GameSemantics.Components
{

    #region Class: EventExtension
    /// <summary>
    /// An extension for an event.
    /// </summary>
    public sealed class EventExtension : IdHolder
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
                return GameDatabase.Current.Select<Event>(this.ID, GameTables.EventExtension, GameColumns.Event);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameTables.EventExtension, GameColumns.Event, value);
                NotifyPropertyChanged("Event");
            }
        }
        #endregion Property: Event

        #region Property: ActorContentConditions
        /// <summary>
        /// Gets the content conditions of the actor.
        /// </summary>
        public ReadOnlyCollection<ContentCondition> ActorContentConditions
        {
            get
            {
                return GameDatabase.Current.SelectAll<ContentCondition>(this.ID, GameTables.EventExtensionActorContentCondition, GameColumns.ContentCondition).AsReadOnly();
            }
        }
        #endregion Property: ActorContentConditions

        #region Property: TargetContentConditions
        /// <summary>
        /// Gets the content conditions of the target.
        /// </summary>
        public ReadOnlyCollection<ContentCondition> TargetContentConditions
        {
            get
            {
                return GameDatabase.Current.SelectAll<ContentCondition>(this.ID, GameTables.EventExtensionTargetContentCondition, GameColumns.ContentCondition).AsReadOnly();
            }
        }
        #endregion Property: TargetContentConditions

        #region Property: ArtifactContentConditions
        /// <summary>
        /// Gets the content conditions of the artifact.
        /// </summary>
        public ReadOnlyCollection<ContentCondition> ArtifactContentConditions
        {
            get
            {
                return GameDatabase.Current.SelectAll<ContentCondition>(this.ID, GameTables.EventExtensionArtifactContentCondition, GameColumns.ContentCondition).AsReadOnly();
            }
        }
        #endregion Property: ArtifactContentConditions

        #region Property: ContentChanges
        /// <summary>
        /// Gets the content changes of the event extension.
        /// </summary>
        public ReadOnlyCollection<ContentChange> ContentChanges
        {
            get
            {
                return GameDatabase.Current.SelectAll<ContentChange>(this.ID, GameTables.EventExtensionContentChange, GameColumns.ContentChange).AsReadOnly();
            }
        }
        #endregion Property: ContentChanges

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: EventExtension()
        /// <summary>
        /// Creates a new event extension.
        /// </summary>
        public EventExtension()
            : base()
        {
        }
        #endregion Constructor: EventExtension()

        #region Constructor: EventExtension(uint id)
        /// <summary>
        /// Creates a new event extension from the given ID.
        /// </summary>
        /// <param name="id">The ID to create an event extension from.</param>
        private EventExtension(uint id)
            : base(id)
        {
        }
        #endregion Constructor: EventExtension(uint id)

        #region Constructor: EventExtension(EventExtension eventExtension)
        /// <summary>
        /// Clones an event extension.
        /// </summary>
        /// <param name="eventExtension">The event extension to clone.</param>
        public EventExtension(EventExtension eventExtension)
            : base()
        {
            if (eventExtension != null)
            {
                GameDatabase.Current.StartChange();

                foreach (ContentCondition contentCondition in eventExtension.ActorContentConditions)
                    AddActorContentCondition(contentCondition.Clone());
                foreach (ContentCondition contentCondition in eventExtension.TargetContentConditions)
                    AddTargetContentCondition(contentCondition.Clone());
                foreach (ContentCondition contentCondition in eventExtension.ArtifactContentConditions)
                    AddArtifactContentCondition(contentCondition.Clone());
                foreach (ContentChange contentChange in eventExtension.ContentChanges)
                    AddContentChange(contentChange.Clone());

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: EventExtension(EventExtension eventExtension)

        #region Constructor: EventExtension(Event even)
        /// <summary>
        /// Creates an extension for the given event.
        /// </summary>
        /// <param name="even">The event to create an extension for.</param>
        public EventExtension(Event even)
            : base()
        {
            this.Event = even;
        }
        #endregion Constructor: EventExtension(Event even)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddActorContentCondition(ContentCondition contentCondition)
        /// <summary>
        /// Adds the given content condition for the actor.
        /// </summary>
        /// <param name="contentCondition">The content condition to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddActorContentCondition(ContentCondition contentCondition)
        {
            if (contentCondition != null)
            {
                // If the content condition is already available in all content conditions, there is no use to add it
                if (HasActorContentCondition(contentCondition))
                    return Message.RelationExistsAlready;

                // Add the content condition
                GameDatabase.Current.Insert(this.ID, GameTables.EventExtensionActorContentCondition, GameColumns.ContentCondition, contentCondition);
                NotifyPropertyChanged("ActorContentConditions");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddActorContentCondition(ContentCondition contentCondition)

        #region Method: RemoveActorContentCondition(ContentCondition contentCondition)
        /// <summary>
        /// Removes a content condition for the actor.
        /// </summary>
        /// <param name="contentCondition">The content condition to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveActorContentCondition(ContentCondition contentCondition)
        {
            if (contentCondition != null)
            {
                if (HasActorContentCondition(contentCondition))
                {
                    // Remove the content condition
                    GameDatabase.Current.Remove(this.ID, GameTables.EventExtensionActorContentCondition, GameColumns.ContentCondition, contentCondition);
                    NotifyPropertyChanged("ActorContentConditions");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveActorContentCondition(ContentCondition contentCondition)

        #region Method: AddTargetContentCondition(ContentCondition contentCondition)
        /// <summary>
        /// Adds the given content condition for the target.
        /// </summary>
        /// <param name="contentCondition">The content condition to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddTargetContentCondition(ContentCondition contentCondition)
        {
            if (contentCondition != null)
            {
                // If the content condition is already available in all content conditions, there is no use to add it
                if (HasTargetContentCondition(contentCondition))
                    return Message.RelationExistsAlready;

                // Add the content condition
                GameDatabase.Current.Insert(this.ID, GameTables.EventExtensionTargetContentCondition, GameColumns.ContentCondition, contentCondition);
                NotifyPropertyChanged("TargetContentConditions");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddTargetContentCondition(ContentCondition contentCondition)

        #region Method: RemoveTargetContentCondition(ContentCondition contentCondition)
        /// <summary>
        /// Removes a content condition for the target.
        /// </summary>
        /// <param name="contentCondition">The content condition to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveTargetContentCondition(ContentCondition contentCondition)
        {
            if (contentCondition != null)
            {
                if (HasTargetContentCondition(contentCondition))
                {
                    // Remove the content condition
                    GameDatabase.Current.Remove(this.ID, GameTables.EventExtensionTargetContentCondition, GameColumns.ContentCondition, contentCondition);
                    NotifyPropertyChanged("TargetContentConditions");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveTargetContentCondition(ContentCondition contentCondition)

        #region Method: AddArtifactContentCondition(ContentCondition contentCondition)
        /// <summary>
        /// Adds the given content condition for the artifact.
        /// </summary>
        /// <param name="contentCondition">The content condition to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddArtifactContentCondition(ContentCondition contentCondition)
        {
            if (contentCondition != null)
            {
                // If the content condition is already available in all content conditions, there is no use to add it
                if (HasArtifactContentCondition(contentCondition))
                    return Message.RelationExistsAlready;

                // Add the content condition
                GameDatabase.Current.Insert(this.ID, GameTables.EventExtensionArtifactContentCondition, GameColumns.ContentCondition, contentCondition);
                NotifyPropertyChanged("ArtifactContentConditions");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddArtifactContentCondition(ContentCondition contentCondition)

        #region Method: RemoveArtifactContentCondition(ContentCondition contentCondition)
        /// <summary>
        /// Removes a content condition for the artifact.
        /// </summary>
        /// <param name="contentCondition">The content condition to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveArtifactContentCondition(ContentCondition contentCondition)
        {
            if (contentCondition != null)
            {
                if (HasArtifactContentCondition(contentCondition))
                {
                    // Remove the content condition
                    GameDatabase.Current.Remove(this.ID, GameTables.EventExtensionArtifactContentCondition, GameColumns.ContentCondition, contentCondition);
                    NotifyPropertyChanged("ArtifactContentConditions");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveArtifactContentCondition(ContentCondition contentCondition)

        #region Method: AddContentChange(ContentChange contentChange)
        /// <summary>
        /// Adds the given content change.
        /// </summary>
        /// <param name="contentChange">The content change to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddContentChange(ContentChange contentChange)
        {
            if (contentChange != null)
            {
                // If the content change is already available in all content changes, there is no use to add it
                if (HasContentChange(contentChange))
                    return Message.RelationExistsAlready;

                // Add the content change
                GameDatabase.Current.Insert(this.ID, GameTables.EventExtensionContentChange, GameColumns.ContentChange, contentChange);
                NotifyPropertyChanged("ContentChanges");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddContentChange(ContentChange contentChange)

        #region Method: RemoveContentChange(ContentChange contentChange)
        /// <summary>
        /// Removes a content change.
        /// </summary>
        /// <param name="contentChange">The content change to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveContentChange(ContentChange contentChange)
        {
            if (contentChange != null)
            {
                if (HasContentChange(contentChange))
                {
                    // Remove the content change
                    GameDatabase.Current.Remove(this.ID, GameTables.EventExtensionContentChange, GameColumns.ContentChange, contentChange);
                    NotifyPropertyChanged("ContentChanges");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveContentChange(ContentChange contentChange)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasActorContentCondition(ContentCondition contentCondition)
        /// <summary>
        /// Checks if this event extension has the given content condition for the actor.
        /// </summary>
        /// <param name="contentCondition">The content condition to check.</param>
        /// <returns>Returns true when this event extension has the content condition for the actor.</returns>
        public bool HasActorContentCondition(ContentCondition contentCondition)
        {
            if (contentCondition != null)
            {
                foreach (ContentCondition myContentCondition in this.ActorContentConditions)
                {
                    if (contentCondition.Equals(myContentCondition))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasActorContentCondition(ContentCondition contentCondition)

        #region Method: HasTargetContentCondition(ContentCondition contentCondition)
        /// <summary>
        /// Checks if this event extension has the given content condition for the target.
        /// </summary>
        /// <param name="contentCondition">The content condition to check.</param>
        /// <returns>Returns true when this event extension has the content condition for the target.</returns>
        public bool HasTargetContentCondition(ContentCondition contentCondition)
        {
            if (contentCondition != null)
            {
                foreach (ContentCondition myContentCondition in this.TargetContentConditions)
                {
                    if (contentCondition.Equals(myContentCondition))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasTargetContentCondition(ContentCondition contentCondition)

        #region Method: HasArtifactContentCondition(ContentCondition contentCondition)
        /// <summary>
        /// Checks if this event extension has the given content condition for the artifact.
        /// </summary>
        /// <param name="contentCondition">The content condition to check.</param>
        /// <returns>Returns true when this event extension has the content condition for the artifact.</returns>
        public bool HasArtifactContentCondition(ContentCondition contentCondition)
        {
            if (contentCondition != null)
            {
                foreach (ContentCondition myContentCondition in this.ArtifactContentConditions)
                {
                    if (contentCondition.Equals(myContentCondition))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasArtifactContentCondition(ContentCondition contentCondition)

        #region Method: HasContentChange(ContentChange contentChange)
        /// <summary>
        /// Checks if this event extension has the given content change.
        /// </summary>
        /// <param name="contentChange">The content change to check.</param>
        /// <returns>Returns true when this event extension has the content change.</returns>
        public bool HasContentChange(ContentChange contentChange)
        {
            if (contentChange != null)
            {
                foreach (ContentChange myContentChange in this.ContentChanges)
                {
                    if (contentChange.Equals(myContentChange))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasContentChange(ContentChange contentChange)

        #region Method: Remove()
        /// <summary>
        /// Remove the event extension.
        /// </summary>
        public override void Remove()
        {
            GameDatabase.Current.StartChange();

            // Remove the content conditions
            foreach (ContentCondition contentCondition in this.ActorContentConditions)
                contentCondition.Remove();
            GameDatabase.Current.Remove(this.ID, GameTables.EventExtensionActorContentCondition);
            foreach (ContentCondition contentCondition in this.TargetContentConditions)
                contentCondition.Remove();
            GameDatabase.Current.Remove(this.ID, GameTables.EventExtensionTargetContentCondition);
            foreach (ContentCondition contentCondition in this.ArtifactContentConditions)
                contentCondition.Remove();
            GameDatabase.Current.Remove(this.ID, GameTables.EventExtensionArtifactContentCondition);

            // Remove the content changes
            foreach (ContentChange contentChange in this.ContentChanges)
                contentChange.Remove();
            GameDatabase.Current.Remove(this.ID, GameTables.EventExtensionContentChange);

            GameDatabase.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>A string representation.</returns>
        public override string ToString()
        {
            if (this.Event != null)
                return this.Event.ToString();

            return base.ToString();
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: EventExtension
		
}