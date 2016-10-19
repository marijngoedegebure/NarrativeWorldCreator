/**************************************************************************
 * 
 * TangibleObject.cs
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
using Semantics.Data;
using Semantics.Utilities;

namespace Semantics.Entities
{

    #region Class: TangibleObject
    /// <summary>
    /// A tangible object.
    /// </summary>
    public class TangibleObject : PhysicalObject, IComparable<TangibleObject>
    {

        #region Properties and Fields

        #region Property: Parts
        /// <summary>
        /// Gets all parts of the tangible object.
        /// </summary>
        public ReadOnlyCollection<Part> Parts
        {
            get
            {
                List<Part> parts = new List<Part>();
                parts.AddRange(this.PersonalParts);
                parts.AddRange(this.InheritedParts);
                parts.AddRange(this.OverriddenParts);
                return parts.AsReadOnly();
            }
        }
        #endregion Property: Parts

        #region Property: PersonalParts
        /// <summary>
        /// Gets the personal parts of this tangible object.
        /// </summary>
        public ReadOnlyCollection<Part> PersonalParts
        {
            get
            {
                return Database.Current.SelectAll<Part>(this.ID, GenericTables.TangibleObjectPart, Columns.Part).AsReadOnly();
            }
        }
        #endregion Property: PersonalParts

        #region Property: InheritedParts
        /// <summary>
        /// Gets the inherited parts of the tangible object.
        /// </summary>
        public ReadOnlyCollection<Part> InheritedParts
        {
            get
            {
                List<Part> inheritedParts = new List<Part>();

                foreach (Node parent in this.PersonalParents)
                {
                    foreach (Part inheritedPart in ((TangibleObject)parent).Parts)
                    {
                        if (!HasOverriddenPart(inheritedPart.TangibleObject))
                            inheritedParts.Add(inheritedPart);
                    }
                }

                return inheritedParts.AsReadOnly();
            }
        }
        #endregion Property: InheritedParts

        #region Property: OverriddenParts
        /// <summary>
        /// Gets the overridden parts.
        /// </summary>
        public ReadOnlyCollection<Part> OverriddenParts
        {
            get
            {
                return Database.Current.SelectAll<Part>(this.ID, GenericTables.TangibleObjectOverriddenPart, Columns.Part).AsReadOnly();
            }
        }
        #endregion Property: OverriddenParts

        #region Property: Connections
        /// <summary>
        /// Gets the tangible objects to which this tangible object can be connected.
        /// </summary>
        public ReadOnlyCollection<ConnectionItem> Connections
        {
            get
            {
                List<ConnectionItem> connections = new List<ConnectionItem>();
                connections.AddRange(this.PersonalConnections);
                connections.AddRange(this.InheritedConnections);
                connections.AddRange(this.OverriddenConnections);
                return connections.AsReadOnly();
            }
        }
        #endregion Property: Connections

        #region Property: PersonalConnections
        /// <summary>
        /// Gets the personal tangible objects to which this tangible object can be connected.
        /// </summary>
        public ReadOnlyCollection<ConnectionItem> PersonalConnections
        {
            get
            {
                return Database.Current.SelectAll<ConnectionItem>(this.ID, GenericTables.TangibleObjectConnectionItem, Columns.ConnectionItem).AsReadOnly();
            }
        }
        #endregion Property: PersonalConnections

        #region Property: InheritedConnections
        /// <summary>
        /// Gets the inherited tangible objects to which this tangible object can be connected.
        /// </summary>
        public ReadOnlyCollection<ConnectionItem> InheritedConnections
        {
            get
            {
                List<ConnectionItem> inheritedConnections = new List<ConnectionItem>();

                foreach (Node parent in this.PersonalParents)
                    inheritedConnections.AddRange(((TangibleObject)parent).Connections);

                foreach (Node parent in this.PersonalParents)
                {
                    foreach (ConnectionItem inheritedConnection in ((TangibleObject)parent).Connections)
                    {
                        if (!HasOverriddenConnectionItem(inheritedConnection.TangibleObject))
                            inheritedConnections.Add(inheritedConnection);
                    }
                }

                return inheritedConnections.AsReadOnly();
            }
        }
        #endregion Property: InheritedConnections

        #region Property: OverriddenConnections
        /// <summary>
        /// Gets the overridden connections.
        /// </summary>
        public ReadOnlyCollection<ConnectionItem> OverriddenConnections
        {
            get
            {
                return Database.Current.SelectAll<ConnectionItem>(this.ID, GenericTables.TangibleObjectOverriddenConnectionItem, Columns.ConnectionItem).AsReadOnly();
            }
        }
        #endregion Property: OverriddenConnections

        #region Property: Matter
        /// <summary>
        /// Gets the matter of this tangible object.
        /// </summary>
        public ReadOnlyCollection<MatterValued> Matter
        {
            get
            {
                List<MatterValued> matter = new List<MatterValued>();
                matter.AddRange(this.PersonalMatter);
                matter.AddRange(this.InheritedMatter);
                matter.AddRange(this.OverriddenMatter);
                return matter.AsReadOnly();
            }
        }
        #endregion Property: Matter

        #region Property: PersonalMatter
        /// <summary>
        /// Gets the personal matter this tangible object consists of.
        /// </summary>
        public ReadOnlyCollection<MatterValued> PersonalMatter
        {
            get
            {
                return Database.Current.SelectAll<MatterValued>(this.ID, GenericTables.TangibleObjectMatter, Columns.MatterValued).AsReadOnly();
            }
        }
        #endregion Property: PersonalMatter

        #region Property: InheritedMatter
        /// <summary>
        /// Gets the inherited matter of the tangible object.
        /// </summary>
        public ReadOnlyCollection<MatterValued> InheritedMatter
        {
            get
            {
                List<MatterValued> inheritedMatter = new List<MatterValued>();

                foreach (Node parent in this.PersonalParents)
                {
                    foreach (MatterValued inheritedMatter2 in ((TangibleObject)parent).Matter)
                    {
                        if (!HasOverriddenMatter(inheritedMatter2.Matter))
                            inheritedMatter.Add(inheritedMatter2);
                    }
                }

                return inheritedMatter.AsReadOnly();
            }
        }
        #endregion Property: InheritedMatter

        #region Property: OverriddenMatter
        /// <summary>
        /// Gets the overridden matter.
        /// </summary>
        public ReadOnlyCollection<MatterValued> OverriddenMatter
        {
            get
            {
                return Database.Current.SelectAll<MatterValued>(this.ID, GenericTables.TangibleObjectOverriddenMatter, Columns.MatterValued).AsReadOnly();
            }
        }
        #endregion Property: OverriddenMatter

        #region Property: Layers
        /// <summary>
        /// Gets the layers of this tangible object.
        /// </summary>
        public ReadOnlyCollection<Layer> Layers
        {
            get
            {
                List<Layer> layers = new List<Layer>();
                layers.AddRange(this.PersonalLayers);
                layers.AddRange(this.InheritedLayers);
                layers.AddRange(this.OverriddenLayers);
                return layers.AsReadOnly();
            }
        }
        #endregion Property: Layers

        #region Property: PersonalLayers
        /// <summary>
        /// Gets the personal layers.
        /// </summary>
        public ReadOnlyCollection<Layer> PersonalLayers
        {
            get
            {
                return Database.Current.SelectAll<Layer>(this.ID, GenericTables.TangibleObjectLayer, Columns.Layer).AsReadOnly();
            }
        }
        #endregion Property: PersonalLayers

        #region Property: InheritedLayers
        /// <summary>
        /// Gets the inherited layers.
        /// </summary>
        public ReadOnlyCollection<Layer> InheritedLayers
        {
            get
            {
                List<Layer> inheritedLayers = new List<Layer>();

                foreach (Node parent in this.PersonalParents)
                {
                    foreach (Layer inheritedLayer in ((TangibleObject)parent).Layers)
                    {
                        if (!HasOverriddenLayer(inheritedLayer.Matter))
                            inheritedLayers.Add(inheritedLayer);
                    }
                }

                return inheritedLayers.AsReadOnly();
            }
        }
        #endregion Property: InheritedLayers

        #region Property: OverriddenLayers
        /// <summary>
        /// Gets the overridden layers.
        /// </summary>
        public ReadOnlyCollection<Layer> OverriddenLayers
        {
            get
            {
                return Database.Current.SelectAll<Layer>(this.ID, GenericTables.TangibleObjectOverriddenLayer, Columns.Layer).AsReadOnly();
            }
        }
        #endregion Property: OverriddenLayers

        #region Property: Covers
        /// <summary>
        /// Gets all covers of the tangible object.
        /// </summary>
        public ReadOnlyCollection<Cover> Covers
        {
            get
            {
                List<Cover> covers = new List<Cover>();
                covers.AddRange(this.PersonalCovers);
                covers.AddRange(this.InheritedCovers);
                covers.AddRange(this.OverriddenCovers);
                return covers.AsReadOnly();
            }
        }
        #endregion Property: Covers

        #region Property: PersonalCovers
        /// <summary>
        /// Gets the personal covers of this tangible object.
        /// </summary>
        public ReadOnlyCollection<Cover> PersonalCovers
        {
            get
            {
                return Database.Current.SelectAll<Cover>(this.ID, GenericTables.TangibleObjectCover, Columns.Cover).AsReadOnly();
            }
        }
        #endregion Property: PersonalCovers

        #region Property: InheritedCovers
        /// <summary>
        /// Gets the inherited covers of the tangible object.
        /// </summary>
        public ReadOnlyCollection<Cover> InheritedCovers
        {
            get
            {
                List<Cover> inheritedCovers = new List<Cover>();

                foreach (Node parent in this.PersonalParents)
                {
                    foreach (Cover inheritedCover in ((TangibleObject)parent).Covers)
                    {
                        if (!HasOverriddenCover(inheritedCover.TangibleObject))
                            inheritedCovers.Add(inheritedCover);
                    }
                }

                return inheritedCovers.AsReadOnly();
            }
        }
        #endregion Property: InheritedCovers

        #region Property: OverriddenCovers
        /// <summary>
        /// Gets the overridden covers.
        /// </summary>
        public ReadOnlyCollection<Cover> OverriddenCovers
        {
            get
            {
                return Database.Current.SelectAll<Cover>(this.ID, GenericTables.TangibleObjectOverriddenCover, Columns.Cover).AsReadOnly();
            }
        }
        #endregion Property: OverriddenCovers

        #region Property: Wholes
        /// <summary>
        /// Gets the tangible objects this object can be a part of.
        /// </summary>
        public ReadOnlyCollection<TangibleObject> Wholes
        {
            get
            {
                return Database.Current.SelectAll<TangibleObject>(GenericTables.TangibleObjectPart, Columns.TangibleObject, this.ID).AsReadOnly();
            }
        }
        #endregion Property: Wholes

        #region Property: CoveredObjects
        /// <summary>
        /// Gets the tangible objects this tangible object covers.
        /// </summary>
        public ReadOnlyCollection<TangibleObject> CoveredObjects
        {
            get
            {
                return Database.Current.SelectAll<TangibleObject>(GenericTables.TangibleObjectCover, Columns.TangibleObject, this.ID).AsReadOnly();
            }
        }
        #endregion Property: CoveredObjects

        #region Property: SpacesItemOf
        /// <summary>
        /// Gets the spaces this tangible object is an item of.
        /// </summary>
        public ReadOnlyCollection<Space> SpacesItemOf
        {
            get
            {
                return Database.Current.SelectAll<Space>(GenericTables.SpaceItem, Columns.TangibleObject, this.ID).AsReadOnly();
            }
        }
        #endregion Property: SpacesItemOf

        #region Property: IsCompound
        /// <summary>
        /// Gets a value that indicates whether this tangible object is a compound object (is made from parts).
        /// </summary>
        public bool IsCompound
        {
            get
            {
                return this.Parts.Count > 0;
            }
        }
        #endregion Property: IsCompound
        
        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: TangibleObject()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static TangibleObject()
        {
            // Parts
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Part, new Tuple<Type, EntryType>(typeof(Part), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.TangibleObjectPart, typeof(TangibleObject), dict);

            // Overridden parts
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Part, new Tuple<Type, EntryType>(typeof(Part), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.TangibleObjectOverriddenPart, typeof(TangibleObject), dict);

            // Connections
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.ConnectionItem, new Tuple<Type, EntryType>(typeof(ConnectionItem), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.TangibleObjectConnectionItem, typeof(TangibleObject), dict);

            // Overridden connections
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.ConnectionItem, new Tuple<Type, EntryType>(typeof(ConnectionItem), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.TangibleObjectOverriddenConnectionItem, typeof(TangibleObject), dict);

            // Matter
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.MatterValued, new Tuple<Type, EntryType>(typeof(MatterValued), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.TangibleObjectMatter, typeof(TangibleObject), dict);

            // Overridden matter
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.MatterValued, new Tuple<Type, EntryType>(typeof(MatterValued), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.TangibleObjectOverriddenMatter, typeof(TangibleObject), dict);

            // Layers
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Layer, new Tuple<Type, EntryType>(typeof(Layer), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.TangibleObjectLayer, typeof(TangibleObject), dict);

            // Overridden layers
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Layer, new Tuple<Type, EntryType>(typeof(Layer), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.TangibleObjectOverriddenLayer, typeof(TangibleObject), dict);

            // Covers
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Cover, new Tuple<Type, EntryType>(typeof(Cover), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.TangibleObjectCover, typeof(TangibleObject), dict);

            // Overridden covers
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Cover, new Tuple<Type, EntryType>(typeof(Cover), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.TangibleObjectOverriddenCover, typeof(TangibleObject), dict);
        }
        #endregion Static Constructor: TangibleObject()

        #region Constructor: TangibleObject()
        /// <summary>
        /// Creates a new tangible object.
        /// </summary>
        public TangibleObject()
            : base()
        {
        }
        #endregion Constructor: TangibleObject()

        #region Constructor: TangibleObject(uint id)
        /// <summary>
        /// Creates a new tangible object from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a tangible object from.</param>
        protected TangibleObject(uint id)
            : base(id)
        {
        }
        #endregion Constructor: TangibleObject(uint id)

        #region Constructor: TangibleObject(string name)
        /// <summary>
        /// Creates a new tangible object with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the tangible object.</param>
        public TangibleObject(string name)
            : base(name)
        {
        }
        #endregion Constructor: TangibleObject(string name)

        #region Constructor: TangibleObject(TangibleObject tangibleObject)
        /// <summary>
        /// Clones a tangible object.
        /// </summary>
        /// <param name="tangibleObject">The tangible object to clone.</param>
        public TangibleObject(TangibleObject tangibleObject)
            : base(tangibleObject)
        {
            if (tangibleObject != null)
            {
                Database.Current.StartChange();

                foreach (Part part in tangibleObject.PersonalParts)
                    AddPart(new Part(part));
                foreach (Part part in tangibleObject.OverriddenParts)
                    AddOverriddenPart(new Part(part));
                foreach (ConnectionItem connection in tangibleObject.PersonalConnections)
                    TangibleObject.CreateConnection(this, connection.TangibleObject);
                foreach (ConnectionItem connection in tangibleObject.OverriddenConnections)
                    AddOverriddenConnection(new ConnectionItem(connection));
                foreach (MatterValued matterValued in tangibleObject.PersonalMatter)
                    AddMatter(matterValued.Clone());
                foreach (MatterValued matterValued in tangibleObject.OverriddenMatter)
                    AddOverriddenMatter(matterValued.Clone());
                foreach (Layer layer in tangibleObject.Layers)
                    AddLayer(new Layer(layer));
                foreach (Layer layer in tangibleObject.OverriddenLayers)
                    AddOverriddenLayer(new Layer(layer));
                foreach (Cover cover in tangibleObject.PersonalCovers)
                    AddCover(new Cover(cover));
                foreach (Cover cover in tangibleObject.OverriddenCovers)
                    AddOverriddenCover(new Cover(cover));

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: TangibleObject(TangibleObject tangibleObject)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddPart(Part part)
        /// <summary>
        /// Add a part.
        /// </summary>
        /// <param name="part">The part to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddPart(Part part)
        {
            if (part != null && part.TangibleObject != null)
            {
                // Check whether the part is already there
                if (HasPart(part.TangibleObject))
                    return Message.RelationExistsAlready;

                // Make sure to prevent looping
                if (HasPhysicalObject(part.TangibleObject))
                    return Message.RelationFail;

                // Insert the part
                Database.Current.Insert(this.ID, GenericTables.TangibleObjectPart, new string[] { Columns.TangibleObject, Columns.Part }, new object[] { part.TangibleObject, part });
                NotifyPropertyChanged("PersonalParts");
                NotifyPropertyChanged("Parts");

                return Message.RelationSuccess;
            }

            return Message.RelationFail;
        }
        #endregion Method: AddPart(Part part)

        #region Method: RemovePart(Part part)
        /// <summary>
        /// Removes a part.
        /// </summary>
        /// <param name="part">The part to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemovePart(Part part)
        {
            if (part != null)
            {
                if (this.Parts.Contains(part))
                {
                    // Remove the part
                    Database.Current.Remove(this.ID, GenericTables.TangibleObjectPart, Columns.Part, part);
                    NotifyPropertyChanged("PersonalParts");
                    NotifyPropertyChanged("Parts");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemovePart(Part part)

        #region Method: OverridePart(Part inheritedPart)
        /// <summary>
        /// Override the given inherited part.
        /// </summary>
        /// <param name="inheritedPart">The inherited part that should be overridden.</param>
        /// <returns>Returns whether the override has been successful.</returns>
        public Message OverridePart(Part inheritedPart)
        {
            if (inheritedPart != null && inheritedPart.TangibleObject != null && this.InheritedParts.Contains(inheritedPart))
            {
                // If the part is already available, there is no use to add it
                foreach (Part personalTangibleObject in this.PersonalParts)
                {
                    if (inheritedPart.TangibleObject.Equals(personalTangibleObject.TangibleObject))
                        return Message.RelationExistsAlready;
                }
                if (HasOverriddenPart(inheritedPart.TangibleObject))
                    return Message.RelationExistsAlready;

                // Copy the part and add it
                return AddOverriddenPart(new Part(inheritedPart));
            }
            return Message.RelationFail;
        }
        #endregion Method: OverridePart(Part inheritedPart)

        #region Method: AddOverriddenPart(Part inheritedPart)
        /// <summary>
        /// Add the given overridden part.
        /// </summary>
        /// <param name="overriddenPart">The overridden part to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        private Message AddOverriddenPart(Part overriddenPart)
        {
            if (overriddenPart != null)
            {
                Database.Current.Insert(this.ID, GenericTables.TangibleObjectOverriddenPart, Columns.Part, overriddenPart);
                NotifyPropertyChanged("OverriddenParts");
                NotifyPropertyChanged("InheritedParts");
                NotifyPropertyChanged("Parts");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddOverriddenPart(Part inheritedPart)

        #region Method: RemoveOverriddenPart(Part overriddenPart)
        /// <summary>
        /// Removes an overridden part.
        /// </summary>
        /// <param name="overriddenPart">The overridden part to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveOverriddenPart(Part overriddenPart)
        {
            if (overriddenPart != null)
            {
                if (this.OverriddenParts.Contains(overriddenPart))
                {
                    // Remove the overridden part
                    Database.Current.Remove(this.ID, GenericTables.TangibleObjectOverriddenPart, Columns.Part, overriddenPart);
                    NotifyPropertyChanged("OverriddenParts");
                    NotifyPropertyChanged("InheritedParts");
                    NotifyPropertyChanged("Parts");
                    overriddenPart.Remove();

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveOverriddenPart(Part overriddenPart)

        #region Method: CreateConnection(TangibleObject connectionItem1, TangibleObject connectionItem2)
        /// <summary>
        /// Create a connection between both items.
        /// </summary>
        /// <param name="connectionItem1">The item to connect.</param>
        /// <param name="connectionItem2">The item to connect with.</param>
        /// <returns>Returns whether the connection has made been successful.</returns>
        public static Message CreateConnection(TangibleObject connectionItem1, TangibleObject connectionItem2)
        {
            // Check if both items are not the same, and if the connection does not exist already
            if (connectionItem1 != null && connectionItem2 != null && !connectionItem1.Equals(connectionItem2))
            {
                if (connectionItem1.HasConnectionItem(connectionItem2) || connectionItem2.HasConnectionItem(connectionItem1))
                    return Message.RelationExistsAlready;

                // Create the connection
                Database.Current.Insert(connectionItem1.ID, GenericTables.TangibleObjectConnectionItem, new string[] { Columns.TangibleObject, Columns.ConnectionItem }, new object[] { connectionItem2, new ConnectionItem(connectionItem2) });
                connectionItem1.NotifyPropertyChanged("PersonalConnections");
                connectionItem1.NotifyPropertyChanged("Connections");
                connectionItem2.NotifyPropertyChanged("PersonalConnections");
                connectionItem2.NotifyPropertyChanged("Connections");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: CreateConnection(TangibleObject connectionItem1, TangibleObject connectionItem2)

        #region Method: RemoveConnection(TangibleObject connectionItem1, TangibleObject connectionItem2)
        /// <summary>
        /// Removes the connection between the given items.
        /// </summary>
        /// <param name="connectionItem1">The item to disconnect.</param>
        /// <param name="connectionItem2">The item to disconnect from.</param>
        /// <returns></returns>
        public static Message RemoveConnection(TangibleObject connectionItem1, TangibleObject connectionItem2)
        {
            if (connectionItem1 != null && connectionItem2 != null)
            {
                // Remove the connection
                bool success = false;
                foreach (ConnectionItem connectionItem in connectionItem1.Connections)
                {
                    if (connectionItem2.Equals(connectionItem.TangibleObject))
                    {
                        Database.Current.Remove(connectionItem1.ID, GenericTables.TangibleObjectConnectionItem, Columns.ConnectionItem, connectionItem);
                        success = true;
                        break;
                    }
                }
                foreach (ConnectionItem connectionItem in connectionItem2.Connections)
                {
                    if (connectionItem1.Equals(connectionItem.TangibleObject))
                    {
                        Database.Current.Remove(connectionItem2.ID, GenericTables.TangibleObjectConnectionItem, Columns.ConnectionItem, connectionItem);
                        success = true;
                        break;
                    }
                }
                if (success)
                {
                    connectionItem1.NotifyPropertyChanged("PersonalConnections");
                    connectionItem1.NotifyPropertyChanged("Connections");
                    connectionItem2.NotifyPropertyChanged("PersonalConnections");
                    connectionItem2.NotifyPropertyChanged("Connections");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveConnection(TangibleObject connectionItem1, TangibleObject connectionItem2)

        #region Method: OverrideConnection(ConnectionItem inheritedConnection)
        /// <summary>
        /// Override the given inherited connection.
        /// </summary>
        /// <param name="inheritedConnection">The inherited connection that should be overridden.</param>
        /// <returns>Returns whether the override has been successful.</returns>
        public Message OverrideConnection(ConnectionItem inheritedConnection)
        {
            if (inheritedConnection != null && inheritedConnection.TangibleObject != null && this.InheritedConnections.Contains(inheritedConnection))
            {
                // If the connection is already available, there is no use to add it
                foreach (ConnectionItem personalConnection in this.PersonalConnections)
                {
                    if (inheritedConnection.TangibleObject.Equals(personalConnection.TangibleObject))
                        return Message.RelationExistsAlready;
                }
                if (HasOverriddenConnectionItem(inheritedConnection.TangibleObject))
                    return Message.RelationExistsAlready;

                // Copy the connection item and add it
                return AddOverriddenConnection(new ConnectionItem(inheritedConnection));
            }
            return Message.RelationFail;
        }
        #endregion Method: OverrideConnection(ConnectionItem inheritedConnection)

        #region Method: AddOverriddenConnection(ConnectionItem inheritedConnection)
        /// <summary>
        /// Add the given overridden connection.
        /// </summary>
        /// <param name="overriddenConnection">The overridden connection to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        private Message AddOverriddenConnection(ConnectionItem overriddenConnection)
        {
            if (overriddenConnection != null)
            {
                Database.Current.Insert(this.ID, GenericTables.TangibleObjectOverriddenConnectionItem, Columns.ConnectionItem, overriddenConnection);
                NotifyPropertyChanged("OverriddenConnections");
                NotifyPropertyChanged("InheritedConnections");
                NotifyPropertyChanged("Connections");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddOverriddenConnection(ConnectionItem inheritedConnection)

        #region Method: RemoveOverriddenConnection(ConnectionItem overriddenConnection)
        /// <summary>
        /// Removes an overridden connection.
        /// </summary>
        /// <param name="overriddenConnection">The overridden connection to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveOverriddenConnection(ConnectionItem overriddenConnection)
        {
            if (overriddenConnection != null)
            {
                if (this.OverriddenConnections.Contains(overriddenConnection))
                {
                    // Remove the overridden connection
                    Database.Current.Remove(this.ID, GenericTables.TangibleObjectOverriddenConnectionItem, Columns.ConnectionItem, overriddenConnection);
                    NotifyPropertyChanged("OverriddenConnections");
                    NotifyPropertyChanged("InheritedConnections");
                    NotifyPropertyChanged("Connections");
                    overriddenConnection.Remove();

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveOverriddenConnection(ConnectionItem overriddenConnection)

        #region Method: AddMatter(MatterValued matterValued)
        /// <summary>
        /// Adds matter.
        /// </summary>
        /// <param name="matterValued">The matter to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddMatter(MatterValued matterValued)
        {
            if (matterValued != null)
            {
                // If the matter is already present, there's no use to add it again
                if (HasMatter(matterValued.Matter))
                    return Message.RelationExistsAlready;

                // Add the matter
                Database.Current.Insert(this.ID, GenericTables.TangibleObjectMatter, new string[] { Columns.Matter, Columns.MatterValued }, new object[] { matterValued.Matter, matterValued });
                NotifyPropertyChanged("PersonalMatter");
                NotifyPropertyChanged("Matter");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddMatter(MatterValued matterValued)

        #region Method: RemoveMatter(MatterValued matterValued)
        /// <summary>
        /// Removes matter.
        /// </summary>
        /// <param name="matterValued">The matter to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveMatter(MatterValued matterValued)
        {
            if (matterValued != null)
            {
                if (this.Matter.Contains(matterValued))
                {
                    // Remove the matter
                    Database.Current.Remove(this.ID, GenericTables.TangibleObjectMatter, Columns.MatterValued, matterValued);
                    NotifyPropertyChanged("PersonalMatter");
                    NotifyPropertyChanged("Matter");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveMatter(MatterValued matterValued)

        #region Method: OverrideMatter(MatterValued inheritedMatter)
        /// <summary>
        /// Override the given inherited matter.
        /// </summary>
        /// <param name="inheritedMatter">The inherited matter that should be overridden.</param>
        /// <returns>Returns whether the override has been successful.</returns>
        public Message OverrideMatter(MatterValued inheritedMatter)
        {
            if (inheritedMatter != null && inheritedMatter.Matter != null && this.InheritedMatter.Contains(inheritedMatter))
            {
                // If the matter is already available, there is no use to add it
                foreach (MatterValued personalMatter in this.PersonalMatter)
                {
                    if (inheritedMatter.Matter.Equals(personalMatter.Matter))
                        return Message.RelationExistsAlready;
                }
                if (HasOverriddenMatter(inheritedMatter.Matter))
                    return Message.RelationExistsAlready;

                // Copy the matter and add it
                return AddOverriddenMatter(inheritedMatter.Clone());
            }
            return Message.RelationFail;
        }
        #endregion Method: OverrideMatter(MatterValued inheritedMatter)

        #region Method: AddOverriddenMatter(MatterValued inheritedMatter)
        /// <summary>
        /// Add the given overridden matter.
        /// </summary>
        /// <param name="overriddenMatter">The overridden matter to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        private Message AddOverriddenMatter(MatterValued overriddenMatter)
        {
            if (overriddenMatter != null)
            {
                Database.Current.Insert(this.ID, GenericTables.TangibleObjectOverriddenMatter, Columns.MatterValued, overriddenMatter);
                NotifyPropertyChanged("OverriddenMatter");
                NotifyPropertyChanged("InheritedMatter");
                NotifyPropertyChanged("Matter");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddOverriddenMatter(MatterValued inheritedMatter)

        #region Method: RemoveOverriddenMatter(MatterValued overriddenMatter)
        /// <summary>
        /// Removes overridden matter.
        /// </summary>
        /// <param name="overriddenMatter">The overridden matter to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveOverriddenMatter(MatterValued overriddenMatter)
        {
            if (overriddenMatter != null)
            {
                if (this.OverriddenMatter.Contains(overriddenMatter))
                {
                    // Remove the overridden matter
                    Database.Current.Remove(this.ID, GenericTables.TangibleObjectOverriddenMatter, Columns.MatterValued, overriddenMatter);
                    NotifyPropertyChanged("OverriddenMatter");
                    NotifyPropertyChanged("InheritedMatter");
                    NotifyPropertyChanged("Matter");
                    overriddenMatter.Remove();

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveOverriddenMatter(MatterValued overriddenMatter)

        #region Method: AddLayer(Layer layer)
        /// <summary>
        /// Adds a layer.
        /// </summary>
        /// <param name="layer">The layer to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddLayer(Layer layer)
        {
            if (layer != null)
            {
                // If the layer is already present, there's no use to add it again
                if (HasLayer(layer.Matter))
                    return Message.RelationExistsAlready;

                // Add the layer
                Database.Current.Insert(this.ID, GenericTables.TangibleObjectLayer, new string[] { Columns.Matter, Columns.Layer }, new object[] { layer.Matter, layer });
                NotifyPropertyChanged("PersonalLayers");
                NotifyPropertyChanged("Layers");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddLayer(Layer layer)

        #region Method: RemoveLayer(Layer layer)
        /// <summary>
        /// Removes a layer.
        /// </summary>
        /// <param name="layer">The layer to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveLayer(Layer layer)
        {
            if (layer != null)
            {
                if (this.Layers.Contains(layer))
                {
                    // Remove the layer
                    Database.Current.Remove(this.ID, GenericTables.TangibleObjectLayer, Columns.Layer, layer);
                    NotifyPropertyChanged("PersonalLayers");
                    NotifyPropertyChanged("Layers");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveLayer(Layer layer)

        #region Method: OverrideLayer(Layer inheritedLayer)
        /// <summary>
        /// Override the given inherited layer.
        /// </summary>
        /// <param name="inheritedLayer">The inherited layer that should be overridden.</param>
        /// <returns>Returns whether the override has been successful.</returns>
        public Message OverrideLayer(Layer inheritedLayer)
        {
            if (inheritedLayer != null && inheritedLayer.Matter != null && this.InheritedLayers.Contains(inheritedLayer))
            {
                // If the layer is already available, there is no use to add it
                foreach (Layer personalMatter in this.PersonalLayers)
                {
                    if (inheritedLayer.Matter.Equals(personalMatter.Matter))
                        return Message.RelationExistsAlready;
                }
                if (HasOverriddenLayer(inheritedLayer.Matter))
                    return Message.RelationExistsAlready;

                // Copy the layer and add it
                return AddOverriddenLayer(new Layer(inheritedLayer));
            }
            return Message.RelationFail;
        }
        #endregion Method: OverrideLayer(Layer inheritedLayer)

        #region Method: AddOverriddenLayer(Layer inheritedLayer)
        /// <summary>
        /// Add the given overridden layer.
        /// </summary>
        /// <param name="overriddenLayer">The overridden layer to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        private Message AddOverriddenLayer(Layer overriddenLayer)
        {
            if (overriddenLayer != null)
            {
                Database.Current.Insert(this.ID, GenericTables.TangibleObjectOverriddenLayer, Columns.Layer, overriddenLayer);
                NotifyPropertyChanged("OverriddenLayers");
                NotifyPropertyChanged("InheritedLayers");
                NotifyPropertyChanged("Layers");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddOverriddenLayer(Layer inheritedLayer)

        #region Method: RemoveOverriddenLayer(Layer overriddenLayer)
        /// <summary>
        /// Removes an overridden layer.
        /// </summary>
        /// <param name="overriddenLayer">The overridden layer to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveOverriddenLayer(Layer overriddenLayer)
        {
            if (overriddenLayer != null)
            {
                if (this.OverriddenLayers.Contains(overriddenLayer))
                {
                    // Remove the overridden layer
                    Database.Current.Remove(this.ID, GenericTables.TangibleObjectOverriddenLayer, Columns.Layer, overriddenLayer);
                    NotifyPropertyChanged("OverriddenLayers");
                    NotifyPropertyChanged("InheritedLayers");
                    NotifyPropertyChanged("Layers");
                    overriddenLayer.Remove();

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveOverriddenLayer(Layer overriddenLayer)

        #region Method: AddCover(Cover cover)
        /// <summary>
        /// Add a cover.
        /// </summary>
        /// <param name="cover">The cover to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddCover(Cover cover)
        {
            if (cover != null)
            {
                // Check whether the cover is already there
                if (HasCover(cover.TangibleObject))
                    return Message.RelationExistsAlready;

                // In case this relation is invalid, make the cover optional
                if (HasPhysicalObject(cover.TangibleObject))
                    cover.Necessity = Necessity.Optional;

                // Insert the cover
                Database.Current.Insert(this.ID, GenericTables.TangibleObjectCover, new string[] { Columns.TangibleObject, Columns.Cover }, new object[] { cover.TangibleObject, cover });
                NotifyPropertyChanged("PersonalCovers");
                NotifyPropertyChanged("Covers");

                return Message.RelationSuccess;
            }

            return Message.RelationFail;
        }
        #endregion Method: AddCover(Cover cover)

        #region Method: RemoveCover(Cover cover)
        /// <summary>
        /// Removes a cover.
        /// </summary>
        /// <param name="cover">The cover to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveCover(Cover cover)
        {
            if (cover != null)
            {
                if (this.Covers.Contains(cover))
                {
                    // Remove the cover
                    Database.Current.Remove(this.ID, GenericTables.TangibleObjectCover, Columns.Cover, cover);
                    NotifyPropertyChanged("PersonalCovers");
                    NotifyPropertyChanged("Covers");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveCover(Cover cover)

        #region Method: OverrideCover(Cover inheritedCover)
        /// <summary>
        /// Override the given inherited cover.
        /// </summary>
        /// <param name="inheritedCover">The inherited cover that should be overridden.</param>
        /// <returns>Returns whether the override has been successful.</returns>
        public Message OverrideCover(Cover inheritedCover)
        {
            if (inheritedCover != null && inheritedCover.TangibleObject != null && this.InheritedCovers.Contains(inheritedCover))
            {
                // If the cover is already available, there is no use to add it
                foreach (Cover personalTangibleObject in this.PersonalCovers)
                {
                    if (inheritedCover.TangibleObject.Equals(personalTangibleObject.TangibleObject))
                        return Message.RelationExistsAlready;
                }
                if (HasOverriddenCover(inheritedCover.TangibleObject))
                    return Message.RelationExistsAlready;

                // Copy the cover and add it
                return AddOverriddenCover(new Cover(inheritedCover));
            }
            return Message.RelationFail;
        }
        #endregion Method: OverrideCover(Cover inheritedCover)

        #region Method: AddOverriddenCover(Cover inheritedCover)
        /// <summary>
        /// Add the given overridden cover.
        /// </summary>
        /// <param name="overriddenCover">The overridden cover to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        private Message AddOverriddenCover(Cover overriddenCover)
        {
            if (overriddenCover != null)
            {
                Database.Current.Insert(this.ID, GenericTables.TangibleObjectOverriddenCover, Columns.Cover, overriddenCover);
                NotifyPropertyChanged("OverriddenCovers");
                NotifyPropertyChanged("InheritedCovers");
                NotifyPropertyChanged("Covers");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddOverriddenCover(Cover inheritedCover)

        #region Method: RemoveOverriddenCover(Cover overriddenCover)
        /// <summary>
        /// Removes an overridden cover.
        /// </summary>
        /// <param name="overriddenCover">The overridden cover to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveOverriddenCover(Cover overriddenCover)
        {
            if (overriddenCover != null)
            {
                if (this.OverriddenCovers.Contains(overriddenCover))
                {
                    // Remove the overridden cover
                    Database.Current.Remove(this.ID, GenericTables.TangibleObjectOverriddenCover, Columns.Cover, overriddenCover);
                    NotifyPropertyChanged("OverriddenCovers");
                    NotifyPropertyChanged("InheritedCovers");
                    NotifyPropertyChanged("Covers");
                    overriddenCover.Remove();

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveOverriddenCover(Cover overriddenCover)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasPart(TangibleObject part)
        /// <summary>
        /// Checks if this tangible object has the given part.
        /// </summary>
        /// <param name="part">The part to check.</param>
        /// <returns>Returns true when the tangible object has the part.</returns>
        public bool HasPart(TangibleObject part)
        {
            if (part != null)
            {
                foreach (Part myPart in this.Parts)
                {
                    if (part.Equals(myPart.TangibleObject))
                        return true;
                }
            }

            return false;
        }
        #endregion Method: HasPart(TangibleObject part)

        #region Method: HasOverriddenPart(TangibleObject part)
        /// <summary>
        /// Checks if this tangible object has the given overridden part.
        /// </summary>
        /// <param name="part">The part to check.</param>
        /// <returns>Returns true when the tangible object has the overridden part.</returns>
        private bool HasOverriddenPart(TangibleObject part)
        {
            if (part != null)
            {
                foreach (Part myPart in this.OverriddenParts)
                {
                    if (part.Equals(myPart.TangibleObject))
                        return true;
                }
            }

            return false;
        }
        #endregion Method: HasOverriddenPart(TangibleObject part)

        #region Method: HasConnectionItem(TangibleObject connectionItem)
        /// <summary>
        /// Checks if this tangible object has a connection item of the given tangible object.
        /// </summary>
        /// <param name="connectionItem">The connection item to check.</param>
        /// <returns>Returns true when this tangible object has a connection item of the tangible object.</returns>
        public bool HasConnectionItem(TangibleObject connectionItem)
        {
            if (connectionItem != null)
            {
                foreach (ConnectionItem myConnectionItem in this.Connections)
                {
                    if (connectionItem.Equals(myConnectionItem.TangibleObject))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasConnectionItem(TangibleObject connectionItem)

        #region Method: HasOverriddenConnectionItem(TangibleObject connectionItem)
        /// <summary>
        /// Checks if this tangible object has an overridden connection item of the given tangible object.
        /// </summary>
        /// <param name="connectionItem">The connection item to check.</param>
        /// <returns>Returns true when this tangible object has an overridden connection item of the tangible object.</returns>
        public bool HasOverriddenConnectionItem(TangibleObject connectionItem)
        {
            if (connectionItem != null)
            {
                foreach (ConnectionItem myConnectionItem in this.OverriddenConnections)
                {
                    if (connectionItem.Equals(myConnectionItem.TangibleObject))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasOverriddenConnectionItem(TangibleObject connectionItem)

        #region Method: HasMatter(Matter matter)
        /// <summary>
        /// Checks if this tangible object has the given matter.
        /// </summary>
        /// <param name="matter">The matter to check.</param>
        /// <returns>Returns true when this tangible object has the matter.</returns>
        public bool HasMatter(Matter matter)
        {
            if (matter != null)
            {
                foreach (MatterValued matterValued in this.Matter)
                {
                    if (matter.Equals(matterValued.Matter))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasMatter(Matter matter)

        #region Method: HasOverriddenMatter(Matter matter)
        /// <summary>
        /// Checks if this tangible object has the given matter as overridden matter.
        /// </summary>
        /// <param name="matter">The matter to check.</param>
        /// <returns>Returns true when this tangible object has the matter as overridden matter.</returns>
        private bool HasOverriddenMatter(Matter matter)
        {
            if (matter != null)
            {
                foreach (MatterValued matterValued in this.OverriddenMatter)
                {
                    if (matter.Equals(matterValued.Matter))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasOverriddenMatter(Matter matter)

        #region Method: HasLayer(Matter matter)
        /// <summary>
        /// Checks if this tangible object has the given matter as a layer.
        /// </summary>
        /// <param name="matter">The matter to check.</param>
        /// <returns>Returns true when this tangible object has the matter as a layer.</returns>
        public bool HasLayer(Matter matter)
        {
            if (matter != null)
            {
                foreach (Layer layer in this.Layers)
                {
                    if (matter.Equals(layer.Matter))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasLayer(Matter matter)

        #region Method: HasOverriddenLayer(Matter matter)
        /// <summary>
        /// Checks if this tangible object has the given matter as an overridden layer.
        /// </summary>
        /// <param name="matter">The matter to check.</param>
        /// <returns>Returns true when this tangible object has the matter as an overridden layer.</returns>
        private bool HasOverriddenLayer(Matter matter)
        {
            if (matter != null)
            {
                foreach (Layer layer in this.OverriddenLayers)
                {
                    if (matter.Equals(layer.Matter))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasOverriddenLayer(Matter matter)

        #region Method: HasCover(TangibleObject cover)
        /// <summary>
        /// Checks if this tangible object has the given cover.
        /// </summary>
        /// <param name="cover">The cover to check.</param>
        /// <returns>Returns true when the tangible object has the cover.</returns>
        public bool HasCover(TangibleObject cover)
        {
            if (cover != null)
            {
                foreach (Cover myCover in this.Covers)
                {
                    if (cover.Equals(myCover.TangibleObject))
                        return true;
                }
            }

            return false;
        }
        #endregion Method: HasCover(TangibleObject cover)

        #region Method: HasOverriddenCover(TangibleObject cover)
        /// <summary>
        /// Checks if this tangible object has the given overridden cover.
        /// </summary>
        /// <param name="cover">The cover to check.</param>
        /// <returns>Returns true when the tangible object has the overridden cover.</returns>
        private bool HasOverriddenCover(TangibleObject cover)
        {
            if (cover != null)
            {
                foreach (Cover myCover in this.OverriddenCovers)
                {
                    if (cover.Equals(myCover.TangibleObject))
                        return true;
                }
            }

            return false;
        }
        #endregion Method: HasOverriddenCover(TangibleObject cover)

        #region Method: Remove()
        /// <summary>
        /// Remove the tangible object.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();
            Database.Current.StartRemove();

            // Remove the parts
            foreach (Part part in this.PersonalParts)
                part.Remove();
            Database.Current.Remove(this.ID, GenericTables.TangibleObjectPart);

            // Remove the overridden parts
            foreach (Part part in this.OverriddenParts)
                part.Remove();
            Database.Current.Remove(this.ID, GenericTables.TangibleObjectOverriddenPart);

            // Remove the matter
            foreach (MatterValued matterValued in this.PersonalMatter)
                matterValued.Remove();
            Database.Current.Remove(this.ID, GenericTables.TangibleObjectMatter);

            // Remove the overridden matter
            foreach (MatterValued matterValued in this.OverriddenMatter)
                matterValued.Remove();
            Database.Current.Remove(this.ID, GenericTables.TangibleObjectOverriddenMatter);

            // Remove the connections
            foreach (ConnectionItem connectionItem in this.PersonalConnections)
                connectionItem.Remove();
            Database.Current.Remove(this.ID, GenericTables.TangibleObjectConnectionItem);
            Database.Current.Remove(GenericTables.TangibleObjectConnectionItem, Columns.ConnectionItem, this.ID);

            // Remove the overridden connections
            foreach (ConnectionItem connectionItem in this.OverriddenConnections)
                connectionItem.Remove();
            Database.Current.Remove(this.ID, GenericTables.TangibleObjectOverriddenConnectionItem);

            // Remove the layers
            foreach (Layer layer in this.PersonalLayers)
                layer.Remove();
            Database.Current.Remove(this.ID, GenericTables.TangibleObjectLayer);

            // Remove the overridden layers
            foreach (Layer layer in this.OverriddenLayers)
                layer.Remove();
            Database.Current.Remove(this.ID, GenericTables.TangibleObjectOverriddenLayer);

            // Remove the covers
            foreach (Cover cover in this.PersonalCovers)
                cover.Remove();
            Database.Current.Remove(this.ID, GenericTables.TangibleObjectCover);

            // Remove the overridden covers
            foreach (Cover cover in this.OverriddenCovers)
                cover.Remove();
            Database.Current.Remove(this.ID, GenericTables.TangibleObjectOverriddenCover);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(TangibleObject other)
        /// <summary>
        /// Compares the tangible object to the other tangible object.
        /// </summary>
        /// <param name="other">The tangible object to compare to this tangible object.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(TangibleObject other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(TangibleObject other)

        #endregion Method Group: Other

    }
    #endregion Class: TangibleObject

    #region Class: TangibleObjectValued
    /// <summary>
    /// A valued version of a tangible object.
    /// </summary>
    public class TangibleObjectValued : PhysicalObjectValued
    {

        #region Properties and Fields

        #region Property: TangibleObject
        /// <summary>
        /// Gets the tangible object of which this is a valued tangible object.
        /// </summary>
        public TangibleObject TangibleObject
        {
            get
            {
                return this.Node as TangibleObject;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: TangibleObject

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: TangibleObjectValued(uint id)
        /// <summary>
        /// Creates a new valued tangible object from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the valued tangible object from.</param>
        protected TangibleObjectValued(uint id)
            : base(id)
        {
        }
        #endregion Constructor: TangibleObjectValued(uint id)

        #region Constructor: TangibleObjectValued(TangibleObjectValued tangibleObjectValued)
        /// <summary>
        /// Clones a valued tangible object.
        /// </summary>
        /// <param name="tangibleObjectValued">The valued tangible object to clone.</param>
        public TangibleObjectValued(TangibleObjectValued tangibleObjectValued)
            : base(tangibleObjectValued)
        {
        }
        #endregion Constructor: TangibleObjectValued(TangibleObjectValued tangibleObjectValued)

        #region Constructor: TangibleObjectValued(TangibleObject tangibleObject)
        /// <summary>
        /// Creates a new valued tangible object from the given tangible object.
        /// </summary>
        /// <param name="tangibleObject">The tangible object to create the valued tangible object from.</param>
        public TangibleObjectValued(TangibleObject tangibleObject)
            : base(tangibleObject)
        {
        }
        #endregion Constructor: TangibleObjectValued(TangibleObject tangibleObject)

        #region Constructor: TangibleObjectValued(TangibleObject tangibleObject, NumericalValueRange quantity)
        /// <summary>
        /// Creates a new valued tangible object from the given tangible object in the given quantity.
        /// </summary>
        /// <param name="tangibleObject">The tangible object to create the valued tangible object from.</param>
        /// <param name="quantity">The quantity of the valued tangible object.</param>
        public TangibleObjectValued(TangibleObject tangibleObject, NumericalValueRange quantity)
            : base(tangibleObject, quantity)
        {
        }
        #endregion Constructor: TangibleObjectValued(TangibleObject tangibleObject, NumericalValueRange quantity)

        #endregion Method Group: Constructors

    }
    #endregion Class: TangibleObjectValued

    #region Class: TangibleObjectCondition
    /// <summary>
    /// A condition on a tangible object.
    /// </summary>
    public class TangibleObjectCondition : PhysicalObjectCondition
    {

        #region Properties and Fields

        #region Property: TangibleObject
        /// <summary>
        /// Gets or sets the required tangible object.
        /// </summary>
        public TangibleObject TangibleObject
        {
            get
            {
                return this.Node as TangibleObject;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: TangibleObject

        #region Property: HasAllMandatoryParts
        /// <summary>
        /// Gets or sets the value that indicates whether all mandatory parts are required.
        /// </summary>
        public bool? HasAllMandatoryParts
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, ValueTables.TangibleObjectCondition, Columns.HasAllMandatoryParts);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.TangibleObjectCondition, Columns.HasAllMandatoryParts, value);
                NotifyPropertyChanged("HasAllMandatoryParts");
            }
        }
        #endregion Property: HasAllMandatoryParts

        #region Property: Parts
        /// <summary>
        /// Gets the required parts.
        /// </summary>
        public ReadOnlyCollection<PartCondition> Parts
        {
            get
            {
                return Database.Current.SelectAll<PartCondition>(this.ID, ValueTables.TangibleObjectConditionPartCondition, Columns.PartCondition).AsReadOnly();
            }
        }
        #endregion Property: Parts

        #region Property: HasAllMandatoryConnections
        /// <summary>
        /// Gets or sets the value that indicates whether all mandatory connections are required.
        /// </summary>
        public bool? HasAllMandatoryConnections
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, ValueTables.TangibleObjectCondition, Columns.HasAllMandatoryConnections);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.TangibleObjectCondition, Columns.HasAllMandatoryConnections, value);
                NotifyPropertyChanged("HasAllMandatoryConnections");
            }
        }
        #endregion Property: HasAllMandatoryConnections

        #region Property: Connections
        /// <summary>
        /// Gets the required connections.
        /// </summary>
        public ReadOnlyCollection<ConnectionItemCondition> Connections
        {
            get
            {
                return Database.Current.SelectAll<ConnectionItemCondition>(this.ID, ValueTables.TangibleObjectConditionConnectionItem, Columns.ConnectionItemCondition).AsReadOnly();
            }
        }
        #endregion Property: Connections

        #region Property: HasAllMandatoryMatter
        /// <summary>
        /// Gets or sets the value that indicates whether all mandatory matter is required.
        /// </summary>
        public bool? HasAllMandatoryMatter
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, ValueTables.TangibleObjectCondition, Columns.HasAllMandatoryMatter);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.TangibleObjectCondition, Columns.HasAllMandatoryMatter, value);
                NotifyPropertyChanged("HasAllMandatoryMatter");
            }
        }
        #endregion Property: HasAllMandatoryMatter

        #region Property: Matter
        /// <summary>
        /// Gets the required matter.
        /// </summary>
        public ReadOnlyCollection<MatterCondition> Matter
        {
            get
            {
                return Database.Current.SelectAll<MatterCondition>(this.ID, ValueTables.TangibleObjectConditionMatterCondition, Columns.MatterCondition).AsReadOnly();
            }
        }
        #endregion Property: Matter

        #region Property: HasAllMandatoryLayers
        /// <summary>
        /// Gets or sets the value that indicates whether all mandatory layers are required.
        /// </summary>
        public bool? HasAllMandatoryLayers
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, ValueTables.TangibleObjectCondition, Columns.HasAllMandatoryLayers);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.TangibleObjectCondition, Columns.HasAllMandatoryLayers, value);
                NotifyPropertyChanged("HasAllMandatoryLayers");
            }
        }
        #endregion Property: HasAllMandatoryLayers

        #region Property: Layers
        /// <summary>
        /// Gets the required layers.
        /// </summary>
        public ReadOnlyCollection<LayerCondition> Layers
        {
            get
            {
                return Database.Current.SelectAll<LayerCondition>(this.ID, ValueTables.TangibleObjectConditionLayerCondition, Columns.LayerCondition).AsReadOnly();
            }
        }
        #endregion Property: Layers

        #region Property: HasAllMandatoryCovers
        /// <summary>
        /// Gets or sets the value that indicates whether all mandatory covers are required.
        /// </summary>
        public bool? HasAllMandatoryCovers
        {
            get
            {
                return Database.Current.Select<bool?>(this.ID, ValueTables.TangibleObjectCondition, Columns.HasAllMandatoryCovers);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.TangibleObjectCondition, Columns.HasAllMandatoryCovers, value);
                NotifyPropertyChanged("HasAllMandatoryCovers");
            }
        }
        #endregion Property: HasAllMandatoryCovers

        #region Property: Covers
        /// <summary>
        /// Gets the required covers.
        /// </summary>
        public ReadOnlyCollection<CoverCondition> Covers
        {
            get
            {
                return Database.Current.SelectAll<CoverCondition>(this.ID, ValueTables.TangibleObjectConditionCoverCondition, Columns.CoverCondition).AsReadOnly();
            }
        }
        #endregion Property: Covers

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: TangibleObjectCondition()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static TangibleObjectCondition()
        {
            // Parts
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.PartCondition, new Tuple<Type, EntryType>(typeof(PartCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.TangibleObjectConditionPartCondition, typeof(TangibleObjectCondition), dict);

            // Connections
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.ConnectionItemCondition, new Tuple<Type, EntryType>(typeof(ConnectionItemCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.TangibleObjectConditionConnectionItem, typeof(TangibleObjectCondition), dict);

            // Matter
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.MatterCondition, new Tuple<Type, EntryType>(typeof(MatterCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.TangibleObjectConditionMatterCondition, typeof(TangibleObjectCondition), dict);

            // Layers
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.LayerCondition, new Tuple<Type, EntryType>(typeof(LayerCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.TangibleObjectConditionLayerCondition, typeof(TangibleObjectCondition), dict);
        }
        #endregion Static Constructor: TangibleObjectCondition()

        #region Constructor: TangibleObjectCondition()
        /// <summary>
        /// Creates a new tangible object condition.
        /// </summary>
        public TangibleObjectCondition()
            : base()
        {
        }
        #endregion Constructor: TangibleObjectCondition()

        #region Constructor: TangibleObjectCondition(uint id)
        /// <summary>
        /// Creates a new tangible object condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the tangible object condition from.</param>
        protected TangibleObjectCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: TangibleObjectCondition(uint id)

        #region Constructor: TangibleObjectCondition(TangibleObjectCondition tangibleObjectCondition)
        /// <summary>
        /// Clones a tangible object condition.
        /// </summary>
        /// <param name="tangibleObjectCondition">The tangible object condition to clone.</param>
        public TangibleObjectCondition(TangibleObjectCondition tangibleObjectCondition)
            : base(tangibleObjectCondition)
        {
            if (tangibleObjectCondition != null)
            {
                Database.Current.StartChange();

                this.HasAllMandatoryParts = tangibleObjectCondition.HasAllMandatoryParts;
                foreach (PartCondition part in tangibleObjectCondition.Parts)
                    AddPart(new PartCondition(part));
                this.HasAllMandatoryConnections = tangibleObjectCondition.HasAllMandatoryConnections;
                foreach (ConnectionItemCondition connectionItem in tangibleObjectCondition.Connections)
                    AddConnection(new ConnectionItemCondition(connectionItem));
                this.HasAllMandatoryMatter = tangibleObjectCondition.HasAllMandatoryMatter;
                foreach (MatterCondition matterCondition in tangibleObjectCondition.Matter)
                    AddMatter(matterCondition.Clone());
                this.HasAllMandatoryLayers = tangibleObjectCondition.HasAllMandatoryLayers;
                foreach (LayerCondition layerCondition in tangibleObjectCondition.Layers)
                    AddLayer(new LayerCondition(layerCondition));
                this.HasAllMandatoryCovers = tangibleObjectCondition.HasAllMandatoryCovers;
                foreach (CoverCondition cover in tangibleObjectCondition.Covers)
                    AddCover(new CoverCondition(cover));

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: TangibleObjectCondition(TangibleObjectCondition tangibleObjectCondition)

        #region Constructor: TangibleObjectCondition(TangibleObject tangibleObject)
        /// <summary>
        /// Creates a condition for the given tangible object.
        /// </summary>
        /// <param name="tangibleObject">The tangible object to create a condition for.</param>
        public TangibleObjectCondition(TangibleObject tangibleObject)
            : base(tangibleObject)
        {
        }
        #endregion Constructor: TangibleObjectCondition(TangibleObject tangibleObject)

        #region Constructor: TangibleObjectCondition(TangibleObject tangibleObject, NumericalValueCondition quantity)
        /// <summary>
        /// Creates a condition for the given tangible object in the given quantity.
        /// </summary>
        /// <param name="tangibleObject">The tangible object to create a condition for.</param>
        /// <param name="quantity">The quantity of the tangible object condition.</param>
        public TangibleObjectCondition(TangibleObject tangibleObject, NumericalValueCondition quantity)
            : base(tangibleObject, quantity)
        {
        }
        #endregion Constructor: TangibleObjectCondition(TangibleObject tangibleObject, NumericalValueCondition quantity)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddPart(PartCondition partCondition)
        /// <summary>
        /// Add a part.
        /// </summary>
        /// <param name="partCondition">The part to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddPart(PartCondition partCondition)
        {
            if (partCondition != null)
            {
                // Check whether the part is already there
                if (HasPart(partCondition.TangibleObject))
                    return Message.RelationExistsAlready;

                // Insert the part
                Database.Current.Insert(this.ID, ValueTables.TangibleObjectConditionPartCondition, Columns.PartCondition, partCondition);
                NotifyPropertyChanged("Parts");

                return Message.RelationSuccess;
            }

            return Message.RelationFail;
        }
        #endregion Method: AddPart(PartCondition partCondition)

        #region Method: RemovePart(PartCondition partCondition)
        /// <summary>
        /// Removes a part.
        /// </summary>
        /// <param name="partCondition">The part to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemovePart(PartCondition partCondition)
        {
            if (partCondition != null)
            {
                if (this.Parts.Contains(partCondition))
                {
                    // Remove the part
                    Database.Current.Remove(this.ID, ValueTables.TangibleObjectConditionPartCondition, Columns.PartCondition, partCondition);
                    NotifyPropertyChanged("Parts");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemovePart(PartCondition partCondition)

        #region Method: AddConnection(ConnectionItemCondition connectionItem)
        /// <summary>
        /// Adds a connection item.
        /// </summary>
        /// <param name="connectionItem">The connection item to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddConnection(ConnectionItemCondition connectionItem)
        {
            if (connectionItem != null)
            {
                // If the connection item is already present, there's no use to add it again
                if (HasConnectionItem(connectionItem.TangibleObject))
                    return Message.RelationExistsAlready;

                // Add the connection item
                Database.Current.Insert(this.ID, ValueTables.TangibleObjectConditionConnectionItem, Columns.ConnectionItemCondition, connectionItem);
                NotifyPropertyChanged("Connections");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddConnection(ConnectionItemCondition connectionItem)

        #region Method: RemoveConnection(ConnectionItemCondition connectionItem)
        /// <summary>
        /// Removes a connection item.
        /// </summary>
        /// <param name="connectionItem">The connection item to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveConnection(ConnectionItemCondition connectionItem)
        {
            if (connectionItem != null)
            {
                if (this.Connections.Contains(connectionItem))
                {
                    // Remove the connection item
                    Database.Current.Remove(this.ID, ValueTables.TangibleObjectConditionConnectionItem, Columns.ConnectionItemCondition, connectionItem);
                    NotifyPropertyChanged("Connections");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveConnection(ConnectionItemCondition connectionItem)

        #region Method: AddLayer(LayerCondition layerCondition)
        /// <summary>
        /// Adds a layer condition.
        /// </summary>
        /// <param name="layerCondition">The layer condition to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddLayer(LayerCondition layerCondition)
        {
            if (layerCondition != null)
            {
                // If the layer condition is already present, there's no use to add it again
                if (HasLayer(layerCondition.Matter))
                    return Message.RelationExistsAlready;

                // Add the layer condition
                Database.Current.Insert(this.ID, ValueTables.TangibleObjectConditionLayerCondition, Columns.LayerCondition, layerCondition);
                NotifyPropertyChanged("Layers");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddLayer(LayerCondition layerCondition)

        #region Method: RemoveLayer(LayerCondition layerCondition)
        /// <summary>
        /// Removes a layer condition.
        /// </summary>
        /// <param name="layerCondition">The layer condition to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveLayer(LayerCondition layerCondition)
        {
            if (layerCondition != null)
            {
                if (this.Layers.Contains(layerCondition))
                {
                    // Remove the layer condition
                    Database.Current.Remove(this.ID, ValueTables.TangibleObjectConditionLayerCondition, Columns.LayerCondition, layerCondition);
                    NotifyPropertyChanged("Layers");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveLayer(LayerCondition layerCondition)

        #region Method: AddMatter(MaterialCondition matterCondition)
        /// <summary>
        /// Adds a matter condition.
        /// </summary>
        /// <param name="matterCondition">The matter condition to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddMatter(MatterCondition matterCondition)
        {
            if (matterCondition != null)
            {
                // If the matter condition is already present, there's no use to add it again
                if (HasMatter(matterCondition.Matter))
                    return Message.RelationExistsAlready;

                // Add the matter condition
                Database.Current.Insert(this.ID, ValueTables.TangibleObjectConditionMatterCondition, Columns.MatterCondition, matterCondition);
                NotifyPropertyChanged("Matter");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddMatter(MaterialCondition matterCondition)

        #region Method: RemoveMatter(MaterialCondition matterCondition)
        /// <summary>
        /// Removes a matter condition.
        /// </summary>
        /// <param name="matterCondition">The matter condition to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveMatter(MatterCondition matterCondition)
        {
            if (matterCondition != null)
            {
                if (this.Matter.Contains(matterCondition))
                {
                    // Remove the matter condition
                    Database.Current.Remove(this.ID, ValueTables.TangibleObjectConditionMatterCondition, Columns.MatterCondition, matterCondition);
                    NotifyPropertyChanged("Matter");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveMatter(MaterialCondition matterCondition)

        #region Method: AddCover(CoverCondition coverCondition)
        /// <summary>
        /// Add a cover.
        /// </summary>
        /// <param name="coverCondition">The cover to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddCover(CoverCondition coverCondition)
        {
            if (coverCondition != null)
            {
                // Check whether the cover is already there
                if (HasCover(coverCondition.TangibleObject))
                    return Message.RelationExistsAlready;

                // Insert the cover
                Database.Current.Insert(this.ID, ValueTables.TangibleObjectConditionCoverCondition, Columns.CoverCondition, coverCondition);
                NotifyPropertyChanged("Covers");

                return Message.RelationSuccess;
            }

            return Message.RelationFail;
        }
        #endregion Method: AddCover(CoverCondition coverCondition)

        #region Method: RemoveCover(CoverCondition coverCondition)
        /// <summary>
        /// Removes a cover.
        /// </summary>
        /// <param name="coverCondition">The cover to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveCover(CoverCondition coverCondition)
        {
            if (coverCondition != null)
            {
                if (this.Covers.Contains(coverCondition))
                {
                    // Remove the cover
                    Database.Current.Remove(this.ID, ValueTables.TangibleObjectConditionCoverCondition, Columns.CoverCondition, coverCondition);
                    NotifyPropertyChanged("Covers");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveCover(CoverCondition coverCondition)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasPart(TangibleObject part)
        /// <summary>
        /// Checks if this tangible object condition has the given part.
        /// </summary>
        /// <param name="part">The part to check.</param>
        /// <returns>Returns true when the tangible object condition has the part.</returns>
        public bool HasPart(TangibleObject part)
        {
            if (part != null)
            {
                foreach (PartCondition myPart in this.Parts)
                {
                    if (part.Equals(myPart.TangibleObject))
                        return true;
                }
            }

            return false;
        }
        #endregion Method: HasPart(TangibleObject part)

        #region Method: HasConnectionItem(TangibleObject connectionItem)
        /// <summary>
        /// Checks if this tangible object condition has the given connection item.
        /// </summary>
        /// <param name="connectionItem">The connection item to check.</param>
        /// <returns>Returns true when this tangible object condition has the connection item.</returns>
        public bool HasConnectionItem(TangibleObject connectionItem)
        {
            if (connectionItem != null)
            {
                foreach (ConnectionItemCondition myConnectionItem in this.Connections)
                {
                    if (connectionItem.Equals(myConnectionItem.TangibleObject))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasConnectionItem(TangibleObject connectionItem)

        #region Method: HasMatter(Material matter)
        /// <summary>
        /// Checks if this tangible object condition has the given matter.
        /// </summary>
        /// <param name="matter">The matter to check.</param>
        /// <returns>Returns true when this tangible object condition has the matter.</returns>
        public bool HasMatter(Matter matter)
        {
            if (matter != null)
            {
                foreach (MatterCondition matterCondition in this.Matter)
                {
                    if (matter.Equals(matterCondition.Matter))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasMatter(Material matter)

        #region Method: HasLayer(Matter matter)
        /// <summary>
        /// Checks if this tangible object condition has the given matter as a layer.
        /// </summary>
        /// <param name="matter">The material to check.</param>
        /// <returns>Returns true when this tangible object condition has the matter as a layer.</returns>
        public bool HasLayer(Matter matter)
        {
            if (matter != null)
            {
                foreach (MatterCondition matterCondition in this.Layers)
                {
                    if (matter.Equals(matterCondition.Matter))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasLayer(Matter matter)

        #region Method: HasCover(TangibleObject cover)
        /// <summary>
        /// Checks if this tangible object condition has the given cover.
        /// </summary>
        /// <param name="cover">The cover to check.</param>
        /// <returns>Returns true when the tangible object condition has the cover.</returns>
        public bool HasCover(TangibleObject cover)
        {
            if (cover != null)
            {
                foreach (CoverCondition myCover in this.Covers)
                {
                    if (cover.Equals(myCover.TangibleObject))
                        return true;
                }
            }

            return false;
        }
        #endregion Method: HasCover(TangibleObject cover)

        #region Method: Remove()
        /// <summary>
        /// Remove the tangible object condition.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the parts
            foreach (PartCondition partCondition in this.Parts)
                partCondition.Remove();
            Database.Current.Remove(this.ID, ValueTables.TangibleObjectConditionPartCondition);

            // Remove the connections
            foreach (ConnectionItemCondition connectionItemCondition in this.Connections)
                connectionItemCondition.Remove();
            Database.Current.Remove(this.ID, ValueTables.TangibleObjectConditionConnectionItem);

            // Remove the matter
            foreach (MatterCondition materialCondition in this.Matter)
                materialCondition.Remove();
            Database.Current.Remove(this.ID, ValueTables.TangibleObjectConditionMatterCondition);

            // Remove the layers
            foreach (LayerCondition layerCondition in this.Layers)
                layerCondition.Remove();
            Database.Current.Remove(this.ID, ValueTables.TangibleObjectConditionLayerCondition);

            // Remove the covers
            foreach (CoverCondition coverCondition in this.Covers)
                coverCondition.Remove();
            Database.Current.Remove(this.ID, ValueTables.TangibleObjectConditionCoverCondition);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: TangibleObjectCondition

    #region Class: TangibleObjectChange
    /// <summary>
    /// A change on a tangible object.
    /// </summary>
    public class TangibleObjectChange : PhysicalObjectChange
    {

        #region Properties and Fields

        #region Property: TangibleObject
        /// <summary>
        /// Gets or sets the affected tangible object.
        /// </summary>
        public TangibleObject TangibleObject
        {
            get
            {
                return this.Node as TangibleObject;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: TangibleObject

        #region Property: Parts
        /// <summary>
        /// Gets the parts to change.
        /// </summary>
        public ReadOnlyCollection<PartChange> Parts
        {
            get
            {
                return Database.Current.SelectAll<PartChange>(this.ID, ValueTables.TangibleObjectChangePartChange, Columns.PartChange).AsReadOnly();
            }
        }
        #endregion Property: Parts

        #region Property: PartsToAdd
        /// <summary>
        /// Gets the parts that should be added during the change.
        /// </summary>
        public ReadOnlyCollection<Part> PartsToAdd
        {
            get
            {
                return Database.Current.SelectAll<Part>(this.ID, ValueTables.TangibleObjectChangePartToAdd, Columns.Part).AsReadOnly();
            }
        }
        #endregion Property: PartsToAdd

        #region Property: PartsToRemove
        /// <summary>
        /// Gets the parts that should be removed during the change.
        /// </summary>
        public ReadOnlyCollection<PartCondition> PartsToRemove
        {
            get
            {
                return Database.Current.SelectAll<PartCondition>(this.ID, ValueTables.TangibleObjectChangePartToRemove, Columns.PartCondition).AsReadOnly();
            }
        }
        #endregion Property: PartsToRemove

        #region Property: Connections
        /// <summary>
        /// Gets the connections to change.
        /// </summary>
        public ReadOnlyCollection<ConnectionItemChange> Connections
        {
            get
            {
                return Database.Current.SelectAll<ConnectionItemChange>(this.ID, ValueTables.TangibleObjectChangeConnectionChange, Columns.ConnectionItemChange).AsReadOnly();
            }
        }
        #endregion Property: Connections

        #region Property: ConnectionsToAdd
        /// <summary>
        /// Gets the connections to add.
        /// </summary>
        public ReadOnlyCollection<ConnectionItem> ConnectionsToAdd
        {
            get
            {
                return Database.Current.SelectAll<ConnectionItem>(this.ID, ValueTables.TangibleObjectChangeConnectionToAdd, Columns.ConnectionItem).AsReadOnly();
            }
        }
        #endregion Property: ConnectionsToAdd

        #region Property: ConnectionsToRemove
        /// <summary>
        /// Gets the connections to remove.
        /// </summary>
        public ReadOnlyCollection<ConnectionItemCondition> ConnectionsToRemove
        {
            get
            {
                return Database.Current.SelectAll<ConnectionItemCondition>(this.ID, ValueTables.TangibleObjectChangeConnectionToRemove, Columns.ConnectionItemCondition).AsReadOnly();
            }
        }
        #endregion Property: ConnectionsToRemove

        #region Property: Matter
        /// <summary>
        /// Gets the matter to change.
        /// </summary>
        public ReadOnlyCollection<MatterChange> Matter
        {
            get
            {
                return Database.Current.SelectAll<MatterChange>(this.ID, ValueTables.TangibleObjectChangeMatterChange, Columns.MatterChange).AsReadOnly();
            }
        }
        #endregion Property: Matter

        #region Property: MatterToAdd
        /// <summary>
        /// Gets the matter to add.
        /// </summary>
        public ReadOnlyCollection<MatterValued> MatterToAdd
        {
            get
            {
                return Database.Current.SelectAll<MatterValued>(this.ID, ValueTables.TangibleObjectChangeMatterToAdd, Columns.MatterValued).AsReadOnly();
            }
        }
        #endregion Property: MatterToAdd

        #region Property: MatterToRemove
        /// <summary>
        /// Gets the matter to remove.
        /// </summary>
        public ReadOnlyCollection<MatterCondition> MatterToRemove
        {
            get
            {
                return Database.Current.SelectAll<MatterCondition>(this.ID, ValueTables.TangibleObjectChangeMatterToRemove, Columns.MatterCondition).AsReadOnly();
            }
        }
        #endregion Property: MatterToRemove

        #region Property: Layers
        /// <summary>
        /// Gets the layers to change.
        /// </summary>
        public ReadOnlyCollection<LayerChange> Layers
        {
            get
            {
                return Database.Current.SelectAll<LayerChange>(this.ID, ValueTables.TangibleObjectChangeLayerChange, Columns.LayerChange).AsReadOnly();
            }
        }
        #endregion Property: Layers

        #region Property: LayersToAdd
        /// <summary>
        /// Gets the layers to add.
        /// </summary>
        public ReadOnlyCollection<Layer> LayersToAdd
        {
            get
            {
                return Database.Current.SelectAll<Layer>(this.ID, ValueTables.TangibleObjectChangeLayerToAdd, Columns.Layer).AsReadOnly();
            }
        }
        #endregion Property: LayersToAdd

        #region Property: LayersToRemove
        /// <summary>
        /// Gets the layers to remove.
        /// </summary>
        public ReadOnlyCollection<LayerCondition> LayersToRemove
        {
            get
            {
                return Database.Current.SelectAll<LayerCondition>(this.ID, ValueTables.TangibleObjectChangeLayerToRemove, Columns.LayerCondition).AsReadOnly();
            }
        }
        #endregion Property: LayersToRemove

        #region Property: Covers
        /// <summary>
        /// Gets the covers to change.
        /// </summary>
        public ReadOnlyCollection<CoverChange> Covers
        {
            get
            {
                return Database.Current.SelectAll<CoverChange>(this.ID, ValueTables.TangibleObjectChangeCoverChange, Columns.CoverChange).AsReadOnly();
            }
        }
        #endregion Property: Covers

        #region Property: CoversToAdd
        /// <summary>
        /// Gets the covers that should be added during the change.
        /// </summary>
        public ReadOnlyCollection<Cover> CoversToAdd
        {
            get
            {
                return Database.Current.SelectAll<Cover>(this.ID, ValueTables.TangibleObjectChangeCoverToAdd, Columns.Cover).AsReadOnly();
            }
        }
        #endregion Property: CoversToAdd

        #region Property: CoversToRemove
        /// <summary>
        /// Gets the covers that should be removed during the change.
        /// </summary>
        public ReadOnlyCollection<CoverCondition> CoversToRemove
        {
            get
            {
                return Database.Current.SelectAll<CoverCondition>(this.ID, ValueTables.TangibleObjectChangeCoverToRemove, Columns.CoverCondition).AsReadOnly();
            }
        }
        #endregion Property: CoversToRemove

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: TangibleObjectChange()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static TangibleObjectChange()
        {
            // Parts
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.PartChange, new Tuple<Type, EntryType>(typeof(PartChange), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.TangibleObjectChangePartChange, typeof(TangibleObjectChange), dict);

            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Part, new Tuple<Type, EntryType>(typeof(Part), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.TangibleObjectChangePartToAdd, typeof(TangibleObjectChange), dict);

            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.PartCondition, new Tuple<Type, EntryType>(typeof(PartCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.TangibleObjectChangePartToRemove, typeof(TangibleObjectChange), dict);

            // Connections
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.ConnectionItemChange, new Tuple<Type, EntryType>(typeof(ConnectionItemChange), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.TangibleObjectChangeConnectionChange, typeof(TangibleObjectChange), dict);

            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.ConnectionItem, new Tuple<Type, EntryType>(typeof(ConnectionItem), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.TangibleObjectChangeConnectionToAdd, typeof(TangibleObjectChange), dict);

            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.ConnectionItemCondition, new Tuple<Type, EntryType>(typeof(ConnectionItemCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.TangibleObjectChangeConnectionToRemove, typeof(TangibleObjectChange), dict);

            // Matter
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.MatterChange, new Tuple<Type, EntryType>(typeof(MatterChange), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.TangibleObjectChangeMatterChange, typeof(TangibleObjectChange), dict);

            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.MatterValued, new Tuple<Type, EntryType>(typeof(MatterValued), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.TangibleObjectChangeMatterToAdd, typeof(TangibleObjectChange), dict);

            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.MatterCondition, new Tuple<Type, EntryType>(typeof(MatterCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.TangibleObjectChangeMatterToRemove, typeof(TangibleObjectChange), dict);

            // Layers
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.LayerChange, new Tuple<Type, EntryType>(typeof(LayerChange), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.TangibleObjectChangeLayerChange, typeof(TangibleObjectChange), dict);

            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Layer, new Tuple<Type, EntryType>(typeof(Layer), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.TangibleObjectChangeLayerToAdd, typeof(TangibleObjectChange), dict);

            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.LayerCondition, new Tuple<Type, EntryType>(typeof(LayerCondition), EntryType.Unique));
            Database.Current.AddTableDefinition(ValueTables.TangibleObjectChangeLayerToRemove, typeof(TangibleObjectChange), dict);
        }
        #endregion Static Constructor: TangibleObjectChange()

        #region Constructor: TangibleObjectChange()
        /// <summary>
        /// Creates a new tangible object change.
        /// </summary>
        public TangibleObjectChange()
            : base()
        {
        }
        #endregion Constructor: TangibleObjectChange()

        #region Constructor: TangibleObjectChange(uint id)
        /// <summary>
        /// Creates a new tangible object change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a tangible object change from.</param>
        protected TangibleObjectChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: TangibleObjectChange(uint id)

        #region Constructor: TangibleObjectChange(TangibleObjectChange tangibleObjectChange)
        /// <summary>
        /// Clones a tangible object change.
        /// </summary>
        /// <param name="tangibleObjectChange">The tangible object change to clone.</param>
        public TangibleObjectChange(TangibleObjectChange tangibleObjectChange)
            : base(tangibleObjectChange)
        {
            if (tangibleObjectChange != null)
            {
                Database.Current.StartChange();

                foreach (PartChange partChange in tangibleObjectChange.Parts)
                    AddPart(new PartChange(partChange));
                foreach (Part part in tangibleObjectChange.PartsToAdd)
                    AddPartToAdd(new Part(part));
                foreach (PartCondition partCondition in tangibleObjectChange.PartsToRemove)
                    AddPartToRemove(new PartCondition(partCondition));
                foreach (ConnectionItemChange connection in tangibleObjectChange.Connections)
                    AddConnection(new ConnectionItemChange(connection));
                foreach (ConnectionItem connection in tangibleObjectChange.ConnectionsToAdd)
                    AddConnectionToAdd(new ConnectionItem(connection));
                foreach (ConnectionItemCondition connection in tangibleObjectChange.ConnectionsToRemove)
                    AddConnectionToRemove(new ConnectionItemCondition(connection));
                foreach (MatterChange matterChange in tangibleObjectChange.Matter)
                    AddMatter(matterChange.Clone());
                foreach (MatterValued matterValued in tangibleObjectChange.MatterToAdd)
                    AddMatterToAdd(matterValued.Clone());
                foreach (MatterCondition matterCondition in tangibleObjectChange.MatterToRemove)
                    AddMatterToRemove(matterCondition.Clone());
                foreach (LayerChange layerChange in tangibleObjectChange.Layers)
                    AddLayer(new LayerChange(layerChange));
                foreach (Layer layer in tangibleObjectChange.LayersToAdd)
                    AddLayerToAdd(new Layer(layer));
                foreach (LayerCondition layerCondition in tangibleObjectChange.LayersToRemove)
                    AddLayerToRemove(new LayerCondition(layerCondition));
                foreach (CoverChange coverChange in tangibleObjectChange.Covers)
                    AddCover(new CoverChange(coverChange));
                foreach (Cover cover in tangibleObjectChange.CoversToAdd)
                    AddCoverToAdd(new Cover(cover));
                foreach (CoverCondition coverCondition in tangibleObjectChange.CoversToRemove)
                    AddCoverToRemove(new CoverCondition(coverCondition));

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: TangibleObjectChange(TangibleObjectChange tangibleObjectChange)

        #region Constructor: TangibleObjectChange(TangibleObject tangibleObject)
        /// <summary>
        /// Creates a change for the given tangible object.
        /// </summary>
        /// <param name="tangibleObject">The tangible object to create a change for.</param>
        public TangibleObjectChange(TangibleObject tangibleObject)
            : base(tangibleObject)
        {
        }
        #endregion Constructor: TangibleObjectChange(TangibleObject tangibleObject)

        #region Constructor: TangibleObjectChange(TangibleObject tangibleObject, NumericalValueChange quantity)
        /// <summary>
        /// Creates a change for the given tangible object in the form of the given quantity.
        /// </summary>
        /// <param name="tangibleObject">The tangible object to create a change for.</param>
        /// <param name="quantity">The change in quantity.</param>
        public TangibleObjectChange(TangibleObject tangibleObject, NumericalValueChange quantity)
            : base(tangibleObject, quantity)
        {
        }
        #endregion Constructor: TangibleObjectChange(TangibleObject tangibleObject, NumericalValueChange quantity)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddPart(PartChange partChange)
        /// <summary>
        /// Adds a part to the list of parts.
        /// </summary>
        /// <param name="partChange">The part to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddPart(PartChange partChange)
        {
            if (partChange != null)
            {
                // If the part is already available in all parts, there is no use to add it
                if (HasPart(partChange.TangibleObject))
                    return Message.RelationExistsAlready;

                // Add the part
                Database.Current.Insert(this.ID, ValueTables.TangibleObjectChangePartChange, Columns.PartChange, partChange);
                NotifyPropertyChanged("Parts");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddPart(PartChange partChange)

        #region Method: RemovePart(PartChange partChange)
        /// <summary>
        /// Removes a part from the list of parts.
        /// </summary>
        /// <param name="partChange">The part to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemovePart(PartChange partChange)
        {
            if (partChange != null)
            {
                if (this.Parts.Contains(partChange))
                {
                    // Remove the part
                    Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangePartChange, Columns.PartChange, partChange);
                    NotifyPropertyChanged("Parts");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemovePart(PartChange partChange)

        #region Method: AddPartToAdd(Part part)
        /// <summary>
        /// Adds a part to the list of parts to add.
        /// </summary>
        /// <param name="part">The part to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddPartToAdd(Part part)
        {
            if (part != null && part.TangibleObject != null)
            {
                // If the part is already available in all parts, there is no use to add it
                if (HasPartToAdd(part.TangibleObject))
                    return Message.RelationExistsAlready;

                // Add the part
                Database.Current.Insert(this.ID, ValueTables.TangibleObjectChangePartToAdd, Columns.Part, part);
                NotifyPropertyChanged("PartsToAdd");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddPartToAdd(Part part)

        #region Method: RemovePartToAdd(Part part)
        /// <summary>
        /// Removes a part from the list of parts to add.
        /// </summary>
        /// <param name="part">The part to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemovePartToAdd(Part part)
        {
            if (part != null)
            {
                if (this.PartsToAdd.Contains(part))
                {
                    // Remove the part
                    Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangePartToAdd, Columns.Part, part);
                    NotifyPropertyChanged("PartsToAdd");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemovePartToAdd(Part part)

        #region Method: AddPartToRemove(PartCondition partCondition)
        /// <summary>
        /// Adds a part to the list of parts to remove.
        /// </summary>
        /// <param name="partCondition">The part to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddPartToRemove(PartCondition partCondition)
        {
            if (partCondition != null)
            {
                // If the part is already available in all parts, there is no use to add it
                if (HasPartToRemove(partCondition.TangibleObject))
                    return Message.RelationExistsAlready;

                // Add the part
                Database.Current.Insert(this.ID, ValueTables.TangibleObjectChangePartToRemove, Columns.PartCondition, partCondition);
                NotifyPropertyChanged("PartsToRemove");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddPartToRemove(PartCondition partCondition)

        #region Method: RemovePartToRemove(PartCondition partCondition)
        /// <summary>
        /// Removes a part from the list of parts to remove.
        /// </summary>
        /// <param name="partCondition">The part to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemovePartToRemove(PartCondition partCondition)
        {
            if (partCondition != null)
            {
                if (this.PartsToRemove.Contains(partCondition))
                {
                    // Remove the part
                    Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangePartToRemove, Columns.PartCondition, partCondition);
                    NotifyPropertyChanged("PartsToRemove");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemovePartToRemove(PartCondition partCondition)

        #region Method: AddConnection(ConnectionItemChange connectionChange)
        /// <summary>
        /// Adds a connection change.
        /// </summary>
        /// <param name="connectionChange">The connection change to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddConnection(ConnectionItemChange connectionChange)
        {
            if (connectionChange != null)
            {
                // If the connection is already present, there's no use to add it again
                if (HasConnection(connectionChange.TangibleObject))
                    return Message.RelationExistsAlready;

                // Add the connection change
                Database.Current.Insert(this.ID, ValueTables.TangibleObjectChangeConnectionChange, Columns.ConnectionItemChange, connectionChange);
                NotifyPropertyChanged("Connections");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddConnection(ConnectionItemChange connectionChange)

        #region Method: RemoveConnection(ConnectionItemChange connectionChange)
        /// <summary>
        /// Removes a connection change.
        /// </summary>
        /// <param name="connectionChange">The connection change to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveConnection(ConnectionItemChange connectionChange)
        {
            if (connectionChange != null)
            {
                if (this.Connections.Contains(connectionChange))
                {
                    // Remove the connection change
                    Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangeConnectionChange, Columns.ConnectionItemChange, connectionChange);
                    NotifyPropertyChanged("Connections");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveConnection(ConnectionItemChange connectionChange)

        #region Method: AddConnectionToAdd(ConnectionItem connection)
        /// <summary>
        /// Adds a connection to add.
        /// </summary>
        /// <param name="connection">The connection to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddConnectionToAdd(ConnectionItem connection)
        {
            if (connection != null && connection.TangibleObject != null)
            {
                // If the connection is already present, there's no use to add it again
                if (HasConnectionToAdd(connection.TangibleObject))
                    return Message.RelationExistsAlready;

                // Add the connection
                Database.Current.Insert(this.ID, ValueTables.TangibleObjectChangeConnectionToAdd, Columns.ConnectionItem, connection);
                NotifyPropertyChanged("ConnectionsToAdd");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddConnectionToAdd(ConnectionItem connection)

        #region Method: RemoveConnectionToAdd(ConnectionItem connection)
        /// <summary>
        /// Removes a connection to add.
        /// </summary>
        /// <param name="connection">The connection to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveConnectionToAdd(ConnectionItem connection)
        {
            if (connection != null)
            {
                if (this.ConnectionsToAdd.Contains(connection))
                {
                    // Remove the connection
                    Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangeConnectionToAdd, Columns.ConnectionItem, connection);
                    NotifyPropertyChanged("ConnectionsToAdd");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveConnectionToAdd(ConnectionItem connection)

        #region Method: AddConnectionToRemove(ConnectionItemCondition connection)
        /// <summary>
        /// Adds a connection to remove.
        /// </summary>
        /// <param name="connection">The connection to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddConnectionToRemove(ConnectionItemCondition connection)
        {
            if (connection != null)
            {
                // If the connection is already present, there's no use to add it again
                if (HasConnectionToRemove(connection.TangibleObject))
                    return Message.RelationExistsAlready;

                // Add the connection
                Database.Current.Insert(this.ID, ValueTables.TangibleObjectChangeConnectionToRemove, Columns.ConnectionItemCondition, connection);
                NotifyPropertyChanged("ConnectionsToRemove");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddConnectionToRemove(ConnectionItemCondition connection)

        #region Method: RemoveConnectionToRemove(ConnectionItemCondition connection)
        /// <summary>
        /// Removes a connection to remove.
        /// </summary>
        /// <param name="connection">The connection to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveConnectionToRemove(ConnectionItemCondition connection)
        {
            if (connection != null)
            {
                if (this.ConnectionsToRemove.Contains(connection))
                {
                    // Remove the connection
                    Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangeConnectionToRemove, Columns.ConnectionItemCondition, connection);
                    NotifyPropertyChanged("ConnectionsToRemove");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveConnectionToRemove(ConnectionItemCondition connection)

        #region Method: AddMatter(MatterChange matterChange)
        /// <summary>
        /// Adds a matter change.
        /// </summary>
        /// <param name="matterChange">The matter change to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddMatter(MatterChange matterChange)
        {
            if (matterChange != null)
            {
                // If the matter is already present, there's no use to add it again
                if (HasMatter(matterChange.Matter))
                    return Message.RelationExistsAlready;

                // Add the matter change
                Database.Current.Insert(this.ID, ValueTables.TangibleObjectChangeMatterChange, Columns.MatterChange, matterChange);
                NotifyPropertyChanged("Matter");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddMatter(MatterChange matterChange)

        #region Method: RemoveMatter(MatterChange matterChange)
        /// <summary>
        /// Removes a matter change.
        /// </summary>
        /// <param name="matterChange">The matter change to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveMatter(MatterChange matterChange)
        {
            if (matterChange != null)
            {
                if (this.Matter.Contains(matterChange))
                {
                    // Remove the matter change
                    Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangeMatterChange, Columns.MatterChange, matterChange);
                    NotifyPropertyChanged("Matter");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveMatter(MatterChange matterChange)

        #region Method: AddMatterToAdd(MatterValued matterValued)
        /// <summary>
        /// Adds a matter to add.
        /// </summary>
        /// <param name="matterValued">The matter to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddMatterToAdd(MatterValued matterValued)
        {
            if (matterValued != null && matterValued.Matter != null)
            {
                // If the matter is already present, there's no use to add it again
                if (HasMatterToAdd(matterValued.Matter))
                    return Message.RelationExistsAlready;

                // Add the matter
                Database.Current.Insert(this.ID, ValueTables.TangibleObjectChangeMatterToAdd, Columns.MatterValued, matterValued);
                NotifyPropertyChanged("MatterToAdd");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddMatterToAdd(MatterValued matterValued)

        #region Method: RemoveMatterToAdd(MatterValued matterValued)
        /// <summary>
        /// Removes a matter to add.
        /// </summary>
        /// <param name="matterValued">The matter to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveMatterToAdd(MatterValued matterValued)
        {
            if (matterValued != null)
            {
                if (this.MatterToAdd.Contains(matterValued))
                {
                    // Remove the matter
                    Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangeMatterToAdd, Columns.MatterValued, matterValued);
                    NotifyPropertyChanged("MatterToAdd");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveMatterToAdd(MatterValued matterValued)

        #region Method: AddMatterToRemove(MatterCondition matterCondition)
        /// <summary>
        /// Adds a matter to remove.
        /// </summary>
        /// <param name="matterCondition">The matter to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddMatterToRemove(MatterCondition matterCondition)
        {
            if (matterCondition != null)
            {
                // If the matter is already present, there's no use to add it again
                if (HasMatterToRemove(matterCondition.Matter))
                    return Message.RelationExistsAlready;

                // Add the matter
                Database.Current.Insert(this.ID, ValueTables.TangibleObjectChangeMatterToRemove, Columns.MatterCondition, matterCondition);
                NotifyPropertyChanged("MatterToRemove");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddMatterToRemove(MatterCondition matterCondition)

        #region Method: RemoveMatterToRemove(MatterCondition matterCondition)
        /// <summary>
        /// Removes a matter to remove.
        /// </summary>
        /// <param name="matterCondition">The matter to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveMatterToRemove(MatterCondition matterCondition)
        {
            if (matterCondition != null)
            {
                if (this.MatterToRemove.Contains(matterCondition))
                {
                    // Remove the matter
                    Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangeMatterToRemove, Columns.MatterCondition, matterCondition);
                    NotifyPropertyChanged("MatterToRemove");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveMatterToRemove(MatterCondition matterCondition)

        #region Method: AddLayer(LayerChange layerChange)
        /// <summary>
        /// Adds a layer change.
        /// </summary>
        /// <param name="layerChange">The layer change to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddLayer(LayerChange layerChange)
        {
            if (layerChange != null)
            {
                // If the layer is already present, there's no use to add it again
                if (HasLayer(layerChange.Matter))
                    return Message.RelationExistsAlready;

                // Add the layer change
                Database.Current.Insert(this.ID, ValueTables.TangibleObjectChangeLayerChange, Columns.LayerChange, layerChange);
                NotifyPropertyChanged("Layers");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddLayer(LayerChange layerChange)

        #region Method: RemoveLayer(LayerChange layerChange)
        /// <summary>
        /// Removes a layer change.
        /// </summary>
        /// <param name="layerChange">The layer change to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveLayer(LayerChange layerChange)
        {
            if (layerChange != null)
            {
                if (this.Layers.Contains(layerChange))
                {
                    // Remove the layer change
                    Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangeLayerChange, Columns.LayerChange, layerChange);
                    NotifyPropertyChanged("Layers");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveLayer(LayerChange layerChange)

        #region Method: AddLayerToAdd(Layer layer)
        /// <summary>
        /// Adds a layer to add.
        /// </summary>
        /// <param name="layer">The layer to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddLayerToAdd(Layer layer)
        {
            if (layer != null && layer.Matter != null)
            {
                // If the layer is already present, there's no use to add it again
                if (HasLayerToAdd(layer.Matter))
                    return Message.RelationExistsAlready;

                // Add the layer
                Database.Current.Insert(this.ID, ValueTables.TangibleObjectChangeLayerToAdd, Columns.Layer, layer);
                NotifyPropertyChanged("LayersToAdd");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddLayerToAdd(Layer layer)

        #region Method: RemoveLayerToAdd(Layer layer)
        /// <summary>
        /// Removes a layer to add.
        /// </summary>
        /// <param name="layer">The layer to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveLayerToAdd(Layer layer)
        {
            if (layer != null)
            {
                if (this.LayersToAdd.Contains(layer))
                {
                    // Remove the layer
                    Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangeLayerToAdd, Columns.Layer, layer);
                    NotifyPropertyChanged("LayersToAdd");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveLayerToAdd(Layer layer)

        #region Method: AddLayerToRemove(LayerCondition layerCondition)
        /// <summary>
        /// Adds a layer to remove.
        /// </summary>
        /// <param name="layerCondition">The layer to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddLayerToRemove(LayerCondition layerCondition)
        {
            if (layerCondition != null)
            {
                // If the layer is already present, there's no use to add it again
                if (HasLayerToRemove(layerCondition.Matter))
                    return Message.RelationExistsAlready;

                // Add the layer
                Database.Current.Insert(this.ID, ValueTables.TangibleObjectChangeLayerToRemove, Columns.LayerCondition, layerCondition);
                NotifyPropertyChanged("LayersToRemove");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddLayerToRemove(LayerCondition layerCondition)

        #region Method: RemoveLayerToRemove(LayerCondition layerCondition)
        /// <summary>
        /// Removes a layer to remove.
        /// </summary>
        /// <param name="layerCondition">The layer to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveLayerToRemove(LayerCondition layerCondition)
        {
            if (layerCondition != null)
            {
                if (this.LayersToRemove.Contains(layerCondition))
                {
                    // Remove the layer
                    Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangeLayerToRemove, Columns.LayerCondition, layerCondition);
                    NotifyPropertyChanged("LayersToRemove");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveLayerToRemove(LayerCondition layerCondition)

        #region Method: AddCover(CoverChange coverChange)
        /// <summary>
        /// Adds a cover to the list of covers.
        /// </summary>
        /// <param name="coverChange">The cover to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddCover(CoverChange coverChange)
        {
            if (coverChange != null)
            {
                // If the cover is already available in all covers, there is no use to add it
                if (HasCover(coverChange.TangibleObject))
                    return Message.RelationExistsAlready;

                // Add the cover
                Database.Current.Insert(this.ID, ValueTables.TangibleObjectChangeCoverChange, Columns.CoverChange, coverChange);
                NotifyPropertyChanged("Covers");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddCover(CoverChange coverChange)

        #region Method: RemoveCover(CoverChange coverChange)
        /// <summary>
        /// Removes a cover from the list of covers.
        /// </summary>
        /// <param name="coverChange">The cover to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveCover(CoverChange coverChange)
        {
            if (coverChange != null)
            {
                if (this.Covers.Contains(coverChange))
                {
                    // Remove the cover
                    Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangeCoverChange, Columns.CoverChange, coverChange);
                    NotifyPropertyChanged("Covers");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveCover(CoverChange coverChange)

        #region Method: AddCoverToAdd(Cover cover)
        /// <summary>
        /// Adds a cover to the list of covers to add.
        /// </summary>
        /// <param name="cover">The cover to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddCoverToAdd(Cover cover)
        {
            if (cover != null && cover.TangibleObject != null)
            {
                // If the cover is already available in all covers, there is no use to add it
                if (HasCoverToAdd(cover.TangibleObject))
                    return Message.RelationExistsAlready;

                // Add the cover
                Database.Current.Insert(this.ID, ValueTables.TangibleObjectChangeCoverToAdd, Columns.Cover, cover);
                NotifyPropertyChanged("CoversToAdd");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddCoverToAdd(Cover cover)

        #region Method: RemoveCoverToAdd(Cover cover)
        /// <summary>
        /// Removes a cover from the list of covers to add.
        /// </summary>
        /// <param name="cover">The cover to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveCoverToAdd(Cover cover)
        {
            if (cover != null)
            {
                if (this.CoversToAdd.Contains(cover))
                {
                    // Remove the cover
                    Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangeCoverToAdd, Columns.Cover, cover);
                    NotifyPropertyChanged("CoversToAdd");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveCoverToAdd(Cover cover)

        #region Method: AddCoverToRemove(CoverCondition coverCondition)
        /// <summary>
        /// Adds a cover to the list of covers to remove.
        /// </summary>
        /// <param name="coverCondition">The cover to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddCoverToRemove(CoverCondition coverCondition)
        {
            if (coverCondition != null)
            {
                // If the cover is already available in all covers, there is no use to add it
                if (HasCoverToRemove(coverCondition.TangibleObject))
                    return Message.RelationExistsAlready;

                // Add the cover
                Database.Current.Insert(this.ID, ValueTables.TangibleObjectChangeCoverToRemove, Columns.CoverCondition, coverCondition);
                NotifyPropertyChanged("CoversToRemove");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddCoverToRemove(CoverCondition coverCondition)

        #region Method: RemoveCoverToRemove(CoverCondition coverCondition)
        /// <summary>
        /// Removes a cover from the list of covers to remove.
        /// </summary>
        /// <param name="coverCondition">The cover to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveCoverToRemove(CoverCondition coverCondition)
        {
            if (coverCondition != null)
            {
                if (this.CoversToRemove.Contains(coverCondition))
                {
                    // Remove the cover
                    Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangeCoverToRemove, Columns.CoverCondition, coverCondition);
                    NotifyPropertyChanged("CoversToRemove");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveCoverToRemove(CoverCondition coverCondition)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasPart(TangibleObject part)
        /// <summary>
        /// Checks whether the tangible object change has the given part.
        /// </summary>
        /// <param name="part">The part that should be checked.</param>
        /// <returns>Returns true when the tangible object change has the given part.</returns>
        public bool HasPart(TangibleObject part)
        {
            if (part != null)
            {
                foreach (PartChange myPart in this.Parts)
                {
                    if (part.Equals(myPart.TangibleObject))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasPart(TangibleObject part)

        #region Method: HasPartToAdd(TangibleObject part)
        /// <summary>
        /// Checks whether the tangible object change has the given part to add.
        /// </summary>
        /// <param name="part">The part that should be checked.</param>
        /// <returns>Returns true when the tangible object change has the given part to add.</returns>
        public bool HasPartToAdd(TangibleObject part)
        {
            if (part != null)
            {
                foreach (Part myPart in this.PartsToAdd)
                {
                    if (part.Equals(myPart.TangibleObject))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasPartToAdd(TangibleObject part)

        #region Method: HasPartToRemove(TangibleObject part)
        /// <summary>
        /// Checks whether the tangible object change has the given part to remove.
        /// </summary>
        /// <param name="part">The part that should be checked.</param>
        /// <returns>Returns true when the tangible object change has the given part to remove.</returns>
        public bool HasPartToRemove(TangibleObject part)
        {
            if (part != null)
            {
                foreach (PartCondition myPart in this.PartsToRemove)
                {
                    if (part.Equals(myPart.TangibleObject))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasPartToRemove(TangibleObject part)

        #region Method: HasConnection(TangibleObject tangibleObject)
        /// <summary>
        /// Checks if this tangible object change has the given tangible object as a connection.
        /// </summary>
        /// <param name="tangibleObject">The tangible object to check.</param>
        /// <returns>Returns true when this tangible object change has the tangible object as a connection.</returns>
        public bool HasConnection(TangibleObject tangibleObject)
        {
            if (tangibleObject != null)
            {
                foreach (ConnectionItemChange connectionItemChange in this.Connections)
                {
                    if (tangibleObject.Equals(connectionItemChange.TangibleObject))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasConnection(TangibleObject tangibleObject)

        #region Method: HasConnectionToAdd(TangibleObject tangibleObject)
        /// <summary>
        /// Checks if this tangible object change has the given tangible object as a connection to add.
        /// </summary>
        /// <param name="tangibleObject">The tangible object to check.</param>
        /// <returns>Returns true when this tangible object change has the tangible object as a connection to add.</returns>
        public bool HasConnectionToAdd(TangibleObject tangibleObject)
        {
            if (tangibleObject != null)
            {
                foreach (ConnectionItem connectionItem in this.ConnectionsToAdd)
                {
                    if (tangibleObject.Equals(connectionItem.TangibleObject))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasConnectionToAdd(TangibleObject tangibleObject)

        #region Method: HasConnectionToRemove(TangibleObject tangibleObject)
        /// <summary>
        /// Checks if this tangible object change has the given tangible object as a connection to remove.
        /// </summary>
        /// <param name="tangibleObject">The tangible object to check.</param>
        /// <returns>Returns true when this tangible object change has the tangible object as a connection to remove.</returns>
        public bool HasConnectionToRemove(TangibleObject tangibleObject)
        {
            if (tangibleObject != null)
            {
                foreach (ConnectionItemCondition connectionItemCondition in this.ConnectionsToRemove)
                {
                    if (tangibleObject.Equals(connectionItemCondition.TangibleObject))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasConnectionToRemove(TangibleObject tangibleObject)

        #region Method: HasMatter(Matter matter)
        /// <summary>
        /// Checks if this tangible object change has the given matter.
        /// </summary>
        /// <param name="matter">The matter to check.</param>
        /// <returns>Returns true when this tangible object change has the matter.</returns>
        public bool HasMatter(Matter matter)
        {
            if (matter != null)
            {
                foreach (MatterChange matterChange in this.Matter)
                {
                    if (matter.Equals(matterChange.Matter))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasMatter(Matter matter)

        #region Method: HasMatterToAdd(Matter matter)
        /// <summary>
        /// Checks if this tangible object change has the given matter as a matter to add.
        /// </summary>
        /// <param name="matter">The matter to check.</param>
        /// <returns>Returns true when this tangible object change has the matter as a matter to add.</returns>
        public bool HasMatterToAdd(Matter matter)
        {
            if (matter != null)
            {
                foreach (MatterValued matterValued in this.MatterToAdd)
                {
                    if (matter.Equals(matterValued.Matter))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasMatterToAdd(Matter matter)

        #region Method: HasMatterToRemove(Matter matter)
        /// <summary>
        /// Checks if this tangible object change has the given matter as a matter to remove.
        /// </summary>
        /// <param name="matter">The matter to check.</param>
        /// <returns>Returns true when this tangible object change has the matter as a matter to remove.</returns>
        public bool HasMatterToRemove(Matter matter)
        {
            if (matter != null)
            {
                foreach (MatterCondition matterCondition in this.MatterToRemove)
                {
                    if (matter.Equals(matterCondition.Matter))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasMatterToRemove(Matter matter)

        #region Method: HasLayer(Matter material)
        /// <summary>
        /// Checks if this tangible object change has the given matter as a layer.
        /// </summary>
        /// <param name="matter">The matter to check.</param>
        /// <returns>Returns true when this tangible object change has the matter as a layer.</returns>
        public bool HasLayer(Matter matter)
        {
            if (matter != null)
            {
                foreach (LayerChange layerChange in this.Layers)
                {
                    if (matter.Equals(layerChange.Matter))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasLayer(Matter matter)

        #region Method: HasLayerToAdd(Matter matter)
        /// <summary>
        /// Checks if this tangible object change has the given matter as a layer to add.
        /// </summary>
        /// <param name="matter">The matter to check.</param>
        /// <returns>Returns true when this tangible object change has the matter as a layer to add.</returns>
        public bool HasLayerToAdd(Matter matter)
        {
            if (matter != null)
            {
                foreach (Layer layer in this.LayersToAdd)
                {
                    if (matter.Equals(layer.Matter))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasLayerToAdd(Matter matter)

        #region Method: HasLayerToRemove(Matter matter)
        /// <summary>
        /// Checks if this tangible object change has the given matter as a layer to remove.
        /// </summary>
        /// <param name="matter">The matter to check.</param>
        /// <returns>Returns true when this tangible object change has the matter as a layer to remove.</returns>
        public bool HasLayerToRemove(Matter matter)
        {
            if (matter != null)
            {
                foreach (LayerCondition layerCondition in this.LayersToRemove)
                {
                    if (matter.Equals(layerCondition.Matter))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasLayerToRemove(Matter matter)

        #region Method: HasCover(TangibleObject cover)
        /// <summary>
        /// Checks whether the tangible object change has the given cover.
        /// </summary>
        /// <param name="cover">The cover that should be checked.</param>
        /// <returns>Returns true when the tangible object change has the given cover.</returns>
        public bool HasCover(TangibleObject cover)
        {
            if (cover != null)
            {
                foreach (CoverChange myCover in this.Covers)
                {
                    if (cover.Equals(myCover.TangibleObject))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasCover(TangibleObject cover)

        #region Method: HasCoverToAdd(TangibleObject cover)
        /// <summary>
        /// Checks whether the tangible object change has the given cover to add.
        /// </summary>
        /// <param name="cover">The cover that should be checked.</param>
        /// <returns>Returns true when the tangible object change has the given cover to add.</returns>
        public bool HasCoverToAdd(TangibleObject cover)
        {
            if (cover != null)
            {
                foreach (Cover myCover in this.CoversToAdd)
                {
                    if (cover.Equals(myCover.TangibleObject))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasCoverToAdd(TangibleObject cover)

        #region Method: HasCoverToRemove(TangibleObject cover)
        /// <summary>
        /// Checks whether the tangible object change has the given cover to remove.
        /// </summary>
        /// <param name="cover">The cover that should be checked.</param>
        /// <returns>Returns true when the tangible object change has the given cover to remove.</returns>
        public bool HasCoverToRemove(TangibleObject cover)
        {
            if (cover != null)
            {
                foreach (CoverCondition myCover in this.CoversToRemove)
                {
                    if (cover.Equals(myCover.TangibleObject))
                        return true;
                }
            }
            return false;
        }
        #endregion Method: HasCoverToRemove(TangibleObject cover)

        #region Method: Remove()
        /// <summary>
        /// Remove the tangible object change.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the parts
            foreach (PartChange partChange in this.Parts)
                partChange.Remove();
            Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangePartChange);

            foreach (Part part in this.PartsToAdd)
                part.Remove();
            Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangePartToAdd);

            foreach (PartCondition partCondition in this.PartsToRemove)
                partCondition.Remove();
            Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangePartToRemove);

            // Remove the connections
            foreach (ConnectionItemChange connectionItemChange in this.Connections)
                connectionItemChange.Remove();
            Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangeConnectionChange);

            foreach (ConnectionItem connectionItem in this.ConnectionsToAdd)
                connectionItem.Remove();
            Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangeConnectionToAdd);

            foreach (ConnectionItemCondition connectionItemCondition in this.ConnectionsToRemove)
                connectionItemCondition.Remove();
            Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangeConnectionToRemove);

            // Remove the matter
            foreach (MatterChange matterChange in this.Matter)
                matterChange.Remove();
            Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangeMatterChange);

            foreach (MatterValued matterValued in this.MatterToAdd)
                matterValued.Remove();
            Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangeMatterToAdd);

            foreach (MatterCondition matterCondition in this.MatterToRemove)
                matterCondition.Remove();
            Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangeMatterToRemove);

            // Remove the layers
            foreach (LayerChange layerChange in this.Layers)
                layerChange.Remove();
            Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangeLayerChange);
            
            foreach (Layer layer in this.LayersToAdd)
                layer.Remove();
            Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangeLayerToAdd);

            foreach (LayerCondition layerCondition in this.LayersToRemove)
                layerCondition.Remove();
            Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangeLayerToRemove);

            // Remove the covers
            foreach (CoverChange coverChange in this.Covers)
                coverChange.Remove();
            Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangeCoverChange);

            foreach (Cover cover in this.CoversToAdd)
                cover.Remove();
            Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangeCoverToAdd);

            foreach (CoverCondition coverCondition in this.CoversToRemove)
                coverCondition.Remove();
            Database.Current.Remove(this.ID, ValueTables.TangibleObjectChangeCoverToRemove);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: TangibleObjectChange

    #region Class: Part
    /// <summary>
    /// A tangible object that acts as a part.
    /// </summary>
    public sealed class Part : TangibleObjectValued
    {

        #region Properties and Fields

        #region Property: Offset
        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        public VectorValue Offset
        {
            get
            {
                return Database.Current.Select<VectorValue>(this.ID, ValueTables.Part, Columns.Offset);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Part, Columns.Offset, value);
                NotifyPropertyChanged("Offset");
            }
        }
        #endregion Property: Offset

        #region Property: Rotation
        /// <summary>
        /// Gets or sets the rotation.
        /// </summary>
        public VectorValue Rotation
        {
            get
            {
                return Database.Current.Select<VectorValue>(this.ID, ValueTables.Part, Columns.Rotation);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Part, Columns.Rotation, value);
                NotifyPropertyChanged("Rotation");
            }
        }
        #endregion Property: Rotation

        #region Property: IsSlotBased
        /// <summary>
        /// Gets or sets the value that indicates whether the part is slot based.
        /// </summary>
        public bool IsSlotBased
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Part, Columns.IsSlotBased);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Part, Columns.IsSlotBased, value);
                NotifyPropertyChanged("IsSlotBased");
            }
        }
        #endregion Property: IsSlotBased

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: Part(uint id)
        /// <summary>
        /// Creates a new part from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the part from.</param>
        private Part(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Part(uint id)

        #region Constructor: Part(Part part)
        /// <summary>
        /// Clones a part.
        /// </summary>
        /// <param name="part">The part to clone.</param>
        public Part(Part part)
            : base(part)
        {
            if (part != null)
            {
                Database.Current.StartChange();

                if (part.Offset != null)
                    this.Offset = new VectorValue(part.Offset);
                if (part.Rotation != null)
                    this.Rotation = new VectorValue(part.Rotation);
                this.IsSlotBased = part.IsSlotBased;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Part(Part part)

        #region Constructor: Part(TangibleObject tangibleObject)
        /// <summary>
        /// Creates a new part from the given tangible object.
        /// </summary>
        /// <param name="tangibleObject">The tangible object to create the part from.</param>
        public Part(TangibleObject tangibleObject)
            : base(tangibleObject)
        {
            Database.Current.StartChange();

            this.Offset = new VectorValue(new Vec4(SemanticsSettings.Values.OffsetX, SemanticsSettings.Values.OffsetY, SemanticsSettings.Values.OffsetZ, 1));
            this.Rotation = new VectorValue(new Vec4(SemanticsSettings.Values.RotationX, SemanticsSettings.Values.RotationY, SemanticsSettings.Values.RotationZ, 1));

            Database.Current.StopChange();
        }
        #endregion Constructor: Part(TangibleObject tangibleObject)

        #region Constructor: Part(TangibleObject tangibleObject, NumericalValueRange quantity)
        /// <summary>
        /// Creates a new part from the given tangible object in the given quantity.
        /// </summary>
        /// <param name="tangibleObject">The tangible object to create the part from.</param>
        /// <param name="quantity">The quantity of the part.</param>
        public Part(TangibleObject tangibleObject, NumericalValueRange quantity)
            : base(tangibleObject, quantity)
        {
            Database.Current.StartChange();

            this.Offset = new VectorValue(new Vec4(SemanticsSettings.Values.OffsetX, SemanticsSettings.Values.OffsetY, SemanticsSettings.Values.OffsetZ, 1));
            this.Rotation = new VectorValue(new Vec4(SemanticsSettings.Values.RotationX, SemanticsSettings.Values.RotationY, SemanticsSettings.Values.RotationZ, 1));

            Database.Current.StopChange();
        }
        #endregion Constructor: Part(TangibleObject tangibleObject, NumericalValueRange quantity)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the part.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the offset and rotation
            if (this.Offset != null)
                this.Offset.Remove();
            if (this.Rotation != null)
                this.Rotation.Remove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: Part

    #region Class: PartCondition
    /// <summary>
    /// A condition on a part.
    /// </summary>
    public sealed class PartCondition : TangibleObjectCondition
    {

        #region Properties and Fields

        #region Property: Offset
        /// <summary>
        /// Gets or sets the required offset.
        /// </summary>
        public VectorValueCondition Offset
        {
            get
            {
                return Database.Current.Select<VectorValueCondition>(this.ID, ValueTables.PartCondition, Columns.Offset);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.PartCondition, Columns.Offset, value);
                NotifyPropertyChanged("Offset");
            }
        }
        #endregion Property: Offset

        #region Property: Rotation
        /// <summary>
        /// Gets or sets the required rotation.
        /// </summary>
        public VectorValueCondition Rotation
        {
            get
            {
                return Database.Current.Select<VectorValueCondition>(this.ID, ValueTables.PartCondition, Columns.Rotation);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.PartCondition, Columns.Rotation, value);
                NotifyPropertyChanged("Rotation");
            }
        }
        #endregion Property: Rotation

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: PartCondition()
        /// <summary>
        /// Creates a new part condition.
        /// </summary>
        public PartCondition()
            : base()
        {
        }
        #endregion Constructor: PartCondition()

        #region Constructor: PartCondition(uint id)
        /// <summary>
        /// Creates a new part condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the part condition from.</param>
        private PartCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: PartCondition(uint id)

        #region Constructor: PartCondition(PartCondition partCondition)
        /// <summary>
        /// Clones a part condition.
        /// </summary>
        /// <param name="partCondition">The part condition to clone.</param>
        public PartCondition(PartCondition partCondition)
            : base(partCondition)
        {
            if (partCondition != null)
            {
                Database.Current.StartChange();

                if (partCondition.Offset != null)
                    this.Offset = new VectorValueCondition(partCondition.Offset);
                if (partCondition.Rotation != null)
                    this.Rotation = new VectorValueCondition(partCondition.Rotation);

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: PartCondition(PartCondition partCondition)

        #region Constructor: PartCondition(TangibleObject tangibleObject)
        /// <summary>
        /// Creates a condition for the given tangible object.
        /// </summary>
        /// <param name="tangibleObject">The tangible object to create a condition for.</param>
        public PartCondition(TangibleObject tangibleObject)
            : base(tangibleObject)
        {
        }
        #endregion Constructor: PartCondition(TangibleObject tangibleObject)

        #region Constructor: PartCondition(TangibleObject tangibleObject, NumericalValueCondition quantity)
        /// <summary>
        /// Creates a condition for the given tangible object in the given condition.
        /// </summary>
        /// <param name="tangibleObject">The tangible object to create a condition for.</param>
        /// <param name="quantity">The quantity of the part condition.</param>
        public PartCondition(TangibleObject tangibleObject, NumericalValueCondition quantity)
            : base(tangibleObject, quantity)
        {
        }
        #endregion Constructor: PartCondition(TangibleObject tangibleObject, NumericalValueCondition quantity)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the part condition.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the offset and rotation
            if (this.Offset != null)
                this.Offset.Remove();
            if (this.Rotation != null)
                this.Rotation.Remove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: PartCondition

    #region Class: PartChange
    /// <summary>
    /// A change on a part.
    /// </summary>
    public sealed class PartChange : TangibleObjectChange
    {

        #region Properties and Fields

        #region Property: Offset
        /// <summary>
        /// Gets or sets the offset to change with.
        /// </summary>
        public VectorValueChange Offset
        {
            get
            {
                return Database.Current.Select<VectorValueChange>(this.ID, ValueTables.PartChange, Columns.Offset);
            }
            set
            {
                if (this.Offset != null)
                    this.Offset.Remove();

                Database.Current.Update(this.ID, ValueTables.PartChange, Columns.Offset, value);
                NotifyPropertyChanged("Offset");
            }
        }
        #endregion Property: Offset

        #region Property: Rotation
        /// <summary>
        /// Gets or sets the rotation to change with.
        /// </summary>
        public VectorValueChange Rotation
        {
            get
            {
                return Database.Current.Select<VectorValueChange>(this.ID, ValueTables.PartChange, Columns.Rotation);
            }
            set
            {
                if (this.Rotation != null)
                    this.Rotation.Remove();

                Database.Current.Update(this.ID, ValueTables.PartChange, Columns.Rotation, value);
                NotifyPropertyChanged("Rotation");
            }
        }
        #endregion Property: Rotation

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: PartChange()
        /// <summary>
        /// Creates a new part change.
        /// </summary>
        public PartChange()
            : base()
        {
        }
        #endregion Constructor: PartChange()

        #region Constructor: PartChange(uint id)
        /// <summary>
        /// Creates a new part change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a part change from.</param>
        private PartChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: PartChange(uint id)

        #region Constructor: PartChange(PartChange partChange)
        /// <summary>
        /// Clones a tangible object change.
        /// </summary>
        /// <param name="tangibleObjectChange">The tangible object change to clone.</param>
        public PartChange(PartChange partChange)
            : base(partChange)
        {
            if (partChange != null)
            {
                Database.Current.StartChange();

                if (partChange.Offset != null)
                    this.Offset = new VectorValueChange(partChange.Offset);
                if (partChange.Rotation != null)
                    this.Rotation = new VectorValueChange(partChange.Rotation);

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: PartChange(PartChange partChange)

        #region Constructor: PartChange(TangibleObject tangibleObject)
        /// <summary>
        /// Creates a change for the given tangible object.
        /// </summary>
        /// <param name="tangibleObject">The tangible object to create a change for.</param>
        public PartChange(TangibleObject tangibleObject)
            : base(tangibleObject)
        {
        }
        #endregion Constructor: PartChange(TangibleObject tangibleObject)

        #region Constructor: PartChange(TangibleObject tangibleObject, ValueChange quantity)
        /// <summary>
        /// Creates a change for the given tangible object in the form of the given quantity..
        /// </summary>
        /// <param name="tangibleObject">The tangible object to create a change for.</param>
        /// <param name="quantity">The change in quantity.</param>
        public PartChange(TangibleObject tangibleObject, ValueChange quantity)
            : base(tangibleObject)
        {
        }
        #endregion Constructor: PartChange(TangibleObject tangibleObject, ValueChange quantity)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the part change.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the offset and rotation
            if (this.Offset != null)
                this.Offset.Remove();
            if (this.Rotation != null)
                this.Rotation.Remove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: PartChange

    #region Class: Cover
    /// <summary>
    /// A tangible object that acts as a cover.
    /// </summary>
    public sealed class Cover : TangibleObjectValued
    {

        #region Properties and Fields

        #region Property: Offset
        /// <summary>
        /// Gets or sets the offset.
        /// </summary>
        public VectorValue Offset
        {
            get
            {
                return Database.Current.Select<VectorValue>(this.ID, ValueTables.Cover, Columns.Offset);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Cover, Columns.Offset, value);
                NotifyPropertyChanged("Offset");
            }
        }
        #endregion Property: Offset

        #region Property: Rotation
        /// <summary>
        /// Gets or sets the rotation.
        /// </summary>
        public VectorValue Rotation
        {
            get
            {
                return Database.Current.Select<VectorValue>(this.ID, ValueTables.Cover, Columns.Rotation);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Cover, Columns.Rotation, value);
                NotifyPropertyChanged("Rotation");
            }
        }
        #endregion Property: Rotation

        #region Property: Index
        /// <summary>
        /// Gets or sets the index of the cover when it is stacked on top of or underneath other covers: "the n-th cover".
        /// </summary>
        public int Index
        {
            get
            {
                return Database.Current.Select<int>(this.ID, ValueTables.Cover, Columns.Index);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Cover, Columns.Index, value);
                NotifyPropertyChanged("Index");
            }
        }
        #endregion Property: Index

        #region Property: IsSlotBased
        /// <summary>
        /// Gets or sets the value that indicates whether the cover is slot based.
        /// </summary>
        public bool IsSlotBased
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.Cover, Columns.IsSlotBased);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.Cover, Columns.IsSlotBased, value);
                NotifyPropertyChanged("IsSlotBased");
            }
        }
        #endregion Property: IsSlotBased

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: Cover(uint id)
        /// <summary>
        /// Creates a new cover from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a cover from.</param>
        private Cover(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Cover(uint id)

        #region Constructor: Cover(Cover cover)
        /// <summary>
        /// Clones a cover.
        /// </summary>
        /// <param name="cover">The cover to clone.</param>
        public Cover(Cover cover)
            : base(cover)
        {
            if (cover != null)
            {
                Database.Current.StartChange();

                if (cover.Offset != null)
                    this.Offset = new VectorValue(cover.Offset);
                if (cover.Rotation != null)
                    this.Rotation = new VectorValue(cover.Rotation);
                this.Index = cover.Index;
                this.IsSlotBased = cover.IsSlotBased;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Cover(Cover cover)

        #region Constructor: Cover(TangibleObject tangibleObject)
        /// <summary>
        /// Creates a cover from the given tangible object.
        /// </summary>
        /// <param name="tangible object">The tangible object to create a cover from.</param>
        public Cover(TangibleObject tangibleObject)
            : base(tangibleObject)
        {
            Database.Current.StartChange();

            this.Offset = new VectorValue(new Vec4(SemanticsSettings.Values.OffsetX, SemanticsSettings.Values.OffsetY, SemanticsSettings.Values.OffsetZ, 1));
            this.Rotation = new VectorValue(new Vec4(SemanticsSettings.Values.RotationX, SemanticsSettings.Values.RotationY, SemanticsSettings.Values.RotationZ, 1));
            this.Index = SemanticsSettings.Values.Index;

            Database.Current.StopChange();
        }
        #endregion Constructor: Cover(TangibleObject tangibleObject)

        #region Constructor: Cover(TangibleObject tangibleObject, NumericalValueRange quantity)
        /// <summary>
        /// Creates a cover from the given tangible object in the given quantity.
        /// </summary>
        /// <param name="tangible object">The tangible object to create a cover from.</param>
        /// <param name="quantity">The quantity of the cover.</param>
        public Cover(TangibleObject tangibleObject, NumericalValueRange quantity)
            : base(tangibleObject, quantity)
        {
            Database.Current.StartChange();

            this.Offset = new VectorValue(new Vec4(SemanticsSettings.Values.OffsetX, SemanticsSettings.Values.OffsetY, SemanticsSettings.Values.OffsetZ, 1));
            this.Rotation = new VectorValue(new Vec4(SemanticsSettings.Values.RotationX, SemanticsSettings.Values.RotationY, SemanticsSettings.Values.RotationZ, 1));
            this.Index = SemanticsSettings.Values.Index;

            Database.Current.StopChange();
        }
        #endregion Constructor: Cover(TangibleObject tangibleObject, NumericalValueRange quantity)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the cover.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the offset and rotation
            if (this.Offset != null)
                this.Offset.Remove();
            if (this.Rotation != null)
                this.Rotation.Remove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: MaterialValued

    #region Class: CoverCondition
    /// <summary>
    /// A condition on a cover.
    /// </summary>
    public sealed class CoverCondition : TangibleObjectCondition
    {

        #region Properties and Fields

        #region Property: Offset
        /// <summary>
        /// Gets or sets the required offset.
        /// </summary>
        public VectorValueCondition Offset
        {
            get
            {
                return Database.Current.Select<VectorValueCondition>(this.ID, ValueTables.CoverCondition, Columns.Offset);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.CoverCondition, Columns.Offset, value);
                NotifyPropertyChanged("Offset");
            }
        }
        #endregion Property: Offset

        #region Property: Rotation
        /// <summary>
        /// Gets or sets the required rotation.
        /// </summary>
        public VectorValueCondition Rotation
        {
            get
            {
                return Database.Current.Select<VectorValueCondition>(this.ID, ValueTables.CoverCondition, Columns.Rotation);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.CoverCondition, Columns.Rotation, value);
                NotifyPropertyChanged("Rotation");
            }
        }
        #endregion Property: Rotation

        #region Property: Index
        /// <summary>
        /// Gets or sets the required index of the cover.
        /// </summary>
        public int? Index
        {
            get
            {
                return Database.Current.Select<int?>(this.ID, ValueTables.CoverCondition, Columns.Index);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.CoverCondition, Columns.Index, value);
                NotifyPropertyChanged("Index");
            }
        }
        #endregion Property: Index

        #region Property: IndexSign
        /// <summary>
        /// Gets or sets the sign for the index in the condition.
        /// </summary>
        public EqualitySignExtended? IndexSign
        {
            get
            {
                return Database.Current.Select<EqualitySignExtended?>(this.ID, ValueTables.CoverCondition, Columns.IndexSign);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.CoverCondition, Columns.IndexSign, value);
                NotifyPropertyChanged("IndexSign");
            }
        }
        #endregion Property: IndexSign

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: CoverCondition()
        /// <summary>
        /// Creates a new cover condition.
        /// </summary>
        public CoverCondition()
            : base()
        {
        }
        #endregion Constructor: CoverCondition()

        #region Constructor: CoverCondition(uint id)
        /// <summary>
        /// Creates a new cover condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a cover condition from.</param>
        private CoverCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: CoverCondition(uint id)

        #region Constructor: CoverCondition(CoverCondition coverCondition)
        /// <summary>
        /// Clones a cover condition.
        /// </summary>
        /// <param name="coverCondition">The cover condition to clone.</param>
        public CoverCondition(CoverCondition coverCondition)
            : base(coverCondition)
        {
            if (coverCondition != null)
            {
                Database.Current.StartChange();

                if (coverCondition.Offset != null)
                    this.Offset = new VectorValueCondition(coverCondition.Offset);
                if (coverCondition.Rotation != null)
                    this.Rotation = new VectorValueCondition(coverCondition.Rotation);
                this.Index = coverCondition.Index;
                this.IndexSign = coverCondition.IndexSign;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: CoverCondition(CoverCondition coverCondition)

        #region Constructor: CoverCondition(TangibleObject tangibleObject)
        /// <summary>
        /// Creates a condition for the given tangible object.
        /// </summary>
        /// <param name="tangible object">The tangible object to create a condition for.</param>
        public CoverCondition(TangibleObject tangibleObject)
            : base(tangibleObject)
        {
        }
        #endregion Constructor: CoverCondition(TangibleObject tangibleObject)

        #region Constructor: CoverCondition(TangibleObject tangibleObject, NumericalValueCondition quantity)
        /// <summary>
        /// Creates a condition for the given tangible object in the given quantity.
        /// </summary>
        /// <param name="tangible object">The tangible object to create a condition for.</param>
        /// <param name="quantity">The quantity of the cover condition.</param>
        public CoverCondition(TangibleObject tangibleObject, NumericalValueCondition quantity)
            : base(tangibleObject, quantity)
        {
        }
        #endregion Constructor: CoverCondition(TangibleObject tangibleObject, NumericalValueCondition quantity)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the cover condition.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the offset and rotation
            if (this.Offset != null)
                this.Offset.Remove();
            if (this.Rotation != null)
                this.Rotation.Remove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: CoverCondition

    #region Class: CoverChange
    /// <summary>
    /// A change on a cover.
    /// </summary>
    public sealed class CoverChange : TangibleObjectChange
    {

        #region Properties and Fields

        #region Property: Offset
        /// <summary>
        /// Gets or sets the offset to change with.
        /// </summary>
        public VectorValueChange Offset
        {
            get
            {
                return Database.Current.Select<VectorValueChange>(this.ID, ValueTables.CoverChange, Columns.Offset);
            }
            set
            {
                if (this.Offset != null)
                    this.Offset.Remove();

                Database.Current.Update(this.ID, ValueTables.CoverChange, Columns.Offset, value);
                NotifyPropertyChanged("Offset");
            }
        }
        #endregion Property: Offset

        #region Property: Rotation
        /// <summary>
        /// Gets or sets the rotation to change with.
        /// </summary>
        public VectorValueChange Rotation
        {
            get
            {
                return Database.Current.Select<VectorValueChange>(this.ID, ValueTables.CoverChange, Columns.Rotation);
            }
            set
            {
                if (this.Rotation != null)
                    this.Rotation.Remove();

                Database.Current.Update(this.ID, ValueTables.CoverChange, Columns.Rotation, value);
                NotifyPropertyChanged("Rotation");
            }
        }
        #endregion Property: Rotation

        #region Property: Index
        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        public int? Index
        {
            get
            {
                return Database.Current.Select<int>(this.ID, ValueTables.CoverChange, Columns.Index);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.CoverChange, Columns.Index, value);
                NotifyPropertyChanged("Index");
            }
        }
        #endregion Property: Index

        #region Property: IndexChange
        /// <summary>
        /// Gets or sets the type of change for the index.
        /// </summary>
        public ValueChangeType? IndexChange
        {
            get
            {
                return Database.Current.Select<ValueChangeType?>(this.ID, ValueTables.CoverChange, Columns.IndexChange);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.CoverChange, Columns.IndexChange, value);
                NotifyPropertyChanged("IndexChange");
            }
        }
        #endregion Property: IndexChange

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: CoverChange()
        /// <summary>
        /// Creates a new cover change.
        /// </summary>
        public CoverChange()
            : base()
        {
        }
        #endregion Constructor: CoverChange()

        #region Constructor: CoverChange(uint id)
        /// <summary>
        /// Creates a new cover change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a cover change from.</param>
        private CoverChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: CoverChange(uint id)

        #region Constructor: CoverChange(CoverChange coverChange)
        /// <summary>
        /// Clones a cover change.
        /// </summary>
        /// <param name="coverChange">The cover change to clone.</param>
        public CoverChange(CoverChange coverChange)
            : base(coverChange)
        {
            if (coverChange != null)
            {
                Database.Current.StartChange();

                if (coverChange.Offset != null)
                    this.Offset = new VectorValueChange(coverChange.Offset);
                if (coverChange.Rotation != null)
                    this.Rotation = new VectorValueChange(coverChange.Rotation);
                this.Index = coverChange.Index;
                this.IndexChange = coverChange.IndexChange;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: CoverChange(CoverChange coverChange)

        #region Constructor: CoverChange(TangibleObject tangibleObject)
        /// <summary>
        /// Creates a change for the given tangible object.
        /// </summary>
        /// <param name="tangible object">The tangible object to create a change for.</param>
        public CoverChange(TangibleObject tangibleObject)
            : base(tangibleObject)
        {
            Database.Current.StartChange();

            this.IndexChange = ValueChangeType.Increase;

            Database.Current.StopChange();
        }
        #endregion Constructor: CoverChange(TangibleObject tangibleObject)

        #region Constructor: CoverChange(TangibleObject tangibleObject, NumericalValueChange quantity)
        /// <summary>
        /// Creates a change for the given tangible object in the form of the given quantity.
        /// </summary>
        /// <param name="tangible object">The tangible object to create a change for.</param>
        /// <param name="quantity">The change in quantity.</param>
        public CoverChange(TangibleObject tangibleObject, NumericalValueChange quantity)
            : base(tangibleObject, quantity)
        {
            Database.Current.StartChange();

            this.IndexChange = ValueChangeType.Increase;

            Database.Current.StopChange();
        }
        #endregion Constructor: CoverChange(TangibleObject tangibleObject, NumericalValueChange quantity)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the cover change.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the offset and rotation
            if (this.Offset != null)
                this.Offset.Remove();
            if (this.Rotation != null)
                this.Rotation.Remove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: CoverChange

    #region Class: ConnectionItem
    /// <summary>
    /// A tangible object that acts as a connection item.
    /// </summary>
    public sealed class ConnectionItem : TangibleObjectValued
    {

        #region Properties and Fields

        #region Property: IsSlotBased
        /// <summary>
        /// Gets or sets the value that indicates whether the connection item is slot based.
        /// </summary>
        public bool IsSlotBased
        {
            get
            {
                return Database.Current.Select<bool>(this.ID, ValueTables.ConnectionItem, Columns.IsSlotBased);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.ConnectionItem, Columns.IsSlotBased, value);
                NotifyPropertyChanged("IsSlotBased");
            }
        }
        #endregion Property: IsSlotBased

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: ConnectionItem(uint id)
        /// <summary>
        /// Creates a new connection item from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the connection item from.</param>
        private ConnectionItem(uint id)
            : base(id)
        {
        }
        #endregion Constructor: ConnectionItem(uint id)

        #region Constructor: ConnectionItem(ConnectionItem connectionItem)
        /// <summary>
        /// Clones a connection item.
        /// </summary>
        /// <param name="connectionItem">The connection item to clone.</param>
        public ConnectionItem(ConnectionItem connectionItem)
            : base(connectionItem)
        {
            if (connectionItem != null)
            {
                Database.Current.StartChange();

                this.IsSlotBased = connectionItem.IsSlotBased;

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: ConnectionItem(ConnectionItem connectionItem)

        #region Constructor: ConnectionItem(TangibleObject tangibleObject)
        /// <summary>
        /// Creates a new connection item from the given tangible object.
        /// </summary>
        /// <param name="tangibleObject">The tangible object to create the connection item from.</param>
        public ConnectionItem(TangibleObject tangibleObject)
            : base(tangibleObject)
        {
        }
        #endregion Constructor: ConnectionItem(TangibleObject tangibleObject)

        #region Constructor: ConnectionItem(TangibleObject tangibleObject, NumericalValueRange quantity)
        /// <summary>
        /// Creates a new connection item from the given tangible object in the given quantity.
        /// </summary>
        /// <param name="tangibleObject">The tangible object to create the connection item from.</param>
        /// <param name="quantity">The quantity of the connection item.</param>
        public ConnectionItem(TangibleObject tangibleObject, NumericalValueRange quantity)
            : base(tangibleObject, quantity)
        {
        }
        #endregion Constructor: ConnectionItem(TangibleObject tangibleObject, NumericalValueRange quantity)

        #endregion Method Group: Constructors

    }
    #endregion Class: ConnectionItem

    #region Class: ConnectionItemCondition
    /// <summary>
    /// A condition on a connection item.
    /// </summary>
    public sealed class ConnectionItemCondition : TangibleObjectCondition
    {

        #region Method Group: Constructors

        #region Constructor: ConnectionItemCondition()
        /// <summary>
        /// Creates a new connection item condition.
        /// </summary>
        public ConnectionItemCondition()
            : base()
        {
        }
        #endregion Constructor: ConnectionItemCondition()

        #region Constructor: ConnectionItemCondition(uint id)
        /// <summary>
        /// Creates a new connection item condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the connection item condition from.</param>
        private ConnectionItemCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: ConnectionItemCondition(uint id)

        #region Constructor: ConnectionItemCondition(ConnectionItemCondition connectionItemCondition)
        /// <summary>
        /// Clones a connection item condition.
        /// </summary>
        /// <param name="connectionItemCondition">The connection item condition to clone.</param>
        public ConnectionItemCondition(ConnectionItemCondition connectionItemCondition)
            : base(connectionItemCondition)
        {
        }
        #endregion Constructor: ConnectionItemCondition(ConnectionItemCondition connectionItemCondition)

        #region Constructor: ConnectionItemCondition(TangibleObject tangibleObject)
        /// <summary>
        /// Creates a condition for the given tangible object.
        /// </summary>
        /// <param name="tangibleObject">The tangible object to create a condition for.</param>
        public ConnectionItemCondition(TangibleObject tangibleObject)
            : base(tangibleObject)
        {
        }
        #endregion Constructor: ConnectionItemCondition(TangibleObject tangibleObject)

        #region Constructor: ConnectionItemCondition(TangibleObject tangibleObject, NumericalValueCondition quantity)
        /// <summary>
        /// Creates a condition for the given tangible object in the given condition.
        /// </summary>
        /// <param name="tangibleObject">The tangible object to create a condition for.</param>
        /// <param name="quantity">The quantity of the connection item condition.</param>
        public ConnectionItemCondition(TangibleObject tangibleObject, NumericalValueCondition quantity)
            : base(tangibleObject, quantity)
        {
        }
        #endregion Constructor: ConnectionItemCondition(TangibleObject tangibleObject, NumericalValueCondition quantity)

        #endregion Method Group: Constructors

    }
    #endregion Class: ConnectionItemCondition

    #region Class: ConnectionItemChange
    /// <summary>
    /// A change on a connection item.
    /// </summary>
    public sealed class ConnectionItemChange : TangibleObjectChange
    {

        #region Method Group: Constructors

        #region Constructor: ConnectionItemChange()
        /// <summary>
        /// Creates a new connection item change.
        /// </summary>
        public ConnectionItemChange()
            : base()
        {
        }
        #endregion Constructor: ConnectionItemChange()

        #region Constructor: ConnectionItemChange(uint id)
        /// <summary>
        /// Creates a new connection item change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a connection item change from.</param>
        private ConnectionItemChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: ConnectionItemChange(uint id)

        #region Constructor: ConnectionItemChange(ConnectionItemChange connectionItemChange)
        /// <summary>
        /// Clones a tangible object change.
        /// </summary>
        /// <param name="tangibleObjectChange">The tangible object change to clone.</param>
        public ConnectionItemChange(ConnectionItemChange connectionItemChange)
            : base(connectionItemChange)
        {
        }
        #endregion Constructor: ConnectionItemChange(ConnectionItemChange connectionItemChange)

        #region Constructor: ConnectionItemChange(TangibleObject tangibleObject)
        /// <summary>
        /// Creates a change for the given tangible object.
        /// </summary>
        /// <param name="tangibleObject">The tangible object to create a change for.</param>
        public ConnectionItemChange(TangibleObject tangibleObject)
            : base(tangibleObject)
        {
        }
        #endregion Constructor: ConnectionItemChange(TangibleObject tangibleObject)

        #region Constructor: ConnectionItemChange(TangibleObject tangibleObject, ValueChange quantity)
        /// <summary>
        /// Creates a change for the given tangible object in the form of the given quantity..
        /// </summary>
        /// <param name="tangibleObject">The tangible object to create a change for.</param>
        /// <param name="quantity">The change in quantity.</param>
        public ConnectionItemChange(TangibleObject tangibleObject, ValueChange quantity)
            : base(tangibleObject)
        {
        }
        #endregion Constructor: ConnectionItemChange(TangibleObject tangibleObject, ValueChange quantity)

        #endregion Method Group: Constructors

    }
    #endregion Class: ConnectionItemChange

}