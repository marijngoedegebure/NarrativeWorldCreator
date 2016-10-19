/**************************************************************************
 * 
 * ParticleEmitter.cs
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
using Common;
using GameSemantics.Data;
using GameSemantics.Utilities;
using Semantics.Abstractions;
using Semantics.Components;
using Semantics.Data;
using Semantics.Entities;
using Semantics.Utilities;

namespace GameSemantics.GameContent
{

    #region Class: ParticleEmitter
    /// <summary>
    /// A particle emitter.
    /// </summary>
    public class ParticleEmitter : DynamicContent, IComparable<ParticleEmitter>
    {

        #region Method Group: Constructors

        #region Constructor: ParticleEmitter()
        /// <summary>
        /// Creates a new particle emitter.
        /// </summary>
        public ParticleEmitter()
            : base()
        {
        }
        #endregion Constructor: ParticleEmitter()

        #region Constructor: ParticleEmitter(uint id)
        /// <summary>
        /// Creates a new particle emitter from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a particle emitter from.</param>
        protected ParticleEmitter(uint id)
            : base(id)
        {
        }
        #endregion Constructor: ParticleEmitter(uint id)

        #region Constructor: ParticleEmitter(string file)
        /// <summary>
        /// Creates a new particle emitter with the given file.
        /// </summary>
        /// <param name="file">The file to assign to the particle emitter.</param>
        public ParticleEmitter(string file)
            : base(file)
        {
        }
        #endregion Constructor: ParticleEmitter(string file)

        #region Constructor: ParticleEmitter(ParticleEmitter particleEmitter)
        /// <summary>
        /// Clones a particle emitter.
        /// </summary>
        /// <param name="particleEmitter">The particle emitter to clone.</param>
        public ParticleEmitter(ParticleEmitter particleEmitter)
            : base(particleEmitter)
        {
        }
        #endregion Constructor: ParticleEmitter(ParticleEmitter particleEmitter)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the particle emitter.
        /// </summary>
        public override void Remove()
        {
            GameDatabase.Current.StartChange();
            GameDatabase.Current.StartRemove();

            base.Remove();

            GameDatabase.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(ParticleEmitter other)
        /// <summary>
        /// Compares the particle emitter to the other particle emitter.
        /// </summary>
        /// <param name="other">The particle emitter to compare to this particle emitter.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(ParticleEmitter other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(ParticleEmitter other)

        #endregion Method Group: Other

    }
    #endregion Class: ParticleEmitter

    #region Class: ParticleEmitterValued
    /// <summary>
    /// A valued version of a particle emitter.
    /// </summary>
    public class ParticleEmitterValued : DynamicContentValued
    {

        #region Properties and Fields

        #region Property: ParticleEmitter
        /// <summary>
        /// Gets the particle emitter of which this is a valued particle emitter.
        /// </summary>
        public ParticleEmitter ParticleEmitter
        {
            get
            {
                return this.Node as ParticleEmitter;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: ParticleEmitter

        #region Property: ParticleProperties
        /// <summary>
        /// Gets or sets the properties for the particles in the emitter.
        /// </summary>
        public ParticlePropertiesValued ParticleProperties
        {
            get
            {
                return GameDatabase.Current.Select<ParticlePropertiesValued>(this.ID, GameValueTables.ParticleEmitterValued, GameColumns.ParticlePropertiesValued);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.ParticleEmitterValued, GameColumns.ParticlePropertiesValued, value);
                NotifyPropertyChanged("ParticleProperties");
            }
        }
        #endregion Property: ParticleProperties

        #region Property: ParticleRepresentation
        /// <summary>
        /// Gets or sets the representation of the particles, which indicates whether the particles are represented by a file, a tangible object, or tangible matter.
        /// </summary>
        public ParticleRepresentation ParticleRepresentation
        {
            get
            {
                return GameDatabase.Current.Select<ParticleRepresentation>(this.ID, GameValueTables.ParticleEmitterValued, GameColumns.ParticleRepresentation);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.ParticleEmitterValued, GameColumns.ParticleRepresentation, value);
                NotifyPropertyChanged("ParticleRepresentation");
            }
        }
        #endregion Property: ParticleRepresentation

        #region Property: ParticleVisualizationFile
        /// <summary>
        /// Gets or sets the visualization file for the particles, in case they are represented by a file.
        /// </summary>
        public String ParticleVisualizationFile
        {
            get
            {
                return GameDatabase.Current.Select<String>(this.ID, GameValueTables.ParticleEmitterValued, GameColumns.ParticleVisualizationFile);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.ParticleEmitterValued, GameColumns.ParticleVisualizationFile, value);
                NotifyPropertyChanged("ParticleVisualizationFile");
            }
        }
        #endregion Property: ParticleVisualizationFile

        #region Property: ParticleSource
        /// <summary>
        /// Gets or sets the space from which the emitter should emit particles, in case they are represented by a tangible object or tangible matter.
        /// </summary>
        public Space ParticleSource
        {
            get
            {
                return GameDatabase.Current.Select<Space>(this.ID, GameValueTables.ParticleEmitterValued, GameColumns.ParticleSource);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.ParticleEmitterValued, GameColumns.ParticleSource, value);
                NotifyPropertyChanged("ParticleSource");
            }
        }
        #endregion Property: ParticleSource

        #region Property: ParticleQuantity
        /// <summary>
        /// Gets the quantity that each particle should get when spawn, in case they are represented by tangible matter.
        /// </summary>
        public NumericalValue ParticleQuantity
        {
            get
            {
                return GameDatabase.Current.Select<NumericalValue>(this.ID, GameValueTables.ParticleEmitterValued, GameColumns.ParticleQuantity);
            }
            private set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.ParticleEmitterValued, GameColumns.ParticleQuantity, value);
                NotifyPropertyChanged("ParticleQuantity");
            }
        }
        #endregion Property: ParticleQuantity

        #region Property: Offset
        /// <summary>
        /// Gets the offset from the particle system this emitter is in.
        /// </summary>
        public VectorValue Offset
        {
            get
            {
                return GameDatabase.Current.Select<VectorValue>(this.ID, GameValueTables.ParticleEmitterValued, GameColumns.Offset);
            }
            private set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.ParticleEmitterValued, GameColumns.Offset, value);
                NotifyPropertyChanged("Offset");
            }
        }
        #endregion Property: Offset

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: ParticleEmitterValued()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static ParticleEmitterValued()
        {
            // Particle properties and particle source
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(GameColumns.ParticlePropertiesValued, new Tuple<Type, EntryType>(typeof(ParticlePropertiesValued), EntryType.Nullable));
            dict.Add(GameColumns.ParticleSource, new Tuple<Type, EntryType>(typeof(Space), EntryType.Nullable));
            GameDatabase.Current.AddTableDefinition(GameValueTables.ParticleEmitterValued, typeof(ParticleEmitterValued), dict);
        }
        #endregion Static Constructor: ParticleEmitterValued()

        #region Constructor: ParticleEmitterValued(uint id)
        /// <summary>
        /// Creates a new valued particle emitter from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the valued particle emitter from.</param>
        protected ParticleEmitterValued(uint id)
            : base(id)
        {
            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);
        }
        #endregion Constructor: ParticleEmitterValued(uint id)

        #region Constructor: ParticleEmitterValued(ParticleEmitterValued particleEmitterValued)
        /// <summary>
        /// Clones a valued particle emitter.
        /// </summary>
        /// <param name="particleEmitterValued">The valued particle emitter to clone.</param>
        public ParticleEmitterValued(ParticleEmitterValued particleEmitterValued)
            : base(particleEmitterValued)
        {
            if (particleEmitterValued != null)
            {
                GameDatabase.Current.StartChange();

                if (particleEmitterValued.ParticleProperties != null)
                    this.ParticleProperties = new ParticlePropertiesValued(particleEmitterValued.ParticleProperties);
                this.ParticleRepresentation = particleEmitterValued.ParticleRepresentation;
                this.ParticleVisualizationFile = particleEmitterValued.ParticleVisualizationFile;
                this.ParticleSource = particleEmitterValued.ParticleSource;
                if (particleEmitterValued.ParticleQuantity != null)
                    this.ParticleQuantity = new NumericalValue(particleEmitterValued.ParticleQuantity);
                if (particleEmitterValued.Offset != null)
                    this.Offset = new VectorValue(particleEmitterValued.Offset);

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: ParticleEmitterValued(ParticleEmitterValued particleEmitterValued)

        #region Constructor: ParticleEmitterValued(ParticleEmitter particleEmitter)
        /// <summary>
        /// Creates a new valued particle emitter from the given particle emitter.
        /// </summary>
        /// <param name="particleEmitter">The particle emitter to create a valued particle emitter from.</param>
        public ParticleEmitterValued(ParticleEmitter particleEmitter)
            : base(particleEmitter)
        {
            GameDatabase.Current.StartChange();

            // Get the special quantity unit category and subscribe to its changes
            UnitCategory quantityUnitCategory = SemanticsManager.GetSpecialUnitCategory(SpecialUnitCategories.Quantity);
            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);

            if (this.ParticleQuantity != null)
                this.ParticleQuantity.UnitCategory = quantityUnitCategory;
            else
                this.ParticleQuantity = new NumericalValue(SemanticsSettings.Values.Quantity, quantityUnitCategory);

            this.Offset = new VectorValue(new Vec2(SemanticsSettings.Values.OffsetX, SemanticsSettings.Values.OffsetY));

            GameDatabase.Current.StopChange();
        }
        #endregion Constructor: ParticleEmitterValued(ParticleEmitter particleEmitter)

        #region Method: SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)
        /// <summary>
        /// Change the special quantity unit category.
        /// </summary>
        /// <param name="specialUnitCategory">The special unit category.</param>
        /// <param name="unitCategory">The unit category.</param>
        private void SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)
        {
            if (specialUnitCategory == SpecialUnitCategories.Quantity && this.ParticleQuantity != null)
                this.ParticleQuantity.UnitCategory = unitCategory;
        }
        #endregion Method: SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)

        #endregion Method Group: Constructors

    }
    #endregion Class: ParticleEmitterValued

    #region Class: ParticleEmitterCondition
    /// <summary>
    /// A condition on a particle emitter.
    /// </summary>
    public class ParticleEmitterCondition : DynamicContentCondition
    {

        #region Properties and Fields

        #region Property: ParticleEmitter
        /// <summary>
        /// Gets or sets the required particle emitter.
        /// </summary>
        public ParticleEmitter ParticleEmitter
        {
            get
            {
                return this.Node as ParticleEmitter;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: ParticleEmitter

        #region Property: ParticleProperties
        /// <summary>
        /// Gets or sets the required particle properties.
        /// </summary>
        public ParticlePropertiesCondition ParticleProperties
        {
            get
            {
                return GameDatabase.Current.Select<ParticlePropertiesCondition>(this.ID, GameValueTables.ParticleEmitterCondition, GameColumns.ParticlePropertiesCondition);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.ParticleEmitterCondition, GameColumns.ParticlePropertiesCondition, value);
                NotifyPropertyChanged("ParticleProperties");
            }
        }
        #endregion Property: ParticleProperties

        #region Property: ParticleRepresentation
        /// <summary>
        /// Gets or sets the required representation of the particles.
        /// </summary>
        public ParticleRepresentation? ParticleRepresentation
        {
            get
            {
                return GameDatabase.Current.Select<ParticleRepresentation?>(this.ID, GameValueTables.ParticleEmitterCondition, GameColumns.ParticleRepresentation);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.ParticleEmitterCondition, GameColumns.ParticleRepresentation, value);
                NotifyPropertyChanged("ParticleRepresentation");
            }
        }
        #endregion Property: ParticleRepresentation

        #region Property: ParticleVisualizationFile
        /// <summary>
        /// Gets or sets the required visualization file for the particles.
        /// </summary>
        public String ParticleVisualizationFile
        {
            get
            {
                return GameDatabase.Current.Select<String>(this.ID, GameValueTables.ParticleEmitterCondition, GameColumns.ParticleVisualizationFile);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.ParticleEmitterCondition, GameColumns.ParticleVisualizationFile, value);
                NotifyPropertyChanged("ParticleVisualizationFile");
            }
        }
        #endregion Property: ParticleVisualizationFile

        #region Property: ParticleSource
        /// <summary>
        /// Gets or sets the required space from which the emitter should emit particles.
        /// </summary>
        public Space ParticleSource
        {
            get
            {
                return GameDatabase.Current.Select<Space>(this.ID, GameValueTables.ParticleEmitterCondition, GameColumns.ParticleSource);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.ParticleEmitterCondition, GameColumns.ParticleSource, value);
                NotifyPropertyChanged("ParticleSource");
            }
        }
        #endregion Property: ParticleSource

        #region Property: ParticleQuantity
        /// <summary>
        /// Gets the required quantity that each particle should get when spawn.
        /// </summary>
        public NumericalValueCondition ParticleQuantity
        {
            get
            {
                return GameDatabase.Current.Select<NumericalValueCondition>(this.ID, GameValueTables.ParticleEmitterCondition, GameColumns.ParticleQuantity);
            }
            set
            {
                if (this.ParticleQuantity != null)
                    this.ParticleQuantity.Remove();

                GameDatabase.Current.Update(this.ID, GameValueTables.ParticleEmitterCondition, GameColumns.ParticleQuantity, value);
                NotifyPropertyChanged("ParticleQuantity");

                // Set the special quantity unit category
                if (value != null)
                    value.UnitCategory = SemanticsManager.GetSpecialUnitCategory(SpecialUnitCategories.Quantity);
            }
        }
        #endregion Property: ParticleQuantity

        #region Property: Offset
        /// <summary>
        /// Gets the required offset from the particle system this emitter is in.
        /// </summary>
        public VectorValueCondition Offset
        {
            get
            {
                return GameDatabase.Current.Select<VectorValueCondition>(this.ID, GameValueTables.ParticleEmitterCondition, GameColumns.Offset);
            }
            set
            {
                if (this.Offset != null)
                    this.Offset.Remove();

                GameDatabase.Current.Update(this.ID, GameValueTables.ParticleEmitterCondition, GameColumns.Offset, value);
                NotifyPropertyChanged("Offset");
            }
        }
        #endregion Property: Offset

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: ParticleEmitterCondition()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static ParticleEmitterCondition()
        {
            // Particle properties and particle source
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(GameColumns.ParticlePropertiesCondition, new Tuple<Type, EntryType>(typeof(ParticlePropertiesCondition), EntryType.Nullable));
            dict.Add(GameColumns.ParticleSource, new Tuple<Type, EntryType>(typeof(Space), EntryType.Nullable));
            GameDatabase.Current.AddTableDefinition(GameValueTables.ParticleEmitterCondition, typeof(ParticleEmitterCondition), dict);
        }
        #endregion Static Constructor: ParticleEmitterCondition()

        #region Constructor: ParticleEmitterCondition()
        /// <summary>
        /// Creates a new particle emitter condition.
        /// </summary>
        public ParticleEmitterCondition()
            : base()
        {
            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);
        }
        #endregion Constructor: ParticleEmitterCondition()

        #region Constructor: ParticleEmitterCondition(uint id)
        /// <summary>
        /// Creates a new particle emitter condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the particle emitter condition from.</param>
        protected ParticleEmitterCondition(uint id)
            : base(id)
        {
            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);
        }
        #endregion Constructor: ParticleEmitterCondition(uint id)

        #region Constructor: ParticleEmitterCondition(ParticleEmitterCondition particleEmitterCondition)
        /// <summary>
        /// Clones a particle emitter condition.
        /// </summary>
        /// <param name="particleEmitterCondition">The particle emitter condition to clone.</param>
        public ParticleEmitterCondition(ParticleEmitterCondition particleEmitterCondition)
            : base(particleEmitterCondition)
        {
            if (particleEmitterCondition != null)
            {
                GameDatabase.Current.StartChange();

                if (particleEmitterCondition.ParticleProperties != null)
                    this.ParticleProperties = new ParticlePropertiesCondition(particleEmitterCondition.ParticleProperties);
                this.ParticleRepresentation = particleEmitterCondition.ParticleRepresentation;
                this.ParticleVisualizationFile = particleEmitterCondition.ParticleVisualizationFile;
                this.ParticleSource = particleEmitterCondition.ParticleSource;
                if (particleEmitterCondition.ParticleQuantity != null)
                    this.ParticleQuantity = new NumericalValueCondition(particleEmitterCondition.ParticleQuantity);
                if (particleEmitterCondition.Offset != null)
                    this.Offset = new VectorValueCondition(particleEmitterCondition.Offset);

                GameDatabase.Current.StopChange();
            }

            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);
        }
        #endregion Constructor: ParticleEmitterCondition(ParticleEmitterCondition particleEmitterCondition)

        #region Constructor: ParticleEmitterCondition(ParticleEmitter particleEmitter)
        /// <summary>
        /// Creates a condition for the given particle emitter.
        /// </summary>
        /// <param name="particleEmitter">The particle emitter to create a condition for.</param>
        public ParticleEmitterCondition(ParticleEmitter particleEmitter)
            : base(particleEmitter)
        {
            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);
        }
        #endregion Constructor: ParticleEmitterCondition(ParticleEmitter particleEmitter)

        #region Method: SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)
        /// <summary>
        /// Change the special quantity unit category.
        /// </summary>
        /// <param name="specialUnitCategory">The special unit category.</param>
        /// <param name="unitCategory">The unit category.</param>
        private void SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)
        {
            if (specialUnitCategory == SpecialUnitCategories.Quantity && this.ParticleQuantity != null)
                this.ParticleQuantity.UnitCategory = unitCategory;
        }
        #endregion Method: SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)

        #endregion Method Group: Constructors

    }
    #endregion Class: ParticleEmitterCondition

    #region Class: ParticleEmitterChange
    /// <summary>
    /// A change on a particle emitter.
    /// </summary>
    public class ParticleEmitterChange : DynamicContentChange
    {

        #region Properties and Fields

        #region Property: ParticleEmitter
        /// <summary>
        /// Gets or sets the affected particle emitter.
        /// </summary>
        public ParticleEmitter ParticleEmitter
        {
            get
            {
                return this.Node as ParticleEmitter;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: ParticleEmitter

        #region Property: ParticleProperties
        /// <summary>
        /// Gets or sets the particle properties to change.
        /// </summary>
        public ParticlePropertiesChange ParticleProperties
        {
            get
            {
                return GameDatabase.Current.Select<ParticlePropertiesChange>(this.ID, GameValueTables.ParticleEmitterChange, GameColumns.ParticlePropertiesChange);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.ParticleEmitterChange, GameColumns.ParticlePropertiesChange, value);
                NotifyPropertyChanged("ParticleProperties");
            }
        }
        #endregion Property: ParticleProperties

        #region Property: ParticleRepresentation
        /// <summary>
        /// Gets or sets the representation of the particles to change to.
        /// </summary>
        public ParticleRepresentation? ParticleRepresentation
        {
            get
            {
                return GameDatabase.Current.Select<ParticleRepresentation?>(this.ID, GameValueTables.ParticleEmitterChange, GameColumns.ParticleRepresentation);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.ParticleEmitterChange, GameColumns.ParticleRepresentation, value);
                NotifyPropertyChanged("ParticleRepresentation");
            }
        }
        #endregion Property: ParticleRepresentation

        #region Property: ParticleVisualizationFile
        /// <summary>
        /// Gets or sets the visualization file for the particles to change to.
        /// </summary>
        public String ParticleVisualizationFile
        {
            get
            {
                return GameDatabase.Current.Select<String>(this.ID, GameValueTables.ParticleEmitterChange, GameColumns.ParticleVisualizationFile);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.ParticleEmitterChange, GameColumns.ParticleVisualizationFile, value);
                NotifyPropertyChanged("ParticleVisualizationFile");
            }
        }
        #endregion Property: ParticleVisualizationFile

        #region Property: ParticleSource
        /// <summary>
        /// Gets or sets the space to change to from which the emitter should emit particles.
        /// </summary>
        public Space ParticleSource
        {
            get
            {
                return GameDatabase.Current.Select<Space>(this.ID, GameValueTables.ParticleEmitterChange, GameColumns.ParticleSource);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameValueTables.ParticleEmitterChange, GameColumns.ParticleSource, value);
                NotifyPropertyChanged("ParticleSource");
            }
        }
        #endregion Property: ParticleSource

        #region Property: ParticleQuantity
        /// <summary>
        /// Gets the quantity change that each particle should get when spawn.
        /// </summary>
        public NumericalValueChange ParticleQuantity
        {
            get
            {
                return GameDatabase.Current.Select<NumericalValueChange>(this.ID, GameValueTables.ParticleEmitterChange, GameColumns.ParticleQuantity);
            }
            set
            {
                if (this.ParticleQuantity != null)
                    this.ParticleQuantity.Remove();

                GameDatabase.Current.Update(this.ID, GameValueTables.ParticleEmitterChange, GameColumns.ParticleQuantity, value);
                NotifyPropertyChanged("ParticleQuantity");

                // Set the special quantity unit category
                if (value != null)
                    value.UnitCategory = SemanticsManager.GetSpecialUnitCategory(SpecialUnitCategories.Quantity);
            }
        }
        #endregion Property: ParticleQuantity

        #region Property: Offset
        /// <summary>
        /// Gets the offset change from the particle system this emitter is in.
        /// </summary>
        public VectorValueChange Offset
        {
            get
            {
                return GameDatabase.Current.Select<VectorValueChange>(this.ID, GameValueTables.ParticleEmitterChange, GameColumns.Offset);
            }
            set
            {
                if (this.Offset != null)
                    this.Offset.Remove();

                GameDatabase.Current.Update(this.ID, GameValueTables.ParticleEmitterChange, GameColumns.Offset, value);
                NotifyPropertyChanged("Offset");
            }
        }
        #endregion Property: Offset

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: ParticleEmitterChange()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static ParticleEmitterChange()
        {
            // Particle properties and particle source
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(GameColumns.ParticlePropertiesChange, new Tuple<Type, EntryType>(typeof(ParticlePropertiesChange), EntryType.Nullable));
            dict.Add(GameColumns.ParticleSource, new Tuple<Type, EntryType>(typeof(SpaceChange), EntryType.Nullable));
            GameDatabase.Current.AddTableDefinition(GameValueTables.ParticleEmitterChange, typeof(ParticleEmitterChange), dict);
        }
        #endregion Static Constructor: ParticleEmitterChange()

        #region Constructor: ParticleEmitterChange()
        /// <summary>
        /// Creates a particle emitter change.
        /// </summary>
        public ParticleEmitterChange()
            : base()
        {
            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);
        }
        #endregion Constructor: ParticleEmitterChange()

        #region Constructor: ParticleEmitterChange(uint id)
        /// <summary>
        /// Creates a new particle emitter change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a particle emitter change from.</param>
        protected ParticleEmitterChange(uint id)
            : base(id)
        {
            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);
        }
        #endregion Constructor: ParticleEmitterChange(uint id)

        #region Constructor: ParticleEmitterChange(ParticleEmitterChange particleEmitterChange)
        /// <summary>
        /// Clones a particle emitter change.
        /// </summary>
        /// <param name="particleEmitterChange">The particle emitter change to clone.</param>
        public ParticleEmitterChange(ParticleEmitterChange particleEmitterChange)
            : base(particleEmitterChange)
        {
            if (particleEmitterChange != null)
            {
                GameDatabase.Current.StartChange();

                if (particleEmitterChange.ParticleProperties != null)
                    this.ParticleProperties = new ParticlePropertiesChange(particleEmitterChange.ParticleProperties);
                this.ParticleRepresentation = particleEmitterChange.ParticleRepresentation;
                this.ParticleVisualizationFile = particleEmitterChange.ParticleVisualizationFile;
                this.ParticleSource = particleEmitterChange.ParticleSource;
                if (particleEmitterChange.ParticleQuantity != null)
                    this.ParticleQuantity = new NumericalValueChange(particleEmitterChange.ParticleQuantity);
                if (particleEmitterChange.Offset != null)
                    this.Offset = new VectorValueChange(particleEmitterChange.Offset);

                GameDatabase.Current.StopChange();
            }

            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);
        }
        #endregion Constructor: ParticleEmitterChange(ParticleEmitterChange particleEmitterChange)

        #region Constructor: ParticleEmitterChange(ParticleEmitter particleEmitter)
        /// <summary>
        /// Creates a change for the given particle emitter.
        /// </summary>
        /// <param name="particleEmitter">The particle emitter to create a change for.</param>
        public ParticleEmitterChange(ParticleEmitter particleEmitter)
            : base(particleEmitter)
        {
            SemanticsManager.SpecialUnitCategoryChanged += new SemanticsManager.SpecialUnitCategoriesHandler(SemanticsManager_SpecialUnitCategoryChanged);
        }
        #endregion Constructor: ParticleEmitterChange(ParticleEmitter particleEmitter)

        #region Method: SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)
        /// <summary>
        /// Change the special quantity unit category.
        /// </summary>
        /// <param name="specialUnitCategory">The special unit category.</param>
        /// <param name="unitCategory">The unit category.</param>
        private void SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)
        {
            if (specialUnitCategory == SpecialUnitCategories.Quantity && this.ParticleQuantity != null)
                this.ParticleQuantity.UnitCategory = unitCategory;
        }
        #endregion Method: SemanticsManager_SpecialUnitCategoryChanged(SpecialUnitCategories specialUnitCategory, UnitCategory unitCategory)

        #endregion Method Group: Constructors

    }
    #endregion Class: ParticleEmitterChange

}
