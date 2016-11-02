/**************************************************************************
 * 
 * GameNodeBase.cs
 * 
 * Jassin Kessing
 * Computer Graphics & CAD/CAM Group
 * TU Delft
 * 2011
 * 
 * This program is free software; you can redistribute it and/or modify it.
 *
 *************************************************************************/

using GameSemantics.Components;
using GameSemanticsEngine.Tools;
using SemanticsEngine.Components;

namespace GameSemanticsEngine.Components
{

    #region Class: GameNodeBase
    /// <summary>
    /// A base of a game node.
    /// </summary>
    public class GameNodeBase : AbstractGameNodeBase
    {

        #region Properties and Fields

        #region Property: GameNode
        /// <summary>
        /// Gets the game node of which this is a game node base.
        /// </summary>
        protected internal GameNode GameNode
        {
            get
            {
                return base.Node as GameNode;
            }
        }
        #endregion Property: GameNode

        #region Property: Node
        /// <summary>
        /// The node for which this is a game node.
        /// </summary>
        private NodeBase node = null;
        
        /// <summary>
        /// Gets the node for which this is a game node.
        /// </summary>
        protected internal new NodeBase Node
        {
            get
            {
                return node;
            }
        }
        #endregion Property: Node

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: GameNodeBase(GameNode gameNode)
        /// <summary>
        /// Creates a new game node base from the given game node.
        /// </summary>
        /// <param name="gameNode">The game node to create a game node base from.</param>
        protected internal GameNodeBase(GameNode gameNode)
            : base(gameNode)
        {
            if (gameNode != null)
                this.node = GameBaseManager.Current.GetBase<NodeBase>(gameNode.Node);
        }
        #endregion Constructor: GameNodeBase(GameNode gameNode)

        #endregion Method Group: Constructors

    }
    #endregion Class: GameNodeBase

}