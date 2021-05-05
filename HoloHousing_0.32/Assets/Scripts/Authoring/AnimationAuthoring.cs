using UnityEngine;
using Unity.Entities;
using Housing.Component;

namespace Housing.Authoring
{
    public class AnimationAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public Sprite[] sprites;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var spritesBuffer = dstManager.AddBuffer<AnimSpritesBuffer>(entity);
            if(sprites != null)
            {
                foreach(var s in sprites)
                {
                    var spriteEntity = conversionSystem.GetPrimaryEntity(s);
                    spritesBuffer.Add(new AnimSpritesBuffer
                    {
                        Sprite = spriteEntity
                    });    
                }    
            }
        }
    }

    [UpdateInGroup(typeof(GameObjectDeclareReferencedObjectsGroup))]
    class DeclareAnimationComponentReference : GameObjectConversionSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((AnimationAuthoring mgr) =>
            {
                if(mgr.sprites != null)
                {
                    foreach(var s in mgr.sprites)
                    {
                        DeclareReferencedAsset(s);
                    }
                }                
            });
        }
    }
}
