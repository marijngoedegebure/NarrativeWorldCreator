/**************************************************************************
 * 
 * Mixture.cs
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

    #region Class: Mixture
    /// <summary>
    /// An aggregate of two or more substances that are not chemically united and that exist in no fixed proportion to each other.
    /// </summary>
    public class Mixture : Matter, IComparable<Mixture>
    {

        #region Properties and Fields

        #region Property: MixtureType
        /// <summary>
        /// Gets or sets the mixture type.
        /// </summary>
        public MixtureType MixtureType
        {
            get
            {
                return Database.Current.Select<MixtureType>(this.ID, GenericTables.Mixture, Columns.Type);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Mixture, Columns.Type, value);
                NotifyPropertyChanged("MixtureType");
            }
        }
        #endregion Property: MixtureType

        #region Property: Composition
        /// <summary>
        /// Gets or sets the composition.
        /// </summary>
        public Composition Composition
        {
            get
            {
                return Database.Current.Select<Composition>(this.ID, GenericTables.Mixture, Columns.Composition);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.Mixture, Columns.Composition, value);
                NotifyPropertyChanged("Composition");
            }
        }
        #endregion Property: MixtureType

        #region Property: Mixtures
        /// <summary>
        /// Gets all mixtures that are part of this mixture.
        /// </summary>
        public ReadOnlyCollection<MixtureValued> Mixtures
        {
            get
            {
                List<MixtureValued> mixtures = new List<MixtureValued>();
                mixtures.AddRange(this.PersonalMixtures);
                mixtures.AddRange(this.InheritedMixtures);
                mixtures.AddRange(this.OverriddenMixtures);
                return mixtures.AsReadOnly();
            }
        }
        #endregion Property: Mixtures		

        #region Property: PersonalMixtures
        /// <summary>
        /// Gets the personal mixtures that are part of this mixture.
        /// </summary>
        public ReadOnlyCollection<MixtureValued> PersonalMixtures
        {
            get
            {
                return Database.Current.SelectAll<MixtureValued>(this.ID, GenericTables.MixtureMixture, Columns.MixtureValued).AsReadOnly();
            }
        }
        #endregion Property: PersonalMixtures

        #region Property: InheritedMixtures
        /// <summary>
        /// Gets the inherited mixtures that are part of this mixture.
        /// </summary>
        public ReadOnlyCollection<MixtureValued> InheritedMixtures
        {
            get
            {
                List<MixtureValued> inheritedMixtures = new List<MixtureValued>();
                foreach (Node parent in this.PersonalParents)
                {
                    foreach (MixtureValued inheritedMixture in ((Mixture)parent).Mixtures)
                    {
                        if (!HasOverriddenMixture(inheritedMixture.Mixture))
                            inheritedMixtures.Add(inheritedMixture);
                    }
                }
                return inheritedMixtures.AsReadOnly();
            }
        }
        #endregion Property: InheritedMixtures

        #region Property: OverriddenMixtures
        /// <summary>
        /// Gets the overridden mixtures.
        /// </summary>
        public ReadOnlyCollection<MixtureValued> OverriddenMixtures
        {
            get
            {
                return Database.Current.SelectAll<MixtureValued>(this.ID, GenericTables.MixtureOverriddenMixture, Columns.MixtureValued).AsReadOnly();
            }
        }
        #endregion Property: OverriddenMixtures

        #region Property: Substances
        /// <summary>
        /// Gets all substances of this mixture.
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
        /// Gets the personal substances of this mixture.
        /// </summary>
        public ReadOnlyCollection<SubstanceValued> PersonalSubstances
        {
            get
            {
                return Database.Current.SelectAll<SubstanceValued>(this.ID, GenericTables.MixtureSubstance, Columns.SubstanceValued).AsReadOnly();
            }
        }
        #endregion Property: PersonalSubstances

        #region Property: InheritedSubstances
        /// <summary>
        /// Gets the inherited substances of this mixture.
        /// </summary>
        public ReadOnlyCollection<SubstanceValued> InheritedSubstances
        {
            get
            {
                List<SubstanceValued> inheritedSubstances = new List<SubstanceValued>();
                foreach (MixtureValued mixtureValued in this.PersonalMixtures)
                    inheritedSubstances.AddRange(mixtureValued.Mixture.Substances);
                foreach (Node parent in this.PersonalParents)
                {
                    foreach (SubstanceValued inheritedSubstance in ((Mixture)parent).Substances)
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
                return Database.Current.SelectAll<SubstanceValued>(this.ID, GenericTables.MixtureOverriddenSubstance, Columns.SubstanceValued).AsReadOnly();
            }
        }
        #endregion Property: OverriddenSubstances

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: Mixture()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static Mixture()
        {
            // Mixtures
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.MixtureValued, new Tuple<Type, EntryType>(typeof(MixtureValued), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.MixtureMixture, typeof(Mixture), dict);

            // Overridden mixtures
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.MixtureValued, new Tuple<Type, EntryType>(typeof(MixtureValued), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.MixtureOverriddenMixture, typeof(Mixture), dict);

            // Substances
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.SubstanceValued, new Tuple<Type, EntryType>(typeof(SubstanceValued), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.MixtureSubstance, typeof(Mixture), dict);

            // Overridden substances
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.SubstanceValued, new Tuple<Type, EntryType>(typeof(SubstanceValued), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.MixtureOverriddenSubstance, typeof(Mixture), dict);
        }
        #endregion Static Constructor: Mixture()

        #region Constructor: Mixture()
        /// <summary>
        /// Creates a new mixture.
        /// </summary>
        public Mixture()
            : base()
        {
            Database.Current.StartChange();
            Database.Current.QueryBegin();

            this.Composition = default(Composition);
            this.MixtureType = default(MixtureType);
            
            Database.Current.QueryCommit();
            Database.Current.StopChange();
        }
        #endregion Constructor: Mixture()

        #region Constructor: Mixture(uint id)
        /// <summary>
        /// Creates a new mixture from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the new mixture from.</param>
        protected Mixture(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Mixture(uint id)

        #region Constructor: Mixture(string name)
        /// <summary>
        /// Creates a new mixture with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the mixture.</param>
        public Mixture(string name)
            : base(name)
        {
        }
        #endregion Constructor: Mixture(string name)

        #region Constructor: Mixture(Mixture mixture)
        /// <summary>
        /// Clones a mixture.
        /// </summary>
        /// <param name="mixture">The mixture to clone.</param>
        public Mixture(Mixture mixture)
            : base(mixture)
        {
            if (mixture != null)
            {
                Database.Current.StartChange();

                this.Composition = mixture.Composition;
                this.MixtureType = mixture.MixtureType;
                foreach (MixtureValued mixtureValued in mixture.PersonalMixtures)
                    AddMixture(new MixtureValued(mixtureValued));
                foreach (MixtureValued mixtureValued in mixture.OverriddenMixtures)
                    AddOverriddenMixture(new MixtureValued(mixtureValued));
                foreach (SubstanceValued substanceValued in mixture.PersonalSubstances)
                    AddSubstance(new SubstanceValued(substanceValued));
                foreach (SubstanceValued substanceValued in mixture.OverriddenSubstances)
                    AddOverriddenSubstance(new SubstanceValued(substanceValued));

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Mixture(Mixture mixture)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddMixture(MixtureValued mixtureValued)
        /// <summary>
        /// Adds a valued mixture.
        /// </summary>
        /// <param name="mixtureValued">The valued mixture to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddMixture(MixtureValued mixtureValued)
        {
            if (mixtureValued != null && mixtureValued.Mixture != null && !this.Equals(mixtureValued.Mixture))
            {
                // If this valued mixture is already available, there is no use to add it
                if (HasMixture(mixtureValued.Mixture))
                    return Message.RelationExistsAlready;

                // Add the valued mixture
                Database.Current.Insert(this.ID, GenericTables.MixtureMixture, new string[] { Columns.Mixture, Columns.MixtureValued }, new object[] { mixtureValued.Mixture, mixtureValued });
                NotifyPropertyChanged("PersonalMixtures");
                NotifyPropertyChanged("Mixtures");
                NotifyPropertyChanged("InheritedSubstances");
                NotifyPropertyChanged("Substances");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddMixture(MixtureValued mixtureValued)

        #region Method: RemoveMixture(MixtureValued mixtureValued)
        /// <summary>
        /// Removes a valued mixture.
        /// </summary>
        /// <param name="mixtureValued">The valued mixture to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveMixture(MixtureValued mixtureValued)
        {
            if (mixtureValued != null)
            {
                if (this.Mixtures.Contains(mixtureValued))
                {
                    // Remove the valued mixture
                    Database.Current.Remove(this.ID, GenericTables.MixtureMixture, Columns.MixtureValued, mixtureValued);
                    NotifyPropertyChanged("PersonalMixtures");
                    NotifyPropertyChanged("Mixtures");
                    NotifyPropertyChanged("InheritedSubstances");
                    NotifyPropertyChanged("Substances");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveMixture(MixtureValued mixtureValued)

        #region Method: OverrideMixture(MixtureValued inheritedMixture)
        /// <summary>
        /// Override the given inherited mixture.
        /// </summary>
        /// <param name="inheritedMixture">The inherited mixture that should be overridden.</param>
        /// <returns>Returns whether the override has been successful.</returns>
        public Message OverrideMixture(MixtureValued inheritedMixture)
        {
            if (inheritedMixture != null && inheritedMixture.Mixture != null && this.InheritedMixtures.Contains(inheritedMixture))
            {
                // If the mixture is already available, there is no use to add it
                foreach (MixtureValued personalMixture in this.PersonalMixtures)
                {
                    if (inheritedMixture.Mixture.Equals(personalMixture.Mixture))
                        return Message.RelationExistsAlready;
                }
                if (HasOverriddenMixture(inheritedMixture.Mixture))
                    return Message.RelationExistsAlready;

                // Copy the valued mixture and add it
                return AddOverriddenMixture(new MixtureValued(inheritedMixture));
            }
            return Message.RelationFail;
        }
        #endregion Method: OverrideMixture(MixtureValued inheritedMixture)

        #region Method: AddOverriddenMixture(MixtureValued inheritedMixture)
        /// <summary>
        /// Add the given overridden mixture.
        /// </summary>
        /// <param name="overriddenMixture">The overridden mixture to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        private Message AddOverriddenMixture(MixtureValued overriddenMixture)
        {
            if (overriddenMixture != null)
            {
                Database.Current.Insert(this.ID, GenericTables.MixtureOverriddenMixture, Columns.MixtureValued, overriddenMixture);
                NotifyPropertyChanged("OverriddenMixtures");
                NotifyPropertyChanged("InheritedMixtures");
                NotifyPropertyChanged("Mixtures");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddOverriddenMixture(MixtureValued inheritedMixture)

        #region Method: RemoveOverriddenMixture(MixtureValued overriddenMixture)
        /// <summary>
        /// Removes an overridden mixture.
        /// </summary>
        /// <param name="overriddenMixture">The overridden mixture to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveOverriddenMixture(MixtureValued overriddenMixture)
        {
            if (overriddenMixture != null)
            {
                if (this.OverriddenMixtures.Contains(overriddenMixture))
                {
                    // Remove the overridden mixture
                    Database.Current.Remove(this.ID, GenericTables.MixtureOverriddenMixture, Columns.MixtureValued, overriddenMixture);
                    NotifyPropertyChanged("OverriddenMixtures");
                    NotifyPropertyChanged("InheritedMixtures");
                    NotifyPropertyChanged("Mixtures");
                    overriddenMixture.Remove();

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveOverriddenMixture(MixtureValued overriddenMixture)

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
                Database.Current.Insert(this.ID, GenericTables.MixtureSubstance, new string[] { Columns.Substance, Columns.SubstanceValued }, new object[] { substanceValued.Substance, substanceValued });
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
                    Database.Current.Remove(this.ID, GenericTables.MixtureSubstance, Columns.SubstanceValued, substanceValued);
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
                Database.Current.Insert(this.ID, GenericTables.MixtureOverriddenSubstance, Columns.SubstanceValued, overriddenSubstance);
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
                    Database.Current.Remove(this.ID, GenericTables.MixtureOverriddenSubstance, Columns.SubstanceValued, overriddenSubstance);
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

        #region Method: HasMixture(Mixture mixture)
        /// <summary>
        /// Checks if this mixture has a valued mixture of the given mixture.
        /// </summary>
        /// <param name="mixture">The mixture to check.</param>
        /// <returns>Returns true when this mixture has a valued mixture of the given mixture.</returns>
        public bool HasMixture(Mixture mixture)
        {
            if (mixture != null)
            {
                foreach (MixtureValued mixtureValued in this.Mixtures)
                {
                    if (mixture.Equals(mixtureValued.Mixture))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasMixture(Mixture mixture)

        #region Method: HasOverriddenMixture(Mixture mixture)
        /// <summary>
        /// Checks if this mixture has an overridden mixture of the given mixture.
        /// </summary>
        /// <param name="mixture">The mixture to check.</param>
        /// <returns>Returns true when this mixture has an overridden mixture of the given mixture.</returns>
        private bool HasOverriddenMixture(Mixture mixture)
        {
            if (mixture != null)
            {
                foreach (MixtureValued mixtureValued in this.OverriddenMixtures)
                {
                    if (mixture.Equals(mixtureValued.Mixture))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasOverriddenMixture(Mixture mixture)

        #region Method: HasSubstance(Substance substance)
        /// <summary>
        /// Checks if this mixture has a valued substance of the given substance.
        /// </summary>
        /// <param name="substance">The substance to check.</param>
        /// <returns>Returns true when the mixture has a valued substance of the given substance.</returns>
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
        /// Checks if this mixture has an overridden substance of the given substance.
        /// </summary>
        /// <param name="substance">The substance to check.</param>
        /// <returns>Returns true when the mixture has an overridden substance of the given substance.</returns>
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
        /// Remove the mixture.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();
            Database.Current.StartRemove();

            // Remove the mixtures
            foreach (MixtureValued mixtureValued in this.PersonalMixtures)
                mixtureValued.Remove();
            Database.Current.Remove(this.ID, GenericTables.MixtureMixture);

            // Remove the overridden mixtures
            foreach (MixtureValued mixtureValued in this.OverriddenMixtures)
                mixtureValued.Remove();
            Database.Current.Remove(this.ID, GenericTables.MixtureOverriddenMixture);

            // Remove the substances
            foreach (SubstanceValued substanceValued in this.PersonalSubstances)
                substanceValued.Remove();
            Database.Current.Remove(this.ID, GenericTables.MixtureSubstance);

            // Remove the overridden substances
            foreach (SubstanceValued substanceValued in this.OverriddenSubstances)
                substanceValued.Remove();
            Database.Current.Remove(this.ID, GenericTables.MixtureOverriddenSubstance);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(Mixture other)
        /// <summary>
        /// Compares the mixture to the other mixture.
        /// </summary>
        /// <param name="other">The mixture to compare to this mixture.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(Mixture other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(Mixture other)

        #endregion Method Group: Other

    }
    #endregion Class: Mixture

    #region Class: MixtureValued
    /// <summary>
    /// A valued version of a mixture.
    /// </summary>
    public class MixtureValued : MatterValued
    {

        #region Properties and Fields

        #region Property: Mixture
        /// <summary>
        /// Gets the mixture of which this is a valued mixture.
        /// </summary>
        public Mixture Mixture
        {
            get
            {
                return this.Node as Mixture;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: Mixture

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: MixtureValued(uint id)
        /// <summary>
        /// Creates a new valued mixture from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the valued mixture from.</param>
        protected MixtureValued(uint id)
            : base(id)
        {
        }
        #endregion Constructor: MixtureValued(uint id)

        #region Constructor: MixtureValued(MixtureValued mixtureValued)
        /// <summary>
        /// Clones a valued mixture.
        /// </summary>
        /// <param name="mixtureValued">The valued mixture to clone.</param>
        public MixtureValued(MixtureValued mixtureValued)
            : base(mixtureValued)
        {
        }
        #endregion Constructor: MixtureValued(MixtureValued mixtureValued)

        #region Constructor: MixtureValued(Mixture mixture)
        /// <summary>
        /// Creates a new valued mixture from the given mixture.
        /// </summary>
        /// <param name="mixture">The mixture to create the valued mixture from.</param>
        public MixtureValued(Mixture mixture)
            : base(mixture)
        {
        }
        #endregion Constructor: MixtureValued(Mixture mixture)

        #region Constructor: MixtureValued(Mixture mixture, NumericalValueRange quantity)
        /// <summary>
        /// Creates a new valued mixture from the given mixture in the given quantity.
        /// </summary>
        /// <param name="mixture">The mixture to create the valued mixture from.</param>
        /// <param name="quantity">The quantity of the valued mixture.</param>
        public MixtureValued(Mixture mixture, NumericalValueRange quantity)
            : base(mixture, quantity)
        {
        }
        #endregion Constructor: MixtureValued(Mixture mixture, NumericalValueRange quantity)

        #endregion Method Group: Constructors

    }
    #endregion Class: MixtureValued

    #region Class: MixtureCondition
    /// <summary>
    /// A condition on a mixture.
    /// </summary>
    public class MixtureCondition : MatterCondition
    {

        #region Properties and Fields

        #region Property: Mixture
        /// <summary>
        /// Gets or sets the required mixture.
        /// </summary>
        public Mixture Mixture
        {
            get
            {
                return this.Node as Mixture;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Mixture

        #region Property: MixtureType
        /// <summary>
        /// Gets or sets the required mixture type.
        /// </summary>
        public MixtureType? MixtureType
        {
            get
            {
                return Database.Current.Select<MixtureType?>(this.ID, ValueTables.MixtureCondition, Columns.Type);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.MixtureCondition, Columns.Type, value);
                NotifyPropertyChanged("MixtureType");
            }
        }
        #endregion Property: MixtureType

        #region Property: MixtureTypeSign
        /// <summary>
        /// Gets or sets the equality sign of the type in the condition.
        /// </summary>
        public EqualitySign? MixtureTypeSign
        {
            get
            {
                return Database.Current.Select<EqualitySign?>(this.ID, ValueTables.MixtureCondition, Columns.TypeSign);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.MixtureCondition, Columns.TypeSign, value);
                NotifyPropertyChanged("MixtureTypeSign");
            }
        }
        #endregion Property: MixtureTypeSign

        #region Property: Composition
        /// <summary>
        /// Gets or sets the required composition.
        /// </summary>
        public Composition? Composition
        {
            get
            {
                return Database.Current.Select<Composition?>(this.ID, ValueTables.MixtureCondition, Columns.Composition);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.MixtureCondition, Columns.Composition, value);
                NotifyPropertyChanged("Composition");
            }
        }
        #endregion Property: MixtureType

        #region Property: CompositionSign
        /// <summary>
        /// Gets or sets the equality sign of the composition in the condition.
        /// </summary>
        public EqualitySign? CompositionSign
        {
            get
            {
                return Database.Current.Select<EqualitySign?>(this.ID, ValueTables.MixtureCondition, Columns.CompositionSign);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.MixtureCondition, Columns.CompositionSign, value);
                NotifyPropertyChanged("CompositionSign");
            }
        }
        #endregion Property: CompositionSign

        #region Property: HasAllMandatorySubstances
        /// <summary>
        /// Gets or sets the value that indicates whether all mandatory substances are required.
        /// </summary>
        public bool? HasAllMandatorySubstances
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, ValueTables.MixtureCondition, Columns.HasAllMandatorySubstances);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.MixtureCondition, Columns.HasAllMandatorySubstances, value);
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
                return Database.Current.SelectAll<SubstanceCondition>(this.ID, ValueTables.MixtureConditionSubstanceCondition, Columns.SubstanceCondition).AsReadOnly();
            }
        }
        #endregion Property: Substances

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: MixtureCondition()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static MixtureCondition()
        {
            // Substances
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.SubstanceCondition, new Tuple<Type, EntryType>(typeof(SubstanceCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.MixtureConditionSubstanceCondition, typeof(MixtureCondition), dict);
        }
        #endregion Static Constructor: MixtureCondition()

        #region Constructor: MixtureCondition()
        /// <summary>
        /// Creates a new mixture condition.
        /// </summary>
        public MixtureCondition()
            : base()
        {
            Database.Current.StartChange();

            this.MixtureTypeSign = EqualitySign.Equal;
            this.CompositionSign = EqualitySign.Equal;

            Database.Current.StopChange();
        }
        #endregion Constructor: MixtureCondition()

        #region Constructor: MixtureCondition(uint id)
        /// <summary>
        /// Creates a new mixture condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the mixture condition from.</param>
        protected MixtureCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: MixtureCondition(uint id)

        #region Constructor: MixtureCondition(MixtureCondition mixtureCondition)
        /// <summary>
        /// Clones a mixture condition.
        /// </summary>
        /// <param name="mixtureCondition">The mixture condition to clone.</param>
        public MixtureCondition(MixtureCondition mixtureCondition)
            : base(mixtureCondition)
        {
            if (mixtureCondition != null)
            {
                Database.Current.StartChange();

                this.MixtureType = mixtureCondition.MixtureType;
                this.MixtureTypeSign = mixtureCondition.MixtureTypeSign;
                this.Composition = mixtureCondition.Composition;
                this.CompositionSign = mixtureCondition.CompositionSign;
                this.HasAllMandatorySubstances = mixtureCondition.HasAllMandatorySubstances;
                foreach (SubstanceCondition substanceCondition in mixtureCondition.Substances)
                    AddSubstance(new SubstanceCondition(substanceCondition));

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: MixtureCondition(MixtureCondition mixtureCondition)

        #region Constructor: MixtureCondition(Mixture mixture)
        /// <summary>
        /// Creates a condition for the given mixture.
        /// </summary>
        /// <param name="mixture">The mixture to create a condition for.</param>
        public MixtureCondition(Mixture mixture)
            : base(mixture)
        {
        }
        #endregion Constructor: MixtureCondition(Mixture mixture)

        #region Constructor: MixtureCondition(Mixture mixture, NumericalValueCondition quantity)
        /// <summary>
        /// Creates a condition for the given mixture in the given quantity.
        /// </summary>
        /// <param name="mixture">The mixture to create a condition for.</param>
        /// <param name="quantity">The quantity of the mixture condition.</param>
        public MixtureCondition(Mixture mixture, NumericalValueCondition quantity)
            : base(mixture, quantity)
        {
        }
        #endregion Constructor: MixtureCondition(Mixture mixture, NumericalValueCondition quantity)

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
                Database.Current.Insert(this.ID, ValueTables.MixtureConditionSubstanceCondition, Columns.SubstanceCondition, substanceCondition);
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
                    Database.Current.Remove(this.ID, ValueTables.MixtureConditionSubstanceCondition, Columns.SubstanceCondition, substanceCondition);
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
        /// Checks if this mixture condition has a substance condition of the given substance.
        /// </summary>
        /// <param name="substance">The substance to check.</param>
        /// <returns>Returns true when the mixture condition has a substance condition of the given substance.</returns>
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
        /// Remove the mixture condition.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the substances
            foreach (SubstanceCondition substanceCondition in this.Substances)
                substanceCondition.Remove();
            Database.Current.Remove(this.ID, ValueTables.MixtureConditionSubstanceCondition);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: MixtureCondition

    #region Class: MixtureChange
    /// <summary>
    /// A change on a mixture.
    /// </summary>
    public class MixtureChange : MatterChange
    {

        #region Properties and Fields

        #region Property: Mixture
        /// <summary>
        /// Gets or sets the affected mixture.
        /// </summary>
        public Mixture Mixture
        {
            get
            {
                return this.Node as Mixture;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: Mixture

        #region Property: Composition
        /// <summary>
        /// Gets or sets the composition to change to.
        /// </summary>
        public Composition? Composition
        {
            get
            {
                return Database.Current.Select<Composition?>(this.ID, ValueTables.MixtureChange, Columns.Composition);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.MixtureChange, Columns.Composition, value);
                NotifyPropertyChanged("Composition");
            }
        }
        #endregion Property: MixtureType

        #region Property: MixtureType
        /// <summary>
        /// Gets or sets the mixture type to change to.
        /// </summary>
        public MixtureType? MixtureType
        {
            get
            {
                return Database.Current.Select<MixtureType?>(this.ID, ValueTables.MixtureChange, Columns.Type);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.MixtureChange, Columns.Type, value);
                NotifyPropertyChanged("MixtureType");
            }
        }
        #endregion Property: MixtureType

        #region Property: MixturesToAdd
        /// <summary>
        /// Gets the mixtures that should be added during the change.
        /// </summary>
        public ReadOnlyCollection<MixtureValued> MixturesToAdd
        {
            get
            {
                return Database.Current.SelectAll<MixtureValued>(this.ID, ValueTables.MixtureChangeMixtureToAdd, Columns.MixtureValued).AsReadOnly();
            }
        }
        #endregion Property: MixturesToAdd

        #region Property: Substances
        /// <summary>
        /// Gets the substances to change.
        /// </summary>
        public ReadOnlyCollection<SubstanceChange> Substances
        {
            get
            {
                return Database.Current.SelectAll<SubstanceChange>(this.ID, ValueTables.MixtureChangeSubstanceChange, Columns.SubstanceChange).AsReadOnly();
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
                return Database.Current.SelectAll<SubstanceValued>(this.ID, ValueTables.MixtureChangeSubstanceToAdd, Columns.SubstanceValued).AsReadOnly();
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
                return Database.Current.SelectAll<SubstanceCondition>(this.ID, ValueTables.MixtureChangeSubstanceToRemove, Columns.SubstanceCondition).AsReadOnly();
            }
        }
        #endregion Property: SubstancesToRemove

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: MixtureChange()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static MixtureChange()
        {
            // Mixtures
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.MixtureValued, new Tuple<Type, EntryType>(typeof(MixtureValued), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.MixtureChangeMixtureToAdd, typeof(MixtureChange), dict);

            // Substances
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.SubstanceChange, new Tuple<Type, EntryType>(typeof(SubstanceChange), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.MixtureChangeSubstanceChange, typeof(MixtureChange), dict);

            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.SubstanceValued, new Tuple<Type, EntryType>(typeof(SubstanceValued), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.MixtureChangeSubstanceToAdd, typeof(MixtureChange), dict);

            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.SubstanceCondition, new Tuple<Type, EntryType>(typeof(SubstanceCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.MixtureChangeSubstanceToRemove, typeof(MixtureChange), dict);
        }
        #endregion Static Constructor: MixtureChange()

        #region Constructor: MixtureChange()
        /// <summary>
        /// Creates a new mixture change.
        /// </summary>
        public MixtureChange()
            : base()
        {
        }
        #endregion Constructor: MixtureChange()

        #region Constructor: MixtureChange(uint id)
        /// <summary>
        /// Creates a new mixture change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a mixture change from.</param>
        protected MixtureChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: MixtureChange(uint id)

        #region Constructor: MixtureChange(MixtureChange mixtureChange)
        /// <summary>
        /// Clones a mixture change.
        /// </summary>
        /// <param name="mixtureChange">The mixture change to clone.</param>
        public MixtureChange(MixtureChange mixtureChange)
            : base(mixtureChange)
        {
            if (mixtureChange != null)
            {
                Database.Current.StartChange();

                this.Composition = mixtureChange.Composition;
                this.MixtureType = mixtureChange.MixtureType;
                foreach (MixtureValued mixtureValued in mixtureChange.MixturesToAdd)
                    AddMixtureToAdd(new MixtureValued(mixtureValued));
                foreach (SubstanceChange substanceChange in mixtureChange.Substances)
                    AddSubstance(new SubstanceChange(substanceChange));
                foreach (SubstanceValued substanceValued in mixtureChange.SubstancesToAdd)
                    AddSubstanceToAdd(new SubstanceValued(substanceValued));
                foreach (SubstanceCondition substanceCondition in mixtureChange.SubstancesToRemove)
                    AddSubstanceToRemove(new SubstanceCondition(substanceCondition));

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: MixtureChange(MixtureChange mixtureChange)

        #region Constructor: MixtureChange(Mixture mixture)
        /// <summary>
        /// Creates a change for the given mixture.
        /// </summary>
        /// <param name="mixture">The mixture to create a change for.</param>
        public MixtureChange(Mixture mixture)
            : base(mixture)
        {
        }
        #endregion Constructor: MixtureChange(Mixture mixture)

        #region Constructor: MixtureChange(Mixture mixture, NumericalValueChange quantity)
        /// <summary>
        /// Creates a change for the given mixture in the form of the given quantity.
        /// </summary>
        /// <param name="mixture">The mixture to create a change for.</param>
        /// <param name="quantity">The change in quantity.</param>
        public MixtureChange(Mixture mixture, NumericalValueChange quantity)
            : base(mixture, quantity)
        {
        }
        #endregion Constructor: MixtureChange(Mixture mixture, NumericalValueChange quantity)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddMixtureToAdd(MixtureValued mixtureValued)
        /// <summary>
        /// Adds a valued mixture to the list of mixtures to add.
        /// </summary>
        /// <param name="mixtureValued">The valued mixture to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddMixtureToAdd(MixtureValued mixtureValued)
        {
            if (mixtureValued != null && mixtureValued.Mixture != null)
            {
                // If the valued mixture is already available in all mixtures, there is no use to add it
                if (HasMixtureToAdd(mixtureValued.Mixture))
                    return Message.RelationExistsAlready;

                // Add the valued mixture
                Database.Current.Insert(this.ID, ValueTables.MixtureChangeMixtureToAdd, Columns.MixtureValued, mixtureValued);
                NotifyPropertyChanged("MixturesToAdd");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddMixtureToAdd(MixtureValued mixtureValued)

        #region Method: RemoveMixtureToAdd(MixtureValued mixtureValued)
        /// <summary>
        /// Removes a valued mixture from the list of mixtures to add.
        /// </summary>
        /// <param name="mixtureValued">The valued mixture to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveMixtureToAdd(MixtureValued mixtureValued)
        {
            if (mixtureValued != null)
            {
                if (this.MixturesToAdd.Contains(mixtureValued))
                {
                    // Remove the valued mixture
                    Database.Current.Remove(this.ID, ValueTables.MixtureChangeMixtureToAdd, Columns.MixtureValued, mixtureValued);
                    NotifyPropertyChanged("MixturesToAdd");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveMixtureToAdd(MixtureValued mixtureValued)

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
                Database.Current.Insert(this.ID, ValueTables.MixtureChangeSubstanceChange, Columns.SubstanceChange, substanceChange);
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
                    Database.Current.Remove(this.ID, ValueTables.MixtureChangeSubstanceChange, Columns.SubstanceChange, substanceChange);
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
                Database.Current.Insert(this.ID, ValueTables.MixtureChangeSubstanceToAdd, Columns.SubstanceValued, substanceValued);
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
                    Database.Current.Remove(this.ID, ValueTables.MixtureChangeSubstanceToAdd, Columns.SubstanceValued, substanceValued);
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
                Database.Current.Insert(this.ID, ValueTables.MixtureChangeSubstanceToRemove, Columns.SubstanceCondition, substanceCondition);
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
                    Database.Current.Remove(this.ID, ValueTables.MixtureChangeSubstanceToRemove, Columns.SubstanceCondition, substanceCondition);
                    NotifyPropertyChanged("SubstancesToRemove");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveSubstanceToRemove(SubstanceCondition substanceCondition)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasMixtureToAdd(Mixture mixture)
        /// <summary>
        /// Checks whether the mixture change has the given mixture to add.
        /// </summary>
        /// <param name="mixture">The mixture that should be checked.</param>
        /// <returns>Returns true when the mixture change has the given mixture to add.</returns>
        public bool HasMixtureToAdd(Mixture mixture)
        {
            if (mixture != null)
            {
                foreach (MixtureValued mixtureValued in this.MixturesToAdd)
                {
                    if (mixture.Equals(mixtureValued.Mixture))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasMixtureToAdd(Mixture mixture)

        #region Method: HasSubstance(Substance substance)
        /// <summary>
        /// Checks whether the mixture change has the given substance.
        /// </summary>
        /// <param name="substance">The substance that should be checked.</param>
        /// <returns>Returns true when the mixture change has the given substance.</returns>
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
        /// Checks whether the mixture change has the given substance to add.
        /// </summary>
        /// <param name="substance">The substance that should be checked.</param>
        /// <returns>Returns true when the mixture change has the given substance to add.</returns>
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
        /// Checks whether the mixture change has the given substance to remove.
        /// </summary>
        /// <param name="substance">The substance that should be checked.</param>
        /// <returns>Returns true when the mixture change has the given substance to remove.</returns>
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
        /// Remove the mixture change.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the mixtures
            foreach (MixtureValued mixtureValued in this.MixturesToAdd)
                mixtureValued.Remove();
            Database.Current.Remove(this.ID, ValueTables.MixtureChangeMixtureToAdd);

            // Remove the substances
            foreach (SubstanceChange substance in this.Substances)
                substance.Remove();
            Database.Current.Remove(this.ID, ValueTables.MixtureChangeSubstanceChange);

            foreach (SubstanceValued substanceValued in this.SubstancesToAdd)
                substanceValued.Remove();
            Database.Current.Remove(this.ID, ValueTables.MixtureChangeSubstanceToAdd);

            foreach (SubstanceCondition substanceCondition in this.SubstancesToRemove)
                substanceCondition.Remove();
            Database.Current.Remove(this.ID, ValueTables.MixtureChangeSubstanceToRemove);

            // Remove the quantity
            if (this.Quantity != null)
                this.Quantity.Remove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: MixtureChange

}