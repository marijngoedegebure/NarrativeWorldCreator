/**************************************************************************
 * 
 * ReactionBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using Semantics.Components;
using Semantics.Utilities;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{

    #region Class: ReactionBase
    /// <summary>
    /// A base of a reaction.
    /// </summary>
    public sealed class ReactionBase : EffectBase
    {

        #region Properties and Fields

        #region Property: Reaction
        /// <summary>
        /// Gets the reaction this is a base of.
        /// </summary>
        internal Reaction Reaction
        {
            get
            {
                return this.IdHolder as Reaction;
            }
        }
        #endregion Property: Reaction

        #region Property: Actor
        /// <summary>
        /// The actor of the reaction: the actor or target of the original event.
        /// </summary>
        private ActorTarget actor = ActorTarget.Target;

        /// <summary>
        /// Gets the actor of the reaction: the actor or target of the original event.
        /// </summary>
        public ActorTarget Actor
        {
            get
            {
                return actor;
            }
        }
        #endregion Property: Actor

        #region Property: Action
        /// <summary>
        /// The action of the reaction.
        /// </summary>
        private ActionBase action = null;

        /// <summary>
        /// Gets the action of the reaction.
        /// </summary>
        public ActionBase Action
        {
            get
            {
                return action;
            }
        }
        #endregion Property: Action

        #region Property: Target
        /// <summary>
        /// The actor of the reaction: the actor or target of the original event.
        /// </summary>
        private ActorTarget? target = ActorTarget.Actor;

        /// <summary>
        /// Gets the actor of the reaction: the actor or target of the original event, or nothing.
        /// </summary>
        public ActorTarget? Target
        {
            get
            {
                return target;
            }
        }
        #endregion Property: Target

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ReactionBase(Reaction reaction)
        /// <summary>
        /// Creates a base of the given reaction.
        /// </summary>
        /// <param name="reaction">The reaction to create a base of.</param>
        internal ReactionBase(Reaction reaction)
            : base(reaction)
        {
            if (reaction != null)
            {
                this.actor = reaction.Actor;
                this.action = BaseManager.Current.GetBase<ActionBase>(reaction.Action);
                this.target = reaction.Target;
            }
        }
        #endregion Constructor: ReactionBase(Reaction reaction)

        #endregion Method Group: Constructors

    }
    #endregion Class: ReactionBase

}