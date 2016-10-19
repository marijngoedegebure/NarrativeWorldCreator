/**************************************************************************
 * 
 * Compound.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2010-2011
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

namespace Semantics.Entities
{

    #region Class: Compound
    /// <summary>
    /// A substance formed by chemical union of two or more elements or ingredients in definite proportion by weight.
    /// </summary>
    public class Compound : Matter, IComparable<Compound>
    {

        #region Properties and Fields

        #region Property: Substances
        /// <summary>
        /// Gets all substances of this compound.
        /// </summary>
        public ReadOnlyCollection<SubstanceValued> Substances
        {
            get
            {
                List<SubstanceValued> substances = new List<SubstanceValued>();
                substances.AddRange(this.PersonalSubstances);
                substances.AddRange(this.InheritedSubstances);
                substances.AddRange(this.OverriddenSubstances);
                return substances.AsReadOnly();
            }
        }
        #endregion Property: Substances

        #region Property: PersonalSubstances
        /// <summary>
        /// Gets the personal substances of this compound.
        /// </summary>
        public ReadOnlyCollection<SubstanceValued> PersonalSubstances
        {
            get
            {
                return Database.Current.SelectAll<SubstanceValued>(this.ID, GenericTables.CompoundSubstance, Columns.SubstanceValued).AsReadOnly();
            }
        }
        #endregion Property: PersonalSubstances

        #region Property: InheritedSubstances
        /// <summary>
        /// Gets the inherited substances of this compound.
        /// </summary>
        public ReadOnlyCollection<SubstanceValued> InheritedSubstances
        {
            get
            {
                List<SubstanceValued> inheritedSubstances = new List<SubstanceValued>();
                foreach (Node parent in this.PersonalParents)
                {
                    foreach (SubstanceValued inheritedSubstance in ((Compound)parent).Substances)
                    {
                        if (!HasOverriddenSubstance(inheritedSubstance.Substance))
                            inheritedSubstances.Add(inheritedSubstance);
                    }
                }
                return inheritedSubstances.AsReadOnly();
            }
        }
        #endregion Property: InheritedSubstances

        #region Property: OverriddenSubstances
        /// <summary>
        /// Gets the overridden substances.
        /// </summary>
        public ReadOnlyCollection<SubstanceValued> OverriddenSubstances
        {
            get
            {
                return Database.Current.SelectAll<SubstanceValued>(this.ID, GenericTables.CompoundOverriddenSubstance, Columns.SubstanceValued).AsReadOnly();
            }
        }
        #endregion Property: OverriddenSubstances

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: Compound()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static Compound()
        {
            // Substances
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.SubstanceValued, new Tuple<Type, EntryType>(typeof(SubstanceValued), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.CompoundSubstance, typeof(Compound), dict);

            // Overridden substances
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.SubstanceValued, new Tuple<Type, EntryType>(typeof(SubstanceValued), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.CompoundOverriddenSubstance, typeof(Compound), dict);
        }
        #endregion Static Constructor: Compound()

        #region Constructor: Compound()
        /// <summary>
        /// Creates a new compound.
        /// </summary>
        public Compound()
            : base()
        {
        }
        #endregion Constructor: Compound()

        #region Constructor: Compound(uint id)
        /// <summary>
        /// Creates a new compound from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the new compound from.</param>
        protected Compound(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Compound(uint id)

        #region Constructor: Compound(string name)
        /// <summary>
        /// Creates a new compound with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the compound.</param>
        public Compound(string name)
            : base(name)
        {
        }
        #endregion Constructor: Compound(string name)

        #region Constructor: Compound(Compound compound)
        /// <summary>
        /// Clones a compound.
        /// </summary>
        /// <param name="compound">The compound to clone.</param>
        public Compound(Compound compound)
            : base(compound)
        {
            if (compound != null)
            {
                Database.Current.StartChange();

                foreach (SubstanceValued substanceValued in compound.PersonalSubstances)
                    AddSubstance(new SubstanceValued(substanceValued));
                foreach (SubstanceValued substanceValued in compound.OverriddenSubstances)
                    AddOverriddenSubstance(new SubstanceValued(substanceValued));

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Compound(Compound compound)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddSubstance(SubstanceValued substanceValued)
        /// <summary>
        /// Adds a valued substance.
        /// </summary>
        /// <param name="substanceValued">The valued substance to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddSubstance(SubstanceValued substanceValued)
        {
            if (substanceValued != null && substanceValued.Substance != null)
            {
                // If this valued substance is already available, there is no use to add it
                if (HasSubstance(substanceValued.Substance))
                    return Message.RelationExistsAlready;

                // Add the valued substance
                Database.Current.Insert(this.ID, GenericTables.CompoundSubstance, new string[] { Columns.Substance, Columns.SubstanceValued }, new object[] { substanceValued.Substance, substanceValued });
                NotifyPropertyChanged("PersonalSubstances");
                NotifyPropertyChanged("Substances");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddSubstance(SubstanceValued substanceValued)

        #region Method: RemoveSubstance(SubstanceValued substanceValued)
        /// <summary>
        /// Removes a valued substance.
        /// </summary>
        /// <param name="substanceValued">The valued substance to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveSubstance(SubstanceValued substanceValued)
        {
            if (substanceValued != null)
            {
                if (this.Substances.Contains(substanceValued))
                {
                    // Remove the valued substance
                    Database.Current.Remove(this.ID, GenericTables.CompoundSubstance, Columns.SubstanceValued, substanceValued);
                    NotifyPropertyChanged("PersonalSubstances");
                    NotifyPropertyChanged("Substances");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveSubstance(SubstanceValued substanceValued)

        #region Method: OverrideSubstance(SubstanceValued inheritedSubstance)
        /// <summary>
        /// Override the given inherited substance.
        /// </summary>
        /// <param name="inheritedSubstance">The inherited substance that should be overridden.</param>
        /// <returns>Returns whether the override has been successful.</returns>
        public Message OverrideSubstance(SubstanceValued inheritedSubstance)
        {
            if (inheritedSubstance != null && inheritedSubstance.Substance != null && this.InheritedSubstances.Contains(inheritedSubstance))
            {
                // If the substance is already available, there is no use to add it
                foreach (SubstanceValued personalSubstance in this.PersonalSubstances)
                {
                    if (inheritedSubstance.Substance.Equals(personalSubstance.Substance))
                        return Message.RelationExistsAlready;
                }
                if (HasOverriddenSubstance(inheritedSubstance.Substance))
                    return Message.RelationExistsAlready;

                // Copy the valued substance and add it
                return AddOverriddenSubstance(new SubstanceValued(inheritedSubstance));
            }
            return Message.RelationFail;
        }
        #endregion Method: OverrideSubstance(SubstanceValued inheritedSubstance)

        #region Method: AddOverriddenSubstance(SubstanceValued inheritedSubstance)
        /// <summary>
        /// Add the given overridden substance.
        /// </summary>
        /// <param name="overriddenSubstance">The overridden substance to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        private Message AddOverriddenSubstance(SubstanceValued overriddenSubstance)
        {
            if (overriddenSubstance != null)
            {
                Database.Current.Insert(this.ID, GenericTables.CompoundOverriddenSubstance, Columns.SubstanceValued, overriddenSubstance);
                NotifyPropertyChanged("OverriddenSubstances");
                NotifyPropertyChanged("InheritedSubstances");
                NotifyPropertyChanged("Substances");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddOverriddenSubstance(SubstanceValued inheritedSubstance)

        #region Method: RemoveOverriddenSubstance(SubstanceValued overriddenSubstance)
        /// <summary>
        /// Removes an overridden substance.
        /// </summary>
        /// <param name="overriddenSubstance">The overridden substance to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveOverriddenSubstance(SubstanceValued overriddenSubstance)
        {
            if (overriddenSubstance != null)
            {
                if (this.OverriddenSubstances.Contains(overriddenSubstance))
                {
                    // Remove the overridden substance
                    Database.Current.Remove(this.ID, GenericTables.CompoundOverriddenSubstance, Columns.SubstanceValued, overriddenSubstance);
                    NotifyPropertyChanged("OverriddenSubstances");
                    NotifyPropertyChanged("InheritedSubstances");
                    NotifyPropertyChanged("Substances");
                    overriddenSubstance.Remove();

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveOverriddenSubstance(SubstanceValued overriddenSubstance)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasSubstance(Substance substance)
        /// <summary>
        /// Checks if this compound has a valued substance of the given substance.
        /// </summary>
        /// <param name="substance">The substance to check.</param>
        /// <returns>Returns true when the compound has a valued substance of the given substance.</returns>
        public bool HasSubstance(Substance substance)
        {
            if (substance != null)
            {
                foreach (SubstanceValued substanceValued in this.Substances)
                {
                    if (substance.Equals(substanceValued.Substance))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasSubstance(Substance substance)

        #region Method: HasOverriddenSubstance(Substance substance)
        /// <summary>
        /// Checks if this compound has a valued substance of the given overridden substance.
        /// </summary>
        /// <param name="substance">The substance to check.</param>
        /// <returns>Returns true when the compound has a valued substance of the given overridden substance.</returns>
        private bool HasOverriddenSubstance(Substance substance)
        {
            if (substance != null)
            {
                foreach (SubstanceValued substanceValued in this.OverriddenSubstances)
                {
                    if (substance.Equals(substanceValued.Substance))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasOverriddenSubstance(Substance substance)

        #region Method: Remove()
        /// <summary>
        /// Remove the compound.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();
            Database.Current.StartRemove();

            // Remove the substances
            foreach (SubstanceValued substanceValued in this.PersonalSubstances)
                substanceValued.Remove();
            Database.Current.Remove(this.ID, GenericTables.CompoundSubstance);

            // Remove the overridden substances
            foreach (SubstanceValued substanceValued in this.OverriddenSubstances)
                substanceValued.Remove();
            Database.Current.Remove(this.ID, GenericTables.CompoundOverriddenSubstance);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(Compound other)
        /// <summary>
        /// Compares the compound to the other compound.
        /// </summary>
        /// <param name="other">The compound to compare to this compound.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(Compound other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(Compound other)

        #endregion Method Group: Other

    }
    #endregion Class: Compound

    #region Class: CompoundValued
    /// <summary>
    /// A valued version of a compound.
    /// </summary>
    public class CompoundValued : MatterValued
    {

        #region Properties and Fields

        #region Property: Compound
        /// <summary>
        /// Gets the compound of which this is a valued compound.
        /// </summary>
        public Compound Compound
        {
            get
            {
                return this.Node as Compound;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: Compound

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: CompoundValued(uint id)
        /// <summary>
        /// Creates a new valued compound from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the valued compound from.</param>
        protected CompoundValued(uint id)
            : base(id)
        {
        }
        #endregion Constructor: CompoundValued(uint id)

        #region Constructor: CompoundValued(CompoundValued compoundValued)
        /// <summary>
        /// Clones a valued compound.
        /// </summary>
        /// <param name="compoundValued">The valued compound to clone.</param>
        public CompoundValued(CompoundValued compoundValued)
            : base(compoundValued)
        {
        }
        #endregion Constructor: CompoundValued(CompoundValued compoundValued)

        #region Constructor: CompoundValued(Compound compound)
        /// <summary>
        /// Creates a new valued compound from the given compound.
        /// </summary>
        /// <param name="compound">The compound to create the valued compound from.</param>
        public CompoundValued(Compound compound)
            : base(compound)
        {
        }
        #endregion Constructor: CompoundValued(Compound compound)

        #region Constructor: CompoundValued(Compound compound, NumericalValueRange quantity)
        /// <summary>
        /// Creates a new valued compound from the given compound in the given quantity.
        /// </summary>
        /// <param name="compound">The compound to create the valued compound from.</param>
        /// <param name="quantity">The quantity of the valued compound.</param>
        public CompoundValued(Compound compound, NumericalValueRange quantity)
            : base(compound, quantity)
        {
        }
        #endregion Constructor: CompoundValued(Compound compound, NumericalValueRange quantity)

        #endregion Method Group: Constructors

    }
    #endregion Class: CompoundValued

    #region Class: CompoundCondition
    /// <summary>
    /// A condition on a compound.
    /// </summary>
    public class CompoundCondition : MatterCondition
    {

        #region Properties and Fields

        #region Property: Compound
        /// <summary>
        /// Gets or sets the required compound.
        /// </summary>
        public Compound Compound
        {
            get
            {
                return this.Node as Compound;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Compound

        #region Property: HasAllMandatorySubstances
        /// <summary>
        /// Gets or sets the value that indicates whether all mandatory substances are required.
        /// </summary>
        public bool? HasAllMandatorySubstances
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, ValueTables.CompoundCondition, Columns.HasAllMandatorySubstances);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.CompoundCondition, Columns.HasAllMandatorySubstances, value);
                NotifyPropertyChanged("HasAllMandatorySubstances");
            }
        }
        #endregion Property: HasAllMandatorySubstances

        #region Property: Substances
        /// <summary>
        /// Gets the required substances.
        /// </summary>
        public ReadOnlyCollection<SubstanceCondition> Substances
        {
            get
            {
                return Database.Current.SelectAll<SubstanceCondition>(this.ID, ValueTables.CompoundConditionSubstanceCondition, Columns.SubstanceCondition).AsReadOnly();
            }
        }
        #endregion Property: Substances

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: CompoundCondition()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static CompoundCondition()
        {
            // Substances
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.SubstanceCondition, new Tuple<Type, EntryType>(typeof(SubstanceCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.CompoundConditionSubstanceCondition, typeof(CompoundCondition), dict);
        }
        #endregion Static Constructor: CompoundCondition()

        #region Constructor: CompoundCondition()
        /// <summary>
        /// Creates a new compound condition.
        /// </summary>
        public CompoundCondition()
            : base()
        {
        }
        #endregion Constructor: CompoundCondition()

        #region Constructor: CompoundCondition(uint id)
        /// <summary>
        /// Creates a new compound condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the compound condition from.</param>
        protected CompoundCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: CompoundCondition(uint id)

        #region Constructor: CompoundCondition(CompoundCondition compoundCondition)
        /// <summary>
        /// Clones a compound condition.
        /// </summary>
        /// <param name="compoundCondition">The compound condition to clone.</param>
        public CompoundCondition(CompoundCondition compoundCondition)
            : base(compoundCondition)
        {
            if (compoundCondition != null)
            {
                Database.Current.StartChange();

                this.HasAllMandatorySubstances = compoundCondition.HasAllMandatorySubstances;
                foreach (SubstanceCondition substanceCondition in compoundCondition.Substances)
                    AddSubstance(new SubstanceCondition(substanceCondition));

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: CompoundCondition(CompoundCondition compoundCondition)

        #region Constructor: CompoundCondition(Compound compound)
        /// <summary>
        /// Creates a condition for the given compound.
        /// </summary>
        /// <param name="compound">The compound to create a condition for.</param>
        public CompoundCondition(Compound compound)
            : base(compound)
        {
        }
        #endregion Constructor: CompoundCondition(Compound compound)

        #region Constructor: CompoundCondition(Compound compound, NumericalValueCondition quantity)
        /// <summary>
        /// Creates a condition for the given compound in the given quantity.
        /// </summary>
        /// <param name="compound">The compound to create a condition for.</param>
        /// <param name="quantity">The quantity of the compound condition.</param>
        public CompoundCondition(Compound compound, NumericalValueCondition quantity)
            : base(compound, quantity)
        {
        }
        #endregion Constructor: CompoundCondition(Compound compound, NumericalValueCondition quantity)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddSubstance(SubstanceCondition substanceCondition)
        /// <summary>
        /// Adds a substance condition.
        /// </summary>
        /// <param name="substanceCondition">The substance condition to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddSubstance(SubstanceCondition substanceCondition)
        {
            if (substanceCondition != null)
            {
                // If this substance condition is already available, there is no use to add it
                if (HasSubstance(substanceCondition.Substance))
                    return Message.RelationExistsAlready;

                // Add the substance condition
                Database.Current.Insert(this.ID, ValueTables.CompoundConditionSubstanceCondition, Columns.SubstanceCondition, substanceCondition);
                NotifyPropertyChanged("Substances");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddSubstance(SubstanceCondition substanceCondition)

        #region Method: RemoveSubstance(SubstanceCondition substanceCondition)
        /// <summary>
        /// Removes a substance condition.
        /// </summary>
        /// <param name="substanceCondition">The substance condition to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveSubstance(SubstanceCondition substanceCondition)
        {
            if (substanceCondition != null)
            {
                if (this.Substances.Contains(substanceCondition))
                {
                    // Remove the substance condition
                    Database.Current.Remove(this.ID, ValueTables.CompoundConditionSubstanceCondition, Columns.SubstanceCondition, substanceCondition);
                    NotifyPropertyChanged("Substances");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveSubstance(SubstanceCondition substanceCondition)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasSubstance(Substance substance)
        /// <summary>
        /// Checks if this compound condition has a substance condition of the given substance.
        /// </summary>
        /// <param name="substance">The substance to check.</param>
        /// <returns>Returns true when the compound condition has a substance condition of the given substance.</returns>
        public bool HasSubstance(Substance substance)
        {
            if (substance != null)
            {
                foreach (SubstanceCondition substanceCondition in this.Substances)
                {
                    if (substance.Equals(substanceCondition.Substance))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasSubstance(Substance substance)

        #region Method: Remove()
        /// <summary>
        /// Remove the compound condition.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the substances
            foreach (SubstanceCondition substanceCondition in this.Substances)
                substanceCondition.Remove();
            Database.Current.Remove(this.ID, ValueTables.CompoundConditionSubstanceCondition);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: CompoundCondition

    #region Class: CompoundChange
    /// <summary>
    /// A change on a compound.
    /// </summary>
    public class CompoundChange : MatterChange
    {

        #region Properties and Fields

        #region Property: Compound
        /// <summary>
        /// Gets or sets the affected compound.
        /// </summary>
        public Compound Compound
        {
            get
            {
                return this.Node as Compound;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Compound

        #region Property: Substances
        /// <summary>
        /// Gets the substances to change.
        /// </summary>
        public ReadOnlyCollection<SubstanceChange> Substances
        {
            get
            {
                return Database.Current.SelectAll<SubstanceChange>(this.ID, ValueTables.CompoundChangeSubstanceChange, Columns.SubstanceChange).AsReadOnly();
            }
        }
        #endregion Property: Substances

        #region Property: SubstancesToAdd
        /// <summary>
        /// Gets the substances that should be added during the change.
        /// </summary>
        public ReadOnlyCollection<SubstanceValued> SubstancesToAdd
        {
            get
            {
                return Database.Current.SelectAll<SubstanceValued>(this.ID, ValueTables.CompoundChangeSubstanceToAdd, Columns.SubstanceValued).AsReadOnly();
            }
        }
        #endregion Property: SubstancesToAdd

        #region Property: SubstancesToRemove
        /// <summary>
        /// Gets the substances that should be removed during the change.
        /// </summary>
        public ReadOnlyCollection<SubstanceCondition> SubstancesToRemove
        {
            get
            {
                return Database.Current.SelectAll<SubstanceCondition>(this.ID, ValueTables.CompoundChangeSubstanceToRemove, Columns.SubstanceCondition).AsReadOnly();
            }
        }
        #endregion Property: SubstancesToRemove

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: CompoundChange()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static CompoundChange()
        {
            // Substances
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.SubstanceChange, new Tuple<Type, EntryType>(typeof(SubstanceChange), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.CompoundChangeSubstanceChange, typeof(CompoundChange), dict);

            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.SubstanceValued, new Tuple<Type, EntryType>(typeof(SubstanceValued), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.CompoundChangeSubstanceToAdd, typeof(CompoundChange), dict);

            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.SubstanceCondition, new Tuple<Type, EntryType>(typeof(SubstanceCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.CompoundChangeSubstanceToRemove, typeof(CompoundChange), dict);
        }
        #endregion Static Constructor: CompoundChange()

        #region Constructor: CompoundChange()
        /// <summary>
        /// Creates a new compound change.
        /// </summary>
        public CompoundChange()
            : base()
        {
        }
        #endregion Constructor: CompoundChange()

        #region Constructor: CompoundChange(uint id)
        /// <summary>
        /// Creates a new compound change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a compound change from.</param>
        protected CompoundChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: CompoundChange(uint id)

        #region Constructor: CompoundChange(CompoundChange compoundChange)
        /// <summary>
        /// Clones a compound change.
        /// </summary>
        /// <param name="compoundChange">The compound change to clone.</param>
        public CompoundChange(CompoundChange compoundChange)
            : base(compoundChange)
        {
            if (compoundChange != null)
            {
                Database.Current.StartChange();

                foreach (SubstanceChange substanceChange in compoundChange.Substances)
                    AddSubstance(new SubstanceChange(substanceChange));
                foreach (SubstanceValued substanceValued in compoundChange.SubstancesToAdd)
                    AddSubstanceToAdd(new SubstanceValued(substanceValued));
                foreach (SubstanceCondition substanceCondition in compoundChange.SubstancesToRemove)
                    AddSubstanceToRemove(new SubstanceCondition(substanceCondition));

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: CompoundChange(CompoundChange compoundChange)

        #region Constructor: CompoundChange(Compound compound)
        /// <summary>
        /// Creates a change for the given compound.
        /// </summary>
        /// <param name="compound">The compound to create a change for.</param>
        public CompoundChange(Compound compound)
            : base(compound)
        {
        }
        #endregion Constructor: CompoundChange(Compound compound)

        #region Constructor: CompoundChange(Compound compound, NumericalValueChange quantity)
        /// <summary>
        /// Creates a change for the given compound in the form of the given quantity.
        /// </summary>
        /// <param name="compound">The compound to create a change for.</param>
        /// <param name="quantity">The change in quantity.</param>
        public CompoundChange(Compound compound, NumericalValueChange quantity)
            : base(compound, quantity)
        {
        }
        #endregion Constructor: CompoundChange(Compound compound, NumericalValueChange quantity)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddSubstance(SubstanceChange substanceChange)
        /// <summary>
        /// Adds a substance change to the list of substances.
        /// </summary>
        /// <param name="substanceChange">The substance change to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddSubstance(SubstanceChange substanceChange)
        {
            if (substanceChange != null)
            {
                // If the substance change is already available in all substances, there is no use to add it
                if (HasSubstance(substanceChange.Substance))
                    return Message.RelationExistsAlready;

                // Add the substance change
                Database.Current.Insert(this.ID, ValueTables.CompoundChangeSubstanceChange, Columns.SubstanceChange, substanceChange);
                NotifyPropertyChanged("Substances");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddSubstance(SubstanceChange substanceChange)

        #region Method: RemoveSubstance(SubstanceChange substanceChange)
        /// <summary>
        /// Removes a substance change from the list of substances.
        /// </summary>
        /// <param name="substanceChange">The substance change to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveSubstance(SubstanceChange substanceChange)
        {
            if (substanceChange != null)
            {
                if (this.Substances.Contains(substanceChange))
                {
                    // Remove the substance change
                    Database.Current.Remove(this.ID, ValueTables.CompoundChangeSubstanceChange, Columns.SubstanceChange, substanceChange);
                    NotifyPropertyChanged("Substances");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveSubstance(SubstanceChange substanceChange)

        #region Method: AddSubstanceToAdd(SubstanceValued substanceValued)
        /// <summary>
        /// Adds a valued substance to the list of substances to add.
        /// </summary>
        /// <param name="substanceValued">The valued substance to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddSubstanceToAdd(SubstanceValued substanceValued)
        {
            if (substanceValued != null && substanceValued.Substance != null)
            {
                // If the valued substance is already available in all substances, there is no use to add it
                if (HasSubstanceToAdd(substanceValued.Substance))
                    return Message.RelationExistsAlready;

                // Add the valued substance
                Database.Current.Insert(this.ID, ValueTables.CompoundChangeSubstanceToAdd, Columns.SubstanceValued, substanceValued);
                NotifyPropertyChanged("SubstancesToAdd");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddSubstanceToAdd(SubstanceValued substanceValued)

        #region Method: RemoveSubstanceToAdd(SubstanceValued substanceValued)
        /// <summary>
        /// Removes a valued substance from the list of substances to add.
        /// </summary>
        /// <param name="substanceValued">The valued substance to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveSubstanceToAdd(SubstanceValued substanceValued)
        {
            if (substanceValued != null)
            {
                if (this.SubstancesToAdd.Contains(substanceValued))
                {
                    // Remove the valued substance
                    Database.Current.Remove(this.ID, ValueTables.CompoundChangeSubstanceToAdd, Columns.SubstanceValued, substanceValued);
                    NotifyPropertyChanged("SubstancesToAdd");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveSubstanceToAdd(SubstanceValued substanceValued)

        #region Method: AddSubstanceToRemove(SubstanceCondition substanceCondition)
        /// <summary>
        /// Adds a substance condition to the list of substances to remove.
        /// </summary>
        /// <param name="substanceCondition">The substance condition to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddSubstanceToRemove(SubstanceCondition substanceCondition)
        {
            if (substanceCondition != null)
            {
                // If the substance condition is already available in all substances, there is no use to add it
                if (HasSubstanceToRemove(substanceCondition.Substance))
                    return Message.RelationExistsAlready;

                // Add the substance condition
                Database.Current.Insert(this.ID, ValueTables.CompoundChangeSubstanceToRemove, Columns.SubstanceCondition, substanceCondition);
                NotifyPropertyChanged("SubstancesToRemove");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddSubstanceToRemove(SubstanceCondition substanceCondition)

        #region Method: RemoveSubstanceToRemove(SubstanceCondition substanceCondition)
        /// <summary>
        /// Removes a substance condition from the list of substances to remove.
        /// </summary>
        /// <param name="substanceCondition">The substance condition to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveSubstanceToRemove(SubstanceCondition substanceCondition)
        {
            if (substanceCondition != null)
            {
                if (this.SubstancesToRemove.Contains(substanceCondition))
                {
                    // Remove the substance condition
                    Database.Current.Remove(this.ID, ValueTables.CompoundChangeSubstanceToRemove, Columns.SubstanceCondition, substanceCondition);
                    NotifyPropertyChanged("SubstancesToRemove");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveSubstanceToRemove(SubstanceCondition substanceCondition)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasSubstance(Substance substance)
        /// <summary>
        /// Checks whether the compound change has the given substance.
        /// </summary>
        /// <param name="substance">The substance that should be checked.</param>
        /// <returns>Returns true when the compound change has the given substance.</returns>
        public bool HasSubstance(Substance substance)
        {
            if (substance != null)
            {
                foreach (SubstanceChange substanceChange in this.Substances)
                {
                    if (substance.Equals(substanceChange.Substance))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasSubstance(Substance substance)

        #region Method: HasSubstanceToAdd(Substance substance)
        /// <summary>
        /// Checks whether the compound change has the given substance to add.
        /// </summary>
        /// <param name="substance">The substance that should be checked.</param>
        /// <returns>Returns true when the compound change has the given substance to add.</returns>
        public bool HasSubstanceToAdd(Substance substance)
        {
            if (substance != null)
            {
                foreach (SubstanceValued substanceValued in this.SubstancesToAdd)
                {
                    if (substance.Equals(substanceValued.Substance))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasSubstanceToAdd(Substance substance)

        #region Method: HasSubstanceToRemove(Substance substance)
        /// <summary>
        /// Checks whether the compound change has the given substance to remove.
        /// </summary>
        /// <param name="substance">The substance that should be checked.</param>
        /// <returns>Returns true when the compound change has the given substance to remove.</returns>
        public bool HasSubstanceToRemove(Substance substance)
        {
            if (substance != null)
            {
                foreach (SubstanceCondition substanceCondition in this.SubstancesToRemove)
                {
                    if (substance.Equals(substanceCondition.Substance))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasSubstanceToRemove(Substance substance)

        #region Method: Remove()
        /// <summary>
        /// Remove the compound change.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the substances
            foreach (SubstanceChange substance in this.Substances)
                substance.Remove();
            Database.Current.Remove(this.ID, ValueTables.CompoundChangeSubstanceChange);

            foreach (SubstanceValued substance in this.SubstancesToAdd)
                substance.Remove();
            Database.Current.Remove(this.ID, ValueTables.CompoundChangeSubstanceToAdd);

            foreach (SubstanceCondition substance in this.SubstancesToRemove)
                substance.Remove();
            Database.Current.Remove(this.ID, ValueTables.CompoundChangeSubstanceToRemove);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: CompoundChange

}