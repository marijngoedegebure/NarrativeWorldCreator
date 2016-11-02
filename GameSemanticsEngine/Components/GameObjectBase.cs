/**************************************************************************
 * 
 * GameObjectBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using GameSemantics.Components;
using GameSemanticsEngine.Tools;
using Semantics.Entities;
using SemanticsEngine.Entities;

namespace GameSemanticsEngine.Components
{

    #region Class: GameObjectBase
    /// <summary>
    /// A base of a game object.
    /// </summary>
    public class GameObjectBase : AbstractGameNodeBase
    {

        #region Properties and Fields

        #region Property: GameObject
        /// <summary>
        /// Gets the game object of which this is a game object base.
        /// </summary>
        protected internal GameObject GameObject
        {
            get
            {
                return this.Node as GameObject;
            }
        }
        #endregion Property: GameObject

        #region Property: TangibleObject
        /// <summary>
        /// The tangible object for which this is a game object.
        /// </summary>
        private TangibleObjectBase tangibleObject = null;
        
        /// <summary>
        /// Gets the tangible object for which this is a game object.
        /// </summary>
        public TangibleObjectBase TangibleObject
        {
            get
            {
                return tangibleObject;
            }
        }
        #endregion Property: TangibleObject

        #region Property: Matter
        /// <summary>
        /// The matter for the tangible object.
        /// </summary>
        private MatterValuedBase[] matter = null;

        /// <summary>
        /// Gets the matter for the tangible object.
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
            if (this.GameObject != null)
            {
                List<MatterValuedBase> matterValuedBases = new List<MatterValuedBase>();
                foreach (MatterValued matterValued in this.GameObject.Matter)
                    matterValuedBases.Add(GameBaseManager.Current.GetBase<MatterValuedBase>(matterValued));
                matter = matterValuedBases.ToArray();
            }
        }
        #endregion Property: Matter

        #region Property: PartGameObjects
        /// <summary>
        /// The dictionary that defines the game objects for each part of this game object.
        /// </summary>
        private Dictionary<PartBase, GameObjectBase> partGameObjects = null;

        /// <summary>
        /// Gets the dictionary that defines the game objects for each part of this game object.
        /// </summary>
        public Dictionary<PartBase, GameObjectBase> PartGameObjects
        {
            get
            {
                if (partGameObjects == null)
                {
                    LoadPartGameObjects();
                    if (partGameObjects == null)
                        return new Dictionary<PartBase, GameObjectBase>();
                }
                return partGameObjects;
            }
        }

        /// <summary>
        /// Loads the dictionary that defines the game objects for each part of this game object.
        /// </summary>
        private void LoadPartGameObjects()
        {
            if (this.GameObject != null)
            {
                partGameObjects = new Dictionary<PartBase, GameObjectBase>();
                foreach (KeyValuePair<Part, GameObject> partGameObject in this.GameObject.PartGameObjects)
                {
                    PartBase partBase = GameBaseManager.Current.GetBase<PartBase>(partGameObject.Key);
                    GameObjectBase gameObjectBase = GameBaseManager.Current.GetBase<GameObjectBase>(partGameObject.Value);
                    partGameObjects.Add(partBase, gameObjectBase);
                }
            }
        }
        #endregion Property: PartGameObjects

        #region Property: CoverGameObjects
        /// <summary>
        /// The dictionary that defines the game objects for each cover of this game object.
        /// </summary>
        private Dictionary<CoverBase, GameObjectBase> coverGameObjects = null;

        /// <summary>
        /// Gets the dictionary that defines the game objects for each cover of this game object.
        /// </summary>
        public Dictionary<CoverBase, GameObjectBase> CoverGameObjects
        {
            get
            {
                if (coverGameObjects == null)
                {
                    LoadCoverGameObjects();
                    if (coverGameObjects == null)
                        return new Dictionary<CoverBase, GameObjectBase>();
                }
                return coverGameObjects;
            }
        }

        /// <summary>
        /// Loads the dictionary that defines the game objects for each cover of this game object.
        /// </summary>
        private void LoadCoverGameObjects()
        {
            if (this.GameObject != null)
            {
                coverGameObjects = new Dictionary<CoverBase, GameObjectBase>();
                foreach (KeyValuePair<Cover, GameObject> coverGameObject in this.GameObject.CoverGameObjects)
                {
                    CoverBase coverBase = GameBaseManager.Current.GetBase<CoverBase>(coverGameObject.Key);
                    GameObjectBase gameObjectBase = GameBaseManager.Current.GetBase<GameObjectBase>(coverGameObject.Value);
                    coverGameObjects.Add(coverBase, gameObjectBase);
                }
            }
        }
        #endregion Property: CoverGameObjects

        #region Property: LayerGameNodes
        /// <summary>
        /// The dictionary that defines the game nodes for each layer of this game object.
        /// </summary>
        private Dictionary<LayerBase, GameNodeBase> layerGameNodes = null;

        /// <summary>
        /// Gets the dictionary that defines the game nodes for each layer of this game object.
        /// </summary>
        public Dictionary<LayerBase, GameNodeBase> LayerGameNodes
        {
            get
            {
                if (layerGameNodes == null)
                {
                    LoadLayerGameNodes();
                    if (layerGameNodes == null)
                        return new Dictionary<LayerBase, GameNodeBase>();
                }
                return layerGameNodes;
            }
        }

        /// <summary>
        /// Loads the dictionary that defines the game nodes for each layer of this game object.
        /// </summary>
        private void LoadLayerGameNodes()
        {
            if (this.GameObject != null)
            {
                layerGameNodes = new Dictionary<LayerBase, GameNodeBase>();
                foreach (KeyValuePair<Layer, GameNode> layerGameNode in this.GameObject.LayerGameNodes)
                {
                    LayerBase layerBase = GameBaseManager.Current.GetBase<LayerBase>(layerGameNode.Key);
                    GameNodeBase gameNodeBase = GameBaseManager.Current.GetBase<GameNodeBase>(layerGameNode.Value);
                    layerGameNodes.Add(layerBase, gameNodeBase);
                }
            }
        }
        #endregion Property: LayerGameNodes

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: GameObjectBase(GameObject gameObject)
        /// <summary>
        /// Creates a new game object base from the given game object.
        /// </summary>
        /// <param name="gameObject">The game object to create a game object base from.</param>
        protected internal GameObjectBase(GameObject gameObject)
            : base(gameObject)
        {
            if (gameObject != null)
            {
                this.tangibleObject = GameBaseManager.Current.GetBase<TangibleObjectBase>(gameObject.TangibleObject);

                if (GameBaseManager.PreloadProperties)
                {
                    LoadMatter();
                    LoadPartGameObjects();
                    LoadCoverGameObjects();
                    LoadLayerGameNodes();
                }
            }
        }
        #endregion Constructor: GameObjectBase(GameObject gameObject)

        #endregion Method Group: Constructors

    }
    #endregion Class: GameObjectBase
		
}