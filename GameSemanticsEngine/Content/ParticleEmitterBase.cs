/**************************************************************************
 * 
 * ParticleEmitterBase.cs
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
using Common;
using GameSemantics.GameContent;
using GameSemantics.Utilities;
using GameSemanticsEngine.Tools;
using Semantics.Utilities;
using SemanticsEngine.Abstractions;
using SemanticsEngine.Components;
using SemanticsEngine.Entities;
using SemanticsEngine.Tools;

namespace GameSemanticsEngine.GameContent
{

    #region Class: ParticleEmitterBase
    /// <summary>
    /// A base of a particle emitter.
    /// </summary>
    public class ParticleEmitterBase : DynamicContentBase
    {

        #region Properties and Fields

        #region Property: ParticleEmitter
        /// <summary>
        /// Gets the particle emitter of which this is a particle emitter base.
        /// </summary>
        protected internal ParticleEmitter ParticleEmitter
        {
            get
            {
                return this.Node as ParticleEmitter;
            }
        }
        #endregion Property: ParticleEmitter

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ParticleEmitterBase(ParticleEmitter particleEmitter)
        /// <summary>
        /// Creates a new particle emitter base from the given particle emitter.
        /// </summary>
        /// <param name="particleEmitter">The particle emitter to create a particle emitter base from.</param>
        protected internal ParticleEmitterBase(ParticleEmitter particleEmitter)
            : base(particleEmitter)
        {
        }
        #endregion Constructor: ParticleEmitterBase(ParticleEmitter particleEmitter)

        #endregion Method Group: Constructors

    }
    #endregion Class: ParticleEmitterBase

    #region Class: ParticleEmitterValuedBase
    /// <summary>
    /// A base of a valued particle emitter.
    /// </summary>
    public class ParticleEmitterValuedBase : DynamicContentValuedBase
    {

        #region Properties and Fields

        #region Property: ParticleEmitterValued
        /// <summary>
        /// Gets the valued particle emitter of which this is a valued particle emitter base.
        /// </summary>
        protected internal ParticleEmitterValued ParticleEmitterValued
        {
            get
            {
                return this.NodeValued as ParticleEmitterValued;
            }
        }
        #endregion Property: ParticleEmitterValued

        #region Property: ParticleEmitterBase
        /// <summary>
        /// Gets the particle emitter base.
        /// </summary>
        public ParticleEmitterBase ParticleEmitterBase
        {
            get
            {
                return this.NodeBase as ParticleEmitterBase;
            }
        }
        #endregion Property: ParticleEmitterBase

        #region Property: ParticleProperties
        /// <summary>
        /// The properties for the particles in the emitter.
        /// </summary>
        private ParticlePropertiesValuedBase particleProperties = null;
        
        /// <summary>
        /// Gets the properties for the particles in the emitter.
        /// </summary>
        public ParticlePropertiesValuedBase ParticleProperties
        {
            get
            {
                return particleProperties;
            }
        }
        #endregion Property: ParticleProperties

        #region Property: ParticleRepresentation
        /// <summary>
        /// The representation of the particles, which indicates whether the particles are represented by a file, a tangible object, or tangible matter.
        /// </summary>
        private ParticleRepresentation particleRepresentation = default(ParticleRepresentation);
        
        /// <summary>
        /// Gets the representation of the particles, which indicates whether the particles are represented by a file, a tangible object, or tangible matter.
        /// </summary>
        public ParticleRepresentation ParticleRepresentation
        {
            get
            {
                return particleRepresentation;
            }
        }
        #endregion Property: ParticleRepresentation

        #region Property: ParticleVisualizationFile
        /// <summary>
        /// The visualization file for the particles, in case they are represented by a file.
        /// </summary>
        private String particleVisualizationFile = string.Empty;
        
        /// <summary>
        /// Gets the visualization file for the particles, in case they are represented by a file.
        /// </summary>
        public String ParticleVisualizationFile
        {
            get
            {
                return particleVisualizationFile;
            }
        }
        #endregion Property: ParticleVisualizationFile

        #region Property: ParticleSource
        /// <summary>
        /// The space from which the emitter should emit particles, in case they are represented by a tangible object or tangible matter.
        /// </summary>
        private SpaceBase particleSource = null;
        
        /// <summary>
        /// Gets the space from which the emitter should emit particles, in case they are represented by a tangible object or tangible matter.
        /// </summary>
        public SpaceBase ParticleSource
        {
            get
            {
                return particleSource;
            }
        }
        #endregion Property: ParticleSource

        #region Property: ParticleQuantity
        /// <summary>
        /// The quantity that each particle should get when spawn, in case they are represented by tangible matter.
        /// </summary>
        private NumericalValueBase particleQuantity = null;
        
        /// <summary>
        /// Gets the quantity that each particle should get when spawn, in case they are represented by tangible matter.
        /// </summary>
        public NumericalValueBase ParticleQuantity
        {
            get
            {
                if (particleQuantity == null)
                {
                    LoadParticleQuantity();
                    if (particleQuantity == null)
                    {
                        UnitCategoryBase quantityUnitCategoryBase = Utils.GetSpecialUnitCategory(SpecialUnitCategories.Quantity);
                        if (quantityUnitCategoryBase != null)
                            particleQuantity = new NumericalValueBase(SemanticsSettings.Values.Quantity, quantityUnitCategoryBase.BaseUnit);
                        else
                            particleQuantity = new NumericalValueBase(SemanticsSettings.Values.Quantity);
                    }
                }
                return particleQuantity;
            }
        }

        /// <summary>
        /// Loads the particle quantity.
        /// </summary>
        private void LoadParticleQuantity()
        {
            if (this.ParticleEmitterValued != null)
                particleQuantity = GameBaseManager.Current.GetBase<NumericalValueBase>(this.ParticleEmitterValued.ParticleQuantity);
        }
        #endregion Property: ParticleQuantity

        #region Property: Offset
        /// <summary>
        /// The offset from the particle system this emitter is in.
        /// </summary>
        private VectorValueBase offset = null;
        
        /// <summary>
        /// Gets the offset from the particle system this emitter is in.
        /// </summary>
        public VectorValueBase Offset
        {
            get
            {
                if (offset == null)
                {
                    LoadOffset();
                    if (offset == null)
                        offset = new VectorValueBase(new Vec4(SemanticsSettings.Values.OffsetX, SemanticsSettings.Values.OffsetY, SemanticsSettings.Values.OffsetZ, SemanticsSettings.Values.OffsetW));
                }
                return offset;
            }
        }

        /// <summary>
        /// Loads the offset.
        /// </summary>
        private void LoadOffset()
        {
            if (this.ParticleEmitterValued != null)
                offset = GameBaseManager.Current.GetBase<VectorValueBase>(this.ParticleEmitterValued.Offset);
        }
        #endregion Property: Offset

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ParticleEmitterValuedBase(ParticleEmitterValued particleEmitterValued)
        /// <summary>
        /// Create a valued particle emitter base from the given valued particle emitter.
        /// </summary>
        /// <param name="particleEmitterValued">The valued particle emitter to create a valued particle emitter base from.</param>
        protected internal ParticleEmitterValuedBase(ParticleEmitterValued particleEmitterValued)
            : base(particleEmitterValued)
        {
            if (particleEmitterValued != null)
            {
                this.particleProperties = GameBaseManager.Current.GetBase<ParticlePropertiesValuedBase>(particleEmitterValued.ParticleProperties);
                this.particleRepresentation = particleEmitterValued.ParticleRepresentation;
                this.particleVisualizationFile = particleEmitterValued.ParticleVisualizationFile;
                this.particleSource = GameBaseManager.Current.GetBase<SpaceBase>(particleEmitterValued.ParticleSource);

                if (GameBaseManager.PreloadProperties)
                {
                    LoadParticleQuantity();
                    LoadOffset();

                }
            }
        }
        #endregion Constructor: ParticleEmitterValuedBase(ParticleEmitterValued particleEmitterValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: ParticleEmitterValuedBase

    #region Class: ParticleEmitterConditionBase
    /// <summary>
    /// A condition on a particle emitter.
    /// </summary>
    public class ParticleEmitterConditionBase : DynamicContentConditionBase
    {

        #region Properties and Fields

        #region Property: ParticleEmitterCondition
        /// <summary>
        /// Gets the particle emitter condition of which this is a particle emitter condition base.
        /// </summary>
        protected internal ParticleEmitterCondition ParticleEmitterCondition
        {
            get
            {
                return this.Condition as ParticleEmitterCondition;
            }
        }
        #endregion Property: ParticleEmitterCondition

        #region Property: ParticleEmitterBase
        /// <summary>
        /// Gets the particle emitter base of which this is a particle emitter condition base.
        /// </summary>
        public ParticleEmitterBase ParticleEmitterBase
        {
            get
            {
                return this.NodeBase as ParticleEmitterBase;
            }
        }
        #endregion Property: ParticleEmitterBase

        #region Property: ParticleProperties
        /// <summary>
        /// The required particle properties.
        /// </summary>
        private ParticlePropertiesConditionBase particleProperties = null;

        /// <summary>
        /// Gets the required particle properties.
        /// </summary>
        public ParticlePropertiesConditionBase ParticleProperties
        {
            get
            {
                return particleProperties;
            }
        }
        #endregion Property: ParticleProperties

        #region Property: ParticleRepresentation
        /// <summary>
        /// The required representation of the particles.
        /// </summary>
        private ParticleRepresentation? particleRepresentation = null;

        /// <summary>
        /// Gets the required representation of the particles.
        /// </summary>
        public ParticleRepresentation? ParticleRepresentation
        {
            get
            {
                return particleRepresentation;
            }
        }
        #endregion Property: ParticleRepresentation

        #region Property: ParticleVisualizationFile
        /// <summary>
        /// The required visualization file for the particles.
        /// </summary>
        private String particleVisualizationFile = null;

        /// <summary>
        /// Gets the required visualization file for the particles.
        /// </summary>
        public String ParticleVisualizationFile
        {
            get
            {
                return particleVisualizationFile;
            }
        }
        #endregion Property: ParticleVisualizationFile

        #region Property: ParticleSource
        /// <summary>
        /// The required space from which the emitter should emit particles.
        /// </summary>
        private SpaceBase particleSource = null;

        /// <summary>
        /// Gets the required space from which the emitter should emit particles.
        /// </summary>
        public SpaceBase ParticleSource
        {
            get
            {
                return particleSource;
            }
        }
        #endregion Property: ParticleSource

        #region Property: ParticleQuantity
        /// <summary>
        /// The required quantity that each particle should get when spawn.
        /// </summary>
        private NumericalValueConditionBase particleQuantity = null;

        /// <summary>
        /// Gets the required quantity that each particle should get when spawn.
        /// </summary>
        public NumericalValueConditionBase ParticleQuantity
        {
            get
            {
                if (particleQuantity == null)
                    LoadParticleQuantity();
                return particleQuantity;
            }
        }

        /// <summary>
        /// Loads the particle quantity.
        /// </summary>
        private void LoadParticleQuantity()
        {
            if (this.ParticleEmitterCondition != null)
                particleQuantity = GameBaseManager.Current.GetBase<NumericalValueConditionBase>(this.ParticleEmitterCondition.ParticleQuantity);
        }
        #endregion Property: ParticleQuantity

        #region Property: Offset
        /// <summary>
        /// The required offset from the particle system this emitter is in.
        /// </summary>
        private VectorValueConditionBase offset = null;

        /// <summary>
        /// Gets the required offset from the particle system this emitter is in.
        /// </summary>
        public VectorValueConditionBase Offset
        {
            get
            {
                if (offset == null)
                    LoadOffset();
                return offset;
            }
        }

        /// <summary>
        /// Loads the offset.
        /// </summary>
        private void LoadOffset()
        {
            if (this.ParticleEmitterCondition != null)
                offset = GameBaseManager.Current.GetBase<VectorValueConditionBase>(this.ParticleEmitterCondition.Offset);
        }
        #endregion Property: Offset

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ParticleEmitterConditionBase(ParticleEmitterCondition particleEmitterCondition)
        /// <summary>
        /// Creates a base of the given particle emitter condition.
        /// </summary>
        /// <param name="particleEmitterCondition">The particle emitter condition to create a base of.</param>
        protected internal ParticleEmitterConditionBase(ParticleEmitterCondition particleEmitterCondition)
            : base(particleEmitterCondition)
        {
            if (particleEmitterCondition != null)
            {
                this.particleProperties = GameBaseManager.Current.GetBase<ParticlePropertiesConditionBase>(particleEmitterCondition.ParticleProperties);
                this.particleRepresentation = particleEmitterCondition.ParticleRepresentation;
                this.particleVisualizationFile = particleEmitterCondition.ParticleVisualizationFile;
                this.particleSource = GameBaseManager.Current.GetBase<SpaceBase>(particleEmitterCondition.ParticleSource);

                if (GameBaseManager.PreloadProperties)
                {
                    LoadParticleQuantity();
                    LoadOffset();
                }
            }
        }
        #endregion Constructor: ParticleEmitterConditionBase(ParticleEmitterCondition particleEmitterCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: ParticleEmitterConditionBase

    #region Class: ParticleEmitterChangeBase
    /// <summary>
    /// A change on particle emitter.
    /// </summary>
    public class ParticleEmitterChangeBase : DynamicContentChangeBase
    {

        #region Properties and Fields

        #region Property: ParticleEmitterChange
        /// <summary>
        /// Gets the particle emitter change of which this is a particle emitter change base.
        /// </summary>
        protected internal ParticleEmitterChange ParticleEmitterChange
        {
            get
            {
                return this.Change as ParticleEmitterChange;
            }
        }
        #endregion Property: ParticleEmitterChange

        #region Property: ParticleEmitterBase
        /// <summary>
        /// Gets the affected particle emitter base.
        /// </summary>
        public ParticleEmitterBase ParticleEmitterBase
        {
            get
            {
                return this.NodeBase as ParticleEmitterBase;
            }
        }
        #endregion Property: ParticleEmitterBase

        #region Property: ParticleProperties
        /// <summary>
        /// The particle properties to change.
        /// </summary>
        private ParticlePropertiesChangeBase particleProperties = null;

        /// <summary>
        /// Gets the particle properties to change.
        /// </summary>
        public ParticlePropertiesChangeBase ParticleProperties
        {
            get
            {
                return particleProperties;
            }
        }
        #endregion Property: ParticleProperties

        #region Property: ParticleRepresentation
        /// <summary>
        /// The representation of the particles to change to.
        /// </summary>
        private ParticleRepresentation? particleRepresentation = null;

        /// <summary>
        /// Gets the representation of the particles to change to.
        /// </summary>
        public ParticleRepresentation? ParticleRepresentation
        {
            get
            {
                return particleRepresentation;
            }
        }
        #endregion Property: ParticleRepresentation

        #region Property: ParticleVisualizationFile
        /// <summary>
        /// The visualization file for the particles to change to.
        /// </summary>
        private String particleVisualizationFile = null;

        /// <summary>
        /// Gets the visualization file for the particles to change to.
        /// </summary>
        public String ParticleVisualizationFile
        {
            get
            {
                return particleVisualizationFile;
            }
        }
        #endregion Property: ParticleVisualizationFile

        #region Property: ParticleSource
        /// <summary>
        /// The space to change from which the emitter should emit particles.
        /// </summary>
        private SpaceBase particleSource = null;

        /// <summary>
        /// Gets the space to change from which the emitter should emit particles.
        /// </summary>
        public SpaceBase ParticleSource
        {
            get
            {
                return particleSource;
            }
        }
        #endregion Property: ParticleSource

        #region Property: ParticleQuantity
        /// <summary>
        /// The quantity change that each particle should get when spawn.
        /// </summary>
        private NumericalValueChangeBase particleQuantity = null;

        /// <summary>
        /// Gets the quantity change that each particle should get when spawn.
        /// </summary>
        public NumericalValueChangeBase ParticleQuantity
        {
            get
            {
                if (particleQuantity == null)
                    LoadParticleQuantity();
                return particleQuantity;
            }
        }

        /// <summary>
        /// Loads the particle quantity.
        /// </summary>
        private void LoadParticleQuantity()
        {
            if (this.ParticleEmitterChange != null)
                particleQuantity = GameBaseManager.Current.GetBase<NumericalValueChangeBase>(this.ParticleEmitterChange.ParticleQuantity);
        }
        #endregion Property: ParticleQuantity

        #region Property: Offset
        /// <summary>
        /// The offset change from the particle system this emitter is in.
        /// </summary>
        private VectorValueChangeBase offset = null;

        /// <summary>
        /// Gets the offset change from the particle system this emitter is in.
        /// </summary>
        public VectorValueChangeBase Offset
        {
            get
            {
                if (offset == null)
                    LoadOffset();
                return offset;
            }
        }

        /// <summary>
        /// Loads the offset.
        /// </summary>
        private void LoadOffset()
        {
            if (this.ParticleEmitterChange != null)
                offset = GameBaseManager.Current.GetBase<VectorValueChangeBase>(this.ParticleEmitterChange.Offset);
        }
        #endregion Property: Offset

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ParticleEmitterChangeBase(ParticleEmitterChange particleEmitterChange)
        /// <summary>
        /// Creates a base of the given particle emitter change.
        /// </summary>
        /// <param name="particleEmitterChange">The particle emitter change to create a base of.</param>
        protected internal ParticleEmitterChangeBase(ParticleEmitterChange particleEmitterChange)
            : base(particleEmitterChange)
        {
            if (particleEmitterChange != null)
            {
                this.particleProperties = GameBaseManager.Current.GetBase<ParticlePropertiesChangeBase>(particleEmitterChange.ParticleProperties);
                this.particleRepresentation = particleEmitterChange.ParticleRepresentation;
                this.particleVisualizationFile = particleEmitterChange.ParticleVisualizationFile;
                this.particleSource = GameBaseManager.Current.GetBase<SpaceBase>(particleEmitterChange.ParticleSource);

                if (GameBaseManager.PreloadProperties)
                {
                    LoadParticleQuantity();
                    LoadOffset();
                }
            }
        }
        #endregion Constructor: ParticleEmitterChangeBase(ParticleEmitterChange particleEmitterChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: ParticleEmitterChangeBase

}