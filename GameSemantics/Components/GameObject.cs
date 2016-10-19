/**************************************************************************
 * 
 * GameObject.cs
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
using GameSemantics.Data;
using Semantics.Data;
using Semantics.Entities;
using Semantics.Utilities;

namespace GameSemantics.Components
{

    #region Class: GameObject
    /// <summary>
    /// A game object.
    /// </summary>
    public class GameObject : AbstractGameNode, IComparable<GameObject>
    {

        #region Properties and Fields

        #region Property: TangibleObject
        /// <summary>
        /// Gets or sets the tangible object for which this is a game object.
        /// </summary>
        public TangibleObject TangibleObject
        {
            get
            {
                return GameDatabase.Current.Select<TangibleObject>(this.ID, GameTables.GameObject, GameColumns.TangibleObject);
            }
            set
            {
                TangibleObject currentTangibleObject = this.TangibleObject;
                if (currentTangibleObject != null)
                    RemoveName(currentTangibleObject.DefaultName);

                GameDatabase.Current.Update(this.ID, GameTables.GameObject, GameColumns.TangibleObject, value);
                NotifyPropertyChanged("TangibleObject");

                if (value != null)
                    AddName(value.DefaultName);
            }
        }
        #endregion Property: TangibleObject

        #region Property: Matter
        /// <summary>
        /// Gets the matter for the tangible object.
        /// </summary>
        public ReadOnlyCollection<MatterValued> Matter
        {
            get
            {
                return GameDatabase.Current.SelectAll<MatterValued>(this.ID, GameTables.GameObjectMatter, GameColumns.MatterValued).AsReadOnly();
            }
        }
        #endregion Property: Matter

        #region Property: PartGameObjects
        /// <summary>
        /// Gets the dictionary that defines the game objects for each part of this game object.
        /// </summary>
        public Dictionary<Part, GameObject> PartGameObjects
        {
            get
            {
                return GameDatabase.Current.SelectAll<Part, GameObject>(this.ID, GameTables.GameObjectPartGameObjects, GameColumns.Part, GameColumns.GameObject);
            }
        }
        #endregion Property: PartGameObjects

        #region Property: CoverGameObjects
        /// <summary>
        /// Gets the dictionary that defines the game objects for each cover of this game object.
        /// </summary>
        public Dictionary<Cover, GameObject> CoverGameObjects
        {
            get
            {
                return GameDatabase.Current.SelectAll<Cover, GameObject>(this.ID, GameTables.GameObjectCoverGameObjects, GameColumns.Cover, GameColumns.GameObject);
            }
        }
        #endregion Property: CoverGameObjects

        #region Property: LayerGameNodes
        /// <summary>
        /// Gets the dictionary that defines the game nodes for each layer of this game object.
        /// </summary>
        public Dictionary<Layer, GameNode> LayerGameNodes
        {
            get
            {
                return GameDatabase.Current.SelectAll<Layer, GameNode>(this.ID, GameTables.GameObjectLayerGameNodes, GameColumns.Layer, GameColumns.GameNode);
            }
        }
        #endregion Property: LayerGameNodes

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: GameObject()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static GameObject()
        {
            // Tangible object
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(GameColumns.TangibleObject, new Tuple<Type, EntryType>(typeof(TangibleObject), EntryType.Nullable));
            GameDatabase.Current.AddTableDefinition(GameTables.GameObject, typeof(GameObject), dict);

            // Matter
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(GameColumns.MatterValued, new Tuple<Type, EntryType>(typeof(MatterValued), EntryType.Intermediate));
            GameDatabase.Current.AddTableDefinition(GameTables.GameObjectMatter, typeof(GameObject), dict);

            // Part
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(GameColumns.Part, new Tuple<Type, EntryType>(typeof(Part), EntryType.Intermediate));
            GameDatabase.Current.AddTableDefinition(GameTables.GameObjectPartGameObjects, typeof(GameObject), dict);

            // Cover
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(GameColumns.Cover, new Tuple<Type, EntryType>(typeof(Cover), EntryType.Intermediate));
            GameDatabase.Current.AddTableDefinition(GameTables.GameObjectCoverGameObjects, typeof(GameObject), dict);

            // Layer
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(GameColumns.Layer, new Tuple<Type, EntryType>(typeof(Layer), EntryType.Intermediate));
            GameDatabase.Current.AddTableDefinition(GameTables.GameObjectLayerGameNodes, typeof(GameObject), dict);
        }
        #endregion Static Constructor: GameObject()

        #region Constructor: GameObject()
        /// <summary>
        /// Creates a new game object.
        /// </summary>
        public GameObject()
            : base()
        {
        }
        #endregion Constructor: GameObject()

        #region Constructor: GameObject(uint id)
        /// <summary>
        /// Creates a new game object from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the game object from.</param>
        protected GameObject(uint id)
            : base(id)
        {
        }
        #endregion Constructor: GameObject(uint id)

        #region Constructor: GameObject(string name)
        /// <summary>
        /// Creates a new game object with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the game object.</param>
        public GameObject(string name)
            : base(name)
        {
        }
        #endregion Constructor: GameObject(string name)

        #region Constructor: GameObject(GameObject gameObject)
        /// <summary>
        /// Clones a game object.
        /// </summary>
        /// <param name="gameObject">The game object to clone.</param>
        public GameObject(GameObject gameObject)
            : base(gameObject)
        {
            if (gameObject != null)
            {
                GameDatabase.Current.StartChange();

                this.TangibleObject = gameObject.TangibleObject;

                foreach (MatterValued matterValued in gameObject.Matter)
                    AddMatter(matterValued.Clone());

                Dictionary<Part, GameObject> partGameObjects = gameObject.PartGameObjects;
                foreach (Part part in partGameObjects.Keys)
                    SetGameObject(part, new GameObject(partGameObjects[part]));

                Dictionary<Cover, GameObject> coverGameObjects = gameObject.CoverGameObjects;
                foreach (Cover cover in coverGameObjects.Keys)
                    SetGameObject(cover, new GameObject(coverGameObjects[cover]));

                Dictionary<Layer, GameNode> layerGameNodes = gameObject.LayerGameNodes;
                foreach (Layer layer in layerGameNodes.Keys)
                    SetGameNode(layer, new GameNode(layerGameNodes[layer]));

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: GameObject(GameObject gameObject)

        #region Constructor: GameObject(TangibleObject tangibleObject)
        /// <summary>
        /// Creates a new game object for the given tangible object.
        /// </summary>
        /// <param name="tangibleObject">The tangible object to create the game object for.</param>
        public GameObject(TangibleObject tangibleObject)
            : base(tangibleObject == null ? null : tangibleObject.DefaultName)
        {
            if (tangibleObject != null)
            {
                GameDatabase.Current.StartChange();

                this.TangibleObject = tangibleObject;

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: GameObject(TangibleObject tangibleObject)

        #region Constructor: GameObject(TangibleObject tangibleObject, MatterValued matterValued)
        /// <summary>
        /// Creates a new game object for the given tangible object with the given matter.
        /// </summary>
        /// <param name="tangibleObject">The tangible object to create the game object for.</param>
        /// <param name="matterValued">The matter to add to the game object.</param>
        public GameObject(TangibleObject tangibleObject, MatterValued matterValued)
            : base(tangibleObject == null ? null : tangibleObject.DefaultName)
        {
            GameDatabase.Current.StartChange();

            if (tangibleObject != null)
                this.TangibleObject = tangibleObject;
            AddMatter(matterValued);

            GameDatabase.Current.StopChange();
        }
        #endregion Constructor: GameObject(TangibleObject tangibleObject, MatterValued matterValued)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

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
                GameDatabase.Current.Insert(this.ID, GameTables.GameObjectMatter, Columns.MatterValued, matterValued);
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
                if (HasMatter(matterValued.Matter))
                {
                    // Remove the matter
                    GameDatabase.Current.Remove(this.ID, GameTables.GameObjectMatter, Columns.MatterValued, matterValued);
                    NotifyPropertyChanged("Matter");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveMatter(MatterValued matterValued)

        #region Method: SetGameObject(Part part, GameObject gameObject)
        /// <summary>
        /// Assign the given game object to the given part.
        /// </summary>
        /// <param name="part">The part to assign a game object to.</param>
        /// <param name="gameObject">The game object to assign to the part, can be null.</param>
        /// <returns>Returns whether the relation has been succesfully established.</returns>
        public Message SetGameObject(Part part, GameObject gameObject)
        {
            if (part != null && part.TangibleObject != null && this.TangibleObject != null)
            {
                // Make sure the tangible object has the part and the tangible object of the part and the game object are the same
                if (this.TangibleObject.Parts.Contains(part) && (gameObject == null || part.TangibleObject.Equals(gameObject.TangibleObject)))
                {
                    // Link the part and the game object by removing or updating the existing value, or by inserting it in the database
                    if (this.PartGameObjects.ContainsKey(part))
                    {
                        if (gameObject == null)
                            GameDatabase.Current.Remove(this.ID, GameTables.GameObjectPartGameObjects, GameColumns.Part, part, true);
                        else
                            GameDatabase.Current.Update(this.ID, GameTables.GameObjectPartGameObjects, GameColumns.GameObject, gameObject);
                    }
                    else
                        GameDatabase.Current.Insert(this.ID, GameTables.GameObjectPartGameObjects, new string[] { GameColumns.Part, GameColumns.GameObject }, new object[] { part, gameObject });

                    NotifyPropertyChanged("PartGameObjects");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: SetGameObject(Part part, GameObject gameObject)

        #region Method: SetGameObject(Cover cover, GameObject gameObject)
        /// <summary>
        /// Assign the given game object to the given cover.
        /// </summary>
        /// <param name="cover">The cover to assign a game object to.</param>
        /// <param name="gameObject">The game object to assign to the cover, can be null.</param>
        /// <returns>Returns whether the relation has been succesfully established.</returns>
        public Message SetGameObject(Cover cover, GameObject gameObject)
        {
            if (cover != null && cover.TangibleObject != null && this.TangibleObject != null)
            {
                // Make sure the tangible object has the cover and the tangible object of the cover and the game object are the same
                if (this.TangibleObject.Covers.Contains(cover) && (gameObject == null || cover.TangibleObject.Equals(gameObject.TangibleObject)))
                {
                    // Link the cover and the game object by removing or updating the existing value, or by inserting it in the database
                    if (this.CoverGameObjects.ContainsKey(cover))
                    {
                        if (gameObject == null)
                            GameDatabase.Current.Remove(this.ID, GameTables.GameObjectCoverGameObjects, GameColumns.Cover, cover, true);
                        else
                            GameDatabase.Current.Update(this.ID, GameTables.GameObjectCoverGameObjects, GameColumns.GameObject, gameObject);
                    }
                    else
                        GameDatabase.Current.Insert(this.ID, GameTables.GameObjectCoverGameObjects, new string[] { GameColumns.Cover, GameColumns.GameObject }, new object[] { cover, gameObject });

                    NotifyPropertyChanged("CoverGameObjects");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: SetGameObject(Cover cover, GameObject gameObject)

        #region Method: SetGameNode(Layer layer, GameNode gameNode)
        /// <summary>
        /// Assign the given game node to the given layer.
        /// </summary>
        /// <param name="layer">The layer to assign a game node to.</param>
        /// <param name="gameNode">The game node to assign to the layer, can be null.</param>
        /// <returns>Returns whether the relation has been succesfully established.</returns>
        public Message SetGameNode(Layer layer, GameNode gameNode)
        {
            if (layer != null && layer.Matter != null && this.TangibleObject != null)
            {
                // Make sure the tangible object has the layer and the matter of the layer and the game node are the same
                if (this.TangibleObject.Layers.Contains(layer))
                {
                    // Link the layer and the game node by removing or updating the existing value, or by inserting it in the database
                    if (this.LayerGameNodes.ContainsKey(layer) && (gameNode == null || layer.Matter.Equals(gameNode.Node)))
                    {
                        if (gameNode == null)
                            GameDatabase.Current.Remove(this.ID, GameTables.GameObjectLayerGameNodes, GameColumns.Layer, layer, true);
                        else
                            GameDatabase.Current.Update(this.ID, GameTables.GameObjectLayerGameNodes, GameColumns.GameNode, gameNode);
                    }
                    else
                        GameDatabase.Current.Insert(this.ID, GameTables.GameObjectLayerGameNodes, new string[] { GameColumns.Layer, GameColumns.GameNode }, new object[] { layer, gameNode });

                    NotifyPropertyChanged("LayerGameNodes");

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: SetGameNode(Layer layer, GameNode gameNode)

        #endregion Method Group: Add/Remove

        #region Method Group: Getters

        #region Method: GetGameObject(Part part)
        /// <summary>
        /// Get the game object that belongs to the given part.
        /// </summary>
        /// <param name="part">The part to get the game object of.</param>
        /// <returns>The game object of the part.</returns>
        public GameObject GetGameObject(Part part)
        {
            GameObject gameObject = null;
            if (part != null)
                this.PartGameObjects.TryGetValue(part, out gameObject);
            return gameObject;
        }
        #endregion Method: GetGameObject(Part part)

        #region Method: GetGameObject(Cover cover)
        /// <summary>
        /// Get the game object that belongs to the given cover.
        /// </summary>
        /// <param name="cover">The cover to get the game object of.</param>
        /// <returns>The game object of the cover.</returns>
        public GameObject GetGameObject(Cover cover)
        {
            GameObject gameObject = null;
            if (cover != null)
                this.CoverGameObjects.TryGetValue(cover, out gameObject);
            return gameObject;
        }
        #endregion Method: GetGameObject(Cover cover)

        #region Method: GetGameNode(Layer layer)
        /// <summary>
        /// Get the game node that belongs to the given layer.
        /// </summary>
        /// <param name="layer">The layer to get the game node of.</param>
        /// <returns>The game node of the layer.</returns>
        public GameNode GetGameNode(Layer layer)
        {
            GameNode gameNode = null;
            if (layer != null)
                this.LayerGameNodes.TryGetValue(layer, out gameNode);
            return gameNode;
        }
        #endregion Method: GetGameNode(Layer layer)

        #endregion Method Group: Getters

        #region Method Group: Other

        #region Method: HasMatter(Matter matter)
        /// <summary>
        /// Checks if this game object has the given matter.
        /// </summary>
        /// <param name="matter">The matter to check.</param>
        /// <returns>Returns true when this game object has the matter.</returns>
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

        #region Method: Remove()
        /// <summary>
        /// Remove the game object.
        /// </summary>
        public override void Remove()
        {
            GameDatabase.Current.StartChange();
            GameDatabase.Current.StartRemove();

            GameDatabase.Current.Remove(this.ID, GameTables.GameObjectPartGameObjects);
            GameDatabase.Current.Remove(this.ID, GameTables.GameObjectCoverGameObjects);
            GameDatabase.Current.Remove(this.ID, GameTables.GameObjectLayerGameNodes);

            base.Remove();

            GameDatabase.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(GameObject other)
        /// <summary>
        /// Compares the game object to the other game object.
        /// </summary>
        /// <param name="other">The game object to compare to this game object.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(GameObject other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(GameObject other)

        #endregion Method Group: Other

    }
    #endregion Class: GameObject

}