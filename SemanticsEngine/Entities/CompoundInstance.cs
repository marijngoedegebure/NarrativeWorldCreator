/**************************************************************************
 * 
 * CompoundInstance.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011-2012
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

    #region Class: CompoundInstance
    /// <summary>
    /// An instance of a compound.
    /// </summary>
    public class CompoundInstance : MatterInstance
    {

        #region Events, Properties, and Fields

        #region Events: SubstanceHandler
        /// <summary>
        /// A handler for added or removed substances.
        /// </summary>
        /// <param name="sender">The compound instance the substance was added to or removed from.</param>
        /// <param name="substance">The added or removed substance.</param>
        public delegate void SubstanceHandler(CompoundInstance sender, SubstanceInstance substance);

        /// <summary>
        /// An event to indicate an added substance.
        /// </summary>
        public event SubstanceHandler SubstanceAdded;

        /// <summary>
        /// An event to indicate a removed substance.
        /// </summary>
        public event SubstanceHandler SubstanceRemoved;
        #endregion Events: SubstanceHandler

        #region Property: CompoundBase
        /// <summary>
        /// Gets the valued compound base of which this is a compound instance.
        /// </summary>
        public CompoundBase CompoundBase
        {
            get
            {
                return this.NodeBase as CompoundBase;
            }
        }
        #endregion Property: CompoundBase

        #region Property: CompoundValuedBase
        /// <summary>
        /// Gets the valued compound base of which this is a compound instance.
        /// </summary>
        public CompoundValuedBase CompoundValuedBase
        {
            get
            {
                return this.Base as CompoundValuedBase;
            }
        }
        #endregion Property: CompoundValuedBase

        #region Property: Substances
        /// <summary>
        /// The substances that this compound is made of.
        /// </summary>
        private SubstanceInstance[] substances = null;
        
        /// <summary>
        /// Gets all the substances that this compound is made of.
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

        #region Constructor: CompoundInstance(CompoundBase compoundBase)
        /// <summary>
        /// Creates a new compound instance from the given compound base.
        /// </summary>
        /// <param name="compoundBase">The compound base to create the compound instance from.</param>
        internal CompoundInstance(CompoundBase compoundBase)
            : this(compoundBase, false)
        {
        }

        /// <summary>
        /// Creates a new compound instance from the given compound base.
        /// </summary>
        /// <param name="compoundBase">The compound base to create the compound instance from.</param>
        /// <param name="ignoreCreation">Indicates whether creation of substances should be ignored.</param>
        private CompoundInstance(CompoundBase compoundBase, bool ignoreCreation)
            : base(compoundBase, ignoreCreation)
        {
            if (!ignoreCreation && compoundBase != null)
            {
                // Create instances from the mandatory base substances
                foreach (SubstanceValuedBase substanceValuedBase in this.CompoundBase.Substances)
                {
                    if ((InstanceManager.IgnoreNecessity || substanceValuedBase.Necessity == Necessity.Mandatory) && substanceValuedBase.Quantity != null)
                        AddSubstance(InstanceManager.Current.Create<SubstanceInstance>(substanceValuedBase));
                }
            }
        }
        #endregion Constructor: CompoundInstance(CompoundBase compoundBase)

        #region Constructor: CompoundInstance(CompoundValuedBase compoundValuedBase)
        /// <summary>
        /// Creates a new compound instance from the given valued compound base.
        /// </summary>
        /// <param name="compoundValuedBase">The valued compound base to create the compound instance from.</param>
        internal CompoundInstance(CompoundValuedBase compoundValuedBase)
            : this(compoundValuedBase, false)
        {
        }

        /// <summary>
        /// Creates a new compound instance from the given valued compound base.
        /// </summary>
        /// <param name="compoundValuedBase">The valued compound base to create the compound instance from.</param>
        /// <param name="ignoreCreation">Indicates whether creation of substances should be ignored.</param>
        private CompoundInstance(CompoundValuedBase compoundValuedBase, bool ignoreCreation)
            : base(compoundValuedBase, ignoreCreation)
        {
            if (!ignoreCreation && compoundValuedBase != null)
            {
                // Create instances from the mandatory base substances
                foreach (SubstanceValuedBase substanceValuedBase in this.CompoundBase.Substances)
                {
                    if ((InstanceManager.IgnoreNecessity || substanceValuedBase.Necessity == Necessity.Mandatory) && substanceValuedBase.Quantity != null)
                        AddSubstance(InstanceManager.Current.Create<SubstanceInstance>(substanceValuedBase));
                }
            }
        }
        #endregion Constructor: CompoundInstance(CompoundValuedBase compoundValuedBase)

        #region Constructor: CompoundInstance(CompoundInstance compoundInstance)
        /// <summary>
        /// Clones a compound instance.
        /// </summary>
        /// <param name="compoundInstance">The compound instance to clone.</param>
        protected internal CompoundInstance(CompoundInstance compoundInstance)
            : base(compoundInstance)
        {
            if (compoundInstance != null)
            {
                foreach (SubstanceInstance substanceInstance in compoundInstance.Substances)
                    AddSubstance(new SubstanceInstance(substanceInstance));
            }
        }
        #endregion Constructor: CompoundInstance(CompoundInstance compoundInstance)

        #region Method: TryCreate(List<MatterInstance> matterInstances, out CompoundInstance compoundInstance)
        /// <summary>
        /// Try to create a compound instance from the given matter.
        /// </summary>
        /// <param name="matterInstances">The matter instances to try mixing into a compound.</param>
        /// <param name="compoundInstance">The resulting compound instance.</param>
        /// <returns>Returns whether the compound creation has been successful.</returns>
        internal static bool TryCreate(List<MatterInstance> matterInstances, out CompoundInstance compoundInstance)
        {
            compoundInstance = null;

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

                // Go through each available compound base
                List<CompoundBase> validCompoundBases = new List<CompoundBase>();
                int mostNrOfSubstances = 0;
                int indexWithMostNrOfSubstances = -1;
                List<CompoundBase> allCompounds = BaseManager.Current.GetBases<Compound, CompoundBase>();
                for (int i = 0; i < allCompounds.Count; i++)
                {
                    CompoundBase compoundBase = allCompounds[i];
                    int nrOfSubstances = compoundBase.Substances.Count;
                    if (nrOfSubstances > 0)
                    {
                        // Only compounds of which all substances are available in the correct state of matter are valid
                        bool validCompound = true;
                        foreach (SubstanceValuedBase substanceValuedBase in compoundBase.Substances)
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
                                validCompound = false;
                                break;
                            }
                        }
                        if (validCompound)
                        {
                            validCompoundBases.Add(compoundBase);
                            if (nrOfSubstances > mostNrOfSubstances)
                                indexWithMostNrOfSubstances = i;
                        }
                    }
                }

                if (indexWithMostNrOfSubstances != -1)
                {
                    // Get the compound base with the most number of substances
                    CompoundBase compoundBase = validCompoundBases[indexWithMostNrOfSubstances];

                    // Get the required substance quantities
                    List<SubstanceBase> requiredSubstanceBases = new List<SubstanceBase>();
                    List<float> requiredQuantities = new List<float>();
                    foreach (SubstanceValuedBase substanceValuedBase in compoundBase.Substances)
                    {
                        requiredSubstanceBases.Add(substanceValuedBase.SubstanceBase);
                        requiredQuantities.Add(substanceValuedBase.Quantity.BaseValue);
                    }

                    // Get the smallest ratio, used to decide the quantities of the substances in the compound 
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

                    // Get the quantities of the substances that have to be used in the compound
                    List<float> creationQuantities = new List<float>();
                    foreach (float requiredQuantity in requiredQuantities)
                        creationQuantities.Add(requiredQuantity * smallestRatio);

                    // Create a new compound instance without substances
                    compoundInstance = new CompoundInstance(compoundBase, true);

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
                                newSubstanceInstance.Quantity.Value = creationQuantities[i];

                                // Add the new substance instance to the compound instance
                                compoundInstance.AddSubstance(newSubstanceInstance);

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

                // Multiply smallest ratio by recipe quantities to get quantity of resulting compound:
                // 3 A (remaining: 5 A)
                // 1 B (remaining: 1 B)
                // 0,5 C (remaining: 3,5 C)
            }

            return false;
        }
        #endregion Method: TryCreate(List<MatterInstance> matterInstances, out CompoundInstance compoundInstance)

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

                // Set this compound
                substanceInstance.Compound = this;

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

                    // Reset this compound
                    substanceInstance.Compound = null;

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

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasSubstance(SubstanceBase substanceBase)
        /// <summary>
        /// Checks if this compound has a substance instance of the given substance.
        /// </summary>
        /// <param name="substanceBase">The substance to check.</param>
        /// <returns>Returns true when the compound has a substance instance of the given substance.</returns>
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
        /// Check whether the compound instance satisfies the given condition.
        /// </summary>
        /// <param name="conditionBase">The compound condition to compare to the compound instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the compound instance satisfies the given condition.</returns>
        public override bool Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (conditionBase != null)
            {
                // Check whether the base satisfies the condition
                if (base.Satisfies(conditionBase, iVariableInstanceHolder))
                {
                    // Compound condition
                    CompoundConditionBase compoundConditionBase = conditionBase as CompoundConditionBase;
                    if (compoundConditionBase != null)
                    {
                        // Check whether the instance has all mandatory substances
                        if (compoundConditionBase.HasAllMandatorySubstances == true)
                        {
                            foreach (SubstanceValuedBase substanceValuedBase in this.CompoundBase.Substances)
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
                        foreach (SubstanceConditionBase substanceConditionBase in compoundConditionBase.Substances)
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
        /// Apply the given change to the compound instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the compound instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        protected internal override bool Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (changeBase != null)
            {
                if (base.Apply(changeBase, iVariableInstanceHolder))
                {
                    // Compound change
                    CompoundChangeBase compoundChangeBase = changeBase as CompoundChangeBase;
                    if (compoundChangeBase != null)
                    {
                        // Change the substances
                        foreach (SubstanceChangeBase substanceChangeBase in compoundChangeBase.Substances)
                        {
                            foreach (SubstanceInstance substanceInstance in this.Substances)
                                substanceInstance.Apply(substanceChangeBase, iVariableInstanceHolder);
                        }

                        // Add the substances
                        foreach (SubstanceValuedBase substanceValuedBase in compoundChangeBase.SubstancesToAdd)
                            AddSubstance(InstanceManager.Current.Create<SubstanceInstance>(substanceValuedBase));

                        // Remove the substances
                        foreach (SubstanceConditionBase substanceConditionBase in compoundChangeBase.SubstancesToRemove)
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
    #endregion Class: CompoundInstance

}