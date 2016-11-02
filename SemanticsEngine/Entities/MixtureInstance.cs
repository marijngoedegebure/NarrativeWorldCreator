/**************************************************************************
 * 
 * MixtureInstance.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2010-2012
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Common;
using Semantics.Entities;
using Semantics.Utilities;
using SemanticsEngine.Components;
using SemanticsEngine.Interfaces;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Entities
{

    #region Class: MixtureInstance
    /// <summary>
    /// An instance of a mixture.
    /// </summary>
    public class MixtureInstance : MatterInstance
    {

        #region Events, Properties, and Fields

        #region Events: SubstanceHandler
        /// <summary>
        /// A handler for added or removed substances.
        /// </summary>
        /// <param name="sender">The mixture instance the substance was added to or removed from.</param>
        /// <param name="substance">The added or removed substance.</param>
        public delegate void SubstanceHandler(MixtureInstance sender, SubstanceInstance substance);

        /// <summary>
        /// An event to indicate an added substance.
        /// </summary>
        public event SubstanceHandler SubstanceAdded;

        /// <summary>
        /// An event to indicate a removed substance.
        /// </summary>
        public event SubstanceHandler SubstanceRemoved;
        #endregion Events: SubstanceHandler

        #region Property: MixtureBase
        /// <summary>
        /// Gets the mixture base of which this is a mixture instance.
        /// </summary>
        public MixtureBase MixtureBase
        {
            get
            {
                return this.NodeBase as MixtureBase;
            }
        }
        #endregion Property: MixtureBase

        #region Property: MixtureValuedBase
        /// <summary>
        /// Gets the valued mixture base of which this is a mixture instance.
        /// </summary>
        public MixtureValuedBase MixtureValuedBase
        {
            get
            {
                return this.Base as MixtureValuedBase;
            }
        }
        #endregion Property: MixtureValuedBase

        #region Property: MixtureType
        /// <summary>
        /// Gets the mixture type.
        /// </summary>
        public MixtureType MixtureType
        {
            get
            {
                if (this.MixtureBase != null)
                    return GetProperty<MixtureType>("MixtureType", this.MixtureBase.MixtureType);
                
                return default(MixtureType);
            }
            protected set
            {
                if (this.MixtureType != value)
                    SetProperty("MixtureType", value);
            }
        }
        #endregion Property: MixtureType

        #region Property: Composition
        /// <summary>
        /// Gets the composition.
        /// </summary>
        public Composition Composition
        {
            get
            {
                if (this.MixtureBase != null)
                    return GetProperty<Composition>("Composition", this.MixtureBase.Composition);
                
                return default(Composition);
            }
            protected set
            {
                if (this.Composition != value)
                    SetProperty("Composition", value);
            }
        }
        #endregion Property: MixtureType

        #region Property: Substances
        /// <summary>
        /// The substances that this mixture is made of.
        /// </summary>
        private SubstanceInstance[] substances = null;
        
        /// <summary>
        /// Gets all the substances that this mixture is made of.
        /// </summary>
        public ReadOnlyCollection<SubstanceInstance> Substances
        {
            get
            {
                if (substances != null)
                    return new ReadOnlyCollection<SubstanceInstance>(substances);

                return new List<SubstanceInstance>(0).AsReadOnly();
            }
        }
        #endregion Property: Substances

        #endregion Events, Properties, and Fields

        #region Method Group: Constructors

        #region Constructor: MixtureInstance(MixtureBase mixtureBase)
        /// <summary>
        /// Creates a new mixture instance from the given mixture base.
        /// </summary>
        /// <param name="mixtureBase">The mixture base to create the mixture instance from.</param>
        internal MixtureInstance(MixtureBase mixtureBase)
            : this(mixtureBase, false)
        {
        }

        /// <summary>
        /// Creates a new mixture instance from the given mixture base.
        /// </summary>
        /// <param name="mixtureBase">The mixture base to create the mixture instance from.</param>
        /// <param name="ignoreCreation">Indicates whether creation of substances should be ignored.</param>
        private MixtureInstance(MixtureBase mixtureBase, bool ignoreCreation)
            : base(mixtureBase, ignoreCreation)
        {
            if (!ignoreCreation && mixtureBase != null)
            {
                // Create instances from the mandatory base substances
                foreach (SubstanceValuedBase substanceValuedBase in this.MixtureBase.Substances)
                {
                    if ((InstanceManager.IgnoreNecessity || substanceValuedBase.Necessity == Necessity.Mandatory) && substanceValuedBase.Quantity != null)
                        AddSubstance(InstanceManager.Current.Create<SubstanceInstance>(substanceValuedBase));
                }
            }
        }
        #endregion Constructor: MixtureInstance(MixtureBase mixtureBase)

        #region Constructor: MixtureInstance(MixtureValuedBase mixtureValuedBase)
        /// <summary>
        /// Creates a new mixture instance from the given valued mixture base.
        /// </summary>
        /// <param name="mixtureValuedBase">The valued mixture base to create the mixture instance from.</param>
        internal MixtureInstance(MixtureValuedBase mixtureValuedBase)
            : this(mixtureValuedBase, false)
        {
        }

        /// <summary>
        /// Creates a new mixture instance from the given valued mixture base.
        /// </summary>
        /// <param name="mixtureValuedBase">The valued mixture base to create the mixture instance from.</param>
        /// <param name="ignoreCreation">Indicates whether creation of substances should be ignored.</param>
        private MixtureInstance(MixtureValuedBase mixtureValuedBase, bool ignoreCreation)
            : base(mixtureValuedBase, ignoreCreation)
        {
            if (!ignoreCreation && mixtureValuedBase != null)
            {
                // Create instances from the mandatory base substances
                foreach (SubstanceValuedBase substanceValuedBase in this.MixtureBase.Substances)
                {
                    if ((InstanceManager.IgnoreNecessity || substanceValuedBase.Necessity == Necessity.Mandatory) && substanceValuedBase.Quantity != null)
                        AddSubstance(InstanceManager.Current.Create<SubstanceInstance>(substanceValuedBase));
                }
            }
        }
        #endregion Constructor: MixtureInstance(MixtureValuedBase mixtureValuedBase)

        #region Constructor: MixtureInstance(MixtureInstance mixtureInstance)
        /// <summary>
        /// Clones a mixture instance.
        /// </summary>
        /// <param name="mixtureInstance">The mixture instance to clone.</param>
        protected internal MixtureInstance(MixtureInstance mixtureInstance)
            : base(mixtureInstance)
        {
            if (mixtureInstance != null)
            {
                this.MixtureType = mixtureInstance.MixtureType;
                this.Composition = mixtureInstance.Composition;
                foreach (SubstanceInstance substanceInstance in mixtureInstance.Substances)
                    AddSubstance(new SubstanceInstance(substanceInstance));
            }
        }
        #endregion Constructor: MixtureInstance(MixtureInstance mixtureInstance)

        #region Method: TryCreate(List<MatterInstance> matterInstances, out MixtureInstance mixtureInstance)
        /// <summary>
        /// Try to create a mixture instance from the given matter.
        /// </summary>
        /// <param name="matterInstances">The matter instances to try mixing into a mixture.</param>
        /// <param name="mixtureInstance">The resulting mixture instance.</param>
        /// <returns>Returns whether the mixture creation has been successful.</returns>
        internal static bool TryCreate(List<MatterInstance> matterInstances, out MixtureInstance mixtureInstance)
        {
            mixtureInstance = null;

            if (matterInstances != null && matterInstances.Count > 0)
            {
                // Only use substances; also get their quantities
                List<SubstanceInstance> availableSubstanceInstances = new List<SubstanceInstance>();
                List<SubstanceBase> availableSubstanceBases = new List<SubstanceBase>();
                List<float> availableQuantities = new List<float>();
                foreach (MatterInstance matterInstance in matterInstances)
                {
                    SubstanceInstance substanceInstance = matterInstance as SubstanceInstance;
                    if (substanceInstance != null)
                    {
                        availableSubstanceInstances.Add(substanceInstance);
                        availableSubstanceBases.Add(substanceInstance.SubstanceBase);
                        availableQuantities.Add(substanceInstance.Quantity.BaseValue);
                    }
                }

                // Go through each available mixture base
                List<MixtureBase> validMixtureBases = new List<MixtureBase>();
                int mostNrOfSubstances = 0;
                int indexWithMostNrOfSubstances = -1;
                List<MixtureBase> allMixtures = BaseManager.Current.GetBases<Mixture, MixtureBase>();
                for (int i = 0; i < allMixtures.Count; i++)
                {
                    MixtureBase mixtureBase = allMixtures[i];
                    int nrOfSubstances = mixtureBase.Substances.Count;
                    if (nrOfSubstances > 0)
                    {
                        // Only mixtures of which all substances are available in the correct state of matter are valid
                        bool validMixture = true;
                        foreach (SubstanceValuedBase substanceValuedBase in mixtureBase.Substances)
                        {
                            bool contains = false;
                            for (int j = 0; j < availableSubstanceBases.Count; j++)
                            {
                                if (availableSubstanceBases[j].IsNodeOf(substanceValuedBase.SubstanceBase) && substanceValuedBase.StateOfMatter == availableSubstanceInstances[j].StateOfMatter)
                                {
                                    contains = true;
                                    break;
                                }
                            }
                            if (!contains)
                            {
                                validMixture = false;
                                break;
                            }
                        }
                        if (validMixture)
                        {
                            validMixtureBases.Add(mixtureBase);
                            if (nrOfSubstances > mostNrOfSubstances)
                                indexWithMostNrOfSubstances = i;
                        }
                    }
                }

                if (indexWithMostNrOfSubstances != -1)
                {
                    // Get the mixture base with the most number of substances
                    MixtureBase mixtureBase = validMixtureBases[indexWithMostNrOfSubstances];

                    // Get the required substance quantities
                    List<SubstanceBase> requiredSubstanceBases = new List<SubstanceBase>();
                    List<float> requiredQuantities = new List<float>();
                    foreach (SubstanceValuedBase substanceValuedBase in mixtureBase.Substances)
                    {
                        requiredSubstanceBases.Add(substanceValuedBase.SubstanceBase);
                        requiredQuantities.Add(substanceValuedBase.Quantity.BaseValue);
                    }

                    // Get the smallest ratio, used to decide the quantities of the substances in the mixture 
                    int indexWithSmallestRatio = 0;
                    float smallestRatio = float.PositiveInfinity;
                    for (int i = 0; i < requiredSubstanceBases.Count; i++)
                    {
                        for (int j = 0; j < availableSubstanceBases.Count; j++)
                        {
                            if (availableSubstanceBases[j].IsNodeOf(requiredSubstanceBases[i]))
                            {
                                float ratio = availableQuantities[j] / requiredQuantities[i];
                                if (ratio < smallestRatio)
                                {
                                    indexWithSmallestRatio = i;
                                    smallestRatio = ratio;
                                }
                            }
                        }
                    }

                    // Get the quantities of the substances that have to be used in the mixture
                    List<float> creationQuantities = new List<float>();
                    foreach (float requiredQuantity in requiredQuantities)
                        creationQuantities.Add(requiredQuantity * smallestRatio);

                    // Create a new mixture instance without substances
                    mixtureInstance = new MixtureInstance(mixtureBase, true);

                    // Go through all required substances and find the corresponding available substance instance
                    for (int i = 0; i < requiredSubstanceBases.Count; i++)
                    {
                        foreach (SubstanceInstance availableSubstanceInstance in availableSubstanceInstances)
                        {
                            if (availableSubstanceInstance.SubstanceBase.IsNodeOf(requiredSubstanceBases[i]))
                            {
                                // Remove the creation quantity from the substance instance
                                availableSubstanceInstance.Quantity -= creationQuantities[i];

                                // Create a new substance instance with that quantity
                                SubstanceInstance newSubstanceInstance = new SubstanceInstance(requiredSubstanceBases[i], true);
                                newSubstanceInstance.Quantity = creationQuantities[i];

                                // Add the new substance instance to the mixture instance
                                mixtureInstance.AddSubstance(newSubstanceInstance);

                                break;
                            }
                        }
                    }

                    return true;
                }

                //--Example--

                // Given substances with quantities:
                // 8 A
                // 1 B
                // 4 C
                // 5 D

                // Recipe with required substances with quantities:
                // 6 A
                // 2 B
                // 1 C

                // D can be ignored

                // Get smallest ratio by dividing given quantity by required quantity
                // 8/6 = 1,33
                // 1/2 = 0,5
                // 4/1 = 4

                // Multiply smallest ratio by recipe quantities to get quantity of resulting mixture:
                // 3 A (remaining: 5 A)
                // 1 B (remaining: 1 B)
                // 0,5 C (remaining: 3,5 C)
            }

            return false;
        }
        #endregion Method: TryCreate(List<MatterInstance> matterInstances, out MixtureInstance mixtureInstance)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddSubstance(SubstanceInstance substanceInstance)
        /// <summary>
        /// Adds a substance instance.
        /// </summary>
        /// <param name="substanceInstance">The substance instance to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        protected internal Message AddSubstance(SubstanceInstance substanceInstance)
        {
            if (substanceInstance != null && substanceInstance.Quantity != null && substanceInstance.Quantity.Value >= 0)
            {
                // If this substance instance is already available, there is no use to add it
                if (this.Substances.Contains(substanceInstance))
                    return Message.RelationExistsAlready;

                // If there is an instance that is based on the same substance, merge them
                if (substanceInstance.SubstanceBase != null)
                {
                    foreach (SubstanceInstance mySubstanceInstance in this.Substances)
                    {
                        if (substanceInstance.SubstanceBase.Equals(mySubstanceInstance.SubstanceBase))
                        {
                            // Increase the quantity of our substance with the quantity of the other one
                            mySubstanceInstance.Quantity += substanceInstance.Quantity;

                            // Deplete the quantity of the other, making it useless and to be removed
                            substanceInstance.Quantity -= substanceInstance.Quantity;

                            return Message.RelationSuccess;
                        }
                    }
                }

                // Add the substance instance
                Utils.AddToArray<SubstanceInstance>(ref this.substances, substanceInstance);
                NotifyPropertyChanged("Substances");

                // Increase the quantity
                this.Quantity += substanceInstance.Quantity;

                // Set this mixture
                substanceInstance.Mixture = this;

                // Set the position
                substanceInstance.Position = this.Position;

                // Set the world if this has not been done before
                if (substanceInstance.World == null && this.World != null)
                    this.World.AddInstance(substanceInstance);

                // Invoke an event
                if (SubstanceAdded != null)
                    SubstanceAdded.Invoke(this, substanceInstance);

                // Notify the engine
                if (SemanticsEngine.Current != null)
                    SemanticsEngine.Current.HandleSubstanceAdded(this, substanceInstance);

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddSubstance(SubstanceInstance substanceInstance)

        #region Method: RemoveSubstance(SubstanceInstance substanceInstance)
        /// <summary>
        /// Removes a substance instance.
        /// </summary>
        /// <param name="substanceInstance">The substance instance to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        protected internal Message RemoveSubstance(SubstanceInstance substanceInstance)
        {
            if (substanceInstance != null)
            {
                if (this.Substances.Contains(substanceInstance))
                {
                    // Remove the substance instance
                    Utils.RemoveFromArray<SubstanceInstance>(ref this.substances, substanceInstance);
                    NotifyPropertyChanged("Substances");

                    // Decrease the quantity
                    this.Quantity -= substanceInstance.Quantity;

                    // Reset this mixture
                    substanceInstance.Mixture = null;

                    // Invoke an event
                    if (SubstanceRemoved != null)
                        SubstanceRemoved.Invoke(this, substanceInstance);

                    // Notify the engine
                    if (SemanticsEngine.Current != null)
                        SemanticsEngine.Current.HandleSubstanceRemoved(this, substanceInstance);

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveSubstance(SubstanceInstance substanceInstance)

        #region Method: AddMixture(MixtureInstance mixtureInstance)
        /// <summary>
        /// Adds a mixture instance.
        /// </summary>
        /// <param name="mixtureInstance">The mixture instance to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        protected internal Message AddMixture(MixtureInstance mixtureInstance)
        {
            if (mixtureInstance != null)
            {
                // Move the substance instances from the given mixture instance to this mixture instance
                List<SubstanceInstance> substances = new List<SubstanceInstance>(mixtureInstance.Substances);
                foreach (SubstanceInstance substanceInstance in substances)
                {
                    mixtureInstance.RemoveSubstance(substanceInstance);
                    AddSubstance(substanceInstance);
                }

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddMixture(MixtureInstance mixtureInstance)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasSubstance(SubstanceBase substanceBase)
        /// <summary>
        /// Checks if this material has a substance instance of the given substance.
        /// </summary>
        /// <param name="substanceBase">The substance to check.</param>
        /// <returns>Returns true when the material has a substance instance of the given substance.</returns>
        public bool HasSubstance(SubstanceBase substanceBase)
        {
            if (substanceBase != null)
            {
                foreach (SubstanceInstance substanceInstance in this.Substances)
                {
                    if (substanceBase.Equals(substanceInstance.SubstanceBase))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasSubstance(SubstanceBase substanceBase)

        #region Method: MarkAsModified(bool modified, bool spread)
        /// <summary>
        /// Mark this instance as modified.
        /// </summary>
        /// <param name="modified">The value that indicates whether the instance has been modified.</param>
        /// <param name="spread">The value that indicates whether the marking should spread further.</param>
        internal override void MarkAsModified(bool modified, bool spread)
        {
            base.MarkAsModified(modified, spread);

            if (spread)
            {
                foreach (SubstanceInstance substance in this.Substances)
                    substance.MarkAsModified(modified, false);
            }
        }
        #endregion Method: MarkAsModified(bool modified, bool spread)

        #region Method: SetPosition(Vec3 newPosition)
        /// <summary>
        /// Set the new position.
        /// </summary>
        /// <param name="newPosition">The new position.</param>
        protected override void SetPosition(Vec3 newPosition)
        {
            Vec3 newPos = new Vec3(newPosition);
            Vec3 oldPosition = new Vec3(this.Position);
            Vec3 delta = newPos - oldPosition;

            // First set the position of the base
            base.SetPosition(newPos);

            // Then move all the substances
            foreach (SubstanceInstance substance in this.Substances)
                substance.Position += delta;
        }
        #endregion Method: SetPosition(Vec3 newPosition)

        #region Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the mixture instance satisfies the given condition.
        /// </summary>
        /// <param name="conditionBase">The condition to compare to the mixture instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the mixture instance satisfies the given condition.</returns>
        public override bool Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (conditionBase != null)
            {
                // Check whether the base satisfies the condition
                if (base.Satisfies(conditionBase, iVariableInstanceHolder))
                {
                    // Mixture condition
                    MixtureConditionBase mixtureConditionBase = conditionBase as MixtureConditionBase;
                    if (mixtureConditionBase != null)
                    {
                        // Check whether the composition and type have been satisfied
                        if ((mixtureConditionBase.MixtureTypeSign == null || mixtureConditionBase.MixtureType == null || Toolbox.Compare(this.MixtureBase.MixtureType, (EqualitySign)mixtureConditionBase.MixtureTypeSign, (MixtureType)mixtureConditionBase.MixtureType)) &&
                            (mixtureConditionBase.CompositionSign == null || mixtureConditionBase.Composition == null || Toolbox.Compare(this.MixtureBase.Composition, (EqualitySign)mixtureConditionBase.CompositionSign, (Composition)mixtureConditionBase.Composition)))
                        {
                            // Check whether the instance has all mandatory substances
                            if (mixtureConditionBase.HasAllMandatorySubstances == true)
                            {
                                foreach (SubstanceValuedBase substanceValuedBase in this.MixtureBase.Substances)
                                {
                                    if (substanceValuedBase.Necessity == Necessity.Mandatory)
                                    {
                                        NumericalValueInstance quantity = new NumericalValueInstance(0);
                                        foreach (SubstanceInstance substanceInstance in this.Substances)
                                        {
                                            if (substanceInstance.IsNodeOf(substanceValuedBase.SubstanceBase))
                                                quantity += substanceInstance.Quantity;
                                        }
                                        if (!substanceValuedBase.Quantity.IsInRange(quantity))
                                            return false;
                                    }
                                }
                            }

                            // Check whether the instance has all required substances
                            foreach (SubstanceConditionBase substanceConditionBase in mixtureConditionBase.Substances)
                            {
                                bool satisfied = false;
                                foreach (SubstanceInstance substanceInstance in this.Substances)
                                {
                                    if (substanceInstance.Satisfies(substanceConditionBase, iVariableInstanceHolder))
                                    {
                                        satisfied = true;
                                        break;
                                    }
                                }
                                if (!satisfied)
                                    return false;
                            }
                            return true;
                        }
                    }
                }
                else
                {
                    // Substance condition
                    SubstanceConditionBase substanceCondition = conditionBase as SubstanceConditionBase;
                    if (substanceCondition != null)
                    {
                        foreach (SubstanceInstance substanceInstance in this.Substances)
                        {
                            if (substanceInstance.Satisfies(substanceCondition, iVariableInstanceHolder))
                                return true;
                        }
                        return false;
                    }
                }
            }
            return false;
        }
        #endregion Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Apply the given change to the mixture instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the mixture instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        protected internal override bool Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (changeBase != null)
            {
                if (base.Apply(changeBase, iVariableInstanceHolder))
                {
                    // Mixture change
                    MixtureChangeBase mixtureChangeBase = changeBase as MixtureChangeBase;
                    if (mixtureChangeBase != null)
                    {
                        // Apply all changes
                        if (mixtureChangeBase.Composition != null)
                            this.Composition = (Composition)mixtureChangeBase.Composition;
                        if (mixtureChangeBase.MixtureType != null)
                            this.MixtureType = (MixtureType)mixtureChangeBase.MixtureType;

                        // Add the right amount of mixtures
                        foreach (MixtureValuedBase mixtureValuedBase in mixtureChangeBase.MixturesToAdd)
                            AddMixture(InstanceManager.Current.Create<MixtureInstance>(mixtureValuedBase));

                        // Change the substances
                        foreach (SubstanceChangeBase substanceChangeBase in mixtureChangeBase.Substances)
                        {
                            foreach (SubstanceInstance substanceInstance in this.Substances)
                                substanceInstance.Apply(substanceChangeBase, iVariableInstanceHolder);
                        }

                        // Add the substances
                        foreach (SubstanceValuedBase substanceValuedBase in mixtureChangeBase.SubstancesToAdd)
                            AddSubstance(InstanceManager.Current.Create<SubstanceInstance>(substanceValuedBase));

                        // Remove the substances
                        foreach (SubstanceConditionBase substanceConditionBase in mixtureChangeBase.SubstancesToRemove)
                        {
                            if (substanceConditionBase.Quantity == null ||
                                substanceConditionBase.Quantity.BaseValue == null ||
                                substanceConditionBase.Quantity.ValueSign == null)
                            {
                                foreach (SubstanceInstance substanceInstance in this.Substances)
                                {
                                    if (substanceInstance.Satisfies(substanceConditionBase, iVariableInstanceHolder))
                                    {
                                        RemoveSubstance(substanceInstance);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                ReadOnlyCollection<SubstanceInstance> substances = this.Substances;
                                foreach (SubstanceInstance substanceInstance in substances)
                                {
                                    if (substanceInstance.Satisfies(substanceConditionBase, iVariableInstanceHolder))
                                    {
                                        substanceInstance.Quantity -= (float)substanceConditionBase.Quantity.BaseValue;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    return true;
                }
                else
                {
                    // Substance change
                    SubstanceChangeBase substanceChange = changeBase as SubstanceChangeBase;
                    if (substanceChange != null)
                    {
                        foreach (SubstanceInstance substanceInstance in this.Substances)
                            substanceInstance.Apply(substanceChange, iVariableInstanceHolder);
                        return true;
                    }
                }
            }
            return false;
        }
        #endregion Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: MixtureInstance

}