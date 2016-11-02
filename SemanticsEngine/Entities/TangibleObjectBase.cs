/**************************************************************************
 * 
 * TangibleObjectBase.cs
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
using SemanticsEngine.Tools;

namespace SemanticsEngine.Entities
{

    #region Class: TangibleObjectBase
    /// <summary>
    /// A base of a tangible object.
    /// </summary>
    public class TangibleObjectBase : PhysicalObjectBase
    {

        #region Properties and Fields

        #region Property: TangibleObject
        /// <summary>
        /// Gets the tangible object of which this is a tangible object base.
        /// </summary>
        protected internal TangibleObject TangibleObject
        {
            get
            {
                return this.IdHolder as TangibleObject;
            }
        }
        #endregion Property: TangibleObject

        #region Property: Parts
        /// <summary>
        /// The parts of this tangible object.
        /// </summary>
        private PartBase[] parts = null;

        /// <summary>
        /// Gets the parts of this tangible object.
        /// </summary>
        public ReadOnlyCollection<PartBase> Parts
        {
            get
            {
                if (parts == null)
                {
                    LoadParts();
                    if (parts == null)
                        return new List<PartBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<PartBase>(parts);
            }
        }

        /// <summary>
        /// Loads the parts.
        /// </summary>
        private void LoadParts()
        {
            if (this.TangibleObject != null)
            {
                List<PartBase> tangibleObjectParts = new List<PartBase>();
                foreach (Part part in this.TangibleObject.Parts)
                    tangibleObjectParts.Add(BaseManager.Current.GetBase<PartBase>(part));
                parts = tangibleObjectParts.ToArray();
            }
        }
        #endregion Property: Parts

        #region Property: Connections
        /// <summary>
        /// The tangible objects to which this tangible object is connected.
        /// </summary>
        private ConnectionItemBase[] connections = null;

        /// <summary>
        /// Gets the tangible objects to which this tangible object is connected.
        /// </summary>
        public ReadOnlyCollection<ConnectionItemBase> Connections
        {
            get
            {
                if (connections == null)
                {
                    LoadConnections();
                    if (connections == null)
                        return new List<ConnectionItemBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<ConnectionItemBase>(connections);
            }
        }

        /// <summary>
        /// Loads the connections.
        /// </summary>
        private void LoadConnections()
        {
            if (this.TangibleObject != null)
            {
                List<ConnectionItemBase> tangibleObjectConnections = new List<ConnectionItemBase>();
                foreach (ConnectionItem connectionItem in this.TangibleObject.Connections)
                    tangibleObjectConnections.Add(BaseManager.Current.GetBase<ConnectionItemBase>(connectionItem));
                connections = tangibleObjectConnections.ToArray();
            }
        }
        #endregion Property: Connections

        #region Property: Matter
        /// <summary>
        /// The matter of which this tangible object exists.
        /// </summary>
        private MatterValuedBase[] matter = null;
        
        /// <summary>
        /// Gets the matter of which this tangible object exists.
        /// </summary>
        public ReadOnlyCollection<MatterValuedBase> Matter
        {
            get
            {
                if (matter == null)
                {
                    LoadMatter();
                    if (matter == null)
                        return new List<MatterValuedBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<MatterValuedBase>(matter);
            }
        }

        /// <summary>
        /// Loads the matter.
        /// </summary>
        private void LoadMatter()
        {
            if (this.TangibleObject != null)
            {
                List<MatterValuedBase> matterValuedBases = new List<MatterValuedBase>();
                foreach (MatterValued matterValued in this.TangibleObject.Matter)
                    matterValuedBases.Add(BaseManager.Current.GetBase<MatterValuedBase>(matterValued));
                matter = matterValuedBases.ToArray();
            }
        }
        #endregion Property: Matter

        #region Property: Layers
        /// <summary>
        /// The layers of the tangible object.
        /// </summary>
        private LayerBase[] layers = null;

        /// <summary>
        /// Gets the layers of the tangible object.
        /// </summary>
        public ReadOnlyCollection<LayerBase> Layers
        {
            get
            {
                if (layers == null)
                {
                    LoadLayers();
                    if (layers == null)
                        return new List<LayerBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<LayerBase>(layers);
            }
        }

        /// <summary>
        /// Loads the layers.
        /// </summary>
        private void LoadLayers()
        {
            if (this.TangibleObject != null)
            {
                List<LayerBase> tangibleObjectLayers = new List<LayerBase>();
                foreach (Layer layer in this.TangibleObject.Layers)
                    tangibleObjectLayers.Add(BaseManager.Current.GetBase<LayerBase>(layer));
                layers = tangibleObjectLayers.ToArray();
            }
        }
        #endregion Property: Layers

        #region Property: Covers
        /// <summary>
        /// The covers of this tangible object.
        /// </summary>
        private CoverBase[] covers = null;

        /// <summary>
        /// Gets the covers of this tangible object.
        /// </summary>
        public ReadOnlyCollection<CoverBase> Covers
        {
            get
            {
                if (covers == null)
                {
                    LoadCovers();
                    if (covers == null)
                        return new List<CoverBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<CoverBase>(covers);
            }
        }

        /// <summary>
        /// Loads the covers.
        /// </summary>
        private void LoadCovers()
        {
            if (this.TangibleObject != null)
            {
                List<CoverBase> tangibleObjectCovers = new List<CoverBase>();
                foreach (Cover cover in this.TangibleObject.Covers)
                    tangibleObjectCovers.Add(BaseManager.Current.GetBase<CoverBase>(cover));
                covers = tangibleObjectCovers.ToArray();
            }
        }
        #endregion Property: Covers

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: TangibleObjectBase(TangibleObject tangibleObject)
        /// <summary>
        /// Creates a new tangible object base from the given tangible object.
        /// </summary>
        /// <param name="tangibleObject">The tangible object to create the tangible object base from.</param>
        protected internal TangibleObjectBase(TangibleObject tangibleObject)
            : base(tangibleObject)
        {
            if (tangibleObject != null)
            {
                if (BaseManager.PreloadProperties)
                {
                    LoadConnections();
                    LoadLayers();
                    LoadMatter();
                    LoadParts();
                    LoadCovers();
                }
            }
        }
        #endregion Constructor: TangibleObjectBase(TangibleObject tangibleObject)

        #region Constructor: TangibleObjectBase(TangibleObjectValued tangibleObjectValued)
        /// <summary>
        /// Creates a new tangible object base from the given valued tangible object.
        /// </summary>
        /// <param name="tangibleObjectValued">The valued tangible object to create the tangible object base from.</param>
        protected internal TangibleObjectBase(TangibleObjectValued tangibleObjectValued)
            : base(tangibleObjectValued)
        {
            if (tangibleObjectValued != null)
            {
                if (BaseManager.PreloadProperties)
                {
                    LoadConnections();
                    LoadLayers();
                    LoadMatter();
                    LoadParts();
                    LoadCovers();
                }
            }
        }
        #endregion Constructor: TangibleObjectBase(TangibleObjectValued tangibleObjectValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: TangibleObjectBase

    #region Class: TangibleObjectValuedBase
    /// <summary>
    /// A base of a valued tangible object.
    /// </summary>
    public class TangibleObjectValuedBase : PhysicalObjectValuedBase
    {

        #region Properties and Fields

        #region Property: TangibleObjectValued
        /// <summary>
        /// Gets the valued tangible object of which this is a tangible object base.
        /// </summary>
        protected internal TangibleObjectValued TangibleObjectValued
        {
            get
            {
                return this.NodeValued as TangibleObjectValued;
            }
        }
        #endregion Property: TangibleObjectValued

        #region Property: TangibleObjectBase
        /// <summary>
        /// Gets the tangible object of which this is a valued tangible object base.
        /// </summary>
        public TangibleObjectBase TangibleObjectBase
        {
            get
            {
                return this.NodeBase as TangibleObjectBase;
            }
        }
        #endregion Property: TangibleObjectBase

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: TangibleObjectValuedBase(TangibleObjectValued tangibleObjectValued)
        /// <summary>
        /// Creates a new valued tangible object base from the given valued tangible object.
        /// </summary>
        /// <param name="tangibleObjectValued">The valued tangible object to create a valued tangible object base from.</param>
        protected internal TangibleObjectValuedBase(TangibleObjectValued tangibleObjectValued)
            : base(tangibleObjectValued)
        {
        }
        #endregion Constructor: TangibleObjectValuedBase(TangibleObjectValued tangibleObjectValued)

        #endregion Method Group: Constructors

    }
    #endregion Class: TangibleObjectValuedBase

    #region Class: TangibleObjectConditionBase
    /// <summary>
    /// A condition on a tangible object.
    /// </summary>
    public class TangibleObjectConditionBase : PhysicalObjectConditionBase
    {

        #region Properties and Fields

        #region Property: TangibleObjectCondition
        /// <summary>
        /// Gets the tangible object condition of which this is a tangible object condition base.
        /// </summary>
        protected internal TangibleObjectCondition TangibleObjectCondition
        {
            get
            {
                return this.Condition as TangibleObjectCondition;
            }
        }
        #endregion Property: TangibleObjectCondition

        #region Property: TangibleObjectBase
        /// <summary>
        /// Gets the tangible object base of which this is a tangible object condition base.
        /// </summary>
        public TangibleObjectBase TangibleObjectBase
        {
            get
            {
                return this.NodeBase as TangibleObjectBase;
            }
        }
        #endregion Property: TangibleObjectBase

        #region Property: HasAllMandatoryParts
        /// <summary>
        /// The value that indicates whether all mandatory parts are required.
        /// </summary>
        private bool? hasAllMandatoryParts = null;
        
        /// <summary>
        /// Gets the value that indicates whether all mandatory parts are required.
        /// </summary>
        public bool? HasAllMandatoryParts
        {
            get
            {
                return hasAllMandatoryParts;
            }
        }
        #endregion Property: HasAllMandatoryParts

        #region Property: Parts
        /// <summary>
        /// The required parts.
        /// </summary>
        private PartConditionBase[] parts = null;

        /// <summary>
        /// Gets the required parts.
        /// </summary>
        public ReadOnlyCollection<PartConditionBase> Parts
        {
            get
            {
                if (parts == null)
                {
                    LoadParts();
                    if (parts == null)
                        return new List<PartConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<PartConditionBase>(parts);
            }
        }

        /// <summary>
        /// Loads the parts.
        /// </summary>
        private void LoadParts()
        {
            if (this.TangibleObjectCondition != null)
            {
                List<PartConditionBase> partConditionBases = new List<PartConditionBase>();
                foreach (PartCondition partCondition in this.TangibleObjectCondition.Parts)
                    partConditionBases.Add(BaseManager.Current.GetBase<PartConditionBase>(partCondition));
                parts = partConditionBases.ToArray();
            }
        }
        #endregion Property: Parts

        #region Property: HasAllMandatoryConnections
        /// <summary>
        /// The value that indicates whether all mandatory connections are required.
        /// </summary>
        private bool? hasAllMandatoryConnections = null;
        
        /// <summary>
        /// Gets the value that indicates whether all mandatory connections are required.
        /// </summary>
        public bool? HasAllMandatoryConnections
        {
            get
            {
                return hasAllMandatoryConnections;
            }
        }
        #endregion Property: HasAllMandatoryConnections

        #region Property: Connections
        /// <summary>
        /// The required connections.
        /// </summary>
        private ConnectionItemConditionBase[] connections = null;

        /// <summary>
        /// Gets the required connections.
        /// </summary>
        public ReadOnlyCollection<ConnectionItemConditionBase> Connections
        {
            get
            {
                if (connections == null)
                {
                    LoadConnections();
                    if (connections == null)
                        return new List<ConnectionItemConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<ConnectionItemConditionBase>(connections);
            }
        }

        /// <summary>
        /// Loads the connections.
        /// </summary>
        private void LoadConnections()
        {
            if (this.TangibleObjectCondition != null)
            {
                List<ConnectionItemConditionBase> connectionItemConditionBases = new List<ConnectionItemConditionBase>();
                foreach (ConnectionItemCondition connectionItemCondition in this.TangibleObjectCondition.Connections)
                    connectionItemConditionBases.Add(BaseManager.Current.GetBase<ConnectionItemConditionBase>(connectionItemCondition));
                connections = connectionItemConditionBases.ToArray();
            }
        }
        #endregion Property: Connections

        #region Property: HasAllMandatoryMatter
        /// <summary>
        /// The value that indicates whether all mandatory matter is required.
        /// </summary>
        private bool? hasAllMandatoryMatter = null;

        /// <summary>
        /// Gets the value that indicates whether all mandatory matter is required.
        /// </summary>
        public bool? HasAllMandatoryMatter
        {
            get
            {
                return hasAllMandatoryMatter;
            }
        }
        #endregion Property: HasAllMandatoryMatter

        #region Property: Matter
        /// <summary>
        /// The required matter.
        /// </summary>
        private MatterConditionBase[] matter = null;

        /// <summary>
        /// Gets the required matter.
        /// </summary>
        public ReadOnlyCollection<MatterConditionBase> Matter
        {
            get
            {
                if (matter == null)
                {
                    LoadMatter();
                    if (matter == null)
                        return new List<MatterConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<MatterConditionBase>(matter);
            }
        }

        /// <summary>
        /// Loads the matter.
        /// </summary>
        private void LoadMatter()
        {
            if (this.TangibleObjectCondition != null)
            {
                List<MatterConditionBase> matterConditionBases = new List<MatterConditionBase>();
                foreach (MatterCondition matterCondition in this.TangibleObjectCondition.Matter)
                    matterConditionBases.Add(BaseManager.Current.GetBase<MatterConditionBase>(matterCondition));
                matter = matterConditionBases.ToArray();
            }
        }
        #endregion Property: Matter

        #region Property: HasAllMandatoryLayers
        /// <summary>
        /// The value that indicates whether all mandatory layers are required.
        /// </summary>
        private bool? hasAllMandatoryLayers = null;

        /// <summary>
        /// Gets the value that indicates whether all mandatory layers are required.
        /// </summary>
        public bool? HasAllMandatoryLayers
        {
            get
            {
                return hasAllMandatoryLayers;
            }
        }
        #endregion Property: HasAllMandatoryLayers

        #region Property: Layers
        /// <summary>
        /// The required layers.
        /// </summary>
        private LayerConditionBase[] layers = null;

        /// <summary>
        /// Gets the required layers.
        /// </summary>
        public ReadOnlyCollection<LayerConditionBase> Layers
        {
            get
            {
                if (layers == null)
                {
                    LoadLayers();
                    if (layers == null)
                        return new List<LayerConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<LayerConditionBase>(layers);
            }
        }

        /// <summary>
        /// Loads the layers.
        /// </summary>
        private void LoadLayers()
        {
            if (this.TangibleObjectCondition != null)
            {
                List<LayerConditionBase> layerConditionBases = new List<LayerConditionBase>();
                foreach (LayerCondition layerCondition in this.TangibleObjectCondition.Layers)
                    layerConditionBases.Add(BaseManager.Current.GetBase<LayerConditionBase>(layerCondition));
                layers = layerConditionBases.ToArray();
            }
        }
        #endregion Property: Layers

        #region Property: HasAllMandatoryCovers
        /// <summary>
        /// The value that indicates whether all mandatory covers are required.
        /// </summary>
        private bool? hasAllMandatoryCovers = null;

        /// <summary>
        /// Gets the value that indicates whether all mandatory covers are required.
        /// </summary>
        public bool? HasAllMandatoryCovers
        {
            get
            {
                return hasAllMandatoryCovers;
            }
        }
        #endregion Property: HasAllMandatoryCovers

        #region Property: Covers
        /// <summary>
        /// The required covers.
        /// </summary>
        private CoverConditionBase[] covers = null;

        /// <summary>
        /// Gets the required covers.
        /// </summary>
        public ReadOnlyCollection<CoverConditionBase> Covers
        {
            get
            {
                if (covers == null)
                {
                    LoadCovers();
                    if (covers == null)
                        return new List<CoverConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<CoverConditionBase>(covers);
            }
        }

        /// <summary>
        /// Loads the covers.
        /// </summary>
        private void LoadCovers()
        {
            if (this.TangibleObjectCondition != null)
            {
                List<CoverConditionBase> coverConditionBases = new List<CoverConditionBase>();
                foreach (CoverCondition coverCondition in this.TangibleObjectCondition.Covers)
                    coverConditionBases.Add(BaseManager.Current.GetBase<CoverConditionBase>(coverCondition));
                covers = coverConditionBases.ToArray();
            }
        }
        #endregion Property: Covers

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: TangibleObjectConditionBase(TangibleObjectCondition tangibleObjectCondition)
        /// <summary>
        /// Creates a base of the given tangible object condition.
        /// </summary>
        /// <param name="tangibleObjectCondition">The tangible object condition to create a base of.</param>
        protected internal TangibleObjectConditionBase(TangibleObjectCondition tangibleObjectCondition)
            : base(tangibleObjectCondition)
        {
            if (tangibleObjectCondition != null)
            {
                this.hasAllMandatoryParts = tangibleObjectCondition.HasAllMandatoryParts;
                this.hasAllMandatoryConnections = tangibleObjectCondition.HasAllMandatoryConnections;
                this.hasAllMandatoryMatter = tangibleObjectCondition.HasAllMandatoryMatter;
                this.hasAllMandatoryLayers = tangibleObjectCondition.HasAllMandatoryLayers;
                this.hasAllMandatoryCovers = tangibleObjectCondition.HasAllMandatoryCovers;

                if (BaseManager.PreloadProperties)
                {
                    LoadParts();
                    LoadConnections();
                    LoadMatter();
                    LoadLayers();
                    LoadCovers();
                }
            }
        }
        #endregion Constructor: TangibleObjectConditionBase(TangibleObjectCondition tangibleObjectCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: TangibleObjectConditionBase

    #region Class: TangibleObjectChangeBase
    /// <summary>
    /// A change on a tangible object.
    /// </summary>
    public class TangibleObjectChangeBase : PhysicalObjectChangeBase
    {

        #region Properties and Fields

        #region Property: TangibleObjectChange
        /// <summary>
        /// Gets the tangible object change of which this is a tangible object change base.
        /// </summary>
        protected internal TangibleObjectChange TangibleObjectChange
        {
            get
            {
                return this.Change as TangibleObjectChange;
            }
        }
        #endregion Property: TangibleObjectChange

        #region Property: TangibleObjectBase
        /// <summary>
        /// Gets the affected tangible object base.
        /// </summary>
        public TangibleObjectBase TangibleObjectBase
        {
            get
            {
                return this.NodeBase as TangibleObjectBase;
            }
        }
        #endregion Property: TangibleObjectBase

        #region Property: Parts
        /// <summary>
        /// The parts to change.
        /// </summary>
        private PartChangeBase[] parts = null;

        /// <summary>
        /// Gets the parts to change.
        /// </summary>
        public ReadOnlyCollection<PartChangeBase> Parts
        {
            get
            {
                if (parts == null)
                {
                    LoadParts();
                    if (parts == null)
                        return new List<PartChangeBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<PartChangeBase>(parts);
            }
        }

        /// <summary>
        /// Loads the parts.
        /// </summary>
        private void LoadParts()
        {
            if (this.TangibleObjectChange != null)
            {
                List<PartChangeBase> partChangeBases = new List<PartChangeBase>();
                foreach (PartChange partChange in this.TangibleObjectChange.Parts)
                    partChangeBases.Add(BaseManager.Current.GetBase<PartChangeBase>(partChange));
                parts = partChangeBases.ToArray();
            }
        }
        #endregion Property: Parts

        #region Property: PartsToAdd
        /// <summary>
        /// The parts that should be added during the change.
        /// </summary>
        private PartBase[] partsToAdd = null;

        /// <summary>
        /// Gets the parts that should be added during the change.
        /// </summary>
        public ReadOnlyCollection<PartBase> PartsToAdd
        {
            get
            {
                if (partsToAdd == null)
                {
                    LoadPartsToAdd();
                    if (partsToAdd == null)
                        return new List<PartBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<PartBase>(partsToAdd);
            }
        }

        /// <summary>
        /// Loads the parts to add.
        /// </summary>
        private void LoadPartsToAdd()
        {
            if (this.TangibleObjectChange != null)
            {
                List<PartBase> partBases = new List<PartBase>();
                foreach (Part part in this.TangibleObjectChange.PartsToAdd)
                {
                    PartBase partBase = BaseManager.Current.GetBase<PartBase>(part);
                    for (int i = 0; i < part.Quantity.Value; i++)
                        partBases.Add(partBase);
                }
                partsToAdd = partBases.ToArray();
            }
        }
        #endregion Property: PartsToAdd

        #region Property: PartsToRemove
        /// <summary>
        /// The parts that should be removed during the change.
        /// </summary>
        private PartConditionBase[] partsToRemove = null;

        /// <summary>
        /// Gets the parts that should be removed during the change.
        /// </summary>
        public ReadOnlyCollection<PartConditionBase> PartsToRemove
        {
            get
            {
                if (partsToRemove == null)
                {
                    LoadPartsToRemove();
                    if (partsToRemove == null)
                        return new List<PartConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<PartConditionBase>(partsToRemove);
            }
        }

        /// <summary>
        /// Loads the parts to remove.
        /// </summary>
        private void LoadPartsToRemove()
        {
            if (this.TangibleObjectChange != null)
            {
                List<PartConditionBase> partConditionBases = new List<PartConditionBase>();
                foreach (PartCondition partCondition in this.TangibleObjectChange.PartsToRemove)
                    partConditionBases.Add(BaseManager.Current.GetBase<PartConditionBase>(partCondition));
                partsToRemove = partConditionBases.ToArray();
            }
        }
        #endregion Property: PartsToRemove

        #region Property: Connections
        /// <summary>
        /// The connections to change.
        /// </summary>
        private ConnectionItemChangeBase[] connections = null;

        /// <summary>
        /// Gets the connections to change.
        /// </summary>
        public ReadOnlyCollection<ConnectionItemChangeBase> Connections
        {
            get
            {
                if (connections == null)
                {
                    LoadConnections();
                    if (connections == null)
                        return new List<ConnectionItemChangeBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<ConnectionItemChangeBase>(connections);
            }
        }

        /// <summary>
        /// Loads the connections.
        /// </summary>
        private void LoadConnections()
        {
            if (this.TangibleObjectChange != null)
            {
                List<ConnectionItemChangeBase> connectionItemChangeBases = new List<ConnectionItemChangeBase>();
                foreach (ConnectionItemChange connectionItemChange in this.TangibleObjectChange.Connections)
                    connectionItemChangeBases.Add(BaseManager.Current.GetBase<ConnectionItemChangeBase>(connectionItemChange));
                connections = connectionItemChangeBases.ToArray();
            }
        }
        #endregion Property: Connections

        #region Property: ConnectionsToAdd
        /// <summary>
        /// The connections to add.
        /// </summary>
        private ConnectionItemBase[] connectionsToAdd = null;

        /// <summary>
        /// Gets the connections to add.
        /// </summary>
        public ReadOnlyCollection<ConnectionItemBase> ConnectionsToAdd
        {
            get
            {
                if (connectionsToAdd == null)
                {
                    LoadConnectionsToAdd();
                    if (connectionsToAdd == null)
                        return new List<ConnectionItemBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<ConnectionItemBase>(connectionsToAdd);
            }
        }

        /// <summary>
        /// Loads the connections to add.
        /// </summary>
        private void LoadConnectionsToAdd()
        {
            if (this.TangibleObjectChange != null)
            {
                List<ConnectionItemBase> connectionItemBases = new List<ConnectionItemBase>();
                foreach (ConnectionItem connectionItem in this.TangibleObjectChange.ConnectionsToAdd)
                    connectionItemBases.Add(BaseManager.Current.GetBase<ConnectionItemBase>(connectionItem));
                connectionsToAdd = connectionItemBases.ToArray();
            }
        }
        #endregion Property: ConnectionsToAdd

        #region Property: ConnectionsToRemove
        /// <summary>
        /// The connections to remove.
        /// </summary>
        private ConnectionItemConditionBase[] connectionsToRemove = null;

        /// <summary>
        /// Gets the connections to remove.
        /// </summary>
        public ReadOnlyCollection<ConnectionItemConditionBase> ConnectionsToRemove
        {
            get
            {
                if (connectionsToRemove == null)
                {
                    LoadConnectionsToRemove();
                    if (connectionsToRemove == null)
                        return new List<ConnectionItemConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<ConnectionItemConditionBase>(connectionsToRemove);
            }
        }

        /// <summary>
        /// Loads the connections to remove.
        /// </summary>
        private void LoadConnectionsToRemove()
        {
            if (this.TangibleObjectChange != null)
            {
                List<ConnectionItemConditionBase> connectionItemConditionBases = new List<ConnectionItemConditionBase>();
                foreach (ConnectionItemCondition connectionItemCondition in this.TangibleObjectChange.ConnectionsToRemove)
                    connectionItemConditionBases.Add(BaseManager.Current.GetBase<ConnectionItemConditionBase>(connectionItemCondition));
                connectionsToRemove = connectionItemConditionBases.ToArray();
            }
        }
        #endregion Property: ConnectionsToRemove

        #region Property: Matter
        /// <summary>
        /// The matter to change.
        /// </summary>
        private MatterChangeBase[] matter = null;

        /// <summary>
        /// Gets the matter to change.
        /// </summary>
        public ReadOnlyCollection<MatterChangeBase> Matter
        {
            get
            {
                if (matter == null)
                {
                    LoadMatter();
                    if (matter == null)
                        return new List<MatterChangeBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<MatterChangeBase>(matter);
            }
        }

        /// <summary>
        /// Loads the matter.
        /// </summary>
        private void LoadMatter()
        {
            if (this.TangibleObjectChange != null)
            {
                List<MatterChangeBase> matterChangeBases = new List<MatterChangeBase>();
                foreach (MatterChange matterChange in this.TangibleObjectChange.Matter)
                    matterChangeBases.Add(BaseManager.Current.GetBase<MatterChangeBase>(matterChange));
                matter = matterChangeBases.ToArray();
            }
        }
        #endregion Property: Matter

        #region Property: MatterToAdd
        /// <summary>
        /// The matter to add.
        /// </summary>
        private MatterValuedBase[] matterToAdd = null;

        /// <summary>
        /// Gets the matter to add.
        /// </summary>
        public ReadOnlyCollection<MatterValuedBase> MatterToAdd
        {
            get
            {
                if (matterToAdd == null)
                {
                    LoadMatterToAdd();
                    if (matterToAdd == null)
                        return new List<MatterValuedBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<MatterValuedBase>(matterToAdd);
            }
        }

        /// <summary>
        /// Loads the matter to add.
        /// </summary>
        private void LoadMatterToAdd()
        {
            if (this.TangibleObjectChange != null)
            {
                List<MatterValuedBase> matterValuedBases = new List<MatterValuedBase>();
                foreach (MatterValued matterValued in this.TangibleObjectChange.MatterToAdd)
                    matterValuedBases.Add(BaseManager.Current.GetBase<MatterValuedBase>(matterValued));
                matterToAdd = matterValuedBases.ToArray();
            }
        }
        #endregion Property: MatterToAdd

        #region Property: MatterToRemove
        /// <summary>
        /// The matter to remove.
        /// </summary>
        private MatterConditionBase[] matterToRemove = null;

        /// <summary>
        /// Gets the matter to remove.
        /// </summary>
        public ReadOnlyCollection<MatterConditionBase> MatterToRemove
        {
            get
            {
                if (matterToRemove == null)
                {
                    LoadMatterToRemove();
                    if (matterToRemove == null)
                        return new List<MatterConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<MatterConditionBase>(matterToRemove);
            }
        }

        /// <summary>
        /// Loads the matter to remove.
        /// </summary>
        private void LoadMatterToRemove()
        {
            if (this.TangibleObjectChange != null)
            {
                List<MatterConditionBase> matterConditionBases = new List<MatterConditionBase>();
                foreach (MatterCondition matterCondition in this.TangibleObjectChange.MatterToRemove)
                    matterConditionBases.Add(BaseManager.Current.GetBase<MatterConditionBase>(matterCondition));
                matterToRemove = matterConditionBases.ToArray();
            }
        }
        #endregion Property: MatterToRemove

        #region Property: Layers
        /// <summary>
        /// The layers to change.
        /// </summary>
        private LayerChangeBase[] layers = null;

        /// <summary>
        /// Gets the layers to change.
        /// </summary>
        public ReadOnlyCollection<LayerChangeBase> Layers
        {
            get
            {
                if (layers == null)
                {
                    LoadLayers();
                    if (layers == null)
                        return new List<LayerChangeBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<LayerChangeBase>(layers);
            }
        }

        /// <summary>
        /// Loads the layers.
        /// </summary>
        private void LoadLayers()
        {
            if (this.TangibleObjectChange != null)
            {
                List<LayerChangeBase> layerChangeBases = new List<LayerChangeBase>();
                foreach (LayerChange layerChange in this.TangibleObjectChange.Layers)
                    layerChangeBases.Add(BaseManager.Current.GetBase<LayerChangeBase>(layerChange));
                layers = layerChangeBases.ToArray();
            }
        }
        #endregion Property: Layers

        #region Property: LayersToAdd
        /// <summary>
        /// The layers to add.
        /// </summary>
        private LayerBase[] layersToAdd = null;

        /// <summary>
        /// Gets the layers to add.
        /// </summary>
        public ReadOnlyCollection<LayerBase> LayersToAdd
        {
            get
            {
                if (layersToAdd == null)
                {
                    LoadLayersToAdd();
                    if (layersToAdd == null)
                        return new List<LayerBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<LayerBase>(layersToAdd);
            }
        }

        /// <summary>
        /// Loads the layers to add.
        /// </summary>
        private void LoadLayersToAdd()
        {
            if (this.TangibleObjectChange != null)
            {
                List<LayerBase> layerBases = new List<LayerBase>();
                foreach (Layer layer in this.TangibleObjectChange.LayersToAdd)
                {
                    LayerBase layerBase = BaseManager.Current.GetBase<LayerBase>(layer);
                    for (int i = 0; i < layer.Quantity.Value; i++)
                        layerBases.Add(layerBase);
                }
                layersToAdd = layerBases.ToArray();
            }
        }
        #endregion Property: LayersToAdd

        #region Property: LayersToRemove
        /// <summary>
        /// The layers to remove.
        /// </summary>
        private LayerConditionBase[] layersToRemove = null;

        /// <summary>
        /// Gets the layers to remove.
        /// </summary>
        public ReadOnlyCollection<LayerConditionBase> LayersToRemove
        {
            get
            {
                if (layersToRemove == null)
                {
                    LoadLayersToRemove();
                    if (layersToRemove == null)
                        return new List<LayerConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<LayerConditionBase>(layersToRemove);
            }
        }

        /// <summary>
        /// Loads the layers to remove.
        /// </summary>
        private void LoadLayersToRemove()
        {
            if (this.TangibleObjectChange != null)
            {
                List<LayerConditionBase> layerConditionBases = new List<LayerConditionBase>();
                foreach (LayerCondition layerCondition in this.TangibleObjectChange.LayersToRemove)
                    layerConditionBases.Add(BaseManager.Current.GetBase<LayerConditionBase>(layerCondition));
                layersToRemove = layerConditionBases.ToArray();
            }
        }
        #endregion Property: LayersToRemove

        #region Property: Covers
        /// <summary>
        /// The covers to change.
        /// </summary>
        private CoverChangeBase[] covers = null;

        /// <summary>
        /// Gets the covers to change.
        /// </summary>
        public ReadOnlyCollection<CoverChangeBase> Covers
        {
            get
            {
                if (covers == null)
                {
                    LoadCovers();
                    if (covers == null)
                        return new List<CoverChangeBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<CoverChangeBase>(covers);
            }
        }

        /// <summary>
        /// Loads the covers.
        /// </summary>
        private void LoadCovers()
        {
            if (this.TangibleObjectChange != null)
            {
                List<CoverChangeBase> coverChangeBases = new List<CoverChangeBase>();
                foreach (CoverChange coverChange in this.TangibleObjectChange.Covers)
                    coverChangeBases.Add(BaseManager.Current.GetBase<CoverChangeBase>(coverChange));
                covers = coverChangeBases.ToArray();
            }
        }
        #endregion Property: Covers

        #region Property: CoversToAdd
        /// <summary>
        /// The covers that should be added during the change.
        /// </summary>
        private CoverBase[] coversToAdd = null;

        /// <summary>
        /// Gets the covers that should be added during the change.
        /// </summary>
        public ReadOnlyCollection<CoverBase> CoversToAdd
        {
            get
            {
                if (coversToAdd == null)
                {
                    LoadCoversToAdd();
                    if (coversToAdd == null)
                        return new List<CoverBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<CoverBase>(coversToAdd);
            }
        }

        /// <summary>
        /// Loads the covers to add.
        /// </summary>
        private void LoadCoversToAdd()
        {
            if (this.TangibleObjectChange != null)
            {
                List<CoverBase> coverBases = new List<CoverBase>();
                foreach (Cover cover in this.TangibleObjectChange.CoversToAdd)
                {
                    CoverBase coverBase = BaseManager.Current.GetBase<CoverBase>(cover);
                    for (int i = 0; i < cover.Quantity.Value; i++)
                        coverBases.Add(coverBase);
                }
                coversToAdd = coverBases.ToArray();
            }
        }
        #endregion Property: CoversToAdd

        #region Property: CoversToRemove
        /// <summary>
        /// The covers that should be removed during the change.
        /// </summary>
        private CoverConditionBase[] coversToRemove = null;

        /// <summary>
        /// Gets the covers that should be removed during the change.
        /// </summary>
        public ReadOnlyCollection<CoverConditionBase> CoversToRemove
        {
            get
            {
                if (coversToRemove == null)
                {
                    LoadCoversToRemove();
                    if (coversToRemove == null)
                        return new List<CoverConditionBase>(0).AsReadOnly();
                }
                return new ReadOnlyCollection<CoverConditionBase>(coversToRemove);
            }
        }

        /// <summary>
        /// Loads the covers to remove.
        /// </summary>
        private void LoadCoversToRemove()
        {
            if (this.TangibleObjectChange != null)
            {
                List<CoverConditionBase> coverConditionBases = new List<CoverConditionBase>();
                foreach (CoverCondition coverCondition in this.TangibleObjectChange.CoversToRemove)
                    coverConditionBases.Add(BaseManager.Current.GetBase<CoverConditionBase>(coverCondition));
                coversToRemove = coverConditionBases.ToArray();
            }
        }
        #endregion Property: CoversToRemove

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: TangibleObjectChangeBase(TangibleObjectChange tangibleObjectChange)
        /// <summary>
        /// Creates a base of the given tangible object change.
        /// </summary>
        /// <param name="tangibleObjectChange">The tangible object change to create a base of.</param>
        protected internal TangibleObjectChangeBase(TangibleObjectChange tangibleObjectChange)
            : base(tangibleObjectChange)
        {
            if (tangibleObjectChange != null)
            {
                if (BaseManager.PreloadProperties)
                {
                    LoadParts();
                    LoadPartsToAdd();
                    LoadPartsToRemove();
                    LoadConnections();
                    LoadConnectionsToAdd();
                    LoadConnectionsToRemove();
                    LoadMatter();
                    LoadMatterToAdd();
                    LoadMatterToRemove();
                    LoadLayers();
                    LoadLayersToAdd();
                    LoadLayersToRemove();
                    LoadCovers();
                    LoadCoversToAdd();
                    LoadCoversToRemove();
                }
            }
        }
        #endregion Constructor: TangibleObjectChangeBase(TangibleObjectChange tangibleObjectChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: TangibleObjectChangeBase

    #region Class: PartBase
    /// <summary>
    /// A base of a part.
    /// </summary>
    public sealed class PartBase : TangibleObjectValuedBase
    {

        #region Properties and Fields

        #region Property: Part
        /// <summary>
        /// Gets the part of which this is a part base.
        /// </summary>
        internal Part Part
        {
            get
            {
                return this.NodeValued as Part;
            }
        }
        #endregion Property: Part
        
        #region Property: Offset
        /// <summary>
        /// The offset.
        /// </summary>
        private VectorValueBase offset = null;

        /// <summary>
        /// Gets the offset.
        /// </summary>
        public VectorValueBase Offset
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
            if (this.Part != null)
                offset = BaseManager.Current.GetBase<VectorValueBase>(this.Part.Offset);
            else
                offset = new VectorValueBase(new Vec4(SemanticsSettings.Values.OffsetX, SemanticsSettings.Values.OffsetY, SemanticsSettings.Values.OffsetZ, SemanticsSettings.Values.OffsetW));
        }
        #endregion Property: Offset

        #region Property: Rotation
        /// <summary>
        /// The rotation.
        /// </summary>
        private VectorValueBase rotation = null;

        /// <summary>
        /// Gets the rotation.
        /// </summary>
        public VectorValueBase Rotation
        {
            get
            {
                if (rotation == null)
                    LoadRotation();
                return rotation;
            }
        }

        /// <summary>
        /// Loads the rotation.
        /// </summary>
        private void LoadRotation()
        {
            if (this.Part != null)
                rotation = BaseManager.Current.GetBase<VectorValueBase>(this.Part.Rotation);
            else
                rotation = new VectorValueBase(new Vec4(SemanticsSettings.Values.RotationX, SemanticsSettings.Values.RotationY, SemanticsSettings.Values.RotationZ, SemanticsSettings.Values.RotationW));
        }
        #endregion Property: Rotation

        #region Property: IsSlotBased
        /// <summary>
        /// The value that indicates whether the part is slot based.
        /// </summary>
        private bool isSlotBased = false;

        /// <summary>
        /// Gets the value that indicates whether the part is slot based.
        /// </summary>
        public bool IsSlotBased
        {
            get
            {
                return isSlotBased;
            }
        }
        #endregion Property: IsSlotBased

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: PartBase(Part part)
        /// <summary>
        /// Creates a new part base from the given part.
        /// </summary>
        /// <param name="part">The part to create the part base from.</param>
        internal PartBase(Part part)
            : base(part)
        {
            if (part != null)
            {
                this.isSlotBased = part.IsSlotBased;

                if (BaseManager.PreloadProperties)
                {
                    LoadOffset();
                    LoadRotation();
                }
            }
        }
        #endregion Constructor: PartBase(Part part)

        #endregion Method Group: Constructors

    }
    #endregion Class: PartBase

    #region Class: PartConditionBase
    /// <summary>
    /// A condition on a part.
    /// </summary>
    public sealed class PartConditionBase : TangibleObjectConditionBase
    {

        #region Properties and Fields

        #region Property: PartCondition
        /// <summary>
        /// Gets the part condition of which this is a part condition base.
        /// </summary>
        internal PartCondition PartCondition
        {
            get
            {
                return this.Condition as PartCondition;
            }
        }
        #endregion Property: PartCondition

        #region Property: Offset
        /// <summary>
        /// The required offset.
        /// </summary>
        private ValueConditionBase offset = null;

        /// <summary>
        /// Gets the required offset.
        /// </summary>
        public ValueConditionBase Offset
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
            if (this.PartCondition != null)
                offset = BaseManager.Current.GetBase<ValueConditionBase>(this.PartCondition.Offset);
        }
        #endregion Property: Offset

        #region Property: Rotation
        /// <summary>
        /// The required rotation.
        /// </summary>
        private ValueConditionBase rotation = null;

        /// <summary>
        /// Gets the required rotation.
        /// </summary>
        public ValueConditionBase Rotation
        {
            get
            {
                if (rotation == null)
                    LoadRotation();
                return rotation;
            }
        }

        /// <summary>
        /// Loads the rotation.
        /// </summary>
        private void LoadRotation()
        {
            if (this.PartCondition != null)
                rotation = BaseManager.Current.GetBase<ValueConditionBase>(this.PartCondition.Rotation);
        }
        #endregion Property: Rotation

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: PartConditionBase(PartCondition partCondition)
        /// <summary>
        /// Creates a base of the given part condition.
        /// </summary>
        /// <param name="partCondition">The part condition to create a base of.</param>
        internal PartConditionBase(PartCondition partCondition)
            : base(partCondition)
        {
            if (partCondition != null)
            {
                if (BaseManager.PreloadProperties)
                {
                    LoadOffset();
                    LoadRotation();
                }
            }
        }
        #endregion Constructor: PartConditionBase(PartCondition partCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: PartConditionBase

    #region Class: PartChangeBase
    /// <summary>
    /// A change on a part.
    /// </summary>
    public sealed class PartChangeBase : TangibleObjectChangeBase
    {

        #region Properties and Fields

        #region Property: PartChange
        /// <summary>
        /// Gets the part change of which this is a part change base.
        /// </summary>
        internal PartChange PartChange
        {
            get
            {
                return this.Change as PartChange;
            }
        }
        #endregion Property: PartChange

        #region Property: Offset
        /// <summary>
        /// The offset to change with.
        /// </summary>
        private ValueChangeBase offset = null;

        /// <summary>
        /// Gets or sets the offset to change with.
        /// </summary>
        public ValueChangeBase Offset
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
            if (this.PartChange != null)
                offset = BaseManager.Current.GetBase<ValueChangeBase>(this.PartChange.Offset);
        }
        #endregion Property: Offset

        #region Property: Rotation
        /// <summary>
        /// The rotation to change with.
        /// </summary>
        private ValueChangeBase rotation = null;

        /// <summary>
        /// Gets or sets the rotation to change with.
        /// </summary>
        public ValueChangeBase Rotation
        {
            get
            {
                if (rotation == null)
                    LoadRotation();
                return rotation;
            }
        }

        /// <summary>
        /// Loads the rotation.
        /// </summary>
        private void LoadRotation()
        {
            if (this.PartChange != null)
                rotation = BaseManager.Current.GetBase<ValueChangeBase>(this.PartChange.Rotation);
        }
        #endregion Property: Rotation

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: PartChangeBase(PartChange partChange)
        /// <summary>
        /// Creates a base of the given tangible object change.
        /// </summary>
        /// <param name="tangibleObjectChange">The tangible object change to create a base of.</param>
        internal PartChangeBase(PartChange partChange)
            : base(partChange)
        {
            if (partChange != null)
            {
                if (BaseManager.PreloadProperties)
                {
                    LoadOffset();
                    LoadRotation();
                }
            }
        }
        #endregion Constructor: PartChangeBase(PartChange partChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: PartChangeBase

    #region Class: CoverBase
    /// <summary>
    /// A base of a cover.
    /// </summary>
    public sealed class CoverBase : TangibleObjectValuedBase
    {

        #region Properties and Fields

        #region Property: Cover
        /// <summary>
        /// Gets the cover of which this is a cover base.
        /// </summary>
        internal Cover Cover
        {
            get
            {
                return this.NodeValued as Cover;
            }
        }
        #endregion Property: Cover

        #region Property: Offset
        /// <summary>
        /// The offset.
        /// </summary>
        private VectorValueBase offset = null;

        /// <summary>
        /// Gets the offset.
        /// </summary>
        public VectorValueBase Offset
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
            if (this.Cover != null)
                offset = BaseManager.Current.GetBase<VectorValueBase>(this.Cover.Offset);
            else
                offset = new VectorValueBase(new Vec4(SemanticsSettings.Values.OffsetX, SemanticsSettings.Values.OffsetY, SemanticsSettings.Values.OffsetZ, SemanticsSettings.Values.OffsetW));
        }
        #endregion Property: Offset

        #region Property: Rotation
        /// <summary>
        /// The rotation.
        /// </summary>
        private VectorValueBase rotation = null;

        /// <summary>
        /// Gets the rotation.
        /// </summary>
        public VectorValueBase Rotation
        {
            get
            {
                if (rotation == null)
                    LoadRotation();
                return rotation;
            }
        }

        /// <summary>
        /// Loads the rotation.
        /// </summary>
        private void LoadRotation()
        {
            if (this.Cover != null)
                rotation = BaseManager.Current.GetBase<VectorValueBase>(this.Cover.Rotation);
            else
                rotation = new VectorValueBase(new Vec4(SemanticsSettings.Values.RotationX, SemanticsSettings.Values.RotationY, SemanticsSettings.Values.RotationZ, SemanticsSettings.Values.RotationW));
        }
        #endregion Property: Rotation

        #region Property: Index
        /// <summary>
        /// The index of the cover when it is stacked on top of or underneath other covers: "the n-th cover".
        /// </summary>
        private int index = SemanticsSettings.Values.Index;

        /// <summary>
        /// Gets the index of the cover when it is stacked on top of or underneath other covers: "the n-th cover".
        /// </summary>
        public int Index
        {
            get
            {
                return index;
            }
        }
        #endregion Property: Index

        #region Property: IsSlotBased
        /// <summary>
        /// The value that indicates whether the cover is slot based.
        /// </summary>
        private bool isSlotBased = false;

        /// <summary>
        /// Gets the value that indicates whether the cover is slot based.
        /// </summary>
        public bool IsSlotBased
        {
            get
            {
                return isSlotBased;
            }
        }
        #endregion Property: IsSlotBased

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: CoverBase(Cover cover)
        /// <summary>
        /// Creates a cover base from the given cover.
        /// </summary>
        /// <param name="cover">The cover to create a cover base from.</param>
        internal CoverBase(Cover cover)
            : base(cover)
        {
            if (cover != null)
            {
                this.index = cover.Index;
                this.isSlotBased = cover.IsSlotBased;

                if (BaseManager.PreloadProperties)
                {
                    LoadOffset();
                    LoadRotation();
                }
            }
        }
        #endregion Constructor: CoverBase(Cover cover)

        #endregion Method Group: Constructors

    }
    #endregion Class: CoverBase

    #region Class: CoverConditionBase
    /// <summary>
    /// A condition on a cover.
    /// </summary>
    public sealed class CoverConditionBase : TangibleObjectConditionBase
    {

        #region Properties and Fields

        #region Property: CoverCondition
        /// <summary>
        /// Gets the cover condition of which this is a cover condition base.
        /// </summary>
        internal CoverCondition CoverCondition
        {
            get
            {
                return this.Condition as CoverCondition;
            }
        }
        #endregion Property: CoverCondition

        #region Property: Offset
        /// <summary>
        /// The required offset.
        /// </summary>
        private ValueConditionBase offset = null;

        /// <summary>
        /// Gets the required offset.
        /// </summary>
        public ValueConditionBase Offset
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
            if (this.CoverCondition != null)
                offset = BaseManager.Current.GetBase<ValueConditionBase>(this.CoverCondition.Offset);
        }
        #endregion Property: Offset

        #region Property: Rotation
        /// <summary>
        /// The required rotation.
        /// </summary>
        private ValueConditionBase rotation = null;

        /// <summary>
        /// Gets the required rotation.
        /// </summary>
        public ValueConditionBase Rotation
        {
            get
            {
                if (rotation == null)
                    LoadRotation();
                return rotation;
            }
        }

        /// <summary>
        /// Loads the rotation.
        /// </summary>
        private void LoadRotation()
        {
            if (this.CoverCondition != null)
                rotation = BaseManager.Current.GetBase<ValueConditionBase>(this.CoverCondition.Rotation);
        }
        #endregion Property: Rotation

        #region Property: Index
        /// <summary>
        /// The required index of the cover.
        /// </summary>
        private int? index = null;

        /// <summary>
        /// Gets the required index of the cover.
        /// </summary>
        public int? Index
        {
            get
            {
                return index;
            }
        }
        #endregion Property: Index

        #region Property: IndexSign
        /// <summary>
        /// The sign for the index in the condition.
        /// </summary>
        private EqualitySignExtended? indexSign = null;

        /// <summary>
        /// Gets the sign for the index in the condition.
        /// </summary>
        public EqualitySignExtended? IndexSign
        {
            get
            {
                return indexSign;
            }
        }
        #endregion Property: IndexSign

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: CoverConditionBase(CoverCondition coverCondition)
        /// <summary>
        /// Creates a base of the given cover condition.
        /// </summary>
        /// <param name="coverCondition">The cover condition to create a base of.</param>
        internal CoverConditionBase(CoverCondition coverCondition)
            : base(coverCondition)
        {
            if (coverCondition != null)
            {
                this.index = coverCondition.Index;
                this.indexSign = coverCondition.IndexSign;

                if (BaseManager.PreloadProperties)
                {
                    LoadOffset();
                    LoadRotation();
                }
            }
        }
        #endregion Constructor: CoverConditionBase(CoverCondition coverCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: CoverConditionBase

    #region Class: CoverChangeBase
    /// <summary>
    /// A change on a cover.
    /// </summary>
    public sealed class CoverChangeBase : TangibleObjectChangeBase
    {

        #region Properties and Fields

        #region Property: CoverChange
        /// <summary>
        /// Gets the cover change of which this is a cover change base.
        /// </summary>
        internal CoverChange CoverChange
        {
            get
            {
                return this.Change as CoverChange;
            }
        }
        #endregion Property: CoverChange

        #region Property: Offset
        /// <summary>
        /// The offset to change with.
        /// </summary>
        private ValueChangeBase offset = null;

        /// <summary>
        /// Gets or sets the offset to change with.
        /// </summary>
        public ValueChangeBase Offset
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
            if (this.CoverChange != null)
                offset = BaseManager.Current.GetBase<ValueChangeBase>(this.CoverChange.Offset);
        }
        #endregion Property: Offset

        #region Property: Rotation
        /// <summary>
        /// The rotation to change with.
        /// </summary>
        private ValueChangeBase rotation = null;

        /// <summary>
        /// Gets or sets the rotation to change with.
        /// </summary>
        public ValueChangeBase Rotation
        {
            get
            {
                if (rotation == null)
                    LoadRotation();
                return rotation;
            }
        }

        /// <summary>
        /// Loads the rotation.
        /// </summary>
        private void LoadRotation()
        {
            if (this.CoverChange != null)
                rotation = BaseManager.Current.GetBase<ValueChangeBase>(this.CoverChange.Rotation);
        }
        #endregion Property: Rotation

        #region Property: Index
        /// <summary>
        /// The index.
        /// </summary>
        private int? index = null;

        /// <summary>
        /// Gets the index.
        /// </summary>
        public int? Index
        {
            get
            {
                return index;
            }
        }
        #endregion Property: Index

        #region Property: IndexChange
        /// <summary>
        /// The type of change for the index.
        /// </summary>
        private ValueChangeType? indexChange = null;

        /// <summary>
        /// Gets the type of change for the index.
        /// </summary>
        public ValueChangeType? IndexChange
        {
            get
            {
                return indexChange;
            }
        }
        #endregion Property: IndexChange

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: CoverChangeBase(CoverChange coverChange)
        /// <summary>
        /// Creates a base of the given cover change.
        /// </summary>
        /// <param name="coverChange">The cover change to create a base of.</param>
        internal CoverChangeBase(CoverChange coverChange)
            : base(coverChange)
        {
            if (coverChange != null)
            {
                this.index = coverChange.Index;
                this.indexChange = coverChange.IndexChange;

                if (BaseManager.PreloadProperties)
                {
                    LoadOffset();
                    LoadRotation();
                }
            }
        }
        #endregion Constructor: CoverChangeBase(CoverChange coverChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: CoverChangeBase

    #region Class: ConnectionItemBase
    /// <summary>
    /// A base of a connection item.
    /// </summary>
    public sealed class ConnectionItemBase : TangibleObjectValuedBase
    {

        #region Properties and Fields

        #region Property: ConnectionItem
        /// <summary>
        /// Gets the connection item of which this is a connection item base.
        /// </summary>
        internal ConnectionItem ConnectionItem
        {
            get
            {
                return this.NodeValued as ConnectionItem;
            }
        }
        #endregion Property: ConnectionItem

        #region Property: IsSlotBased
        /// <summary>
        /// The value that indicates whether the connection item is slot based.
        /// </summary>
        private bool isSlotBased = false;

        /// <summary>
        /// Gets the value that indicates whether the connection item is slot based.
        /// </summary>
        public bool IsSlotBased
        {
            get
            {
                return isSlotBased;
            }
        }
        #endregion Property: IsSlotBased

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ConnectionItemBase(ConnectionItem connectionItem)
        /// <summary>
        /// Creates a new connection item base from the given connection item.
        /// </summary>
        /// <param name="connectionItem">The connection item to create the connection item base from.</param>
        internal ConnectionItemBase(ConnectionItem connectionItem)
            : base(connectionItem)
        {
            if (connectionItem != null)
                this.isSlotBased = connectionItem.IsSlotBased;
        }
        #endregion Constructor: ConnectionItemBase(ConnectionItem connectionItem)

        #endregion Method Group: Constructors

    }
    #endregion Class: ConnectionItemBase

    #region Class: ConnectionItemConditionBase
    /// <summary>
    /// A condition on a connection item.
    /// </summary>
    public sealed class ConnectionItemConditionBase : TangibleObjectConditionBase
    {

        #region Properties and Fields

        #region Property: ConnectionItemCondition
        /// <summary>
        /// Gets the connection item condition of which this is a connection item condition base.
        /// </summary>
        internal ConnectionItemCondition ConnectionItemCondition
        {
            get
            {
                return this.Condition as ConnectionItemCondition;
            }
        }
        #endregion Property: ConnectionItemCondition

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ConnectionItemConditionBase(ConnectionItemCondition connectionItemCondition)
        /// <summary>
        /// Creates a base of the given connection item condition.
        /// </summary>
        /// <param name="connectionItemCondition">The connection item condition to create a base of.</param>
        internal ConnectionItemConditionBase(ConnectionItemCondition connectionItemCondition)
            : base(connectionItemCondition)
        {
        }
        #endregion Constructor: ConnectionItemConditionBase(ConnectionItemCondition connectionItemCondition)

        #endregion Method Group: Constructors

    }
    #endregion Class: ConnectionItemConditionBase

    #region Class: ConnectionItemChangeBase
    /// <summary>
    /// A change on a connection item.
    /// </summary>
    public sealed class ConnectionItemChangeBase : TangibleObjectChangeBase
    {

        #region Properties and Fields

        #region Property: ConnectionItemChange
        /// <summary>
        /// Gets the connection item change of which this is a connection item change base.
        /// </summary>
        internal ConnectionItemChange ConnectionItemChange
        {
            get
            {
                return this.Change as ConnectionItemChange;
            }
        }
        #endregion Property: ConnectionItemChange

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ConnectionItemChangeBase(ConnectionItemChange connectionItemChange)
        /// <summary>
        /// Creates a base of the given tangible object change.
        /// </summary>
        /// <param name="tangibleObjectChange">The tangible object change to create a base of.</param>
        internal ConnectionItemChangeBase(ConnectionItemChange connectionItemChange)
            : base(connectionItemChange)
        {
        }
        #endregion Constructor: ConnectionItemChangeBase(ConnectionItemChange connectionItemChange)

        #endregion Method Group: Constructors

    }
    #endregion Class: ConnectionItemChangeBase

}