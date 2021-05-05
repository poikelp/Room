using Unity.Entities;
using UnityEngine;
using Housing.Component;

namespace Housing.Authoring
{
    public class PlayerWaitAnimAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent<PlayerWaitAnimTag>(entity);
        }
        
    }
}
