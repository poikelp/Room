using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Tiny;
using Unity.Tiny.UI;
using Unity.Tiny.Input;
using Unity.Tiny.Rendering;
using Housing.Component;

namespace Housing
{
    [UpdateInGroup(typeof(MyUISystemGroup))]
    public class FlipSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var uiSystem = World.GetExistingSystem<ProcessUIEvents>();
            Entity flipButtonEntity = uiSystem.GetEntityByUIName("Flip");
            bool hasMove = HasSingleton<MoveFurnitureTag>();

            RectTransform rect = GetComponent<RectTransform>(flipButtonEntity);
            rect.Hidden = !hasMove;
            SetComponent(flipButtonEntity, rect);

            if(!hasMove)
            {
                return;
            }

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
                if(eClicked == flipButtonEntity)
                {
                    Entity moveEntity = GetSingletonEntity<MoveFurnitureTag>();
                    Rotation rot = GetComponent<Rotation>(moveEntity);
                    if(rot.Value.value.w == 1)
                    {
                        rot.Value = quaternion.EulerXYZ(0,math.PI,0);
                    }
                    else
                    {
                        rot.Value = quaternion.EulerXYZ(0,0,0);
                    }
                    SetComponent(moveEntity, rot);
                    Furniture furniture = GetComponent<Furniture>(moveEntity);
                    int tmp = furniture.Size.x;
                    furniture.Size.x = furniture.Size.y;
                    furniture.Size.y = tmp;
                    furniture.isFlip = !furniture.isFlip;
                    SetComponent(moveEntity, furniture);
                }
            }
        }
    }
}
