using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Housing.Component;

namespace Housing.Authoring
{
    public class FurnitureButtonAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public Sprite img;
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var spriteEntity = conversionSystem.GetPrimaryEntity(img);
            dstManager.AddComponentData<uiImage>(entity, new uiImage
            {
                Value = spriteEntity
            });
        }
    }   
    [UpdateInGroup(typeof(GameObjectDeclareReferencedObjectsGroup))]
    class DeclareFurnitureButtonComponentReference : GameObjectConversionSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((FurnitureButtonAuthoring mgr) =>
            {
                if(mgr.img != null)
                {
                    
                    DeclareReferencedAsset(mgr.img);
                    
                }                
            });
        }
    
    }
}
