/**************************************************************************
 * 
 * EntityBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2009-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Semantics.Abstractions;
using Semantics.Components;
using Semantics.Entities;
using Semantics.Utilities;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Components;
using SemanticsEngine.Tools;
using Action = Semantics.Abstractions.Action;

namespace SemanticsEngine.Entities
{

    #region Class: EntityBase
    /// <summary>
    /// A base of an entity.
    /// </summary>
    public abstract class EntityBase : NodeBase
    {

        #region Properties and Fields

        #region Property: Entity
        /// <summary>
        /// Gets the entity of which this is an entity base.
        /// </summary>
        protected internal Entity Entity
        {
            get
            {
                return this.IdHolder as Entity;
            }
        }
        #endregion Property: Entity

        #region Property: Attributes
        /// <summary>
        /// The attributes.
        /// </summary>
        private AttributeValuedBase[] attributes = null;

        /// <summary>
        /// Gets the attributes.
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
            if (this.Entity != null)
            {
                List<AttributeValuedBase> attributeValuedBases = new List<AttributeValuedBase>();
                foreach (AttributeValued attributeValued in this.Entity.Attributes)
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

        #region Property: StateGroups
        /// <summary>
        /// The state groups.
        /// </summary>
        private StateGroupBase[] stateGroups = null;

        /// <summary>
        /// Gets the state groups.
        /// </summary>
        public ReadOnlyCollection<StateGroupBase> StateGroups
        {
            get
            {
                if (stateGroups == null)
                {
                    LoadStateGroups();
                    if (stateGroups == null)
                        return new List<StateGroupBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<StateGroupBase>(stateGroups);
            }
        }

        /// <summary>
        /// Loads the state groups.
        /// </summary>
        private void LoadStateGroups()
        {
            if (this.Entity != null)
            {
                List<StateGroupBase> stateGroupBases = new List<StateGroupBase>();
                foreach (StateGroup stateGroup in this.Entity.StateGroups)
                    stateGroupBases.Add(BaseManager.Current.GetBase<StateGroupBase>(stateGroup));
                stateGroups = stateGroupBases.ToArray();
            }
        }
        #endregion Property: StateGroups

        #region Property: Actions
        /// <summary>
        /// The actions that this entity can perform.
        /// </summary>
        private ActionBase[] actions = null;

        /// <summary>
        /// Gets the actions that this entity can perform.
        /// </summary>
        public ReadOnlyCollection<ActionBase> Actions
        {
            get
            {
                if (actions == null)
                {
                    List<ActionBase> actionBases = new List<ActionBase>();
                    foreach (EventBase eventBase in this.ManualEvents)
                    {
                        if (!actionBases.Contains(eventBase.Action))
                            actionBases.Add(eventBase.Action);
                    }
                    actions = actionBases.ToArray();
                }
                return new ReadOnlyCollection<ActionBase>(actions);
            }
        }
        #endregion Property: Actions

        #region Property: Events
        /// <summary>
        /// Gets all the events of the entity base.
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
        /// The automatic events of the entity base.
        /// </summary>
        private EventBase[] automaticEvents = null;

        /// <summary>
        /// Gets the automatic events of the entity base.
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
            if (this.Entity != null)
            {
                List<EventBase> eventBases = new List<EventBase>();

                foreach (Event even in this.Entity.Events)
                {
                    if (even.Behavior == EventBehavior.Automatic)
                        eventBases.Add(BaseManager.Current.GetBase<EventBase>(even));
                }
                automaticEvents = eventBases.ToArray();
            }
        }
        #endregion Property: AutomaticEvents

        #region Property: ManualEvents
        /// <summary>
        /// The manual events of the entity base.
        /// </summary>
        private EventBase[] manualEvents = null;

        /// <summary>
        /// Gets the manual events of the entity base.
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
            if (this.Entity != null)
            {
                List<EventBase> eventBases = new List<EventBase>();

                foreach (Event even in this.Entity.Events)
                {
                    if (even.Behavior == EventBehavior.Manual)
                        eventBases.Add(BaseManager.Current.GetBase<EventBase>(even));
                }
                manualEvents = eventBases.ToArray();
            }
        }
        #endregion Property: ManualEvents

        #region Property: PredicatesAsActor
        /// <summary>
        /// All predicates where this entity is the actor.
        /// </summary>
        private PredicateBase[] predicatesAsActor = null;

        /// <summary>
        /// Gets all predicates where this entity is the actor.
        /// </summary>
        public ReadOnlyCollection<PredicateBase> PredicatesAsActor
        {
            get
            {
                if (predicatesAsActor == null)
                {
                    LoadPredicatesAsActor();
                    if (predicatesAsActor == null)
                        return new List<PredicateBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<PredicateBase>(predicatesAsActor);
            }
        }

        /// <summary>
        /// Loads the predicates where this entity is the actor.
        /// </summary>
        private void LoadPredicatesAsActor()
        {
            if (this.Entity != null)
            {
                List<PredicateBase> entityPredicates = new List<PredicateBase>();
                foreach (Predicate predicate in this.Entity.PredicatesAsActor)
                    entityPredicates.Add(BaseManager.Current.GetBase<PredicateBase>(predicate));
                predicatesAsActor = entityPredicates.ToArray();
            }
        }
        #endregion Property: PredicatesAsActor

        #region Property: PredicatesAsTarget
        /// <summary>
        /// All predicates where this entity is the target.
        /// </summary>
        private PredicateBase[] predicatesAsTarget = null;

        /// <summary>
        /// Gets all predicates where this entity is the target.
        /// </summary>
        public ReadOnlyCollection<PredicateBase> PredicatesAsTarget
        {
            get
            {
                if (predicatesAsTarget == null)
                {
                    LoadPredicatesAsTarget();
                    if (predicatesAsTarget == null)
                        return new List<PredicateBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<PredicateBase>(predicatesAsTarget);
            }
        }

        /// <summary>
        /// Loads the predicates where this entity is the target.
        /// </summary>
        private void LoadPredicatesAsTarget()
        {
            if (this.Entity != null)
            {
                List<PredicateBase> entityPredicates = new List<PredicateBase>();
                foreach (Predicate predicate in this.Entity.PredicatesAsTarget)
                    entityPredicates.Add(BaseManager.Current.GetBase<PredicateBase>(predicate));
                predicatesAsTarget = entityPredicates.ToArray();
            }
        }
        #endregion Property: PredicatesAsTarget

        #region Property: RelationshipsAsSource
        /// <summary>
        /// All relationships where this entity is the source.
        /// </summary>
        private RelationshipBase[] relationshipsAsSource = null;

        /// <summary>
        /// Gets all relationships where this entity is the source.
        /// </summary>
        public ReadOnlyCollection<RelationshipBase> RelationshipsAsSource
        {
            get
            {
                if (relationshipsAsSource == null)
                {
                    LoadRelationshipsAsSource();
                    if (relationshipsAsSource == null)
                        return new List<RelationshipBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<RelationshipBase>(relationshipsAsSource);
            }
        }

        /// <summary>
        /// Loads the relationships where this entity is the source.
        /// </summary>
        private void LoadRelationshipsAsSource()
        {
            if (this.Entity != null)
            {
                List<RelationshipBase> entityRelationships = new List<RelationshipBase>();
                foreach (Relationship relationship in this.Entity.RelationshipsAsSource)
                    entityRelationships.Add(BaseManager.Current.GetBase<RelationshipBase>(relationship));
                relationshipsAsSource = entityRelationships.ToArray();
            }
        }
        #endregion Property: RelationshipsAsSource

        #region Property: RelationshipsAsTarget
        /// <summary>
        /// All relationships where this entity is the target.
        /// </summary>
        private RelationshipBase[] relationshipsAsTarget = null;

        /// <summary>
        /// Gets all relationships where this entity is the target.
        /// </summary>
        public ReadOnlyCollection<RelationshipBase> RelationshipsAsTarget
        {
            get
            {
                if (relationshipsAsTarget == null)
                {
                    LoadRelationshipsAsTarget();
                    if (relationshipsAsTarget == null)
                        return new List<RelationshipBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<RelationshipBase>(relationshipsAsTarget);
            }
        }

        /// <summary>
        /// Loads the relationships where this entity is the target.
        /// </summary>
        private void LoadRelationshipsAsTarget()
        {
            if (this.Entity != null)
            {
                List<RelationshipBase> entityRelationships = new List<RelationshipBase>();
                foreach (Relationship relationship in this.Entity.RelationshipsAsTarget)
                    entityRelationships.Add(BaseManager.Current.GetBase<RelationshipBase>(relationship));
                relationshipsAsTarget = entityRelationships.ToArray();
            }
        }
        #endregion Property: RelationshipsAsTarget

        #region Property: CombinedRelationships
        /// <summary>
        /// The combined relationships.
        /// </summary>
        private CombinedRelationshipBase[] combinedRelationships = null;

        /// <summary>
        /// Gets the combined relationships.
        /// </summary>
        public ReadOnlyCollection<CombinedRelationshipBase> CombinedRelationships
        {
            get
            {
                if (combinedRelationships == null)
                {
                    LoadCombinedRelationships();
                    if (combinedRelationships == null)
                        return new List<CombinedRelationshipBase>(0).AsReadOnly();
                }

                return new ReadOnlyCollection<CombinedRelationshipBase>(combinedRelationships);
            }
        }

        /// <summary>
        /// Loads the combined relationships.
        /// </summary>
        private void LoadCombinedRelationships()
        {
            List<CombinedRelationshipBase> combinedRelationshipBases = new List<CombinedRelationshipBase>();
            if (this.Entity != null)
            {
                foreach (CombinedRelationship combinedRelationship in this.Entity.CombinedRelationships)
                    combinedRelationshipBases.Add(BaseManager.Current.GetBase<CombinedRelationshipBase>(combinedRelationship));
            }
            combinedRelationships = combinedRelationshipBases.ToArray();
        }
        #endregion Property: CombinedRelationships

        #region Property: Necessity
        /// <summary>
        /// The necessity.
        /// </summary>
        private Necessity necessity = default(Necessity);

        /// <summary>
        /// Gets the necessity.
        /// </summary>
        public Necessity Necessity
        {
            get
            {
                return necessity;
            }
        }
        #endregion Property: Necessity

        #region Property: PossibleContextTypes
        /// <summary>
        /// The possible context types.
        /// </summary>
        private ContextTypeBase[] possibleContextTypes = null;

        /// <summary>
        /// Gets the possible context types.
        /// </summary>
        internal ReadOnlyCollection<ContextTypeBase> PossibleContextTypes
        {
            get
            {
                if (possibleContextTypes == null)
                {
                    LoadPossibleContextTypes();
                    if (possibleContextTypes == null)
                        return new List<ContextTypeBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<ContextTypeBase>(possibleContextTypes);
            }
        }

        /// <summary>
        /// Loads the possible context types.
        /// </summary>
        private void LoadPossibleContextTypes()
        {
            List<ContextTypeBase> contextTypeBases = new List<ContextTypeBase>();
            NodeModel nodeModel = SemanticsManager.GetNodeModel<ContextType>();
            if (nodeModel != null)
            {
                foreach (Node node in nodeModel.GetNodes())
                {
                    ContextType contextType = node as ContextType;
                    if (contextType != null)
                    {
                        ContextTypeBase contextTypeBase = BaseManager.Current.GetBase<ContextTypeBase>(contextType);
                        foreach (ContextBase contextBase in contextTypeBase.Contexts)
                        {
                            if (contextBase.Subject == null || this.IsNodeOf(contextBase.Subject))
                            {
                                contextTypeBases.Add(contextTypeBase);
                                break;
                            }
                        }
                    }
                }
            }
            possibleContextTypes = contextTypeBases.ToArray();
        }
        #endregion Property: PossibleContextTypes

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
            if (this.Entity != null)
            {
                foreach (Variable variable in this.Entity.Variables)
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
            if (this.Entity != null)
            {
                foreach (Reference reference in this.Entity.References)
                    referenceBases.Add(BaseManager.Current.GetBase<ReferenceBase>(reference));
            }
            references = referenceBases.ToArray();
        }
        #endregion Property: References

        #region Property: AbstractEntities
        /// <summary>
        /// The abstract entities.
        /// </summary>
        private AbstractEntityValuedBase[] abstractEntities = null;

        /// <summary>
        /// Gets the abstract entities.
        /// </summary>
        public ReadOnlyCollection<AbstractEntityValuedBase> AbstractEntities
        {
            get
            {
                if (abstractEntities == null)
                {
                    LoadAbstractEntities();
                    if (abstractEntities == null)
                        return new List<AbstractEntityValuedBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<AbstractEntityValuedBase>(abstractEntities);
            }
        }

        /// <summary>
        /// Loads the abstract entities.
        /// </summary>
        private void LoadAbstractEntities()
        {
            if (this.Entity != null)
            {
                List<AbstractEntityValuedBase> abstractEntityValuedBases = new List<AbstractEntityValuedBase>();
                foreach (AbstractEntityValued abstractEntityValued in this.Entity.AbstractEntities)
                    abstractEntityValuedBases.Add(BaseManager.Current.GetBase<AbstractEntityValuedBase>(abstractEntityValued));
                abstractEntities = abstractEntityValuedBases.ToArray();
            }
        }
        #endregion Property: AbstractEntities

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: EntityBase(Entity entity)
        /// <summary>
        /// Creates a new entity base from the given entity.
        /// </summary>
        /// <param name="entity">The entity to create the entity base from.</param>
        protected EntityBase(Entity entity)
            : base(entity)
        {
            Initialize();
        }
        #endregion Constructor: EntityBase(Entity entity)

        #region Constructor: EntityBase(EntityValued entityValued)
        /// <summary>
        /// Creates a new entity base from the given valued entity.
        /// </summary>
        /// <param name="entityValued">The valued entity to create the entity base from.</param>
        protected EntityBase(EntityValued entityValued)
            : base(entityValued)
        {
            if (entityValued != null)
            {
                this.necessity = entityValued.Necessity;

                Initialize();
            }
        }
        #endregion Constructor: EntityBase(EntityValued entityValued)

        #region Method: Initialize()
        /// <summary>
        /// Initialize.
        /// </summary>
        private void Initialize()
        {
            if (BaseManager.PreloadProperties)
            {
                LoadAttributes();
                LoadManualEvents();
                LoadAutomaticEvents();
                LoadPredicatesAsActor();
                LoadPredicatesAsTarget();
                LoadStateGroups();
                LoadRelationshipsAsSource();
                LoadRelationshipsAsTarget();
                LoadCombinedRelationships();
                LoadVariables();
                LoadReferences();
                LoadAbstractEntities();
            }
        }
        #endregion Method: Initialize()
		
        #endregion Method Group: Constructors

    }
    #endregion Class: EntityBase

    #region Class: EntityValuedBase
    /// <summary>
    /// A base of a valued entity.
    /// </summary>
    public abstract class EntityValuedBase : NodeValuedBase
    {

        #region Properties and Fields

        #region Property: EntityValued
        /// <summary>
        /// Gets the valued entity of which this is an entity base.
        /// </summary>
        protected internal EntityValued EntityValued
        {
            get
            {
                return this.NodeValued as EntityValued;
            }
        }
        #endregion Property: EntityValued

        #region Property: EntityBase
        /// <summary>
        /// Gets the entity of which this is a valued entity base.
        /// </summary>
        public EntityBase EntityBase
        {
            get
            {
                return this.NodeBase as EntityBase;
            }
        }
        #endregion Property: EntityBase

        #region Property: Quantity
        /// <summary>
        /// The quantity.
        /// </summary>
        private NumericalValueRangeBase quantity = null;

        /// <summary>
        /// Gets the quantity.
        /// </summary>
        public NumericalValueRangeBase Quantity
        {
            get
            {
                if (quantity == null)
                {
                    LoadQuantity();
                    if (quantity == null)
                        quantity = new NumericalValueRangeBase(SemanticsSettings.Values.Quantity);
                    if (quantity.Unit == null)
                    {
                        UnitCategoryBase quantityUnitCategoryBase = Utils.GetSpecialUnitCategory(SpecialUnitCategories.Quantity);
                        if (quantityUnitCategoryBase != null)
                            quantity.Unit = quantityUnitCategoryBase.BaseUnit;
                    }
                }
                return quantity;
            }
        }

        /// <summary>
        /// Loads the quantity.
        /// </summary>
        private void LoadQuantity()
        {
            if (this.EntityValued != null)
                quantity = BaseManager.Current.GetBase<NumericalValueRangeBase>(this.EntityValued.Quantity);
        }
        #endregion Property: Quantity

        #region Property: Necessity
        /// <summary>
        /// The necessity.
        /// </summary>
        private Necessity necessity = default(Necessity);
        
        /// <summary>
        /// Gets the necessity.
        /// </summary>
        public Necessity Necessity
        {
            get
            {
                return necessity;
            }
        }
        #endregion Property: Necessity

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: EntityValuedBase(EntityValued entityValued)
        /// <summary>
        /// Creates a new valued entity base from the given valued entity.
        /// </summary>
        /// <param name="entityValued">The valued entity to create a valued entity base from.</param>
        protected EntityValuedBase(EntityValued entityValued)
            : base(entityValued)
        {
            if (entityValued != null)
            {
                this.necessity = entityValued.Necessity;

                if (BaseManager.PreloadProperties)
                    LoadQuantity();
            }
        }
        #endregion Constructor: EntityValuedBase(EntityValued entityValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: EntityValuedBase

    #region Class: EntityConditionBase
    /// <summary>
    /// A condition on an entity.
    /// </summary>
    public abstract class EntityConditionBase : NodeConditionBase
    {

        #region Properties and Fields

        #region Property: EntityCondition
        /// <summary>
        /// Gets the entity condition of which this is an entity condition base.
        /// </summary>
        protected internal EntityCondition EntityCondition
        {
            get
            {
                return this.Condition as EntityCondition;
            }
        }
        #endregion Property: EntityCondition

        #region Property: EntityBase
        /// <summary>
        /// Gets the entity base of which this is an entity condition base.
        /// </summary>
        public EntityBase EntityBase
        {
            get
            {
                return this.NodeBase as EntityBase;
            }
        }
        #endregion Property: EntityBase

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
            if (this.EntityCondition != null)
            {
                List<AttributeConditionBase> attributeConditionBases = new List<AttributeConditionBase>();
                foreach (AttributeCondition attributeCondition in this.EntityCondition.Attributes)
                    attributeConditionBases.Add(BaseManager.Current.GetBase<AttributeConditionBase>(attributeCondition));
                attributes = attributeConditionBases.ToArray();
            }
        }
        #endregion Property: Attributes

        #region Property: StateGroups
        /// <summary>
        /// The required state groups.
        /// </summary>
        private StateGroupConditionBase[] stateGroups = null;

        /// <summary>
        /// Gets the required state groups.
        /// </summary>
        public ReadOnlyCollection<StateGroupConditionBase> StateGroups
        {
            get
            {
                if (stateGroups == null)
                {
                    LoadStateGroups();
                    if (stateGroups == null)
                        return new List<StateGroupConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<StateGroupConditionBase>(stateGroups);
            }
        }

        /// <summary>
        /// Loads the state groups.
        /// </summary>
        private void LoadStateGroups()
        {
            if (this.EntityCondition != null)
            {
                List<StateGroupConditionBase> stateGroupConditionBases = new List<StateGroupConditionBase>();
                foreach (StateGroupCondition stateGroupCondition in this.EntityCondition.StateGroups)
                    stateGroupConditionBases.Add(BaseManager.Current.GetBase<StateGroupConditionBase>(stateGroupCondition));
                stateGroups = stateGroupConditionBases.ToArray();
            }
        }
        #endregion Property: StateGroups

        #region Property: Relationships
        /// <summary>
        /// The required relationships.
        /// </summary>
        private RelationshipConditionBase[] relationships = null;

        /// <summary>
        /// Gets the required relationships.
        /// </summary>
        public ReadOnlyCollection<RelationshipConditionBase> Relationships
        {
            get
            {
                if (relationships == null)
                {
                    LoadRelationships();
                    if (relationships == null)
                        return new List<RelationshipConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<RelationshipConditionBase>(relationships);
            }
        }

        /// <summary>
        /// Loads the relationships.
        /// </summary>
        private void LoadRelationships()
        {
            if (this.EntityCondition != null)
            {
                List<RelationshipConditionBase> relationshipConditionBases = new List<RelationshipConditionBase>();
                foreach (RelationshipCondition relationshipCondition in this.EntityCondition.Relationships)
                    relationshipConditionBases.Add(BaseManager.Current.GetBase<RelationshipConditionBase>(relationshipCondition));
                relationships = relationshipConditionBases.ToArray();
            }
        }
        #endregion Property: Relationships

        #region Property: Actions
        /// <summary>
        /// The actions that this entity should be able to perform.
        /// </summary>
        private ActionBase[] actions = null;

        /// <summary>
        /// Gets the actions that this entity should be able to perform.
        /// </summary>
        public ReadOnlyCollection<ActionBase> Actions
        {
            get
            {
                if (actions == null)
                {
                    LoadActions();
                    if (actions == null)
                        return new List<ActionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<ActionBase>(actions);
            }
        }

        /// <summary>
        /// Loads the actions.
        /// </summary>
        private void LoadActions()
        {
            if (this.EntityCondition != null)
            {
                List<ActionBase> actionBases = new List<ActionBase>();
                foreach (Action action in this.EntityCondition.Actions)
                    actionBases.Add(BaseManager.Current.GetBase<ActionBase>(action));
                actions = actionBases.ToArray();
            }
        }
        #endregion Property: Actions

        #region Property: AbstractEntities
        /// <summary>
        /// The required abstract entities.
        /// </summary>
        private AbstractEntityConditionBase[] abstractEntities = null;

        /// <summary>
        /// Gets the required abstract entities.
        /// </summary>
        public ReadOnlyCollection<AbstractEntityConditionBase> AbstractEntities
        {
            get
            {
                if (abstractEntities == null)
                {
                    LoadAbstractEntities();
                    if (abstractEntities == null)
                        return new List<AbstractEntityConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<AbstractEntityConditionBase>(abstractEntities);
            }
        }

        /// <summary>
        /// Loads the abstract entities.
        /// </summary>
        private void LoadAbstractEntities()
        {
            if (this.EntityCondition != null)
            {
                List<AbstractEntityConditionBase> abstractEntityConditionBases = new List<AbstractEntityConditionBase>();
                foreach (AbstractEntityCondition abstractEntityCondition in this.EntityCondition.AbstractEntities)
                    abstractEntityConditionBases.Add(BaseManager.Current.GetBase<AbstractEntityConditionBase>(abstractEntityCondition));
                abstractEntities = abstractEntityConditionBases.ToArray();
            }
        }
        #endregion Property: AbstractEntities

        #region Property: Quantity
        /// <summary>
        /// The required quantity.
        /// </summary>
        private NumericalValueConditionBase quantity = null;

        /// <summary>
        /// Gets the required quantity.
        /// </summary>
        public NumericalValueConditionBase Quantity
        {
            get
            {
                if (quantity == null)
                    LoadQuantity();
                return quantity;
            }
        }

        /// <summary>
        /// Loads the quantity.
        /// </summary>
        private void LoadQuantity()
        {
            if (this.EntityCondition != null)
                quantity = BaseManager.Current.GetBase<NumericalValueConditionBase>(this.EntityCondition.Quantity);
        }
        #endregion Property: Quantity

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: EntityConditionBase(EntityCondition entityCondition)
        /// <summary>
        /// Creates a base of the given entity condition.
        /// </summary>
        /// <param name="entityCondition">The entity condition to create a base of.</param>
        protected EntityConditionBase(EntityCondition entityCondition)
            : base(entityCondition)
        {
            if (entityCondition != null)
            {
                if (BaseManager.PreloadProperties)
                {
                    LoadQuantity();
                    LoadAttributes();
                    LoadActions();
                    LoadStateGroups();
                    LoadRelationships();
                    LoadAbstractEntities();
                }
            }
        }
        #endregion Constructor: EntityConditionBase(EntityCondition entityCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: EntityConditionBase

    #region Class: EntityChangeBase
    /// <summary>
    /// A change on an entity.
    /// </summary>
    public abstract class EntityChangeBase : NodeChangeBase
    {

        #region Properties and Fields

        #region Property: EntityChange
        /// <summary>
        /// Gets the entity change of which this is an entity change base.
        /// </summary>
        protected internal EntityChange EntityChange
        {
            get
            {
                return this.Change as EntityChange;
            }
        }
        #endregion Property: EntityChange

        #region Property: EntityBase
        /// <summary>
        /// Gets the affected entity base.
        /// </summary>
        public EntityBase EntityBase
        {
            get
            {
                return this.NodeBase as EntityBase;
            }
        }
        #endregion Property: EntityBase

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
            if (this.EntityChange != null)
            {
                List<AttributeChangeBase> attributeChangeBases = new List<AttributeChangeBase>();
                foreach (AttributeChange attributeChange in this.EntityChange.Attributes)
                    attributeChangeBases.Add(BaseManager.Current.GetBase<AttributeChangeBase>(attributeChange));
                attributes = attributeChangeBases.ToArray();
            }
        }
        #endregion Property: Attributes

        #region Property: AttributesToAdd
        /// <summary>
        /// The attributes that should be added during the change.
        /// </summary>
        private AttributeValuedBase[] attributesToAdd = null;

        /// <summary>
        /// Gets the attributes that should be added during the change.
        /// </summary>
        public ReadOnlyCollection<AttributeValuedBase> AttributesToAdd
        {
            get
            {
                if (attributesToAdd == null)
                {
                    LoadAttributesToAdd();
                    if (attributesToAdd == null)
                        return new List<AttributeValuedBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<AttributeValuedBase>(attributesToAdd);
            }
        }

        /// <summary>
        /// Loads the attributes to add.
        /// </summary>
        private void LoadAttributesToAdd()
        {
            if (this.EntityChange != null)
            {
                List<AttributeValuedBase> attributeValuedBases = new List<AttributeValuedBase>();
                foreach (AttributeValued attributeValued in this.EntityChange.AttributesToAdd)
                    attributeValuedBases.Add(BaseManager.Current.GetBase<AttributeValuedBase>(attributeValued));
                attributesToAdd = attributeValuedBases.ToArray();
            }
        }
        #endregion Property: AttributesToAdd

        #region Property: AttributesToRemove
        /// <summary>
        /// The attributes that should be removed during the change.
        /// </summary>
        private AttributeConditionBase[] attributesToRemove = null;

        /// <summary>
        /// Gets the attributes that should be removed during the change.
        /// </summary>
        public ReadOnlyCollection<AttributeConditionBase> AttributesToRemove
        {
            get
            {
                if (attributesToRemove == null)
                {
                    LoadAttributesToRemove();
                    if (attributesToRemove == null)
                        return new List<AttributeConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<AttributeConditionBase>(attributesToRemove);
            }
        }

        /// <summary>
        /// Loads the attributes to remove.
        /// </summary>
        private void LoadAttributesToRemove()
        {
            if (this.EntityChange != null)
            {
                List<AttributeConditionBase> attributeConditionBases = new List<AttributeConditionBase>();
                foreach (AttributeCondition attributeCondition in this.EntityChange.AttributesToRemove)
                    attributeConditionBases.Add(BaseManager.Current.GetBase<AttributeConditionBase>(attributeCondition));
                attributesToRemove = attributeConditionBases.ToArray();
            }
        }
        #endregion Property: AttributesToRemove

        #region Property: StateGroups
        /// <summary>
        /// The state groups to change.
        /// </summary>
        private StateGroupChangeBase[] stateGroups = null;

        /// <summary>
        /// Gets the state groups to change.
        /// </summary>
        public ReadOnlyCollection<StateGroupChangeBase> StateGroups
        {
            get
            {
                if (stateGroups == null)
                {
                    LoadStateGroups();
                    if (stateGroups == null)
                        return new List<StateGroupChangeBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<StateGroupChangeBase>(stateGroups);
            }
        }

        /// <summary>
        /// Loads the state groups.
        /// </summary>
        private void LoadStateGroups()
        {
            if (this.EntityChange != null)
            {
                List<StateGroupChangeBase> stateGroupChangeBases = new List<StateGroupChangeBase>();
                foreach (StateGroupChange stateGroupChange in this.EntityChange.StateGroups)
                    stateGroupChangeBases.Add(BaseManager.Current.GetBase<StateGroupChangeBase>(stateGroupChange));
                stateGroups = stateGroupChangeBases.ToArray();
            }
        }
        #endregion Property: StateGroups

        #region Property: Relationships
        /// <summary>
        /// The required relationships.
        /// </summary>
        private RelationshipChangeBase[] relationships = null;

        /// <summary>
        /// Gets the required relationships.
        /// </summary>
        public ReadOnlyCollection<RelationshipChangeBase> Relationships
        {
            get
            {
                if (relationships == null)
                {
                    LoadRelationships();
                    if (relationships == null)
                        return new List<RelationshipChangeBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<RelationshipChangeBase>(relationships);
            }
        }

        /// <summary>
        /// Loads the relationships.
        /// </summary>
        private void LoadRelationships()
        {
            if (this.EntityChange != null)
            {
                List<RelationshipChangeBase> relationshipChangeBases = new List<RelationshipChangeBase>();
                foreach (RelationshipChange relationshipChange in this.EntityChange.Relationships)
                    relationshipChangeBases.Add(BaseManager.Current.GetBase<RelationshipChangeBase>(relationshipChange));
                relationships = relationshipChangeBases.ToArray();
            }
        }
        #endregion Property: Relationships

        #region Property: AbstractEntities
        /// <summary>
        /// The abstract entities to change.
        /// </summary>
        private AbstractEntityChangeBase[] abstractEntities = null;

        /// <summary>
        /// Gets the abstract entities to change.
        /// </summary>
        public ReadOnlyCollection<AbstractEntityChangeBase> AbstractEntities
        {
            get
            {
                if (abstractEntities == null)
                {
                    LoadAbstractEntities();
                    if (abstractEntities == null)
                        return new List<AbstractEntityChangeBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<AbstractEntityChangeBase>(abstractEntities);
            }
        }

        /// <summary>
        /// Loads the abstractEntities.
        /// </summary>
        private void LoadAbstractEntities()
        {
            if (this.EntityChange != null)
            {
                List<AbstractEntityChangeBase> abstractEntityChangeBases = new List<AbstractEntityChangeBase>();
                foreach (AbstractEntityChange abstractEntityChange in this.EntityChange.AbstractEntities)
                    abstractEntityChangeBases.Add(BaseManager.Current.GetBase<AbstractEntityChangeBase>(abstractEntityChange));
                abstractEntities = abstractEntityChangeBases.ToArray();
            }
        }
        #endregion Property: AbstractEntities

        #region Property: AbstractEntitiesToAdd
        /// <summary>
        /// The abstract entities that should be added during the change.
        /// </summary>
        private AbstractEntityValuedBase[] abstractEntitiesToAdd = null;

        /// <summary>
        /// Gets the abstract entities that should be added during the change.
        /// </summary>
        public ReadOnlyCollection<AbstractEntityValuedBase> AbstractEntitiesToAdd
        {
            get
            {
                if (abstractEntitiesToAdd == null)
                {
                    LoadAbstractEntitiesToAdd();
                    if (abstractEntitiesToAdd == null)
                        return new List<AbstractEntityValuedBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<AbstractEntityValuedBase>(abstractEntitiesToAdd);
            }
        }

        /// <summary>
        /// Loads the abstract entities to add.
        /// </summary>
        private void LoadAbstractEntitiesToAdd()
        {
            if (this.EntityChange != null)
            {
                List<AbstractEntityValuedBase> abstractEntityValuedBases = new List<AbstractEntityValuedBase>();
                foreach (AbstractEntityValued abstractEntityValued in this.EntityChange.AbstractEntitiesToAdd)
                    abstractEntityValuedBases.Add(BaseManager.Current.GetBase<AbstractEntityValuedBase>(abstractEntityValued));
                abstractEntitiesToAdd = abstractEntityValuedBases.ToArray();
            }
        }
        #endregion Property: AbstractEntitiesToAdd

        #region Property: AbstractEntitiesToRemove
        /// <summary>
        /// The abstract entities that should be removed during the change.
        /// </summary>
        private AbstractEntityConditionBase[] abstractEntitiesToRemove = null;

        /// <summary>
        /// Gets the abstract entities that should be removed during the change.
        /// </summary>
        public ReadOnlyCollection<AbstractEntityConditionBase> AbstractEntitiesToRemove
        {
            get
            {
                if (abstractEntitiesToRemove == null)
                {
                    LoadAbstractEntitiesToRemove();
                    if (abstractEntitiesToRemove == null)
                        return new List<AbstractEntityConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<AbstractEntityConditionBase>(abstractEntitiesToRemove);
            }
        }

        /// <summary>
        /// Loads the abstract entities to remove.
        /// </summary>
        private void LoadAbstractEntitiesToRemove()
        {
            if (this.EntityChange != null)
            {
                List<AbstractEntityConditionBase> abstractEntityConditionBases = new List<AbstractEntityConditionBase>();
                foreach (AbstractEntityCondition abstractEntityCondition in this.EntityChange.AbstractEntitiesToRemove)
                    abstractEntityConditionBases.Add(BaseManager.Current.GetBase<AbstractEntityConditionBase>(abstractEntityCondition));
                abstractEntitiesToRemove = abstractEntityConditionBases.ToArray();
            }
        }
        #endregion Property: AbstractEntitiesToRemove

        #region Property: Quantity
        /// <summary>
        /// The quantity.
        /// </summary>
        private ValueChangeBase quantity = null;

        /// <summary>
        /// Gets the quantity.
        /// </summary>
        public ValueChangeBase Quantity
        {
            get
            {
                if (quantity == null)
                    LoadQuantity();
                return quantity;
            }
        }

        /// <summary>
        /// Loads the quantity.
        /// </summary>
        private void LoadQuantity()
        {
            if (this.EntityChange != null)
                quantity = BaseManager.Current.GetBase<ValueChangeBase>(this.EntityChange.Quantity);
        }
        #endregion Property: Quantity

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: EntityChangeBase(EntityChange entityChange)
        /// <summary>
        /// Creates a base of the given entity change.
        /// </summary>
        /// <param name="entityChange">The entity change to create a base of.</param>
        protected EntityChangeBase(EntityChange entityChange)
            : base(entityChange)
        {
            if (entityChange != null)
            {
                if (BaseManager.PreloadProperties)
                {
                    LoadQuantity();
                    LoadAttributes();
                    LoadAttributesToAdd();
                    LoadAttributesToRemove();
                    LoadStateGroups();
                    LoadRelationships();
                    LoadAbstractEntities();
                    LoadAbstractEntitiesToAdd();
                    LoadAbstractEntitiesToRemove();
                }
            }
        }
        #endregion Constructor: EntityChangeBase(EntityChange entityChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: EntityChangeBase

    #region Class: EntityCreationBase
    /// <summary>
    /// A entity that should be created.
    /// </summary>
    public sealed class EntityCreationBase : EffectBase
    {

        #region Properties and Fields

        #region Property: EntityCreation
        /// <summary>
        /// Gets the entity creation of which this is a entity creation base.
        /// </summary>
        internal EntityCreation EntityCreation
        {
            get
            {
                return this.Effect as EntityCreation;
            }
        }
        #endregion Property: EntityCreation

        #region Property: EntityValuedBase
        /// <summary>
        /// The entity that has to be created.
        /// </summary>
        private EntityValuedBase entityValuedBase = null;

        /// <summary>
        /// Gets the entity that has to be created.
        /// </summary>
        public EntityValuedBase EntityValuedBase
        {
            get
            {
                return entityValuedBase;
            }
        }
        #endregion Property: EntityValuedBase

        #region Property: Destination
        /// <summary>
        /// Gets the destination of the entity that is created.
        /// </summary>
        private Destination destination = default(Destination);
        
        /// <summary>
        /// Gets the destination of the entity that is created.
        /// </summary>
        public Destination Destination
        {
            get
            {
                return destination;
            }
        }
        #endregion Property: Destination

        #region Property: Position
        /// <summary>
        /// The position the created entity should get; only valid when 'Destination' has been set to 'World'.
        /// </summary>
        private VectorValueBase position = null;

        /// <summary>
        /// Gets the position the created entity should get; only valid when 'Destination' has been set to 'World'.
        /// </summary>
        public VectorValueBase Position
        {
            get
            {
                return position;
            }
        }
        #endregion Property: Position		

        #region Property: Rotation
        /// <summary>
        /// The rotation (in degrees) the created entity should get.
        /// </summary>
        private VectorValueBase rotation = null;

        /// <summary>
        /// Gets the rotation (in degrees) the created entity should get.
        /// </summary>
        public VectorValueBase Rotation
        {
            get
            {
                return rotation;
            }
        }
        #endregion Property: Rotation

        #region Property: RelationshipType
        /// <summary>
        /// The relationship that has to be established with the created entity.
        /// </summary>
        private RelationshipTypeBase relationshipType = null;
        
        /// <summary>
        /// Gets the relationship that has to be established with the created entity.
        /// </summary>
        public RelationshipTypeBase RelationshipType
        {
            get
            {
                return relationshipType;
            }
        }
        #endregion Property: RelationshipType

        #region Property: RelationshipSourceTarget
        /// <summary>
        /// Indicates who should be the source and the target of the relationship that has to be created, in case 'RelationshipType' has been set.
        /// </summary>
        private CreationRelationshipSourceTarget relationshipSourceTarget = default(CreationRelationshipSourceTarget);
        
        /// <summary>
        /// Gets who should be the source and the target of the relationship that has to be created, in case 'RelationshipType' has been set.
        /// </summary>
        public CreationRelationshipSourceTarget RelationshipSourceTarget
        {
            get
            {
                return relationshipSourceTarget;
            }
        }
        #endregion Property: RelationshipSourceTarget

        #region Property: MatterSource
        /// <summary>
        /// The source of the matter from which the matter of the created entity should be retrieved, which is 'null' when this should not be taken into account. Only valid when tangible objects should be created!
        /// </summary>
        private ActorTargetArtifactReference? matterSource = null;
        
        /// <summary>
        /// Gets the source of the matter from which the matter of the created entity should be retrieved, which is 'null' when this should not be taken into account. Only valid when tangible objects should be created!
        /// </summary>
        public ActorTargetArtifactReference? MatterSource
        {
            get
            {
                return matterSource;
            }
        }
        #endregion Property: MatterSource

        #region Property: MatterSourceReference
        /// <summary>
        /// The reference of the matter source, in case MatterSource has been set to 'Reference'. Only valid when tangible objects should be created!
        /// </summary>
        private ReferenceBase matterSourceReference = null;
        
        /// <summary>
        /// Gets the reference of the matter source, in case MatterSource has been set to 'Reference'. Only valid when tangible objects should be created!
        /// </summary>
        public ReferenceBase MatterSourceReference
        {
            get
            {
                return matterSourceReference;
            }
        }
        #endregion Property: MatterSourceReference

        #region Property: PartSource
        /// <summary>
        /// The source of the parts from which the parts of the created entity should be retrieved, which is 'null' when this should not be taken into account. Only valid when tangible objects should be created!
        /// </summary>
        private ActorTargetArtifactReference? partSource = null;

        /// <summary>
        /// Gets the source of the parts from which the parts of the created entity should be retrieved, which is 'null' when this should not be taken into account. Only valid when tangible objects should be created!
        /// </summary>
        public ActorTargetArtifactReference? PartSource
        {
            get
            {
                return partSource;
            }
        }
        #endregion Property: PartSource

        #region Property: PartSourceReference
        /// <summary>
        /// The reference of the part source, in case PartSource has been set to 'Reference'. Only valid when tangible objects should be created!
        /// </summary>
        private ReferenceBase partSourceReference = null;

        /// <summary>
        /// Gets the reference of the part source, in case PartSource has been set to 'Reference'. Only valid when tangible objects should be created!
        /// </summary>
        public ReferenceBase PartSourceReference
        {
            get
            {
                return partSourceReference;
            }
        }
        #endregion Property: PartSourceReference

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: EntityCreationBase(EntityCreation entityCreation)
        /// <summary>
        /// Creates a base of the given entity creation.
        /// </summary>
        /// <param name="entityCreation">The entity creation to create a base from.</param>
        internal EntityCreationBase(EntityCreation entityCreation)
            : base(entityCreation)
        {
            if (entityCreation != null)
            {
                this.entityValuedBase = BaseManager.Current.GetBase<EntityValuedBase>(entityCreation.EntityValued);
                this.destination = entityCreation.Destination;
                this.position = BaseManager.Current.GetBase<VectorValueBase>(entityCreation.Position);
                this.rotation = BaseManager.Current.GetBase<VectorValueBase>(entityCreation.Rotation);
                this.relationshipType = BaseManager.Current.GetBase<RelationshipTypeBase>(entityCreation.RelationshipType);
                this.relationshipSourceTarget = entityCreation.RelationshipSourceTarget;
                this.matterSource = entityCreation.MatterSource;
                this.matterSourceReference = BaseManager.Current.GetBase<ReferenceBase>(entityCreation.MatterSourceReference);
                this.partSource = entityCreation.PartSource;
                this.partSourceReference = BaseManager.Current.GetBase<ReferenceBase>(entityCreation.PartSourceReference);
            }
        }
        #endregion Constructor: EntityCreationBase(EntityCreation entityCreation)

        #endregion Method Group: Constructors

    }
    #endregion Class: EntityCreationBase

}