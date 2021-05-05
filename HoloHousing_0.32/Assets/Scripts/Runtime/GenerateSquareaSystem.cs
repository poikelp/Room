using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Tiny;
using Housing.Component;

namespace Housing
{


    
    public class GenerateSquareaSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireSingletonForUpdate<SquareaGenerator>();
        }
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);

            var generatorEntity = GetSingletonEntity<SquareaGenerator>();
            var generator = GetSingleton<SquareaGenerator>();

            Entity prefab = generator.Prefab;
            for(int x=0;x<generator.RoomSize;x++)
            {
                for(int y=0;y<generator.RoomSize;y++)
                {
                    var squarea = cmdBuffer.Instantiate(prefab);
                    var _pos = new int2(x,y);
                    cmdBuffer.SetComponent(squarea, new Squarea
                    {
                        pos = _pos,
                        isLock = false
                    });
                    cmdBuffer.SetComponent(squarea, new Translation
                    {
                        Value = RoomPos.P2W(_pos)
                    });
                    cmdBuffer.SetComponent(squarea, new Renderer2D
                    {
                        OrderInLayer = (short)(generator.RoomSize*2+1 - (x+y))
                    }); 
                }
            }

            cmdBuffer.DestroyEntity(generator.Prefab);
            cmdBuffer.RemoveComponent<SquareaGenerator>(generatorEntity);

            cmdBuffer.Playback(EntityManager);
            cmdBuffer.Dispose();
        }
        protected override void OnUpdate()
        {
            
        }
    }
}
