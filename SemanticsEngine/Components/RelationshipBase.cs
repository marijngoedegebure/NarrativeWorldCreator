/**************************************************************************
 * 
 * RelationshipBase.cs
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
using Common;
using Semantics.Abstractions;
using Semantics.Components;
using Semantics.Entities;
using Semantics.Utilities;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Entities;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Components
{

    #region Class: RelationshipBase
    /// <summary>
    /// A base of a relationship.
    /// </summary>
    public sealed class RelationshipBase : Base
    {

        #region Properties and Fields

        #region Property: Relationship
        /// <summary>
        /// Gets the relationship of which this is a relationship base.
        /// </summary>
        internal Relationship Relationship
        {
            get
            {
                return this.IdHolder as Relationship;
            }
        }
        #endregion Property: Relationship

        #region Property: Source
        /// <summary>
        /// The source of the relationship.
        /// </summary>
        private EntityBase source = null;
        
        /// <summary>
        /// Gets the source of the relationship.
        /// </summary>
        public EntityBase Source
        {
            get
            {
                return source;
            }
        }
        #endregion Property: Source
        
        #region Property: SourceSpace
        /// <summary>
        /// Gets the space of the source that is used in the relationship; only valid for physical objects.
        /// </summary>
        private SpaceValuedBase sourceSpace = null;
        
        /// <summary>
        /// Gets the space of the source that is used in the relationship; only valid for physical objects.
        /// </summary>
        public SpaceValuedBase SourceSpace
        {
            get
            {
                return sourceSpace;
            }
        }
        #endregion Property: SourceSpace

        #region Property: RelationshipType
        /// <summary>
        /// The relationship type of the relationship.
        /// </summary>
        private RelationshipTypeBase relationshipType = null;
        
        /// <summary>
        /// Gets the relationship type of the relationship.
        /// </summary>
        public RelationshipTypeBase RelationshipType
        {
            get
            {
                return relationshipType;
            }
        }
        #endregion Property: RelationshipType

        #region Property: Targets
        /// <summary>
        /// Gets the targets of the relationship.
        /// </summary>
        private EntityBase[] targets = null;
        
        /// <summary>
        /// Gets the targets of the relationship.
        /// </summary>
        public ReadOnlyCollection<EntityBase> Targets
        {
            get
            {
                if (targets == null)
                {
                    LoadTargets();
                    if (targets == null)
                        return new List<EntityBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<EntityBase>(targets);
            }
        }

        /// <summary>
        /// Loads the targets.
        /// </summary>
        private void LoadTargets()
        {
            if (this.Relationship != null)
            {
                List<EntityBase> entityBases = new List<EntityBase>();
                foreach (Entity entity in this.Relationship.Targets)
                    entityBases.Add(BaseManager.Current.GetBase<EntityBase>(entity));
                targets = entityBases.ToArray();
            }
        }
        #endregion Property: Targets

        #region Property: TargetSpaces
        /// <summary>
        /// Gets the dictionary that stores the space of the target that is used in the relationship; only valid for physical objects.
        /// </summary>
        private Dictionary<EntityBase, SpaceValuedBase> targetSpaces = null;
        
        /// <summary>
        /// Gets the dictionary that stores the space of the target that is used in the relationship; only valid for physical objects.
        /// </summary>
        public Common.ReadOnlyDictionary<EntityBase, SpaceValuedBase> TargetSpaces
        {
            get
            {
                if (targetSpaces == null)
                    LoadTargetSpaces();
                return new Common.ReadOnlyDictionary<EntityBase, SpaceValuedBase>(targetSpaces);
            }
        }

        /// <summary>
        /// Loads the dictionary that stores the space of the target that is used in the relationship.
        /// </summary>
        private void LoadTargetSpaces()
        {
            if (this.Relationship != null)
            {
                targetSpaces = new Dictionary<EntityBase, SpaceValuedBase>();
                Common.ReadOnlyDictionary<Entity, SpaceValued> relationshipTargetspaces = this.Relationship.TargetSpaces;
                foreach (Entity entity in relationshipTargetspaces.Keys)
                {
                    EntityBase entityBase = BaseManager.Current.GetBase<EntityBase>(entity);
                    SpaceValuedBase spaceValuedBase = BaseManager.Current.GetBase<SpaceValuedBase>(relationshipTargetspaces[entity]);
                    targetSpaces.Add(entityBase, spaceValuedBase);
                }
            }
        }
        #endregion Property: TargetSpaces

        #region Property: SourceRequirements
        /// <summary>
        /// The requirements of the source.
        /// </summary>
        private RequirementsBase sourceRequirements = null;

        /// <summary>
        /// Gets the requirements of the source.
        /// </summary>
        public RequirementsBase SourceRequirements
        {
            get
            {
                if (sourceRequirements == null)
                    LoadSourceRequirements();
                return sourceRequirements;
            }
        }

        /// <summary>
        /// Loads the requirements of the source.
        /// </summary>
        private void LoadSourceRequirements()
        {
            if (this.Relationship != null)
                sourceRequirements = BaseManager.Current.GetBase<RequirementsBase>(this.Relationship.SourceRequirements);
        }
        #endregion Property: SourceRequirements

        #region Property: TargetsRequirements
        /// <summary>
        /// The requirements of the targets.
        /// </summary>
        private RequirementsBase targetsRequirements = null;

        /// <summary>
        /// Gets the requirements of the targets.
        /// </summary>
        public RequirementsBase TargetsRequirements
        {
            get
            {
                if (targetsRequirements == null)
                    LoadTargetsRequirements();
                return targetsRequirements;
            }
        }

        /// <summary>
        /// Loads the requirements of the targets.
        /// </summary>
        private void LoadTargetsRequirements()
        {
            if (this.Relationship != null)
                targetsRequirements = BaseManager.Current.GetBase<RequirementsBase>(this.Relationship.TargetsRequirements);
        }
        #endregion Property: TargetsRequirements

        #region Property: SpatialRequirementBetweenSourceAndTargets
        /// <summary>
        /// The spatial condition between the source and targets.
        /// </summary>
        private SpatialRequirementBase spatialRequirementBetweenSourceAndTargets = null;

        /// <summary>
        /// Gets the spatial condition between the source and targets.
        /// </summary>
        public SpatialRequirementBase SpatialRequirementBetweenSourceAndTargets
        {
            get
            {
                return spatialRequirementBetweenSourceAndTargets;
            }
        }
        #endregion Property: SpatialRequirementBetweenSourceAndTargets

        #region Property: Parameters
        /// <summary>
        /// The parameters of the relationship.
        /// </summary>
        private AttributeConditionBase[] parameters = null;
        
        /// <summary>
        /// Gets the parameters of the relationship.
        /// </summary>
        public ReadOnlyCollection<AttributeConditionBase> Parameters
        {
            get
            {
                if (parameters == null)
                {
                    LoadParameters();
                    if (parameters == null)
                        return new List<AttributeConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<AttributeConditionBase>(parameters);
            }
        }

        /// <summary>
        /// Loads the parameters.
        /// </summary>
        private void LoadParameters()
        {
            if (this.Relationship != null)
            {
                List<AttributeConditionBase> attributeConditionBases = new List<AttributeConditionBase>();
                foreach (AttributeCondition parameter in this.Relationship.Parameters)
                    attributeConditionBases.Add(BaseManager.Current.GetBase<AttributeConditionBase>(parameter));
                parameters = attributeConditionBases.ToArray();
            }
        }
        #endregion Property: Parameters

        #region Property: Attributes
        /// <summary>
        /// The attributes of the relationship.
        /// </summary>
        private AttributeValuedBase[] attributes = null;

        /// <summary>
        /// Gets the attributes of the relationship.
        /// </summary>
        public ReadOnlyCollection<AttributeValuedBase> Attributes
        {
            get
            {
                if (attributes == null)
                {
                    LoadAttributes();
                    if (attributes == null)
                        return new List<AttributeValuedBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<AttributeValuedBase>(attributes);
            }
        }

        /// <summary>
        /// Loads the attributes.
        /// </summary>
        private void LoadAttributes()
        {
            if (this.Relationship != null)
            {
                List<AttributeValuedBase> attributeValuedBases = new List<AttributeValuedBase>();
                foreach (AttributeValued attributeValued in this.Relationship.Attributes)
                    attributeValuedBases.Add(BaseManager.Current.GetBase<AttributeValuedBase>(attributeValued));
                attributes = attributeValuedBases.ToArray();
            }
        }
        #endregion Property: Attributes

        #region Property: ConstantAttributes
        /// <summary>
        /// The constant attributes.
        /// </summary>
        private AttributeInstance[] constantAttributes = null;

        /// <summary>
        /// Gets the constant attributes.
        /// </summary>
        internal IEnumerable<AttributeInstance> ConstantAttributes
        {
            get
            {
                if (constantAttributes == null)
                {
                    List<AttributeInstance> attributeInstances = new List<AttributeInstance>();
                    foreach (AttributeValuedBase attributeValuedBase in this.Attributes)
                    {
                        if (attributeValuedBase.AttributeBase.AttributeType == AttributeType.Constant)
                            attributeInstances.Add(InstanceManager.Current.Create<AttributeInstance>(attributeValuedBase));
                    }
                    constantAttributes = attributeInstances.ToArray();
                }

                foreach (AttributeInstance constantAttribute in this.constantAttributes)
                    yield return constantAttribute;
            }
        }
        #endregion Property: ConstantAttributes

        #region Property: Necessity
        /// <summary>
        /// The necessity of the relationship.
        /// </summary>
        private Necessity necessity = default(Necessity);
        
        /// <summary>
        /// Gets the necessity of the relationship.
        /// </summary>
        public Necessity Necessity
        {
            get
            {
                return necessity;
            }
        }
        #endregion Property: Necessity

        #region Property: Priority
        /// <summary>
        /// The priority.
        /// </summary>
        private int priority = SemanticsSettings.Values.Priority;
        
        /// <summary>
        /// Gets the priority.
        /// </summary>
        public int Priority
        {
            get
            {
                return priority;
            }
        }
        #endregion Property: Priority

        #region Property: Cardinality
        /// <summary>
        /// The cardinality of the relationship.
        /// </summary>
        private Cardinality cardinality = default(Cardinality);
        
        /// <summary>
        /// Gets the cardinality of the relationship.
        /// </summary>
        public Cardinality Cardinality
        {
            get
            {
                return cardinality;
            }
        }
        #endregion Property: Cardinality

        #region Property: IsExclusive
        /// <summary>
        /// The value that indicates whether the relationship is exclusive, and cannot be changed after establishment.
        /// </summary>
        private bool isExclusive = SemanticsSettings.Values.IsExclusive;
        
        /// <summary>
        /// Gets the value that indicates whether the relationship is exclusive, and cannot be changed after establishment.
        /// </summary>
        public bool IsExclusive
        {
            get
            {
                return isExclusive;
            }
        }
        #endregion Property: IsExclusive

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
            if (this.Relationship != null)
            {
                foreach (Variable variable in this.Relationship.Variables)
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
            if (this.Relationship != null)
            {
                foreach (Reference reference in this.Relationship.References)
                    referenceBases.Add(BaseManager.Current.GetBase<ReferenceBase>(reference));
            }
            references = referenceBases.ToArray();
        }
        #endregion Property: References

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: RelationshipBase(Relationship relationship)
        /// <summary>
        /// Creates a new relationship base from the given relationship.
        /// </summary>
        /// <param name="relationship">The relationship to create a relationship base from.</param>
        internal RelationshipBase(Relationship relationship)
            : base(relationship)
        {
            if (relationship != null)
            {
                this.source = BaseManager.Current.GetBase<EntityBase>(relationship.Source);
                this.sourceSpace = BaseManager.Current.GetBase<SpaceValuedBase>(relationship.SourceSpace);
                this.spatialRequirementBetweenSourceAndTargets = BaseManager.Current.GetBase<SpatialRequirementBase>(relationship.SpatialRequirementBetweenSourceAndTargets);
                this.relationshipType = BaseManager.Current.GetBase<RelationshipTypeBase>(relationship.RelationshipType);
                this.necessity = relationship.Necessity;
                this.priority = relationship.Priority;
                this.cardinality = relationship.Cardinality;
                this.isExclusive = relationship.IsExclusive;

                if (BaseManager.PreloadProperties)
                {
                    LoadTargetSpaces();
                    LoadSourceRequirements();
                    LoadTargetsRequirements();
                    LoadParameters();
                    LoadAttributes();
                    LoadVariables();
                    LoadReferences();
                }
            }
        }
        #endregion Constructor: RelationshipBase(Relationship relationship)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: ToString()
        /// <summary>
        /// Returns a string representation.
        /// </summary>
        /// <returns>A string representation.</returns>
        public override string ToString()
        {
            if (this.RelationshipType != null)
                return this.RelationshipType.ToString();

            return base.ToString();
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: RelationshipBase

    #region Class: RelationshipConditionBase
    /// <summary>
    /// A base of a relationship condition.
    /// </summary>
    public sealed class RelationshipConditionBase : ConditionBase
    {

        #region Properties and Fields

        #region Property: RelationshipCondition
        /// <summary>
        /// Gets the relationship condition of which this is a relationship condition base.
        /// </summary>
        internal RelationshipCondition RelationshipCondition
        {
            get
            {
                return this.Condition as RelationshipCondition;
            }
        }
        #endregion Property: RelationshipCondition

        #region Property: Source
        /// <summary>
        /// The required source.
        /// </summary>
        private EntityBase source = null;
        
        /// <summary>
        /// Gets the required source.
        /// </summary>
        public EntityBase Source
        {
            get
            {
                return source;
            }
        }
        #endregion Property: Source

        #region Property: SourceSpace
        /// <summary>
        /// The required source space.
        /// </summary>
        private SpaceBase sourceSpace = null;
        
        /// <summary>
        /// Gets the required source space.
        /// </summary>
        public SpaceBase SourceSpace
        {
            get
            {
                return sourceSpace;
            }
        }
        #endregion Property: SourceSpace

        #region Property: RelationshipType
        /// <summary>
        /// The required relationship type.
        /// </summary>
        private RelationshipTypeBase relationshipType = null;
        
        /// <summary>
        /// Gets the required relationship type.
        /// </summary>
        public RelationshipTypeBase RelationshipType
        {
            get
            {
                return relationshipType;
            }
        }
        #endregion Property: RelationshipType

        #region Property: Target
        /// <summary>
        /// The required target.
        /// </summary>
        private EntityBase target = null;
        
        /// <summary>
        /// Gets the required target.
        /// </summary>
        public EntityBase Target
        {
            get
            {
                return target;
            }
        }
        #endregion Property: Target

        #region Property: TargetSpace
        /// <summary>
        /// The required target space.
        /// </summary>
        private SpaceBase targetSpace = null;

        /// <summary>
        /// Gets the required target space.
        /// </summary>
        public SpaceBase TargetSpace
        {
            get
            {
                return targetSpace;
            }
        }
        #endregion Property: TargetSpace

        #region Property: Attributes
        /// <summary>
        /// The required attributes.
        /// </summary>
        private AttributeConditionBase[] attributes = null;

        /// <summary>
        /// Gets the required attributes.
        /// </summary>
        public ReadOnlyCollection<AttributeConditionBase> Attributes
        {
            get
            {
                if (attributes == null)
                {
                    LoadAttributes();
                    if (attributes == null)
                        return new List<AttributeConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<AttributeConditionBase>(attributes);
            }
        }

        /// <summary>
        /// Loads the attributes.
        /// </summary>
        private void LoadAttributes()
        {
            if (this.RelationshipCondition != null)
            {
                List<AttributeConditionBase> attributeConditionBases = new List<AttributeConditionBase>();
                foreach (AttributeCondition attributeCondition in this.RelationshipCondition.Attributes)
                    attributeConditionBases.Add(BaseManager.Current.GetBase<AttributeConditionBase>(attributeCondition));
                attributes = attributeConditionBases.ToArray();
            }
        }
        #endregion Property: Attributes

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: RelationshipConditionBase(RelationshipCondition relationshipCondition)
        /// <summary>
        /// Creates a base of the given relationship condition.
        /// </summary>
        /// <param name="relationshipCondition">The relationship condition to create a base of.</param>
        internal RelationshipConditionBase(RelationshipCondition relationshipCondition)
            : base(relationshipCondition)
        {
            if (relationshipCondition != null)
            {
                this.source = BaseManager.Current.GetBase<EntityBase>(relationshipCondition.Source);
                this.sourceSpace = BaseManager.Current.GetBase<SpaceBase>(relationshipCondition.SourceSpace);
                this.relationshipType = BaseManager.Current.GetBase<RelationshipTypeBase>(relationshipCondition.RelationshipType);
                this.target = BaseManager.Current.GetBase<EntityBase>(relationshipCondition.Target);
                this.targetSpace = BaseManager.Current.GetBase<SpaceBase>(relationshipCondition.TargetSpace);

                if (BaseManager.PreloadProperties)
                    LoadAttributes();
            }
        }
        #endregion Constructor: RelationshipConditionBase(RelationshipCondition relationshipCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: RelationshipConditionBase

    #region Class: RelationshipChangeBase
    /// <summary>
    /// A base of a relationship change.
    /// </summary>
    public sealed class RelationshipChangeBase : ChangeBase
    {

        #region Properties and Fields

        #region Property: RelationshipChange
        /// <summary>
        /// Gets the relationship change of which this is a relationship change base.
        /// </summary>
        internal RelationshipChange RelationshipChange
        {
            get
            {
                return this.Change as RelationshipChange;
            }
        }
        #endregion Property: RelationshipChange

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

        #region Property: Attributes
        /// <summary>
        /// The attributes to change.
        /// </summary>
        private AttributeChangeBase[] attributes = null;

        /// <summary>
        /// Gets the attributes to change.
        /// </summary>
        public ReadOnlyCollection<AttributeChangeBase> Attributes
        {
            get
            {
                if (attributes == null)
                {
                    LoadAttributes();
                    if (attributes == null)
                        return new List<AttributeChangeBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<AttributeChangeBase>(attributes);
            }
        }

        /// <summary>
        /// Loads the attributes.
        /// </summary>
        private void LoadAttributes()
        {
            if (this.RelationshipChange != null)
            {
                List<AttributeChangeBase> attributeChangeBases = new List<AttributeChangeBase>();
                foreach (AttributeChange attributeChange in this.RelationshipChange.Attributes)
                    attributeChangeBases.Add(BaseManager.Current.GetBase<AttributeChangeBase>(attributeChange));
                attributes = attributeChangeBases.ToArray();
            }
        }
        #endregion Property: Attributes

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: RelationshipChangeBase(RelationshipChange relationshipChange)
        /// <summary>
        /// Creates a base of the given relationship change.
        /// </summary>
        /// <param name="relationshipChange">The relationship change to create a base of.</param>
        internal RelationshipChangeBase(RelationshipChange relationshipChange)
            : base(relationshipChange)
        {
            if (relationshipChange != null)
            {
                this.relationshipType = BaseManager.Current.GetBase<RelationshipTypeBase>(relationshipChange.RelationshipType);

                if (BaseManager.PreloadProperties)
                    LoadAttributes();
            }
        }
        #endregion Constructor: RelationshipChangeBase(RelationshipChange relationshipChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: RelationshipChangeBase

    #region Class: CombinedRelationshipBase
    /// <summary>
    /// A base of a combined relationship.
    /// </summary>
    public sealed class CombinedRelationshipBase : Base
    {

        #region Properties and Fields

        #region Property: CombinedRelationship
        /// <summary>
        /// Gets the combined relationship of which this is a combined relationship base.
        /// </summary>
        internal CombinedRelationship CombinedRelationship
        {
            get
            {
                return this.IdHolder as CombinedRelationship;
            }
        }
        #endregion Property: CombinedRelationship

        #region Property: Relationship1
        /// <summary>
        /// The first relationship of the combined relationship. Make sure to choose between Relationship1 and CombinedRelationship1!
        /// </summary>
        private RelationshipBase relationship1 = null;

        /// <summary>
        /// Gets the first relationship of the combined relationship. Make sure to choose between Relationship1 and CombinedRelationship1!
        /// </summary>
        public RelationshipBase Relationship1
        {
            get
            {
                return relationship1;
            }
        }
        #endregion Property: Relationship1

        #region Property: CombinedRelationship1
        /// <summary>
        /// The first relationship of the combined relationship. Make sure to choose between Relationship1 and CombinedRelationship1!
        /// </summary>
        private CombinedRelationshipBase combinedRelationship1 = null;

        /// <summary>
        /// Gets the first relationship of the combined relationship. Make sure to choose between Relationship1 and CombinedRelationship1!
        /// </summary>
        public CombinedRelationshipBase CombinedRelationship1
        {
            get
            {
                return combinedRelationship1;
            }
        }
        #endregion Property: CombinedRelationship1

        #region Property: Operator
        /// <summary>
        /// The logical operator of the combined relationship.
        /// </summary>
        private LogicalOperator logicalOperator = default(LogicalOperator);

        /// <summary>
        /// Gets the logical operator of the combined relationship.
        /// </summary>
        public LogicalOperator Operator
        {
            get
            {
                return logicalOperator;
            }
        }
        #endregion Property: Operator

        #region Property: Relationship2
        /// <summary>
        /// The second relationship of the combined relationship. Make sure to choose between Relationship2 and CombinedRelationship2!
        /// </summary>
        private RelationshipBase relationship2 = null;

        /// <summary>
        /// Gets the second relationship of the combined relationship. Make sure to choose between Relationship2 and CombinedRelationship2!
        /// </summary>
        public RelationshipBase Relationship2
        {
            get
            {
                return relationship2;
            }
        }
        #endregion Property: Relationship2

        #region Property: CombinedRelationship2
        /// <summary>
        /// The second relationship of the combined relationship. Make sure to choose between Relationship2 and CombinedRelationship2!
        /// </summary>
        private CombinedRelationshipBase combinedRelationship2 = null;

        /// <summary>
        /// Gets the second relationship of the combined relationship. Make sure to choose between Relationship2 and CombinedRelationship2!
        /// </summary>
        public CombinedRelationshipBase CombinedRelationship2
        {
            get
            {
                return combinedRelationship2;
            }
        }
        #endregion Property: CombinedRelationship2

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: CombinedRelationshipBase(CombinedRelationship combinedRelationship)
        /// <summary>
        /// Creates a new combined relationship base from the given combined relationship.
        /// </summary>
        /// <param name="combinedRelationship">The combined relationship to create a combined relationship base from.</param>
        internal CombinedRelationshipBase(CombinedRelationship combinedRelationship)
            : base(combinedRelationship)
        {
            if (combinedRelationship != null)
            {
                this.relationship1 = BaseManager.Current.GetBase<RelationshipBase>(combinedRelationship.Relationship1);
                this.combinedRelationship1 = BaseManager.Current.GetBase<CombinedRelationshipBase>(combinedRelationship.CombinedRelationship1);
                this.logicalOperator = combinedRelationship.Operator;
                this.relationship2 = BaseManager.Current.GetBase<RelationshipBase>(combinedRelationship.Relationship2);
                this.combinedRelationship2 = BaseManager.Current.GetBase<CombinedRelationshipBase>(combinedRelationship.CombinedRelationship2);
            }
        }
        #endregion Constructor: CombinedRelationshipBase(CombinedRelationship combinedRelationship)

        #endregion Method Group: Constructors

    }
    #endregion Class: CombinedRelationshipBase

}