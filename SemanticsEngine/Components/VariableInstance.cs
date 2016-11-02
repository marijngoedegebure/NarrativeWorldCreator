/**************************************************************************
 * 
 * VariableInstance.cs
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
using Semantics.Components;
using Semantics.Utilities;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Entities;
using SemanticsEngine.Interfaces;
using SemanticsEngine.Tools;
using ValueType = Semantics.Utilities.ValueType;

namespace SemanticsEngine.Components
{

    #region Class: VariableInstance
    /// <summary>
    /// An instance of a variable.
    /// </summary>
    public abstract class VariableInstance : Instance
    {

        #region Properties and Fields

        #region Property: VariableBase
        /// <summary>
        /// Gets the variable base of which this is a variable instance.
        /// </summary>
        public VariableBase VariableBase
        {
            get
            {
                return this.Base as VariableBase;
            }
        }
        #endregion Property: VariableBase

        #region Property: VariableInstanceHolder
        /// <summary>
        /// The variable instance holder.
        /// </summary>
        private IVariableInstanceHolder iVariableInstanceHolder = null;

        /// <summary>
        /// Gets the variable instance holder.
        /// </summary>
        public IVariableInstanceHolder VariableInstanceHolder
        {
            get
            {
                return iVariableInstanceHolder;
            }
        }
        #endregion Property: VariableInstanceHolder

        #region Property: Actor
        /// <summary>
        /// Gets the actor.
        /// </summary>
        public EntityInstance Actor
        {
            get
            {
                if (this.VariableInstanceHolder != null)
                    return this.VariableInstanceHolder.GetActor();
                return null;
            }
        }
        #endregion Property: Actor

        #region Property: Target
        /// <summary>
        /// Gets the target.
        /// </summary>
        public EntityInstance Target
        {
            get
            {
                if (this.VariableInstanceHolder != null)
                    return this.VariableInstanceHolder.GetTarget();
                return null;
            }
        }
        #endregion Property: Target

        #region Property: Artifact
        /// <summary>
        /// Gets the artifact.
        /// </summary>
        public EntityInstance Artifact
        {
            get
            {
                if (this.VariableInstanceHolder != null)
                    return this.VariableInstanceHolder.GetArtifact();
                return null;
            }
        }
        #endregion Property: Artifact

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: VariableInstance(VariableBase variableBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Create a new variable instance from the given variable base.
        /// </summary>
        /// <param name="variableBase">The variable base to create a variable instance from.</param>
        /// <param name="iVariableInstanceHolder">The variable and reference holder that will own the bool variable instance.</param>
        protected VariableInstance(VariableBase variableBase, IVariableInstanceHolder iVariableInstanceHolder)
            : base(variableBase)
        {
            this.iVariableInstanceHolder = iVariableInstanceHolder;
        }
        #endregion Constructor: VariableInstance(VariableBase variableInstance, IVariableInstanceHolder iVariableInstanceHolder)

        #region Constructor: VariableInstance(VariableInstance variableInstance)
        /// <summary>
        /// Clones the variable instance.
        /// </summary>
        /// <param name="variableInstance">The variable instance to clone.</param>
        protected VariableInstance(VariableInstance variableInstance)
            : base(variableInstance)
        {
            if (variableInstance != null)
                this.iVariableInstanceHolder = variableInstance.VariableInstanceHolder;
        }
        #endregion Constructor: VariableInstance(VariableInstance variableInstance)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: GetValue<T>()
        /// <summary>
        /// Get the value of the variable.
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve.</typeparam>
        /// <returns>The value of the variable.</returns>
        protected T GetValue<T>()
        {
            if (this.VariableBase != null)
            {
                switch (this.VariableBase.VariableType)
                {
                    #region Attribute

                    // Get the attribute value of the source
                    case VariableType.Attribute:
                        if (this.VariableBase.Attribute != null)
                        {
                            ValueInstance valueInstance = null;

                            switch (this.VariableBase.Source)
                            {
                                case ActorTargetArtifactReference.Actor:
                                    if (this.Actor != null)
                                    {
                                        ValueInstance valueOfAttribute = this.Actor.GetValueOfAttribute(this.VariableBase.Attribute);
                                        if (valueOfAttribute != null)
                                            valueInstance = valueOfAttribute;
                                    }
                                    break;

                                case ActorTargetArtifactReference.Target:
                                    if (this.Target != null)
                                    {
                                        ValueInstance valueOfAttribute = this.Target.GetValueOfAttribute(this.VariableBase.Attribute);
                                        if (valueOfAttribute != null)
                                            valueInstance = valueOfAttribute;
                                    }
                                    break;

                                case ActorTargetArtifactReference.Artifact:
                                    if (this.Artifact != null)
                                    {
                                        ValueInstance valueOfAttribute = this.Artifact.GetValueOfAttribute(this.VariableBase.Attribute);
                                        if (valueOfAttribute != null)
                                            valueInstance = valueOfAttribute;
                                    }
                                    break;

                                case ActorTargetArtifactReference.Reference:
                                    if (this.VariableBase.Reference != null)
                                    {
                                        ReadOnlyCollection<EntityInstance> referencedEntities = this.VariableBase.Reference.GetEntities(this.VariableInstanceHolder);
                                        if (referencedEntities.Count > 0)
                                        {
                                            ValueInstance valueOfAttribute = referencedEntities[0].GetValueOfAttribute(this.VariableBase.Attribute);
                                            if (valueOfAttribute != null)
                                                valueInstance = valueOfAttribute;
                                        }
                                    }
                                    break;

                                default:
                                    break;
                            }

                            if (valueInstance != null)
                            {
                                BoolValueInstance boolValueInstance = valueInstance as BoolValueInstance;
                                if (boolValueInstance != null)
                                    return (T)(object)boolValueInstance.Bool;
                                NumericalValueInstance numericalValueInstance = valueInstance as NumericalValueInstance;
                                if (numericalValueInstance != null)
                                    return (T)(object)numericalValueInstance.BaseValue;
                                StringValueInstance stringValueInstance = valueInstance as StringValueInstance;
                                if (stringValueInstance != null)
                                    return (T)(object)stringValueInstance.String;
                                VectorValueInstance vectorValueInstance = valueInstance as VectorValueInstance;
                                if (vectorValueInstance != null)
                                    return (T)(object)vectorValueInstance.Vector;
                            }
                        }
                        break;

                    #endregion Attribute

                    #region Count

                    // Get the count of the countable entities
                    case VariableType.Count:
                        if (typeof(T) == typeof(float))
                        {
                            int count = 0;
                            switch (this.VariableBase.Source)
                            {
                                case ActorTargetArtifactReference.Actor:
                                    if (this.Actor != null)
                                        count += GetCount(this.Actor, this.VariableBase.CountType, this.VariableBase.CountableEntity);
                                    break;

                                case ActorTargetArtifactReference.Target:
                                    if (this.Target != null)
                                        count += GetCount(this.Target, this.VariableBase.CountType, this.VariableBase.CountableEntity);
                                    break;

                                case ActorTargetArtifactReference.Artifact:
                                    if (this.Artifact != null)
                                        count += GetCount(this.Artifact, this.VariableBase.CountType, this.VariableBase.CountableEntity);
                                    break;

                                case ActorTargetArtifactReference.Reference:
                                    if (this.VariableBase.Reference != null)
                                    {
                                        foreach (EntityInstance referencedEntity in this.VariableBase.Reference.GetEntities(this.VariableInstanceHolder))
                                            count += GetCount(referencedEntity, this.VariableBase.CountType, this.VariableBase.CountableEntity);
                                    }
                                    break;

                                default:
                                    break;
                            }
                            return (T)(object)(float)count;
                        }
                        break;

                    #endregion Count

                    #region Quantity

                    // Get the quantity of the matter
                    case VariableType.Quantity:
                        if (typeof(T) == typeof(float) && this.VariableBase.Matter != null)
                        {
                            NumericalValueInstance quantity = new NumericalValueInstance(0);
                            switch (this.VariableBase.Source)
                            {
                                case ActorTargetArtifactReference.Actor:
                                    if (this.Actor != null)
                                    {
                                        TangibleObjectInstance tangibleActor = this.Actor as TangibleObjectInstance;
                                        if (tangibleActor != null)
                                        {
                                            foreach (MatterInstance matter in tangibleActor.Matter)
                                            {
                                                if (matter.IsNodeOf(this.VariableBase.Matter))
                                                    quantity += matter.Quantity;
                                            }
                                        }
                                    }
                                    break;

                                case ActorTargetArtifactReference.Target:
                                    if (this.Target != null)
                                    {
                                        TangibleObjectInstance tangibleTarget = this.Target as TangibleObjectInstance;
                                        if (tangibleTarget != null)
                                        {
                                            foreach (MatterInstance matter in tangibleTarget.Matter)
                                            {
                                                if (matter.IsNodeOf(this.VariableBase.Matter))
                                                    quantity += matter.Quantity;
                                            }
                                        }
                                    }
                                    break;

                                case ActorTargetArtifactReference.Artifact:
                                    if (this.Artifact != null)
                                    {
                                        TangibleObjectInstance tangibleArtifact = this.Artifact as TangibleObjectInstance;
                                        if (tangibleArtifact != null)
                                        {
                                            foreach (MatterInstance matter in tangibleArtifact.Matter)
                                            {
                                                if (matter.IsNodeOf(this.VariableBase.Matter))
                                                    quantity += matter.Quantity;
                                            }
                                        }
                                    }
                                    break;

                                case ActorTargetArtifactReference.Reference:
                                    if (this.VariableBase.Reference != null)
                                    {
                                        foreach (EntityInstance referencedEntity in this.VariableBase.Reference.GetEntities(this.VariableInstanceHolder))
                                        {
                                            TangibleObjectInstance tangibleObjectInstance = referencedEntity as TangibleObjectInstance;
                                            if (tangibleObjectInstance != null)
                                            {
                                                foreach (MatterInstance matter in tangibleObjectInstance.Matter)
                                                {
                                                    if (matter.IsNodeOf(this.VariableBase.Matter))
                                                        quantity += matter.Quantity;
                                                }
                                            }

                                            MatterInstance matterInstance = referencedEntity as MatterInstance;
                                            if (matterInstance != null && matterInstance.IsNodeOf(this.VariableBase.Matter))
                                                quantity += matterInstance.Quantity;
                                        }
                                    }
                                    break;

                                default:
                                    break;
                            }
                            return (T)(object)quantity.BaseValue;
                        }
                        break;

                    #endregion Quantity

                    #region Sum

                    // Get the sum of the attribute values
                    case VariableType.Sum:
                        if (typeof(T) == typeof(float))
                        {
                            float sum = 0;
                            switch (this.VariableBase.Source)
                            {
                                case ActorTargetArtifactReference.Actor:
                                    if (this.Actor != null)
                                        sum += GetSum(this.Actor, this.VariableBase.Attribute);
                                    break;

                                case ActorTargetArtifactReference.Target:
                                    if (this.Target != null)
                                        sum += GetSum(this.Actor, this.VariableBase.Attribute);
                                    break;

                                case ActorTargetArtifactReference.Artifact:
                                    if (this.Artifact != null)
                                        sum += GetSum(this.Artifact, this.VariableBase.Attribute);
                                    break;

                                case ActorTargetArtifactReference.Reference:
                                    if (this.VariableBase.Reference != null)
                                    {
                                        foreach (EntityInstance referencedEntity in this.VariableBase.Reference.GetEntities(this.VariableInstanceHolder))
                                            sum += GetSum(this.Actor, this.VariableBase.Attribute);
                                    }
                                    break;

                                default:
                                    break;
                            }
                            return (T)(object)(float)sum;
                        }
                        break;

                    #endregion Sum

                    #region Distance

                    // Get the distance between actor and target
                    case VariableType.Distance:
                        if (typeof(T) == typeof(float))
                        {
                            float distance = 0;
                            PhysicalEntityInstance actorInstance = this.Actor as PhysicalEntityInstance;
                            if (actorInstance != null)
                            {
                                switch (this.VariableBase.Source)
                                {
                                    case ActorTargetArtifactReference.Actor:
                                    case ActorTargetArtifactReference.Target:
                                        PhysicalEntityInstance targetInstance = this.Target as PhysicalEntityInstance;
                                        if (targetInstance != null)
                                            distance = Vec3.Distance(actorInstance.Position, targetInstance.Position);

                                        break;

                                    case ActorTargetArtifactReference.Artifact:
                                        PhysicalEntityInstance artifactInstance = this.Artifact as PhysicalEntityInstance;
                                        if (artifactInstance != null)
                                            distance = Vec3.Distance(actorInstance.Position, artifactInstance.Position);
                                        break;

                                    case ActorTargetArtifactReference.Reference:
                                        if (this.VariableBase.Reference != null)
                                        {
                                            foreach (EntityInstance referencedEntity in this.VariableBase.Reference.GetEntities(this.VariableInstanceHolder))
                                            {
                                                PhysicalEntityInstance referenceInstance = referencedEntity as PhysicalEntityInstance;
                                                if (referenceInstance != null)
                                                {
                                                    distance = Vec3.Distance(actorInstance.Position, referenceInstance.Position);
                                                    break;
                                                }
                                            }
                                        }
                                        break;

                                    default:
                                        break;
                                }
                            }
                            return (T)(object)(float)distance;
                        }

                        break;

                    #endregion Distance

                    default:
                        break;
                }
            }

            return default(T);
        }
        #endregion Method: GetValue<T>()

        #region Method: GetCount(EntityInstance source, CountType countType, EntityBase countableEntity)
        /// <summary>
        /// Get the count of the given type and possible entity from the source.
        /// </summary>
        /// <param name="source">The source in which should be counted.</param>
        /// <param name="countType">Indicates what should be counted.</param>
        /// <param name="countableEntity">The entity to count.</param>
        /// <returns>The count of the type and possible entity from the source.</returns>
        private int GetCount(EntityInstance source, CountType countType, EntityBase countableEntity)
        {
            int count = 0;

            PhysicalObjectInstance physicalSource = source as PhysicalObjectInstance;
            if (physicalSource != null)
            {
                if (countType == CountType.Everything || countType == CountType.Spaces)
                {
                    foreach (PhysicalObjectInstance space in physicalSource.Spaces)
                    {
                        if (countableEntity == null || space.IsNodeOf(countableEntity))
                            count++;
                    }
                }
            }

            if (countType == CountType.Everything || countType == CountType.SpaceItems)
            {
                if (physicalSource != null)
                {
                    foreach (TangibleObjectInstance spaceItem in physicalSource.SpaceItems)
                    {
                        if (countableEntity == null || spaceItem.IsNodeOf(countableEntity))
                            count++;
                    }
                }
                SpaceInstance spaceInstance = source as SpaceInstance;
                if (spaceInstance != null)
                {
                    foreach (TangibleObjectInstance spaceItem in spaceInstance.Items)
                    {
                        if (countableEntity == null || spaceItem.IsNodeOf(countableEntity))
                            count++;
                    }
                }
            }

            TangibleObjectInstance tangibleSource = source as TangibleObjectInstance;
            if (tangibleSource != null)
            {
                if (countType == CountType.Everything || countType == CountType.Parts)
                {
                    foreach (TangibleObjectInstance part in tangibleSource.Parts)
                    {
                        if (countableEntity == null || part.IsNodeOf(countableEntity))
                            count++;
                    }
                }
                if (countType == CountType.Everything || countType == CountType.Covers)
                {
                    foreach (TangibleObjectInstance cover in tangibleSource.Covers)
                    {
                        if (countableEntity == null || cover.IsNodeOf(countableEntity))
                            count++;
                    }
                }
                if (countType == CountType.Everything || countType == CountType.Connections)
                {
                    foreach (TangibleObjectInstance connection in tangibleSource.Connections)
                    {
                        if (countableEntity == null || connection.IsNodeOf(countableEntity))
                            count++;
                    }
                }
                if (countType == CountType.Everything || countType == CountType.Layers)
                {
                    foreach (MatterInstance layer in tangibleSource.Layers)
                    {
                        if (countableEntity == null || layer.IsNodeOf(countableEntity))
                            count++;
                    }
                }
            }

            return count;
        }
        #endregion Method: GetCount(EntityInstance source, CountType countType, EntityBase countableEntity)

        #region Method: GetSum(EntityInstance source, AttributeBase attribute)
        /// <summary>
        /// Get the sum of the attribute values of the given attribute in the source.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="attribute">The attribute to look for.</param>
        /// <returns>The sum of the attribute values of the given attribute in the source.</returns>
        private float GetSum(EntityInstance source, AttributeBase attribute)
        {
            float sum = 0;

            // Add the attribute value of the source
            NumericalValueInstance numericalValueInstance = source.GetValueOfAttribute(attribute) as NumericalValueInstance;
            if (numericalValueInstance != null)
                sum += numericalValueInstance.BaseValue;

            // Add the attribute value of the spaces
            PhysicalObjectInstance physicalSource = source as PhysicalObjectInstance;
            if (physicalSource != null)
            {
                foreach (PhysicalObjectInstance space in physicalSource.Spaces)
                    sum += GetSum(space, attribute);
            }

            // Add the attribute value of the items
            SpaceInstance spaceInstance = source as SpaceInstance;
            if (spaceInstance != null)
            {
                foreach (TangibleObjectInstance spaceItem in spaceInstance.Items)
                    sum += GetSum(spaceItem, attribute);
            }

            // Add the attribute value of the parts, covers, layers
            TangibleObjectInstance tangibleSource = source as TangibleObjectInstance;
            if (tangibleSource != null)
            {
                foreach (TangibleObjectInstance part in tangibleSource.Parts)
                    sum += GetSum(part, attribute);

                foreach (TangibleObjectInstance cover in tangibleSource.Covers)
                    sum += GetSum(cover, attribute);

                foreach (MatterInstance layer in tangibleSource.Layers)
                    sum += GetSum(layer, attribute);
            }

            return sum;
        }
        #endregion Method: GetSum(EntityInstance source, AttributeBase attribute)
		
        #region Method: Clone()
        /// <summary>
        /// Clones the variable instance.
        /// </summary>
        /// <returns>A clone of the variable instance.</returns>
        public new VariableInstance Clone()
        {
            return base.Clone() as VariableInstance;
        }
        #endregion Method: Clone()

        #endregion Method Group: Other

    }
    #endregion Class: VariableInstance

    #region Class: BoolVariableInstance
    /// <summary>
    /// An instance of a bool variable.
    /// </summary>
    public class BoolVariableInstance : VariableInstance
    {

        #region Properties and Fields

        #region Property: BoolVariableBase
        /// <summary>
        /// Gets the bool variable base of which this is a bool variable instance.
        /// </summary>
        public BoolVariableBase BoolVariableBase
        {
            get
            {
                return this.Base as BoolVariableBase;
            }
        }
        #endregion Property: BoolVariableBase

        #region Property: Bool
        /// <summary>
        /// Gets the boolean value.
        /// </summary>
        public bool Bool
        {
            get
            {
                if (this.BoolVariableBase.VariableType == VariableType.Fixed)
                {
                    // Return the fixed value
                    return this.BoolVariableBase.FixedValue;
                }
                else if (this.BoolVariableBase.VariableType == VariableType.RequiresManualInput)
                {
                    if (this.VariableInstanceHolder != null)
                    {
                        Dictionary<string, object> manualInput = this.VariableInstanceHolder.GetManualInput();
                        if (manualInput != null)
                        {
                            // Get the correct manual input
                            object correctInput = manualInput[this.BoolVariableBase.Name];
                            if (correctInput == null)
                                correctInput = SemanticsEngine.Current.GetManualInput(this.BoolVariableBase.Name, ValueType.Boolean);
                            if (correctInput != null)
                            {
                                if (correctInput is bool)
                                    return (bool)correctInput;
                                else if (correctInput is BoolValueInstance)
                                    return ((BoolValueInstance)correctInput).Bool;
                                else if (correctInput is BoolValueBase)
                                    return ((BoolValueBase)correctInput).Bool;
                                else if (correctInput is BoolValue)
                                    return (BaseManager.Current.GetBase<BoolValueBase>((BoolValue)correctInput)).Bool;
                            }
                        }
                    }
                }
                
                return base.GetValue<bool>();
            }
        }
        #endregion Property: Bool
        
        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: BoolVariableInstance(BoolVariableBase boolVariableBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Create a new bool variable instance from the given bool variable base.
        /// </summary>
        /// <param name="boolVariableBase">The bool variable base to create a bool variable instance from.</param>
        /// <param name="iVariableInstanceHolder">The variable and reference holder that will own the bool variable instance.</param>
        public BoolVariableInstance(BoolVariableBase boolVariableBase, IVariableInstanceHolder iVariableInstanceHolder)
            : base(boolVariableBase, iVariableInstanceHolder)
        {
        }
        #endregion Constructor: BoolVariableInstance(BoolVariableBase boolVariableBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Constructor: BoolVariableInstance(BoolVariableInstance boolVariableInstance)
        /// <summary>
        /// Clones the bool variable instance.
        /// </summary>
        /// <param name="boolVariableInstance">The bool variable instance to clone.</param>
        public BoolVariableInstance(BoolVariableInstance boolVariableInstance)
            : base(boolVariableInstance)
        {
        }
        #endregion Constructor: BoolVariableInstance(BoolVariableInstance boolVariableInstance)

        #endregion Method Group: Constructors

    }
    #endregion Class: BoolVariableInstance

    #region Class: NumericalVariableInstance
    /// <summary>
    /// An instance of a numerical variable.
    /// </summary>
    public class NumericalVariableInstance : VariableInstance
    {

        #region Properties and Fields

        #region Property: NumericalVariableBase
        /// <summary>
        /// Gets the numerical variable base of which this is a numerical variable instance.
        /// </summary>
        public NumericalVariableBase NumericalVariableBase
        {
            get
            {
                return this.Base as NumericalVariableBase;
            }
        }
        #endregion Property: NumericalVariableBase

        #region Property: Value
        /// <summary>
        /// Gets the numerical value.
        /// </summary>
        public float Value
        {
            get
            {
                if (this.NumericalVariableBase.VariableType == VariableType.Fixed)
                {
                    // Return the fixed value
                    return this.NumericalVariableBase.FixedValue;
                }
                else if (this.NumericalVariableBase.VariableType == VariableType.RequiresManualInput)
                {
                    if (this.VariableInstanceHolder != null)
                    {
                        Dictionary<string, object> manualInput = this.VariableInstanceHolder.GetManualInput();
                        if (manualInput != null)
                        {
                            // Get the correct manual input
                            object correctInput = manualInput[this.NumericalVariableBase.Name];
                            if (correctInput == null)
                                correctInput = SemanticsEngine.Current.GetManualInput(this.NumericalVariableBase.Name, ValueType.Numerical);
                            if (correctInput != null)
                            {
                                if (correctInput is float)
                                    return (float)correctInput;
                                else if (correctInput is int)
                                    return Convert.ToSingle((int)correctInput);
                                else if (correctInput is uint)
                                    return Convert.ToSingle((uint)correctInput);
                                else if (correctInput is double)
                                    return Convert.ToSingle((double)correctInput);
                                else if (correctInput is byte)
                                    return Convert.ToSingle((byte)correctInput);
                                else if (correctInput is short)
                                    return Convert.ToSingle((short)correctInput);
                                else if (correctInput is long)
                                    return Convert.ToSingle((long)correctInput);
                                else if (correctInput is NumericalValueInstance)
                                    return ((NumericalValueInstance)correctInput).BaseValue;
                                else if (correctInput is NumericalValueBase)
                                    return ((NumericalValueBase)correctInput).BaseValue;
                                else if (correctInput is NumericalValue)
                                    return (BaseManager.Current.GetBase<NumericalValueBase>((NumericalValue)correctInput)).BaseValue;
                            }
                        }
                    }
                }
                
                return base.GetValue<float>();
            }
        }
        #endregion Property: Value

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: NumericalVariableInstance(NumericalVariableBase numericalVariableBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Create a new numerical variable instance from the given numerical variable base.
        /// </summary>
        /// <param name="numericalVariableBase">The numerical variable base to create a numerical variable instance from.</param>
        /// <param name="iVariableInstanceHolder">The variable and reference holder that will own the numerical variable instance.</param>
        public NumericalVariableInstance(NumericalVariableBase numericalVariableBase, IVariableInstanceHolder iVariableInstanceHolder)
            : base(numericalVariableBase, iVariableInstanceHolder)
        {
        }
        #endregion Constructor: NumericalVariableInstance(NumericalVariableBase numericalVariableBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Constructor: NumericalVariableInstance(NumericalVariableInstance numericalVariableInstance)
        /// <summary>
        /// Clones the numerical variable instance.
        /// </summary>
        /// <param name="numericalVariableInstance">The numerical variable instance to clone.</param>
        public NumericalVariableInstance(NumericalVariableInstance numericalVariableInstance)
            : base(numericalVariableInstance)
        {
        }
        #endregion Constructor: NumericalVariableInstance(NumericalVariableInstance numericalVariableInstance)

        #endregion Method Group: Constructors

    }
    #endregion Class: NumericalVariableInstance

    #region Class: StringVariableInstance
    /// <summary>
    /// An instance of a string variable.
    /// </summary>
    public class StringVariableInstance : VariableInstance
    {

        #region Properties and Fields

        #region Property: StringVariableBase
        /// <summary>
        /// Gets the string variable base of which this is a string variable instance.
        /// </summary>
        public StringVariableBase StringVariableBase
        {
            get
            {
                return this.Base as StringVariableBase;
            }
        }
        #endregion Property: StringVariableBase

        #region Property: String
        /// <summary>
        /// Gets the string value.
        /// </summary>
        public string String
        {
            get
            {
                if (this.StringVariableBase.VariableType == VariableType.Fixed)
                {
                    // Return the fixed value
                    return this.StringVariableBase.FixedValue;
                }
                else if (this.StringVariableBase.VariableType == VariableType.RequiresManualInput)
                {
                    if (this.VariableInstanceHolder != null)
                    {
                        Dictionary<string, object> manualInput = this.VariableInstanceHolder.GetManualInput();
                        if (manualInput != null)
                        {
                            // Get the correct manual input
                            object correctInput = manualInput[this.StringVariableBase.Name];
                            if (correctInput == null)
                                correctInput = SemanticsEngine.Current.GetManualInput(this.StringVariableBase.Name, ValueType.String);
                            if (correctInput != null)
                            {
                                if (correctInput is string)
                                    return (string)correctInput;
                                else if (correctInput is char)
                                    return ((char)correctInput).ToString();
                                else if (correctInput is StringValueInstance)
                                    return ((StringValueInstance)correctInput).String;
                                else if (correctInput is StringValueBase)
                                    return ((StringValueBase)correctInput).String;
                                else if (correctInput is StringValue)
                                    return (BaseManager.Current.GetBase<StringValueBase>((StringValue)correctInput)).String;
                            }
                        }
                    }
                }
                
                return base.GetValue<string>();
            }
        }
        #endregion Property: String

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: StringVariableInstance(StringVariableBase stringVariableBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Create a new string variable instance from the given string variable base.
        /// </summary>
        /// <param name="stringVariableBase">The string variable base to create a string variable instance from.</param>
        /// <param name="iVariableInstanceHolder">The variable and reference holder that will own the string variable instance.</param>
        public StringVariableInstance(StringVariableBase stringVariableBase, IVariableInstanceHolder iVariableInstanceHolder)
            : base(stringVariableBase, iVariableInstanceHolder)
        {
        }
        #endregion Constructor: StringVariableInstance(StringVariableBase stringVariableBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Constructor: StringVariableInstance(StringVariableInstance stringVariableInstance)
        /// <summary>
        /// Clones the string variable instance.
        /// </summary>
        /// <param name="stringVariableInstance">The string variable instance to clone.</param>
        public StringVariableInstance(StringVariableInstance stringVariableInstance)
            : base(stringVariableInstance)
        {
        }
        #endregion Constructor: StringVariableInstance(StringVariableInstance stringVariableInstance)

        #endregion Method Group: Constructors

    }
    #endregion Class: StringVariableInstance

    #region Class: VectorVariableInstance
    /// <summary>
    /// An instance of a vector variable.
    /// </summary>
    public class VectorVariableInstance : VariableInstance
    {

        #region Properties and Fields

        #region Property: VectorVariableBase
        /// <summary>
        /// Gets the vector variable base of which this is a vector variable instance.
        /// </summary>
        public VectorVariableBase VectorVariableBase
        {
            get
            {
                return this.Base as VectorVariableBase;
            }
        }
        #endregion Property: VectorVariableBase

        #region Property: Vector
        /// <summary>
        /// Gets the vector value.
        /// </summary>
        public Vec4 Vector
        {
            get
            {
                if (this.VectorVariableBase.VariableType == VariableType.Fixed)
                {
                    // Return the fixed value
                    return this.VectorVariableBase.FixedValue;
                }
                else if (this.VectorVariableBase.VariableType == VariableType.RequiresManualInput)
                {
                    if (this.VariableInstanceHolder != null)
                    {
                        Dictionary<string, object> manualInput = this.VariableInstanceHolder.GetManualInput();
                        if (manualInput != null)
                        {
                            // Get the correct manual input
                            object correctInput = manualInput[this.VectorVariableBase.Name];
                            if (correctInput == null)
                                correctInput = SemanticsEngine.Current.GetManualInput(this.VectorVariableBase.Name, ValueType.Vector);
                            if (correctInput != null)
                            {
                                if (correctInput is Vec4)
                                    return (Vec4)correctInput;
                                else if (correctInput is Vec3)
                                    return new Vec4((Vec3)correctInput);
                                else if (correctInput is Vec2)
                                    return new Vec4((Vec2)correctInput);
                                else if (correctInput is VectorValueInstance)
                                    return ((VectorValueInstance)correctInput).Vector;
                                else if (correctInput is VectorValueBase)
                                    return ((VectorValueBase)correctInput).Vector;
                                else if (correctInput is VectorValue)
                                    return (BaseManager.Current.GetBase<VectorValueBase>((VectorValue)correctInput)).Vector;
                            }
                        }
                    }
                }
                
                return base.GetValue<Vec4>();
            }
        }
        #endregion Property: Vector

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: VectorVariableInstance(VectorVariableBase vectorVariableBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Create a new vector variable instance from the given vector variable base.
        /// </summary>
        /// <param name="vectorVariableBase">The vector variable base to create a vector variable instance from.</param>
        /// <param name="iVariableInstanceHolder">The variable and reference holder that will own the vector variable instance.</param>
        public VectorVariableInstance(VectorVariableBase vectorVariableBase, IVariableInstanceHolder iVariableInstanceHolder)
            : base(vectorVariableBase, iVariableInstanceHolder)
        {
        }
        #endregion Constructor: VectorVariableInstance(VectorVariableBase vectorVariableBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Constructor: VectorVariableInstance(VectorVariableInstance vectorVariableInstance)
        /// <summary>
        /// Clones the vector variable instance.
        /// </summary>
        /// <param name="vectorVariableInstance">The vector variable instance to clone.</param>
        public VectorVariableInstance(VectorVariableInstance vectorVariableInstance)
            : base(vectorVariableInstance)
        {
        }
        #endregion Constructor: VectorVariableInstance(VectorVariableInstance vectorVariableInstance)

        #endregion Method Group: Constructors

    }
    #endregion Class: VectorVariableInstance

    #region Class: RandomVariableInstance
    /// <summary>
    /// An instance of a random variable.
    /// </summary>
    public class RandomVariableInstance : VariableInstance
    {

        #region Properties and Fields

        #region Property: RandomVariableBase
        /// <summary>
        /// Gets the random variable base of which this is a random variable instance.
        /// </summary>
        public RandomVariableBase RandomVariableBase
        {
            get
            {
                return this.Base as RandomVariableBase;
            }
        }
        #endregion Property: RandomVariableBase

        #region Property: Value
        /// <summary>
        /// Gets the random value.
        /// </summary>
        public float Value
        {
            get
            {
                if (this.RandomVariableBase.VariableType == VariableType.Fixed)
                {
                    // Return the fixed value
                    return this.RandomVariableBase.FixedValue;
                }
                else if (this.RandomVariableBase.VariableType == VariableType.RequiresManualInput)
                {
                    if (this.VariableInstanceHolder != null)
                    {
                        Dictionary<string, object> manualInput = this.VariableInstanceHolder.GetManualInput();
                        if (manualInput != null)
                        {
                            // Get the correct manual input
                            object correctInput = manualInput[this.RandomVariableBase.Name];
                            if (correctInput == null)
                                correctInput = SemanticsEngine.Current.GetManualInput(this.RandomVariableBase.Name, ValueType.Numerical);
                            if (correctInput != null)
                            {
                                if (correctInput is float)
                                    return (float)correctInput;
                                else if (correctInput is int)
                                    return Convert.ToSingle((int)correctInput);
                                else if (correctInput is uint)
                                    return Convert.ToSingle((uint)correctInput);
                                else if (correctInput is double)
                                    return Convert.ToSingle((double)correctInput);
                                else if (correctInput is byte)
                                    return Convert.ToSingle((byte)correctInput);
                                else if (correctInput is short)
                                    return Convert.ToSingle((short)correctInput);
                                else if (correctInput is long)
                                    return Convert.ToSingle((long)correctInput);
                            }
                        }
                    }
                }
                
                return base.GetValue<float>();
            }
        }
        #endregion Property: Value

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: RandomVariableInstance(RandomVariableBase randomVariableBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Create a new random variable instance from the given random variable base.
        /// </summary>
        /// <param name="randomVariableBase">The random variable base to create a random variable instance from.</param>
        /// <param name="iVariableInstanceHolder">The variable and reference holder that will own the random variable instance.</param>
        public RandomVariableInstance(RandomVariableBase randomVariableBase, IVariableInstanceHolder iVariableInstanceHolder)
            : base(randomVariableBase, iVariableInstanceHolder)
        {
        }
        #endregion Constructor: RandomVariableInstance(RandomVariableBase randomVariableBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Constructor: RandomVariableInstance(RandomVariableInstance randomVariableInstance)
        /// <summary>
        /// Clones the random variable instance.
        /// </summary>
        /// <param name="randomVariableInstance">The random variable instance to clone.</param>
        public RandomVariableInstance(RandomVariableInstance randomVariableInstance)
            : base(randomVariableInstance)
        {
        }
        #endregion Constructor: RandomVariableInstance(RandomVariableInstance randomVariableInstance)

        #endregion Method Group: Constructors

    }
    #endregion Class: RandomVariableInstance

    #region Class: TermVariableInstance
    /// <summary>
    /// An instance of a term variable.
    /// </summary>
    public class TermVariableInstance : VariableInstance
    {

        #region Properties and Fields

        #region Property: TermVariableBase
        /// <summary>
        /// Gets the term variable base of which this is a term variable instance.
        /// </summary>
        public TermVariableBase TermVariableBase
        {
            get
            {
                return this.Base as TermVariableBase;
            }
        }
        #endregion Property: TermVariableBase

        #region Property: Value
        /// <summary>
        /// Gets the term value.
        /// </summary>
        public float Value
        {
            get
            {
                if (this.TermVariableBase.VariableType == VariableType.Fixed)
                {
                    // Calculate the first value
                    float val1 = 0;
                    if (this.TermVariableBase.Variable1 != null)
                    {
                        if (this.TermVariableBase.Variable1 is NumericalVariableBase)
                            val1 = new NumericalVariableInstance((NumericalVariableBase)this.TermVariableBase.Variable1, this.VariableInstanceHolder).Value;
                        else if (this.TermVariableBase.Variable1 is TermVariableBase)
                            val1 = new TermVariableInstance((TermVariableBase)this.TermVariableBase.Variable1, this.VariableInstanceHolder).Value;
                    }
                    else if (this.TermVariableBase.Value1 != null)
                        val1 = (float)this.TermVariableBase.Value1;
                    
                    if (this.TermVariableBase.Function1 != null)
                        val1 = Toolbox.Calculate((Function)this.TermVariableBase.Function1, val1);

                    // Possibly combine it with the second calculated value
                    if (this.TermVariableBase.Operator != null)
                    {
                        float val2 = 0;
                        if (this.TermVariableBase.Variable2 != null)
                        {
                            if (this.TermVariableBase.Variable2 is NumericalVariableBase)
                                val2 = new NumericalVariableInstance((NumericalVariableBase)this.TermVariableBase.Variable2, this.VariableInstanceHolder).Value;
                            else if (this.TermVariableBase.Variable2 is TermVariableBase)
                                val2 = new TermVariableInstance((TermVariableBase)this.TermVariableBase.Variable2, this.VariableInstanceHolder).Value;
                        }
                        else if (this.TermVariableBase.Value2 != null)
                            val2 = (float)this.TermVariableBase.Value2;

                        if (this.TermVariableBase.Function2 != null)
                            val2 = Toolbox.Calculate((Function)this.TermVariableBase.Function2, val2);

                        return Toolbox.Calculate(val1, (Operator)this.TermVariableBase.Operator, val2);
                    }

                    return val1;
                }
                else if (this.TermVariableBase.VariableType == VariableType.RequiresManualInput)
                {
                    if (this.VariableInstanceHolder != null)
                    {
                        Dictionary<string, object> manualInput = this.VariableInstanceHolder.GetManualInput();
                        if (manualInput != null)
                        {
                            // Get the correct manual input
                            object correctInput = manualInput[this.TermVariableBase.Name];
                            if (correctInput == null)
                                correctInput = SemanticsEngine.Current.GetManualInput(this.TermVariableBase.Name, ValueType.Numerical);
                            if (correctInput != null)
                            {
                                if (correctInput is float)
                                    return (float)correctInput;
                                else if (correctInput is int)
                                    return Convert.ToSingle((int)correctInput);
                                else if (correctInput is uint)
                                    return Convert.ToSingle((uint)correctInput);
                                else if (correctInput is double)
                                    return Convert.ToSingle((double)correctInput);
                                else if (correctInput is byte)
                                    return Convert.ToSingle((byte)correctInput);
                                else if (correctInput is short)
                                    return Convert.ToSingle((short)correctInput);
                                else if (correctInput is long)
                                    return Convert.ToSingle((long)correctInput);
                            }
                        }
                    }
                }
                
                return base.GetValue<float>();
            }
        }
        #endregion Property: Value

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: TermVariableInstance(TermVariableBase termVariableBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Create a new term variable instance from the given term variable base.
        /// </summary>
        /// <param name="termVariableBase">The term variable base to create a term variable instance from.</param>
        /// <param name="iVariableInstanceHolder">The variable and reference holder that will own the term variable instance.</param>
        public TermVariableInstance(TermVariableBase termVariableBase, IVariableInstanceHolder iVariableInstanceHolder)
            : base(termVariableBase, iVariableInstanceHolder)
        {
        }
        #endregion Constructor: TermVariableInstance(TermVariableBase termVariableBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Constructor: TermVariableInstance(TermVariableInstance termVariableInstance)
        /// <summary>
        /// Clones the term variable instance.
        /// </summary>
        /// <param name="termVariableInstance">The term variable instance to clone.</param>
        public TermVariableInstance(TermVariableInstance termVariableInstance)
            : base(termVariableInstance)
        {
        }
        #endregion Constructor: TermVariableInstance(TermVariableInstance termVariableInstance)

        #endregion Method Group: Constructors

    }
    #endregion Class: TermVariableInstance

}
