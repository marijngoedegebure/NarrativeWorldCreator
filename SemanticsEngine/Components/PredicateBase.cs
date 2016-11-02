/**************************************************************************
 * 
 * PredicateBase.cs
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
using Semantics.Components;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Entities;
using SemanticsEngine.Interfaces;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{

    #region Class: PredicateBase
    /// <summary>
    /// A predicate keeps track of actor and target of a predicate type in a certain context.
    /// </summary>
    public sealed class PredicateBase : Base
    {

        #region Properties and Fields

        #region Property: Predicate
        /// <summary>
        /// Gets the predicate of which this is a predicate base.
        /// </summary>
        internal Predicate Predicate
        {
            get
            {
                return this.IdHolder as Predicate;
            }
        }
        #endregion Property: Predicate

        #region Property: PredicateType
        /// <summary>
        /// The predicate type of the predicate.
        /// </summary>
        private PredicateTypeBase predicateType = null;
        
        /// <summary>
        /// Gets the predicate type of the predicate.
        /// </summary>
        public PredicateTypeBase PredicateType
        {
            get
            {
                return predicateType;
            }
        }
        #endregion Property: PredicateType

        #region Property: Actor
        /// <summary>
        /// The actor of the predicate.
        /// </summary>
        private EntityBase actor = null;
        
        /// <summary>
        /// Gets the actor of the predicate.
        /// </summary>
        public EntityBase Actor
        {
            get
            {
                return actor;
            }
        }
        #endregion Property: Actor

        #region Property: Target
        /// <summary>
        /// The target of the predicate.
        /// </summary>
        private EntityBase target = null;
        
        /// <summary>
        /// Gets the target of the predicate.
        /// </summary>
        public EntityBase Target
        {
            get
            {
                return target;
            }
        }
        #endregion Property: Target

        #region Property: ActorRequirements
        /// <summary>
        /// The requirements of the actor.
        /// </summary>
        private RequirementsBase actorRequirements = null;

        /// <summary>
        /// Gets the requirements of the actor.
        /// </summary>
        public RequirementsBase ActorRequirements
        {
            get
            {
                if (actorRequirements == null)
                    LoadActorRequirements();
                return actorRequirements;
            }
        }

        /// <summary>
        /// Loads the requirements of the actor.
        /// </summary>
        private void LoadActorRequirements()
        {
            if (this.Predicate != null)
                actorRequirements = BaseManager.Current.GetBase<RequirementsBase>(this.Predicate.ActorRequirements);
        }
        #endregion Property: ActorRequirements

        #region Property: TargetRequirements
        /// <summary>
        /// The requirements of the target.
        /// </summary>
        private RequirementsBase targetRequirements = null;

        /// <summary>
        /// Gets the requirements of the target.
        /// </summary>
        public RequirementsBase TargetRequirements
        {
            get
            {
                if (targetRequirements == null)
                    LoadTargetRequirements();
                return targetRequirements;
            }
        }

        /// <summary>
        /// Loads the requirements of the target.
        /// </summary>
        private void LoadTargetRequirements()
        {
            if (this.Predicate != null)
                targetRequirements = BaseManager.Current.GetBase<RequirementsBase>(this.Predicate.TargetRequirements);
        }
        #endregion Property: TargetRequirements

        #region Property: SpatialRequirementBetweenActorAndTarget
        /// <summary>
        /// The spatial condition between the actor and target.
        /// </summary>
        private SpatialRequirementBase spatialRequirementBetweenActorAndTarget = null;

        /// <summary>
        /// Gets the spatial condition between the actor and target.
        /// </summary>
        public SpatialRequirementBase SpatialRequirementBetweenActorAndTarget
        {
            get
            {
                return spatialRequirementBetweenActorAndTarget;
            }
        }
        #endregion Property: SpatialRequirementBetweenActorAndTarget

        #region Property: Variables
        /// <summary>
        /// The variables.
        /// </summary>
        private VariableBase[] variables = null;

        /// <summary>
        /// Gets the variables.
        /// </summary>
        public IEnumerable<VariableBase> Variables
        {
            get
            {
                if (variables == null)
                    LoadVariables();
                foreach (VariableBase variableBase in variables)
                    yield return variableBase;
            }
        }

        /// <summary>
        /// Loads the variables.
        /// </summary>
        private void LoadVariables()
        {
            List<VariableBase> variableBases = new List<VariableBase>();
            if (this.Predicate != null)
            {
                foreach (Variable variable in this.Predicate.Variables)
                    variableBases.Add(BaseManager.Current.GetBase<VariableBase>(variable));
            }
            variables = variableBases.ToArray();
        }
        #endregion Property: Variables

        #region Property: References
        /// <summary>
        /// The references.
        /// </summary>
        private ReferenceBase[] references = null;

        /// <summary>
        /// Gets the references.
        /// </summary>
        public IEnumerable<ReferenceBase> References
        {
            get
            {
                if (references == null)
                    LoadReferences();
                foreach (ReferenceBase referenceBase in references)
                    yield return referenceBase;
            }
        }

        /// <summary>
        /// Loads the references.
        /// </summary>
        private void LoadReferences()
        {
            List<ReferenceBase> referenceBases = new List<ReferenceBase>();
            if (this.Predicate != null)
            {
                foreach (Reference reference in this.Predicate.References)
                    referenceBases.Add(BaseManager.Current.GetBase<ReferenceBase>(reference));
            }
            references = referenceBases.ToArray();
        }
        #endregion Property: References

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: PredicateBase(Predicate predicate)
        /// <summary>
        /// Creates a predicate base from the given predicate.
        /// </summary>
        /// <param name="predicate">The predicate to create a predicate base from.</param>
        internal PredicateBase(Predicate predicate)
            : base(predicate)
        {
            if (predicate != null)
            {
                this.predicateType = BaseManager.Current.GetBase<PredicateTypeBase>(predicate.PredicateType);
                this.actor = BaseManager.Current.GetBase<EntityBase>(predicate.Actor);
                this.target = BaseManager.Current.GetBase<EntityBase>(predicate.Target);
                this.spatialRequirementBetweenActorAndTarget = BaseManager.Current.GetBase<SpatialRequirementBase>(predicate.SpatialRequirementBetweenActorAndTarget);

                if (BaseManager.PreloadProperties)
                {
                    LoadActorRequirements();
                    LoadTargetRequirements();
                    LoadVariables();
                    LoadReferences();
                }
            }
        }
        #endregion Constructor: PredicateBase(Predicate predicate)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: IsSatisfied(EntityInstance actor, EntityInstance target, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the predicate is satisfied for the given actor and target.
        /// </summary>
        /// <param name="actor">The actor.</param>
        /// <param name="target">The target.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the predicate is satisfied for the given actor and target.</returns>
        public bool IsSatisfied(EntityInstance actor, EntityInstance target, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (actor != null && target != null)
            {
                // Check whether the instances are of the correct type
                if (actor.IsNodeOf(this.Actor) && target.IsNodeOf(this.Target))
                {
                    // Check whether the requirements have been satisfied
                    if (!this.ActorRequirements.IsSatisfied(actor, iVariableInstanceHolder))
                        return false;
                    if (!this.TargetRequirements.IsSatisfied(target, iVariableInstanceHolder))
                        return false;

                    // Check whether the spatial requirements have been satisfied
                    PhysicalObjectInstance physicalActor = actor as PhysicalObjectInstance;
                    PhysicalObjectInstance physicalTarget = target as PhysicalObjectInstance;
                    if (this.SpatialRequirementBetweenActorAndTarget != null && !this.SpatialRequirementBetweenActorAndTarget.IsSatisfied(physicalActor, physicalTarget, iVariableInstanceHolder))
                        return false;

                    return true;
                }
            }
            return false;
        }
        #endregion Method: IsSatisfied(EntityInstance actor, EntityInstance target, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>A string representation.</returns>
        public override string ToString()
        {
            if (this.PredicateType != null)
                return this.PredicateType.ToString();

            return base.ToString();
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: PredicateBase

}