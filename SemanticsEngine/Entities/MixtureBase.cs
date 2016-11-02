/**************************************************************************
 * 
 * MixtureBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2010-2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Semantics.Entities;
using Semantics.Utilities;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Entities
{

    #region Class: MixtureBase
    /// <summary>
    /// A base of a mixture.
    /// </summary>
    public class MixtureBase : MatterBase
    {

        #region Properties and Fields

        #region Property: Mixture
        /// <summary>
        /// Gets the mixture of which this is a mixture base.
        /// </summary>
        protected internal Mixture Mixture
        {
            get
            {
                return this.IdHolder as Mixture;
            }
        }
        #endregion Property: Mixture

        #region Property: MixtureType
        /// <summary>
        /// The mixture type.
        /// </summary>
        private MixtureType mixtureType = default(MixtureType);

        /// <summary>
        /// Gets the mixture type.
        /// </summary>
        public MixtureType MixtureType
        {
            get
            {
                return mixtureType;
            }
        }
        #endregion Property: MixtureType

        #region Property: Composition
        /// <summary>
        /// The composition.
        /// </summary>
        private Composition composition = default(Composition);

        /// <summary>
        /// Gets the composition.
        /// </summary>
        public Composition Composition
        {
            get
            {
                return composition;
            }
        }
        #endregion Property: MixtureType

        #region Property: Substances
        /// <summary>
        /// All the substances that this mixture is made of.
        /// </summary>
        private SubstanceValuedBase[] substances = null;

        /// <summary>
        /// Gets all the substances that this mixture is made of.
        /// </summary>
        public ReadOnlyCollection<SubstanceValuedBase> Substances
        {
            get
            {   
                if (substances == null)
                {
                    LoadSubstances();
                    if (substances == null)
                        return new List<SubstanceValuedBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<SubstanceValuedBase>(substances);
            }
        }

        /// <summary>
        /// Loads the substances.
        /// </summary>
        private void LoadSubstances()
        {
            if (this.Mixture != null)
            {
                List<SubstanceValuedBase> substanceValuedBases = new List<SubstanceValuedBase>();
                foreach (SubstanceValued substanceValued in this.Mixture.Substances)
                    substanceValuedBases.Add(BaseManager.Current.GetBase<SubstanceValuedBase>(substanceValued));
                substances = substanceValuedBases.ToArray();
            }
        }
        #endregion Property: Substances

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: MixtureBase(Mixture mixture)
        /// <summary>
        /// Creates a new mixture base from the given mixture.
        /// </summary>
        /// <param name="mixture">The mixture to create the mixture base from.</param>
        protected internal MixtureBase(Mixture mixture)
            : base(mixture)
        {
            if (mixture != null)
            {
                this.mixtureType = mixture.MixtureType;
                this.composition = mixture.Composition;

                if (BaseManager.PreloadProperties)
                    LoadSubstances();
            }
        }
        #endregion Constructor: MixtureBase(Mixture mixture)

        #endregion Method Group: Constructors

    }
    #endregion Class: MixtureBase

    #region Class: MixtureValuedBase
    /// <summary>
    /// A base of a valued mixture.
    /// </summary>
    public class MixtureValuedBase : MatterValuedBase
    {

        #region Properties and Fields

        #region Property: MixtureValued
        /// <summary>
        /// Gets the valued mixture of which this is a valued mixture base.
        /// </summary>
        protected internal MixtureValued MixtureValued
        {
            get
            {
                return this.NodeValued as MixtureValued;
            }
        }
        #endregion Property: MixtureValued

        #region Property: MixtureBase
        /// <summary>
        /// Gets the mixture base of which this is a valued mixture base.
        /// </summary>
        public MixtureBase MixtureBase
        {
            get
            {
                return this.NodeBase as MixtureBase;
            }
        }
        #endregion Property: MixtureBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: MixtureValuedBase(MixtureValued mixtureValued)
        /// <summary>
        /// Create a valued mixture base from the given valued mixture.
        /// </summary>
        /// <param name="mixtureValued">The valued mixture to create a valued mixture base from.</param>
        protected internal MixtureValuedBase(MixtureValued mixtureValued)
            : base(mixtureValued)
        {
        }
        #endregion Constructor: MixtureValuedBase(MixtureValued mixtureValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: MixtureValuedBase

    #region Class: MixtureConditionBase
    /// <summary>
    /// A condition on a mixture.
    /// </summary>
    public class MixtureConditionBase : MatterConditionBase
    {

        #region Properties and Fields

        #region Property: MixtureCondition
        /// <summary>
        /// Gets the mixture condition of which this is a mixture condition base.
        /// </summary>
        protected internal MixtureCondition MixtureCondition
        {
            get
            {
                return this.Condition as MixtureCondition;
            }
        }
        #endregion Property: MixtureCondition

        #region Property: MixtureBase
        /// <summary>
        /// Gets the mixture base of which this is a mixture condition base.
        /// </summary>
        public MixtureBase MixtureBase
        {
            get
            {
                return this.NodeBase as MixtureBase;
            }
        }
        #endregion Property: MixtureBase

        #region Property: MixtureType
        /// <summary>
        /// The required mixture type.
        /// </summary>
        private MixtureType? mixtureType = null;

        /// <summary>
        /// Gets the required mixture type.
        /// </summary>
        public MixtureType? MixtureType
        {
            get
            {
                return mixtureType;
            }
        }
        #endregion Property: MixtureType

        #region Property: MixtureTypeSign
        /// <summary>
        /// The equality sign of the type in the condition.
        /// </summary>
        private EqualitySign? mixtureTypeSign = null;

        /// <summary>
        /// Gets the equality sign of the type in the condition.
        /// </summary>
        public EqualitySign? MixtureTypeSign
        {
            get
            {
                return mixtureTypeSign;
            }
        }
        #endregion Property: MixtureTypeSign

        #region Property: Composition
        /// <summary>
        /// The required composition.
        /// </summary>
        private Composition? composition = null;

        /// <summary>
        /// Gets the required composition.
        /// </summary>
        public Composition? Composition
        {
            get
            {
                return composition;
            }
        }
        #endregion Property: MixtureType

        #region Property: CompositionSign
        /// <summary>
        /// The equality sign of the composition in the condition.
        /// </summary>
        private EqualitySign? compositionSign = null;

        /// <summary>
        /// Gets the equality sign of the composition in the condition.
        /// </summary>
        public EqualitySign? CompositionSign
        {
            get
            {
                return compositionSign;
            }
        }
        #endregion Property: CompositionSign

        #region Property: HasAllMandatorySubstances
        /// <summary>
        /// The value that indicates whether all mandatory substances are required.
        /// </summary>
        private bool? hasAllMandatorySubstances = null;

        /// <summary>
        /// Gets the value that indicates whether all mandatory substances are required.
        /// </summary>
        public bool? HasAllMandatorySubstances
        {
            get
            {
                return hasAllMandatorySubstances;
            }
        }
        #endregion Property: HasAllMandatorySubstances

        #region Property: Substances
        /// <summary>
        /// The required substances.
        /// </summary>
        private SubstanceConditionBase[] substances = null;

        /// <summary>
        /// Gets the required substances.
        /// </summary>
        public ReadOnlyCollection<SubstanceConditionBase> Substances
        {
            get
            {
                if (substances == null)
                {
                    LoadSubstances();
                    if (substances == null)
                        return new List<SubstanceConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<SubstanceConditionBase>(substances);
            }
        }

        /// <summary>
        /// Loads the substances.
        /// </summary>
        private void LoadSubstances()
        {
            if (this.MixtureCondition != null)
            {
                List<SubstanceConditionBase> substanceConditionBases = new List<SubstanceConditionBase>();
                foreach (SubstanceCondition substanceCondition in this.MixtureCondition.Substances)
                    substanceConditionBases.Add(BaseManager.Current.GetBase<SubstanceConditionBase>(substanceCondition));
                substances = substanceConditionBases.ToArray();
            }
        }
        #endregion Property: Substances

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: MixtureConditionBase(MixtureCondition mixtureCondition)
        /// <summary>
        /// Creates a base of the given mixture condition.
        /// </summary>
        /// <param name="mixtureCondition">The mixture condition to create a base of.</param>
        protected internal MixtureConditionBase(MixtureCondition mixtureCondition)
            : base(mixtureCondition)
        {
            if (mixtureCondition != null)
            {
                this.mixtureType = mixtureCondition.MixtureType;
                this.mixtureTypeSign = mixtureCondition.MixtureTypeSign;
                this.composition = mixtureCondition.Composition;
                this.compositionSign = mixtureCondition.CompositionSign;
                this.hasAllMandatorySubstances = mixtureCondition.HasAllMandatorySubstances;

                if (BaseManager.PreloadProperties)
                    LoadSubstances();
            }
        }
        #endregion Constructor: MixtureConditionBase(MixtureCondition mixtureCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: MixtureConditionBase

    #region Class: MixtureChangeBase
    /// <summary>
    /// A change on a mixture.
    /// </summary>
    public class MixtureChangeBase : MatterChangeBase
    {

        #region Properties and Fields

        #region Property: MixtureChange
        /// <summary>
        /// Gets the mixture change of which this is a mixture change base.
        /// </summary>
        protected internal MixtureChange MixtureChange
        {
            get
            {
                return this.Change as MixtureChange;
            }
        }
        #endregion Property: MixtureChange

        #region Property: MixtureBase
        /// <summary>
        /// Gets the affected mixture base.
        /// </summary>
        public MixtureBase MixtureBase
        {
            get
            {
                return this.NodeBase as MixtureBase;
            }
        }
        #endregion Property: MixtureBase

        #region Property: Composition
        /// <summary>
        /// The composition to change to.
        /// </summary>
        private Composition? composition = null;

        /// <summary>
        /// Gets the composition to change to.
        /// </summary>
        public Composition? Composition
        {
            get
            {
                return composition;
            }
        }
        #endregion Property: MixtureType

        #region Property: MixtureType
        /// <summary>
        /// The mixture type to change to.
        /// </summary>
        private MixtureType? mixtureType = null;

        /// <summary>
        /// Gets the mixture type to change to.
        /// </summary>
        public MixtureType? MixtureType
        {
            get
            {
                return mixtureType;
            }
        }
        #endregion Property: MixtureType

        #region Property: MixturesToAdd
        /// <summary>
        /// The mixtures that should be added during the change.
        /// </summary>
        private MixtureValuedBase[] mixturesToAdd = null;

        /// <summary>
        /// Gets the mixtures that should be added during the change.
        /// </summary>
        public ReadOnlyCollection<MixtureValuedBase> MixturesToAdd
        {
            get
            {
                if (mixturesToAdd == null)
                {
                    LoadMixturesToAdd();
                    if (mixturesToAdd == null)
                        return new List<MixtureValuedBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<MixtureValuedBase>(mixturesToAdd);
            }
        }

        /// <summary>
        /// Loads the mixtures to add.
        /// </summary>
        private void LoadMixturesToAdd()
        {
            if (this.MixtureChange != null)
            {
                List<MixtureValuedBase> mixtureValuedBases = new List<MixtureValuedBase>();
                foreach (MixtureValued mixtureValued in this.MixtureChange.MixturesToAdd)
                    mixtureValuedBases.Add(BaseManager.Current.GetBase<MixtureValuedBase>(mixtureValued));
                mixturesToAdd = mixtureValuedBases.ToArray();
            }
        }
        #endregion Property: MixturesToAdd

        #region Property: Substances
        /// <summary>
        /// The substances to change.
        /// </summary>
        private SubstanceChangeBase[] substances = null;

        /// <summary>
        /// Gets the substances to change.
        /// </summary>
        public ReadOnlyCollection<SubstanceChangeBase> Substances
        {
            get
            {
                if (substances == null)
                {
                    LoadSubstances();
                    if (substances == null)
                        return new List<SubstanceChangeBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<SubstanceChangeBase>(substances);
            }
        }

        /// <summary>
        /// Loads the substances.
        /// </summary>
        private void LoadSubstances()
        {
            if (this.MixtureChange != null)
            {
                List<SubstanceChangeBase> substanceChangeBases = new List<SubstanceChangeBase>();
                foreach (SubstanceChange substanceChange in this.MixtureChange.Substances)
                    substanceChangeBases.Add(BaseManager.Current.GetBase<SubstanceChangeBase>(substanceChange));
                substances = substanceChangeBases.ToArray();
            }
        }
        #endregion Property: Substances

        #region Property: SubstancesToAdd
        /// <summary>
        /// The substances that should be added during the change.
        /// </summary>
        private SubstanceValuedBase[] substancesToAdd = null;

        /// <summary>
        /// Gets the substances that should be added during the change.
        /// </summary>
        public ReadOnlyCollection<SubstanceValuedBase> SubstancesToAdd
        {
            get
            {
                if (substancesToAdd == null)
                {
                    LoadSubstancesToAdd();
                    if (substancesToAdd == null)
                        return new List<SubstanceValuedBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<SubstanceValuedBase>(substancesToAdd);
            }
        }

        /// <summary>
        /// Loads the substances to add.
        /// </summary>
        private void LoadSubstancesToAdd()
        {
            if (this.MixtureChange != null)
            {
                List<SubstanceValuedBase> substanceValuedBases = new List<SubstanceValuedBase>();
                foreach (SubstanceValued substanceValued in this.MixtureChange.SubstancesToAdd)
                    substanceValuedBases.Add(BaseManager.Current.GetBase<SubstanceValuedBase>(substanceValued));
                substancesToAdd = substanceValuedBases.ToArray();
            }
        }
        #endregion Property: SubstancesToAdd

        #region Property: SubstancesToRemove
        /// <summary>
        /// The substances that should be removed during the change.
        /// </summary>
        private SubstanceConditionBase[] substancesToRemove = null;

        /// <summary>
        /// Gets the substances that should be removed during the change.
        /// </summary>
        public ReadOnlyCollection<SubstanceConditionBase> SubstancesToRemove
        {
            get
            {
                if (substancesToRemove == null)
                {
                    LoadSubstancesToRemove();
                    if (substancesToRemove == null)
                        return new List<SubstanceConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<SubstanceConditionBase>(substancesToRemove);
            }
        }

        /// <summary>
        /// Loads the substances to remove.
        /// </summary>
        private void LoadSubstancesToRemove()
        {
            if (this.MixtureChange != null)
            {
                List<SubstanceConditionBase> substanceConditionBases = new List<SubstanceConditionBase>();
                foreach (SubstanceCondition substanceCondition in this.MixtureChange.SubstancesToRemove)
                    substanceConditionBases.Add(BaseManager.Current.GetBase<SubstanceConditionBase>(substanceCondition));
                substancesToRemove = substanceConditionBases.ToArray();
            }
        }
        #endregion Property: SubstancesToRemove

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: MixtureChangeBase(MixtureChange mixtureChange)
        /// <summary>
        /// Creates a base of the given mixture change.
        /// </summary>
        /// <param name="mixtureChange">The mixture change to create a base of.</param>
        protected internal MixtureChangeBase(MixtureChange mixtureChange)
            : base(mixtureChange)
        {
            if (mixtureChange != null)
            {
                this.composition = mixtureChange.Composition;
                this.mixtureType = mixtureChange.MixtureType;

                if (BaseManager.PreloadProperties)
                {
                    LoadMixturesToAdd();
                    LoadSubstances();
                    LoadSubstancesToAdd();
                    LoadSubstancesToRemove();
                }
            }
        }
        #endregion Constructor: MixtureChangeBase(MixtureChange mixtureChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: MixtureChangeBase

}