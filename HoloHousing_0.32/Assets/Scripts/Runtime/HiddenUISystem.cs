using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Tiny.UI;
using Unity.Tiny.Input;
using Housing.Component;

namespace Housing
{
    [UpdateInGroup(typeof(MyUISystemGroup))]
    public class HiddenUISystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var isHidden = GetSingleton<isUIHidden>();

            var uiSystem = World.GetExistingSystem<ProcessUIEvents>();
            
            Entity hiddenButtonEntity = uiSystem.GetEntityByUIName("Hidden");

            if(isHidden.Value)
            {
                var input = World.GetExistingSystem<InputSystem>();
                if(input.GetMouseButtonDown(0))
                {
                    var recttrans = GetComponent<RectTransform>(hiddenButtonEntity);
                    recttrans.Hidden = false;
                    SetComponent<RectTransform>(hiddenButtonEntity, recttrans);
                    isHidden.Value = false;
                }
            }
            else
            {
                Entity eClicked = Entity.Null;
                Entities.ForEach((Entity entity, in UIState state) =>
                {
                    if(state.IsClicked)
                    {
                        eClicked = entity;
                    }
                }).Run();
                if(eClicked == hiddenButtonEntity)
                {
                    var recttrans = GetComponent<RectTransform>(hiddenButtonEntity);
                    recttrans.Hidden = true;
                    SetComponent<RectTransform>(hiddenButtonEntity, recttrans);
                    isHidden.Value = true;
                }

            }
            SetSingleton<isUIHidden>(isHidden);
        }
    }
}
