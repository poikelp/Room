using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

using Housing.Component;

namespace Housing.Authoring
{
    public class SquareaAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public int2 pos;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData<Squarea>(entity, new Squarea
            {
                pos = this.pos,
                isLock = false
            });
            dstManager.AddComponentData<OverFurnitureEntity>(entity, new OverFurnitureEntity
            {
                Value = Entity.Null
            });
            dstManager.AddComponent<YetSelliarizeTag>(entity);
        }
    }
}
