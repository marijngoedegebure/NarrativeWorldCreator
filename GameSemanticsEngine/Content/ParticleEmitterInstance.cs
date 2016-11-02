/**************************************************************************
 * 
 * ParticleEmitterInstance.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System;
using System.ComponentModel;
using GameSemantics.Utilities;
using GameSemanticsEngine.Tools;
using SemanticsEngine.Components;
using SemanticsEngine.Entities;
using SemanticsEngine.Interfaces;

namespace GameSemanticsEngine.GameContent
{

    #region Class: ParticleEmitterInstance
    /// <summary>
    /// An instance of a particle emitter.
    /// </summary>
    public class ParticleEmitterInstance : DynamicContentInstance
    {

        #region Properties and Fields

        #region Property: ParticleEmitterBase
        /// <summary>
        /// Gets the particle emitter base of which this is a particle emitter instance.
        /// </summary>
        public ParticleEmitterBase ParticleEmitterBase
        {
            get
            {
                if (this.ParticleEmitterValuedBase != null)
                    return this.ParticleEmitterValuedBase.ParticleEmitterBase;
                return null;
            }
        }
        #endregion Property: ParticleEmitterBase

        #region Property: ParticleEmitterValuedBase
        /// <summary>
        /// Gets the valued particle emitter base of which this is a particle emitter instance.
        /// </summary>
        public ParticleEmitterValuedBase ParticleEmitterValuedBase
        {
            get
            {
                return this.Base as ParticleEmitterValuedBase;
            }
        }
        #endregion Property: ParticleEmitterValuedBase

        #region Property: ParticleProperties
        /// <summary>
        /// The properties for the particles in the emitter.
        /// </summary>
        private ParticlePropertiesInstance particleProperties = null;
        
        /// <summary>
        /// Gets the properties for the particles in the emitter.
        /// </summary>
        public ParticlePropertiesInstance ParticleProperties
        {
            get
            {
                return null;
            }
        }
        #endregion Property: ParticleProperties

        #region Property: ParticleRepresentation
        /// <summary>
        /// Gets the representation of the particles, which indicates whether the particles are represented by a file, a tangible object, or tangible matter.
        /// </summary>
        public ParticleRepresentation ParticleRepresentation
        {
            get
            {
                if (this.ParticleEmitterValuedBase != null)
                    return GetProperty<ParticleRepresentation>("ParticleRepresentation", this.ParticleEmitterValuedBase.ParticleRepresentation);

                return default(ParticleRepresentation);
            }
            protected set
            {
                if (this.ParticleRepresentation != value)
                    SetProperty("ParticleRepresentation", value);
            }
        }
        #endregion Property: ParticleRepresentation

        #region Property: ParticleVisualizationFile
        /// <summary>
        /// Gets the visualization file for the particles, in case they are represented by a file.
        /// </summary>
        public String ParticleVisualizationFile
        {
            get
            {
                if (this.ParticleEmitterValuedBase != null)
                    return GetProperty<String>("ParticleVisualizationFile", this.ParticleEmitterValuedBase.ParticleVisualizationFile);

                return String.Empty;
            }
            protected set
            {
                if (this.ParticleVisualizationFile != value)
                    SetProperty("ParticleVisualizationFile", value);
            }
        }
        #endregion Property: ParticleVisualizationFile

        #region Property: ParticleSource
        /// <summary>
        /// Gets the space from which the emitter should emit particles, in case they are represented by a tangible object or tangible matter.
        /// </summary>
        public SpaceBase ParticleSource
        {
            get
            {
                if (this.ParticleEmitterValuedBase != null)
                    return GetProperty<SpaceBase>("ParticleSource", this.ParticleEmitterValuedBase.ParticleSource);

                return null;
            }
            protected set
            {
                if (this.ParticleSource != value)
                    SetProperty("ParticleSource", value);
            }
        }
        #endregion Property: ParticleSource

        #region Property: ParticleQuantity
        /// <summary>
        /// Gets the quantity that each particle should get when spawn, in case they are represented by tangible matter.
        /// </summary>
        public NumericalValueInstance ParticleQuantity
        {
            get
            {
                if (this.ParticleEmitterValuedBase != null)
                {
                    // Return the locally modified value, or create a new instance from the base and subscribe to possible changes
                    NumericalValueInstance particleQuantity = GetProperty<NumericalValueInstance>("ParticleQuantity", null);
                    if (particleQuantity == null)
                    {
                        particleQuantity = GameInstanceManager.Current.Create<NumericalValueInstance>(this.ParticleEmitterValuedBase.ParticleQuantity);
                        if (particleQuantity != null)
                            particleQuantity.PropertyChanged += new PropertyChangedEventHandler(particleQuantity_PropertyChanged);
                    }
                    return particleQuantity;
                }

                return null;
            }
            private set
            {
                if (this.ParticleQuantity != value)
                    SetProperty("ParticleQuantity", value);
            }
        }

        /// <summary>
        /// If the particle quantity changes, add the property and the modified quantity to the modifications table.
        /// </summary>
        private void particleQuantity_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender != null)
                SetProperty("ParticleQuantity", sender);
        }
        #endregion Property: ParticleQuantity

        #region Property: Offset
        /// <summary>
        /// Gets the offset from the particle system this emitter is in.
        /// </summary>
        public VectorValueInstance Offset
        {
            get
            {
                if (this.ParticleEmitterValuedBase != null)
                {
                    // Return the locally modified value, or create a new instance from the base and subscribe to possible changes
                    VectorValueInstance offset = GetProperty<VectorValueInstance>("Offset", null);
                    if (offset == null)
                    {
                        offset = GameInstanceManager.Current.Create<VectorValueInstance>(this.ParticleEmitterValuedBase.Offset);
                        if (offset != null)
                            offset.PropertyChanged += new PropertyChangedEventHandler(offset_PropertyChanged);
                    }
                    return offset;
                }

                return null;
            }
            private set
            {
                if (this.Offset != value)
                    SetProperty("Offset", value);
            }
        }

        /// <summary>
        /// If the offset changes, add the property and the modified offset to the modifications table.
        /// </summary>
        private void offset_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender != null)
                SetProperty("Offset", sender);
        }
        #endregion Property: Offset

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ParticleEmitterInstance(ParticleEmitterValuedBase particleEmitterValuedBase)
        /// <summary>
        /// Creates a new particle emitter instance from the given valued particle emitter base.
        /// </summary>
        /// <param name="particleEmitterValuedBase">The valued particle emitter base to create the particle emitter instance from.</param>
        internal ParticleEmitterInstance(ParticleEmitterValuedBase particleEmitterValuedBase)
            : base(particleEmitterValuedBase)
        {
            if (particleEmitterValuedBase.ParticleProperties != null)
                GameInstanceManager.Current.Create<ParticlePropertiesInstance>(particleEmitterValuedBase.ParticleProperties);
        }
        #endregion Constructor: ParticleEmitterInstance(ParticleEmitterValuedBase particleEmitterValuedBase)

        #region Constructor: ParticleEmitterInstance(ParticleEmitterInstance particleEmitterInstance)
        /// <summary>
        /// Clones a particle emitter instance.
        /// </summary>
        /// <param name="particleEmitterInstance">The particle emitter instance to clone.</param>
        protected internal ParticleEmitterInstance(ParticleEmitterInstance particleEmitterInstance)
            : base(particleEmitterInstance)
        {
            if (particleEmitterInstance != null)
            {
                if (particleEmitterInstance.ParticleProperties != null)
                    this.particleProperties = new ParticlePropertiesInstance(particleEmitterInstance.ParticleProperties);
                this.ParticleRepresentation = particleEmitterInstance.ParticleRepresentation;
                this.ParticleVisualizationFile = particleEmitterInstance.ParticleVisualizationFile;
                this.ParticleSource = particleEmitterInstance.ParticleSource;
                if (particleEmitterInstance.ParticleQuantity != null)
                    this.ParticleQuantity = new NumericalValueInstance(particleEmitterInstance.ParticleQuantity);
                if (particleEmitterInstance.Offset != null)
                    this.Offset = new VectorValueInstance(particleEmitterInstance.Offset);
            }
        }
        #endregion Constructor: ParticleEmitterInstance(ParticleEmitterInstance particleEmitterInstance)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the particle emitter instance satisfies the given condition.
        /// </summary>
        /// <param name="conditionBase">The condition to compare to the particle emitter instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the particle emitter instance is satisfies the given condition.</returns>
        public override bool Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            // Check whether the base satisfies the condition
            if (conditionBase != null && base.Satisfies(conditionBase, iVariableInstanceHolder))
            {
                ParticleEmitterConditionBase particleEmitterConditionBase = conditionBase as ParticleEmitterConditionBase;
                if (particleEmitterConditionBase != null)
                {
                    // Check whether all the properties have the correct values
                    if ((particleEmitterConditionBase.ParticleProperties == null || this.ParticleProperties.Satisfies(particleEmitterConditionBase.ParticleProperties, iVariableInstanceHolder)) &&
                        (particleEmitterConditionBase.ParticleRepresentation == null || this.ParticleRepresentation == (ParticleRepresentation)particleEmitterConditionBase.ParticleRepresentation) &&
                        (particleEmitterConditionBase.ParticleVisualizationFile == null || this.ParticleVisualizationFile == particleEmitterConditionBase.ParticleVisualizationFile) &&
                        (particleEmitterConditionBase.ParticleSource == null || this.ParticleSource == particleEmitterConditionBase.ParticleSource) &&
                        (particleEmitterConditionBase.ParticleQuantity == null || this.ParticleQuantity.Satisfies(particleEmitterConditionBase.ParticleQuantity, iVariableInstanceHolder)) &&
                        (particleEmitterConditionBase.Offset == null || this.Offset.Satisfies(particleEmitterConditionBase.Offset, iVariableInstanceHolder)))
                    {
                        return true;
                    }
                }
                else
                    return true;
            }
            return false;
        }
        #endregion Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)

        #region Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Apply the change to the particle emitter instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the particle emitter instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        protected internal override bool Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (changeBase != null && base.Apply(changeBase, iVariableInstanceHolder))
            {
                ParticleEmitterChangeBase particleEmitterChangeBase = changeBase as ParticleEmitterChangeBase;
                if (particleEmitterChangeBase != null)
                {
                    // Adjust the particle properties
                    if (particleEmitterChangeBase.ParticleProperties != null)
                        this.ParticleProperties.Apply(particleEmitterChangeBase.ParticleProperties, iVariableInstanceHolder);

                    // Adjust the particle representation
                    if (particleEmitterChangeBase.ParticleRepresentation != null)
                        this.ParticleRepresentation = (ParticleRepresentation)particleEmitterChangeBase.ParticleRepresentation;

                    // Adjust the particle visualization file
                    if (particleEmitterChangeBase.ParticleVisualizationFile != null)
                        this.ParticleVisualizationFile = particleEmitterChangeBase.ParticleVisualizationFile;

                    // Adjust the particle source
                    if (particleEmitterChangeBase.ParticleSource != null)
                        this.ParticleSource = particleEmitterChangeBase.ParticleSource;

                    // Adjust the particle quantity
                    if (particleEmitterChangeBase.ParticleQuantity != null)
                        this.ParticleQuantity.Apply(particleEmitterChangeBase.ParticleQuantity, iVariableInstanceHolder);

                    // Adjust the offset
                    if (particleEmitterChangeBase.Offset != null)
                        this.Offset.Apply(particleEmitterChangeBase.Offset, iVariableInstanceHolder);
                }
                return true;
            }
            return false;
        }
        #endregion Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: ParticleEmitterInstance

}