/**************************************************************************
 * 
 * Relationship.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Common;
using Semantics.Abstractions;
using Semantics.Data;
using Semantics.Entities;
using Semantics.Interfaces;
using Semantics.Utilities;
using Attribute = Semantics.Abstractions.Attribute;

namespace Semantics.Components
{

    #region Class: Relationship
    /// <summary>
    /// A relationship keeps track of (the space of) a source and target of a relationship type, possibly accompanied by parameters.
    /// Furthermore, it has a necessity and a priority.
    /// </summary>
    public sealed class Relationship : IdHolder, IVariableReferenceHolder
    {

        #region Properties and Fields

        #region Property: Source
        /// <summary>
        /// Gets or sets the source of the relationship.
        /// </summary>
        public Entity Source
        {
            get
            {
                return Database.Current.Select<Entity>(this.ID, GenericTables.Relationship, Columns.Source);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Relationship, Columns.Source, value);
                NotifyPropertyChanged("Source");
                
                // Reset the source space
                this.SourceSpace = null;
            }
        }
        #endregion Property: Source

        #region Property: SourceSpace
        /// <summary>
        /// Gets or sets the space of the source that is used in the relationship; only valid for physical objects.
        /// </summary>
        public SpaceValued SourceSpace
        {
            get
            {
                return Database.Current.Select<SpaceValued>(this.ID, GenericTables.Relationship, Columns.SourceSpace);
            }
            set
            {
                if (value == null || (value != null && this.Source is PhysicalObject && ((PhysicalObject)this.Source).HasSpace(((SpaceValued)value).Space)))
                {
                    Database.Current.Update(this.ID, GenericTables.Relationship, Columns.SourceSpace, value);
                    NotifyPropertyChanged("SourceSpace");
                }
            }
        }
        #endregion Property: SourceSpace

        #region Property: RelationshipType
        /// <summary>
        /// Gets the relationship type of the relationship.
        /// </summary>
        public RelationshipType RelationshipType
        {
            get
            {
                return Database.Current.Select<RelationshipType>(this.ID, GenericTables.Relationship, Columns.RelationshipType);
            }
            private set
            {
                Database.Current.Update(this.ID, GenericTables.Relationship, Columns.RelationshipType, value);
                NotifyPropertyChanged("RelationshipType");
            }
        }
        #endregion Property: RelationshipType

        #region Property: Targets
        /// <summary>
        /// Gets the targets of the relationship.
        /// </summary>
        public ReadOnlyCollection<Entity> Targets
        {
            get
            {
                return Database.Current.SelectAll<Entity>(this.ID, GenericTables.RelationshipTarget, Columns.Target).AsReadOnly();
            }
        }
        #endregion Property: Targets

        #region Property: TargetSpaces
        /// <summary>
        /// Gets the dictionary that stores the space of the target that is used in the relationship; only valid for physical objects.
        /// </summary>
        public ReadOnlyDictionary<Entity, SpaceValued> TargetSpaces
        {
            get
            {
                Dictionary<Entity, SpaceValued> targetSpaces = new Dictionary<Entity, SpaceValued>();
                foreach (Entity target in this.Targets)
                {
                    SpaceValued targetSpace = Database.Current.Select<SpaceValued>(this.ID, GenericTables.RelationshipTarget, Columns.SpaceValued, Columns.Target, target.ID);
                    if (targetSpace != null)
                        targetSpaces.Add(target, targetSpace);
                }
                return new ReadOnlyDictionary<Entity,SpaceValued>(targetSpaces);
            }
        }
        #endregion Property: TargetSpaces

        #region Property: SourceRequirements
        /// <summary>
        /// Gets the requirements for the source.
        /// </summary>
        public Requirements SourceRequirements
        {
            get
            {
                return Database.Current.Select<Requirements>(this.ID, GenericTables.Relationship, Columns.SourceRequirements);
            }
            private set
            {
                Database.Current.Update(this.ID, GenericTables.Relationship, Columns.SourceRequirements, value);
                NotifyPropertyChanged("SourceRequirements");
            }
        }
        #endregion Property: SourceRequirements

        #region Property: TargetsRequirements
        /// <summary>
        /// Gets the requirements for the targets.
        /// </summary>
        public Requirements TargetsRequirements
        {
            get
            {
                return Database.Current.Select<Requirements>(this.ID, GenericTables.Relationship, Columns.TargetsRequirements);
            }
            private set
            {
                Database.Current.Update(this.ID, GenericTables.Relationship, Columns.TargetsRequirements, value);
                NotifyPropertyChanged("TargetsRequirements");
            }
        }
        #endregion Property: TargetsRequirements

        #region Property: SpatialRequirementBetweenSourceAndTargets
        /// <summary>
        /// Gets or sets the spatial condition between the source and targets.
        /// </summary>
        public SpatialRequirement SpatialRequirementBetweenSourceAndTargets
        {
            get
            {
                return Database.Current.Select<SpatialRequirement>(this.ID, GenericTables.Relationship, Columns.SpatialRequirementBetweenSourceAndTargets);
            }
            set
            {
                SpatialRequirement spatialRequirement = this.SpatialRequirementBetweenSourceAndTargets;
                if (spatialRequirement != value)
                {
                    if (spatialRequirement != null)
                        spatialRequirement.Remove();

                    Database.Current.Update(this.ID, GenericTables.Relationship, Columns.SpatialRequirementBetweenSourceAndTargets, value);
                    NotifyPropertyChanged("SpatialRequirementBetweenSourceAndTargets");
                }
            }
        }
        #endregion Property: SpatialRequirementBetweenSourceAndTargets

        #region Property: Parameters
        /// <summary>
        /// Gets the parameters of the relationship.
        /// </summary>
        public ReadOnlyCollection<AttributeCondition> Parameters
        {
            get
            {
                return Database.Current.SelectAll<AttributeCondition>(this.ID, GenericTables.RelationshipParameter, Columns.ParameterValued).AsReadOnly();
            }
        }
        #endregion Property: Parameters

        #region Property: Attributes
        /// <summary>
        /// Gets the attributes of the relationship.
        /// </summary>
        public ReadOnlyCollection<AttributeValued> Attributes
        {
            get
            {
                return Database.Current.SelectAll<AttributeValued>(this.ID, GenericTables.RelationshipAttribute, Columns.AttributeValued).AsReadOnly();
            }
        }
        #endregion Property: Attributes

        #region Property: Necessity
        /// <summary>
        /// Gets or sets the necessity of the relationship.
        /// </summary>
        public Necessity Necessity
        {
            get
            {
                return Database.Current.Select<Necessity>(this.ID, GenericTables.Relationship, Columns.Necessity);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Relationship, Columns.Necessity, value);
                NotifyPropertyChanged("Necessity");
            }
        }
        #endregion Property: Necessity

        #region Property: Priority
        /// <summary>
        /// Gets or sets the priority.
        /// </summary>
        public int Priority
        {
            get
            {
                return Database.Current.Select<int>(this.ID, GenericTables.Relationship, Columns.Priority);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Relationship, Columns.Priority, value);
                NotifyPropertyChanged("Priority");
            }
        }
        #endregion Property: Priority

        #region Property: Cardinality
        /// <summary>
        /// Gets or sets the cardinality of the relationship.
        /// </summary>
        public Cardinality Cardinality
        {
            get
            {
                return Database.Current.Select<Cardinality>(this.ID, GenericTables.Relationship, Columns.Cardinality);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Relationship, Columns.Cardinality, value);
                NotifyPropertyChanged("Cardinality");
            }
        }
        #endregion Property: Cardinality

        #region Property: IsExclusive
        /// <summary>
        /// Gets or sets the value that indicates whether the relationship is exclusive, and cannot be changed after establishment.
        /// </summary>
        public bool IsExclusive
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, GenericTables.Relationship, Columns.IsExclusive);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Relationship, Columns.IsExclusive, value);
                NotifyPropertyChanged("IsExclusive");
            }
        }
        #endregion Property: IsExclusive

        #region Property: Variables
        /// <summary>
        /// Gets the variables.
        /// </summary>
        public ReadOnlyCollection<Variable> Variables
        {
            get
            {
                return Database.Current.SelectAll<Variable>(this.ID, GenericTables.RelationshipVariable, Columns.Variable).AsReadOnly();
            }
        }
        #endregion Property: Variables

        #region Property: References
        /// <summary>
        /// Gets the references.
        /// </summary>
        public ReadOnlyCollection<Reference> References
        {
            get
            {
                return Database.Current.SelectAll<Reference>(this.ID, GenericTables.RelationshipReferences, Columns.Reference).AsReadOnly();
            }
        }
        #endregion Property: References

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: Relationship()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static Relationship()
        {
            // Relationship type, source, and space
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.RelationshipType, new Tuple<Type, EntryType>(typeof(RelationshipType), EntryType.Nullable));
            dict.Add(Columns.Source, new Tuple<Type, EntryType>(typeof(Entity), EntryType.Nullable));
            dict.Add(Columns.SourceSpace, new Tuple<Type, EntryType>(typeof(SpaceValued), EntryType.Nullable));
            Database.Current.AddTableDefinition(GenericTables.Relationship, typeof(Relationship), dict);

            // Targets and spaces
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Target, new Tuple<Type, EntryType>(typeof(Entity), EntryType.Unique));
            dict.Add(Columns.SpaceValued, new Tuple<Type, EntryType>(typeof(SpaceValued), EntryType.Nullable));
            Database.Current.AddTableDefinition(GenericTables.RelationshipTarget, typeof(Relationship), dict);

            // Parameters
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.ParameterValued, new Tuple<Type, EntryType>(typeof(AttributeCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.RelationshipParameter, typeof(Relationship), dict);

            // Attributes
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.AttributeValued, new Tuple<Type, EntryType>(typeof(AttributeValued), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.RelationshipAttribute, typeof(Relationship), dict);

            // Variables
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Variable, new Tuple<Type, EntryType>(typeof(Variable), EntryType.Intermediate));
            Database.Current.AddTableDefinition(GenericTables.RelationshipVariable, typeof(Relationship), dict);

            // References
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Reference, new Tuple<Type, EntryType>(typeof(Reference), EntryType.Intermediate));
            Database.Current.AddTableDefinition(GenericTables.RelationshipReferences, typeof(Relationship), dict);
        }
        #endregion Static Constructor: Relationship()

        #region Constructor: Relationship(uint id)
        /// <summary>
        /// Creates a new relationship with the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new relationship from.</param>
        private Relationship(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Relationship(uint id)

        #region Constructor: Relationship(Relationship relationship)
        /// <summary>
        /// Clones the relationship.
        /// </summary>
        /// <param name="relationship">The relationship to clone.</param>
        public Relationship(Relationship relationship)
            : base()
        {
            if (relationship != null)
            {
                Database.Current.StartChange();

                this.Source = relationship.Source;
                if (relationship.SourceSpace != null)
                    this.SourceSpace = new SpaceValued(relationship.SourceSpace);
                this.RelationshipType = relationship.RelationshipType;
                foreach (Entity target in relationship.Targets)
                    AddTarget(target);
                foreach (Entity target in relationship.TargetSpaces.Keys)
                    SetTargetSpace(target, relationship.TargetSpaces[target]);
                if (relationship.SourceRequirements != null)
                    this.SourceRequirements = new Requirements(relationship.SourceRequirements);
                if (relationship.TargetsRequirements != null)
                    this.TargetsRequirements = new Requirements(relationship.TargetsRequirements);
                if (relationship.SpatialRequirementBetweenSourceAndTargets != null)
                    this.SpatialRequirementBetweenSourceAndTargets = new SpatialRequirement(relationship.SpatialRequirementBetweenSourceAndTargets);
                foreach (AttributeCondition parameter in relationship.Parameters)
                    AddParameter(new AttributeCondition(parameter));
                foreach (AttributeValued attribute in relationship.Attributes)
                    AddAttribute(new AttributeValued(attribute));
                this.Necessity = relationship.Necessity;
                this.Priority = relationship.Priority;
                this.Cardinality = relationship.Cardinality;
                this.IsExclusive = relationship.IsExclusive;
                foreach (Reference reference in relationship.References)
                    AddReference(reference.Clone());
                foreach (Variable variable in relationship.Variables)
                    AddVariable(variable.Clone());

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Relationship(Relationship relationship)

        #region Constructor: Relationship(RelationshipType relationshipType)
        /// <summary>
        /// Creates a relationship from the given relationship type.
        /// </summary>
        /// <param name="relationshipType">The relationship type to create a relationship from.</param>
        public Relationship(RelationshipType relationshipType)
            : base()
        {
            if (relationshipType != null)
            {
                Database.Current.StartChange();

                this.SourceRequirements = new Requirements();
                this.TargetsRequirements = new Requirements();
                this.RelationshipType = relationshipType;
                relationshipType.AddRelationship(this);

                Database.Current.StopChange();
            }
            else
                Remove();
        }
        #endregion Constructor: Relationship(RelationshipType relationshipType)

        #region Constructor: Relationship(RelationshipType relationshipType, Entity source)
        /// <summary>
        /// Creates a relationship from the given relationship type for the given source.
        /// </summary>
        /// <param name="relationshipType">The relationship type to create a relationship from.</param>
        /// <param name="source">The source.</param>
        public Relationship(RelationshipType relationshipType, Entity source)
            : this(relationshipType)
        {
            if (relationshipType != null)
            {
                Database.Current.StartChange();

                this.Source = source;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Relationship(RelationshipType relationshipType, Entity source)

        #region Constructor: Relationship(RelationshipType relationshipType, Entity source, Entity target)
        /// <summary>
        /// Creates a relationship from the given relationship type between the given source and target.
        /// </summary>
        /// <param name="relationshipType">The relationship type to create a relationship from.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        public Relationship(RelationshipType relationshipType, Entity source, Entity target)
            : this(relationshipType)
        {
            if (relationshipType != null)
            {
                Database.Current.StartChange();

                this.Source = source;
                AddTarget(target);

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Relationship(RelationshipType relationshipType, Entity source, Entity target)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddTarget(Entity entity)
        /// <summary>
        /// Add a target to the relationship.
        /// </summary>
        /// <param name="entity">The target to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddTarget(Entity entity)
        {
            if (entity != null)
            {
                // If the target is already available in all targets, there is no use to add it
                if (HasTarget(entity))
                    return Message.RelationExistsAlready;

                // Add the target to the targets
                Database.Current.Insert(this.ID, GenericTables.RelationshipTarget, Columns.Target, entity);
                NotifyPropertyChanged("Targets");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddTarget(Entity entity)

        #region Method: RemoveTarget(Entity entity)
        /// <summary>
        /// Remove a target from the relationship.
        /// </summary>
        /// <param name="entity">The target to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveTarget(Entity entity)
        {
            if (entity != null)
            {
                if (HasTarget(entity))
                {
                    // Remove the target
                    Database.Current.Remove(this.ID, GenericTables.RelationshipTarget, Columns.Target, entity);
                    NotifyPropertyChanged("Targets");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveTarget(Entity entity)

        #region Method: SetTargetSpace(Entity target, SpaceValued targetSpace)
        /// <summary>
        /// Set the space of a target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="targetSpace">The space of the target, can be null.</param>
        /// <returns>Indicates whether the relation has been succesfully set.</returns>
        public Message SetTargetSpace(Entity target, SpaceValued targetSpace)
        {
            if (target != null)
            {
                PhysicalObject targetObject = target as PhysicalObject;
                if (targetObject != null && (targetSpace == null || targetObject.HasSpace(targetSpace.Space)))
                {
                    // Set the space of the target
                    Database.Current.Update(this.ID, GenericTables.RelationshipTarget, Columns.SpaceValued, targetSpace, Columns.Target, targetObject);

                    NotifyPropertyChanged("TargetSpaces");
                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: SetTargetSpace(Entity target, SpaceValued targetSpace)

        #region Method: AddParameter(AttributeCondition parameter)
        /// <summary>
        /// Add a parameter to the relationship.
        /// </summary>
        /// <param name="parameter">The parameter to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        internal Message AddParameter(AttributeCondition parameter)
        {
            if (parameter != null)
            {
                // If the parameter is already available in all parameters, there is no use to add it
                if (HasParameter(parameter.Attribute))
                    return Message.RelationExistsAlready;

                // Add the parameter to the parameters
                Database.Current.Insert(this.ID, GenericTables.RelationshipParameter, Columns.ParameterValued, parameter);
                NotifyPropertyChanged("Parameters");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddParameter(AttributeCondition parameter)

        #region Method: RemoveParameter(AttributeCondition parameter)
        /// <summary>
        /// Remove a parameter from the relationship.
        /// </summary>
        /// <param name="parameter">The parameter to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        internal Message RemoveParameter(AttributeCondition parameter)
        {
            if (parameter != null)
            {
                if (HasParameter(parameter.Attribute))
                {
                    // Remove the parameter
                    Database.Current.Remove(this.ID, GenericTables.RelationshipParameter, Columns.ParameterValued, parameter);
                    NotifyPropertyChanged("Parameters");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveParameter(AttributeCondition parameter)

        #region Method: AddAttribute(AttributeValued attributeValued)
        /// <summary>
        /// Add a attribute to the relationship.
        /// </summary>
        /// <param name="attributeValued">The attribute to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        internal Message AddAttribute(AttributeValued attributeValued)
        {
            if (attributeValued != null)
            {
                // If the attribute is already available in all attributes, there is no use to add it
                if (HasAttribute(attributeValued.Attribute))
                    return Message.RelationExistsAlready;

                // Add the attribute to the attributes
                Database.Current.Insert(this.ID, GenericTables.RelationshipAttribute, Columns.AttributeValued, attributeValued);
                NotifyPropertyChanged("Attributes");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddAttribute(AttributeValued attributeValued)

        #region Method: RemoveAttribute(AttributeValued attributeValued)
        /// <summary>
        /// Remove a attribute from the relationship.
        /// </summary>
        /// <param name="attributeValued">The attribute to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        internal Message RemoveAttribute(AttributeValued attributeValued)
        {
            if (attributeValued != null)
            {
                if (HasAttribute(attributeValued.Attribute))
                {
                    // Remove the attribute
                    Database.Current.Remove(this.ID, GenericTables.RelationshipAttribute, Columns.AttributeValued, attributeValued);
                    NotifyPropertyChanged("Attributes");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveAttribute(AttributeValued attributeValued)

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
                Database.Current.Insert(this.ID, GenericTables.RelationshipVariable, Columns.Variable, variable);
                NotifyPropertyChanged("Variables");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddVariable(Variable variable)

        #region Method: RemoveVariable(Variable variable)
        /// <summary>
        /// Removes an variable.
        /// </summary>
        /// <param name="variable">The variable to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveVariable(Variable variable)
        {
            if (variable != null)
            {
                if (HasVariable(variable))
                {
                    // Remove the variable
                    Database.Current.Remove(this.ID, GenericTables.RelationshipVariable, Columns.Variable, variable);
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
                Database.Current.Insert(this.ID, GenericTables.RelationshipReferences, Columns.Reference, reference);
                NotifyPropertyChanged("References");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddReference(Reference reference)

        #region Method: RemoveReference(Reference reference)
        /// <summary>
        /// Removes an reference.
        /// </summary>
        /// <param name="reference">The reference to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveReference(Reference reference)
        {
            if (reference != null)
            {
                if (HasReference(reference))
                {
                    // Remove the reference
                    Database.Current.Remove(this.ID, GenericTables.RelationshipReferences, Columns.Reference, reference);
                    NotifyPropertyChanged("References");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveReference(Reference reference)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasTarget(Entity entity)
        /// <summary>
        /// Checks whether the relationship has the given entity as a target.
        /// </summary>
        /// <param name="entity">The entity to check.</param>
        /// <returns>Returns true when the relationship has the entity as a target.</returns>
        public bool HasTarget(Entity entity)
        {
            if (entity != null)
                return this.Targets.Contains(entity);
            return false;
        }
        #endregion Method: HasTarget(Entity entity)

        #region Method: HasVariable(Variable variable)
        /// <summary>
        /// Checks if this relationship has the given variable.
        /// </summary>
        /// <param name="variable">The variable to check.</param>
        /// <returns>Returns true when this relationship has the variable.</returns>
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
        /// Checks if this relationship has the given reference.
        /// </summary>
        /// <param name="reference">The reference to check.</param>
        /// <returns>Returns true when this relationship has the reference.</returns>
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

        #region Method: HasParameter(Attribute parameter)
        /// <summary>
        /// Checks whether the relationship has the given parameter.
        /// </summary>
        /// <param name="parameter">The parameter to check.</param>
        /// <returns>Returns true when the relationship has the parameter.</returns>
        public bool HasParameter(Attribute parameter)
        {
            if (parameter != null)
            {
                foreach (AttributeCondition relationshipParameter in this.Parameters)
                {
                    if (parameter.Equals(relationshipParameter.Attribute))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasParameter(Attribute parameter)

        #region Method: HasAttribute(Attribute attribute)
        /// <summary>
        /// Checks whether the relationship has the given attribute.
        /// </summary>
        /// <param name="attribute">The attribute to check.</param>
        /// <returns>Returns true when the relationship has the attribute.</returns>
        public bool HasAttribute(Attribute attribute)
        {
            if (attribute != null)
            {
                foreach (AttributeValued relationshipAttribute in this.Attributes)
                {
                    if (attribute.Equals(relationshipAttribute.Attribute))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasAttribute(Attribute attribute)

        #region Method: GetParameterValue(String name)
        /// <summary>
        /// Gets the value of the parameter with the given name.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>The value of the parameter.</returns>
        public ValueCondition GetParameterValue(String name)
        {
            if (name != null)
            {
                foreach (AttributeCondition relationshipParameter in this.Parameters)
                {
                    if (relationshipParameter.Attribute != null)
                    {
                        if (relationshipParameter.Attribute.HasName(name))
                            return relationshipParameter.Value;
                    }
                }
            }
            return null;
        }
        #endregion Method: GetParameterValue(String name)

        #region Method: GetParameterValue(Attribute parameter)
        /// <summary>
        /// Gets the value of the given parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The value of the parameter.</returns>
        public ValueCondition GetParameterValue(Attribute parameter)
        {
            if (parameter != null)
            {
                foreach (AttributeCondition relationshipParameter in this.Parameters)
                {
                    if (parameter.Equals(relationshipParameter.Attribute))
                        return relationshipParameter.Value;
                }
            }
            return null;
        }
        #endregion Method: GetParameterValue(Attribute parameter)

        #region Method: Remove()
        /// <summary>
        /// Remove the relationship.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the targets
            Database.Current.Remove(this.ID, GenericTables.RelationshipTarget);

            // Remove the requirements
            if (this.SourceRequirements != null)
                this.SourceRequirements.Remove();
            if (this.TargetsRequirements != null)
                this.TargetsRequirements.Remove();

            // Remove the spatial requirement
            if (this.SpatialRequirementBetweenSourceAndTargets != null)
                this.SpatialRequirementBetweenSourceAndTargets.Remove();

            // Remove the parameters
            foreach (AttributeCondition parameter in this.Parameters)
                parameter.Remove();
            Database.Current.Remove(this.ID, GenericTables.RelationshipParameter);

            // Remove the attributes
            foreach (AttributeValued attributeValued in this.Attributes)
                attributeValued.Remove();
            Database.Current.Remove(this.ID, GenericTables.RelationshipAttribute);

            // Remove the variables
            foreach (Variable variable in this.Variables)
                variable.Remove();
            Database.Current.Remove(this.ID, GenericTables.RelationshipVariable);

            // Remove the references
            foreach (Reference reference in this.References)
                reference.Remove();
            Database.Current.Remove(this.ID, GenericTables.RelationshipReferences);

            RelationshipType relationshipType = this.RelationshipType;

            base.Remove();

            // Remove the relationship from the relationship type
            if (relationshipType != null)
                relationshipType.RemoveRelationship(this);

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

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
    #endregion Class: Relationship

    #region Class: RelationshipCondition
    /// <summary>
    /// A condition on a relationship.
    /// </summary>
    public sealed class RelationshipCondition : Condition
    {

        #region Properties and Fields

        #region Property: Source
        /// <summary>
        /// Gets or sets the required source.
        /// </summary>
        public Entity Source
        {
            get
            {
                return Database.Current.Select<Entity>(this.ID, ValueTables.RelationshipCondition, Columns.Source);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.RelationshipCondition, Columns.Source, value);
                NotifyPropertyChanged("Source");
            }
        }
        #endregion Property: Source

        #region Property: SourceSpace
        /// <summary>
        /// Gets or sets the required source space.
        /// </summary>
        public Space SourceSpace
        {
            get
            {
                return Database.Current.Select<Space>(this.ID, ValueTables.RelationshipCondition, Columns.SourceSpace);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.RelationshipCondition, Columns.SourceSpace, value);
                NotifyPropertyChanged("SourceSpace");
            }
        }
        #endregion Property: SourceSpace

        #region Property: RelationshipType
        /// <summary>
        /// Gets or sets the required relationship type.
        /// </summary>
        public RelationshipType RelationshipType
        {
            get
            {
                return Database.Current.Select<RelationshipType>(this.ID, ValueTables.RelationshipCondition, Columns.RelationshipType);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.RelationshipCondition, Columns.RelationshipType, value);
                NotifyPropertyChanged("RelationshipType");
            }
        }
        #endregion Property: RelationshipType

        #region Property: Target
        /// <summary>
        /// Gets or sets the required target.
        /// </summary>
        public Entity Target
        {
            get
            {
                return Database.Current.Select<Entity>(this.ID, ValueTables.RelationshipCondition, Columns.Target);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.RelationshipCondition, Columns.Target, value);
                NotifyPropertyChanged("Target");
            }
        }
        #endregion Property: Target

        #region Property: TargetSpace
        /// <summary>
        /// Gets or sets the required target space.
        /// </summary>
        public Space TargetSpace
        {
            get
            {
                return Database.Current.Select<Space>(this.ID, ValueTables.RelationshipCondition, Columns.TargetSpace);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.RelationshipCondition, Columns.TargetSpace, value);
                NotifyPropertyChanged("TargetSpace");
            }
        }
        #endregion Property: TargetSpace

        #region Property: Attributes
        /// <summary>
        /// Gets the required attributes.
        /// </summary>
        public ReadOnlyCollection<AttributeCondition> Attributes
        {
            get
            {
                return Database.Current.SelectAll<AttributeCondition>(this.ID, ValueTables.RelationshipConditionAttributeCondition, Columns.AttributeCondition).AsReadOnly();
            }
        }
        #endregion Property: Attributes

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: RelationshipCondition()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static RelationshipCondition()
        {
            // Source, source space, relationship type, target, and target space
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Source, new Tuple<Type, EntryType>(typeof(Entity), EntryType.Nullable));
            dict.Add(Columns.SourceSpace, new Tuple<Type, EntryType>(typeof(Space), EntryType.Nullable));
            dict.Add(Columns.RelationshipType, new Tuple<Type, EntryType>(typeof(RelationshipType), EntryType.Nullable));
            dict.Add(Columns.Target, new Tuple<Type, EntryType>(typeof(Entity), EntryType.Nullable));
            dict.Add(Columns.TargetSpace, new Tuple<Type, EntryType>(typeof(Space), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.RelationshipCondition, typeof(RelationshipCondition), dict);

            // Attributes
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.AttributeCondition, new Tuple<Type, EntryType>(typeof(AttributeCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.RelationshipConditionAttributeCondition, typeof(RelationshipCondition), dict);
        }
        #endregion Static Constructor: RelationshipCondition()

        #region Constructor: RelationshipCondition()
        /// <summary>
        /// Creates a new relationship condition.
        /// </summary>
        public RelationshipCondition()
            : base()
        {
        }
        #endregion Constructor: RelationshipCondition()

        #region Constructor: RelationshipCondition(uint id)
        /// <summary>
        /// Creates a new relationship condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a relationship condition from.</param>
        private RelationshipCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: RelationshipCondition(uint id)

        #region Constructor: RelationshipCondition(RelationshipType relationshipType)
        /// <summary>
        /// Creates a condition for the given relationship type.
        /// </summary>
        /// <param name="relationshipType">The relationship type to create a condition for.</param>
        public RelationshipCondition(RelationshipType relationshipType)
        {
            this.RelationshipType = relationshipType;
        }
        #endregion Constructor: RelationshipCondition(RelationshipType relationshipType)

        #region Constructor: RelationshipCondition(RelationshipCondition relationshipCondition)
        /// <summary>
        /// Clones a relationship condition.
        /// </summary>
        /// <param name="relationshipCondition">The relationship condition to clone.</param>
        public RelationshipCondition(RelationshipCondition relationshipCondition)
            : base(relationshipCondition)
        {
            if (relationshipCondition != null)
            {
                Database.Current.StartChange();

                this.Source = relationshipCondition.Source;
                this.SourceSpace = relationshipCondition.SourceSpace;
                this.RelationshipType = relationshipCondition.RelationshipType;
                this.Target = relationshipCondition.Target;
                this.TargetSpace = relationshipCondition.TargetSpace;
                foreach (AttributeCondition attributeCondition in relationshipCondition.Attributes)
                    AddAttribute(new AttributeCondition(attributeCondition));

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: RelationshipCondition(RelationshipCondition relationshipCondition)

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
            if (attributeCondition != null && attributeCondition.Attribute != null)
            {
                // If the attribute condition is already available in all attributes, there is no use to add it
                if (HasAttribute(attributeCondition.Attribute))
                    return Message.RelationExistsAlready;

                // Add the attribute condition to the attributes
                Database.Current.Insert(this.ID, ValueTables.RelationshipConditionAttributeCondition, Columns.AttributeCondition, attributeCondition);
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
                if (HasAttribute(attributeCondition.Attribute))
                {
                    // Remove the attribute condition
                    Database.Current.Remove(this.ID, ValueTables.RelationshipConditionAttributeCondition, Columns.AttributeCondition, attributeCondition);
                    NotifyPropertyChanged("Attributes");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveAttribute(AttributeCondition attributeCondition)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasAttribute(Attribute attribute)
        /// <summary>
        /// Checks if this relationship condition has the given attribute.
        /// </summary>
        /// <param name="attribute">The attribute to check.</param>
        /// <returns>Returns true when the relationship condition has the attribute.</returns>
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

        #region Method: Remove()
        /// <summary>
        /// Remove the relationship condition.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the attributes
            foreach (AttributeCondition attributeCondition in this.Attributes)
                attributeCondition.Remove();
            Database.Current.Remove(this.ID, ValueTables.RelationshipConditionAttributeCondition);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: RelationshipCondition

    #region Class: RelationshipChange
    /// <summary>
    /// A change on a relationship.
    /// </summary>
    public sealed class RelationshipChange : Change
    {

        #region Properties and Fields

        #region Property: RelationshipType
        /// <summary>
        /// Gets or sets the relationship type.
        /// </summary>
        public RelationshipType RelationshipType
        {
            get
            {
                return Database.Current.Select<RelationshipType>(this.ID, ValueTables.RelationshipChange, Columns.RelationshipType);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.RelationshipChange, Columns.RelationshipType, value);
                NotifyPropertyChanged("RelationshipType");
            }
        }
        #endregion Property: RelationshipType

        #region Property: Attributes
        /// <summary>
        /// Gets the attributes to change.
        /// </summary>
        public ReadOnlyCollection<AttributeChange> Attributes
        {
            get
            {
                return Database.Current.SelectAll<AttributeChange>(this.ID, ValueTables.RelationshipChangeAttributeChange, Columns.AttributeChange).AsReadOnly();
            }
        }
        #endregion Property: Attributes

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: RelationshipChange()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static RelationshipChange()
        {
            // Relationship type
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.RelationshipType, new Tuple<Type, EntryType>(typeof(RelationshipType), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.RelationshipChange, typeof(RelationshipChange), dict);

            // Attributes
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.AttributeChange, new Tuple<Type, EntryType>(typeof(AttributeChange), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.RelationshipChangeAttributeChange, typeof(RelationshipChange), dict);
        }
        #endregion Static Constructor: RelationshipChange()

        #region Constructor: RelationshipChange()
        /// <summary>
        /// Creates a new relationship change.
        /// </summary>
        public RelationshipChange()
            : base()
        {
        }
        #endregion Constructor: RelationshipChange()

        #region Constructor: RelationshipChange(uint id)
        /// <summary>
        /// Creates a new relationship change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a relationship change from.</param>
        private RelationshipChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: RelationshipChange(uint id)

        #region Constructor: RelationshipChange(RelationshipType relationshipType)
        /// <summary>
        /// Creates a change for the given relationship type.
        /// </summary>
        /// <param name="relationshipType">The relationship type to create a change for.</param>
        public RelationshipChange(RelationshipType relationshipType)
        {
            this.RelationshipType = relationshipType;
        }
        #endregion Constructor: RelationshipChange(RelationshipType relationshipType)

        #region Constructor: RelationshipChange(RelationshipChange relationshipChange)
        /// <summary>
        /// Clones a relationship change.
        /// </summary>
        /// <param name="relationshipChange">The relationship change to clone.</param>
        public RelationshipChange(RelationshipChange relationshipChange)
            : base(relationshipChange)
        {
            if (relationshipChange != null)
            {
                Database.Current.StartChange();

                this.RelationshipType = relationshipChange.RelationshipType;
                foreach (AttributeChange attributeChange in relationshipChange.Attributes)
                    AddAttribute(new AttributeChange(attributeChange));

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: RelationshipChange(RelationshipChange relationshipChange)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddAttribute(AttributeChange attributeChange)
        /// <summary>
        /// Adds an attribute change.
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
                Database.Current.Insert(this.ID, ValueTables.RelationshipChangeAttributeChange, Columns.AttributeChange, attributeChange);
                NotifyPropertyChanged("Attributes");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddAttribute(AttributeChange attributeChange)

        #region Method: RemoveAttribute(AttributeChange attributeChange)
        /// <summary>
        /// Removes an attribute change.
        /// </summary>
        /// <param name="attributeChange">The attribute change to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveAttribute(AttributeChange attributeChange)
        {
            if (attributeChange != null)
            {
                if (HasAttribute(attributeChange.Attribute))
                {
                    // Remove the attribute change
                    Database.Current.Remove(this.ID, ValueTables.RelationshipChangeAttributeChange, Columns.AttributeChange, attributeChange);
                    NotifyPropertyChanged("Attributes");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveAttribute(AttributeChange attributeChange)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasAttribute(Attribute attribute)
        /// <summary>
        /// Checks if this relationship change has the given attribute.
        /// </summary>
        /// <param name="attribute">The attribute to check.</param>
        /// <returns>Returns true when the relationship change has the attribute.</returns>
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

        #region Method: Remove()
        /// <summary>
        /// Remove the relationship change.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the attributes
            foreach (AttributeChange attributeChange in this.Attributes)
                attributeChange.Remove();
            Database.Current.Remove(this.ID, ValueTables.RelationshipChangeAttributeChange);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: RelationshipChange

    #region Class: CombinedRelationship
    /// <summary>
    /// A combined relationship.
    /// </summary>
    public sealed class CombinedRelationship : IdHolder
    {

        #region Properties and Fields

        #region Property: Relationship1
        /// <summary>
        /// Gets or sets the first relationship of the combined relationship. Make sure to choose between Relationship1 and CombinedRelationship1!
        /// </summary>
        public Relationship Relationship1
        {
            get
            {
                return Database.Current.Select<Relationship>(this.ID, GenericTables.CombinedRelationship, Columns.Relationship1);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.CombinedRelationship, Columns.Relationship1, value);
                NotifyPropertyChanged("Relationship1");
            }
        }
        #endregion Property: Relationship1

        #region Property: CombinedRelationship1
        /// <summary>
        /// Gets or sets the first relationship of the combined relationship. Make sure to choose between Relationship1 and CombinedRelationship1!
        /// </summary>
        public CombinedRelationship CombinedRelationship1
        {
            get
            {
                return Database.Current.Select<CombinedRelationship>(this.ID, GenericTables.CombinedRelationship, Columns.CombinedRelationship1);
            }
            set
            {
                if (!this.Equals(value))
                {
                    Database.Current.Update(this.ID, GenericTables.CombinedRelationship, Columns.CombinedRelationship1, value);
                    NotifyPropertyChanged("CombinedRelationship1");
                }
            }
        }
        #endregion Property: CombinedRelationship1

        #region Property: Operator
        /// <summary>
        /// Gets or sets the logical operator of the combined relationship.
        /// </summary>
        public LogicalOperator Operator
        {
            get
            {
                return Database.Current.Select<LogicalOperator>(this.ID, GenericTables.CombinedRelationship, Columns.Operator);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.CombinedRelationship, Columns.Operator, value);
                NotifyPropertyChanged("Operator");
            }
        }
        #endregion Property: Operator

        #region Property: Relationship2
        /// <summary>
        /// Gets or sets the second relationship of the combined relationship. Make sure to choose between Relationship2 and CombinedRelationship2!
        /// </summary>
        public Relationship Relationship2
        {
            get
            {
                return Database.Current.Select<Relationship>(this.ID, GenericTables.CombinedRelationship, Columns.Relationship2);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.CombinedRelationship, Columns.Relationship2, value);
                NotifyPropertyChanged("Relationship2");
            }
        }
        #endregion Property: Relationship2

        #region Property: CombinedRelationship2
        /// <summary>
        /// Gets or sets the second relationship of the combined relationship. Make sure to choose between Relationship2 and CombinedRelationship2!
        /// </summary>
        public CombinedRelationship CombinedRelationship2
        {
            get
            {
                return Database.Current.Select<CombinedRelationship>(this.ID, GenericTables.CombinedRelationship, Columns.CombinedRelationship2);
            }
            set
            {
                if (!this.Equals(value))
                {
                    Database.Current.Update(this.ID, GenericTables.CombinedRelationship, Columns.CombinedRelationship2, value);
                    NotifyPropertyChanged("CombinedRelationship2");
                }
            }
        }
        #endregion Property: CombinedRelationship2

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: CombinedRelationship()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static CombinedRelationship()
        {
            // Relationships
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Relationship1, new Tuple<Type, EntryType>(typeof(Relationship), EntryType.Nullable));
            dict.Add(Columns.CombinedRelationship1, new Tuple<Type, EntryType>(typeof(CombinedRelationship), EntryType.Nullable));
            dict.Add(Columns.Relationship2, new Tuple<Type, EntryType>(typeof(Relationship), EntryType.Nullable));
            dict.Add(Columns.CombinedRelationship2, new Tuple<Type, EntryType>(typeof(CombinedRelationship), EntryType.Nullable));
            Database.Current.AddTableDefinition(GenericTables.CombinedRelationship, typeof(CombinedRelationship), dict);
        }
        #endregion Static Constructor: CombinedRelationship()

        #region Constructor: CombinedRelationship()
        /// <summary>
        /// Creates a new combined relationship.
        /// </summary>
        public CombinedRelationship()
            : base()
        {
        }
        #endregion Constructor: CombinedRelationship()

        #region Constructor: CombinedRelationship(uint id)
        /// <summary>
        /// Creates a new combined relationship with the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new combined relationship from.</param>
        private CombinedRelationship(uint id)
            : base(id)
        {
        }
        #endregion Constructor: CombinedRelationship(uint id)

        #region Constructor: CombinedRelationship(CombinedRelationship combinedRelationship)
        /// <summary>
        /// Clones the combined relationship.
        /// </summary>
        /// <param name="combinedRelationship">The combined relationship to clone.</param>
        public CombinedRelationship(CombinedRelationship combinedRelationship)
            : base()
        {
            if (combinedRelationship != null)
            {
                Database.Current.StartChange();

                this.Relationship1 = combinedRelationship.Relationship1;
                this.Operator = combinedRelationship.Operator;
                this.Relationship2 = combinedRelationship.Relationship2;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: CombinedRelationship(CombinedRelationship combinedRelationship)

        #region Constructor: CombinedRelationship(Relationship relationship1, LogicalOperator logicalOperator, Relationship relationship2)
        /// <summary>
        /// Creates a combined relationship of the given relationships.
        /// </summary>
        /// <param name="relationship1">The first relationship.</param>
        /// <param name="logicalOperator">The operator.</param>
        /// <param name="relationship2">The second relationship.</param>
        public CombinedRelationship(Relationship relationship1, LogicalOperator logicalOperator, Relationship relationship2)
            : base()
    {
            Database.Current.StartChange();

            this.Relationship1 = relationship1;
            this.Operator = logicalOperator;
            this.Relationship2 = relationship2;

            Database.Current.StopChange();
        }
        #endregion Constructor: CombinedRelationship(Relationship relationship1, LogicalOperator logicalOperator, Relationship relationship2)

        #region Constructor: CombinedRelationship(Relationship relationship1, LogicalOperator logicalOperator, CombinedRelationship combinedRelationship2)
        /// <summary>
        /// Creates a combined relationship of the given relationships.
        /// </summary>
        /// <param name="relationship1">The first relationship.</param>
        /// <param name="logicalOperator">The operator.</param>
        /// <param name="combinedRelationship2">The second relationship.</param>
        public CombinedRelationship(Relationship relationship1, LogicalOperator logicalOperator, CombinedRelationship combinedRelationship2)
            : base()
        {
            Database.Current.StartChange();

            this.Relationship1 = relationship1;
            this.Operator = logicalOperator;
            this.CombinedRelationship2 = combinedRelationship2;

            Database.Current.StopChange();
        }
        #endregion Constructor: CombinedRelationship(Relationship relationship1, LogicalOperator logicalOperator, CombinedRelationship combinedRelationship2)

        #region Constructor: CombinedRelationship(CombinedRelationship combinedRelationship1, LogicalOperator logicalOperator, CombinedRelationship combinedRelationship2)
        /// <summary>
        /// Creates a combined relationship of the given relationships.
        /// </summary>
        /// <param name="combinedRelationship1">The first relationship.</param>
        /// <param name="logicalOperator">The operator.</param>
        /// <param name="combinedRelationship2">The second relationship.</param>
        public CombinedRelationship(CombinedRelationship combinedRelationship1, LogicalOperator logicalOperator, CombinedRelationship combinedRelationship2)
            : base()
        {
            Database.Current.StartChange();

            this.CombinedRelationship1 = combinedRelationship1;
            this.Operator = logicalOperator;
            this.CombinedRelationship2 = combinedRelationship2;

            Database.Current.StopChange();
        }
        #endregion Constructor: CombinedRelationship(CombinedRelationship combinedRelationship1, LogicalOperator logicalOperator, CombinedRelationship combinedRelationship2)

        #region Constructor: CombinedRelationship(CombinedRelationship combinedRelationship1, LogicalOperator logicalOperator, Relationship relationship2)
        /// <summary>
        /// Creates a combined relationship of the given relationships.
        /// </summary>
        /// <param name="combinedRelationship1">The first relationship.</param>
        /// <param name="logicalOperator">The operator.</param>
        /// <param name="relationship2">The second relationship.</param>
        public CombinedRelationship(CombinedRelationship combinedRelationship1, LogicalOperator logicalOperator, Relationship relationship2)
            : base()
        {
            Database.Current.StartChange();

            this.CombinedRelationship1 = combinedRelationship1;
            this.Operator = logicalOperator;
            this.Relationship2 = relationship2;

            Database.Current.StopChange();
        }
        #endregion Constructor: CombinedRelationship(CombinedRelationship combinedRelationship1, LogicalOperator logicalOperator, Relationship relationship2)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetRelationships()
        /// <summary>
        /// Get all relationships of this combined relationship.
        /// </summary>
        /// <returns>All relationships of this combined relationship.</returns>
        public List<Relationship> GetRelationships()
        {
            List<Relationship> relationships = new List<Relationship>();

            if (this.Relationship1 != null)
                relationships.Add(this.Relationship1);
            if (this.Relationship2 != null)
                relationships.Add(this.Relationship2);

            if (this.CombinedRelationship1 != null)
                relationships.AddRange(this.CombinedRelationship1.GetRelationships());
            if (this.CombinedRelationship2 != null)
                relationships.AddRange(this.CombinedRelationship2.GetRelationships());

            return relationships;
        }
        #endregion Method: GetRelationships()

        #endregion Method Group: Other

    }
    #endregion Class: Relationship

}
