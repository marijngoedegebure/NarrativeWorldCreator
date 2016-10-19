/**************************************************************************
 * 
 * Scene.cs
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
using System.Collections.ObjectModel;
using Common;
using Semantics.Components;
using Semantics.Data;
using Semantics.Entities;
using Semantics.Utilities;

namespace Semantics.Abstractions
{

    #region Class: Scene
    /// <summary>
    /// A scene.
    /// </summary>
    public class Scene : Abstraction, IComparable<Scene>
    {

        #region Properties and Fields

        #region Property: PhysicalObjects
        /// <summary>
        /// Gets all physical objects that belong to the scene.
        /// </summary>
        public ReadOnlyCollection<PhysicalObjectValued> PhysicalObjects
        {
            get
            {
                List<PhysicalObjectValued> physicalObjects = new List<PhysicalObjectValued>();
                physicalObjects.AddRange(this.PersonalPhysicalObjects);
                physicalObjects.AddRange(this.InheritedPhysicalObjects);
                physicalObjects.AddRange(this.OverriddenPhysicalObjects);
                return physicalObjects.AsReadOnly();
            }
        }
        #endregion Property: PhysicalObjects

        #region Property: PersonalPhysicalObjects
        /// <summary>
        /// Gets the personal physical objects that belong to the scene.
        /// </summary>
        public ReadOnlyCollection<PhysicalObjectValued> PersonalPhysicalObjects
        {
            get
            {
                return Database.Current.SelectAll<PhysicalObjectValued>(this.ID, GenericTables.ScenePhysicalObject, Columns.PhysicalObjectValued).AsReadOnly();
            }
        }
        #endregion Property: PersonalPhysicalObjects

        #region Property: InheritedPhysicalObjects
        /// <summary>
        /// Gets the inherited physical objects that belong to the scene.
        /// </summary>
        public ReadOnlyCollection<PhysicalObjectValued> InheritedPhysicalObjects
        {
            get
            {
                List<PhysicalObjectValued> inheritedPhysicalObjects = new List<PhysicalObjectValued>();

                // Add the physical objects of the parents
                foreach (Node parent in this.PersonalParents)
                {
                    foreach (PhysicalObjectValued inheritedPhysicalObject in ((Scene)parent).PhysicalObjects)
                    {
                        if (!HasOverriddenPhysicalObject(inheritedPhysicalObject.PhysicalObject))
                            inheritedPhysicalObjects.Add(inheritedPhysicalObject);
                    }
                }

                // Add the physical objects of the personal scenes
                foreach (SceneValued sceneValued in this.PersonalScenes)
                    inheritedPhysicalObjects.AddRange(sceneValued.Scene.PhysicalObjects);

                return inheritedPhysicalObjects.AsReadOnly();
            }
        }
        #endregion Property: InheritedPhysicalObjects

        #region Property: OverriddenPhysicalObjects
        /// <summary>
        /// Gets the overridden physical objects.
        /// </summary>
        public ReadOnlyCollection<PhysicalObjectValued> OverriddenPhysicalObjects
        {
            get
            {
                return Database.Current.SelectAll<PhysicalObjectValued>(this.ID, GenericTables.SceneOverriddenPhysicalObject, Columns.PhysicalObjectValued).AsReadOnly();
            }
        }
        #endregion Property: OverriddenPhysicalObjects

        #region Property: Scenes
        /// <summary>
        /// Gets all scenes that belong to the scene.
        /// </summary>
        public ReadOnlyCollection<SceneValued> Scenes
        {
            get
            {
                List<SceneValued> scenes = new List<SceneValued>();
                scenes.AddRange(this.PersonalScenes);
                scenes.AddRange(this.InheritedScenes);
                scenes.AddRange(this.OverriddenScenes);
                return scenes.AsReadOnly();
            }
        }
        #endregion Property: Scenes

        #region Property: PersonalScenes
        /// <summary>
        /// Gets the personal scenes that belong to the scene.
        /// </summary>
        public ReadOnlyCollection<SceneValued> PersonalScenes
        {
            get
            {
                return Database.Current.SelectAll<SceneValued>(this.ID, GenericTables.SceneScene, Columns.SceneValued).AsReadOnly();
            }
        }
        #endregion Property: PersonalScenes

        #region Property: InheritedScenes
        /// <summary>
        /// Gets the inherited scenes that belong to the scene.
        /// </summary>
        public ReadOnlyCollection<SceneValued> InheritedScenes
        {
            get
            {
                List<SceneValued> inheritedScenes = new List<SceneValued>();
                foreach (Node parent in this.PersonalParents)
                {
                    foreach (SceneValued inheritedScene in ((Scene)parent).Scenes)
                    {
                        if (!HasOverriddenScene(inheritedScene.Scene))
                            inheritedScenes.Add(inheritedScene);
                    }
                }
                return inheritedScenes.AsReadOnly();
            }
        }
        #endregion Property: InheritedScenes

        #region Property: OverriddenScenes
        /// <summary>
        /// Gets the overridden scenes.
        /// </summary>
        public ReadOnlyCollection<SceneValued> OverriddenScenes
        {
            get
            {
                return Database.Current.SelectAll<SceneValued>(this.ID, GenericTables.SceneOverriddenScene, Columns.SceneValued).AsReadOnly();
            }
        }
        #endregion Property: OverriddenScenes

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Static Constructor: Scene()
        /// <summary>
        /// A static constructor to add table definitions.
        /// </summary>
        static Scene()
        {
            // Physical objects
            Dictionary<string, Tuple<Type, EntryType>> dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.PhysicalObjectValued, new Tuple<Type, EntryType>(typeof(PhysicalObjectValued), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.ScenePhysicalObject, typeof(Scene), dict);

            // Overridden physical objects
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.PhysicalObjectValued, new Tuple<Type, EntryType>(typeof(PhysicalObjectValued), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.SceneOverriddenPhysicalObject, typeof(Scene), dict);

            // Scenes
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.SceneValued, new Tuple<Type, EntryType>(typeof(SceneValued), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.SceneScene, typeof(Scene), dict);

            // Overridden scenes
            dict = new Dictionary<string, Tuple<Type, EntryType>>();
            dict.Add(Columns.SceneValued, new Tuple<Type, EntryType>(typeof(SceneValued), EntryType.Unique));
            Database.Current.AddTableDefinition(GenericTables.SceneOverriddenScene, typeof(Scene), dict);
        }
        #endregion Static Constructor: Scene()

        #region Constructor: Scene()
        /// <summary>
        /// Creates a new scene.
        /// </summary>
        public Scene()
            : base()
        {
        }
        #endregion Constructor: Scene()

        #region Constructor: Scene(uint id)
        /// <summary>
        /// Creates a new scene from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a scene from.</param>
        protected Scene(uint id)
            : base(id)
        {
        }
        #endregion Constructor: Scene(uint id)

        #region Constructor: Scene(string name)
        /// <summary>
        /// Creates a new scene with the given name.
        /// </summary>
        /// <param name="name">The name to assign to the scene.</param>
        public Scene(string name)
            : base(name)
        {
        }
        #endregion Constructor: Scene(string name)

        #region Constructor: Scene(Scene scene)
        /// <summary>
        /// Clones a scene.
        /// </summary>
        /// <param name="scene">The scene to clone.</param>
        public Scene(Scene scene)
            : base(scene)
        {
            if (scene != null)
            {
                Database.Current.StartChange();

                foreach (PhysicalObjectValued physicalObjectValued in scene.PersonalPhysicalObjects)
                    AddPhysicalObject(physicalObjectValued.Clone());
                foreach (PhysicalObjectValued physicalObjectValued in scene.OverriddenPhysicalObjects)
                    AddOverriddenPhysicalObject(physicalObjectValued.Clone());
                foreach (SceneValued sceneValued in scene.PersonalScenes)
                    AddScene(new SceneValued(sceneValued));
                foreach (SceneValued sceneValued in scene.OverriddenScenes)
                    AddOverriddenScene(new SceneValued(sceneValued));

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: Scene(Scene scene)

        #endregion Method Group: Constructors

        #region Method Group: Add/Remove

        #region Method: AddPhysicalObject(PhysicalObjectValued physicalObjectValued)
        /// <summary>
        /// Add a valued physical object.
        /// </summary>
        /// <param name="physicalObjectValued">The valued physical object to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddPhysicalObject(PhysicalObjectValued physicalObjectValued)
        {
            if (physicalObjectValued != null)
            {
                // Check whether the physical object is already there
                if (HasPhysicalObject(physicalObjectValued.PhysicalObject))
                    return Message.RelationExistsAlready;

                // Insert the valued physical object
                Database.Current.Insert(this.ID, GenericTables.ScenePhysicalObject, new string[] { Columns.PhysicalObject, Columns.PhysicalObjectValued }, new object[] { physicalObjectValued.PhysicalObject, physicalObjectValued });
                NotifyPropertyChanged("PersonalPhysicalObjects");
                NotifyPropertyChanged("PhysicalObjects");

                return Message.RelationSuccess;
            }

            return Message.RelationFail;
        }
        #endregion Method: AddPhysicalObject(PhysicalObjectValued physicalObjectValued)

        #region Method: RemovePhysicalObject(PhysicalObjectValued physicalObjectValued)
        /// <summary>
        /// Removes a valued physical object.
        /// </summary>
        /// <param name="physicalObjectValued">The valued physical object to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemovePhysicalObject(PhysicalObjectValued physicalObjectValued)
        {
            if (physicalObjectValued != null)
            {
                if (HasPhysicalObject(physicalObjectValued.PhysicalObject))
                {
                    // Remove the valued physical object
                    Database.Current.Remove(this.ID, GenericTables.ScenePhysicalObject, Columns.PhysicalObjectValued, physicalObjectValued);
                    NotifyPropertyChanged("PersonalPhysicalObjects");
                    NotifyPropertyChanged("PhysicalObjects");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemovePhysicalObject(PhysicalObjectValued physicalObjectValued)

        #region Method: OverridePhysicalObject(PhysicalObjectValued inheritedPhysicalObject)
        /// <summary>
        /// Override the given inherited physical object.
        /// </summary>
        /// <param name="inheritedPhysicalObject">The inherited physical object that should be overridden.</param>
        /// <returns>Returns whether the override has been successful.</returns>
        public Message OverridePhysicalObject(PhysicalObjectValued inheritedPhysicalObject)
        {
            if (inheritedPhysicalObject != null && inheritedPhysicalObject.PhysicalObject != null && this.InheritedPhysicalObjects.Contains(inheritedPhysicalObject))
            {
                // If the physical object is already available, there is no use to add it
                foreach (PhysicalObjectValued personalPhysicalObject in this.PersonalPhysicalObjects)
                {
                    if (inheritedPhysicalObject.PhysicalObject.Equals(personalPhysicalObject.PhysicalObject))
                        return Message.RelationExistsAlready;
                }
                if (HasOverriddenPhysicalObject(inheritedPhysicalObject.PhysicalObject))
                    return Message.RelationExistsAlready;

                // Copy the valued physical object and add it
                return AddOverriddenPhysicalObject(inheritedPhysicalObject.Clone());
            }
            return Message.RelationFail;
        }
        #endregion Method: OverridePhysicalObject(PhysicalObjectValued inheritedPhysicalObject)

        #region Method: AddOverriddenPhysicalObject(PhysicalObjectValued inheritedPhysicalObject)
        /// <summary>
        /// Add the given overridden physical object.
        /// </summary>
        /// <param name="overriddenPhysicalObject">The overridden physical object to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        private Message AddOverriddenPhysicalObject(PhysicalObjectValued overriddenPhysicalObject)
        {
            if (overriddenPhysicalObject != null)
            {
                Database.Current.Insert(this.ID, GenericTables.SceneOverriddenPhysicalObject, Columns.PhysicalObjectValued, overriddenPhysicalObject);
                NotifyPropertyChanged("OverriddenPhysicalObjects");
                NotifyPropertyChanged("InheritedPhysicalObjects");
                NotifyPropertyChanged("PhysicalObjects");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddOverriddenPhysicalObject(PhysicalObjectValued inheritedPhysicalObject)

        #region Method: RemoveOverriddenPhysicalObject(PhysicalObjectValued overriddenPhysicalObject)
        /// <summary>
        /// Removes an overridden physical object.
        /// </summary>
        /// <param name="overriddenPhysicalObject">The overridden physical object to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveOverriddenPhysicalObject(PhysicalObjectValued overriddenPhysicalObject)
        {
            if (overriddenPhysicalObject != null)
            {
                if (this.OverriddenPhysicalObjects.Contains(overriddenPhysicalObject))
                {
                    // Remove the overridden physical object
                    Database.Current.Remove(this.ID, GenericTables.SceneOverriddenPhysicalObject, Columns.PhysicalObjectValued, overriddenPhysicalObject);
                    NotifyPropertyChanged("OverriddenPhysicalObjects");
                    NotifyPropertyChanged("InheritedPhysicalObjects");
                    NotifyPropertyChanged("PhysicalObjects");
                    overriddenPhysicalObject.Remove();

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveOverriddenPhysicalObject(PhysicalObjectValued overriddenPhysicalObject)

        #region Method: AddScene(SceneValued sceneValued)
        /// <summary>
        /// Add a valued scene.
        /// </summary>
        /// <param name="sceneValued">The valued scene to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        public Message AddScene(SceneValued sceneValued)
        {
            if (sceneValued != null && sceneValued.Scene != null && !this.Equals(sceneValued.Scene))
            {
                // Check whether the scene is already there
                if (HasScene(sceneValued.Scene))
                    return Message.RelationExistsAlready;

                // Insert the valued scene
                Database.Current.Insert(this.ID, GenericTables.SceneScene, new string[] { Columns.Scene, Columns.SceneValued }, new object[] { sceneValued.Scene, sceneValued });
                NotifyPropertyChanged("PersonalScenes");
                NotifyPropertyChanged("Scenes");

                return Message.RelationSuccess;
            }

            return Message.RelationFail;
        }
        #endregion Method: AddScene(SceneValued sceneValued)

        #region Method: RemoveScene(SceneValued sceneValued)
        /// <summary>
        /// Removes a valued scene.
        /// </summary>
        /// <param name="sceneValued">The valued scene to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveScene(SceneValued sceneValued)
        {
            if (sceneValued != null)
            {
                if (HasScene(sceneValued.Scene))
                {
                    // Remove the valued scene
                    Database.Current.Remove(this.ID, GenericTables.SceneScene, Columns.SceneValued, sceneValued);
                    NotifyPropertyChanged("PersonalScenes");
                    NotifyPropertyChanged("Scenes");

                    return Message.RelationSuccess;
                }
            }

            return Message.RelationFail;
        }
        #endregion Method: RemoveScene(SceneValued sceneValued)

        #region Method: OverrideScene(SceneValued inheritedScene)
        /// <summary>
        /// Override the given inherited scene.
        /// </summary>
        /// <param name="inheritedScene">The inherited scene that should be overridden.</param>
        /// <returns>Returns whether the override has been successful.</returns>
        public Message OverrideScene(SceneValued inheritedScene)
        {
            if (inheritedScene != null && inheritedScene.Scene != null && this.InheritedScenes.Contains(inheritedScene))
            {
                // If the scene is already available, there is no use to add it
                foreach (SceneValued personalScene in this.PersonalScenes)
                {
                    if (inheritedScene.Scene.Equals(personalScene.Scene))
                        return Message.RelationExistsAlready;
                }
                if (HasOverriddenScene(inheritedScene.Scene))
                    return Message.RelationExistsAlready;

                // Copy the valued scene and add it
                return AddOverriddenScene(new SceneValued(inheritedScene));
            }
            return Message.RelationFail;
        }
        #endregion Method: OverrideScene(SceneValued inheritedScene)

        #region Method: AddOverriddenScene(SceneValued inheritedScene)
        /// <summary>
        /// Add the given overridden scene.
        /// </summary>
        /// <param name="overriddenScene">The overridden scene to add.</param>
        /// <returns>Returns whether the addition has been successful.</returns>
        private Message AddOverriddenScene(SceneValued overriddenScene)
        {
            if (overriddenScene != null)
            {
                Database.Current.Insert(this.ID, GenericTables.SceneOverriddenScene, Columns.SceneValued, overriddenScene);
                NotifyPropertyChanged("OverriddenScenes");
                NotifyPropertyChanged("InheritedScenes");
                NotifyPropertyChanged("Scenes");

                return Message.RelationSuccess;
            }
            return Message.RelationFail;
        }
        #endregion Method: AddOverriddenScene(SceneValued inheritedScene)

        #region Method: RemoveOverriddenScene(SceneValued overriddenScene)
        /// <summary>
        /// Removes an overridden scene.
        /// </summary>
        /// <param name="overriddenScene">The overridden scene to remove.</param>
        /// <returns>Returns whether the removal has been successful.</returns>
        public Message RemoveOverriddenScene(SceneValued overriddenScene)
        {
            if (overriddenScene != null)
            {
                if (this.OverriddenScenes.Contains(overriddenScene))
                {
                    // Remove the overridden scene
                    Database.Current.Remove(this.ID, GenericTables.SceneOverriddenScene, Columns.SceneValued, overriddenScene);
                    NotifyPropertyChanged("OverriddenScenes");
                    NotifyPropertyChanged("InheritedScenes");
                    NotifyPropertyChanged("Scenes");
                    overriddenScene.Remove();

                    return Message.RelationSuccess;
                }
            }
            return Message.RelationFail;
        }
        #endregion Method: RemoveOverriddenScene(SceneValued overriddenScene)

        #endregion Method Group: Add/Remove

        #region Method Group: Other

        #region Method: HasPhysicalObject(PhysicalObject physicalObject)
        /// <summary>
        /// Checks if this scene has the given physical object.
        /// </summary>
        /// <param name="physicalObject">The physical object to check.</param>
        /// <returns>Returns true when the scene has the physical object.</returns>
        public bool HasPhysicalObject(PhysicalObject physicalObject)
        {
            if (physicalObject != null)
            {
                foreach (PhysicalObjectValued physicalObjectValued in this.PhysicalObjects)
                {
                    if (physicalObject.Equals(physicalObjectValued.PhysicalObject))
                        return true;
                }
            }

            return false;
        }
        #endregion Method: HasPhysicalObject(PhysicalObject physicalObject)

        #region Method: HasOverriddenPhysicalObject(PhysicalObject physicalObject)
        /// <summary>
        /// Checks if this scene has the given overridden physical object.
        /// </summary>
        /// <param name="physicalObject">The physical object to check.</param>
        /// <returns>Returns true when the scene has the overridden physical object.</returns>
        public bool HasOverriddenPhysicalObject(PhysicalObject physicalObject)
        {
            if (physicalObject != null)
            {
                foreach (PhysicalObjectValued physicalObjectValued in this.OverriddenPhysicalObjects)
                {
                    if (physicalObject.Equals(physicalObjectValued.PhysicalObject))
                        return true;
                }
            }

            return false;
        }
        #endregion Method: HasOverriddenPhysicalObject(PhysicalObject physicalObject)

        #region Method: HasScene(Scene scene)
        /// <summary>
        /// Checks if this scene has the given scene.
        /// </summary>
        /// <param name="scene">The scene to check.</param>
        /// <returns>Returns true when the scene has the scene.</returns>
        public bool HasScene(Scene scene)
        {
            if (scene != null)
            {
                foreach (SceneValued sceneValued in this.Scenes)
                {
                    if (scene.Equals(sceneValued.Scene))
                        return true;
                }
            }

            return false;
        }
        #endregion Method: HasScene(Scene scene)

        #region Method: HasOverriddenScene(Scene scene)
        /// <summary>
        /// Checks if this scene has the given overridden scene.
        /// </summary>
        /// <param name="scene">The scene to check.</param>
        /// <returns>Returns true when the scene has the overridden scene.</returns>
        public bool HasOverriddenScene(Scene scene)
        {
            if (scene != null)
            {
                foreach (SceneValued sceneValued in this.OverriddenScenes)
                {
                    if (scene.Equals(sceneValued.Scene))
                        return true;
                }
            }

            return false;
        }
        #endregion Method: HasOverriddenScene(Scene scene)

        #region Method: Remove()
        /// <summary>
        /// Remove the scene.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();
            Database.Current.StartRemove();

            // Remove the physical objects
            foreach (PhysicalObjectValued physicalObjectValued in this.PersonalPhysicalObjects)
                physicalObjectValued.Remove();
            Database.Current.Remove(this.ID, GenericTables.ScenePhysicalObject);

            // Remove the overridden physical objects
            foreach (PhysicalObjectValued physicalObjectValued in this.OverriddenPhysicalObjects)
                physicalObjectValued.Remove();
            Database.Current.Remove(this.ID, GenericTables.SceneOverriddenPhysicalObject);

            // Remove the scenes
            foreach (SceneValued sceneValued in this.PersonalScenes)
                sceneValued.Remove();
            Database.Current.Remove(this.ID, GenericTables.SceneScene);

            // Remove the overridden scenes
            foreach (SceneValued sceneValued in this.OverriddenScenes)
                sceneValued.Remove();
            Database.Current.Remove(this.ID, GenericTables.SceneOverriddenScene);

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #region Method: CompareTo(Scene other)
        /// <summary>
        /// Compares the scene to the other scene.
        /// </summary>
        /// <param name="other">The scene to compare to this scene.</param>
        /// <returns>A value that indicates the relative order of the objects being compared.</returns>
        public int CompareTo(Scene other)
        {
            return base.CompareTo(other);
        }
        #endregion Method: CompareTo(Scene other)

        #endregion Method Group: Other

    }
    #endregion Class: Scene

    #region Class: SceneValued
    /// <summary>
    /// A valued version of a scene.
    /// </summary>
    public class SceneValued : AbstractionValued
    {

        #region Properties and Fields

        #region Property: Scene
        /// <summary>
        /// Gets the scene of which this is a valued scene.
        /// </summary>
        public Scene Scene
        {
            get
            {
                return this.Node as Scene;
            }
            protected set
            {
                this.Node = value;
            }
        }
        #endregion Property: Scene

        #region Property: Quantity
        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        public NumericalValueRange Quantity
        {
            get
            {
                return Database.Current.Select<NumericalValueRange>(this.ID, ValueTables.SceneValued, Columns.Quantity);
            }
            set
            {
                Database.Current.Update(this.ID, ValueTables.SceneValued, Columns.Quantity, value);
                NotifyPropertyChanged("Quantity");
            }
        }
        #endregion Property: Quantity

        #endregion Properties and Fields

        #region Method Group: Constructors

        #region Constructor: SceneValued(uint id)
        /// <summary>
        /// Creates a new valued scene from the given ID.
        /// </summary>
        /// <param name="id">The ID to create a valued scene from.</param>
        protected SceneValued(uint id)
            : base(id)
        {
        }
        #endregion Constructor: SceneValued(uint id)

        #region Constructor: SceneValued(SceneValued sceneValued)
        /// <summary>
        /// Clones a valued scene.
        /// </summary>
        /// <param name="sceneValued">The scene to clone.</param>
        public SceneValued(SceneValued sceneValued)
            : base(sceneValued)
        {
            if (sceneValued != null)
            {
                Database.Current.StartChange();

                this.Quantity = new NumericalValueRange(sceneValued.Quantity);

                Database.Current.StopChange();
            }
        }
        #endregion Constructor: SceneValued(SceneValued sceneValued)

        #region Constructor: SceneValued(Scene scene)
        /// <summary>
        /// Creates a new valued scene from the given scene.
        /// </summary>
        /// <param name="scene">The scene to create a valued scene from.</param>
        public SceneValued(Scene scene)
            : base(scene)
        {
            Database.Current.StartChange();

            this.Quantity = new NumericalValueRange(SemanticsSettings.Values.Quantity);

            Database.Current.StopChange();
        }
        #endregion Constructor: SceneValued(Scene scene)

        #endregion Method Group: Constructors

        #region Method Group: Other

        #region Method: Remove()
        /// <summary>
        /// Remove the valued scene.
        /// </summary>
        public override void Remove()
        {
            Database.Current.StartChange();

            // Remove the quantity
            if (this.Quantity != null)
                this.Quantity.Remove();

            base.Remove();

            Database.Current.StopChange();
        }
        #endregion Method: Remove()

        #endregion Method Group: Other

    }
    #endregion Class: SceneValued

}