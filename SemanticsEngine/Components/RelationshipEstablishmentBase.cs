/**************************************************************************
 * 
 * RelationshipEstablishmentBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
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

    #region Class: RelationshipEstablishmentBase
    /// <summary>
    /// A base of a relationship establishment.
    /// </summary>
    public sealed class RelationshipEstablishmentBase : EffectBase
    {

        #region Properties and Fields

        #region Property: RelationshipEstablishment
        /// <summary>
        /// Gets the relationship establishment this is a base of.
        /// </summary>
        internal RelationshipEstablishment RelationshipEstablishment
        {
            get
            {
                return this.IdHolder as RelationshipEstablishment;
            }
        }
        #endregion Property: RelationshipEstablishment

        #region Property: RelationshipType
        /// <summary>
        /// The relationship type.
        /// </summary>
        private RelationshipTypeBase relationshipType = null;
        
        /// <summary>
        /// Gets the relationship type.
        /// </summary>
        public RelationshipTypeBase RelationshipType
        {
            get
            {
                return relationshipType;
            }
        }
        #endregion Property: RelationshipType

        #region Property: Source
        /// <summary>
        /// The source for the relationship: the actor or target.
        /// </summary>
        private ActorTarget source = ActorTarget.Actor;

        /// <summary>
        /// Gets the source for the relationship: the actor or target.
        /// </summary>
        public ActorTarget Source
        {
            get
            {
                return source;
            }
        }
        #endregion Property: Source

        #region Property: Target
        /// <summary>
        /// The target for the relationship: the actor, target, or a reference.
        /// </summary>
        private ActorTargetArtifactReference target = ActorTargetArtifactReference.Target;

        /// <summary>
        /// Gets the target for the relationship: the actor, target, or a reference.
        /// </summary>
        public ActorTargetArtifactReference Target
        {
            get
            {
                return target;
            }
        }
        #endregion Property: Target

        #region Property: TargetReference
        /// <summary>
        /// The reference, in case the Target has been set to Reference.
        /// </summary>
        private ReferenceBase targetReference = null;
        
        /// <summary>
        /// Gets the reference, in case the Target has been set to Reference.
        /// </summary>
        public ReferenceBase TargetReference
        {
            get
            {
                return targetReference;
            }
        }
        #endregion Property: TargetReference

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: RelationshipEstablishmentBase(RelationshipEstablishment relationshipEstablishment)
        /// <summary>
        /// Creates a base of the given relationship establishment.
        /// </summary>
        /// <param name="relationshipEstablishment">The relationship establishment to create a base of.</param>
        internal RelationshipEstablishmentBase(RelationshipEstablishment relationshipEstablishment)
            : base(relationshipEstablishment)
        {
            if (relationshipEstablishment != null)
            {
                this.relationshipType = BaseManager.Current.GetBase<RelationshipTypeBase>(relationshipEstablishment.RelationshipType);
                this.source = relationshipEstablishment.Source;
                this.target = relationshipEstablishment.Target;
                this.targetReference = BaseManager.Current.GetBase<ReferenceBase>(relationshipEstablishment.TargetReference);
            }
        }
        #endregion Constructor: RelationshipEstablishmentBase(RelationshipEstablishment relationshipEstablishment)

        #endregion Method Group: Constructors

    }
    #endregion Class: RelationshipEstablishmentBase

}