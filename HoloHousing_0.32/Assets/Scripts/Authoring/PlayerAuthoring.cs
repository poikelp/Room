using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Housing.Component;

namespace Housing.Authoring
{
    public class PlayerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent<Player>(entity);
            dstManager.AddComponentData<CharacterState>(entity, new CharacterState
            {
                Value = CharacterStates.Wait
            });
            dstManager.AddComponentData<SpriteAnimationData>(entity, new SpriteAnimationData
            {
                ElapsedTime = 0,
            });
            dstManager.AddComponentData<ElapsedTime>(entity, new ElapsedTime
            {
                Value = 0
            });
            dstManager.AddComponentData<MoveData>(entity, new MoveData
            {
                goal = int2.zero,
                nextPointIndex = 0
            });
            dstManager.AddBuffer<MovePoints>(entity);
            
        }
    }
}
