using Unity.Entities;
using UnityEngine;
using Housing.Component;

namespace Housing.Authoring
{
    public class PlayerRunAnimAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent<PlayerRunAnimTag>(entity);
        }
        
    }
}
