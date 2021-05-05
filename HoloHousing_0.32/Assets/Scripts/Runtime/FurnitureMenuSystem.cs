using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Tiny.UI;
using Unity.Tiny.Text;
using Unity.Tiny;
using Unity.Tiny.Input;
using Housing.Component;

namespace Housing
{
    [UpdateInGroup(typeof(MyUISystemGroup))]
    public class FurnitureMenuSystem : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
        }
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            var uiSystem = World.GetExistingSystem<ProcessUIEvents>();
            Entity openedButtonEntity = uiSystem.GetEntityByUIName("Opened");
            var openTrans = GetComponent<RectTransform>(openedButtonEntity);
            openTrans.Hidden = true;
            SetComponent<RectTransform>(openedButtonEntity, openTrans);

            Entity cmpButtonEntity = uiSystem.GetEntityByUIName("Complete");
            var cmpTrans = GetComponent<RectTransform>(cmpButtonEntity);
            cmpTrans.Hidden = true;
            SetComponent<RectTransform>(cmpButtonEntity, cmpTrans);
        }
        protected override void OnUpdate()
        {
            Entity eClicked = Entity.Null;
            var uiSystem = World.GetExistingSystem<ProcessUIEvents>();
            Entity closedButtonEntity = uiSystem.GetEntityByUIName("Closed");
            Entity openedButtonEntity = uiSystem.GetEntityByUIName("Opened");

            var closeTrans = GetComponent<RectTransform>(closedButtonEntity);
            var openTrans = GetComponent<RectTransform>(openedButtonEntity);       
            closeTrans.Hidden = HasSingleton<MoveFurnitureTag>() || !openTrans.Hidden;
                    
            Entities.ForEach((Entity entity, in UIState state) =>
            {
                if(state.IsClicked)
                {
                    eClicked = entity;
                }
            }).Run();

            if(eClicked != null && eClicked != Entity.Null)
            {
                if(eClicked == closedButtonEntity)
                {
                    openTrans.Hidden = false;
                    closeTrans.Hidden = true;
                }
                else if(eClicked == openedButtonEntity)
                {
                    openTrans.Hidden = true;
                }
                else if(HasComponent<Furniture>(eClicked))
                {
                    openTrans.Hidden = true;

                    var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);

                    var data = GetComponent<Furniture>(eClicked);

                    var generator = GetSingleton<FurnitureGenerator>();
                    var prefabSR = GetComponent<SpriteRenderer>(generator.Prefab);
                    prefabSR.Sprite = data.Sprite;

                    var furniture = cmdBuffer.Instantiate(generator.Prefab);
                    cmdBuffer.SetComponent(furniture, data);
                    cmdBuffer.SetComponent(furniture, prefabSR);
                    cmdBuffer.AddComponent<MoveFurnitureTag>(furniture);

                    cmdBuffer.Playback(EntityManager);
                    cmdBuffer.Dispose();

                    var input = World.GetExistingSystem<InputSystem>();
                    if(input.IsTouchSupported())
                    {
                        Entity cmpButtonEntity = uiSystem.GetEntityByUIName("Complete");
                        var cmpTrans = GetComponent<RectTransform>(cmpButtonEntity);
                        cmpTrans.Hidden = false;
                        SetComponent<RectTransform>(cmpButtonEntity, cmpTrans);
                    }
                }
            }
            SetComponent<RectTransform>(closedButtonEntity, closeTrans);
            SetComponent<RectTransform>(openedButtonEntity, openTrans);
        }
    }
}
