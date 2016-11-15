using GameSemantics.Components;
using GameSemantics.Data;
using GameSemanticsEngine.GameContent;
using GameSemanticsEngine.Tools;
using Microsoft.Xna.Framework;
using Semantics.Data;
using SemanticsEngine.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NarrativeWorlds
{
    public class NarrativeCharacterInstance
    {
        public NarrativeCharacter NarrativeCharacter { get; set; }
        public TangibleObjectInstance Instance { get; set; }

        // Model instance derived from the Gameobject
        public ModelInstance ModelInstance { get; set; }

        // 3D position
        public Vector3 Position { get; set; }

        public NarrativeCharacterInstance(string name, Vector3 pos)
        {
            ReadOnlyCollection<GameObject> gameObjectForFirstPhysicalObject = GameDatabaseSearch.GetGameObjects(NarrativeCharacter.TangibleObject);
            Instance = GameInstanceManager.Current.Create(gameObjectForFirstPhysicalObject[0]);
            ContentWrapper contentWrapper;
            if (ContentManager.TryGetContentWrapper(Instance, out contentWrapper))
            {
                ModelInstance = contentWrapper.GetContent<ModelInstance>().First();
            }
            Position = pos;
        }
    }
}