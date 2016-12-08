using GameSemantics.Components;
using GameSemantics.Data;
using GameSemanticsEngine.GameContent;
using GameSemanticsEngine.Tools;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Semantics.Data;
using Semantics.Entities;
using SemanticsEngine.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarrativeWorlds
{
    public class InstancedEntikaObject
    {
        public string Name { get; set; }

        // Original TangibleObject class which this is an instance of
        public TangibleObject TangibleObject { get; set; }
        // Entika TangibleObject instance
        public TangibleObjectInstance TangibleObjectInstance { get; set; }

        // Model instance derived from the Gameobject
        public ModelInstance ModelInstance { get; set; }

        // XNA Model
        public Model Model { get; set; }

        // 3D position
        public Vector3 Position { get; set; }

        public InstancedEntikaObject(string name, Vector3 pos, Model model)
        {
            this.Name = name;
            TangibleObject = DatabaseSearch.GetNode<TangibleObject>(name);
            ReadOnlyCollection<GameObject> gameObjectForFirstPhysicalObject = GameDatabaseSearch.GetGameObjects(TangibleObject);
            TangibleObjectInstance = GameInstanceManager.Current.Create(gameObjectForFirstPhysicalObject[0]);
            ContentWrapper contentWrapper;
            if (ContentManager.TryGetContentWrapper(TangibleObjectInstance, out contentWrapper))
            {
                ModelInstance = contentWrapper.GetContent<ModelInstance>().First();
            }
            Position = pos;
            this.Model = model;
        }
    }
}
