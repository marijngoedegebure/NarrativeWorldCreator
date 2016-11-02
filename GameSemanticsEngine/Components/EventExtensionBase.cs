/**************************************************************************
 * 
 * EventExtensionBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011-2012
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
using SemanticsEngine.Interfaces;

namespace GameSemanticsEngine.Components
{

    #region Class: EventExtensionBase
    /// <summary>
    /// A base of an event extension.
    /// </summary>
    public sealed class EventExtensionBase : Base, IEventExtension
    {

        #region Properties and Fields

        #region Property: EventExtension
        /// <summary>
        /// Gets the event extension of which this is an event extension base.
        /// </summary>
        internal EventExtension EventExtension
        {
            get
            {
                return this.IdHolder as EventExtension;
            }
        }
        #endregion Property: EventExtension

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

        #region Property: ActorContentConditions
        /// <summary>
        /// The content conditions of actor.
        /// </summary>
        private ContentConditionBase[] actorContentConditions = null;

        /// <summary>
        /// Gets the content conditions of the actor.
        /// </summary>
        public IEnumerable<ContentConditionBase> ActorContentConditions
        {
            get
            {
                if (actorContentConditions == null)
                    LoadActorContentConditions();
                foreach (ContentConditionBase contentConditionBase in actorContentConditions)
                    yield return contentConditionBase;
            }
        }

        /// <summary>
        /// Loads the content conditions of the actor.
        /// </summary>
        private void LoadActorContentConditions()
        {
            List<ContentConditionBase> contentConditionBases = new List<ContentConditionBase>();
            if (this.EventExtension != null)
            {
                foreach (ContentCondition contentCondition in this.EventExtension.ActorContentConditions)
                    contentConditionBases.Add(GameBaseManager.Current.GetBase<ContentConditionBase>(contentCondition));
            }
            actorContentConditions = contentConditionBases.ToArray();
        }
        #endregion Property: ActorContentConditions

        #region Property: TargetContentConditions
        /// <summary>
        /// The content conditions of target.
        /// </summary>
        private ContentConditionBase[] targetContentConditions = null;

        /// <summary>
        /// Gets the content conditions of the target.
        /// </summary>
        public IEnumerable<ContentConditionBase> TargetContentConditions
        {
            get
            {
                if (targetContentConditions == null)
                    LoadTargetContentConditions();
                foreach (ContentConditionBase contentConditionBase in targetContentConditions)
                    yield return contentConditionBase;
            }
        }

        /// <summary>
        /// Loads the content conditions of the target.
        /// </summary>
        private void LoadTargetContentConditions()
        {
            List<ContentConditionBase> contentConditionBases = new List<ContentConditionBase>();
            if (this.EventExtension != null)
            {
                foreach (ContentCondition contentCondition in this.EventExtension.TargetContentConditions)
                    contentConditionBases.Add(GameBaseManager.Current.GetBase<ContentConditionBase>(contentCondition));
            }
            targetContentConditions = contentConditionBases.ToArray();
        }
        #endregion Property: TargetContentConditions

        #region Property: ArtifactContentConditions
        /// <summary>
        /// The content conditions of artifact.
        /// </summary>
        private ContentConditionBase[] artifactContentConditions = null;

        /// <summary>
        /// Gets the content conditions of the artifact.
        /// </summary>
        public IEnumerable<ContentConditionBase> ArtifactContentConditions
        {
            get
            {
                if (artifactContentConditions == null)
                    LoadArtifactContentConditions();
                foreach (ContentConditionBase contentConditionBase in artifactContentConditions)
                    yield return contentConditionBase;
            }
        }

        /// <summary>
        /// Loads the content conditions of the artifact.
        /// </summary>
        private void LoadArtifactContentConditions()
        {
            List<ContentConditionBase> contentConditionBases = new List<ContentConditionBase>();
            if (this.EventExtension != null)
            {
                foreach (ContentCondition contentCondition in this.EventExtension.ArtifactContentConditions)
                    contentConditionBases.Add(GameBaseManager.Current.GetBase<ContentConditionBase>(contentCondition));
            }
            artifactContentConditions = contentConditionBases.ToArray();
        }
        #endregion Property: ArtifactContentConditions

        #region Property: ContentChanges
        /// <summary>
        /// The content changes of the event extension.
        /// </summary>
        private ContentChangeBase[] contentChanges = null;

        /// <summary>
        /// Gets the content changes of the event extension.
        /// </summary>
        public IEnumerable<ContentChangeBase> ContentChanges
        {
            get
            {
                if (contentChanges == null)
                    LoadContentChanges();
                foreach (ContentChangeBase contentChangeBase in contentChanges)
                    yield return contentChangeBase;
            }
        }

        /// <summary>
        /// Loads the content changes.
        /// </summary>
        private void LoadContentChanges()
        {
            List<ContentChangeBase> contentChangeBases = new List<ContentChangeBase>();
            if (this.EventExtension != null)
            {
                foreach (ContentChange contentChange in this.EventExtension.ContentChanges)
                    contentChangeBases.Add(GameBaseManager.Current.GetBase<ContentChangeBase>(contentChange));

                // Sort by priority
                contentChangeBases.Sort(delegate(ContentChangeBase c1, ContentChangeBase c2) { return c1.Priority.CompareTo(c2.Priority); });
            }
            contentChanges = contentChangeBases.ToArray();
        }
        #endregion Property: ContentChanges

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: EventExtensionBase(EventExtension eventExtension)
        /// <summary>
        /// Creates a new event extension base from the given event extension.
        /// </summary>
        /// <param name="eventExtension">The event extension to create an event extension base from.</param>
        internal EventExtensionBase(EventExtension eventExtension)
            : base(eventExtension)
        {
            if (eventExtension != null)
            {
                this.even = GameBaseManager.Current.GetBase<EventBase>(eventExtension.Event);

                // Add this extension to the event
                if (this.even != null)
                    this.even.AddExtension(this);

                if (GameBaseManager.PreloadProperties)
                {
                    LoadActorContentConditions();
                    LoadTargetContentConditions();
                    LoadArtifactContentConditions();
                    LoadContentChanges();
                }
            }
        }
        #endregion Constructor: EventExtensionBase(EventExtension eventExtension)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetActorConditions()
        /// <summary>
        /// Get the actor conditions of the extension.
        /// </summary>
        /// <returns>The actor conditions of the extension.</returns>
        public ReadOnlyCollection<ConditionBase> GetActorConditions()
        {
            List<ConditionBase> conditions = new List<ConditionBase>();

            foreach (ContentConditionBase contentConditionBase in this.ActorContentConditions)
                conditions.Add(contentConditionBase);

            return conditions.AsReadOnly();
        }
        #endregion Method: GetActorConditions()

        #region Method: GetTargetConditions()
        /// <summary>
        /// Get the target conditions of the extension.
        /// </summary>
        /// <returns>The target conditions of the extension.</returns>
        public ReadOnlyCollection<ConditionBase> GetTargetConditions()
        {
            List<ConditionBase> conditions = new List<ConditionBase>();

            foreach (ContentConditionBase contentConditionBase in this.TargetContentConditions)
                conditions.Add(contentConditionBase);

            return conditions.AsReadOnly();
        }
        #endregion Method: GetTargetConditions()

        #region Method: GetArtifactConditions()
        /// <summary>
        /// Get the artifact conditions of the extension.
        /// </summary>
        /// <returns>The artifact conditions of the extension.</returns>
        public ReadOnlyCollection<ConditionBase> GetArtifactConditions()
        {
            List<ConditionBase> conditions = new List<ConditionBase>();

            foreach (ContentConditionBase contentConditionBase in this.ArtifactContentConditions)
                conditions.Add(contentConditionBase);

            return conditions.AsReadOnly();
        }
        #endregion Method: GetArtifactConditions()

        #region Method: GetEffects()
        /// <summary>
        /// Get the effects of the extension.
        /// </summary>
        /// <returns>The effects of the extension.</returns>
        public ReadOnlyCollection<EffectBase> GetEffects()
        {
            List<EffectBase> effects = new List<EffectBase>();

            foreach (ContentChangeBase contentChangeBase in this.ContentChanges)
                effects.Add(contentChangeBase);

            return effects.AsReadOnly();
        }
        #endregion Method: GetEffects()

        #endregion Method Group: Other
    }
    #endregion Class: EventExtensionBase

}