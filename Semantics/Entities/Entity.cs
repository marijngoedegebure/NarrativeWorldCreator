/**************************************************************************
 * 
 * Entity.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2009-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Common;
using Semantics.Abstractions;
using Semantics.Components;
using Semantics.Data;
using Semantics.Interfaces;
using Semantics.Utilities;
using Action = Semantics.Abstractions.Action;
using Attribute = Semantics.Abstractions.Attribute;

namespace Semantics.Entities
{

    #region Class: Entity
    /// <summary>
    /// An entity with attributes, state groups, services, predicates, and groups.
    /// </summary>
    public abstract class Entity : Node, IComparable<Entity>, IVariableReferenceHolder
    {

        #region Properties and Fields

        #region Property: Attributes
        /// <summary>
        /// Gets the personal, inherited, and overridden attributes of the entity.
        /// </summary>
        public ReadOnlyCollection<AttributeValued> Attributes
        {
            get
            {
                List<AttributeValued> attributes = new List<AttributeValued>();
                attributes.AddRange(this.PersonalAttributes);
                attributes.AddRange(this.InheritedAttributes);
                attributes.AddRange(this.OverriddenAttributes);
                return attributes.AsReadOnly();
            }
        }
        #endregion Property: Attributes

        #region Property: PersonalAttributes
        /// <summary>
        /// Gets the personal attributes.
        /// </summary>
        public ReadOnlyCollection<AttributeValued> PersonalAttributes
        {
            get
            {
                return Database.Current.SelectAll<AttributeValued>(this.ID, GenericTables.EntityAttribute, Columns.AttributeValued).AsReadOnly();
            }
        }
        #endregion Property: PersonalAttributes

        #region Property: InheritedAttributes
        /// <summary>
        /// Gets the inherited attributes.
        /// </summary>
        public ReadOnlyCollection<AttributeValued> InheritedAttributes
        {
            get
            {
                List<AttributeValued> inheritedAttributes = new List<AttributeValued>();
                foreach (Node parent in this.PersonalParents)
                {
                    foreach (AttributeValued inheritedAttribute in ((Entity)parent).Attributes)
                    {
                        if (!HasOverriddenAttribute(inheritedAttribute.Attribute))
                            inheritedAttributes.Add(inheritedAttribute);
                    }
                }
                return inheritedAttributes.AsReadOnly();
            }
        }
        #endregion Property: InheritedAttributes

        #region Property: OverriddenAttributes
        /// <summary>
        /// Gets the overridden attributes.
        /// </summary>
        public ReadOnlyCollection<AttributeValued> OverriddenAttributes
        {
            get
            {
                return Database.Current.SelectAll<AttributeValued>(this.ID, GenericTables.EntityOverriddenAttribute, Columns.AttributeValued).AsReadOnly();
            }
        }
        #endregion Property: OverriddenAttributes

        #region Property: StateGroups
        /// <summary>
        /// Gets the personal and inherited state groups of the entity.
        /// </summary>
        public ReadOnlyCollection<StateGroup> StateGroups
        {
            get
            {
                List<StateGroup> stateGroups = new List<StateGroup>();
                stateGroups.AddRange(this.PersonalStateGroups);
                stateGroups.AddRange(this.InheritedStateGroups);
                return stateGroups.AsReadOnly();
            }
        }
        #endregion Property: StateGroups

        #region Property: PersonalStateGroups
        /// <summary>
        /// Gets the personal state groups.
        /// </summary>
        public ReadOnlyCollection<StateGroup> PersonalStateGroups
        {
            get
            {
                return Database.Current.SelectAll<StateGroup>(this.ID, GenericTables.EntityStateGroup, Columns.StateGroup).AsReadOnly();
            }
        }
        #endregion Property: PersonalStateGroups

        #region Property: InheritedStateGroups
        /// <summary>
        /// Gets the inherited state groups.
        /// </summary>
        public ReadOnlyCollection<StateGroup> InheritedStateGroups
        {
            get
            {
                List<StateGroup> inheritedStateGroups = new List<StateGroup>();
                foreach (Node parent in this.PersonalParents)
                    inheritedStateGroups.AddRange(((Entity)parent).StateGroups);
                return inheritedStateGroups.AsReadOnly();
            }
        }
        #endregion Property: InheritedStateGroups

        #region Property: Events
        /// <summary>
        /// Gets the events of the entity.
        /// </summary>
        public ReadOnlyCollection<Event> Events
        {
            get
            {
                List<Event> events = new List<Event>();
                events.AddRange(this.PersonalEvents);
                events.AddRange(this.InheritedEvents);
                return events.AsReadOnly();
            }
        }
        #endregion Property: Events

        #region Property: PersonalEvents
        /// <summary>
        /// Gets the personal events of the entity.
        /// </summary>
        public ReadOnlyCollection<Event> PersonalEvents
        {
            get
            {
                List<Event> events = new List<Event>();

                // Get all events where this entity is the actor
                foreach (Event e in Database.Current.SelectAll<Event>(GenericTables.Event, Columns.Actor, this.ID))
                    events.Add(e);

                return events.AsReadOnly();
            }
        }
        #endregion Property: PersonalEvents

        #region Property: InheritedEvents
        /// <summary>
        /// Gets the inherited events of the entity.
        /// </summary>
        public ReadOnlyCollection<Event> InheritedEvents
        {
            get
            {
                List<Event> events = new List<Event>();

                // Add the events of the parents
                foreach (Node parent in this.PersonalParents)
                    events.AddRange(((Entity)parent).Events);

                // Also add all inherited events that have been defined for the children of the action behind each personal event
                foreach (Event personalEvent in this.PersonalEvents)
                {
                    foreach (Event actionEvent in personalEvent.Action.InheritedEvents)
                    {
                        if (this.Equals(actionEvent.Actor))
                            events.Add(actionEvent);
                    }
                }

                return events.AsReadOnly();
            }
        }
        #endregion Property: InheritedEvents

        #region Property: EventsAsTarget
        /// <summary>
        /// Gets all events where this entity is the target.
        /// </summary>
        public ReadOnlyCollection<Event> EventsAsTarget
        {
            get
            {
                List<Event> events = new List<Event>();
                events.AddRange(this.PersonalEventsAsTarget);
                events.AddRange(this.InheritedEventsAsTarget);
                return events.AsReadOnly();
            }
        }
        #endregion Property: EventsAsTarget

        #region Property: PersonalEventsAsTarget
        /// <summary>
        /// Gets all personal events where this entity is the target.
        /// </summary>
        public ReadOnlyCollection<Event> PersonalEventsAsTarget
        {
            get
            {
                List<Event> events = new List<Event>();

                // Get all events where this entity is the target
                foreach (Event even in Database.Current.SelectAll<Event>(GenericTables.Event, Columns.Target, this.ID))
                    events.Add(even);

                return events.AsReadOnly();
            }
        }
        #endregion Property: PersonalEventsAsTarget

        #region Property: InheritedEventsAsTarget
        /// <summary>
        /// Gets all inherited events where this entity is the target.
        /// </summary>
        public ReadOnlyCollection<Event> InheritedEventsAsTarget
        {
            get
            {
                List<Event> events = new List<Event>();
                foreach (Node parent in this.PersonalParents)
                    events.AddRange(((Entity)parent).EventsAsTarget);
                return events.AsReadOnly();
            }
        }
        #endregion Property: InheritedEventsAsTarget

        #region Property: PredicatesAsActor
        /// <summary>
        /// Gets all predicates where this entity is the actor.
        /// </summary>
        public ReadOnlyCollection<Predicate> PredicatesAsActor
        {
            get
            {
                List<Predicate> predicates = new List<Predicate>();
                predicates.AddRange(this.PersonalPredicatesAsActor);
                predicates.AddRange(this.InheritedPredicatesAsActor);
                return predicates.AsReadOnly();
            }
        }
        #endregion Property: PredicatesAsActor

        #region Property: PersonalPredicatesAsActor
        /// <summary>
        /// Gets all personal predicates where this entity is the actor.
        /// </summary>
        public ReadOnlyCollection<Predicate> PersonalPredicatesAsActor
        {
            get
            {
                List<Predicate> predicates = new List<Predicate>();

                // Get all predicates where this entity is the actor
                foreach (Predicate predicate in Database.Current.SelectAll<Predicate>(GenericTables.Predicate, Columns.Actor, this.ID))
                    predicates.Add(predicate);

                return predicates.AsReadOnly();
            }
        }
        #endregion Property: PersonalPredicatesAsActor

        #region Property: InheritedPredicatesAsActor
        /// <summary>
        /// Gets all inherited predicates where this entity is the actor.
        /// </summary>
        public ReadOnlyCollection<Predicate> InheritedPredicatesAsActor
        {
            get
            {
                List<Predicate> predicates = new List<Predicate>();
                foreach (Node parent in this.PersonalParents)
                    predicates.AddRange(((Entity)parent).PredicatesAsActor);
                return predicates.AsReadOnly();
            }
        }
        #endregion Property: InheritedPredicatesAsActor

        #region Property: PredicatesAsTarget
        /// <summary>
        /// Gets all predicates where this entity is the target.
        /// </summary>
        public ReadOnlyCollection<Predicate> PredicatesAsTarget
        {
            get
            {
                List<Predicate> predicates = new List<Predicate>();
                predicates.AddRange(this.PersonalPredicatesAsTarget);
                predicates.AddRange(this.InheritedPredicatesAsTarget);
                return predicates.AsReadOnly();
            }
        }
        #endregion Property: PredicatesAsTarget

        #region Property: PersonalPredicatesAsTarget
        /// <summary>
        /// Gets all personal predicates where this entity is the target.
        /// </summary>
        public ReadOnlyCollection<Predicate> PersonalPredicatesAsTarget
        {
            get
            {
                List<Predicate> predicates = new List<Predicate>();

                // Get all predicates where this entity is the target
                foreach (Predicate predicate in Database.Current.SelectAll<Predicate>(GenericTables.Predicate, Columns.Target, this.ID))
                    predicates.Add(predicate);

                return predicates.AsReadOnly();
            }
        }
        #endregion Property: PersonalPredicatesAsTarget

        #region Property: InheritedPredicatesAsTarget
        /// <summary>
        /// Gets all inherited predicates where this entity is the target.
        /// </summary>
        public ReadOnlyCollection<Predicate> InheritedPredicatesAsTarget
        {
            get
            {
                List<Predicate> predicates = new List<Predicate>();
                foreach (Node parent in this.PersonalParents)
                    predicates.AddRange(((Entity)parent).PredicatesAsTarget);
                return predicates.AsReadOnly();
            }
        }
        #endregion Property: InheritedPredicatesAsTarget

        #region Property: Groups
        /// <summary>
        /// Gets the groups to which the entity belongs.
        /// </summary>
        public ReadOnlyCollection<Group> Groups
        {
            get
            {
                List<Group> groups = new List<Group>();
                groups.AddRange(this.PersonalGroups);
                groups.AddRange(this.InheritedGroups);
                return groups.AsReadOnly();
            }
        }
        #endregion Property: Groups

        #region Property: PersonalGroups
        /// <summary>
        /// Gets te personal groups to which the entity belongs.
        /// </summary>
        public ReadOnlyCollection<Group> PersonalGroups
        {
            get
            {
                List<Group> groups = new List<Group>();

                // Get all groups that contain this entity
                foreach (Group group in Database.Current.SelectAll<Group>(GenericTables.GroupEntity, Columns.Entity, this.ID))
                    groups.Add(group);

                return groups.AsReadOnly();
            }
        }
        #endregion Property: PersonalGroups

        #region Property: InheritedGroups
        /// <summary>
        /// Gets the inherited groups to which the entity belongs.
        /// </summary>
        public ReadOnlyCollection<Group> InheritedGroups
        {
            get
            {
                List<Group> groups = new List<Group>();

                // Add the parents of the personal groups
                foreach (Group group in this.PersonalGroups)
                {
                    foreach (Node parent in group.Parents)
                    {
                        Group parentGroup = (Group)parent;
                        if (!groups.Contains(parentGroup))
                            groups.Add(parentGroup);
                    }
                }

                // Add the groups of the personal parents
                foreach (Node parent in this.PersonalParents)
                {
                    foreach (Group group in ((Entity)parent).Groups)
                    {
                        if (!groups.Contains(group))
                            groups.Add(group);
                    }
                }

                return groups.AsReadOnly();
            }
        }
        #endregion Property: InheritedGroups

        #region Property: RelationshipsAsSource
        /// <summary>
        /// Gets all relationships where this entity is the source.
        /// </summary>
        public ReadOnlyCollection<Relationship> RelationshipsAsSource
        {
            get
            {
                List<Relationship> relationships = new List<Relationship>();
                relationships.AddRange(this.PersonalRelationshipsAsSource);
                relationships.AddRange(this.InheritedRelationshipsAsSource);
                return relationships.AsReadOnly();
            }
        }
        #endregion Property: RelationshipsAsSource

        #region Property: PersonalRelationshipsAsSource
        /// <summary>
        /// Gets all personal relationships where this entity is the source.
        /// </summary>
        public ReadOnlyCollection<Relationship> PersonalRelationshipsAsSource
        {
            get
            {
                List<Relationship> relationships = new List<Relationship>();

                // Get all relationships where this entity is the source
                foreach (Relationship r in Database.Current.SelectAll<Relationship>(GenericTables.Relationship, Columns.Source, this.ID))
                    relationships.Add(r);

                return relationships.AsReadOnly();
            }
        }
        #endregion Property: PersonalRelationshipsAsSource

        #region Property: InheritedRelationshipsAsSource
        /// <summary>
        /// Gets all inherited relationships where this entity is the source.
        /// </summary>
        public ReadOnlyCollection<Relationship> InheritedRelationshipsAsSource
        {
            get
            {
                List<Relationship> relationships = new List<Relationship>();
                foreach (Node parent in this.PersonalParents)
                    relationships.AddRange(((Entity)parent).RelationshipsAsSource);
                return relationships.AsReadOnly();
            }
        }
        #endregion Property: InheritedRelationshipsAsSource

        #region Property: RelationshipsAsTarget
        /// <summary>
        /// Gets all relationships where this entity is the target.
        /// </summary>
        public ReadOnlyCollection<Relationship> RelationshipsAsTarget
        {
            get
            {
                List<Relationship> relationships = new List<Relationship>();
                relationships.AddRange(this.PersonalRelationshipsAsTarget);
                relationships.AddRange(this.InheritedRelationshipsAsTarget);
                return relationships.AsReadOnly();
            }
        }
        #endregion Property: RelationshipsAsTarget

        #region Property: PersonalRelationshipsAsTarget
        /// <summary>
        /// Gets all personal relationships where this entity is the target.
        /// </summary>
        public ReadOnlyCollection<Relationship> PersonalRelationshipsAsTarget
        {
            get
            {
                List<Relationship> relationships = new List<Relationship>();

                // Get all relationships where this entity is the target
                foreach (Relationship r in Database.Current.SelectAll<Relationship>(GenericTables.RelationshipTarget, Columns.Target, this.ID))
                    relationships.Add(r);

                return relationships.AsReadOnly();
            }
        }
        #endregion Property: PersonalRelationshipsAsTarget

        #region Property: InheritedRelationshipsAsTarget
        /// <summary>
        /// Gets all inherited relationships where this entity is the target.
        /// </summary>
        public ReadOnlyCollection<Relationship> InheritedRelationshipsAsTarget
        {
            get
            {
                List<Relationship> relationships = new List<Relationship>();
                foreach (Node parent in this.PersonalParents)
                    relationships.AddRange(((Entity)parent).RelationshipsAsTarget);
                return relationships.AsReadOnly();
            }
        }
        #endregion Property: InheritedRelationshipsAsTarget

        #region Property: CombinedRelationships
        /// <summary>
        /// Gets the combined relationships.
        /// </summary>
        public ReadOnlyCollection<CombinedRelationship> CombinedRelationships
        {
            get
            {
                return Database.Current.SelectAll<CombinedRelationship>(this.ID, GenericTables.EntityCombinedRelationship, Columns.CombinedRelationship).AsReadOnly();
            }
        }
        #endregion Property: CombinedRelationships

        #region Property: Variables
        /// <summary>
        /// Gets all variables.
        /// </summary>
        public ReadOnlyCollection<Variable> Variables
        {
            get
            {
                List<Variable> variables = new List<Variable>();
                variables.AddRange(this.PersonalVariables);
                variables.AddRange(this.InheritedVariables);
                return variables.AsReadOnly();
            }
        }
        #endregion Property: Variables

        #region Property: PersonalVariables
        /// <summary>
        /// Gets the personal variables.
        /// </summary>
        public ReadOnlyCollection<Variable> PersonalVariables
        {
            get
            {
                return Database.Current.SelectAll<Variable>(this.ID, GenericTables.EntityVariable, Columns.Variable).AsReadOnly();
            }
        }
        #endregion Property: PersonalVariables

        #region Property: InheritedVariables
        /// <summary>
        /// Gets the inherited variables.
        /// </summary>
        public ReadOnlyCollection<Variable> InheritedVariables
        {
            get
            {
                List<Variable> inheritedVariables = new List<Variable>();
                foreach (Node parent in this.PersonalParents)
                    inheritedVariables.AddRange(((Entity)parent).Variables);
                return inheritedVariables.AsReadOnly();
            }
        }
        #endregion Property: InheritedVariables

        #region Property: References
        /// <summary>
        /// Gets all references.
        /// </summary>
        public ReadOnlyCollection<Reference> References
        {
            get
            {
                List<Reference> references = new List<Reference>();
                references.AddRange(this.PersonalReferences);
                references.AddRange(this.InheritedReferences);
                return references.AsReadOnly();
            }
        }
        #endregion Property: References

        #region Property: PersonalReferences
        /// <summary>
        /// Gets the personal references.
        /// </summary>
        public ReadOnlyCollection<Reference> PersonalReferences
        {
            get
            {
                return Database.Current.SelectAll<Reference>(this.ID, GenericTables.EntityReference, Columns.Reference).AsReadOnly();
            }
        }
        #endregion Property: PersonalReferences

        #region Property: InheritedReferences
        /// <summary>
        /// Gets the inherited references.
        /// </summary>
        public ReadOnlyCollection<Reference> InheritedReferences
        {
            get
            {
                List<Reference> inheritedReferences = new List<Reference>();
                foreach (Node parent in this.PersonalParents)
                    inheritedReferences.AddRange(((Entity)parent).References);
                return inheritedReferences.AsReadOnly();
            }
        }
        #endregion Property: InheritedReferences

        #region Property: AbstractEntities
        /// <summary>
        /// Gets all abstract entities of the entity.
        /// </summary>
        public ReadOnlyCollection<AbstractEntityValued> AbstractEntities
        {
            get
            {
                List<AbstractEntityValued> abstractEntities = new List<AbstractEntityValued>();
                abstractEntities.AddRange(this.PersonalAbstractEntities);
                abstractEntities.AddRange(this.InheritedAbstractEntities);
                abstractEntities.AddRange(this.OverriddenAbstractEntities);
                return abstractEntities.AsReadOnly();
            }
        }
        #endregion Property: AbstractEntities

        #region Property: PersonalAbstractEntities
        /// <summary>
        /// Gets the personal abstract entities of the entity.
        /// </summary>
        public ReadOnlyCollection<AbstractEntityValued> PersonalAbstractEntities
        {
            get
            {
                return Database.Current.SelectAll<AbstractEntityValued>(this.ID, GenericTables.EntityAbstractEntity, Columns.AbstractEntityValued).AsReadOnly();
            }
        }
        #endregion Property: PersonalAbstractEntities

        #region Property: InheritedAbstractEntities
        /// <summary>
        /// Gets the inherited abstract entities of the entity.
        /// </summary>
        public ReadOnlyCollection<AbstractEntityValued> InheritedAbstractEntities
        {
            get
            {
                List<AbstractEntityValued> inheritedAbstractEntities = new List<AbstractEntityValued>();
                foreach (Node parent in this.PersonalParents)
                {
                    foreach (AbstractEntityValued inheritedAbstractEntity in ((Entity)parent).AbstractEntities)
                    {
                        if (!HasOverriddenAbstractEntity(inheritedAbstractEntity.AbstractEntity))
                            inheritedAbstractEntities.Add(inheritedAbstractEntity);
                    }
                }
                return inheritedAbstractEntities.AsReadOnly();
            }
        }
        #endregion Property: InheritedAbstractEntities

        #region Property: OverriddenAbstractEntities
        /// <summary>
        /// Gets the overridden abstract entities.
        /// </summary>
        public ReadOnlyCollection<AbstractEntityValued> OverriddenAbstractEntities
        {
            get
            {
                return Database.Current.SelectAll<AbstractEntityValued>(this.ID, GenericTables.EntityOverriddenAbstractEntity, Columns.AbstractEntityValued).AsReadOnly();
            }
        }
        #endregion Property: OverriddenAbstractEntities

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: Entity()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static Entity()
        {
            // Attributes
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Attribute, new Tuple<Type, EntryType>(typeof(Attribute), EntryType.Unique));
            dict.Add(Columns.AttributeValued, new Tuple<Type, EntryType>(typeof(AttributeValued), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.EntityAttribute, typeof(Entity), dict);

            // Overridden attributes
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.AttributeValued, new Tuple<Type, EntryType>(typeof(AttributeValued), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.EntityOverriddenAttribute, typeof(Entity), dict);

            // State groups
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.StateGroup, new Tuple<Type, EntryType>(typeof(StateGroup), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.EntityStateGroup, typeof(Entity), dict);

            // Variables
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Variable, new Tuple<Type, EntryType>(typeof(Variable), EntryType.Intermediate));
            Database.Current.AddTableDefinition(GenericTables.EntityVariable, typeof(Entity), dict);

            // References
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Reference, new Tuple<Type, EntryType>(typeof(Reference), EntryType.Intermediate));
            Database.Current.AddTableDefinition(GenericTables.EntityReference, typeof(Entity), dict);

            // Combined relationships
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.CombinedRelationship, new Tuple<Type, EntryType>(typeof(CombinedRelationship), EntryType.Intermediate));
            Database.Current.AddTableDefinition(GenericTables.EntityCombinedRelationship, typeof(Entity), dict);

            // Attributes
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.AbstractEntityValued, new Tuple<Type, EntryType>(typeof(AbstractEntityValued), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.EntityAbstractEntity, typeof(Entity), dict);
        }
        #endregion Static Constructor: Entity()

        #region Constructor: Entity()
        /// <summary>
        /// Creates a new entity.
        /// </summary>
        protected Entity()
            : base()
        {
        }
        #endregion Constructor: Entity()

        #region Constructor: Entity(uint id)
        /// <summary>
        /// Creates a new entity from the given ID.
        /// </summary>
        /// <param name="id">The ID to create an entity from.</param>
        protected Entity(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Entity(uint id)

        #region Constructor: Entity(string name)
        /// <summary>
        /// Creates a new entity with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the entity.</param>
        protected Entity(string name)
            : base(name)
        {
        }
        #endregion Constructor: Entity(string name)

        #region Constructor: Entity(Entity entity)
        /// <summary>
        /// Clones the entity.
        /// </summary>
        /// <param name="entity">The entity to clone.</param>
        protected Entity(Entity entity)
            : base(entity)
        {
            if (entity != null)
            {
                Database.Current.StartChange();

                foreach (AttributeValued attributeValued in entity.PersonalAttributes)
                    AddAttribute(new AttributeValued(attributeValued));
                foreach (AttributeValued attributeValued in entity.OverriddenAttributes)
                    AddOverriddenAttribute(new AttributeValued(attributeValued));
                foreach (StateGroup stateGroup in entity.PersonalStateGroups)
                    AddStateGroup(new StateGroup(stateGroup));
                foreach (Reference reference in entity.PersonalReferences)
                    AddReference(reference.Clone());
                foreach (Variable variable in entity.PersonalVariables)
                    AddVariable(variable.Clone());
                foreach (CombinedRelationship combinedRelationship in entity.CombinedRelationships)
                    AddCombinedRelationship(new CombinedRelationship(combinedRelationship));
                foreach (AbstractEntityValued abstractEntityValued in entity.PersonalAbstractEntities)
                    AddAbstractEntity(new AbstractEntityValued(abstractEntityValued));
                foreach (AbstractEntityValued abstractEntityValued in entity.OverriddenAbstractEntities)
                    AddOverriddenAbstractEntity(new AbstractEntityValued(abstractEntityValued));

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Entity(Entity entity)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddAttribute(AttributeValued attributeValued)
        /// <summary>
        /// Adds a valued attribute.
        /// </summary>
        /// <param name="attributeValued">The valued attribute to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddAttribute(AttributeValued attributeValued)
        {
            if (attributeValued != null && attributeValued.Attribute != null)
            {
                // If the valued attribute is already available in all attributes, there is no use to add it
                if (HasAttribute(attributeValued.Attribute))
                    return Message.RelationExistsAlready;

                // Add the valued attribute to the attributes
                Database.Current.Insert(this.ID, GenericTables.EntityAttribute, new string[] { Columns.Attribute, Columns.AttributeValued }, new object[] { attributeValued.Attribute, attributeValued });
                NotifyPropertyChanged("PersonalAttributes");
                NotifyPropertyChanged("Attributes");
                
                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddAttribute(AttributeValued attributeValued)

        #region Method: RemoveAttribute(AttributeValued attributeValued)
        /// <summary>
        /// Removes a valued attribute.
        /// </summary>
        /// <param name="attributeValued">The valued attribute to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveAttribute(AttributeValued attributeValued)
        {
            if (attributeValued != null)
            {
                if (this.Attributes.Contains(attributeValued))
                {
                    // Remove the valued attribute
                    Database.Current.Remove(this.ID, GenericTables.EntityAttribute, Columns.AttributeValued, attributeValued);
                    NotifyPropertyChanged("PersonalAttributes");
                    NotifyPropertyChanged("Attributes");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveAttribute(AttributeValued attributeValued)

        #region Method: OverrideAttribute(AttributeValued inheritedAttribute)
        /// <summary>
        /// Override the given inherited attribute.
        /// </summary>
        /// <param name="inheritedAttribute">The inherited attribute that should be overridden.</param>
        /// <returns>Returns whether the override has been successful.</returns>
        public Message OverrideAttribute(AttributeValued inheritedAttribute)
        {
            if (inheritedAttribute != null && inheritedAttribute.Attribute != null && this.InheritedAttributes.Contains(inheritedAttribute))
            {
                // If the valued attribute is already available, there is no use to add it
                foreach (AttributeValued personalAttribute in this.PersonalAttributes)
                {
                    if (inheritedAttribute.Attribute.Equals(personalAttribute.Attribute))
                        return Message.RelationExistsAlready;
                }
                if (HasOverriddenAttribute(inheritedAttribute.Attribute))
                    return Message.RelationExistsAlready;

                // Copy the valued attribute and add it
                return AddOverriddenAttribute(new AttributeValued(inheritedAttribute));
            }
            return Message.RelationFail;
        }
        #endregion Method: OverrideAttribute(AttributeValued inheritedAttribute)

        #region Method: AddOverriddenAttribute(AttributeValued inheritedAttribute)
        /// <summary>
        /// Add the given overridden attribute.
        /// </summary>
        /// <param name="overriddenAttribute">The overridden attribute to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        private Message AddOverriddenAttribute(AttributeValued overriddenAttribute)
        {
            if (overriddenAttribute != null)
            {
                Database.Current.Insert(this.ID, GenericTables.EntityOverriddenAttribute, Columns.AttributeValued, overriddenAttribute);
                NotifyPropertyChanged("OverriddenAttributes");
                NotifyPropertyChanged("InheritedAttributes");
                NotifyPropertyChanged("Attributes");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddOverriddenAttribute(AttributeValued inheritedAttribute)

        #region Method: RemoveOverriddenAttribute(AttributeValued overriddenAttribute)
        /// <summary>
        /// Removes an overridden valued attribute.
        /// </summary>
        /// <param name="overriddenAttribute">The overridden valued attribute to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveOverriddenAttribute(AttributeValued overriddenAttribute)
        {
            if (overriddenAttribute != null)
            {
                if (this.OverriddenAttributes.Contains(overriddenAttribute))
                {
                    // Remove the valued attribute
                    Database.Current.Remove(this.ID, GenericTables.EntityOverriddenAttribute, Columns.AttributeValued, overriddenAttribute);
                    NotifyPropertyChanged("OverriddenAttributes");
                    NotifyPropertyChanged("InheritedAttributes");
                    NotifyPropertyChanged("Attributes");
                    overriddenAttribute.Remove();

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveOverriddenAttribute(AttributeValued overriddenAttribute)

        #region Method: AddStateGroup(StateGroup stateGroup)
        /// <summary>
        /// Adds a state group.
        /// </summary>
        /// <param name="stateGroup">The state group to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddStateGroup(StateGroup stateGroup)
        {
            if (stateGroup != null)
            {
                // If the state group is already available in all state groups, there is no use to add it
                if (HasStateGroup(stateGroup))
                    return Message.RelationExistsAlready;

                // Add the state group to the state groups
                Database.Current.Insert(this.ID, GenericTables.EntityStateGroup, Columns.StateGroup, stateGroup);
                NotifyPropertyChanged("PersonalStateGroups");
                NotifyPropertyChanged("StateGroups");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddStateGroup(StateGroup stateGroup)

        #region Method: RemoveStateGroup(StateGroup stateGroup)
        /// <summary>
        /// Removes a state group.
        /// </summary>
        /// <param name="stateGroup">The state group to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveStateGroup(StateGroup stateGroup)
        {
            if (stateGroup != null)
            {
                if (HasStateGroup(stateGroup))
                {
                    // Remove the state group
                    Database.Current.Remove(this.ID, GenericTables.EntityStateGroup, Columns.StateGroup, stateGroup);
                    NotifyPropertyChanged("PersonalStateGroups");
                    NotifyPropertyChanged("StateGroups");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveStateGroup(StateGroup stateGroup)

        #region Method: AddVariable(Variable variable)
        /// <summary>
        /// Adds the given variable.
        /// </summary>
        /// <param name="variable">The variable to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddVariable(Variable variable)
        {
            if (variable != null)
            {
                // If the variable is already available in all variables, there is no use to add it
                if (HasVariable(variable))
                    return Message.RelationExistsAlready;

                // Add the variable
                Database.Current.Insert(this.ID, GenericTables.EntityVariable, Columns.Variable, variable);
                NotifyPropertyChanged("PersonalVariables");
                NotifyPropertyChanged("Variables");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddVariable(Variable variable)

        #region Method: RemoveVariable(Variable variable)
        /// <summary>
        /// Removes a variable.
        /// </summary>
        /// <param name="variable">The variable to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveVariable(Variable variable)
        {
            if (variable != null)
            {
                if (this.PersonalVariables.Contains(variable))
                {
                    // Remove the variable
                    Database.Current.Remove(this.ID, GenericTables.EntityVariable, Columns.Variable, variable);
                    NotifyPropertyChanged("PersonalVariables");
                    NotifyPropertyChanged("Variables");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveVariable(Variable variable)

        #region Method: AddReference(Reference reference)
        /// <summary>
        /// Adds the given reference.
        /// </summary>
        /// <param name="reference">The reference to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddReference(Reference reference)
        {
            if (reference != null)
            {
                // If the reference is already available in all references, there is no use to add it
                if (HasReference(reference))
                    return Message.RelationExistsAlready;

                // Add the reference
                Database.Current.Insert(this.ID, GenericTables.EntityReference, Columns.Reference, reference);
                NotifyPropertyChanged("PersonalReferences");
                NotifyPropertyChanged("References");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddReference(Reference reference)

        #region Method: RemoveReference(Reference reference)
        /// <summary>
        /// Removes a reference.
        /// </summary>
        /// <param name="reference">The reference to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveReference(Reference reference)
        {
            if (reference != null)
            {
                if (this.PersonalReferences.Contains(reference))
                {
                    // Remove the reference
                    Database.Current.Remove(this.ID, GenericTables.EntityReference, Columns.Reference, reference);
                    NotifyPropertyChanged("PersonalReferences");
                    NotifyPropertyChanged("References");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveReference(Reference reference)

        #region Method: AddCombinedRelationship(CombinedRelationship combinedRelationship)
        /// <summary>
        /// Adds the given combined relationship.
        /// </summary>
        /// <param name="combinedRelationship">The combined relationship to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddCombinedRelationship(CombinedRelationship combinedRelationship)
        {
            if (combinedRelationship != null)
            {
                // If the combined relationship is already available, there is no use to add it
                if (this.CombinedRelationships.Contains(combinedRelationship))
                    return Message.RelationExistsAlready;

                // Add the combined relationship
                Database.Current.Insert(this.ID, GenericTables.EntityCombinedRelationship, Columns.CombinedRelationship, combinedRelationship);
                NotifyPropertyChanged("CombinedRelationships");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddCombinedRelationship(CombinedRelationship combinedRelationship)

        #region Method: RemoveCombinedRelationship(CombinedRelationship combinedRelationship)
        /// <summary>
        /// Removes a combined relationship.
        /// </summary>
        /// <param name="combinedRelationship">The combined relationship to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveCombinedRelationship(CombinedRelationship combinedRelationship)
        {
            if (combinedRelationship != null)
            {
                if (this.CombinedRelationships.Contains(combinedRelationship))
                {
                    // Remove the combined relationship
                    Database.Current.Remove(this.ID, GenericTables.EntityCombinedRelationship, Columns.CombinedRelationship, combinedRelationship);
                    NotifyPropertyChanged("CombinedRelationships");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveCombinedRelationship(CombinedRelationship combinedRelationship)

        #region Method: AddAbstractEntity(AbstractEntityValued abstractEntityValued)
        /// <summary>
        /// Adds a valued abstract entity.
        /// </summary>
        /// <param name="abstractEntityValued">The valued abstract entity to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddAbstractEntity(AbstractEntityValued abstractEntityValued)
        {
            if (abstractEntityValued != null && abstractEntityValued.AbstractEntity != null)
            {
                // If the valued abstract entity is already available in all abstract entities, there is no use to add it
                if (HasAbstractEntity(abstractEntityValued.AbstractEntity))
                    return Message.RelationExistsAlready;

                // Add the valued abstract entity to the abstract entities
                Database.Current.Insert(this.ID, GenericTables.EntityAbstractEntity, Columns.AbstractEntityValued, abstractEntityValued);
                NotifyPropertyChanged("PersonalAbstractEntities");
                NotifyPropertyChanged("AbstractEntities");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddAbstractEntity(AbstractEntityValued abstractEntityValued)

        #region Method: RemoveAbstractEntity(AbstractEntityValued abstractEntityValued)
        /// <summary>
        /// Removes a valued abstract entity.
        /// </summary>
        /// <param name="abstractEntityValued">The valued abstract entity to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveAbstractEntity(AbstractEntityValued abstractEntityValued)
        {
            if (abstractEntityValued != null)
            {
                if (this.AbstractEntities.Contains(abstractEntityValued))
                {
                    // Remove the valued abstract entity
                    Database.Current.Remove(this.ID, GenericTables.EntityAbstractEntity, Columns.AbstractEntityValued, abstractEntityValued);
                    NotifyPropertyChanged("PersonalAbstractEntities");
                    NotifyPropertyChanged("AbstractEntities");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveAbstractEntity(AbstractEntityValued abstractEntityValued)

        #region Method: OverrideAbstractEntity(AbstractEntityValued inheritedAbstractEntity)
        /// <summary>
        /// Override the given inherited abstract entity.
        /// </summary>
        /// <param name="inheritedAbstractEntity">The inherited abstract entity that should be overridden.</param>
        /// <returns>Returns whether the override has been successful.</returns>
        public Message OverrideAbstractEntity(AbstractEntityValued inheritedAbstractEntity)
        {
            if (inheritedAbstractEntity != null && inheritedAbstractEntity.AbstractEntity != null && this.InheritedAbstractEntities.Contains(inheritedAbstractEntity))
            {
                // If the valued abstract entity is already available, there is no use to add it
                foreach (AbstractEntityValued personalAbstractEntity in this.PersonalAbstractEntities)
                {
                    if (inheritedAbstractEntity.AbstractEntity.Equals(personalAbstractEntity.AbstractEntity))
                        return Message.RelationExistsAlready;
                }
                if (HasOverriddenAbstractEntity(inheritedAbstractEntity.AbstractEntity))
                    return Message.RelationExistsAlready;

                // Copy the valued abstract entity and add it
                return AddOverriddenAbstractEntity(new AbstractEntityValued(inheritedAbstractEntity));
            }
            return Message.RelationFail;
        }
        #endregion Method: OverrideAbstractEntity(AbstractEntityValued inheritedAbstractEntity)

        #region Method: AddOverriddenAbstractEntity(AbstractEntityValued inheritedAbstractEntity)
        /// <summary>
        /// Add the given overridden abstract entity.
        /// </summary>
        /// <param name="overriddenAbstractEntity">The overridden abstract entity to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        private Message AddOverriddenAbstractEntity(AbstractEntityValued overriddenAbstractEntity)
        {
            if (overriddenAbstractEntity != null)
            {
                Database.Current.Insert(this.ID, GenericTables.EntityOverriddenAbstractEntity, Columns.AbstractEntityValued, overriddenAbstractEntity);
                NotifyPropertyChanged("OverriddenAbstractEntities");
                NotifyPropertyChanged("InheritedAbstractEntities");
                NotifyPropertyChanged("AbstractEntities");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddOverriddenAbstractEntity(AbstractEntityValued inheritedAbstractEntity)

        #region Method: RemoveOverriddenAbstractEntity(AbstractEntityValued overriddenAbstractEntity)
        /// <summary>
        /// Removes an overridden valued abstract entity.
        /// </summary>
        /// <param name="overriddenAbstractEntity">The overridden valued abstract entity to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveOverriddenAbstractEntity(AbstractEntityValued overriddenAbstractEntity)
        {
            if (overriddenAbstractEntity != null)
            {
                if (this.OverriddenAbstractEntities.Contains(overriddenAbstractEntity))
                {
                    // Remove the valued abstract entity
                    Database.Current.Remove(this.ID, GenericTables.EntityOverriddenAbstractEntity, Columns.AbstractEntityValued, overriddenAbstractEntity);
                    NotifyPropertyChanged("OverriddenAbstractEntities");
                    NotifyPropertyChanged("InheritedAbstractEntities");
                    NotifyPropertyChanged("AbstractEntities");
                    overriddenAbstractEntity.Remove();

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveOverriddenAbstractEntity(AbstractEntityValued overriddenAbstractEntity)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasAttribute(Attribute attribute)
        /// <summary>
        /// Checks if this entity has the given attribute.
        /// </summary>
        /// <param name="attribute">The attribute to check.</param>
        /// <returns>Returns true when the entity has the attribute.</returns>
        public bool HasAttribute(Attribute attribute)
        {
            if (attribute != null)
            {
                foreach (AttributeValued attributeValued in this.Attributes)
                {
                    if (attribute.Equals(attributeValued.Attribute))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasAttribute(Attribute attribute)

        #region Method: HasOverriddenAttribute(Attribute attribute)
        /// <summary>
        /// Checks if this entity has the given overridden attribute.
        /// </summary>
        /// <param name="attribute">The attribute to check.</param>
        /// <returns>Returns true when the entity has the overridden attribute.</returns>
        private bool HasOverriddenAttribute(Attribute attribute)
        {
            if (attribute != null)
            {
                foreach (AttributeValued attributeValued in this.OverriddenAttributes)
                {
                    if (attribute.Equals(attributeValued.Attribute))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasOverriddenAttribute(Attribute attribute)

        #region Method: HasStateGroup(StateGroup stateGroup)
        /// <summary>
        /// Checks if this entity has the given state group.
        /// </summary>
        /// <param name="stateGroup">The state group to check.</param>
        /// <returns>Returns true when the entity has the state group.</returns>
        public bool HasStateGroup(StateGroup stateGroup)
        {
            if (stateGroup != null)
            {
                foreach (StateGroup myStateGroup in this.StateGroups)
                {
                    if (stateGroup.Equals(myStateGroup))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasStateGroup(StateGroup stateGroup)

        #region Method: HasEvent(Event even)
        /// <summary>
        /// Checks if this entity has the given event.
        /// </summary>
        /// <param name="even">The event to check.</param>
        /// <returns>Returns true when the entity has the event.</returns>
        public bool HasEvent(Event even)
        {
            if (even != null)
            {
                foreach (Event myEvent in this.Events)
                {
                    if (even.Equals(myEvent))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasEvent(Event even)

        #region Method: HasVariable(Variable variable)
        /// <summary>
        /// Checks if this entity has the given variable.
        /// </summary>
        /// <param name="variable">The variable to check.</param>
        /// <returns>Returns true when this entity has the variable.</returns>
        public bool HasVariable(Variable variable)
        {
            if (variable != null)
            {
                foreach (Variable myVariable in this.Variables)
                {
                    if (variable.Equals(myVariable))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasVariable(Variable variable)

        #region Method: HasReference(Reference reference)
        /// <summary>
        /// Checks if this entity has the given reference.
        /// </summary>
        /// <param name="reference">The reference to check.</param>
        /// <returns>Returns true when this entity has the reference.</returns>
        public bool HasReference(Reference reference)
        {
            if (reference != null)
            {
                foreach (Reference myReference in this.References)
                {
                    if (reference.Equals(myReference))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasReference(Reference reference)

        #region Method: HasAbstractEntity(AbstractEntity abstractEntity)
        /// <summary>
        /// Checks if this entity has the given abstract entity.
        /// </summary>
        /// <param name="abstractEntity">The abstract entity to check.</param>
        /// <returns>Returns true when the entity has the abstractEntity.</returns>
        public bool HasAbstractEntity(AbstractEntity abstractEntity)
        {
            if (abstractEntity != null)
            {
                foreach (AbstractEntityValued abstractEntityValued in this.AbstractEntities)
                {
                    if (abstractEntity.Equals(abstractEntityValued.AbstractEntity))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasAbstractEntity(AbstractEntity abstractEntity)

        #region Method: HasOverriddenAbstractEntity(AbstractEntity abstractEntity)
        /// <summary>
        /// Checks if this entity has the given overridden abstract entity.
        /// </summary>
        /// <param name="abstractEntity">The abstract entity to check.</param>
        /// <returns>Returns true when the entity has the overridden abstractEntity.</returns>
        private bool HasOverriddenAbstractEntity(AbstractEntity abstractEntity)
        {
            if (abstractEntity != null)
            {
                foreach (AbstractEntityValued abstractEntityValued in this.OverriddenAbstractEntities)
                {
                    if (abstractEntity.Equals(abstractEntityValued.AbstractEntity))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasOverriddenAbstractEntity(AbstractEntity abstractEntity)

        #region Method: Remove()
        /// <summary>
        /// Remove the entity.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the attributes
            foreach (AttributeValued attributeValued in this.PersonalAttributes)
                attributeValued.Remove();
            Database.Current.Remove(this.ID, GenericTables.EntityAttribute);

            // Remove the overridden attributes
            foreach (AttributeValued attributeValued in this.OverriddenAttributes)
                attributeValued.Remove();
            Database.Current.Remove(this.ID, GenericTables.EntityOverriddenAttribute);

            // Remove the state groups
            foreach (StateGroup stateGroup in this.PersonalStateGroups)
                stateGroup.Remove();
            Database.Current.Remove(this.ID, GenericTables.EntityStateGroup);

            // Remove the variables
            foreach (Variable variable in this.PersonalVariables)
                variable.Remove();
            Database.Current.Remove(this.ID, GenericTables.EntityVariable);

            // Remove the references
            foreach (Reference reference in this.PersonalReferences)
                reference.Remove();
            Database.Current.Remove(this.ID, GenericTables.EntityReference);

            // Remove the combined relationships
            foreach (CombinedRelationship combinedRelationship in this.CombinedRelationships)
                combinedRelationship.Remove();
            Database.Current.Remove(this.ID, GenericTables.EntityCombinedRelationship);

            // Remove the abstract entities
            foreach (AbstractEntityValued abstractEntityValued in this.PersonalAbstractEntities)
                abstractEntityValued.Remove();
            Database.Current.Remove(this.ID, GenericTables.EntityAbstractEntity);

            // Remove the overridden abstract entities
            foreach (AbstractEntityValued abstractEntityValued in this.OverriddenAbstractEntities)
                abstractEntityValued.Remove();
            Database.Current.Remove(this.ID, GenericTables.EntityOverriddenAbstractEntity);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: Clone()
        /// <summary>
        /// Clones the entity.
        /// </summary>
        /// <returns>A clone of the entity.</returns>
        public new Entity Clone()
        {
            return base.Clone() as Entity;
        }
        #endregion Method: Clone()

        #region Method: CompareTo(Entity other)
        /// <summary>
        /// Compares the entity to the other entity.
        /// </summary>
        /// <param name="other">The entity to compare to this entity.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(Entity other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(Entity other)

        #endregion Method Group: Other

    }
    #endregion Class: Entity

    #region Class: EntityValued
    /// <summary>
    /// A valued version of an entity.
    /// </summary>
    public abstract class EntityValued : NodeValued
    {

        #region Properties and Fields

        #region Property: Entity
        /// <summary>
        /// Gets the entity of which this is an valued entity.
        /// </summary>
        public Entity Entity
        {
            get
            {
                return this.Node as Entity;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: Entity

        #region Property: Quantity
        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        public NumericalValueRange Quantity
        {
            get
            {
                return Database.Current.Select<NumericalValueRange>(this.ID, ValueTables.EntityValued, Columns.Quantity);
            }
            private set
            {
                Database.Current.Update(this.ID, ValueTables.EntityValued, Columns.Quantity, value);
                NotifyPropertyChanged("Quantity");
            }
        }
        #endregion Property: Quantity

        #region Property: Necessity
        /// <summary>
        /// Gets or sets the necessity.
        /// </summary>
        public Necessity Necessity
        {
            get
            {
                return Database.Current.Select<Necessity>(this.ID, ValueTables.EntityValued, Columns.Necessity);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.EntityValued, Columns.Necessity, value);
                NotifyPropertyChanged("Necessity");
            }
        }
        #endregion Property: Necessity

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: EntityValued(uint id)
        /// <summary>
        /// Creates a new valued entity from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the valued entity from.</param>
        protected EntityValued(uint id)
            : base(id)
        {
            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);
        }
        #endregion Constructor: EntityValued(uint id)

        #region Constructor: EntityValued(EntityValued entityValued)
        /// <summary>
        /// Clones a valued entity.
        /// </summary>
        /// <param name="entityValued">The valued entity to clone.</param>
        protected EntityValued(EntityValued entityValued)
            : base(entityValued)
        {
            if (entityValued != null)
            {
                Database.Current.StartChange();

                if (entityValued.Quantity != null)
                    this.Quantity = new NumericalValueRange(entityValued.Quantity);
                this.Necessity = entityValued.Necessity;

                Database.Current.StopChange();
            }

            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);
        }
        #endregion Constructor: EntityValued(EntityValued entityValued)

        #region Constructor: EntityValued(Entity entity)
        /// <summary>
        /// Creates a new valued entity from the given entity.
        /// </summary>
        /// <param name="entity">The entity to create the valued entity from.</param>
        protected EntityValued(Entity entity)
            : this(entity, new NumericalValueRange(SemanticsSettings.Values.Quantity))
        {
        }
        #endregion Constructor: EntityValued(Entity entity)

        #region Constructor: EntityValued(Entity entity, NumericalValueRange quantity)
        /// <summary>
        /// Creates a new valued entity from the given entity in the given quantity.
        /// </summary>
        /// <param name="entity">The entity to create the valued entity from.</param>
        /// <param name="quantity">The quantity of the valued entity.</param>
        protected EntityValued(Entity entity, NumericalValueRange quantity)
            : base(entity)
        {
            Database.Current.StartChange();

            // Get the special quantity unit category and subscribe to its changes
            UnitCategory quantityUnitCategory = SemanticsManager.GetSpecialUnitCategory(SpecialUnitCategories.Quantity);
            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);

            if (quantity != null)
            {
                quantity.UnitCategory = quantityUnitCategory;
                this.Quantity = quantity;
            }
            else
                this.Quantity = new NumericalValueRange(SemanticsSettings.Values.Quantity, quantityUnitCategory);

            Database.Current.StopChange();
        }
        #endregion Constructor: EntityValued(Entity entity, NumericalValueRange quantity)

        #region Method: Create(Entity entity)
        /// <summary>
        /// Create a valued entity of the given entity.
        /// </summary>
        /// <param name="entity">The entity to create a valued entity of.</param>
        /// <returns>A valued entity of the given entity.</returns>
        public static EntityValued Create(Entity entity)
        {
            PhysicalEntity physicalEntity = entity as PhysicalEntity;
            if (physicalEntity != null)
                return PhysicalEntityValued.Create(physicalEntity);

            AbstractEntity abstractEntity = entity as AbstractEntity;
            if (abstractEntity != null)
                return new AbstractEntityValued(abstractEntity);

            return null;
        }
        #endregion Method: Create(Entity entity)

        #region Method: SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)
        /// <summary>
        /// Change the special quantity unit category.
        /// </summary>
        /// <param name="specialUnitCategory">The special unit category.</param>
        /// <param name="unitCategory">The unit category.</param>
        private void SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)
        {
            if (specialUnitCategory == SpecialUnitCategories.Quantity && this.Quantity != null)
                this.Quantity.UnitCategory = unitCategory;
        }
        #endregion Method: SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the valued entity.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the quantity
            if (this.Quantity != null)
                this.Quantity.Remove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: Clone()
        /// <summary>
        /// Clones the valued entity.
        /// </summary>
        /// <returns>A clone of the valued entity.</returns>
        public new EntityValued Clone()
        {
            return base.Clone() as EntityValued;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: EntityValued

    #region Class: EntityCondition
    /// <summary>
    /// A condition on an entity.
    /// </summary>
    public abstract class EntityCondition : NodeCondition
    {

        #region Properties and Fields

        #region Property: Entity
        /// <summary>
        /// Gets or sets the required entity.
        /// </summary>
        public Entity Entity
        {
            get
            {
                return this.Node as Entity;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Entity

        #region Property: Quantity
        /// <summary>
        /// Gets or sets the required quantity.
        /// </summary>
        public NumericalValueCondition Quantity
        {
            get
            {
                return Database.Current.Select<NumericalValueCondition>(this.ID, ValueTables.EntityCondition, Columns.Quantity);
            }
            set
            {
                if (this.Quantity != null)
                    this.Quantity.Remove();

                Database.Current.Update(this.ID, ValueTables.EntityCondition, Columns.Quantity, value);
                NotifyPropertyChanged("Quantity");

                // Set the special quantity unit category
                if (value != null)
                    value.UnitCategory = SemanticsManager.GetSpecialUnitCategory(SpecialUnitCategories.Quantity);
            }
        }
        #endregion Property: Quantity

        #region Property: Attributes
        /// <summary>
        /// Gets the required attributes.
        /// </summary>
        public ReadOnlyCollection<AttributeCondition> Attributes
        {
            get
            {
                return Database.Current.SelectAll<AttributeCondition>(this.ID, ValueTables.EntityConditionAttributeCondition, Columns.AttributeCondition).AsReadOnly();
            }
        }
        #endregion Property: Attributes

        #region Property: StateGroups
        /// <summary>
        /// Gets the required state groups.
        /// </summary>
        public ReadOnlyCollection<StateGroupCondition> StateGroups
        {
            get
            {
                return Database.Current.SelectAll<StateGroupCondition>(this.ID, ValueTables.EntityConditionStateGroupCondition, Columns.StateGroupCondition).AsReadOnly();
            }
        }
        #endregion Property: StateGroups

        #region Property: Relationships
        /// <summary>
        /// Gets the required relationships.
        /// </summary>
        public ReadOnlyCollection<RelationshipCondition> Relationships
        {
            get
            {
                return Database.Current.SelectAll<RelationshipCondition>(this.ID, ValueTables.EntityConditionRelationshipCondition, Columns.RelationshipCondition).AsReadOnly();
            }
        }
        #endregion Property: Relationships

        #region Property: Actions
        /// <summary>
        /// Gets the actions that this entity should be able to perform.
        /// </summary>
        public ReadOnlyCollection<Action> Actions
        {
            get
            {
                return Database.Current.SelectAll<Action>(this.ID, ValueTables.EntityConditionAction, Columns.Action).AsReadOnly();
            }
        }
        #endregion Property: Actions

        #region Property: AbstractEntities
        /// <summary>
        /// Gets the required abstractEntities.
        /// </summary>
        public ReadOnlyCollection<AbstractEntityCondition> AbstractEntities
        {
            get
            {
                return Database.Current.SelectAll<AbstractEntityCondition>(this.ID, ValueTables.EntityConditionAbstractEntityCondition, Columns.AbstractEntityCondition).AsReadOnly();
            }
        }
        #endregion Property: AbstractEntities

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: EntityCondition()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static EntityCondition()
        {
            // Attributes
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.AttributeCondition, new Tuple<Type, EntryType>(typeof(AttributeCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.EntityConditionAttributeCondition, typeof(EntityCondition), dict);

            // State groups
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.StateGroupCondition, new Tuple<Type, EntryType>(typeof(StateGroupCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.EntityConditionStateGroupCondition, typeof(EntityCondition), dict);

            // Relationships
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.RelationshipCondition, new Tuple<Type, EntryType>(typeof(RelationshipCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.EntityConditionRelationshipCondition, typeof(EntityCondition), dict);

            // Actions
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Action, new Tuple<Type, EntryType>(typeof(Action), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.EntityConditionAction, typeof(EntityCondition), dict);

            // Attributes
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.AbstractEntityCondition, new Tuple<Type, EntryType>(typeof(AbstractEntityCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.EntityConditionAbstractEntityCondition, typeof(EntityCondition), dict);
        }
        #endregion Static Constructor: EntityCondition()

        #region Constructor: EntityCondition()
        /// <summary>
        /// Creates a new entity condition.
        /// </summary>
        protected EntityCondition()
            : base()
        {
            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);
        }
        #endregion Constructor: EntityCondition()

        #region Constructor: EntityCondition(uint id)
        /// <summary>
        /// Creates a new entity condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create an entity condition from.</param>
        protected EntityCondition(uint id)
            : base(id)
        {
            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);
        }
        #endregion Constructor: EntityCondition(uint id)

        #region Constructor: EntityCondition(EntityCondition entityCondition)
        /// <summary>
        /// Clones an entity condition.
        /// </summary>
        /// <param name="entityCondition">The entity condition to clone.</param>
        protected EntityCondition(EntityCondition entityCondition)
            : base(entityCondition)
        {
            if (entityCondition != null)
            {
                Database.Current.StartChange();

                if (entityCondition.Quantity != null)
                    this.Quantity = new NumericalValueCondition(entityCondition.Quantity);
                foreach (AttributeCondition attributeCondition in entityCondition.Attributes)
                    AddAttribute(new AttributeCondition(attributeCondition));
                foreach (StateGroupCondition stateGroupCondition in entityCondition.StateGroups)
                    AddStateGroup(new StateGroupCondition(stateGroupCondition));
                foreach (RelationshipCondition relationshipCondition in entityCondition.Relationships)
                    AddRelationship(new RelationshipCondition(relationshipCondition));
                foreach (Action action in entityCondition.Actions)
                    AddAction(new Action(action));
                foreach (AbstractEntityCondition abstractEntityCondition in entityCondition.AbstractEntities)
                    AddAbstractEntity(new AbstractEntityCondition(abstractEntityCondition));

                Database.Current.StopChange();
            }

            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);
        }
        #endregion Constructor: EntityCondition(EntityCondition entityCondition)

        #region Constructor: EntityCondition(Entity entity)
        /// <summary>
        /// Creates a condition for the given entity.
        /// </summary>
        /// <param name="entity">The entity to create a condition for.</param>
        protected EntityCondition(Entity entity)
            : base(entity)
        {
            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);
        }
        #endregion Constructor: EntityCondition(Entity entity)

        #region Constructor: EntityCondition(Entity entity, NumericalValueCondition quantity)
        /// <summary>
        /// Creates a condition for the given entity in the given quantity.
        /// </summary>
        /// <param name="entity">The entity to create a condition for.</param>
        /// <param name="quantity">The quantity of the entity condition.</param>
        protected EntityCondition(Entity entity, NumericalValueCondition quantity)
            : base(entity)
        {
            Database.Current.StartChange();

            if (quantity != null)
                this.Quantity = quantity;

            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);

            Database.Current.StopChange();
        }
        #endregion Constructor: EntityCondition(Entity entity, NumericalValueCondition quantity)

        #region Method: SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)
        /// <summary>
        /// Change the special quantity unit category.
        /// </summary>
        /// <param name="specialUnitCategory">The special unit category.</param>
        /// <param name="unitCategory">The unit category.</param>
        private void SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)
        {
            if (specialUnitCategory == SpecialUnitCategories.Quantity && this.Quantity != null)
                this.Quantity.UnitCategory = unitCategory;
        }
        #endregion Method: SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)

        #region Method: Create(Entity entity)
        /// <summary>
        /// Create an entity condition of the given entity.
        /// </summary>
        /// <param name="entity">The entity to create an entity condition of.</param>
        /// <returns>An entity condition of the given entity.</returns>
        public static EntityCondition Create(Entity entity)
        {
            AbstractEntity abstractEntity = entity as AbstractEntity;
            if (abstractEntity != null)
                return new AbstractEntityCondition(abstractEntity);

            PhysicalEntity physicalEntity = entity as PhysicalEntity;
            if (physicalEntity != null)
                return PhysicalEntityCondition.Create(physicalEntity);

            return null;
        }
        #endregion Method: Create(Entity entity)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddAttribute(AttributeCondition attributeCondition)
        /// <summary>
        /// Adds an attribute condition.
        /// </summary>
        /// <param name="attributeCondition">The attribute condition to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddAttribute(AttributeCondition attributeCondition)
        {
            if (attributeCondition != null)
            {
                // If the attribute condition is already available in all attributes, there is no use to add it
                if (HasAttribute(attributeCondition.Attribute))
                    return Message.RelationExistsAlready;

                // Add the attribute condition to the attributes
                Database.Current.Insert(this.ID, ValueTables.EntityConditionAttributeCondition, Columns.AttributeCondition, attributeCondition);
                NotifyPropertyChanged("Attributes");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddAttribute(AttributeCondition attributeCondition)

        #region Method: RemoveAttribute(AttributeCondition attributeCondition)
        /// <summary>
        /// Removes an attribute condition.
        /// </summary>
        /// <param name="attributeCondition">The attribute condition to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveAttribute(AttributeCondition attributeCondition)
        {
            if (attributeCondition != null)
            {
                if (this.Attributes.Contains(attributeCondition))
                {
                    // Remove the attribute condition
                    Database.Current.Remove(this.ID, ValueTables.EntityConditionAttributeCondition, Columns.AttributeCondition, attributeCondition);
                    NotifyPropertyChanged("Attributes");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveAttribute(AttributeCondition attributeCondition)

        #region Method: AddStateGroup(StateGroupCondition stateGroupCondition)
        /// <summary>
        /// Adds a state group condition.
        /// </summary>
        /// <param name="stateGroupCondition">The state group to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddStateGroup(StateGroupCondition stateGroupCondition)
        {
            if (stateGroupCondition != null)
            {
                // If the state group condition is already available in all state groups, there is no use to add it
                if (HasStateGroup(stateGroupCondition))
                    return Message.RelationExistsAlready;

                // Add the state group condition to the state groups
                Database.Current.Insert(this.ID, ValueTables.EntityConditionStateGroupCondition, Columns.StateGroupCondition, stateGroupCondition);
                NotifyPropertyChanged("StateGroups");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddStateGroup(StateGroupCondition stateGroupCondition)

        #region Method: RemoveStateGroup(StateGroupCondition stateGroupCondition)
        /// <summary>
        /// Removes a state group condition.
        /// </summary>
        /// <param name="stateGroupCondition">The state group condition to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveStateGroup(StateGroupCondition stateGroupCondition)
        {
            if (stateGroupCondition != null)
            {
                if (this.StateGroups.Contains(stateGroupCondition))
                {
                    // Remove the state group condition
                    Database.Current.Remove(this.ID, ValueTables.EntityConditionStateGroupCondition, Columns.StateGroupCondition, stateGroupCondition);
                    NotifyPropertyChanged("StateGroups");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveStateGroup(StateGroupCondition stateGroupCondition)

        #region Method: AddRelationship(RelationshipCondition relationshipCondition)
        /// <summary>
        /// Add an relationship condition.
        /// </summary>
        /// <param name="relationshipCondition">The relationship condition to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddRelationship(RelationshipCondition relationshipCondition)
        {
            if (relationshipCondition != null)
            {
                // If the relationship condition is already available in all relationships, there is no use to add it
                if (this.Relationships.Contains(relationshipCondition))
                    return Message.RelationExistsAlready;

                // Add the relationship condition to the relationships
                Database.Current.Insert(this.ID, ValueTables.EntityConditionRelationshipCondition, Columns.RelationshipCondition, relationshipCondition);
                NotifyPropertyChanged("Relationships");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddRelationship(RelationshipCondition relationshipCondition)

        #region Method: RemoveRelationship(RelationshipCondition relationshipCondition)
        /// <summary>
        /// Removes a relationship condition.
        /// </summary>
        /// <param name="relationshipCondition">The relationship condition to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveRelationship(RelationshipCondition relationshipCondition)
        {
            if (relationshipCondition != null)
            {
                if (this.Relationships.Contains(relationshipCondition))
                {
                    // Remove the relationship condition
                    Database.Current.Remove(this.ID, ValueTables.EntityConditionRelationshipCondition, Columns.RelationshipCondition, relationshipCondition);
                    NotifyPropertyChanged("Relationships");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveRelationship(RelationshipCondition relationshipCondition)

        #region Method: AddAction(Action action)
        /// <summary>
        /// Adds an action.
        /// </summary>
        /// <param name="action">The action to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddAction(Action action)
        {
            if (action != null)
            {
                // If the action is already available in all actions, there is no use to add it
                if (HasAction(action))
                    return Message.RelationExistsAlready;

                // Add the action to the actions
                Database.Current.Insert(this.ID, ValueTables.EntityConditionAction, Columns.Action, action);
                NotifyPropertyChanged("Actions");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddAction(Action action)

        #region Method: RemoveAction(Action action)
        /// <summary>
        /// Removes an action.
        /// </summary>
        /// <param name="action">The action to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveAction(Action action)
        {
            if (action != null)
            {
                if (HasAction(action))
                {
                    // Remove the action
                    Database.Current.Remove(this.ID, ValueTables.EntityConditionAction, Columns.Action, action);
                    NotifyPropertyChanged("Actions");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveAction(Action action)

        #region Method: AddAbstractEntity(AbstractEntityCondition abstractEntityCondition)
        /// <summary>
        /// Adds an abstract entity condition.
        /// </summary>
        /// <param name="abstractEntityCondition">The abstract entity condition to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddAbstractEntity(AbstractEntityCondition abstractEntityCondition)
        {
            if (abstractEntityCondition != null)
            {
                // If the abstract entity condition is already available in all abstractEntities, there is no use to add it
                if (HasAbstractEntity(abstractEntityCondition.AbstractEntity))
                    return Message.RelationExistsAlready;

                // Add the abstract entity condition to the abstractEntities
                Database.Current.Insert(this.ID, ValueTables.EntityConditionAbstractEntityCondition, Columns.AbstractEntityCondition, abstractEntityCondition);
                NotifyPropertyChanged("AbstractEntities");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddAbstractEntity(AbstractEntityCondition abstractEntityCondition)

        #region Method: RemoveAbstractEntity(AbstractEntityCondition abstractEntityCondition)
        /// <summary>
        /// Removes an abstract entity condition.
        /// </summary>
        /// <param name="abstractEntityCondition">The abstract entity condition to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveAbstractEntity(AbstractEntityCondition abstractEntityCondition)
        {
            if (abstractEntityCondition != null)
            {
                if (this.AbstractEntities.Contains(abstractEntityCondition))
                {
                    // Remove the abstract entity condition
                    Database.Current.Remove(this.ID, ValueTables.EntityConditionAbstractEntityCondition, Columns.AbstractEntityCondition, abstractEntityCondition);
                    NotifyPropertyChanged("AbstractEntities");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveAbstractEntity(AbstractEntityCondition abstractEntityCondition)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasAttribute(Attribute attribute)
        /// <summary>
        /// Checks if this entity condition has the given attribute.
        /// </summary>
        /// <param name="attribute">The attribute to check.</param>
        /// <returns>Returns true when the entity condition has the attribute.</returns>
        public bool HasAttribute(Attribute attribute)
        {
            if (attribute != null)
            {
                foreach (AttributeCondition attributeCondition in this.Attributes)
                {
                    if (attribute.Equals(attributeCondition.Attribute))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasAttribute(Attribute attribute)

        #region Method: HasStateGroup(StateGroupCondition stateGroupCondition)
        /// <summary>
        /// Checks if this entity condition has the given state group.
        /// </summary>
        /// <param name="stateGroup">The state group to check.</param>
        /// <returns>Returns true when the entity condition has the state group.</returns>
        public bool HasStateGroup(StateGroupCondition stateGroupCondition)
        {
            if (stateGroupCondition != null)
            {
                foreach (StateGroupCondition myStateGroupCondition in this.StateGroups)
                {
                    if (stateGroupCondition.Equals(myStateGroupCondition))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasStateGroup(StateGroupCondition stateGroupCondition)

        #region Method: HasAction(Action action)
        /// <summary>
        /// Checks if this entity condition has the given action.
        /// </summary>
        /// <param name="action">The action to check.</param>
        /// <returns>Returns true when the entity condition has the action.</returns>
        public bool HasAction(Action action)
        {
            if (action != null)
            {
                foreach (Action myAction in this.Actions)
                {
                    if (action.Equals(myAction))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasAction(Action action)

        #region Method: HasAbstractEntity(AbstractEntity abstractEntity)
        /// <summary>
        /// Checks if this entity condition has the given abstract entity.
        /// </summary>
        /// <param name="abstractEntity">The abstract entity to check.</param>
        /// <returns>Returns true when the entity condition has the abstract entity.</returns>
        public bool HasAbstractEntity(AbstractEntity abstractEntity)
        {
            if (abstractEntity != null)
            {
                foreach (AbstractEntityCondition abstractEntityCondition in this.AbstractEntities)
                {
                    if (abstractEntity.Equals(abstractEntityCondition.AbstractEntity))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasAbstractEntity(AbstractEntity abstractEntity)

        #region Method: Remove()
        /// <summary>
        /// Remove the entity condition.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the quantity
            if (this.Quantity != null)
                this.Quantity.Remove();

            // Remove the attributes
            foreach (AttributeCondition attributeCondition in this.Attributes)
                attributeCondition.Remove();
            Database.Current.Remove(this.ID, ValueTables.EntityConditionAttributeCondition);

            // Remove the state groups
            foreach (StateGroupCondition stateGroupCondition in this.StateGroups)
                stateGroupCondition.Remove();
            Database.Current.Remove(this.ID, ValueTables.EntityConditionStateGroupCondition);

            // Remove the relationships
            foreach (RelationshipCondition relationshipCondition in this.Relationships)
                relationshipCondition.Remove();
            Database.Current.Remove(this.ID, ValueTables.EntityConditionRelationshipCondition);

            // Remove the actions
            Database.Current.Remove(this.ID, ValueTables.EntityConditionAction);

            // Remove the abstract entities
            foreach (AbstractEntityCondition abstractEntityCondition in this.AbstractEntities)
                abstractEntityCondition.Remove();
            Database.Current.Remove(this.ID, ValueTables.EntityConditionAbstractEntityCondition);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: Clone()
        /// <summary>
        /// Clones the entity condition.
        /// </summary>
        /// <returns>A clone of the entity condition.</returns>
        public new EntityCondition Clone()
        {
            return base.Clone() as EntityCondition;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: EntityCondition

    #region Class: EntityChange
    /// <summary>
    /// A change on an entity.
    /// </summary>
    public abstract class EntityChange : NodeChange
    {

        #region Properties and Fields

        #region Property: Entity
        /// <summary>
        /// Gets the affected entity.
        /// </summary>
        public Entity Entity
        {
            get
            {
                return this.Node as Entity;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: Entity

        #region Property: Quantity
        /// <summary>
        /// Gets or sets the quantity change.
        /// </summary>
        public NumericalValueChange Quantity
        {
            get
            {
                return Database.Current.Select<NumericalValueChange>(this.ID, ValueTables.EntityChange, Columns.Quantity);
            }
            set
            {
                if (this.Quantity != null)
                    this.Quantity.Remove();

                Database.Current.Update(this.ID, ValueTables.EntityChange, Columns.Quantity, value);
                NotifyPropertyChanged("Quantity");

                // Set the special quantity unit category
                if (value != null)
                    value.UnitCategory = SemanticsManager.GetSpecialUnitCategory(SpecialUnitCategories.Quantity);
            }
        }
        #endregion Property: Quantity

        #region Property: Attributes
        /// <summary>
        /// Gets the attributes to change.
        /// </summary>
        public ReadOnlyCollection<AttributeChange> Attributes
        {
            get
            {
                return Database.Current.SelectAll<AttributeChange>(this.ID, ValueTables.EntityChangeAttributeChange, Columns.AttributeChange).AsReadOnly();
            }
        }
        #endregion Property: Attributes

        #region Property: AttributesToAdd
        /// <summary>
        /// Gets the attributes that should be added during the change.
        /// </summary>
        public ReadOnlyCollection<AttributeValued> AttributesToAdd
        {
            get
            {
                return Database.Current.SelectAll<AttributeValued>(this.ID, ValueTables.EntityChangeAttributeToAdd, Columns.AttributeValued).AsReadOnly();
            }
        }
        #endregion Property: AttributesToAdd

        #region Property: AttributesToRemove
        /// <summary>
        /// Gets the attributes that should be removed during the change.
        /// </summary>
        public ReadOnlyCollection<AttributeCondition> AttributesToRemove
        {
            get
            {
                return Database.Current.SelectAll<AttributeCondition>(this.ID, ValueTables.EntityChangeAttributeToRemove, Columns.AttributeCondition).AsReadOnly();
            }
        }
        #endregion Property: AttributesToRemove

        #region Property: StateGroups
        /// <summary>
        /// Gets the state groups to change.
        /// </summary>
        public ReadOnlyCollection<StateGroupChange> StateGroups
        {
            get
            {
                return Database.Current.SelectAll<StateGroupChange>(this.ID, ValueTables.EntityChangeStateGroupChange, Columns.StateGroupChange).AsReadOnly();
            }
        }
        #endregion Property: StateGroups

        #region Property: Relationships
        /// <summary>
        /// Gets the relationships to change.
        /// </summary>
        public ReadOnlyCollection<RelationshipChange> Relationships
        {
            get
            {
                return Database.Current.SelectAll<RelationshipChange>(this.ID, ValueTables.EntityChangeRelationshipChange, Columns.RelationshipChange).AsReadOnly();
            }
        }
        #endregion Property: Relationships

        #region Property: AbstractEntities
        /// <summary>
        /// Gets the abstract entities to change.
        /// </summary>
        public ReadOnlyCollection<AbstractEntityChange> AbstractEntities
        {
            get
            {
                return Database.Current.SelectAll<AbstractEntityChange>(this.ID, ValueTables.EntityChangeAbstractEntityChange, Columns.AbstractEntityChange).AsReadOnly();
            }
        }
        #endregion Property: AbstractEntities

        #region Property: AbstractEntitiesToAdd
        /// <summary>
        /// Gets the abstract entities that should be added during the change.
        /// </summary>
        public ReadOnlyCollection<AbstractEntityValued> AbstractEntitiesToAdd
        {
            get
            {
                return Database.Current.SelectAll<AbstractEntityValued>(this.ID, ValueTables.EntityChangeAbstractEntityToAdd, Columns.AbstractEntityValued).AsReadOnly();
            }
        }
        #endregion Property: AbstractEntitiesToAdd

        #region Property: AbstractEntitiesToRemove
        /// <summary>
        /// Gets the abstract entities that should be removed during the change.
        /// </summary>
        public ReadOnlyCollection<AbstractEntityCondition> AbstractEntitiesToRemove
        {
            get
            {
                return Database.Current.SelectAll<AbstractEntityCondition>(this.ID, ValueTables.EntityChangeAbstractEntityToRemove, Columns.AbstractEntityCondition).AsReadOnly();
            }
        }
        #endregion Property: AbstractEntitiesToRemove

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: EntityChange()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static EntityChange()
        {
            // Attributes
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.AttributeChange, new Tuple<Type, EntryType>(typeof(AttributeChange), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.EntityChangeAttributeChange, typeof(EntityChange), dict);

            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.AttributeValued, new Tuple<Type, EntryType>(typeof(AttributeValued), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.EntityChangeAttributeToAdd, typeof(EntityChange), dict);

            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.AttributeCondition, new Tuple<Type, EntryType>(typeof(AttributeCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.EntityChangeAttributeToRemove, typeof(EntityChange), dict);

            // State groups
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.StateGroupChange, new Tuple<Type, EntryType>(typeof(StateGroupChange), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.EntityChangeStateGroupChange, typeof(EntityChange), dict);

            // Relationships
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.RelationshipChange, new Tuple<Type, EntryType>(typeof(RelationshipChange), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.EntityChangeRelationshipChange, typeof(EntityChange), dict);

            // AbstractEntities
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.AbstractEntityChange, new Tuple<Type, EntryType>(typeof(AbstractEntityChange), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.EntityChangeAbstractEntityChange, typeof(EntityChange), dict);

            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.AbstractEntityValued, new Tuple<Type, EntryType>(typeof(AbstractEntityValued), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.EntityChangeAbstractEntityToAdd, typeof(EntityChange), dict);

            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.AbstractEntityCondition, new Tuple<Type, EntryType>(typeof(AbstractEntityCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.EntityChangeAbstractEntityToRemove, typeof(EntityChange), dict);
        }
        #endregion Static Constructor: EntityChange()

        #region Constructor: EntityChange()
        /// <summary>
        /// Creates a new entity change.
        /// </summary>
        protected EntityChange()
            : base()
        {
            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);
        }
        #endregion Constructor: EntityChange()

        #region Constructor: EntityChange(uint id)
        /// <summary>
        /// Creates a new entity change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create an entity change from.</param>
        protected EntityChange(uint id)
            : base(id)
        {
            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);
        }
        #endregion Constructor: EntityChange(uint id)

        #region Constructor: EntityChange(EntityChange entityChange)
        /// <summary>
        /// Clones an entity change.
        /// </summary>
        /// <param name="entityChange">The entity change to clone.</param>
        protected EntityChange(EntityChange entityChange)
            : base(entityChange)
        {
            if (entityChange != null)
            {
                Database.Current.StartChange();

                if (entityChange.Quantity != null)
                    this.Quantity = new NumericalValueChange(entityChange.Quantity);
                foreach (AttributeChange attributeChange in entityChange.Attributes)
                    AddAttribute(new AttributeChange(attributeChange));
                foreach (AttributeValued attributeValued in entityChange.AttributesToAdd)
                    AddAttributeToAdd(new AttributeValued(attributeValued));
                foreach (AttributeCondition attributeCondition in entityChange.AttributesToRemove)
                    AddAttributeToRemove(new AttributeCondition(attributeCondition));
                foreach (StateGroupChange stateGroupChange in entityChange.StateGroups)
                    AddStateGroup(new StateGroupChange(stateGroupChange));
                foreach (RelationshipChange relationshipChange in entityChange.Relationships)
                    AddRelationship(new RelationshipChange(relationshipChange));
                foreach (AbstractEntityChange abstractEntityChange in entityChange.AbstractEntities)
                    AddAbstractEntity(new AbstractEntityChange(abstractEntityChange));
                foreach (AbstractEntityValued abstractEntityValued in entityChange.AbstractEntitiesToAdd)
                    AddAbstractEntityToAdd(new AbstractEntityValued(abstractEntityValued));
                foreach (AbstractEntityCondition abstractEntityCondition in entityChange.AbstractEntitiesToRemove)
                    AddAbstractEntityToRemove(new AbstractEntityCondition(abstractEntityCondition));

                Database.Current.StopChange();
            }

            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);
        }
        #endregion Constructor: EntityChange(EntityChange entityChange)

        #region Constructor: EntityChange(Entity entity)
        /// <summary>
        /// Creates a change for the given entity.
        /// </summary>
        /// <param name="entity">The entity to create a change for.</param>
        protected EntityChange(Entity entity)
            : this(entity, null)
        {
        }
        #endregion Constructor: EntityChange(Entity entity)

        #region Constructor: EntityChange(Entity entity, NumericalValueChange quantity)
        /// <summary>
        /// Creates a change for the given entity in the form of the given quantity.
        /// </summary>
        /// <param name="entity">The entity to create a change for.</param>
        /// <param name="quantity">The change in quantity.</param>
        protected EntityChange(Entity entity, NumericalValueChange quantity)
            : base(entity)
        {
            Database.Current.StartChange();

            if (quantity != null)
                this.Quantity = quantity;

            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);

            Database.Current.StopChange();
        }
        #endregion Constructor: EntityChange(Entity entity, NumericalValueChange quantity)

        #region Method: SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)
        /// <summary>
        /// Change the special quantity unit category.
        /// </summary>
        /// <param name="specialUnitCategory">The special unit category.</param>
        /// <param name="unitCategory">The unit category.</param>
        private void SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)
        {
            if (specialUnitCategory == SpecialUnitCategories.Quantity && this.Quantity != null)
                this.Quantity.UnitCategory = unitCategory;
        }
        #endregion Method: SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)

        #region Method: Create(Entity entity)
        /// <summary>
        /// Create an entity change of the given entity.
        /// </summary>
        /// <param name="entity">The entity to create an entity change of.</param>
        /// <returns>An entity change of the given entity.</returns>
        public static EntityChange Create(Entity entity)
        {
            AbstractEntity abstractEntity = entity as AbstractEntity;
            if (abstractEntity != null)
                return new AbstractEntityChange(abstractEntity);

            PhysicalEntity physicalEntity = entity as PhysicalEntity;
            if (physicalEntity != null)
                return PhysicalEntityChange.Create(physicalEntity);

            return null;
        }
        #endregion Method: Create(Entity entity)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddAttribute(AttributeChange attributeChange)
        /// <summary>
        /// Adds an attribute change to the list with attributes.
        /// </summary>
        /// <param name="attributeChange">The attribute change to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddAttribute(AttributeChange attributeChange)
        {
            if (attributeChange != null)
            {
                // If the attribute change is already available in all attributes, there is no use to add it
                if (HasAttribute(attributeChange.Attribute))
                    return Message.RelationExistsAlready;

                // Add the attribute change to the attributes
                Database.Current.Insert(this.ID, ValueTables.EntityChangeAttributeChange, Columns.AttributeChange, attributeChange);
                NotifyPropertyChanged("Attributes");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddAttribute(AttributeChange attributeChange)

        #region Method: RemoveAttribute(AttributeChange attributeChange)
        /// <summary>
        /// Removes an attribute change from the list of attributes.
        /// </summary>
        /// <param name="attributeChange">The attribute change to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveAttribute(AttributeChange attributeChange)
        {
            if (attributeChange != null)
            {
                if (this.Attributes.Contains(attributeChange))
                {
                    // Remove the attribute change
                    Database.Current.Remove(this.ID, ValueTables.EntityChangeAttributeChange, Columns.AttributeChange, attributeChange);
                    NotifyPropertyChanged("Attributes");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveAttribute(AttributeChange attributeChange)

        #region Method: AddAttributeToAdd(AttributeValued attributeValued)
        /// <summary>
        /// Adds a valued attribute to the list with attributes to add.
        /// </summary>
        /// <param name="attributeValued">The valued attribute to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddAttributeToAdd(AttributeValued attributeValued)
        {
            if (attributeValued != null && attributeValued.Attribute != null)
            {
                // If the valued attribute is already available in all attributes, there is no use to add it
                if (HasAttributeToAdd(attributeValued.Attribute))
                    return Message.RelationExistsAlready;

                // Add the valued attribute to the attributes
                Database.Current.Insert(this.ID, ValueTables.EntityChangeAttributeToAdd, Columns.AttributeValued, attributeValued);
                NotifyPropertyChanged("AttributesToAdd");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddAttributeToAdd(AttributeValued attributeValued)

        #region Method: RemoveAttributeToAdd(AttributeValued attributeValued)
        /// <summary>
        /// Removes a valued attribute from the list of attributes to add.
        /// </summary>
        /// <param name="attributeValued">The valued attribute to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveAttributeToAdd(AttributeValued attributeValued)
        {
            if (attributeValued != null)
            {
                if (this.AttributesToAdd.Contains(attributeValued))
                {
                    // Remove the valued attribute
                    Database.Current.Remove(this.ID, ValueTables.EntityChangeAttributeToAdd, Columns.AttributeValued, attributeValued);
                    NotifyPropertyChanged("AttributesToAdd");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveAttributeToAdd(AttributeValued attributeValued)

        #region Method: AddAttributeToRemove(AttributeCondition attributeCondition)
        /// <summary>
        /// Adds an attribute condition to the list with attributes to remove.
        /// </summary>
        /// <param name="attributeCondition">The attribute condition to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddAttributeToRemove(AttributeCondition attributeCondition)
        {
            if (attributeCondition != null)
            {
                // If the attribute condition is already available in all attributes, there is no use to add it
                if (HasAttributeToRemove(attributeCondition.Attribute))
                    return Message.RelationExistsAlready;

                // Add the attribute condition to the attributes
                Database.Current.Insert(this.ID, ValueTables.EntityChangeAttributeToRemove, Columns.AttributeCondition, attributeCondition);
                NotifyPropertyChanged("AttributesToRemove");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddAttributeToRemove(AttributeCondition attributeCondition)

        #region Method: RemoveAttributeToRemove(AttributeCondition attributeCondition)
        /// <summary>
        /// Removes an attribute condition from the list of attributes to remove.
        /// </summary>
        /// <param name="attributeCondition">The attribute condition to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveAttributeToRemove(AttributeCondition attributeCondition)
        {
            if (attributeCondition != null)
            {
                if (this.AttributesToRemove.Contains(attributeCondition))
                {
                    // Remove the attribute condition
                    Database.Current.Remove(this.ID, ValueTables.EntityChangeAttributeToRemove, Columns.AttributeCondition, attributeCondition);
                    NotifyPropertyChanged("AttributesToRemove");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveAttributeToRemove(AttributeCondition attributeCondition)

        #region Method: AddStateGroup(StateGroupChange stateGroupChange)
        /// <summary>
        /// Adds a state group change to the list with state groups.
        /// </summary>
        /// <param name="stateGroupChange">The state group change to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddStateGroup(StateGroupChange stateGroupChange)
        {
            if (stateGroupChange != null)
            {
                // If the state group change is already available in all state groups, there is no use to add it
                if (this.StateGroups.Contains(stateGroupChange))
                    return Message.RelationExistsAlready;

                // Add the state group change to the state groups
                Database.Current.Insert(this.ID, ValueTables.EntityChangeStateGroupChange, Columns.StateGroupChange, stateGroupChange);
                NotifyPropertyChanged("StateGroups");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddStateGroup(StateGroupChange stateGroupChange)

        #region Method: RemoveStateGroup(StateGroupChange stateGroupChange)
        /// <summary>
        /// Removes a state group change from the list of state groups.
        /// </summary>
        /// <param name="stateGroupChange">The state group change to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveStateGroup(StateGroupChange stateGroupChange)
        {
            if (stateGroupChange != null)
            {
                if (this.StateGroups.Contains(stateGroupChange))
                {
                    // Remove the state group change
                    Database.Current.Remove(this.ID, ValueTables.EntityChangeStateGroupChange, Columns.StateGroupChange, stateGroupChange);
                    NotifyPropertyChanged("StateGroups");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveStateGroup(StateGroupChange stateGroupChange)

        #region Method: AddRelationship(RelationshipChange relationshipChange)
        /// <summary>
        /// Add an relationship change.
        /// </summary>
        /// <param name="relationshipChange">The relationship change to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddRelationship(RelationshipChange relationshipChange)
        {
            if (relationshipChange != null)
            {
                // If the relationship change is already available in all relationships, there is no use to add it
                if (this.Relationships.Contains(relationshipChange))
                    return Message.RelationExistsAlready;

                // Add the relationship change to the relationships
                Database.Current.Insert(this.ID, ValueTables.EntityChangeRelationshipChange, Columns.RelationshipChange, relationshipChange);
                NotifyPropertyChanged("Relationships");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddRelationship(RelationshipChange relationshipChange)

        #region Method: RemoveRelationship(RelationshipChange relationshipChange)
        /// <summary>
        /// Removes a relationship change.
        /// </summary>
        /// <param name="relationshipChange">The relationship change to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveRelationship(RelationshipChange relationshipChange)
        {
            if (relationshipChange != null)
            {
                if (this.Relationships.Contains(relationshipChange))
                {
                    // Remove the relationship change
                    Database.Current.Remove(this.ID, ValueTables.EntityChangeRelationshipChange, Columns.RelationshipChange, relationshipChange);
                    NotifyPropertyChanged("Relationships");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveRelationship(RelationshipChange relationshipChange)

        #region Method: AddAbstractEntity(AbstractEntityChange abstractEntityChange)
        /// <summary>
        /// Adds an abstract entity change to the list with abstract entities.
        /// </summary>
        /// <param name="abstractEntityChange">The abstract entity change to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddAbstractEntity(AbstractEntityChange abstractEntityChange)
        {
            if (abstractEntityChange != null)
            {
                // If the abstract entity change is already available in all abstract entities, there is no use to add it
                if (HasAbstractEntity(abstractEntityChange.AbstractEntity))
                    return Message.RelationExistsAlready;

                // Add the abstract entity change to the abstract entities
                Database.Current.Insert(this.ID, ValueTables.EntityChangeAbstractEntityChange, Columns.AbstractEntityChange, abstractEntityChange);
                NotifyPropertyChanged("AbstractEntities");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddAbstractEntity(AbstractEntityChange abstractEntityChange)

        #region Method: RemoveAbstractEntity(AbstractEntityChange abstractEntityChange)
        /// <summary>
        /// Removes an abstract entity change from the list of abstract entities.
        /// </summary>
        /// <param name="abstractEntityChange">The abstract entity change to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveAbstractEntity(AbstractEntityChange abstractEntityChange)
        {
            if (abstractEntityChange != null)
            {
                if (this.AbstractEntities.Contains(abstractEntityChange))
                {
                    // Remove the abstract entity change
                    Database.Current.Remove(this.ID, ValueTables.EntityChangeAbstractEntityChange, Columns.AbstractEntityChange, abstractEntityChange);
                    NotifyPropertyChanged("AbstractEntities");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveAbstractEntity(AbstractEntityChange abstractEntityChange)

        #region Method: AddAbstractEntityToAdd(AbstractEntityValued abstractEntityValued)
        /// <summary>
        /// Adds a valued abstract entity to the list with abstract entities to add.
        /// </summary>
        /// <param name="abstractEntityValued">The valued abstract entity to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddAbstractEntityToAdd(AbstractEntityValued abstractEntityValued)
        {
            if (abstractEntityValued != null && abstractEntityValued.AbstractEntity != null)
            {
                // If the valued abstract entity is already available in all abstract entities, there is no use to add it
                if (HasAbstractEntityToAdd(abstractEntityValued.AbstractEntity))
                    return Message.RelationExistsAlready;

                // Add the valued abstract entity to the abstract entities
                Database.Current.Insert(this.ID, ValueTables.EntityChangeAbstractEntityToAdd, Columns.AbstractEntityValued, abstractEntityValued);
                NotifyPropertyChanged("AbstractEntitiesToAdd");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddAbstractEntityToAdd(AbstractEntityValued abstractEntityValued)

        #region Method: RemoveAbstractEntityToAdd(AbstractEntityValued abstractEntityValued)
        /// <summary>
        /// Removes a valued abstract entity from the list of abstract entities to add.
        /// </summary>
        /// <param name="abstractEntityValued">The valued abstract entity to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveAbstractEntityToAdd(AbstractEntityValued abstractEntityValued)
        {
            if (abstractEntityValued != null)
            {
                if (this.AbstractEntitiesToAdd.Contains(abstractEntityValued))
                {
                    // Remove the valued abstractEntity
                    Database.Current.Remove(this.ID, ValueTables.EntityChangeAbstractEntityToAdd, Columns.AbstractEntityValued, abstractEntityValued);
                    NotifyPropertyChanged("AbstractEntitiesToAdd");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveAbstractEntityToAdd(AbstractEntityValued abstractEntityValued)

        #region Method: AddAbstractEntityToRemove(AbstractEntityCondition abstractEntityCondition)
        /// <summary>
        /// Adds an abstract entity condition to the list with abstract entities to remove.
        /// </summary>
        /// <param name="abstractEntityCondition">The abstract entity condition to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddAbstractEntityToRemove(AbstractEntityCondition abstractEntityCondition)
        {
            if (abstractEntityCondition != null)
            {
                // If the abstract entity condition is already available in all abstract entities, there is no use to add it
                if (HasAbstractEntityToRemove(abstractEntityCondition.AbstractEntity))
                    return Message.RelationExistsAlready;

                // Add the abstract entity condition to the abstract entities
                Database.Current.Insert(this.ID, ValueTables.EntityChangeAbstractEntityToRemove, Columns.AbstractEntityCondition, abstractEntityCondition);
                NotifyPropertyChanged("AbstractEntitiesToRemove");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddAbstractEntityToRemove(AbstractEntityCondition abstractEntityCondition)

        #region Method: RemoveAbstractEntityToRemove(AbstractEntityCondition abstractEntityCondition)
        /// <summary>
        /// Removes an abstract entity condition from the list of abstract entities to remove.
        /// </summary>
        /// <param name="abstractEntityCondition">The abstract entity condition to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveAbstractEntityToRemove(AbstractEntityCondition abstractEntityCondition)
        {
            if (abstractEntityCondition != null)
            {
                if (this.AbstractEntitiesToRemove.Contains(abstractEntityCondition))
                {
                    // Remove the abstract entity condition
                    Database.Current.Remove(this.ID, ValueTables.EntityChangeAbstractEntityToRemove, Columns.AbstractEntityCondition, abstractEntityCondition);
                    NotifyPropertyChanged("AbstractEntitiesToRemove");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveAbstractEntityToRemove(AbstractEntityCondition abstractEntityCondition)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasAttribute(Attribute attribute)
        /// <summary>
        /// Checks if this entity change has the given attribute.
        /// </summary>
        /// <param name="attribute">The attribute to check.</param>
        /// <returns>Returns true when the entity change has the attribute.</returns>
        public bool HasAttribute(Attribute attribute)
        {
            if (attribute != null)
            {
                foreach (AttributeChange attributeChange in this.Attributes)
                {
                    if (attribute.Equals(attributeChange.Attribute))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasAttribute(Attribute attribute)

        #region Method: HasAttributeToAdd(Attribute attribute)
        /// <summary>
        /// Checks if this entity change has the given attribute to add.
        /// </summary>
        /// <param name="attribute">The attribute to check.</param>
        /// <returns>Returns true when the entity change has the attribute to add.</returns>
        public bool HasAttributeToAdd(Attribute attribute)
        {
            if (attribute != null)
            {
                foreach (AttributeValued attributeValued in this.AttributesToAdd)
                {
                    if (attribute.Equals(attributeValued.Attribute))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasAttributeToAdd(Attribute attribute)

        #region Method: HasAttributeToRemove(Attribute attribute)
        /// <summary>
        /// Checks if this entity change has the given attribute to remove.
        /// </summary>
        /// <param name="attribute">The attribute to check.</param>
        /// <returns>Returns true when the entity change has the attribute to remove.</returns>
        public bool HasAttributeToRemove(Attribute attribute)
        {
            if (attribute != null)
            {
                foreach (AttributeCondition attributeCondition in this.AttributesToRemove)
                {
                    if (attribute.Equals(attributeCondition.Attribute))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasAttributeToRemove(Attribute attribute)

        #region Method: HasAbstractEntity(AbstractEntity abstractEntity)
        /// <summary>
        /// Checks if this entity change has the given abstract entity.
        /// </summary>
        /// <param name="abstractEntity">The abstract entity to check.</param>
        /// <returns>Returns true when the entity change has the abstract entity.</returns>
        public bool HasAbstractEntity(AbstractEntity abstractEntity)
        {
            if (abstractEntity != null)
            {
                foreach (AbstractEntityChange abstractEntityChange in this.AbstractEntities)
                {
                    if (abstractEntity.Equals(abstractEntityChange.AbstractEntity))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasAbstractEntity(AbstractEntity abstractEntity)

        #region Method: HasAbstractEntityToAdd(AbstractEntity abstractEntity)
        /// <summary>
        /// Checks if this entity change has the given abstract entity to add.
        /// </summary>
        /// <param name="abstractEntity">The abstract entity to check.</param>
        /// <returns>Returns true when the entity change has the abstract entity to add.</returns>
        public bool HasAbstractEntityToAdd(AbstractEntity abstractEntity)
        {
            if (abstractEntity != null)
            {
                foreach (AbstractEntityValued abstractEntityValued in this.AbstractEntitiesToAdd)
                {
                    if (abstractEntity.Equals(abstractEntityValued.AbstractEntity))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasAbstractEntityToAdd(AbstractEntity abstractEntity)

        #region Method: HasAbstractEntityToRemove(AbstractEntity abstractEntity)
        /// <summary>
        /// Checks if this entity change has the given abstract entity to remove.
        /// </summary>
        /// <param name="abstractEntity">The abstract entity to check.</param>
        /// <returns>Returns true when the entity change has the abstract entity to remove.</returns>
        public bool HasAbstractEntityToRemove(AbstractEntity abstractEntity)
        {
            if (abstractEntity != null)
            {
                foreach (AbstractEntityCondition abstractEntityCondition in this.AbstractEntitiesToRemove)
                {
                    if (abstractEntity.Equals(abstractEntityCondition.AbstractEntity))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasAbstractEntityToRemove(AbstractEntity abstractEntity)

        #region Method: Remove()
        /// <summary>
        /// Remove the entity change.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the quantity
            if (this.Quantity != null)
                this.Quantity.Remove();

            // Remove the attributes
            foreach (AttributeChange attributeChange in this.Attributes)
                attributeChange.Remove();
            Database.Current.Remove(this.ID, ValueTables.EntityChangeAttributeChange);

            foreach (AttributeValued attributeValued in this.AttributesToAdd)
                attributeValued.Remove();
            Database.Current.Remove(this.ID, ValueTables.EntityChangeAttributeToAdd);
            
            foreach (AttributeCondition attributeCondition in this.AttributesToRemove)
                attributeCondition.Remove();
            Database.Current.Remove(this.ID, ValueTables.EntityChangeAttributeToRemove);

            // Remove the state groups
            foreach (StateGroupChange stateGroupChange in this.StateGroups)
                stateGroupChange.Remove();
            Database.Current.Remove(this.ID, ValueTables.EntityChangeStateGroupChange);

            // Remove the relationships
            foreach (RelationshipChange relationshipChange in this.Relationships)
                relationshipChange.Remove();
            Database.Current.Remove(this.ID, ValueTables.EntityChangeRelationshipChange);

            // Remove the abstract entities
            foreach (AbstractEntityChange abstractEntityChange in this.AbstractEntities)
                abstractEntityChange.Remove();
            Database.Current.Remove(this.ID, ValueTables.EntityChangeAbstractEntityChange);

            foreach (AbstractEntityValued abstractEntityValued in this.AbstractEntitiesToAdd)
                abstractEntityValued.Remove();
            Database.Current.Remove(this.ID, ValueTables.EntityChangeAbstractEntityToAdd);

            foreach (AbstractEntityCondition abstractEntityCondition in this.AbstractEntitiesToRemove)
                abstractEntityCondition.Remove();
            Database.Current.Remove(this.ID, ValueTables.EntityChangeAbstractEntityToRemove);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: Clone()
        /// <summary>
        /// Clones the entity change.
        /// </summary>
        /// <returns>A clone of the entity change.</returns>
        public new EntityChange Clone()
        {
            return base.Clone() as EntityChange;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: EntityChange

    #region Class: EntityCreation
    /// <summary>
    /// A entity that should be created.
    /// </summary>
    public sealed class EntityCreation : Effect
    {

        #region Properties and Fields

        #region Property: EntityValued
        /// <summary>
        /// Gets or sets the entity that has to be created.
        /// </summary>
        public EntityValued EntityValued
        {
            get
            {
                return Database.Current.Select<EntityValued>(this.ID, ValueTables.EntityCreation, Columns.EntityValued);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.EntityCreation, Columns.EntityValued, value);
                NotifyPropertyChanged("EntityValued");
            }
        }
        #endregion Property: EntityValued

        #region Property: Destination
        /// <summary>
        /// Gets or sets the destination of the entity that is created.
        /// </summary>
        public Destination Destination
        {
            get
            {
                return Database.Current.Select<Destination>(this.ID, ValueTables.EntityCreation, Columns.Destination);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.EntityCreation, Columns.Destination, value);
                NotifyPropertyChanged("Destination");
            }
        }
        #endregion Property: Destination

        #region Property: Position
        /// <summary>
        /// Gets or sets the position the created entity should get; only valid when 'Destination' has been set to 'World'.
        /// </summary>
        public VectorValue Position
        {
            get
            {
                return Database.Current.Select<VectorValue>(this.ID, ValueTables.EntityCreation, Columns.Position);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.EntityCreation, Columns.Position, value);
                NotifyPropertyChanged("Position");
            }
        }
        #endregion Property: Position

        #region Property: Rotation
        /// <summary>
        /// Gets or sets the rotation (in degrees) the created entity should get.
        /// </summary>
        public VectorValue Rotation
        {
            get
            {
                return Database.Current.Select<VectorValue>(this.ID, ValueTables.EntityCreation, Columns.Rotation);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.EntityCreation, Columns.Rotation, value);
                NotifyPropertyChanged("Rotation");
            }
        }
        #endregion Property: Rotation

        #region Property: RelationshipType
        /// <summary>
        /// Gets or sets the relationship that has to be established with the created entity.
        /// </summary>
        public RelationshipType RelationshipType
        {
            get
            {
                return Database.Current.Select<RelationshipType>(this.ID, ValueTables.EntityCreation, Columns.RelationshipType);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.EntityCreation, Columns.RelationshipType, value);
                NotifyPropertyChanged("RelationshipType");
            }
        }
        #endregion Property: RelationshipType

        #region Property: RelationshipSourceTarget
        /// <summary>
        /// Gets or sets who should be the source and the target of the relationship that has to be created, in case 'RelationshipType' has been set.
        /// </summary>
        public CreationRelationshipSourceTarget RelationshipSourceTarget
        {
            get
            {
                return Database.Current.Select<CreationRelationshipSourceTarget>(this.ID, ValueTables.EntityCreation, Columns.RelationshipSourceTarget);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.EntityCreation, Columns.RelationshipSourceTarget, value);
                NotifyPropertyChanged("RelationshipSourceTarget");
            }
        }
        #endregion Property: RelationshipSourceTarget

        #region Property: MatterSource
        /// <summary>
        /// Gets or sets the source of the matter from which the matter of the created entity should be retrieved, which is 'null' when this should not be taken into account. Only valid when tangible objects should be created!
        /// </summary>
        public ActorTargetArtifactReference? MatterSource
        {
            get
            {
                return Database.Current.Select<ActorTargetArtifactReference?>(this.ID, ValueTables.EntityCreation, Columns.MatterSource);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.EntityCreation, Columns.MatterSource, value);
                NotifyPropertyChanged("MatterSource");
            }
        }
        #endregion Property: MatterSource

        #region Property: MatterSourceReference
        /// <summary>
        /// Gets or sets the reference of the matter source, in case MatterSource has been set to 'Reference'. Only valid when tangible objects should be created!
        /// </summary>
        public Reference MatterSourceReference
        {
            get
            {
                return Database.Current.Select<Reference>(this.ID, ValueTables.EntityCreation, Columns.MatterSourceReference);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.EntityCreation, Columns.MatterSourceReference, value);
                NotifyPropertyChanged("MatterSourceReference");
            }
        }
        #endregion Property: MatterSourceReference

        #region Property: PartSource
        /// <summary>
        /// Gets or sets the source of the parts from which the parts of the created entity should be retrieved, which is 'null' when this should not be taken into account. Only valid when tangible objects should be created!
        /// </summary>
        public ActorTargetArtifactReference? PartSource
        {
            get
            {
                return Database.Current.Select<ActorTargetArtifactReference?>(this.ID, ValueTables.EntityCreation, Columns.PartSource);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.EntityCreation, Columns.PartSource, value);
                NotifyPropertyChanged("PartSource");
            }
        }
        #endregion Property: PartSource

        #region Property: PartSourceReference
        /// <summary>
        /// Gets or sets the reference of the part source, in case PartSource has been set to 'Reference'. Only valid when tangible objects should be created!
        /// </summary>
        public Reference PartSourceReference
        {
            get
            {
                return Database.Current.Select<Reference>(this.ID, ValueTables.EntityCreation, Columns.PartSourceReference);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.EntityCreation, Columns.PartSourceReference, value);
                NotifyPropertyChanged("PartSourceReference");
            }
        }
        #endregion Property: PartSourceReference

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: EntityCreation()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static EntityCreation()
        {
            // Valued entity, relationship type, and reference
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.EntityValued, new Tuple<Type, EntryType>(typeof(EntityValued), EntryType.Unique));
            dict.Add(Columns.RelationshipType, new Tuple<Type, EntryType>(typeof(RelationshipType), EntryType.Nullable));
            dict.Add(Columns.MatterSourceReference, new Tuple<Type, EntryType>(typeof(Reference), EntryType.Nullable));
            dict.Add(Columns.PartSourceReference, new Tuple<Type, EntryType>(typeof(Reference), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.EntityCreation, typeof(EntityCreation), dict);
        }
        #endregion Static Constructor: EntityCreation()

        #region Constructor: EntityCreation()
        /// <summary>
        /// Creates a new entity creation.
        /// </summary>
        public EntityCreation()
            : base()
        {
            Database.Current.StartChange();

            this.Position = new VectorValue(new Vec4(SemanticsSettings.Values.PositionX, SemanticsSettings.Values.PositionY, SemanticsSettings.Values.PositionZ));
            this.Rotation = new VectorValue(new Vec4(SemanticsSettings.Values.RotationX, SemanticsSettings.Values.RotationY, SemanticsSettings.Values.RotationZ));

            Database.Current.StopChange();
        }
        #endregion Constructor: EntityCreation()

        #region Constructor: EntityCreation(uint id)
        /// <summary>
        /// Creates a new entity creation from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a entity creation from.</param>
        private EntityCreation(uint id)
            : base(id)
        {
        }
        #endregion Constructor: EntityCreation(uint id)

        #region Constructor: EntityCreation(EntityCreation entityCreation)
        /// <summary>
        /// Clones a entity creation.
        /// </summary>
        /// <param name="entityCreation">The entity creation to clone.</param>
        public EntityCreation(EntityCreation entityCreation)
            : base()
        {
            if (entityCreation != null)
            {
                Database.Current.StartChange();

                this.EntityValued = entityCreation.EntityValued.Clone();
                this.Destination = entityCreation.Destination;
                if (entityCreation.Position != null)
                    this.Position = new VectorValue(entityCreation.Position);
                if (entityCreation.Rotation != null)
                    this.Rotation = new VectorValue(entityCreation.Rotation);
                this.RelationshipType = entityCreation.RelationshipType;
                this.RelationshipSourceTarget = entityCreation.RelationshipSourceTarget;
                this.MatterSource = entityCreation.MatterSource;
                this.MatterSourceReference = entityCreation.MatterSourceReference;
                this.PartSource = entityCreation.PartSource;
                this.PartSourceReference = entityCreation.PartSourceReference;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: EntityCreation(EntityCreation entityCreation)

        #region Constructor: EntityCreation(EntityValued entityValued)
        /// <summary>
        /// Creates a creation for the given valued entity.
        /// </summary>
        /// <param name="entityValued">The valued entity to create a creation for.</param>
        public EntityCreation(EntityValued entityValued)
            : this()
        {
            Database.Current.StartChange();

            this.EntityValued = entityValued;

            Database.Current.StopChange();
        }
        #endregion Constructor: EntityCreation(EntityValued entityValued)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the entity creation.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the valued entity
            if (this.EntityValued != null)
                this.EntityValued.Remove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: EntityCreation

}