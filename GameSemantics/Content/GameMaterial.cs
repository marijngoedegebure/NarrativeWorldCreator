/**************************************************************************
 * 
 * GameMaterial.cs
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
using GameSemantics.Data;

namespace GameSemantics.GameContent
{

    #region Class: GameMaterial
    /// <summary>
    /// A game material.
    /// </summary>
    public class GameMaterial : StaticContent, IComparable<GameMaterial>
    {

        #region Method Group: Constructors

        #region Constructor: GameMaterial()
        /// <summary>
        /// Creates a new game material.
        /// </summary>
        public GameMaterial()
            : base()
        {
        }
        #endregion Constructor: GameMaterial()

        #region Constructor: GameMaterial(uint id)
        /// <summary>
        /// Creates a new game material from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a game material from.</param>
        protected GameMaterial(uint id)
            : base(id)
        {
        }
        #endregion Constructor: GameMaterial(uint id)

        #region Constructor: GameMaterial(string file)
        /// <summary>
        /// Creates a new game material with the given file.
        /// </summary>
        /// <param name="file">The file to assign to the game material.</param>
        public GameMaterial(string file)
            : base(file)
        {
        }
        #endregion Constructor: GameMaterial(string file)

        #region Constructor: GameMaterial(GameMaterial gameMaterial)
        /// <summary>
        /// Clones a game material.
        /// </summary>
        /// <param name="gameMaterial">The game material to clone.</param>
        public GameMaterial(GameMaterial gameMaterial)
            : base(gameMaterial)
        {
        }
        #endregion Constructor: GameMaterial(GameMaterial gameMaterial)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the game material.
        /// </summary>
        public override void Remove()
        {
            GameDatabase.Current.StartChange();
            GameDatabase.Current.StartRemove();

            base.Remove();

            GameDatabase.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(GameMaterial other)
        /// <summary>
        /// Compares the game material to the other game material.
        /// </summary>
        /// <param name="other">The game material to compare to this game material.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(GameMaterial other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(GameMaterial other)

        #endregion Method Group: Other

    }
    #endregion Class: GameMaterial

    #region Class: GameMaterialValued
    /// <summary>
    /// A valued version of a game material.
    /// </summary>
    public class GameMaterialValued : StaticContentValued
    {

        #region Properties and Fields

        #region Property: GameMaterial
        /// <summary>
        /// Gets the game material of which this is a valued game material.
        /// </summary>
        public GameMaterial GameMaterial
        {
            get
            {
                return this.Node as GameMaterial;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: GameMaterial

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: GameMaterialValued(uint id)
        /// <summary>
        /// Creates a new valued game material from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the valued game material from.</param>
        protected GameMaterialValued(uint id)
            : base(id)
        {
        }
        #endregion Constructor: GameMaterialValued(uint id)

        #region Constructor: GameMaterialValued(GameMaterialValued gameMaterialValued)
        /// <summary>
        /// Clones a valued game material.
        /// </summary>
        /// <param name="gameMaterialValued">The valued game material to clone.</param>
        public GameMaterialValued(GameMaterialValued gameMaterialValued)
            : base(gameMaterialValued)
        {
        }
        #endregion Constructor: GameMaterialValued(GameMaterialValued game materialValued)

        #region Constructor: GameMaterialValued(GameMaterial gameMaterial)
        /// <summary>
        /// Creates a new valued game material from the given game material.
        /// </summary>
        /// <param name="gameMaterial">The game material to create a valued game material from.</param>
        public GameMaterialValued(GameMaterial gameMaterial)
            : base(gameMaterial)
        {
        }
        #endregion Constructor: GameMaterialValued(GameMaterial gameMaterial)

        #endregion Method Group: Constructors

    }
    #endregion Class: GameMaterialValued

    #region Class: GameMaterialCondition
    /// <summary>
    /// A condition on a game material.
    /// </summary>
    public class GameMaterialCondition : StaticContentCondition
    {

        #region Properties and Fields

        #region Property: GameMaterial
        /// <summary>
        /// Gets or sets the required game material.
        /// </summary>
        public GameMaterial GameMaterial
        {
            get
            {
                return this.Node as GameMaterial;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: GameMaterial

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: GameMaterialCondition()
        /// <summary>
        /// Creates a new game material condition.
        /// </summary>
        public GameMaterialCondition()
            : base()
        {
        }
        #endregion Constructor: GameMaterialCondition()

        #region Constructor: GameMaterialCondition(uint id)
        /// <summary>
        /// Creates a new game material condition from the given ID.
        /// </summary>
        /// <param name="id">The ID to create the game material condition from.</param>
        protected GameMaterialCondition(uint id)
            : base(id)
        {
        }
        #endregion Constructor: GameMaterialCondition(uint id)

        #region Constructor: GameMaterialCondition(GameMaterialCondition gameMaterialCondition)
        /// <summary>
        /// Clones an game material condition.
        /// </summary>
        /// <param name="gameMaterialCondition">The game material condition to clone.</param>
        public GameMaterialCondition(GameMaterialCondition gameMaterialCondition)
            : base(gameMaterialCondition)
        {
        }
        #endregion Constructor: GameMaterialCondition(GameMaterialCondition gameMaterialCondition)

        #region Constructor: GameMaterialCondition(GameMaterial gameMaterial)
        /// <summary>
        /// Creates a condition for the given game material.
        /// </summary>
        /// <param name="gameMaterial">The game material to create a condition for.</param>
        public GameMaterialCondition(GameMaterial gameMaterial)
            : base(gameMaterial)
        {
        }
        #endregion Constructor: GameMaterialCondition(GameMaterial gameMaterial)

        #endregion Method Group: Constructors

    }
    #endregion Class: GameMaterialCondition

    #region Class: GameMaterialChange
    /// <summary>
    /// A change on a game material.
    /// </summary>
    public class GameMaterialChange : StaticContentChange
    {

        #region Properties and Fields

        #region Property: GameMaterial
        /// <summary>
        /// Gets or sets the affected game material.
        /// </summary>
        public GameMaterial GameMaterial
        {
            get
            {
                return this.Node as GameMaterial;
            }
            set
            {
                this.Node = value;
            }
        }
        #endregion Property: GameMaterial

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: GameMaterialChange()
        /// <summary>
        /// Creates a game material change.
        /// </summary>
        public GameMaterialChange()
            : base()
        {
        }
        #endregion Constructor: GameMaterialChange()

        #region Constructor: GameMaterialChange(uint id)
        /// <summary>
        /// Creates a new game material change from the given ID.
        /// </summary>
        /// <param name="id">The ID to create an game material change from.</param>
        protected GameMaterialChange(uint id)
            : base(id)
        {
        }
        #endregion Constructor: GameMaterialChange(uint id)

        #region Constructor: GameMaterialChange(GameMaterialChange gameMaterialChange)
        /// <summary>
        /// Clones a game material change.
        /// </summary>
        /// <param name="gameMaterialChange">The game material change to clone.</param>
        public GameMaterialChange(GameMaterialChange gameMaterialChange)
            : base(gameMaterialChange)
        {
        }
        #endregion Constructor: GameMaterialChange(GameMaterialChange gameMaterialChange)

        #region Constructor: GameMaterialChange(GameMaterial gameMaterial)
        /// <summary>
        /// Creates a change for the given game material.
        /// </summary>
        /// <param name="gameMaterial">The game material to create a change for.</param>
        public GameMaterialChange(GameMaterial gameMaterial)
            : base(gameMaterial)
        {
        }
        #endregion Constructor: GameMaterialChange(GameMaterial gameMaterial)

        #endregion Method Group: Constructors

    }
    #endregion Class: GameMaterialChange

}