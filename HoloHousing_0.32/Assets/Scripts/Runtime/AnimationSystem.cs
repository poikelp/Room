using Unity.Entities;
using Unity.Tiny;
using Housing.Component;

namespace Housing
{
    [UpdateInGroup(typeof(CharacterSystemGroup))]
    [UpdateAfter(typeof(CharacterMoveSystem))]
    public class AnimationSystem : SystemBase
    {
        private const float timePerSprite = 0.1f;

        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<Player>();
        }

        protected override void OnUpdate()
        {

            var ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            var ecb = ecbSystem.CreateCommandBuffer().AsParallelWriter();

            var character = GetSingletonEntity<Player>();
            var state = GetComponent<CharacterState>(character);
            var animData = GetComponent<SpriteAnimationData>(character);        

            float elapsedTime = animData.ElapsedTime;
            elapsedTime += Time.DeltaTime;

            if(elapsedTime > timePerSprite)
            {
                
                var renderer = GetComponent<SpriteRenderer>(character);

                Entity animEntity;
                      
                switch(state.Value)
                {
                    case CharacterStates.Wait:
                        animEntity = GetSingletonEntity<PlayerWaitAnimTag>();
                        break;
                    case CharacterStates.Move:
                        animEntity = GetSingletonEntity<PlayerRunAnimTag>();
                        break;
                    default:
                        animEntity = GetSingletonEntity<PlayerWaitAnimTag>();
                        break;
                }
                var spritesList = GetBuffer<AnimSpritesBuffer>(animEntity);
                
                if(elapsedTime >= timePerSprite*spritesList.Length)
                {
                    elapsedTime = 0;
                }
                var activeSprite = (int)(elapsedTime / timePerSprite);

                renderer.Sprite = spritesList[activeSprite].Sprite;

                EntityManager.SetComponentData<SpriteRenderer>(character, renderer);
            }
            
            animData.ElapsedTime = elapsedTime;
            EntityManager.SetComponentData<SpriteAnimationData>(character, animData);
        }
    }
}
