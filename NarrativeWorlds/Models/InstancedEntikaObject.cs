using GameSemantics.Components;
using GameSemantics.Data;
using GameSemanticsEngine.GameContent;
using GameSemanticsEngine.Tools;
using Microsoft.Xna.Framework;
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
        // Original TangibleObject class which this is an instance of
        public TangibleObject TangibleObject { get; set; }
        // Entika TangibleObject instance
        public TangibleObjectInstance TangibleObjectInstance { get; set; }

        // Model instance derived from the Gameobject
        public ModelInstance ModelInstance { get; set; }

        // 3D position
        public Vector3 Position { get; set; }

        public InstancedEntikaObject(string name, Vector3 pos)
        {
            TangibleObject = DatabaseSearch.GetNode<TangibleObject>("couch");
            ReadOnlyCollection<GameObject> gameObjectForFirstPhysicalObject = GameDatabaseSearch.GetGameObjects(TangibleObject);
            TangibleObjectInstance = GameInstanceManager.Current.Create(gameObjectForFirstPhysicalObject[0]);
            ContentWrapper contentWrapper;
            if (ContentManager.TryGetContentWrapper(TangibleObjectInstance, out contentWrapper))
            {
                ModelInstance = contentWrapper.GetContent<ModelInstance>().First();
            }
            Position = pos;
        }
    }
}
