/**************************************************************************
 * 
 * TangibleObjectInstance.cs
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
using System.ComponentModel;
using Common;
using Semantics.Components;
using Semantics.Utilities;
using SemanticsEngine.Components;
using SemanticsEngine.Interfaces;
using SemanticsEngine.Tools;

namespace SemanticsEngine.Entities
{

    #region TODO Class: TangibleObjectInstance
    /// <summary>
    /// An instance of a tangible object.
    /// </summary>
    public class TangibleObjectInstance : PhysicalObjectInstance
    {

        #region Events, Properties, and Fields

        #region Events: PartHandler
        /// <summary>
        /// A handler for added or removed parts.
        /// </summary>
        /// <param name="sender">The tangible object instance the part was added to or removed from.</param>
        /// <param name="part">The added or removed part.</param>
        public delegate void PartHandler(TangibleObjectInstance sender, TangibleObjectInstance part);

        /// <summary>
        /// An event to indicate an added part.
        /// </summary>
        public event PartHandler PartAdded;

        /// <summary>
        /// An event to indicate a removed part.
        /// </summary>
        public event PartHandler PartRemoved;
        #endregion Events: PartHandler

        #region Events: ConnectionItemHandler
        /// <summary>
        /// A handler for added or removed connection items.
        /// </summary>
        /// <param name="sender">The tangible object instance the connection item was added to or removed from.</param>
        /// <param name="connectionItem">The added or removed connection item.</param>
        public delegate void ConnectionItemHandler(TangibleObjectInstance sender, TangibleObjectInstance connectionItem);

        /// <summary>
        /// An event to indicate an added connection item.
        /// </summary>
        public event ConnectionItemHandler ConnectionItemAdded;

        /// <summary>
        /// An event to indicate a removed connection item.
        /// </summary>
        public event ConnectionItemHandler ConnectionItemRemoved;
        #endregion Events: ConnectionItemHandler

        #region Events: CoverHandler
        /// <summary>
        /// A handler for added or removed covers.
        /// </summary>
        /// <param name="sender">The tangible object instance the cover was added to or removed from.</param>
        /// <param name="cover">The added or removed cover.</param>
        public delegate void CoverHandler(TangibleObjectInstance sender, TangibleObjectInstance cover);

        /// <summary>
        /// An event to indicate an added cover.
        /// </summary>
        public event CoverHandler CoverAdded;

        /// <summary>
        /// An event to indicate a removed cover.
        /// </summary>
        public event CoverHandler CoverRemoved;
        #endregion Events: CoverHandler

        #region Events: MatterHandler
        /// <summary>
        /// A handler for added or removed matter.
        /// </summary>
        /// <param name="sender">The tangible object instance the matter was added to or removed from.</param>
        /// <param name="matter">The added or removed matter.</param>
        public delegate void MatterHandler(TangibleObjectInstance sender, MatterInstance matter);

        /// <summary>
        /// An event to indicate an added matter.
        /// </summary>
        public event MatterHandler MatterAdded;

        /// <summary>
        /// An event to indicate a removed matter.
        /// </summary>
        public event MatterHandler MatterRemoved;
        #endregion Events: MatterHandler

        #region Events: LayerHandler
        /// <summary>
        /// A handler for added or removed layers.
        /// </summary>
        /// <param name="sender">The tangible object instance the layer was added to or removed from.</param>
        /// <param name="layer">The added or removed layer.</param>
        public delegate void LayerHandler(TangibleObjectInstance sender, MatterInstance layer);

        /// <summary>
        /// An event to indicate an added layer.
        /// </summary>
        public event LayerHandler LayerAdded;

        /// <summary>
        /// An event to indicate a removed layer.
        /// </summary>
        public event LayerHandler LayerRemoved;
        #endregion Events: LayerHandler

        #region Property: TangibleObjectBase
        /// <summary>
        /// Gets the tangible object base of which this is a tangible object instance.
        /// </summary>
        public TangibleObjectBase TangibleObjectBase
        {
            get
            {
                return this.NodeBase as TangibleObjectBase;
            }
        }
        #endregion Property: TangibleObjectBase

        #region Property: PartSlots
        /// <summary>
        /// All the slots that can be filled with parts.
        /// </summary>
        private PartSlot[] partSlots = null;

        /// <summary>
        /// All the slots that can be filled with parts.
        /// </summary>
        public ReadOnlyCollection<PartSlot> PartSlots
        {
            get
            {
                if (partSlots != null)
                    return new ReadOnlyCollection<PartSlot>(partSlots);

                return new List<PartSlot>(0).AsReadOnly();
            }
        }
        #endregion Property: PartSlots

        #region Property: Parts
        /// <summary>
        /// All the parts that are not slot-based.
        /// </summary>
        private TangibleObjectInstance[] parts = null;

        /// <summary>
        /// Gets the parts of this tangible object.
        /// </summary>
        public ReadOnlyCollection<TangibleObjectInstance> Parts
        {
            get
            {
                List<TangibleObjectInstance> allParts = new List<TangibleObjectInstance>();

                // Add the non-slot-based parts
                if (this.parts != null)
                    allParts.AddRange(this.parts);

                // Add the parts from the slots
                foreach (PartSlot partSlot in this.PartSlots)
                {
                    if (partSlot.Part != null)
                        allParts.Add(partSlot.Part);
                }

                return allParts.AsReadOnly();
            }
        }
        #endregion Property: Parts

        #region Property: ConnectionSlots
        /// <summary>
        /// All the slots that can be filled with connection items.
        /// </summary>
        private ConnectionSlot[] connectionSlots = null;

        /// <summary>
        /// All the slots that can be filled with connection items.
        /// </summary>
        public ReadOnlyCollection<ConnectionSlot> ConnectionSlots
        {
            get
            {
                if (connectionSlots != null)
                    return new ReadOnlyCollection<ConnectionSlot>(connectionSlots);

                return new List<ConnectionSlot>(0).AsReadOnly();
            }
        }
        #endregion Property: ConnectionSlots

        #region Property: Connections
        /// <summary>
        /// All the connections that are not slot-based.
        /// </summary>
        private TangibleObjectInstance[] connections = null;
        
        /// <summary>
        /// Gets the tangible objects to which this tangible object is connected.
        /// </summary>
        public ReadOnlyCollection<TangibleObjectInstance> Connections
        {
            get
            {
                List<TangibleObjectInstance> allConnections = new List<TangibleObjectInstance>();

                // Add the non-slot-based connections
                if (this.connections != null)
                    allConnections.AddRange(this.connections);

                // Add the connections from the slots
                foreach (ConnectionSlot connectionSlot in this.ConnectionSlots)
                {
                    if (connectionSlot.ConnectionItem != null)
                        allConnections.Add(connectionSlot.ConnectionItem);
                }

                return allConnections.AsReadOnly();
            }
        }
        #endregion Property: Connections

        #region Property: Matter
        /// <summary>
        /// All the matter of which this tangible object exists.
        /// </summary>
        private MatterInstance[] matter = null;
        
        /// <summary>
        /// Gets the matter of which this tangible object exists.
        /// </summary>
        public ReadOnlyCollection<MatterInstance> Matter
        {
            get
            {
                if (matter != null)
                    return new ReadOnlyCollection<MatterInstance>(matter);

                return new List<MatterInstance>(0).AsReadOnly();
            }
        }
        #endregion Property: Matter

        #region Property: Layers
        /// <summary>
        /// Gets the layers of the tangible object.
        /// </summary>
        public ReadOnlyCollection<MatterInstance> Layers
        {
            get
            {
                if (this.TangibleObjectBase != null)
                {
                    // Create instances from the base tangible objects
                    List<MatterInstance> layerInstances = new List<MatterInstance>();
                    foreach (LayerBase layerBase in this.TangibleObjectBase.Layers)
                    {
                        if (InstanceManager.IgnoreNecessity || layerBase.Necessity == Necessity.Mandatory)
                            layerInstances.Add(InstanceManager.Current.Create<MatterInstance>(layerBase));
                    }

                    // Replace the instances that have local modifications
                    return ReplaceByModifications<MatterInstance>("Layers", layerInstances, new PropertyChangedEventHandler(layerInstance_PropertyChanged)).AsReadOnly();
                }

                return new List<MatterInstance>(0).AsReadOnly();
            }
        }

        /// <summary>
        /// Add modified instances to the array with modifications.
        /// </summary>
        private void layerInstance_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            MatterInstance layerInstance = sender as MatterInstance;
            if (layerInstance != null)
                AddToModificationsArray<MatterInstance>("Layers", layerInstance);
        }
        #endregion Property: Layers

        #region Property: CoverSlots
        /// <summary>
        /// All the slots that can be filled with covers.
        /// </summary>
        private CoverSlot[] coverSlots = null;

        /// <summary>
        /// All the slots that can be filled with covers.
        /// </summary>
        public ReadOnlyCollection<CoverSlot> CoverSlots
        {
            get
            {
                if (coverSlots != null)
                    return new ReadOnlyCollection<CoverSlot>(coverSlots);

                return new List<CoverSlot>(0).AsReadOnly();
            }
        }
        #endregion Property: CoverSlots

        #region Property: Covers
        /// <summary>
        /// The covers of this tangible object.
        /// </summary>
        private TangibleObjectInstance[] covers = null;

        /// <summary>
        /// Gets the covers of this tangible object.
        /// </summary>
        public ReadOnlyCollection<TangibleObjectInstance> Covers
        {
            get
            {
                if (covers != null)
                {
                    List<TangibleObjectInstance> allCovers = new List<TangibleObjectInstance>();

                    // Add the non-slot-based covers
                    if (this.covers != null)
                        allCovers.AddRange(this.covers);

                    // Add the covers from the slots
                    foreach (CoverSlot coverSlot in this.CoverSlots)
                    {
                        if (coverSlot.Cover != null)
                            allCovers.Add(coverSlot.Cover);
                    }

                    return allCovers.AsReadOnly();
                }

                return new List<TangibleObjectInstance>(0).AsReadOnly();
            }
        }
        #endregion Property: Covers

        #region Property: Space
        /// <summary>
        /// The space in which this is an item.
        /// </summary>
        private SpaceInstance space = null;

        /// <summary>
        /// Gets the space in which this is an item.
        /// </summary>
        public SpaceInstance Space
        {
            get
            {
                return space;
            }
            internal set
            {
                if (space != value)
                {
                    space = value;
                    NotifyPropertyChanged("Space");
                }
            }
        }
        #endregion Property: Space

        #region Property: Whole
        /// <summary>
        /// The whole this tangible object instance is a part of.
        /// </summary>
        private TangibleObjectInstance whole = null;

        /// <summary>
        /// Gets the whole this tangible object instance is a part of.
        /// </summary>
        public TangibleObjectInstance Whole
        {
            get
            {
                return whole;
            }
            private set
            {
                if (whole != value)
                {
                    whole = value;
                    NotifyPropertyChanged("Whole");
                }
            }
        }
        #endregion Property: Whole

        #region Property: CoveredObject
        /// <summary>
        /// The tangible object instance this instance covers.
        /// </summary>
        private TangibleObjectInstance coveredObject = null;

        /// <summary>
        /// Gets the tangible object instance this instance covers.
        /// </summary>
        public TangibleObjectInstance CoveredObject
        {
            get
            {
                return coveredObject;
            }
            private set
            {
                if (coveredObject != value)
                {
                    coveredObject = value;
                    NotifyPropertyChanged("CoveredObject");
                }
            }
        }
        #endregion Property: CoveredObject

        #region Property: CoverIndex
        /// <summary>
        /// The index (the n-the cover) of all covers of the covered object, in case this tangible object instance acts as a cover.
        /// </summary>
        private int coverIndex = -1;

        /// <summary>
        /// Gets or sets the index (the n-the cover) of all covers of the covered object, in case this tangible object instance acts as a cover.
        /// </summary>
        public int CoverIndex
        {
            get
            {
                return coverIndex;
            }
            set
            {
                if (coverIndex != value)
                {
                    coverIndex = value;
                    NotifyPropertyChanged("CoverIndex");
                }
            }
        }
        #endregion Property: CoverIndex

        #region Property: IsCompound
        /// <summary>
        /// Gets the value that indicates whether this tangible object is a compound object (is made from parts).
        /// </summary>
        public bool IsCompound
        {
            get
            {
                return this.Parts.Count > 0;
            }
        }
        #endregion Property: IsCompound

        #region Property: Volume
        /// <summary>
        /// Gets the volume of the tangible object instance.
        /// </summary>
        public NumericalValueInstance Volume
        {
            get
            {
                return PhysicsManager.Current.GetVolume(this);
            }
        }
        #endregion Property: Volume

        #endregion Events, Properties, and Fields

        #region Method Group: Constructors

        #region Constructor: TangibleObjectInstance(TangibleObjectBase tangibleObjectBase)
        /// <summary>
        /// Creates a new tangible object instance from the given tangible object base.
        /// </summary>
        /// <param name="tangibleObjectBase">The tangible object base to create the tangible object instance from.</param>
        internal TangibleObjectInstance(TangibleObjectBase tangibleObjectBase)
            : this(tangibleObjectBase, SemanticsEngineSettings.DefaultCreateOptions)
        {
        }
        #endregion Constructor: TangibleObjectInstance(TangibleObjectBase tangibleObjectBase)

        #region Constructor: TangibleObjectInstance(TangibleObjectBase tangibleObjectBase, CreateOptions createOptions)
        /// <summary>
        /// Creates a new tangible object instance from the given tangible object base.
        /// </summary>
        /// <param name="tangibleObjectBase">The tangible object base to create the tangible object instance from.</param>
        /// <param name="createOptions">The create options.</param>
        internal TangibleObjectInstance(TangibleObjectBase tangibleObjectBase, CreateOptions createOptions)
            : this(tangibleObjectBase, createOptions, new List<TangibleObjectBase>())
        {
        }

        private TangibleObjectInstance(TangibleObjectBase tangibleObjectBase, CreateOptions createOptions, List<TangibleObjectBase> itemsToSkip)
            : base(tangibleObjectBase, createOptions)
        {
            // Create the parts, connections, space items, covers, spaces, and matter
            Create(createOptions, itemsToSkip);
        }
        #endregion Constructor: TangibleObjectInstance(TangibleObjectBase tangibleObjectBase, CreateOptions createOptions)

        #region Constructor: TangibleObjectInstance(PartBase partBase, CreateOptions createOptions)
        /// <summary>
        /// Creates a new tangible object instance from the given part base.
        /// </summary>
        /// <param name="partBase">The part base to create the tangible object instance from.</param>
        /// <param name="createOptions">The create options.</param>
        internal TangibleObjectInstance(PartBase partBase, CreateOptions createOptions)
            : base(partBase, createOptions)
        {
            if (partBase != null)
            {
                // Create the parts, connections, space items, covers, spaces, and matter
                Create(createOptions, new List<TangibleObjectBase>());
            }
        }
        #endregion Constructor: TangibleObjectInstance(PartBase partBase, CreateOptions createOptions)

        #region Constructor: TangibleObjectInstance(ConnectionItemBase connectionItemBase, CreateOptions createOptions)
        /// <summary>
        /// Creates a new tangible object instance from the given connection item base.
        /// </summary>
        /// <param name="connectionItemBase">The connection item base to create the tangible object instance from.</param>
        /// <param name="createOptions">The create options.</param>
        internal TangibleObjectInstance(ConnectionItemBase connectionItemBase, CreateOptions createOptions)
            : base(connectionItemBase, createOptions)
        {
            if (connectionItemBase != null)
            {
                // Create the parts, connections, space items, covers, spaces, and matter
                Create(createOptions, new List<TangibleObjectBase>());
            }
        }
        #endregion Constructor: TangibleObjectInstance(ConnectionItemBase connectionItemBase, CreateOptions createOptions)

        #region Constructor: TangibleObjectInstance(CoverBase coverBase, CreateOptions createOptions)
        /// <summary>
        /// Creates a new tangible object instance from the given cover base.
        /// </summary>
        /// <param name="coverBase">The cover base to create the tangible object instance from.</param>
        /// <param name="createOptions">The create options.</param>
        internal TangibleObjectInstance(CoverBase coverBase, CreateOptions createOptions)
            : base(coverBase, createOptions)
        {
            if (coverBase != null)
            {
                // Set the index of the cover
                this.CoverIndex = coverBase.Index;

                // Create the parts, connections, space items, covers, spaces, and matter
                Create(createOptions, new List<TangibleObjectBase>());
            }
        }
        #endregion Constructor: TangibleObjectInstance(CoverBase coverBase, CreateOptions createOptions)

        #region Constructor: TangibleObjectInstance(TangibleObjectInstance tangibleObjectInstance)
        /// <summary>
        /// Clones a tangible object instance.
        /// </summary>
        /// <param name="tangibleObjectInstance">The tangible object instance to clone.</param>
        protected internal TangibleObjectInstance(TangibleObjectInstance tangibleObjectInstance)
            : base(tangibleObjectInstance)
        {
            if (tangibleObjectInstance != null)
            {
                foreach (TangibleObjectInstance part in tangibleObjectInstance.Parts)
                    AddPart(new TangibleObjectInstance(part));
                foreach (TangibleObjectInstance connectionItem in tangibleObjectInstance.Connections)
                    CreateConnection(this, new TangibleObjectInstance(connectionItem));
                foreach (MatterInstance matter in tangibleObjectInstance.Matter)
                    AddMatter(matter.Clone());
                foreach (MatterInstance layerInstance in tangibleObjectInstance.Layers)
                    AddLayer(layerInstance.Clone());
                foreach (TangibleObjectInstance cover in tangibleObjectInstance.Covers)
                    AddCover(new TangibleObjectInstance(cover));
                this.Space = tangibleObjectInstance.Space;
                this.Whole = tangibleObjectInstance.Whole;
                this.CoveredObject = tangibleObjectInstance.CoveredObject;
                this.CoverIndex = tangibleObjectInstance.CoverIndex;
            }
        }
        #endregion Constructor: TangibleObjectInstance(TangibleObjectInstance tangibleObjectInstance)

        #region Method: Create(CreateOptions createOptions, List<TangibleObjectBase> itemsToSkip)
        /// <summary>
        /// Create the parts and connections.
        /// </summary>
        /// <param name="createOptions">The create options.</param>
        /// <param name="itemsToSkip">All items that should be skipped for instance creation.</param>
        private void Create(CreateOptions createOptions, List<TangibleObjectBase> itemsToSkip)
        {
            if (this.TangibleObjectBase != null)
            {
                // Parts
                if (createOptions.Has(CreateOptions.Parts))
                {
                    Vec3 position = this.Position;
                    Vec3 rotation = this.Rotation.ToEulerAngles();
                    foreach (PartBase partBase in this.TangibleObjectBase.Parts)
                    {
                        if (partBase.IsSlotBased && partBase.Quantity != null)
                        {
                            // Create the minimum amount of part slots
                            for (int i = 0; i < partBase.Quantity.GetMinimumHighestInteger(this); i++)
                                AddPartSlot(new PartSlot(partBase.TangibleObjectBase));
                        }

                        // Create instances from the mandatory base parts
                        if ((InstanceManager.IgnoreNecessity || partBase.Necessity == Necessity.Mandatory) && partBase.Quantity != null)
                        {
                            for (int i = 0; i < partBase.Quantity.GetRandomInteger(this); i++)
                            {
                                // Create a part and add it
                                TangibleObjectInstance partInstance = InstanceManager.Current.Create(partBase, createOptions);
                                AddPart(partInstance);

                                // Set the position of the part based on this position and the required offset
                                partInstance.Position = position + partBase.Offset.Vector.XYZ;

                                // Do the same for the rotation
                                Vec3 partRotation = rotation + partBase.Rotation.Vector.XYZ;
                                partInstance.Rotation = Quaternion.FromEulerAngles(partRotation.X, partRotation.Y, partRotation.Z);
                            }
                        }
                    }
                }

                // Covers
                if (createOptions.Has(CreateOptions.Covers))
                {
                    Vec3 position = this.Position;
                    Vec3 rotation = this.Rotation.ToEulerAngles();
                    foreach (CoverBase coverBase in this.TangibleObjectBase.Covers)
                    {
                        if (coverBase.IsSlotBased && coverBase.Quantity != null)
                        {
                            // Create the minimum amount of cover slots
                            for (int i = 0; i < coverBase.Quantity.GetMinimumHighestInteger(this); i++)
                                AddCoverSlot(new CoverSlot(coverBase.TangibleObjectBase));
                        }

                        // Create instances from the mandatory base covers
                        if (coverBase.Necessity == Necessity.Mandatory && coverBase.Quantity != null)
                        {
                            for (int i = 0; i < coverBase.Quantity.GetRandomInteger(this); i++)
                            {
                                TangibleObjectInstance coverInstance = InstanceManager.Current.Create(coverBase, createOptions);
                                AddCover(coverInstance);

                                // Set the position of the cover based on this position and the required offset
                                coverInstance.Position = position + coverBase.Offset.Vector.XYZ;

                                // Do the same for the rotation
                                Vec3 partRotation = rotation + coverBase.Rotation.Vector.XYZ;
                                coverInstance.Rotation = Quaternion.FromEulerAngles(partRotation.X, partRotation.Y, partRotation.Z);
                            }
                        }
                    }
                }

                // Connections
                if (createOptions.Has(CreateOptions.Connections))
                {
                    // Create instances from the mandatory base connection items, and establish the connection between this instance and the created instances;
                    // don't create connections of connections yet, to avoid infinite looping
                    List<Tuple<TangibleObjectBase, TangibleObjectInstance>> list = new List<Tuple<TangibleObjectBase, TangibleObjectInstance>>();
                    foreach (ConnectionItemBase connectionItemBase in this.TangibleObjectBase.Connections)
                    {
                        if (connectionItemBase.IsSlotBased && connectionItemBase.Quantity != null)
                        {
                            // Create a connection slot
                            for (int i = 0; i < connectionItemBase.Quantity.GetMinimumHighestInteger(this); i++)
                                AddConnectionSlot(new ConnectionSlot(connectionItemBase.TangibleObjectBase));
                        }

                        if (connectionItemBase.Necessity == Necessity.Mandatory)
                        {
                            if (!itemsToSkip.Contains(connectionItemBase.TangibleObjectBase) && connectionItemBase.Quantity != null)
                            {
                                for (int i = 0; i < connectionItemBase.Quantity.GetRandomInteger(this); i++)
                                {
                                    // Create the connection item
                                    createOptions = createOptions.Remove(CreateOptions.Connections);
                                    TangibleObjectInstance tangibleObjectInstance = new TangibleObjectInstance(connectionItemBase.TangibleObjectBase, createOptions);

                                    // Create a connection slot for the connection item
                                    tangibleObjectInstance.AddConnectionSlot(new ConnectionSlot(this.TangibleObjectBase));

                                    // Create the connection
                                    CreateConnection(this, tangibleObjectInstance);
                                    list.Add(new Tuple<TangibleObjectBase, TangibleObjectInstance>(connectionItemBase.TangibleObjectBase, tangibleObjectInstance));
                                }
                            }
                        }
                    }
                    // Now create the connections of the connections
                    foreach (Tuple<TangibleObjectBase, TangibleObjectInstance> tuple in list)
                    {
                        itemsToSkip.Add(tuple.Item1);
                        foreach (ConnectionItemBase connectionOfConnectionItemBase in tuple.Item1.Connections)
                        {
                            if (!connectionOfConnectionItemBase.TangibleObjectBase.Equals(this.TangibleObjectBase))
                            {
                                // Create the connection item
                                TangibleObjectInstance tangibleObjectInstance = new TangibleObjectInstance(connectionOfConnectionItemBase.TangibleObjectBase, createOptions, itemsToSkip);

                                // Create a connection slot for both
                                tuple.Item2.AddConnectionSlot(new ConnectionSlot(tangibleObjectInstance.TangibleObjectBase));
                                tangibleObjectInstance.AddConnectionSlot(new ConnectionSlot(tuple.Item2.TangibleObjectBase));

                                CreateConnection(tuple.Item2, tangibleObjectInstance);
                            }
                        }
                        itemsToSkip.Clear();
                    }
                    NotifyPropertyChanged("Connections");
                }

                // Matter
                if (createOptions.Has(CreateOptions.Matter))
                {
                    foreach (MatterValuedBase matterValuedBase in this.TangibleObjectBase.Matter)
                        AddMatter(InstanceManager.Current.Create<MatterInstance>(matterValuedBase));
                }
            }
        }
        #endregion Method: Create(CreateOptions createOptions, List<TangibleObjectBase> itemsToSkip)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddMatter(MatterInstance matterInstance)
        /// <summary>
        /// Adds a matter instance.
        /// </summary>
        /// <param name="matterInstance">The matter instance to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddMatter(MatterInstance matterInstance)
        {
            if (matterInstance != null)
            {
                // Check whether the matter is already there
                if (this.Matter.Contains(matterInstance))
                    return Message.RelationExistsAlready;

                // Add the matter
                Utils.AddToArray<MatterInstance>(ref this.matter, matterInstance);
                NotifyPropertyChanged("Matter");

                // Set the tangible object of the matter
                matterInstance.TangibleObject = this;

                // Set the position
                matterInstance.Position = new Vec3(this.Position);

                // Invoke an event
                if (MatterAdded != null)
                    MatterAdded.Invoke(this, matterInstance);

                // Notify the engine
                if (SemanticsEngine.Current != null)
                    SemanticsEngine.Current.HandleMatterAdded(this, matterInstance);

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddMatter(MatterInstance matterInstance)

        #region Method: RemoveMatter(MatterInstance matterInstance)
        /// <summary>
        /// Removes a matter instance.
        /// </summary>
        /// <param name="matterInstance">The matter instance to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveMatter(MatterInstance matterInstance)
        {
            if (matterInstance != null)
            {
                if (this.Matter.Contains(matterInstance))
                {
                    // Remove the matter
                    Utils.RemoveFromArray<MatterInstance>(ref this.matter, matterInstance);
                    NotifyPropertyChanged("Matter");

                    // Reset the tangible object of the matter
                    matterInstance.TangibleObject = null;

                    // Invoke an event
                    if (MatterRemoved != null)
                        MatterRemoved.Invoke(this, matterInstance);

                    // Notify the engine
                    if (SemanticsEngine.Current != null)
                        SemanticsEngine.Current.HandleMatterRemoved(this, matterInstance);

                    // If there is no matter left, remove the tangible object
                    if (this.Matter.Count == 0 && this.World != null)
                        this.World.RemoveInstance(this, false);

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveMatter(MatterInstance matterInstance)

        #region Method: AddPartSlot(PartSlot partSlot)
        /// <summary>
        /// Add a part slot.
        /// </summary>
        /// <param name="partSlot">The part slot to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddPartSlot(PartSlot partSlot)
        {
            if (partSlot != null)
            {
                // Check whether the part is already there
                if (this.PartSlots.Contains(partSlot))
                    return Message.RelationExistsAlready;

                // Add the part slot
                Utils.AddToArray<PartSlot>(ref this.partSlots, partSlot);
                NotifyPropertyChanged("PartSlots");

                return Message.RelationSuccess;
            }

            return Message.RelationFail;
        }
        #endregion Method: AddPartSlot(PartSlot partSlot)

        #region Method: AddPart(TangibleObjectInstance part)
        /// <summary>
        /// Add a part.
        /// </summary>
        /// <param name="part">The part to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddPart(TangibleObjectInstance part)
        {
            if (part != null && part.TangibleObjectBase != null && this.TangibleObjectBase != null)
            {
                // Check whether the part is already there
                if (this.Parts.Contains(part))
                    return Message.RelationExistsAlready;

                // Check whether this is a valid part
                bool success = false;
                PartBase matchingPart = null;
                foreach (PartBase partBase in this.TangibleObjectBase.Parts)
                {
                    if (part.TangibleObjectBase.IsNodeOf(partBase.TangibleObjectBase))
                    {
                        matchingPart = partBase;

                        // Check whether this part is slot-based
                        if (partBase.IsSlotBased)
                        {
                            // Add the part by finding a free slot for it
                            foreach (PartSlot partSlot in this.PartSlots)
                            {
                                if (partSlot.IsEmpty && part.TangibleObjectBase.Equals(partSlot.PartBase))
                                {
                                    // Set the part
                                    partSlot.Part = part;
                                    success = true;
                                    break;
                                }
                            }

                            if (!success)
                            {
                                // Create a new part slot if the quantity is not exceeded
                                int currentQuantity = 0;
                                foreach (TangibleObjectInstance currentPart in this.Parts)
                                {
                                    if (currentPart.TangibleObjectBase.IsNodeOf(partBase.TangibleObjectBase))
                                        currentQuantity++;
                                }
                                if (Toolbox.Compare(currentQuantity + 1, partBase.Quantity.ValueSign, partBase.Quantity.BaseValue, partBase.Quantity.BaseValue2))
                                {
                                    // Create a filled part slot and add it
                                    PartSlot partSlot = new PartSlot(partBase.TangibleObjectBase);
                                    partSlot.Part = part;
                                    AddPartSlot(partSlot);
                                    success = true;
                                }
                            }
                        }
                        else
                        {
                            // Count the number of similar parts
                            int currentQuantity = 0;
                            foreach (TangibleObjectInstance currentPart in this.Parts)
                            {
                                if (currentPart.TangibleObjectBase.IsNodeOf(partBase.TangibleObjectBase))
                                    currentQuantity++;
                            }
                            // Check whether we can still add the part
                            if (Toolbox.Compare(currentQuantity + 1, partBase.Quantity.ValueSign, partBase.Quantity.BaseValue, partBase.Quantity.BaseValue2))
                            {
                                // Add it
                                Utils.AddToArray<TangibleObjectInstance>(ref this.parts, part);
                                success = true;
                                break;
                            }
                        }
                        break;
                    }
                }

                if (success)
                {
                    NotifyPropertyChanged("Parts");

                    // Make this instance the whole of the part
                    part.Whole = this;

                    // Set the world if this has not been done before
                    if (part.World == null && this.World != null)
                        this.World.AddInstance(part);

                    // Set the position and rotation, based on the offset
                    if (matchingPart != null)
                    {
                        part.Position = this.Position + matchingPart.Offset.Vector.XYZ;
                        Vec3 partRotation = this.Rotation.ToEulerAngles() + matchingPart.Rotation.Vector.XYZ;
                        part.Rotation = Quaternion.FromEulerAngles(partRotation.X, partRotation.Y, partRotation.Z);
                    }
                    else
                        part.Position = new Vec3(this.Position);

                    // Invoke an event
                    if (PartAdded != null)
                        PartAdded.Invoke(this, part);

                    // Notify the engine
                    if (SemanticsEngine.Current != null)
                        SemanticsEngine.Current.HandlePartAdded(this, part);

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: AddPart(TangibleObjectInstance part)

        #region Method: RemovePart(TangibleObjectInstance part)
        /// <summary>
        /// Removes a part.
        /// </summary>
        /// <param name="part">The part to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemovePart(TangibleObjectInstance part)
        {
            if (part != null)
            {
                if (this.Parts.Contains(part))
                {
                    // Remove the part from its slot
                    bool removedFromSlot = false;
                    foreach (PartSlot partSlot in this.PartSlots)
                    {
                        if (part.Equals(partSlot.Part))
                        {
                            partSlot.Part = null;
                            removedFromSlot = true;
                            break;
                        }
                    }
                    // Or remove the part from the non-slot-based parts
                    if (!removedFromSlot && this.parts != null)
                        Utils.RemoveFromArray<TangibleObjectInstance>(ref this.parts, part);

                    NotifyPropertyChanged("Parts");

                    // Reset the whole of the part
                    part.Whole = null;

                    // Invoke an event
                    if (PartRemoved != null)
                        PartRemoved.Invoke(this, part);

                    // Notify the engine
                    if (SemanticsEngine.Current != null)
                        SemanticsEngine.Current.HandlePartRemoved(this, part);

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemovePart(TangibleObjectInstance part)

        #region Method: AddConnectionSlot(ConnectionSlot connectionSlot)
        /// <summary>
        /// Add a connection slot.
        /// </summary>
        /// <param name="connectionSlot">The connection slot to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddConnectionSlot(ConnectionSlot connectionSlot)
        {
            if (connectionSlot != null)
            {
                // Check whether the connection is already there
                if (this.ConnectionSlots.Contains(connectionSlot))
                    return Message.RelationExistsAlready;

                // Add the connection slot
                Utils.AddToArray<ConnectionSlot>(ref this.connectionSlots, connectionSlot);
                NotifyPropertyChanged("ConnectionSlots");

                return Message.RelationSuccess;
            }

            return Message.RelationFail;
        }
        #endregion Method: AddConnectionSlot(ConnectionSlot partSlot)

        #region Method: CreateConnection(TangibleObjectInstance connectionItem1, TangibleObjectInstance connectionItem2)
        /// <summary>
        /// Create a connection between both items.
        /// </summary>
        /// <param name="connectionItem1">The item to connect.</param>
        /// <param name="connectionItem2">The item to connect with.</param>
        /// <returns>Returns whether the connection has made been successful.</returns>
        public static Message CreateConnection(TangibleObjectInstance connectionItem1, TangibleObjectInstance connectionItem2)
        {
            // Check if both items are not the same, and if the connection does not exist already
            if (connectionItem1 != null && connectionItem2 != null && !connectionItem1.Equals(connectionItem2))
            {
                bool firstHas = connectionItem1.Connections.Contains(connectionItem2);
                bool secondHas = connectionItem2.Connections.Contains(connectionItem1);

                if (firstHas && secondHas)
                    return Message.RelationExistsAlready;

                // Create the connection
                bool success = false;
                if (!firstHas)
                {
                    // Check whether this is a valid connection item
                    bool success2 = false;
                    foreach (ConnectionItemBase connectionItemBase in connectionItem1.TangibleObjectBase.Connections)
                    {
                        if (connectionItem2.TangibleObjectBase.IsNodeOf(connectionItemBase.TangibleObjectBase))
                        {
                            // Check whether this connection item is slot-based
                            if (connectionItemBase.IsSlotBased)
                            {
                                // Add the connection item by finding a free slot for it
                                foreach (ConnectionSlot connectionSlot in connectionItem1.ConnectionSlots)
                                {
                                    if (connectionSlot.IsEmpty && connectionItem2.TangibleObjectBase.Equals(connectionSlot.ConnectionItemBase))
                                    {
                                        // Set the connection item
                                        connectionSlot.ConnectionItem = connectionItem2;
                                        success2 = true;
                                        break;
                                    }
                                }

                                if (!success2)
                                {
                                    // Create a new connection item slot if the quantity is not exceeded
                                    int currentQuantity = 0;
                                    foreach (TangibleObjectInstance currentConnectionItem in connectionItem1.Connections)
                                    {
                                        if (currentConnectionItem.TangibleObjectBase.IsNodeOf(connectionItemBase.TangibleObjectBase))
                                            currentQuantity++;
                                    }
                                    if (Toolbox.Compare(currentQuantity + 1, connectionItemBase.Quantity.ValueSign, connectionItemBase.Quantity.BaseValue, connectionItemBase.Quantity.BaseValue2))
                                    {
                                        // Create a filled connection item slot and add it
                                        ConnectionSlot connectionSlot = new ConnectionSlot(connectionItemBase.TangibleObjectBase);
                                        connectionSlot.ConnectionItem = connectionItem2;
                                        connectionItem1.AddConnectionSlot(connectionSlot);
                                        success2 = true;
                                    }
                                }
                            }
                            else
                            {
                                // Count the number of similar connection items
                                int currentQuantity = 0;
                                foreach (TangibleObjectInstance currentConnectionItem in connectionItem1.Connections)
                                {
                                    if (currentConnectionItem.TangibleObjectBase.IsNodeOf(connectionItemBase.TangibleObjectBase))
                                        currentQuantity++;
                                }
                                // Check whether we can still add the connection item
                                if (Toolbox.Compare(currentQuantity + 1, connectionItemBase.Quantity.ValueSign, connectionItemBase.Quantity.BaseValue, connectionItemBase.Quantity.BaseValue2))
                                {
                                    // Add it
                                    Utils.AddToArray<TangibleObjectInstance>(ref connectionItem1.connections, connectionItem2);
                                    success2 = true;
                                    break;
                                }
                            }

                            break;
                        }
                    }
                    if (success2)
                    {
                        // Set the connection item
                        connectionItem1.NotifyPropertyChanged("Connections");

                        // Set the world if this has not been done before
                        if (connectionItem2.World == null && connectionItem1.World != null)
                            connectionItem1.World.AddInstance(connectionItem2);

                        // Invoke an event
                        if (connectionItem1.ConnectionItemAdded != null)
                            connectionItem1.ConnectionItemAdded.Invoke(connectionItem1, connectionItem2);

                        // Notify the engine
                        if (SemanticsEngine.Current != null)
                            SemanticsEngine.Current.HandleConnectionItemAdded(connectionItem1, connectionItem2);

                        success = true;
                    }
                }
                if (success && !secondHas)
                {
                    // Check whether this is a valid connection item
                    bool success2 = false;
                    foreach (ConnectionItemBase connectionItemBase in connectionItem2.TangibleObjectBase.Connections)
                    {
                        if (connectionItem1.TangibleObjectBase.IsNodeOf(connectionItemBase.TangibleObjectBase))
                        {
                            // Check whether this connection item is slot-based
                            if (connectionItemBase.IsSlotBased)
                            {
                                // Add the connection item by finding a free slot for it
                                foreach (ConnectionSlot connectionSlot in connectionItem2.ConnectionSlots)
                                {
                                    if (connectionSlot.IsEmpty && connectionItem1.TangibleObjectBase.Equals(connectionSlot.ConnectionItemBase))
                                    {
                                        // Set the connection item
                                        connectionSlot.ConnectionItem = connectionItem1;
                                        success2 = true;
                                        break;
                                    }
                                }

                                if (!success2)
                                {
                                    // Create a new connection item slot if the quantity is not exceeded
                                    int currentQuantity = 0;
                                    foreach (TangibleObjectInstance currentConnectionItem in connectionItem2.Connections)
                                    {
                                        if (currentConnectionItem.TangibleObjectBase.IsNodeOf(connectionItemBase.TangibleObjectBase))
                                            currentQuantity++;
                                    }
                                    if (Toolbox.Compare(currentQuantity + 1, connectionItemBase.Quantity.ValueSign, connectionItemBase.Quantity.BaseValue, connectionItemBase.Quantity.BaseValue2))
                                    {
                                        // Create a filled connection item slot and add it
                                        ConnectionSlot connectionSlot = new ConnectionSlot(connectionItemBase.TangibleObjectBase);
                                        connectionSlot.ConnectionItem = connectionItem1;
                                        connectionItem2.AddConnectionSlot(connectionSlot);
                                        success2 = true;
                                    }
                                }
                            }
                            else
                            {
                                // Count the number of similar connection items
                                int currentQuantity = 0;
                                foreach (TangibleObjectInstance currentConnectionItem in connectionItem2.Connections)
                                {
                                    if (currentConnectionItem.TangibleObjectBase.IsNodeOf(connectionItemBase.TangibleObjectBase))
                                        currentQuantity++;
                                }
                                // Check whether we can still add the connection item
                                if (Toolbox.Compare(currentQuantity + 1, connectionItemBase.Quantity.ValueSign, connectionItemBase.Quantity.BaseValue, connectionItemBase.Quantity.BaseValue2))
                                {
                                    // Add it
                                    Utils.AddToArray<TangibleObjectInstance>(ref connectionItem2.connections, connectionItem1);
                                    success2 = true;
                                    break;
                                }
                            }

                            break;
                        }
                    }
                    if (success2)
                    {
                        // Set the connection item
                        connectionItem2.NotifyPropertyChanged("Connections");

                        // Set the world if this has not been done before
                        if (connectionItem1.World == null && connectionItem2.World != null)
                            connectionItem2.World.AddInstance(connectionItem1);

                        // Invoke an event
                        if (connectionItem2.ConnectionItemAdded != null)
                            connectionItem2.ConnectionItemAdded.Invoke(connectionItem2, connectionItem1);

                        // Notify the engine
                        if (SemanticsEngine.Current != null)
                            SemanticsEngine.Current.HandleConnectionItemAdded(connectionItem2, connectionItem1);

                        success = true;
                    }
                }

                if (success)
                    return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: CreateConnection(TangibleObjectInstance connectionItem1, TangibleObjectInstance connectionItem2)

        #region Method: RemoveConnection(TangibleObjectInstance connectionItem1, TangibleObjectInstance connectionItem2)
        /// <summary>
        /// Removes the connection between the given items.
        /// </summary>
        /// <param name="connectionItem1">The item to disconnect.</param>
        /// <param name="connectionItem2">The item to disconnect from.</param>
        /// <returns>Returns whether the connection has been removed successfully.</returns>
        public static Message RemoveConnection(TangibleObjectInstance connectionItem1, TangibleObjectInstance connectionItem2)
        {
            if (connectionItem1 != null && connectionItem2 != null)
            {
                bool firstHas = connectionItem1.Connections.Contains(connectionItem2);
                bool secondHas = connectionItem2.Connections.Contains(connectionItem1);
                if (!firstHas && !secondHas)
                    return Message.RelationFail;

                // Remove the connection
                bool success = false;
                if (firstHas)
                {
                    // Remove the connection item from its slot
                    bool removedFromSlot = false;
                    foreach (ConnectionSlot connectionSlot in connectionItem1.ConnectionSlots)
                    {
                        if (connectionItem2.Equals(connectionSlot.ConnectionItem))
                        {
                            connectionSlot.ConnectionItem = null;
                            removedFromSlot = true;
                            break;
                        }
                    }
                    // Or remove the connection item from the non-slot-based connection items
                    if (!removedFromSlot && connectionItem1.connections != null)
                        Utils.RemoveFromArray<TangibleObjectInstance>(ref connectionItem1.connections, connectionItem2);

                    connectionItem1.NotifyPropertyChanged("Connections");

                    // Invoke an event
                    if (connectionItem1.ConnectionItemRemoved != null)
                        connectionItem1.ConnectionItemRemoved.Invoke(connectionItem1, connectionItem2);

                    // Notify the engine
                    if (SemanticsEngine.Current != null)
                        SemanticsEngine.Current.HandleConnectionItemRemoved(connectionItem1, connectionItem2);

                    success = true;
                }
                if (success && secondHas)
                {
                    // Remove the connection item from its slot
                    bool removedFromSlot = false;
                    foreach (ConnectionSlot connectionSlot in connectionItem2.ConnectionSlots)
                    {
                        if (connectionItem1.Equals(connectionSlot.ConnectionItem))
                        {
                            connectionSlot.ConnectionItem = null;
                            removedFromSlot = true;
                            break;
                        }
                    }
                    // Or remove the connection item from the non-slot-based connection items
                    if (!removedFromSlot && connectionItem2.connections != null)
                        Utils.RemoveFromArray<TangibleObjectInstance>(ref connectionItem2.connections, connectionItem1);

                    connectionItem2.NotifyPropertyChanged("Connections");

                    // Invoke an event
                    if (connectionItem2.ConnectionItemRemoved != null)
                        connectionItem2.ConnectionItemRemoved.Invoke(connectionItem2, connectionItem1);

                    // Notify the engine
                    if (SemanticsEngine.Current != null)
                        SemanticsEngine.Current.HandleConnectionItemRemoved(connectionItem2, connectionItem1);
                }

                if (success)
                    return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveConnection(TangibleObjectInstance connectionItem1, TangibleObjectInstance connectionItem2)

        #region Method: AddLayer(MatterInstance layerInstance)
        /// <summary>
        /// Adds a layer instance.
        /// </summary>
        /// <param name="layerInstance">The layer instance to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        protected internal Message AddLayer(MatterInstance layerInstance)
        {
            if (layerInstance != null)
            {
                // If the layer is already present, there's no use to add it again
                if (this.Layers.Contains(layerInstance))
                    return Message.RelationExistsAlready;

                // Add the layer
                AddToArrayProperty<MatterInstance>("Layers", layerInstance);

                // Make this instance the applicant of the layer
                layerInstance.Applicant = this;

                // Set the layer index to the highest index plus one
                int highestLayerIndex = -1;
                foreach (MatterInstance existingLayer in this.Layers)
                {
                    if (!layerInstance.Equals(existingLayer))
                    {
                        if (existingLayer.LayerIndex > highestLayerIndex)
                            highestLayerIndex = existingLayer.LayerIndex;
                    }
                }
                if (highestLayerIndex <= 1)
                    layerInstance.LayerIndex = 1;
                else
                    layerInstance.LayerIndex = highestLayerIndex + 1;

                // Set the position
                layerInstance.Position = new Vec3(this.Position);

                // Invoke an event
                if (LayerAdded != null)
                    LayerAdded.Invoke(this, layerInstance);

                // Notify the engine
                if (SemanticsEngine.Current != null)
                    SemanticsEngine.Current.HandleLayerAdded(this, layerInstance);

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddLayer(MatterInstance layerInstance)

        #region Method: RemoveLayer(MatterInstance layerInstance)
        /// <summary>
        /// Removes a layer instance.
        /// </summary>
        /// <param name="layerInstance">The layer instance to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        protected internal Message RemoveLayer(MatterInstance layerInstance)
        {
            if (layerInstance != null)
            {
                if (this.Layers.Contains(layerInstance))
                {
                    // Remove the layer
                    RemoveFromArrayProperty<MatterInstance>("Layers", layerInstance);

                    // Reset the applicant of the layer
                    layerInstance.Applicant = null;

                    // Reset the layer index
                    layerInstance.LayerIndex = -1;

                    // Invoke an event
                    if (LayerRemoved != null)
                        LayerRemoved.Invoke(this, layerInstance);

                    // Notify the engine
                    if (SemanticsEngine.Current != null)
                        SemanticsEngine.Current.HandleLayerRemoved(this, layerInstance);

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveLayer(MatterInstance layerInstance)

        #region Method: AddCoverSlot(CoverSlot coverSlot)
        /// <summary>
        /// Add a cover slot.
        /// </summary>
        /// <param name="coverSlot">The cover slot to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddCoverSlot(CoverSlot coverSlot)
        {
            if (coverSlot != null)
            {
                // Check whether the cover is already there
                if (this.CoverSlots.Contains(coverSlot))
                    return Message.RelationExistsAlready;

                // Add the cover slot
                Utils.AddToArray<CoverSlot>(ref this.coverSlots, coverSlot);
                NotifyPropertyChanged("CoverSlots");

                return Message.RelationSuccess;
            }

            return Message.RelationFail;
        }
        #endregion Method: AddCoverSlot(CoverSlot coverSlot)

        #region Method: AddCover(TangibleObjectInstance cover)
        /// <summary>
        /// Add a cover.
        /// </summary>
        /// <param name="cover">The cover to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddCover(TangibleObjectInstance cover)
        {
            if (cover != null)
            {
                // Check whether the cover is already there
                if (this.Covers.Contains(cover))
                    return Message.RelationExistsAlready;

                // Check whether this is a valid cover
                bool success = false;
                CoverBase matchingCover = null;
                foreach (CoverBase coverBase in this.TangibleObjectBase.Covers)
                {
                    if (cover.TangibleObjectBase.IsNodeOf(coverBase.TangibleObjectBase))
                    {
                        matchingCover = coverBase;

                        // Check whether this cover is slot-based
                        if (coverBase.IsSlotBased)
                        {
                            // Add the cover by finding a free slot for it
                            foreach (CoverSlot coverSlot in this.CoverSlots)
                            {
                                if (coverSlot.IsEmpty && cover.TangibleObjectBase.Equals(coverSlot.CoverBase))
                                {
                                    // Set the cover
                                    coverSlot.Cover = cover;
                                    success = true;
                                    break;
                                }
                            }

                            if (!success)
                            {
                                // Create a new cover slot if the quantity is not exceeded
                                int currentQuantity = 0;
                                foreach (TangibleObjectInstance currentCover in this.Covers)
                                {
                                    if (currentCover.TangibleObjectBase.IsNodeOf(coverBase.TangibleObjectBase))
                                        currentQuantity++;
                                }
                                if (Toolbox.Compare(currentQuantity + 1, coverBase.Quantity.ValueSign, coverBase.Quantity.BaseValue, coverBase.Quantity.BaseValue2))
                                {
                                    // Create a filled cover slot and add it
                                    CoverSlot coverSlot = new CoverSlot(coverBase.TangibleObjectBase);
                                    coverSlot.Cover = cover;
                                    AddCoverSlot(coverSlot);
                                    success = true;
                                }
                            }
                        }
                        else
                        {
                            // Count the number of similar covers
                            int currentQuantity = 0;
                            foreach (TangibleObjectInstance currentCover in this.Covers)
                            {
                                if (currentCover.TangibleObjectBase.IsNodeOf(coverBase.TangibleObjectBase))
                                    currentQuantity++;
                            }
                            // Check whether we can still add the cover
                            if (Toolbox.Compare(currentQuantity + 1, coverBase.Quantity.ValueSign, coverBase.Quantity.BaseValue, coverBase.Quantity.BaseValue2))
                            {
                                // Add it
                                Utils.AddToArray<TangibleObjectInstance>(ref this.covers, cover);
                                success = true;
                                break;
                            }
                        }
                        break;
                    }
                }

                if (success)
                {
                    // Make this instance the covered instance of the cover
                    cover.CoveredObject = this;

                    // Set the cover index to the highest index plus one
                    int highestCoverIndex = -1;
                    foreach (TangibleObjectInstance existingCover in this.Covers)
                    {
                        if (!cover.Equals(existingCover))
                        {
                            if (existingCover.CoverIndex > highestCoverIndex)
                                highestCoverIndex = existingCover.CoverIndex;
                        }
                    }
                    if (highestCoverIndex <= 1)
                        cover.CoverIndex = 1;
                    else
                        cover.CoverIndex = highestCoverIndex + 1;

                    NotifyPropertyChanged("Covers");

                    // Set the world if this has not been done before
                    if (cover.World == null && this.World != null)
                        this.World.AddInstance(cover);

                    // Set the position and rotation, based on the offset
                    if (matchingCover != null)
                    {
                        cover.Position = this.Position + matchingCover.Offset.Vector.XYZ;
                        Vec3 coverRotation = this.Rotation.ToEulerAngles() + matchingCover.Rotation.Vector.XYZ;
                        cover.Rotation = Quaternion.FromEulerAngles(coverRotation.X, coverRotation.Y, coverRotation.Z);
                    }
                    else
                        cover.Position = new Vec3(this.Position);
                    
                    // Invoke an event
                    if (CoverAdded != null)
                        CoverAdded.Invoke(this, cover);

                    // Notify the engine
                    if (SemanticsEngine.Current != null)
                        SemanticsEngine.Current.HandleCoverAdded(this, cover);

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: AddCover(TangibleObjectInstance cover)

        #region Method: RemoveCover(TangibleObjectInstance cover)
        /// <summary>
        /// Removes a cover.
        /// </summary>
        /// <param name="cover">The cover to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveCover(TangibleObjectInstance cover)
        {
            if (cover != null)
            {
                if (this.Covers.Contains(cover))
                {
                    // Remove the cover from its slot
                    bool removedFromSlot = false;
                    foreach (CoverSlot coverSlot in this.CoverSlots)
                    {
                        if (cover.Equals(coverSlot.Cover))
                        {
                            coverSlot.Cover = null;
                            removedFromSlot = true;
                            break;
                        }
                    }
                    // Or remove the cover from the non-slot-based covers
                    if (!removedFromSlot && this.covers != null)
                        Utils.RemoveFromArray<TangibleObjectInstance>(ref this.covers, cover);

                    NotifyPropertyChanged("Covers");

                    // Reset the covered instance of the cover
                    cover.CoveredObject = null;

                    // Reset the cover index
                    cover.CoverIndex = -1;

                    // Invoke an event
                    if (CoverRemoved != null)
                        CoverRemoved.Invoke(this, cover);

                    // Notify the engine
                    if (SemanticsEngine.Current != null)
                        SemanticsEngine.Current.HandleCoverRemoved(this, cover);

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveCover(TangibleObjectInstance cover)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasPart(TangibleObjectBase part)
        /// <summary>
        /// Checks if this tangible object has the given tangible object as a part.
        /// </summary>
        /// <param name="part">The tangible object to check.</param>
        /// <returns>Returns true when the tangible object has the tangible object as a part.</returns>
        public bool HasPart(TangibleObjectBase part)
        {
            if (part != null)
            {
                foreach (TangibleObjectInstance tangibleObjectInstance in this.Parts)
                {
                    if (part.Equals(tangibleObjectInstance.TangibleObjectBase))
                        return true;
                }
            }

            return false;
        }
        #endregion Method: HasPart(TangibleObjectBase part)

        #region Method: HasConnection(TangibleObjectBase connectionItem)
        /// <summary>
        /// Checks if this tangible object instance has a connection with an instance of the given tangible object.
        /// </summary>
        /// <param name="connectionItem">The connection item to check.</param>
        /// <returns>Returns true when this tangible object instance has a connection with an instance of the tangible object.</returns>
        public bool HasConnection(TangibleObjectBase connectionItem)
        {
            if (connectionItem != null)
            {
                foreach (TangibleObjectInstance tangibleObjectInstance in this.Connections)
                {
                    if (connectionItem.Equals(tangibleObjectInstance.TangibleObjectBase))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasConnection(TangibleObjectBase connectionItem)

        #region Method: HasLayer(MatterBase matter)
        /// <summary>
        /// Checks if this tangible object has the given matter as a layer.
        /// </summary>
        /// <param name="matter">The matter to check.</param>
        /// <returns>Returns true when this tangible object has the matter as a layer.</returns>
        public bool HasLayer(MatterBase matter)
        {
            if (matter != null)
            {
                foreach (MatterInstance layer in this.Layers)
                {
                    if (matter.Equals(layer.MatterBase))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasLayer(MatterBase matter)

        #region Method: HasCover(TangibleObjectBase cover)
        /// <summary>
        /// Checks if this tangible object has the given tangible object as a cover.
        /// </summary>
        /// <param name="cover">The tangible object to check.</param>
        /// <returns>Returns true when the tangible object has the tangible object as a cover.</returns>
        public bool HasCover(TangibleObjectBase cover)
        {
            if (cover != null)
            {
                foreach (TangibleObjectInstance tangibleObjectInstance in this.Covers)
                {
                    if (cover.Equals(tangibleObjectInstance.TangibleObjectBase))
                        return true;
                }
            }

            return false;
        }
        #endregion Method: HasCover(TangibleObjectBase cover)

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
                foreach (TangibleObjectInstance part in this.Parts)
                    part.MarkAsModified(modified, false);
                foreach (TangibleObjectInstance cover in this.Covers)
                    cover.MarkAsModified(modified, false);
                foreach (TangibleObjectInstance connectionItem in this.Connections)
                    connectionItem.MarkAsModified(modified, false);
                foreach (MatterInstance matter in this.Matter)
                    matter.MarkAsModified(modified, false);
                foreach (MatterInstance layer in this.Layers)
                    layer.MarkAsModified(modified, false);

                if (this.Space != null)
                    this.Space.MarkAsModified(modified, false);
                if (this.Whole != null)
                    this.Whole.MarkAsModified(modified, false);
                if (this.CoveredObject != null)
                    this.CoveredObject.MarkAsModified(modified, false);
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

            // Then move all the parts, covers, matter, and layers
            foreach (TangibleObjectInstance part in this.Parts)
                part.Position += delta;
            foreach (TangibleObjectInstance cover in this.Covers)
                cover.Position += delta;
            foreach (MatterInstance matter in this.Matter)
                matter.Position += delta;
            foreach (MatterInstance layer in this.Layers)
                layer.Position += delta;
        }
        #endregion Method: SetPosition(Vec3 newPosition)

        #region TODO Method: SetRotation(Quaternion newRotation)
        /// <summary>
        /// TODO: is er andere manier om de delta rotation te kennen dan die te kopieren in dit field?
        /// </summary>
        private Quaternion previousRotation = new Quaternion();

        /// <summary>
        /// Set the new rotation.
        /// </summary>
        /// <param name="newRotation">The new rotation.</param>
        protected override void SetRotation(Quaternion newRotation)
        {
            base.SetRotation(newRotation);

            float deltaYaw = newRotation.Yaw - previousRotation.Yaw;
            float deltaPitch = newRotation.Pitch - previousRotation.Pitch;
            float deltaRoll = newRotation.Roll - previousRotation.Roll;

            // Then rotate all the parts and covers
            foreach (TangibleObjectInstance part in this.Parts)
            {
                // TODO: When we assume an offset -> this should be different
                if ((part.Position - this.Position).length() > float.Epsilon)
                {
                    if (deltaYaw != 0)
                    {
                        if (deltaPitch != 0 || deltaRoll != 0)
                            throw new System.NotImplementedException();
                        else
                        {
                            Vec3 offset = part.Position - this.Position;
                            Vec3 newOffset = Matrix4.RotationmY(deltaYaw) * offset;
                            part.Position = this.Position + newOffset;
                        }
                    }
                    else if (deltaPitch != 0 || deltaRoll != 0)
                        throw new System.NotImplementedException();
                }
                part.Rotation.Yaw += deltaYaw;
                part.Rotation.Pitch += deltaPitch;
                part.Rotation.Roll += deltaRoll;
            }
            foreach (TangibleObjectInstance cover in this.Covers)
            {
                // TODO: When we assume an offset -> this should be different
                if ((cover.Position - this.Position).length() > float.Epsilon)
                    throw new System.NotImplementedException();
                cover.Rotation = newRotation;
            }
            previousRotation = new Quaternion(newRotation);
        }
        #endregion Method: SetRotation(Quaternion newRotation)

        #region Method: SetIsLocked(bool isLockedValue)
        /// <summary>
        /// Set the new value for IsLocked.
        /// </summary>
        /// <param name="isLockedValue">The new value for IsLocked.</param>
        protected override void SetIsLocked(bool isLockedValue)
        {
            // First set the base
            base.SetIsLocked(isLockedValue);

            // Then lock all parts
            foreach (TangibleObjectInstance part in this.Parts)
                part.IsLocked = isLockedValue;
        }
        #endregion Method: SetIsLocked(bool isLockedValue)

        #region Method: Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        /// <summary>
        /// Check whether the given condition satisfies the tangible object instance.
        /// </summary>
        /// <param name="conditionBase">The condition to compare to the tangible object instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the condition satisfies the tangible object instance.</returns>
        public override bool Satisfies(ConditionBase conditionBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (conditionBase != null)
            {
                // Check whether the base satisfies the condition
                if (base.Satisfies(conditionBase, iVariableInstanceHolder))
                {
                    // Tangible object condition
                    TangibleObjectConditionBase tangibleObjectConditionBase = conditionBase as TangibleObjectConditionBase;
                    if (tangibleObjectConditionBase != null)
                    {
                        // Part condition
                        PartConditionBase partConditionBase = conditionBase as PartConditionBase;
                        if (partConditionBase != null)
                        {
                            // Check whether the offset is right
                            if (partConditionBase.Offset != null)
                            {
                                if (this.Whole == null)
                                    return false;
                                VectorValueInstance currentOffset = new VectorValueInstance(new Vec4(this.Position - this.Whole.Position));
                                if (!currentOffset.Satisfies(partConditionBase.Offset, iVariableInstanceHolder))
                                    return false;
                            }

                            // Check whether the rotation is right
                            if (partConditionBase.Rotation != null)
                            {
                                VectorValueInstance currentRotation = new VectorValueInstance(new Vec4(this.Rotation.ToEulerAngles()));
                                if (!currentRotation.Satisfies(partConditionBase.Rotation, iVariableInstanceHolder))
                                    return false;
                            }
                        }

                        // Cover condition
                        CoverConditionBase coverConditionBase = conditionBase as CoverConditionBase;
                        if (coverConditionBase != null)
                        {
                            if (coverConditionBase.IndexSign == null || coverConditionBase.Index == null || Toolbox.Compare(this.CoverIndex, (EqualitySignExtended)coverConditionBase.IndexSign, (uint)coverConditionBase.Index))
                            {
                                // Check whether the offset is right
                                if (coverConditionBase.Offset != null)
                                {
                                    if (this.CoveredObject == null)
                                        return false;
                                    VectorValueInstance currentOffset = new VectorValueInstance(new Vec4(this.Position - this.CoveredObject.Position));
                                    if (!currentOffset.Satisfies(coverConditionBase.Offset, iVariableInstanceHolder))
                                        return false;
                                }

                                // Check whether the rotation is right
                                if (coverConditionBase.Rotation != null)
                                {
                                    VectorValueInstance currentRotation = new VectorValueInstance(new Vec4(this.Rotation.ToEulerAngles()));
                                    if (!currentRotation.Satisfies(coverConditionBase.Rotation, iVariableInstanceHolder))
                                        return false;
                                }
                            }
                        }

                        // Check whether the instance has all mandatory parts
                        if (tangibleObjectConditionBase.HasAllMandatoryParts == true)
                        {
                            foreach (PartBase partBase in this.TangibleObjectBase.Parts)
                            {
                                if (partBase.Necessity == Necessity.Mandatory)
                                {
                                    int quantity = 0;
                                    foreach (TangibleObjectInstance partInstance in this.Parts)
                                    {
                                        if (partInstance.IsNodeOf(partBase.TangibleObjectBase))
                                            quantity++;
                                    }
                                    if (!partBase.Quantity.IsInRange(quantity))
                                        return false;
                                }
                            }
                        }

                        // Check whether the instance has the required parts
                        foreach (PartConditionBase partCondition in tangibleObjectConditionBase.Parts)
                        {
                            if (partCondition.Quantity == null ||
                                partCondition.Quantity.BaseValue == null ||
                                partCondition.Quantity.ValueSign == null)
                            {
                                bool satisfied = false;
                                foreach (TangibleObjectInstance partInstance in this.Parts)
                                {
                                    if (partInstance.Satisfies(partCondition, iVariableInstanceHolder))
                                    {
                                        satisfied = true;
                                        break;
                                    }
                                }
                                if (!satisfied)
                                    return false;
                            }
                            else
                            {
                                int quantity = 0;
                                int requiredQuantity = (int)partCondition.Quantity.BaseValue;
                                foreach (TangibleObjectInstance partInstance in this.Parts)
                                {
                                    if (partInstance.Satisfies(partCondition, iVariableInstanceHolder))
                                        quantity++;
                                }
                                if (!Toolbox.Compare(quantity, (EqualitySignExtended)partCondition.Quantity.ValueSign, requiredQuantity))
                                    return false;
                            }
                        }

                        // Check whether the instance has all mandatory covers
                        if (tangibleObjectConditionBase.HasAllMandatoryCovers == true)
                        {
                            foreach (CoverBase coverBase in this.TangibleObjectBase.Covers)
                            {
                                if (coverBase.Necessity == Necessity.Mandatory)
                                {
                                    int quantity = 0;
                                    foreach (TangibleObjectInstance coverInstance in this.Covers)
                                    {
                                        if (coverInstance.IsNodeOf(coverBase.TangibleObjectBase))
                                            quantity++;
                                    }
                                    if (!coverBase.Quantity.IsInRange(quantity))
                                        return false;
                                }
                            }
                        }

                        // Check whether the instance has the required covers
                        foreach (CoverConditionBase coverCondition in tangibleObjectConditionBase.Covers)
                        {
                            if (coverCondition.Quantity == null ||
                                coverCondition.Quantity.BaseValue == null ||
                                coverCondition.Quantity.ValueSign == null)
                            {
                                bool satisfied = false;
                                foreach (TangibleObjectInstance coverInstance in this.Covers)
                                {
                                    if (coverInstance.Satisfies(coverCondition, iVariableInstanceHolder))
                                    {
                                        satisfied = true;
                                        break;
                                    }
                                }
                                if (!satisfied)
                                    return false;
                            }
                            else
                            {
                                int quantity = 0;
                                int requiredQuantity = (int)coverCondition.Quantity.BaseValue;
                                foreach (TangibleObjectInstance coverInstance in this.Covers)
                                {
                                    if (coverInstance.Satisfies(coverCondition, iVariableInstanceHolder))
                                        quantity++;
                                }
                                if (!Toolbox.Compare(quantity, (EqualitySignExtended)coverCondition.Quantity.ValueSign, requiredQuantity))
                                    return false;
                            }
                        }

                        // Check whether the instance has all mandatory connections
                        if (tangibleObjectConditionBase.HasAllMandatoryConnections == true)
                        {
                            foreach (ConnectionItemBase connectionItemBase in this.TangibleObjectBase.Connections)
                            {
                                if (connectionItemBase.Necessity == Necessity.Mandatory)
                                {
                                    int quantity = 0;
                                    foreach (TangibleObjectInstance connectionItem in this.Connections)
                                    {
                                        if (connectionItem.IsNodeOf(connectionItemBase.TangibleObjectBase))
                                            quantity++;
                                    }
                                    if (!connectionItemBase.Quantity.IsInRange(quantity))
                                        return false;
                                }
                            }
                        }

                        // Check whether the instance has the required connections
                        foreach (ConnectionItemConditionBase connectionItemConditionBase in tangibleObjectConditionBase.Connections)
                        {
                            if (connectionItemConditionBase.Quantity == null ||
                                connectionItemConditionBase.Quantity.BaseValue == null ||
                                connectionItemConditionBase.Quantity.ValueSign == null)
                            {
                                bool satisfied = false;
                                foreach (TangibleObjectInstance connectionItem in this.Connections)
                                {
                                    if (connectionItem.Satisfies(connectionItemConditionBase, iVariableInstanceHolder))
                                    {
                                        satisfied = true;
                                        break;
                                    }
                                }
                                if (!satisfied)
                                    return false;
                            }
                            else
                            {
                                int quantity = 0;
                                int requiredQuantity = (int)connectionItemConditionBase.Quantity.BaseValue;
                                foreach (TangibleObjectInstance connectionItem in this.Connections)
                                {
                                    if (connectionItem.Satisfies(connectionItemConditionBase, iVariableInstanceHolder))
                                        quantity++;
                                }
                                if (!Toolbox.Compare(quantity, (EqualitySignExtended)connectionItemConditionBase.Quantity.ValueSign, requiredQuantity))
                                    return false;
                            }
                        }

                        // Check whether the instance has all mandatory layers
                        if (tangibleObjectConditionBase.HasAllMandatoryLayers == true)
                        {
                            foreach (LayerBase layerBase in this.TangibleObjectBase.Layers)
                            {
                                if (layerBase.Necessity == Necessity.Mandatory)
                                {
                                    int quantity = 0;
                                    foreach (MatterInstance layerInstance in this.Layers)
                                    {
                                        if (layerInstance.IsNodeOf(layerBase.MatterBase))
                                            quantity++;
                                    }
                                    if (!layerBase.Quantity.IsInRange(quantity))
                                        return false;
                                }
                            }
                        }

                        // Check whether the instance has the required layers
                        foreach (LayerConditionBase layerConditionBase in tangibleObjectConditionBase.Layers)
                        {
                            bool satisfied = false;
                            foreach (MatterInstance layerInstance in this.Layers)
                            {
                                if (layerInstance.Satisfies(layerConditionBase, iVariableInstanceHolder))
                                {
                                    satisfied = true;
                                    break;
                                }
                            }
                            if (!satisfied)
                                return false;
                        }

                        // Check whether the instance has all mandatory matter
                        if (tangibleObjectConditionBase.HasAllMandatoryMatter == true)
                        {
                            foreach (MatterValuedBase matterValuedBase in this.TangibleObjectBase.Matter)
                            {
                                if (matterValuedBase.Necessity == Necessity.Mandatory)
                                {
                                    NumericalValueInstance quantity = new NumericalValueInstance(0);
                                    foreach (MatterInstance matterInstance in this.Matter)
                                    {
                                        if (matterInstance.IsNodeOf(matterValuedBase.MatterBase))
                                            quantity += matterInstance.Quantity;
                                    }
                                    if (!matterValuedBase.Quantity.IsInRange(quantity))
                                        return false;
                                }
                            }
                        }

                        // Check whether the instance has the required matter
                        foreach (MatterConditionBase matterConditionBase in tangibleObjectConditionBase.Matter)
                        {
                            bool satisfied = false;
                            foreach (MatterInstance matterInstance in this.Matter)
                            {
                                if (matterInstance.Satisfies(matterConditionBase, iVariableInstanceHolder))
                                {
                                    satisfied = true;
                                    break;
                                }
                            }
                            if (!satisfied)
                                return false;
                        }
                    }

                    return true;
                }
                else
                {
                    // Matter condition
                    MatterConditionBase matterCondition = conditionBase as MatterConditionBase;
                    if (matterCondition != null)
                    {
                        foreach (MatterInstance matterInstance in this.Matter)
                        {
                            if (matterInstance.Satisfies(matterCondition, iVariableInstanceHolder))
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
        /// Apply the given change to the tangible object instance.
        /// </summary>
        /// <param name="changeBase">The change to apply to the tangible object instance.</param>
        /// <param name="iVariableInstanceHolder">The variable instance holder.</param>
        /// <returns>Returns whether the application has been done successfully.</returns>
        protected internal override bool Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)
        {
            if (changeBase != null)
            {
                if (base.Apply(changeBase, iVariableInstanceHolder))
                {
                    // Tangible object change
                    TangibleObjectChangeBase tangibleObjectChangeBase = changeBase as TangibleObjectChangeBase;
                    if (tangibleObjectChangeBase != null)
                    {
                        // Change the parts
                        foreach (PartChangeBase partChange in tangibleObjectChangeBase.Parts)
                        {
                            foreach (TangibleObjectInstance tangibleObjectInstance in this.Parts)
                                tangibleObjectInstance.Apply(partChange, iVariableInstanceHolder);
                        }

                        // Add the parts
                        foreach (PartBase partBase in tangibleObjectChangeBase.PartsToAdd)
                        {
                            for (int i = 0; i < partBase.Quantity.BaseValue; i++)
                                AddPart(InstanceManager.Current.Create<TangibleObjectInstance>(partBase));
                        }

                        // Remove the parts
                        foreach (PartConditionBase partConditionBase in tangibleObjectChangeBase.PartsToRemove)
                        {
                            if (partConditionBase.Quantity == null ||
                                partConditionBase.Quantity.BaseValue == null ||
                                partConditionBase.Quantity.ValueSign == null)
                            {
                                foreach (TangibleObjectInstance tangibleObjectInstance in this.Parts)
                                {
                                    if (tangibleObjectInstance.Satisfies(partConditionBase, iVariableInstanceHolder))
                                    {
                                        RemovePart(tangibleObjectInstance);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                int quantity = (int)partConditionBase.Quantity.BaseValue;
                                if (quantity > 0)
                                {
                                    ReadOnlyCollection<TangibleObjectInstance> parts = this.Parts;
                                    foreach (TangibleObjectInstance tangibleObjectInstance in parts)
                                    {
                                        if (tangibleObjectInstance.Satisfies(partConditionBase, iVariableInstanceHolder))
                                        {
                                            RemovePart(tangibleObjectInstance);
                                            quantity--;
                                            if (quantity <= 0)
                                                break;
                                        }
                                    }
                                }
                            }
                        }

                        // Change the covers
                        foreach (CoverChangeBase coverChange in tangibleObjectChangeBase.Covers)
                        {
                            foreach (TangibleObjectInstance tangibleObjectInstance in this.Covers)
                                tangibleObjectInstance.Apply(coverChange, iVariableInstanceHolder);
                        }

                        // Add the covers
                        foreach (CoverBase coverBase in tangibleObjectChangeBase.CoversToAdd)
                        {
                            for (int i = 0; i < coverBase.Quantity.BaseValue; i++)
                                AddCover(InstanceManager.Current.Create<TangibleObjectInstance>(coverBase));
                        }

                        // Remove the covers
                        foreach (CoverConditionBase coverConditionBase in tangibleObjectChangeBase.CoversToRemove)
                        {
                            if (coverConditionBase.Quantity == null ||
                                coverConditionBase.Quantity.BaseValue == null ||
                                coverConditionBase.Quantity.ValueSign == null)
                            {
                                foreach (TangibleObjectInstance tangibleObjectInstance in this.Covers)
                                {
                                    if (tangibleObjectInstance.Satisfies(coverConditionBase, iVariableInstanceHolder))
                                    {
                                        RemoveCover(tangibleObjectInstance);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                int quantity = (int)coverConditionBase.Quantity.BaseValue;
                                if (quantity > 0)
                                {
                                    ReadOnlyCollection<TangibleObjectInstance> covers = this.Covers;
                                    foreach (TangibleObjectInstance tangibleObjectInstance in covers)
                                    {
                                        if (tangibleObjectInstance.Satisfies(coverConditionBase, iVariableInstanceHolder))
                                        {
                                            RemoveCover(tangibleObjectInstance);
                                            quantity--;
                                            if (quantity <= 0)
                                                break;
                                        }
                                    }
                                }
                            }
                        }

                        // Change the connections
                        foreach (ConnectionItemChangeBase connectionChangeBase in tangibleObjectChangeBase.Connections)
                        {
                            foreach (TangibleObjectInstance tangibleObjectInstance in this.Connections)
                                tangibleObjectInstance.Apply(connectionChangeBase, iVariableInstanceHolder);
                        }

                        // Add the connections
                        foreach (ConnectionItemBase connectionItemBase in tangibleObjectChangeBase.ConnectionsToAdd)
                        {
                            for (int i = 0; i < connectionItemBase.Quantity.BaseValue; i++)
                                CreateConnection(this, InstanceManager.Current.Create<TangibleObjectInstance>(connectionItemBase.TangibleObjectBase));
                        }

                        // Remove the connections
                        foreach (ConnectionItemConditionBase connectionItemConditionBase in tangibleObjectChangeBase.ConnectionsToRemove)
                        {
                            if (connectionItemConditionBase.Quantity == null ||
                                connectionItemConditionBase.Quantity.BaseValue == null ||
                                connectionItemConditionBase.Quantity.ValueSign == null)
                            {
                                foreach (TangibleObjectInstance tangibleObjectInstance in this.Connections)
                                {
                                    if (tangibleObjectInstance.Satisfies(connectionItemConditionBase, iVariableInstanceHolder))
                                    {
                                        RemoveConnection(this, tangibleObjectInstance);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                int quantity = (int)connectionItemConditionBase.Quantity.BaseValue;
                                if (quantity > 0)
                                {
                                    ReadOnlyCollection<TangibleObjectInstance> connections = this.Connections;
                                    foreach (TangibleObjectInstance tangibleObjectInstance in connections)
                                    {
                                        if (tangibleObjectInstance.Satisfies(connectionItemConditionBase, iVariableInstanceHolder))
                                        {
                                            RemoveConnection(this, tangibleObjectInstance);
                                            quantity--;
                                            if (quantity <= 0)
                                                break;
                                        }
                                    }
                                }
                            }
                        }

                        // Change the matter
                        foreach (MatterChangeBase matterChangeBase in tangibleObjectChangeBase.Matter)
                        {
                            foreach (MatterInstance matterInstance in this.Matter)
                                matterInstance.Apply(matterChangeBase, iVariableInstanceHolder);
                        }

                        // Add the matter
                        foreach (MatterValuedBase matterValuedBase in tangibleObjectChangeBase.MatterToAdd)
                            AddMatter(InstanceManager.Current.Create<MatterInstance>(matterValuedBase));

                        // Remove the matter
                        foreach (MatterConditionBase matterConditionBase in tangibleObjectChangeBase.MatterToRemove)
                        {
                            foreach (MatterInstance matterInstance in this.Matter)
                            {
                                if (matterInstance.Satisfies(matterConditionBase, iVariableInstanceHolder))
                                {
                                    RemoveMatter(matterInstance);
                                    break;
                                }
                            }
                        }

                        // Change the layers
                        foreach (LayerChangeBase layerChangeBase in tangibleObjectChangeBase.Layers)
                        {
                            foreach (MatterInstance layerInstance in this.Layers)
                                layerInstance.Apply(layerChangeBase, iVariableInstanceHolder);
                        }

                        // Add the layers
                        foreach (LayerBase layerBase in tangibleObjectChangeBase.LayersToAdd)
                            AddLayer(InstanceManager.Current.Create<MatterInstance>(layerBase));

                        // Remove the layers
                        foreach (LayerConditionBase layerConditionBase in tangibleObjectChangeBase.LayersToRemove)
                        {
                            foreach (MatterInstance layerInstance in this.Layers)
                            {
                                if (layerInstance.Satisfies(layerConditionBase, iVariableInstanceHolder))
                                {
                                    RemoveLayer(layerInstance);
                                    break;
                                }
                            }
                        }

                        // Part change
                        PartChangeBase partChangeBase = changeBase as PartChangeBase;
                        if (partChangeBase != null)
                        {
                            // Adjust the offset
                            if (partChangeBase.Offset != null)
                            {
                                if (this.Whole != null)
                                {
                                    VectorValueInstance currentOffset = new VectorValueInstance(new Vec4(this.Whole.Position - this.Position));
                                    if (currentOffset.Apply(partChangeBase.Offset, iVariableInstanceHolder))
                                        this.Position = this.Whole.Position + new Vec3(currentOffset.Vector);
                                }
                            }

                            // Adjust the rotation
                            if (partChangeBase.Rotation != null)
                            {
                                VectorValueInstance currentRotation = new VectorValueInstance(new Vec4(this.Rotation.ToEulerAngles()));
                                if (currentRotation.Apply(partChangeBase.Rotation, iVariableInstanceHolder))
                                    this.Rotation = Quaternion.FromEulerAngles(currentRotation.Vector.X, currentRotation.Vector.Y, currentRotation.Vector.Z);
                            }
                        }

                        // Cover change
                        CoverChangeBase coverChangeBase = changeBase as CoverChangeBase;
                        if (coverChangeBase != null)
                        {
                            // Adjust the offset
                            if (coverChangeBase.Offset != null)
                            {
                                if (this.CoveredObject != null)
                                {
                                    VectorValueInstance currentOffset = new VectorValueInstance(new Vec4(this.CoveredObject.Position - this.Position));
                                    if (currentOffset.Apply(coverChangeBase.Offset, iVariableInstanceHolder))
                                        this.Position = this.CoveredObject.Position + new Vec3(currentOffset.Vector);
                                }
                            }

                            // Adjust the rotation
                            if (coverChangeBase.Rotation != null)
                            {
                                VectorValueInstance currentRotation = new VectorValueInstance(new Vec4(this.Rotation.ToEulerAngles()));
                                if (currentRotation.Apply(coverChangeBase.Rotation, iVariableInstanceHolder))
                                    this.Rotation = Quaternion.FromEulerAngles(currentRotation.Vector.X, currentRotation.Vector.Y, currentRotation.Vector.Z);
                            }

                            // Adjust the index
                            if (coverChangeBase.Index != null && coverChangeBase.IndexChange != null)
                                this.CoverIndex = Toolbox.CalcValue(this.CoverIndex, (ValueChangeType)coverChangeBase.IndexChange, (int)coverChangeBase.Index);
                        }
                    }
                    return true;
                }
                else
                {
                    // Matter change
                    MatterChangeBase matterChange = changeBase as MatterChangeBase;
                    if (matterChange != null)
                    {
                        foreach (MatterInstance matterInstance in this.Matter)
                            matterInstance.Apply(matterChange, iVariableInstanceHolder);
                    }
                }
            }
            return false;
        }
        #endregion Method: Apply(ChangeBase changeBase, IVariableInstanceHolder iVariableInstanceHolder)

        #endregion Method Group: Other

    }
    #endregion Class: TangibleObjectInstance

    #region Class: PartSlot
    /// <summary>
    /// A slot that can be filled with a part, which is a tangible object instance.
    /// </summary>
    public class PartSlot : PropertyChangedComponent
    {

        #region Properties and Fields

        #region Property: PartBase
        /// <summary>
        /// The tangible object that should fill the slot.
        /// </summary>
        private TangibleObjectBase partBase = null;

        /// <summary>
        /// Gets the tangible object that should fill the slot.
        /// </summary>
        internal TangibleObjectBase PartBase
        {
            get
            {
                return partBase;
            }
        }
        #endregion Property: PartBase

        #region Property: Part
        /// <summary>
        /// The part that fills the slot.
        /// </summary>
        private TangibleObjectInstance part = null;

        /// <summary>
        /// Gets the part that fills the slot.
        /// </summary>
        public TangibleObjectInstance Part
        {
            get
            {
                return part;
            }
            internal set
            {
                part = value;
                NotifyPropertyChanged("Part");
                NotifyPropertyChanged("IsEmpty");
                NotifyPropertyChanged("IsFilled");
            }
        }
        #endregion Property: Part

        #region Property: IsEmpty
        /// <summary>
        /// Gets the value that indicates whether this slot is empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this.part == null;
            }
        }
        #endregion Property: IsEmpty

        #region Property: IsFilled
        /// <summary>
        /// Gets the value that indicates whether this slot is filled.
        /// </summary>
        public bool IsFilled
        {
            get
            {
                return this.part != null;
            }
        }
        #endregion Property: IsFilled

        #endregion Properties and Fields

        #region Constructor: PartSlot(TangibleObjectBase tangibleObjectBase)
        /// <summary>
        /// Creates a new slot filled for the given tangible object base.
        /// </summary>
        /// <param name="tangibleObjectBase">The tangible object that should be filled in the slot.</param>
        public PartSlot(TangibleObjectBase tangibleObjectBase)
        {
            this.partBase = tangibleObjectBase;
        }
        #endregion Constructor: PartSlot(TangibleObjectBase tangibleObjectBase)

    }
    #endregion Class: PartSlot

    #region Class: CoverSlot
    /// <summary>
    /// A slot that can be filled with a cover, which is a tangible object instance.
    /// </summary>
    public class CoverSlot : PropertyChangedComponent
    {

        #region Properties and Fields

        #region Property: CoverBase
        /// <summary>
        /// The tangible object that should fill the slot.
        /// </summary>
        private TangibleObjectBase coverBase = null;

        /// <summary>
        /// Gets the tangible object that should fill the slot.
        /// </summary>
        internal TangibleObjectBase CoverBase
        {
            get
            {
                return coverBase;
            }
        }
        #endregion Property: CoverBase

        #region Property: Cover
        /// <summary>
        /// The cover that fills the slot.
        /// </summary>
        private TangibleObjectInstance cover = null;

        /// <summary>
        /// Gets the cover that fills the slot.
        /// </summary>
        public TangibleObjectInstance Cover
        {
            get
            {
                return cover;
            }
            internal set
            {
                cover = value;
                NotifyPropertyChanged("Cover");
                NotifyPropertyChanged("IsEmpty");
                NotifyPropertyChanged("IsFilled");
            }
        }
        #endregion Property: Cover

        #region Property: IsEmpty
        /// <summary>
        /// Gets the value that indicates whether this slot is empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this.cover == null;
            }
        }
        #endregion Property: IsEmpty

        #region Property: IsFilled
        /// <summary>
        /// Gets the value that indicates whether this slot is filled.
        /// </summary>
        public bool IsFilled
        {
            get
            {
                return this.cover != null;
            }
        }
        #endregion Property: IsFilled

        #endregion Properties and Fields

        #region Constructor: CoverSlot(TangibleObjectBase tangibleObjectBase)
        /// <summary>
        /// Creates a new slot filled for the given tangible object base.
        /// </summary>
        /// <param name="tangibleObjectBase">The tangible object that should be filled in the slot.</param>
        public CoverSlot(TangibleObjectBase tangibleObjectBase)
        {
            this.coverBase = tangibleObjectBase;
        }
        #endregion Constructor: CoverSlot(TangibleObjectBase tangibleObjectBase)

    }
    #endregion Class: CoverSlot

    #region Class: ConnectionSlot
    /// <summary>
    /// A slot that can be filled with a connection item, which is a tangible object instance.
    /// </summary>
    public class ConnectionSlot : PropertyChangedComponent
    {

        #region Properties and Fields

        #region Property: ConnectionItemBase
        /// <summary>
        /// The tangible object that should fill the slot.
        /// </summary>
        private TangibleObjectBase connectionItemBase = null;

        /// <summary>
        /// Gets the tangible object that should fill the slot.
        /// </summary>
        internal TangibleObjectBase ConnectionItemBase
        {
            get
            {
                return connectionItemBase;
            }
        }
        #endregion Property: ConnectionItemBase

        #region Property: ConnectionItem
        /// <summary>
        /// The connection item that fills the slot.
        /// </summary>
        private TangibleObjectInstance connectionItem = null;

        /// <summary>
        /// Gets the connection item that fills the slot.
        /// </summary>
        public TangibleObjectInstance ConnectionItem
        {
            get
            {
                return connectionItem;
            }
            internal set
            {
                connectionItem = value;
                NotifyPropertyChanged("ConnectionItem");
                NotifyPropertyChanged("IsEmpty");
                NotifyPropertyChanged("IsFilled");
            }
        }
        #endregion Property: ConnectionItem

        #region Property: IsEmpty
        /// <summary>
        /// Gets the value that indicates whether this slot is empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this.connectionItem == null;
            }
        }
        #endregion Property: IsEmpty

        #region Property: IsFilled
        /// <summary>
        /// Gets the value that indicates whether this slot is filled.
        /// </summary>
        public bool IsFilled
        {
            get
            {
                return this.connectionItem != null;
            }
        }
        #endregion Property: IsFilled

        #endregion Properties and Fields

        #region Constructor: ConnectionSlot(TangibleObjectBase tangibleObjectBase)
        /// <summary>
        /// Creates a new slot filled with the given connection item.
        /// </summary>
        /// <param name="tangibleObjectBase">The tangible object that should be filled in the slot.</param>
        public ConnectionSlot(TangibleObjectBase tangibleObjectBase)
        {
            this.connectionItemBase = tangibleObjectBase;
        }
        #endregion Constructor: ConnectionSlot(TangibleObjectBase tangibleObjectBase)

    }
    #endregion Class: ConnectionSlot

}