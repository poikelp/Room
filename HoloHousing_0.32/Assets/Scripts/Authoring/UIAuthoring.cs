using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Housing.Component;

namespace Housing.Authoring
{
    public class UIAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public int index;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            
            dstManager.AddComponentData<Page>(entity, new Page
            {
                Index = index
            });
        }
    }   
    
}
