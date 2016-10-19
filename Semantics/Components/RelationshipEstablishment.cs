/**************************************************************************
 * 
 * RelationshipEstablishment.cs
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
using Common;
using Semantics.Abstractions;
using Semantics.Data;
using Semantics.Utilities;

namespace Semantics.Components
{

    #region Class: RelationshipEstablishment
    /// <summary>
    /// An effect that establishes a relationship between the actor and target.
    /// </summary>
    public sealed class RelationshipEstablishment : Effect
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
                return Database.Current.Select<RelationshipType>(this.ID, ValueTables.RelationshipEstablishment, Columns.RelationshipType);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.RelationshipEstablishment, Columns.RelationshipType, value);
                NotifyPropertyChanged("RelationshipType");
            }
        }
        #endregion Property: RelationshipType

        #region Property: Source
        /// <summary>
        /// Gets or sets the source for the relationship: the actor or target.
        /// </summary>
        public ActorTarget Source
        {
            get
            {
                return Database.Current.Select<ActorTarget>(this.ID, ValueTables.RelationshipEstablishment, Columns.Source);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.RelationshipEstablishment, Columns.Source, value);
                NotifyPropertyChanged("Source");
            }
        }
        #endregion Property: Source

        #region Property: Target
        /// <summary>
        /// Gets or sets the target for the relationship: the actor, target, or a reference.
        /// </summary>
        public ActorTargetArtifactReference Target
        {
            get
            {
                return Database.Current.Select<ActorTargetArtifactReference>(this.ID, ValueTables.RelationshipEstablishment, Columns.Target);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.RelationshipEstablishment, Columns.Target, value);
                NotifyPropertyChanged("Target");
            }
        }
        #endregion Property: Target

        #region Property: TargetReference
        /// <summary>
        /// Gets or sets the reference, in case the Target has been set to Reference.
        /// </summary>
        public Reference TargetReference
        {
            get
            {
                return Database.Current.Select<Reference>(this.ID, ValueTables.RelationshipEstablishment, Columns.TargetReference);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.RelationshipEstablishment, Columns.TargetReference, value);
                NotifyPropertyChanged("TargetReference");
            }
        }
        #endregion Property: TargetReference

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: RelationshipEstablishment()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static RelationshipEstablishment()
        {
            // Relationship type
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.RelationshipType, new Tuple<Type, EntryType>(typeof(RelationshipType), EntryType.Nullable));
            Database.Current.AddTableDefinition(ValueTables.RelationshipEstablishment, typeof(RelationshipEstablishment), dict);
        }
        #endregion Static Constructor: RelationshipEstablishment()

        #region Constructor: RelationshipEstablishment()
        /// <summary>
        /// Creates a new relationship establishment.
        /// </summary>
        public RelationshipEstablishment()
            : base()
        {
        }
        #endregion Constructor: RelationshipEstablishment()

        #region Constructor: RelationshipEstablishment(uint id)
        /// <summary>
        /// Creates a new relationship establishment from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a relationship establishment from.</param>
        private RelationshipEstablishment(uint id)
            : base(id)
        {
        }
        #endregion Constructor: RelationshipEstablishment(uint id)

        #region Constructor: RelationshipEstablishment(RelationshipEstablishment relationshipEstablishment)
        /// <summary>
        /// Clones a relationship establishment.
        /// </summary>
        /// <param name="relationshipEstablishment">The relationship establishment to clone.</param>
        public RelationshipEstablishment(RelationshipEstablishment relationshipEstablishment)
            : base(relationshipEstablishment)
        {
            if (relationshipEstablishment != null)
            {
                Database.Current.StartChange();

                this.RelationshipType = relationshipEstablishment.RelationshipType;
                this.Source = relationshipEstablishment.Source;
                this.Target = relationshipEstablishment.Target;
                this.TargetReference = relationshipEstablishment.TargetReference;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: RelationshipEstablishment(RelationshipEstablishment relationshipEstablishment)

        #region Constructor: RelationshipEstablishment(RelationshipType relationshipType, ActorTarget source)
        /// <summary>
        /// Creates a new relationship establishment.
        /// </summary>
        /// <param name="relationshipType">The relationship type.</param>
        /// <param name="source">The source.</param>
        public RelationshipEstablishment(RelationshipType relationshipType, ActorTarget source)
            : base()
        {
            Database.Current.StartChange();

            this.RelationshipType = relationshipType;
            this.Source = source;

            Database.Current.StopChange();
        }
        #endregion Constructor: RelationshipEstablishment(RelationshipType relationshipType, ActorTarget source)

        #region Constructor: RelationshipEstablishment(RelationshipType relationshipType, ActorTarget source, ActorTargetReference target)
        /// <summary>
        /// Creates a new relationship establishment.
        /// </summary>
        /// <param name="relationshipType">The relationship type.</param>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        public RelationshipEstablishment(RelationshipType relationshipType, ActorTarget source, ActorTargetArtifactReference target)
            : base()
        {
            Database.Current.StartChange();

            this.RelationshipType = relationshipType;
            this.Source = source;
            this.Target = target;

            Database.Current.StopChange();
        }
        #endregion Constructor: RelationshipEstablishment(RelationshipType relationshipType, ActorTarget source, ActorTargetReference target)

        #endregion Method Group: Constructors

    }
    #endregion Class: RelationshipEstablishment

}