/**************************************************************************
 * 
 * Predicate.cs
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

namespace Semantics.Components
{

    #region Class: Predicate
    /// <summary>
    /// A predicate keeps track of actor and target of a predicate type under certain requirements.
    /// </summary>
    public sealed class Predicate : IdHolder, IVariableReferenceHolder
    {

        #region Properties and Fields

        #region Property: PredicateType
        /// <summary>
        /// Gets the predicate type of the predicate.
        /// </summary>
        public PredicateType PredicateType
        {
            get
            {
                return Database.Current.Select<PredicateType>(this.ID, GenericTables.Predicate, Columns.PredicateType);
            }
            private set
            {
                Database.Current.Update(this.ID, GenericTables.Predicate, Columns.PredicateType, value);
                NotifyPropertyChanged("PredicateType");
            }
        }
        #endregion Property: PredicateType

        #region Property: Actor
        /// <summary>
        /// Gets or sets the actor of the predicate.
        /// </summary>
        public Entity Actor
        {
            get
            {
                return Database.Current.Select<Entity>(this.ID, GenericTables.Predicate, Columns.Actor);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Predicate, Columns.Actor, value);
                NotifyPropertyChanged("Actor");
            }
        }
        #endregion Property: Actor

        #region Property: Target
        /// <summary>
        /// Gets or sets the target of the predicate.
        /// </summary>
        public Entity Target
        {
            get
            {
                return Database.Current.Select<Entity>(this.ID, GenericTables.Predicate, Columns.Target);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Predicate, Columns.Target, value);
                NotifyPropertyChanged("Target");
                NotifyPropertyChanged("NamesString");
            }
        }
        #endregion Property: Target

        #region Property: ActorRequirements
        /// <summary>
        /// Gets the requirements for the actor.
        /// </summary>
        public Requirements ActorRequirements
        {
            get
            {
                return Database.Current.Select<Requirements>(this.ID, GenericTables.Predicate, Columns.ActorRequirements);
            }
            private set
            {
                Database.Current.Update(this.ID, GenericTables.Predicate, Columns.ActorRequirements, value);
                NotifyPropertyChanged("ActorRequirements");
            }
        }
        #endregion Property: ActorRequirements

        #region Property: TargetRequirements
        /// <summary>
        /// Gets the requirements for the target.
        /// </summary>
        public Requirements TargetRequirements
        {
            get
            {
                return Database.Current.Select<Requirements>(this.ID, GenericTables.Predicate, Columns.TargetRequirements);
            }
            private set
            {
                Database.Current.Update(this.ID, GenericTables.Predicate, Columns.TargetRequirements, value);
                NotifyPropertyChanged("TargetRequirements");
            }
        }
        #endregion Property: TargetRequirements

        #region Property: SpatialRequirementBetweenActorAndTarget
        /// <summary>
        /// Gets or sets the spatial condition between the actor and target.
        /// </summary>
        public SpatialRequirement SpatialRequirementBetweenActorAndTarget
        {
            get
            {
                return Database.Current.Select<SpatialRequirement>(this.ID, GenericTables.Predicate, Columns.SpatialRequirementBetweenActorAndTarget);
            }
            set
            {
                SpatialRequirement spatialRequirement = this.SpatialRequirementBetweenActorAndTarget;
                if (spatialRequirement != value)
                {
                    if (spatialRequirement != null)
                        spatialRequirement.Remove();

                    Database.Current.Update(this.ID, GenericTables.Predicate, Columns.SpatialRequirementBetweenActorAndTarget, value);
                    NotifyPropertyChanged("SpatialRequirementBetweenActorAndTarget");
                }
            }
        }
        #endregion Property: SpatialRequirementBetweenActorAndTarget

        #region Property: Variables
        /// <summary>
        /// Gets the variables.
        /// </summary>
        public ReadOnlyCollection<Variable> Variables
        {
            get
            {
                return Database.Current.SelectAll<Variable>(this.ID, GenericTables.PredicateVariable, Columns.Variable).AsReadOnly();
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
                return Database.Current.SelectAll<Reference>(this.ID, GenericTables.PredicateReference, Columns.Reference).AsReadOnly();
            }
        }
        #endregion Property: References

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: Predicate()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static Predicate()
        {
            // Predicate type, actor, and target
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.PredicateType, new Tuple<Type, EntryType>(typeof(PredicateType), EntryType.Nullable));
            dict.Add(Columns.Actor, new Tuple<Type, EntryType>(typeof(Entity), EntryType.Nullable));
            dict.Add(Columns.Target, new Tuple<Type, EntryType>(typeof(Entity), EntryType.Nullable));
            Database.Current.AddTableDefinition(GenericTables.Predicate, typeof(Predicate), dict);

            // Variables
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Variable, new Tuple<Type, EntryType>(typeof(Variable), EntryType.Intermediate));
            Database.Current.AddTableDefinition(GenericTables.PredicateVariable, typeof(Predicate), dict);

            // References
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Reference, new Tuple<Type, EntryType>(typeof(Reference), EntryType.Intermediate));
            Database.Current.AddTableDefinition(GenericTables.PredicateReference, typeof(Predicate), dict);
        }
        #endregion Static Constructor: Predicate()

        #region Constructor: Predicate(uint id)
        /// <summary>
        /// Creates a new predicate with the given ID.
        /// </summary>
        /// <param name="id">The ID to create a new predicate from.</param>
        private Predicate(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Predicate(uint id)

        #region Constructor: Predicate(Predicate predicate)
        /// <summary>
        /// Clones the predicate.
        /// </summary>
        /// <param name="predicate">The predicate to clone.</param>
        public Predicate(Predicate predicate)
            : base()
        {
            if (predicate != null)
            {
                Database.Current.StartChange();

                this.Actor = predicate.Actor;
                this.Target = predicate.Target;
                if (predicate.ActorRequirements != null)
                    this.ActorRequirements = new Requirements(predicate.ActorRequirements);
                if (predicate.TargetRequirements != null)
                    this.TargetRequirements = new Requirements(predicate.TargetRequirements);
                if (predicate.SpatialRequirementBetweenActorAndTarget != null)
                    this.SpatialRequirementBetweenActorAndTarget = new SpatialRequirement(predicate.SpatialRequirementBetweenActorAndTarget);
                foreach (Reference reference in predicate.References)
                    AddReference(reference.Clone());
                foreach (Variable variable in predicate.Variables)
                    AddVariable(variable.Clone());

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Predicate(Predicate predicate)

        #region Constructor: Predicate(PredicateType predicateType)
        /// <summary>
        /// Creates a predicate from the given predicate type.
        /// </summary>
        /// <param name="predicateType">The predicate type to create a predicate from.</param>
        public Predicate(PredicateType predicateType)
            : base()
        {
            if (predicateType != null)
            {
                Database.Current.StartChange();

                this.ActorRequirements = new Requirements();
                this.TargetRequirements = new Requirements();
                this.PredicateType = predicateType;
                predicateType.AddPredicate(this);

                Database.Current.StopChange();
            }
            else
                Remove();
        }
        #endregion Constructor: Predicate(PredicateType predicateType)

        #region Constructor: Predicate(PredicateType predicateType, Entity actor, Entity target)
        /// <summary>
        /// Creates a predicate from the given predicate type, and set the actor and target.
        /// </summary>
        /// <param name="predicateType">The predicate type to create a predicate from.</param>
        /// <param name="actor">The actor of the predicate.</param>
        /// <param name="target">The target of the predicate.</param>
        public Predicate(PredicateType predicateType, Entity actor, Entity target)
            : this(predicateType)
        {
            if (predicateType != null)
            {
                Database.Current.StartChange();
                Database.Current.QueryBegin();

                this.Actor = actor;
                this.Target = target;

                Database.Current.QueryCommit();
                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Predicate(PredicateType predicateType, Entity actor, Entity target)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

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
                Database.Current.Insert(this.ID, GenericTables.PredicateVariable, Columns.Variable, variable);
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
                    Database.Current.Remove(this.ID, GenericTables.PredicateVariable, Columns.Variable, variable);
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
                Database.Current.Insert(this.ID, GenericTables.PredicateReference, Columns.Reference, reference);
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
                    Database.Current.Remove(this.ID, GenericTables.PredicateReference, Columns.Reference, reference);
                    NotifyPropertyChanged("References");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveReference(Reference reference)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasVariable(Variable variable)
        /// <summary>
        /// Checks if this predicate has the given variable.
        /// </summary>
        /// <param name="variable">The variable to check.</param>
        /// <returns>Returns true when this predicate has the variable.</returns>
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
        /// Checks if this predicate has the given reference.
        /// </summary>
        /// <param name="reference">The reference to check.</param>
        /// <returns>Returns true when this predicate has the reference.</returns>
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

        #region Method: Remove()
        /// <summary>
        /// Remove the predicate.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the requirements
            if (this.ActorRequirements != null)
                this.ActorRequirements.Remove();
            if (this.TargetRequirements != null)
                this.TargetRequirements.Remove();

            // Remove the spatial requirement
            if (this.SpatialRequirementBetweenActorAndTarget != null)
                this.SpatialRequirementBetweenActorAndTarget.Remove();

            // Remove the variables
            foreach (Variable variable in this.Variables)
                variable.Remove();
            Database.Current.Remove(this.ID, GenericTables.PredicateVariable);

            // Remove the references
            foreach (Reference reference in this.References)
                reference.Remove();
            Database.Current.Remove(this.ID, GenericTables.PredicateReference);

            PredicateType predicateType = this.PredicateType;

            base.Remove();

            // Remove the predicate from the predicate type
            if (predicateType != null)
                predicateType.RemovePredicate(this);

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
            if (this.PredicateType != null)
                return this.PredicateType.ToString();

            return base.ToString();
        }
        #endregion Method: ToString()

        #endregion Method Group: Other

    }
    #endregion Class: Predicate

}