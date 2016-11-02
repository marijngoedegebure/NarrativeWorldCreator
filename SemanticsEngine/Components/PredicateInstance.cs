/**************************************************************************
 * 
 * PredicateInstance.cs
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
using SemanticsEngine.Abstractions;
using SemanticsEngine.Entities;
using SemanticsEngine.Interfaces;

namespace SemanticsEngine.Components
{

    #region Class: PredicateInstance
    /// <summary>
    /// An instance of a specific predicate.
    /// </summary>
    public sealed class PredicateInstance : Instance, IVariableInstanceHolder
    {

        #region Properties and Fields

        #region Property: PredicateBase
        /// <summary>
        /// Gets the predicate base of which this is a predicate instance.
        /// </summary>
        public PredicateBase PredicateBase
        {
            get
            {
                return this.Base as PredicateBase;
            }
        }
        #endregion Property: PredicateBase

        #region Property: Actor
        /// <summary>
        /// Gets the actor of the predicate.
        /// </summary>
        public EntityInstance Actor
        {
            get
            {
                return GetProperty<EntityInstance>("Actor", null);
            }
            private set
            {
                if (this.Actor != value)
                    SetProperty("Actor", value);
            }
        }
        #endregion Property: Actor

        #region Property: PredicateType
        /// <summary>
        /// Gets the predicate type of the predicate.
        /// </summary>
        public PredicateTypeBase PredicateType
        {
            get
            {
                if (this.PredicateBase != null)
                    return this.PredicateBase.PredicateType;
                return null;
            }
        }
        #endregion Property: PredicateType

        #region Property: Target
        /// <summary>
        /// Gets the target of the predicate.
        /// </summary>
        public EntityInstance Target
        {
            get
            {
                return GetProperty<EntityInstance>("Target", null);
            }
            private set
            {
                if (this.Target != value)
                    SetProperty("Target", value);
            }
        }
        #endregion Property: Target

        #region Field: variableReferenceInstanceHolder
        /// <summary>
        /// The variable instance holder.
        /// </summary>
        private VariableReferenceInstanceHolder variableReferenceInstanceHolder = new VariableReferenceInstanceHolder();
        #endregion Field: variableReferenceInstanceHolder

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: PredicateInstance(PredicateBase predicateBase)
        /// <summary>
        /// Creates a new predicate instance from the given predicate base.
        /// </summary>
        /// <param name="predicateBase">The predicate base to create the predicate instance from.</param>
        internal PredicateInstance(PredicateBase predicateBase)
            : base(predicateBase)
        {
        }
        #endregion Constructor: PredicateInstance(PredicateBase predicateBase)

        #region Constructor: PredicateInstance(PredicateBase predicateBase, EntityInstance actor, EntityInstance target)
        /// <summary>
        /// Creates a new predicate instance from the given predicate base for the given actor and target.
        /// </summary>
        /// <param name="predicateBase">The predicate base to create the predicate instance from.</param>
        /// <param name="actor">The actor.</param>
        /// <param name="target">The target.</param>
        internal PredicateInstance(PredicateBase predicateBase, EntityInstance actor, EntityInstance target)
            : base(predicateBase)
        {
            this.Actor = actor;
            this.Target = target;
        }
        #endregion Constructor: PredicateInstance(PredicateBase predicateBase, EntityInstance actor, EntityInstance target)

        #region Constructor: PredicateInstance(PredicateInstance predicateInstance)
        /// <summary>
        /// Clones a predicate instance.
        /// </summary>
        /// <param name="predicateInstance">The predicate instance to clone.</param>
        internal PredicateInstance(PredicateInstance predicateInstance)
            : base(predicateInstance)
        {
            if (predicateInstance != null)
            {
                this.Actor = predicateInstance.Actor;
                this.Target = predicateInstance.Target;
                this.variableReferenceInstanceHolder = new VariableReferenceInstanceHolder(predicateInstance.variableReferenceInstanceHolder);
            }
        }
        #endregion Constructor: PredicateInstance(PredicateInstance predicateInstance)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetVariable(VariableBase variableBase)
        /// <summary>
        /// Get the variable instance for the given variable base.
        /// </summary>
        /// <param name="variableBase">The variable base to get the variable instance of.</param>
        /// <returns>The variable instance of the variable base.</returns>
        public VariableInstance GetVariable(VariableBase variableBase)
        {
            return this.variableReferenceInstanceHolder.GetVariable(variableBase, this);
        }
        #endregion Method: GetVariable(VariableBase variableBase)

        #region Method: GetManualInput()
        /// <summary>
        /// Get the manual input.
        /// </summary>
        /// <returns>The manual input.</returns>
        public Dictionary<string, object> GetManualInput()
        {
            return null;
        }
        #endregion Method: GetManualInput()

        #region Method: GetActor()
        /// <summary>
        /// Get the actor.
        /// </summary>
        /// <returns>The actor.</returns>
        public EntityInstance GetActor()
        {
            return this.Actor;
        }
        #endregion Method: GetActor()

        #region Method: GetTarget()
        /// <summary>
        /// Get the target.
        /// </summary>
        /// <returns>The target.</returns>
        public EntityInstance GetTarget()
        {
            return this.Target;
        }
        #endregion Method: GetTarget()

        #region Method: GetArtifact()
        /// <summary>
        /// Get the artifact.
        /// </summary>
        /// <returns>The artifact.</returns>
        public EntityInstance GetArtifact()
        {
            return null;
        }
        #endregion Method: GetArtifact()

        #endregion Method Group: Other

    }
    #endregion Class: PredicateInstance

}