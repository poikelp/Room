using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Tiny.Input;
using Unity.Tiny.UI;
using Unity.Tiny.Text;
using Unity.Tiny.Rendering;
using Unity.Tiny;
using Housing.Component;

namespace Housing
{
    [UpdateInGroup(typeof(FurnitureSystemGroup))]
    public class GetSquareaSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<Camera>();
        }

        protected override void OnUpdate()
        {
            Entity eClicked = Entity.Null;
            Entities.ForEach((Entity entity, in UIState state) =>
            {
                if(state.IsClicked)
                {
                    eClicked = entity;
                }
            }).Run();
            if(eClicked != null && eClicked != Entity.Null)
            {
                return;
            }

            
            var input = World.GetExistingSystem<InputSystem>();
            var pos = input.GetInputPosition();

            var s2w = World.GetExistingSystem<ScreenToWorld>();
            short s = -1;
            var cam = GetSingletonEntity<Camera>();
            var worldPos = s2w.ScreenSpaceToWorldSpace(pos, s, cam);
            pos = new float2(worldPos.x, worldPos.y);

            Entity nearSquareaEntity = Entity.Null;
            float distance = 10000;
            Entities
                .WithAll<Squarea>()
                .ForEach((Entity e, in Translation trans) =>
                {
                    float2 p = new float2(trans.Value.x,trans.Value.y);
                    float d =math.distance(p, pos);
                    if(d < distance)
                    {
                        nearSquareaEntity = e;
                        distance = d;
                    }
                }).Run();
            
            if(nearSquareaEntity != null && nearSquareaEntity != Entity.Null)
            {
                Squarea nearSquarea = GetComponent<Squarea>(nearSquareaEntity);
                
                var selected = GetSingleton<SelectedSquareaEntity>();
                if(distance < 0.5f)
                {
                    selected.Value = nearSquareaEntity;
                }
                else
                {
                    selected.Value = Entity.Null;
                }
                SetSingleton<SelectedSquareaEntity>(selected);
            }
        }
    }
}
