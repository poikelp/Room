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
    public class GarbageSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var uiSystem = World.GetExistingSystem<ProcessUIEvents>();
            Entity garbageButtonEntity = uiSystem.GetEntityByUIName("Garbage");
            Entity closedButtonEntity = uiSystem.GetEntityByUIName("Closed");
            Entity cmpButtonEntity = uiSystem.GetEntityByUIName("Complete");
            bool hasMove = HasSingleton<MoveFurnitureTag>();

            RectTransform rect = GetComponent<RectTransform>(garbageButtonEntity);
            rect.Hidden = !hasMove;
            SetComponent(garbageButtonEntity, rect);

            if(!hasMove)
            {
                return;
            }

            Entity furnitureEntity = GetSingletonEntity<MoveFurnitureTag>();                

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
                if(eClicked == garbageButtonEntity)
                {
                    EntityManager.DestroyEntity(furnitureEntity);
                    Entities
                        .WithAll<Squarea>()
                        .ForEach((ref SpriteRenderer sr) =>
                    {
                        sr.Color.w = 0;
                    }).ScheduleParallel();
                    var closeTrans = GetComponent<RectTransform>(closedButtonEntity);
                    closeTrans.Hidden = false;
                    SetComponent<RectTransform>(closedButtonEntity, closeTrans);
                    var cmpTrans = GetComponent<RectTransform>(cmpButtonEntity);
                    cmpTrans.Hidden = true;
                    SetComponent<RectTransform>(cmpButtonEntity, cmpTrans);
                }
            }
        }
    }
}
