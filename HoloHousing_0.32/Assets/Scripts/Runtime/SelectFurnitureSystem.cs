using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Tiny.Input;
using Unity.Tiny.UI;
using Housing.Component;

namespace Housing
{
    [UpdateBefore(typeof(SetFurnitureSystem))]
    [UpdateInGroup(typeof(FurnitureSystemGroup))]
    public class SelectFurnitureSystem : SystemBase
    {
        private float elapsedTime = 0;
        private Entity furnitureEntity = Entity.Null;
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<SelectedSquareaEntity>();
        }
        protected override void OnUpdate()
        {
            if(HasSingleton<MoveFurnitureTag>())
            {
                return;
            }
            Entity ePressed = Entity.Null;
            Entities.ForEach((Entity entity, in UIState state) =>
            {
                if(state.IsHighlight || state.IsPressed || state.IsClicked)
                {
                    ePressed = entity;
                }
            }).Run();
            if(ePressed != null && ePressed != Entity.Null)
            {
                return;
            }

            var input = World.GetExistingSystem<InputSystem>();

            Entity squareaEntity = GetSingleton<SelectedSquareaEntity>().Value;
            Entity overFurnitureEntity = GetComponent<OverFurnitureEntity>(squareaEntity).Value;

            if(input.GetMouseButton(0))
            {
                
                if(input.GetMouseButtonDown(0))
                {
                    furnitureEntity = overFurnitureEntity;
                    elapsedTime = 0;
                }
                else
                {
                    if(furnitureEntity == overFurnitureEntity && furnitureEntity != Entity.Null)
                    {
                        elapsedTime += Time.DeltaTime;
                        if(elapsedTime > 1.0f)
                        {
                            EntityManager.AddComponent<MoveFurnitureTag>(furnitureEntity);
                            Entities.ForEach((ref OverFurnitureEntity over, ref Squarea squ) =>
                            {
                                if(over.Value == overFurnitureEntity)
                                {
                                    over.Value = Entity.Null;
                                    squ.isLock = false;
                                }
                            }).ScheduleParallel();
                            furnitureEntity = Entity.Null;

                            if(input.IsTouchSupported())
                            {
                                var uiSystem = World.GetExistingSystem<ProcessUIEvents>();
                                Entity cmpButtonEntity = uiSystem.GetEntityByUIName("Complete");
                                var cmpTrans = GetComponent<RectTransform>(cmpButtonEntity);
                                cmpTrans.Hidden = false;
                                SetComponent<RectTransform>(cmpButtonEntity, cmpTrans);
                            }
                        }
                    }
                    else
                    {
                        furnitureEntity = Entity.Null;
                    }
                }

            }
            else if(input.GetMouseButtonUp(0))
            {
                if(furnitureEntity == overFurnitureEntity && furnitureEntity != Entity.Null)
                {
                    EntityManager.AddComponent<MoveFurnitureTag>(furnitureEntity);
                    Entities.ForEach((ref OverFurnitureEntity over, ref Squarea squ) =>
                    {
                        if(over.Value == overFurnitureEntity)
                        {
                            over.Value = Entity.Null;
                            squ.isLock = false;
                        }
                    }).ScheduleParallel();
                    furnitureEntity = Entity.Null;

                    if(input.IsTouchSupported())
                    {
                        var uiSystem = World.GetExistingSystem<ProcessUIEvents>();
                        Entity cmpButtonEntity = uiSystem.GetEntityByUIName("Complete");
                        var cmpTrans = GetComponent<RectTransform>(cmpButtonEntity);
                        cmpTrans.Hidden = false;
                        SetComponent<RectTransform>(cmpButtonEntity, cmpTrans);
                    }
                }
            }
        }
    }
}
