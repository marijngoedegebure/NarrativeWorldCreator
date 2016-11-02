/**************************************************************************
 * 
 * RelationshipTypeBase.cs
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
using System.Collections.ObjectModel;
using Semantics.Abstractions;
using Semantics.Components;
using SemanticsEngine.Components;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Abstractions
{

    #region Class: RelationshipTypeBase
    /// <summary>
    /// A base of a relationship type.
    /// </summary>
    public class RelationshipTypeBase : AbstractionBase
    {

        #region Properties and Fields

        #region Property: RelationshipType
        /// <summary>
        /// Gets the relationship type of which this is a relationship type base.
        /// </summary>
        protected internal RelationshipType RelationshipType
        {
            get
            {
                return this.IdHolder as RelationshipType;
            }
        }
        #endregion Property: RelationshipType

        #region Property: Parameters
        /// <summary>
        /// The parameters of the relationship type.
        /// </summary>
        private AttributeBase[] parameters = null;
        
        /// <summary>
        /// Gets all parameters of the relationship type.
        /// </summary>
        public ReadOnlyCollection<AttributeBase> Parameters
        {
            get
            {
                if (parameters == null)
                {
                    LoadParameters();
                    if (parameters == null)
                        return new List<AttributeBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<AttributeBase>(parameters);
            }
        }

        /// <summary>
        /// Loads the parameters.
        /// </summary>
        private void LoadParameters()
        {
            if (this.RelationshipType != null)
            {
                List<AttributeBase> parameterBases = new List<AttributeBase>();
                foreach (Attribute parameter in this.RelationshipType.Parameters)
                    parameterBases.Add(BaseManager.Current.GetBase<AttributeBase>(parameter));
                parameters = parameterBases.ToArray();
            }
        }
        #endregion Property: Parameters

        #region Property: Attributes
        /// <summary>
        /// The attributes of the relationship type.
        /// </summary>
        private AttributeBase[] attributes = null;

        /// <summary>
        /// Gets all attributes of the relationship type.
        /// </summary>
        public ReadOnlyCollection<AttributeBase> Attributes
        {
            get
            {
                if (attributes == null)
                {
                    LoadAttributes();
                    if (attributes == null)
                        return new List<AttributeBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<AttributeBase>(attributes);
            }
        }

        /// <summary>
        /// Loads the attributes.
        /// </summary>
        private void LoadAttributes()
        {
            if (this.RelationshipType != null)
            {
                List<AttributeBase> attributeBases = new List<AttributeBase>();
                foreach (Attribute attribute in this.RelationshipType.Attributes)
                    attributeBases.Add(BaseManager.Current.GetBase<AttributeBase>(attribute));
                attributes = attributeBases.ToArray();
            }
        }
        #endregion Property: Attributes

        #region Property: Relationships
        /// <summary>
        /// The relationships of the relationship type.
        /// </summary>
        private RelationshipBase[] relationships = null;
        
        /// <summary>
        /// Gets the relationships of the relationship type.
        /// </summary>
        public ReadOnlyCollection<RelationshipBase> Relationships
        {
            get
            {
                if (relationships == null)
                {
                    LoadRelationships();
                    if (relationships == null)
                        return new List<RelationshipBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<RelationshipBase>(relationships);
            }
        }

        /// <summary>
        /// Loads the relationships.
        /// </summary>
        private void LoadRelationships()
        {
            if (this.RelationshipType != null)
            {
                List<RelationshipBase> relationshipBases = new List<RelationshipBase>();
                foreach (Relationship relationship in this.RelationshipType.PersonalRelationships)
                    relationshipBases.Add(BaseManager.Current.GetBase<RelationshipBase>(relationship));
                relationships = relationshipBases.ToArray();
            }
        }
        #endregion Property: Relationships

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: RelationshipTypeBase(RelationshipType relationshipType)
        /// <summary>
        /// Create a relationship type base from the given relationship type.
        /// </summary>
        /// <param name="relationshipType">The relationship type to create a relationship type base from.</param>
        protected internal RelationshipTypeBase(RelationshipType relationshipType)
            : base(relationshipType)
        {
            if (relationshipType != null)
            {
                if (BaseManager.PreloadProperties)
                {
                    LoadParameters();
                    LoadAttributes();
                    LoadRelationships();
                }
            }
        }
        #endregion Constructor: RelationshipTypeBase(RelationshipType relationshipType)

        #endregion Method Group: Constructors

    }
    #endregion Class: RelationshipTypeBase

}
