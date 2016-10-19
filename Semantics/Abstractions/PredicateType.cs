/**************************************************************************
 * 
 * PredicateType.cs
 * 
 * Tim Tutenel & Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2009-2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Semantics.Components;
using Semantics.Data;
using Semantics.Utilities;

namespace Semantics.Abstractions
{

    #region Class: PredicateType
    /// <summary>
    /// A predicate type with predicates.
    /// </summary>
    public class PredicateType : Abstraction, IComparable<PredicateType>
    {
        
        #region Properties and Fields

        #region Property: Predicates
        /// <summary>
        /// Gets the personal and inherited predicates.
        /// </summary>
        public ReadOnlyCollection<Predicate> Predicates
        {
            get
            {
                List<Predicate> predicates = new List<Predicate>();
                predicates.AddRange(this.PersonalPredicates);
                predicates.AddRange(this.InheritedPredicates);
                return predicates.AsReadOnly();
            }
        }
        #endregion Property: Predicates

        #region Property: PersonalPredicates
        /// <summary>
        /// Gets the personal predicates of the predicate type.
        /// </summary>
        public ReadOnlyCollection<Predicate> PersonalPredicates
        {
            get
            {
                return Database.Current.SelectAll<Predicate>(GenericTables.Predicate, Columns.PredicateType, this).AsReadOnly();
            }
        }
        #endregion Property: PersonalPredicates

        #region Property: InheritedPredicates
        /// <summary>
        /// Gets the inherited predicates.
        /// </summary>
        public ReadOnlyCollection<Predicate> InheritedPredicates
        {
            get
            {
                // The inherited predicates are the predicates of the children!
                List<Predicate> inheritedPredicates = new List<Predicate>();
                foreach (Node child in this.PersonalChildren)
                    inheritedPredicates.AddRange(((PredicateType)child).Predicates);
                return inheritedPredicates.AsReadOnly();
            }
        }
        #endregion Property: InheritedPredicates

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: PredicateType()
        /// <summary>
        /// Creates a new predicate type.
        /// </summary>
        public PredicateType()
            : base()
        {
        }
        #endregion Constructor: PredicateType()

        #region Constructor: PredicateType(uint id)
        /// <summary>
        /// Creates a new predicate type from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the predicate type from.</param>
        protected PredicateType(uint id)
            : base(id)
        {
        }
        #endregion Constructor: PredicateType(uint id)

        #region Constructor: PredicateType(string name)
        /// <summary>
        /// Creates a new predicate type with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the predicate type.</param>
        public PredicateType(string name)
            : base(name)
        {
        }
        #endregion Constructor: PredicateType(string name)

        #region Constructor: PredicateType(PredicateType predicateType)
        /// <summary>
        /// Clones a predicate type.
        /// </summary>
        /// <param name="predicateType">The predicate type to clone.</param>
        public PredicateType(PredicateType predicateType)
            : base(predicateType)
        {
            if (predicateType != null)
            {
                Database.Current.StartChange();

                foreach (Predicate predicate in predicateType.PersonalPredicates)
                    AddPredicate(new Predicate(predicate));

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: PredicateType(PredicateType predicateType)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddPredicate(Predicate predicate)
        /// <summary>
        /// Add a predicate to the predicate type.
        /// </summary>
        /// <param name="predicate">The predicate to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        internal Message AddPredicate(Predicate predicate)
        {
            if (predicate != null)
            {
                NotifyPropertyChanged("PersonalPredicates");
                NotifyPropertyChanged("Predicates");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddPredicate(Predicate predicate)

        #region Method: RemovePredicate(Predicate predicate)
        /// <summary>
        /// Remove a predicate from the predicate type.
        /// </summary>
        /// <param name="predicate">The predicate to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        internal Message RemovePredicate(Predicate predicate)
        {
            if (predicate != null)
            {
                NotifyPropertyChanged("PersonalPredicates");
                NotifyPropertyChanged("Predicates");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: RemovePredicate(Predicate predicate)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasPredicate(Predicate predicate)
        /// <summary>
        /// Checks whether the predicate type has the given predicate.
        /// </summary>
        /// <param name="predicate">The predicate to check.</param>
        /// <returns>Returns true when the predicate type has the predicate.</returns>
        public bool HasPredicate(Predicate predicate)
        {
            if (predicate != null)
            {
                foreach (Predicate myPredicate in this.Predicates)
                {
                    if (predicate.Equals(myPredicate))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasPredicate(Predicate predicate)

        #region Method: Remove()
        /// <summary>
        /// Remove the predicate type.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();
            Database.Current.StartRemove();

            // Remove the predicates
            foreach (Predicate predicate in this.PersonalPredicates)
                predicate.Remove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(PredicateType other)
        /// <summary>
        /// Compares the predicate type to the other predicate type.
        /// </summary>
        /// <param name="other">The predicate type to compare to this predicate type.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(PredicateType other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(PredicateType other)

        #endregion Method Group: Other

    }
    #endregion Class: PredicateType

}
