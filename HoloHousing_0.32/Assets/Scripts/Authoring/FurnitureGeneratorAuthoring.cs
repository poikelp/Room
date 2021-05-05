using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using System.Collections.Generic;
using Housing.Component;

namespace Housing.Authoring
{

    [DisallowMultipleComponent]
    public class FurnitureGeneratorAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public GameObject _prefab;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var prefabEntity = conversionSystem.GetPrimaryEntity(_prefab);
            dstManager.AddComponentData(entity, new FurnitureGenerator
            {
                Prefab = prefabEntity
            });
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(_prefab);
        }
    }
}
