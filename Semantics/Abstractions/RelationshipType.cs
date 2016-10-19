/**************************************************************************
 * 
 * RelationshipType.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Common;
using Semantics.Components;
using Semantics.Data;
using Semantics.Utilities;

namespace Semantics.Abstractions
{

    #region Class: RelationshipType
    /// <summary>
    /// A relationship type with relationships and parameters.
    /// </summary>
    public class RelationshipType : Abstraction, IComparable<RelationshipType>
    {

        #region Properties and Fields

        #region Property: Parameters
        /// <summary>
        /// Gets all parameters of the relationship type.
        /// </summary>
        public ReadOnlyCollection<Attribute> Parameters
        {
            get
            {
                List<Attribute> parameters = new List<Attribute>();
                parameters.AddRange(this.PersonalParameters);
                parameters.AddRange(this.InheritedParameters);
                return parameters.AsReadOnly();
            }
        }
        #endregion Property: Parameters

        #region Property: PersonalParameters
        /// <summary>
        /// Gets the personal parameters of the relationship type.
        /// </summary>
        public ReadOnlyCollection<Attribute> PersonalParameters
        {
            get
            {
                return Database.Current.SelectAll<Attribute>(this.ID, GenericTables.RelationshipTypeParameter, Columns.Parameter).AsReadOnly();
            }
        }
        #endregion Property: PersonalParameters

        #region Property: InheritedParameters
        /// <summary>
        /// Gets the inherited parameters of the relationship type.
        /// </summary>
        public ReadOnlyCollection<Attribute> InheritedParameters
        {
            get
            {
                List<Attribute> inheritedParameters = new List<Attribute>();
                foreach (Node parent in this.PersonalParents)
                    inheritedParameters.AddRange(((RelationshipType)parent).Parameters);
                return inheritedParameters.AsReadOnly();
            }
        }
        #endregion Property: InheritedParameters

        #region Property: Attributes
        /// <summary>
        /// Gets all attributes of the relationship type.
        /// </summary>
        public ReadOnlyCollection<Attribute> Attributes
        {
            get
            {
                List<Attribute> attributes = new List<Attribute>();
                attributes.AddRange(this.PersonalAttributes);
                attributes.AddRange(this.InheritedAttributes);
                return attributes.AsReadOnly();
            }
        }
        #endregion Property: Attributes

        #region Property: PersonalAttributes
        /// <summary>
        /// Gets the personal attributes of the relationship type.
        /// </summary>
        public ReadOnlyCollection<Attribute> PersonalAttributes
        {
            get
            {
                return Database.Current.SelectAll<Attribute>(this.ID, GenericTables.RelationshipTypeAttribute, Columns.Attribute).AsReadOnly();
            }
        }
        #endregion Property: PersonalAttributes

        #region Property: InheritedAttributes
        /// <summary>
        /// Gets the inherited attributes of the relationship type.
        /// </summary>
        public ReadOnlyCollection<Attribute> InheritedAttributes
        {
            get
            {
                List<Attribute> inheritedAttributes = new List<Attribute>();
                foreach (Node parent in this.PersonalParents)
                    inheritedAttributes.AddRange(((RelationshipType)parent).Attributes);
                return inheritedAttributes.AsReadOnly();
            }
        }
        #endregion Property: InheritedAttributes

        #region Property: Relationships
        /// <summary>
        /// Gets the personal and inherited relationships.
        /// </summary>
        public ReadOnlyCollection<Relationship> Relationships
        {
            get
            {
                List<Relationship> relationships = new List<Relationship>();
                relationships.AddRange(this.PersonalRelationships);
                relationships.AddRange(this.InheritedRelationships);
                return relationships.AsReadOnly();
            }
        }
        #endregion Property: Relationships

        #region Property: PersonalRelationships
        /// <summary>
        /// Gets the personal relationships of the relationship type.
        /// </summary>
        public ReadOnlyCollection<Relationship> PersonalRelationships
        {
            get
            {
                return Database.Current.SelectAll<Relationship>(GenericTables.Relationship, Columns.RelationshipType, this).AsReadOnly();
            }
        }
        #endregion Property: PersonalRelationships

        #region Property: InheritedRelationships
        /// <summary>
        /// Gets the inherited relationships.
        /// </summary>
        public ReadOnlyCollection<Relationship> InheritedRelationships
        {
            get
            {
                // The inherited relationships are the relationships of the children!
                List<Relationship> inheritedRelationships = new List<Relationship>();
                foreach (Node child in this.PersonalChildren)
                    inheritedRelationships.AddRange(((RelationshipType)child).Relationships);
                return inheritedRelationships.AsReadOnly();
            }
        }
        #endregion Property: InheritedRelationships

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: RelationshipType()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static RelationshipType()
        {
            // Parameters
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Parameter, new Tuple<Type, EntryType>(typeof(Attribute), EntryType.Intermediate));
            Database.Current.AddTableDefinition(GenericTables.RelationshipTypeParameter, typeof(RelationshipType), dict);

            // Attributes
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Attribute, new Tuple<Type, EntryType>(typeof(Attribute), EntryType.Intermediate));
            Database.Current.AddTableDefinition(GenericTables.RelationshipTypeAttribute, typeof(RelationshipType), dict);
        }
        #endregion Static Constructor: RelationshipType()

        #region Constructor: RelationshipType()
        /// <summary>
        /// Creates a relationship type.
        /// </summary>
        public RelationshipType()
            : base()
        {
        }
        #endregion Constructor: RelationshipType()

        #region Constructor: RelationshipType(uint id)
        /// <summary>
        /// Creates a new relationship type from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a relationship type from.</param>
        protected RelationshipType(uint id)
            : base(id)
        {
        }
        #endregion Constructor: RelationshipType(uint id)

        #region Constructor: RelationshipType(string name)
        /// <summary>
        /// Creates a new relationship type with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the relationship type.</param>
        public RelationshipType(string name)
            : base(name)
        {
        }
        #endregion Constructor: RelationshipType(string name)

        #region Constructor: RelationshipType(RelationshipType relationshipType)
        /// <summary>
        /// Clones a relationship type.
        /// </summary>
        /// <param name="relationshipType">The relationship type to clone.</param>
        public RelationshipType(RelationshipType relationshipType)
            : base(relationshipType)
        {
            if (relationshipType != null)
            {
                Database.Current.StartChange();

                foreach (Attribute parameter in relationshipType.PersonalParameters)
                    AddParameter(parameter);
                foreach (Attribute attribute in relationshipType.PersonalAttributes)
                    AddAttribute(attribute);
                foreach (Relationship relationship in relationshipType.PersonalRelationships)
                    AddRelationship(new Relationship(relationship));

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: RelationshipType(RelationshipType relationshipType)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddParameter(Attribute parameter)
        /// <summary>
        /// Add a parameter to the relationship type.
        /// </summary>
        /// <param name="parameter">The parameter to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddParameter(Attribute parameter)
        {
            if (parameter != null)
            {
                // If the parameter is already available in all parameters, there is no use to add it
                if (HasParameter(parameter))
                    return Message.RelationExistsAlready;

                // Add the parameter to the parameters
                Database.Current.Insert(this.ID, GenericTables.RelationshipTypeParameter, Columns.Parameter, parameter);
                NotifyPropertyChanged("PersonalParameters");
                NotifyPropertyChanged("Parameters");

                // Add this parameter to all relationships
                foreach (Relationship relationship in this.PersonalRelationships)
                    relationship.AddParameter(new AttributeCondition(parameter));

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddParameter(Attribute parameter)

        #region Method: RemoveParameter(Attribute parameter)
        /// <summary>
        /// Remove a parameter from the relationship type.
        /// </summary>
        /// <param name="parameter">The parameter to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveParameter(Attribute parameter)
        {
            if (parameter != null)
            {
                if (HasParameter(parameter))
                {
                    // Remove the parameter
                    Database.Current.Remove(this.ID, GenericTables.RelationshipTypeParameter, Columns.Parameter, parameter);
                    NotifyPropertyChanged("PersonalParameters");
                    NotifyPropertyChanged("Parameters");

                    // Remove this parameter from all relationships
                    foreach (Relationship relationship in this.PersonalRelationships)
                    {
                        foreach (AttributeCondition relationshipParameter in relationship.Parameters)
                        {
                            if (parameter.Equals(relationshipParameter.Attribute))
                            {
                                relationship.RemoveParameter(relationshipParameter);
                                break;
                            }
                        }
                    }

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveParameter(Attribute parameter)

        #region Method: AddAttribute(Attribute attribute)
        /// <summary>
        /// Add a attribute to the relationship type.
        /// </summary>
        /// <param name="attribute">The attribute to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddAttribute(Attribute attribute)
        {
            if (attribute != null)
            {
                // If the attribute is already available in all attributes, there is no use to add it
                if (HasAttribute(attribute))
                    return Message.RelationExistsAlready;

                // Add the attribute to the attributes
                Database.Current.Insert(this.ID, GenericTables.RelationshipTypeAttribute, Columns.Attribute, attribute);
                NotifyPropertyChanged("PersonalAttributes");
                NotifyPropertyChanged("Attributes");

                // Add this attribute to all relationships
                foreach (Relationship relationship in this.PersonalRelationships)
                    relationship.AddAttribute(new AttributeValued(attribute));

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddAttribute(Attribute attribute)

        #region Method: RemoveAttribute(Attribute attribute)
        /// <summary>
        /// Remove a attribute from the relationship type.
        /// </summary>
        /// <param name="attribute">The attribute to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveAttribute(Attribute attribute)
        {
            if (attribute != null)
            {
                if (HasAttribute(attribute))
                {
                    // Remove the attribute
                    Database.Current.Remove(this.ID, GenericTables.RelationshipTypeAttribute, Columns.Attribute, attribute);
                    NotifyPropertyChanged("PersonalAttributes");
                    NotifyPropertyChanged("Attributes");

                    // Remove this attribute from all relationships
                    foreach (Relationship relationship in this.PersonalRelationships)
                    {
                        foreach (AttributeValued relationshipAttribute in relationship.Attributes)
                        {
                            if (attribute.Equals(relationshipAttribute.Attribute))
                            {
                                relationship.RemoveAttribute(relationshipAttribute);
                                break;
                            }
                        }
                    }

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveAttribute(Attribute attribute)

        #region Method: AddRelationship(Relationship relationship)
        /// <summary>
        /// Add a relationship to the relationship type.
        /// </summary>
        /// <param name="relationship">The relationship to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        internal Message AddRelationship(Relationship relationship)
        {
            if (relationship != null)
            {
                NotifyPropertyChanged("PersonalRelationships");
                NotifyPropertyChanged("Relationships");

                // Add the parameters
                foreach (Attribute parameter in this.Parameters)
                    relationship.AddParameter(new AttributeCondition(parameter));

                // Add the attributes
                foreach (Attribute attribute in this.Attributes)
                    relationship.AddAttribute(new AttributeValued(attribute));

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddRelationship(Relationship relationship)

        #region Method: RemoveRelationship(Relationship relationship)
        /// <summary>
        /// Remove a relationship from the relationship type.
        /// </summary>
        /// <param name="relationship">The relationship to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        internal Message RemoveRelationship(Relationship relationship)
        {
            if (relationship != null)
            {
                NotifyPropertyChanged("PersonalRelationships");
                NotifyPropertyChanged("Relationships");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveRelationship(Relationship relationship)        

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasParameter(Attribute parameter)
        /// <summary>
        /// Checks whether the relationship type has the given parameter.
        /// </summary>
        /// <param name="parameter">The parameter to check.</param>
        /// <returns>Returns true when the relationship type has the parameter.</returns>
        public bool HasParameter(Attribute parameter)
        {
            if (parameter != null)
            {
                foreach (Attribute myParameter in this.Parameters)
                {
                    if (parameter.Equals(myParameter))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasParameter(Attribute parameter)

        #region Method: HasAttribute(Attribute attribute)
        /// <summary>
        /// Checks whether the relationship type has the given attribute.
        /// </summary>
        /// <param name="attribute">The attribute to check.</param>
        /// <returns>Returns true when the relationship type has the attribute.</returns>
        public bool HasAttribute(Attribute attribute)
        {
            if (attribute != null)
            {
                foreach (Attribute myAttribute in this.Attributes)
                {
                    if (attribute.Equals(myAttribute))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasAttribute(Attribute attribute)

        #region Method: HasRelationship(Relationship relationship)
        /// <summary>
        /// Checks whether the relationship type has the given relationship.
        /// </summary>
        /// <param name="relationship">The relationship to check.</param>
        /// <returns>Returns true when the relationship type has the relationship.</returns>
        public bool HasRelationship(Relationship relationship)
        {
            if (relationship != null)
            {
                foreach (Relationship myRelationship in this.Relationships)
                {
                    if (relationship.Equals(myRelationship))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasRelationship(Relationship relationship)

        #region Method: Remove()
        /// <summary>
        /// Remove the relationship type.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();
            Database.Current.StartRemove();

            // Remove the parameters
            Database.Current.Remove(this.ID, GenericTables.RelationshipTypeParameter);

            // Remove the attributes
            Database.Current.Remove(this.ID, GenericTables.RelationshipTypeAttribute);

            // Remove the relationships
            foreach (Relationship relationship in this.PersonalRelationships)
                relationship.Remove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(RelationshipType other)
        /// <summary>
        /// Compares the relationship type to the other relationship type.
        /// </summary>
        /// <param name="other">The relationship type to compare to this relationship type.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(RelationshipType other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(RelationshipType other)

        #endregion Method Group: Other

    }
    #endregion Class: RelationshipType

}