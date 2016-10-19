/**************************************************************************
 * 
 * GameNode.cs
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
using System.Collections.Generic;
using Common;
using GameSemantics.Data;
using Semantics.Components;
using Semantics.Data;

namespace GameSemantics.Components
{

    #region Class: GameNode
    /// <summary>
    /// A game node.
    /// </summary>
    public class GameNode : AbstractGameNode, IComparable<GameNode>
    {

        #region Properties and Fields

        #region Property: Node
        /// <summary>
        /// Gets or sets the node for which this is a game node.
        /// </summary>
        public Node Node
        {
            get
            {
                return GameDatabase.Current.Select<Node>(this.ID, GameTables.GameNode, GameColumns.Node);
            }
            set
            {
                GameDatabase.Current.Update(this.ID, GameTables.GameNode, GameColumns.Node, value);
                NotifyPropertyChanged("Node");
            }
        }
        #endregion Property: Node

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: GameNode()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static GameNode()
        {
            // Node
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.Node, new Tuple<Type, EntryType>(typeof(Node), EntryType.Nullable));
            GameDatabase.Current.AddTableDefinition(GameTables.GameNode, typeof(GameNode), dict);
        }
        #endregion Static Constructor: GameNode()

        #region Constructor: GameNode()
        /// <summary>
        /// Creates a new game node.
        /// </summary>
        public GameNode()
            : base()
        {
        }
        #endregion Constructor: GameNode()

        #region Constructor: GameNode(uint id)
        /// <summary>
        /// Creates a new game node from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the game node from.</param>
        protected GameNode(uint id)
            : base(id)
        {
        }
        #endregion Constructor: GameNode(uint id)

        #region Constructor: GameNode(string name)
        /// <summary>
        /// Creates a new game node with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the game node.</param>
        public GameNode(string name)
            : base(name)
        {
        }
        #endregion Constructor: GameNode(string name)

        #region Constructor: GameNode(GameNode gameNode)
        /// <summary>
        /// Clones a game node.
        /// </summary>
        /// <param name="gameNode">The game node to clone.</param>
        public GameNode(GameNode gameNode)
            : base(gameNode)
        {
            if (gameNode != null)
            {
                GameDatabase.Current.StartChange();

                this.Node = gameNode.Node;

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: GameNode(GameNode gameNode)

        #region Constructor: GameNode(Node node)
        /// <summary>
        /// Creates a new game node for the given node.
        /// </summary>
        /// <param name="node">The node to create the game node for.</param>
        public GameNode(Node node)
            : base()
        {
            if (node != null)
            {
                GameDatabase.Current.StartChange();

                this.Node = node;
                AddName(node.DefaultName);

                GameDatabase.Current.StopChange();
            }
        }
        #endregion Constructor: GameNode(Node node)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the game node.
        /// </summary>
        public override void Remove()
        {
            GameDatabase.Current.StartChange();
            GameDatabase.Current.StartRemove();

            base.Remove();

            GameDatabase.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(GameNode other)
        /// <summary>
        /// Compares the game node to the other game node.
        /// </summary>
        /// <param name="other">The game node to compare to this game node.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(GameNode other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(GameNode other)

        #endregion Method Group: Other

    }
    #endregion Class: GameNode

}