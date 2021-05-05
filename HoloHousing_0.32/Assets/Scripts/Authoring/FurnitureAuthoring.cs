using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Housing.Component;

namespace Housing.Authoring
{

    [DisallowMultipleComponent]
    public class FurnitureAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public Sprite _sprite;
        public int2 _size;
        public byte _ID;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var spriteEntity = conversionSystem.GetPrimaryEntity(_sprite);
            dstManager.AddComponentData(entity, new Furniture
            {
                Size = _size,
                isFlip = false,
                Sprite = spriteEntity,
                ID = _ID
            });
        }
    }

    [UpdateInGroup(typeof(GameObjectDeclareReferencedObjectsGroup))]
    class DeclareFurnitureComponentReference : GameObjectConversionSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((FurnitureAuthoring mgr) =>
            {
                if(mgr._sprite != null)
                {
                    DeclareReferencedAsset(mgr._sprite);   
                }                
            });
        }
    }
}
