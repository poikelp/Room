using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using System.Collections.Generic;
using Housing.Component;

namespace Housing.Authoring
{
    public class GameStateAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public Sprite alpha;
        public GameObject squareaPrefab;
        public int roomSize;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var alphaEntity = conversionSystem.GetPrimaryEntity(alpha);
            dstManager.AddComponentData(entity, new AlphaEntity
            {
                Value = alphaEntity
            });
            dstManager.AddComponentData(entity, new SelectedSquareaEntity
            {
                Value = Entity.Null
            });
            dstManager.AddComponentData(entity, new isUIHidden
            {
                Value = false
            });
            dstManager.AddComponentData(entity, new PageIndex
            {
                Value = 1
            });
            dstManager.AddComponentData(entity, new SquareaGenerator
            {
                Prefab = conversionSystem.GetPrimaryEntity(squareaPrefab),
                RoomSize = roomSize
            });
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(squareaPrefab);
        }
    }

    [UpdateInGroup(typeof(GameObjectDeclareReferencedObjectsGroup))]
    class DeclareAlphaReference : GameObjectConversionSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((GameStateAuthoring mgr) =>
            {
                if(mgr.alpha != null)
                {
                    
                    DeclareReferencedAsset(mgr.alpha);
                    
                }                
            });

        }
    
    }
}