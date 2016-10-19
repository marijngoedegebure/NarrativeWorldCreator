/**************************************************************************
 * 
 * AddRemoveRequirements.cs
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
using Common;
using Semantics.Abstractions;
using Semantics.Data;
using Semantics.Entities;

namespace Semantics.Components
{

    #region Class: AddRemoveRequirements
    /// <summary>
    /// A class that wraps all add and remove requirements of event requirements.
    /// </summary>
    public class AddRemoveRequirements : IdHolder
    {

        #region Properties and Fields

        #region Property: AbstractEntityAdded
        /// <summary>
        /// Gets or sets the abstract entity that should have been added.
        /// </summary>
        public AbstractEntityCondition AbstractEntityAdded
        {
            get
            {
                return Database.Current.Select<AbstractEntityCondition>(this.ID, GenericTables.AddRemoveRequirements, Columns.AbstractEntityAdded);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.AddRemoveRequirements, Columns.AbstractEntityAdded, value);
                NotifyPropertyChanged("AbstractEntityAdded");
            }
        }
        #endregion Property: AbstractEntityAdded

        #region Property: AbstractEntityRemoved
        /// <summary>
        /// Gets or sets the abstract entity that should have been removed.
        /// </summary>
        public AbstractEntityCondition AbstractEntityRemoved
        {
            get
            {
                return Database.Current.Select<AbstractEntityCondition>(this.ID, GenericTables.AddRemoveRequirements, Columns.AbstractEntityRemoved);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.AddRemoveRequirements, Columns.AbstractEntityRemoved, value);
                NotifyPropertyChanged("AbstractEntityRemoved");
            }
        }
        #endregion Property: AbstractEntityRemoved

        #region Property: ConnectionItemAdded
        /// <summary>
        /// Gets or sets the connection item that should have been added.
        /// </summary>
        public TangibleObjectCondition ConnectionItemAdded
        {
            get
            {
                return Database.Current.Select<TangibleObjectCondition>(this.ID, GenericTables.AddRemoveRequirements, Columns.ConnectionItemAdded);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.AddRemoveRequirements, Columns.ConnectionItemAdded, value);
                NotifyPropertyChanged("ConnectionItemAdded");
            }
        }
        #endregion Property: ConnectionItemAdded

        #region Property: ConnectionItemRemoved
        /// <summary>
        /// Gets or sets the connection item that should have been removed.
        /// </summary>
        public TangibleObjectCondition ConnectionItemRemoved
        {
            get
            {
                return Database.Current.Select<TangibleObjectCondition>(this.ID, GenericTables.AddRemoveRequirements, Columns.ConnectionItemRemoved);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.AddRemoveRequirements, Columns.ConnectionItemRemoved, value);
                NotifyPropertyChanged("ConnectionItemRemoved");
            }
        }
        #endregion Property: ConnectionItemRemoved

        #region Property: CoverAdded
        /// <summary>
        /// Gets or sets the cover that should have been added.
        /// </summary>
        public TangibleObjectCondition CoverAdded
        {
            get
            {
                return Database.Current.Select<TangibleObjectCondition>(this.ID, GenericTables.AddRemoveRequirements, Columns.CoverAdded);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.AddRemoveRequirements, Columns.CoverAdded, value);
                NotifyPropertyChanged("CoverAdded");
            }
        }
        #endregion Property: CoverAdded

        #region Property: CoverRemoved
        /// <summary>
        /// Gets or sets the cover that should have been removed.
        /// </summary>
        public TangibleObjectCondition CoverRemoved
        {
            get
            {
                return Database.Current.Select<TangibleObjectCondition>(this.ID, GenericTables.AddRemoveRequirements, Columns.CoverRemoved);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.AddRemoveRequirements, Columns.CoverRemoved, value);
                NotifyPropertyChanged("CoverRemoved");
            }
        }
        #endregion Property: CoverRemoved

        #region Property: ElementAdded
        /// <summary>
        /// Gets or sets the element that should have been added.
        /// </summary>
        public ElementCondition ElementAdded
        {
            get
            {
                return Database.Current.Select<ElementCondition>(this.ID, GenericTables.AddRemoveRequirements, Columns.ElementAdded);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.AddRemoveRequirements, Columns.ElementAdded, value);
                NotifyPropertyChanged("ElementAdded");
            }
        }
        #endregion Property: ElementAdded

        #region Property: ElementRemoved
        /// <summary>
        /// Gets or sets the element that should have been removed.
        /// </summary>
        public ElementCondition ElementRemoved
        {
            get
            {
                return Database.Current.Select<ElementCondition>(this.ID, GenericTables.AddRemoveRequirements, Columns.ElementRemoved);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.AddRemoveRequirements, Columns.ElementRemoved, value);
                NotifyPropertyChanged("ElementRemoved");
            }
        }
        #endregion Property: ElementRemoved

        #region Property: ItemAdded
        /// <summary>
        /// Gets or sets the item that should have been added.
        /// </summary>
        public TangibleObjectCondition ItemAdded
        {
            get
            {
                return Database.Current.Select<TangibleObjectCondition>(this.ID, GenericTables.AddRemoveRequirements, Columns.ItemAdded);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.AddRemoveRequirements, Columns.ItemAdded, value);
                NotifyPropertyChanged("ItemAdded");
            }
        }
        #endregion Property: ItemAdded

        #region Property: ItemRemoved
        /// <summary>
        /// Gets or sets the item that should have been removed.
        /// </summary>
        public TangibleObjectCondition ItemRemoved
        {
            get
            {
                return Database.Current.Select<TangibleObjectCondition>(this.ID, GenericTables.AddRemoveRequirements, Columns.ItemRemoved);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.AddRemoveRequirements, Columns.ItemRemoved, value);
                NotifyPropertyChanged("ItemRemoved");
            }
        }
        #endregion Property: ItemRemoved

        #region Property: LayerAdded
        /// <summary>
        /// Gets or sets the layer that should have been added.
        /// </summary>
        public LayerCondition LayerAdded
        {
            get
            {
                return Database.Current.Select<LayerCondition>(this.ID, GenericTables.AddRemoveRequirements, Columns.LayerAdded);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.AddRemoveRequirements, Columns.LayerAdded, value);
                NotifyPropertyChanged("LayerAdded");
            }
        }
        #endregion Property: LayerAdded

        #region Property: LayerRemoved
        /// <summary>
        /// Gets or sets the layer that should have been removed.
        /// </summary>
        public LayerCondition LayerRemoved
        {
            get
            {
                return Database.Current.Select<LayerCondition>(this.ID, GenericTables.AddRemoveRequirements, Columns.LayerRemoved);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.AddRemoveRequirements, Columns.LayerRemoved, value);
                NotifyPropertyChanged("LayerRemoved");
            }
        }
        #endregion Property: LayerRemoved

        #region Property: MatterAdded
        /// <summary>
        /// Gets or sets the matter that should have been added.
        /// </summary>
        public MatterCondition MatterAdded
        {
            get
            {
                return Database.Current.Select<MatterCondition>(this.ID, GenericTables.AddRemoveRequirements, Columns.MatterAdded);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.AddRemoveRequirements, Columns.MatterAdded, value);
                NotifyPropertyChanged("MatterAdded");
            }
        }
        #endregion Property: MatterAdded

        #region Property: MatterRemoved
        /// <summary>
        /// Gets or sets the matter that should have been removed.
        /// </summary>
        public MatterCondition MatterRemoved
        {
            get
            {
                return Database.Current.Select<MatterCondition>(this.ID, GenericTables.AddRemoveRequirements, Columns.MatterRemoved);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.AddRemoveRequirements, Columns.MatterRemoved, value);
                NotifyPropertyChanged("MatterRemoved");
            }
        }
        #endregion Property: MatterRemoved

        #region Property: PartAdded
        /// <summary>
        /// Gets or sets the part that should have been added.
        /// </summary>
        public TangibleObjectCondition PartAdded
        {
            get
            {
                return Database.Current.Select<TangibleObjectCondition>(this.ID, GenericTables.AddRemoveRequirements, Columns.PartAdded);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.AddRemoveRequirements, Columns.PartAdded, value);
                NotifyPropertyChanged("PartAdded");
            }
        }
        #endregion Property: PartAdded

        #region Property: PartRemoved
        /// <summary>
        /// Gets or sets the part that should have been removed.
        /// </summary>
        public TangibleObjectCondition PartRemoved
        {
            get
            {
                return Database.Current.Select<TangibleObjectCondition>(this.ID, GenericTables.AddRemoveRequirements, Columns.PartRemoved);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.AddRemoveRequirements, Columns.PartRemoved, value);
                NotifyPropertyChanged("PartRemoved");
            }
        }
        #endregion Property: PartRemoved

        #region Property: RelationshipAdded
        /// <summary>
        /// Gets or sets the relationship that should have been added.
        /// </summary>
        public RelationshipType RelationshipAdded
        {
            get
            {
                return Database.Current.Select<RelationshipType>(this.ID, GenericTables.AddRemoveRequirements, Columns.RelationshipAdded);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.AddRemoveRequirements, Columns.RelationshipAdded, value);
                NotifyPropertyChanged("RelationshipAdded");
            }
        }
        #endregion Property: RelationshipAdded

        #region Property: RelationshipRemoved
        /// <summary>
        /// Gets or sets the relationship that should have been removed.
        /// </summary>
        public RelationshipType RelationshipRemoved
        {
            get
            {
                return Database.Current.Select<RelationshipType>(this.ID, GenericTables.AddRemoveRequirements, Columns.RelationshipRemoved);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.AddRemoveRequirements, Columns.RelationshipRemoved, value);
                NotifyPropertyChanged("RelationshipRemoved");
            }
        }
        #endregion Property: RelationshipRemoved

        #region Property: SpaceAdded
        /// <summary>
        /// Gets or sets the space that should have been added.
        /// </summary>
        public SpaceCondition SpaceAdded
        {
            get
            {
                return Database.Current.Select<SpaceCondition>(this.ID, GenericTables.AddRemoveRequirements, Columns.SpaceAdded);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.AddRemoveRequirements, Columns.SpaceAdded, value);
                NotifyPropertyChanged("SpaceAdded");
            }
        }
        #endregion Property: SpaceAdded

        #region Property: SpaceRemoved
        /// <summary>
        /// Gets or sets the space that should have been removed.
        /// </summary>
        public SpaceCondition SpaceRemoved
        {
            get
            {
                return Database.Current.Select<SpaceCondition>(this.ID, GenericTables.AddRemoveRequirements, Columns.SpaceRemoved);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.AddRemoveRequirements, Columns.SpaceRemoved, value);
                NotifyPropertyChanged("SpaceRemoved");
            }
        }
        #endregion Property: SpaceRemoved

        #region Property: SubstanceAdded
        /// <summary>
        /// Gets or sets the substance that should have been added.
        /// </summary>
        public SubstanceCondition SubstanceAdded
        {
            get
            {
                return Database.Current.Select<SubstanceCondition>(this.ID, GenericTables.AddRemoveRequirements, Columns.SubstanceAdded);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.AddRemoveRequirements, Columns.SubstanceAdded, value);
                NotifyPropertyChanged("SubstanceAdded");
            }
        }
        #endregion Property: SubstanceAdded

        #region Property: SubstanceRemoved
        /// <summary>
        /// Gets or sets the substance that should have been removed.
        /// </summary>
        public SubstanceCondition SubstanceRemoved
        {
            get
            {
                return Database.Current.Select<SubstanceCondition>(this.ID, GenericTables.AddRemoveRequirements, Columns.SubstanceRemoved);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.AddRemoveRequirements, Columns.SubstanceRemoved, value);
                NotifyPropertyChanged("SubstanceRemoved");
            }
        }
        #endregion Property: SubstanceRemoved

        #region Property: TangibleMatterAdded
        /// <summary>
        /// Gets or sets the tangible matter that should have been added.
        /// </summary>
        public MatterCondition TangibleMatterAdded
        {
            get
            {
                return Database.Current.Select<MatterCondition>(this.ID, GenericTables.AddRemoveRequirements, Columns.TangibleMatterAdded);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.AddRemoveRequirements, Columns.TangibleMatterAdded, value);
                NotifyPropertyChanged("TangibleMatterAdded");
            }
        }
        #endregion Property: TangibleMatterAdded

        #region Property: TangibleMatterRemoved
        /// <summary>
        /// Gets or sets the tangible matter that should have been removed.
        /// </summary>
        public MatterCondition TangibleMatterRemoved
        {
            get
            {
                return Database.Current.Select<MatterCondition>(this.ID, GenericTables.AddRemoveRequirements, Columns.TangibleMatterRemoved);
            }
            set
            {
                Database.Current.Update(this.ID, GenericTables.AddRemoveRequirements, Columns.TangibleMatterRemoved, value);
                NotifyPropertyChanged("TangibleMatterRemoved");
            }
        }
        #endregion Property: TangibleMatterRemoved

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: AddRemoveRequirements()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static AddRemoveRequirements()
        {
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.AbstractEntityAdded, new Tuple<Type, EntryType>(typeof(AbstractEntityCondition), EntryType.Unique));
            dict.Add(Columns.AbstractEntityRemoved, new Tuple<Type, EntryType>(typeof(AbstractEntityCondition), EntryType.Unique));
            dict.Add(Columns.ConnectionItemAdded, new Tuple<Type, EntryType>(typeof(TangibleObjectCondition), EntryType.Unique));
            dict.Add(Columns.ConnectionItemRemoved, new Tuple<Type, EntryType>(typeof(TangibleObjectCondition), EntryType.Unique));
            dict.Add(Columns.CoverAdded, new Tuple<Type, EntryType>(typeof(TangibleObjectCondition), EntryType.Unique));
            dict.Add(Columns.CoverRemoved, new Tuple<Type, EntryType>(typeof(TangibleObjectCondition), EntryType.Unique));
            dict.Add(Columns.ElementAdded, new Tuple<Type, EntryType>(typeof(ElementCondition), EntryType.Unique));
            dict.Add(Columns.ElementRemoved, new Tuple<Type, EntryType>(typeof(ElementCondition), EntryType.Unique));
            dict.Add(Columns.ItemAdded, new Tuple<Type, EntryType>(typeof(TangibleObjectCondition), EntryType.Unique));
            dict.Add(Columns.ItemRemoved, new Tuple<Type, EntryType>(typeof(TangibleObjectCondition), EntryType.Unique));
            dict.Add(Columns.LayerAdded, new Tuple<Type, EntryType>(typeof(LayerCondition), EntryType.Unique));
            dict.Add(Columns.LayerRemoved, new Tuple<Type, EntryType>(typeof(LayerCondition), EntryType.Unique));
            dict.Add(Columns.MatterAdded, new Tuple<Type, EntryType>(typeof(MatterCondition), EntryType.Unique));
            dict.Add(Columns.MatterRemoved, new Tuple<Type, EntryType>(typeof(MatterCondition), EntryType.Unique));
            dict.Add(Columns.PartAdded, new Tuple<Type, EntryType>(typeof(TangibleObjectCondition), EntryType.Unique));
            dict.Add(Columns.PartRemoved, new Tuple<Type, EntryType>(typeof(TangibleObjectCondition), EntryType.Unique));
            dict.Add(Columns.RelationshipAdded, new Tuple<Type, EntryType>(typeof(RelationshipType), EntryType.Nullable));
            dict.Add(Columns.RelationshipRemoved, new Tuple<Type, EntryType>(typeof(RelationshipType), EntryType.Nullable));
            dict.Add(Columns.SpaceAdded, new Tuple<Type, EntryType>(typeof(SpaceCondition), EntryType.Unique));
            dict.Add(Columns.SpaceRemoved, new Tuple<Type, EntryType>(typeof(SpaceCondition), EntryType.Unique));
            dict.Add(Columns.SubstanceAdded, new Tuple<Type, EntryType>(typeof(SubstanceCondition), EntryType.Unique));
            dict.Add(Columns.SubstanceRemoved, new Tuple<Type, EntryType>(typeof(SubstanceCondition), EntryType.Unique));
            dict.Add(Columns.TangibleMatterAdded, new Tuple<Type, EntryType>(typeof(MatterCondition), EntryType.Unique));
            dict.Add(Columns.TangibleMatterRemoved, new Tuple<Type, EntryType>(typeof(MatterCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.AddRemoveRequirements, typeof(AddRemoveRequirements), dict);
        }
        #endregion Static Constructor: AddRemoveRequirements()

        #region Constructor: AddRemoveRequirements()
        /// <summary>
        /// Creates new add/remove requirements.
        /// </summary>
        internal AddRemoveRequirements()
            : base()
        {
        }
        #endregion Constructor: AddRemoveRequirements()

        #region Constructor: AddRemoveRequirements(uint id)
        /// <summary>
        /// Creates new add/remove requirements with the given ID.
        /// </summary>
        /// <param name="id">The ID to create new add/remove requirements from.</param>
        private AddRemoveRequirements(uint id)
            : base(id)
        {
        }
        #endregion Constructor: AddRemoveRequirements(uint id)

        #region Constructor: AddRemoveRequirements(AddRemoveRequirements addRemoveRequirements)
        /// <summary>
        /// Clones the add/remove requirements.
        /// </summary>
        /// <param name="addRemoveRequirements">The add/remove requirements to clone.</param>
        public AddRemoveRequirements(AddRemoveRequirements addRemoveRequirements)
            : base()
        {
            if (addRemoveRequirements != null)
            {
                Database.Current.StartChange();

                if (addRemoveRequirements.AbstractEntityAdded != null)
                    this.AbstractEntityAdded = new AbstractEntityCondition(addRemoveRequirements.AbstractEntityAdded);
                if (addRemoveRequirements.AbstractEntityRemoved != null)
                    this.AbstractEntityRemoved = new AbstractEntityCondition(addRemoveRequirements.AbstractEntityRemoved);
                if (addRemoveRequirements.ConnectionItemAdded != null)
                    this.ConnectionItemAdded = new TangibleObjectCondition(addRemoveRequirements.ConnectionItemAdded);
                if (addRemoveRequirements.ConnectionItemRemoved != null)
                    this.ConnectionItemRemoved = new TangibleObjectCondition(addRemoveRequirements.ConnectionItemRemoved);
                if (addRemoveRequirements.CoverAdded != null)
                    this.CoverAdded = new TangibleObjectCondition(addRemoveRequirements.CoverAdded);
                if (addRemoveRequirements.CoverRemoved != null)
                    this.CoverRemoved = new TangibleObjectCondition(addRemoveRequirements.CoverRemoved);
                if (addRemoveRequirements.ElementAdded != null)
                    this.ElementAdded = new ElementCondition(addRemoveRequirements.ElementAdded);
                if (addRemoveRequirements.ElementRemoved != null)
                    this.ElementRemoved = new ElementCondition(addRemoveRequirements.ElementRemoved);
                if (addRemoveRequirements.ItemAdded != null)
                    this.ItemAdded = new TangibleObjectCondition(addRemoveRequirements.ItemAdded);
                if (addRemoveRequirements.ItemRemoved != null)
                    this.ItemRemoved = new TangibleObjectCondition(addRemoveRequirements.ItemRemoved);
                if (addRemoveRequirements.MatterAdded != null)
                    this.MatterAdded = addRemoveRequirements.MatterAdded.Clone();
                if (addRemoveRequirements.MatterRemoved != null)
                    this.MatterRemoved = addRemoveRequirements.MatterRemoved.Clone();
                if (addRemoveRequirements.LayerAdded != null)
                    this.LayerAdded = new LayerCondition(addRemoveRequirements.LayerAdded);
                if (addRemoveRequirements.LayerRemoved != null)
                    this.LayerRemoved = new LayerCondition(addRemoveRequirements.LayerRemoved);
                if (addRemoveRequirements.PartAdded != null)
                    this.PartAdded = new TangibleObjectCondition(addRemoveRequirements.PartAdded);
                if (addRemoveRequirements.PartRemoved != null)
                    this.PartRemoved = new TangibleObjectCondition(addRemoveRequirements.PartRemoved);
                if (addRemoveRequirements.RelationshipAdded != null)
                    this.RelationshipAdded = addRemoveRequirements.RelationshipAdded;
                if (addRemoveRequirements.RelationshipRemoved != null)
                    this.RelationshipRemoved = addRemoveRequirements.RelationshipRemoved;
                if (addRemoveRequirements.SpaceAdded != null)
                    this.SpaceAdded = new SpaceCondition(addRemoveRequirements.SpaceAdded);
                if (addRemoveRequirements.SpaceRemoved != null)
                    this.SpaceRemoved = new SpaceCondition(addRemoveRequirements.SpaceRemoved);
                if (addRemoveRequirements.SubstanceAdded != null)
                    this.SubstanceAdded =  new SubstanceCondition(addRemoveRequirements.SubstanceAdded);
                if (addRemoveRequirements.SubstanceRemoved != null)
                    this.SubstanceRemoved = new SubstanceCondition(addRemoveRequirements.SubstanceRemoved);
                if (addRemoveRequirements.TangibleMatterAdded != null)
                    this.TangibleMatterAdded = addRemoveRequirements.TangibleMatterAdded.Clone();
                if (addRemoveRequirements.ItemRemoved != null)
                    this.TangibleMatterRemoved = addRemoveRequirements.TangibleMatterRemoved.Clone();

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: AddRemoveRequirements(AddRemoveRequirements addRemoveRequirements)

        #endregion Method Group: Constructors

    }
    #endregion Class: AddRemoveRequirements

}