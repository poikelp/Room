using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Tiny.Text;
using Unity.Tiny.UI;
using Housing.Component;

namespace Housing
{
    /// <summary>
    /// 家具一覧のページを切り替える
    /// </summary>
    
    [UpdateInGroup(typeof(MyUISystemGroup))]
    public class PageSystem : SystemBase
    {
        private const int MAX_INDEX = 2;

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            Entities
                .ForEach((ref RectTransform rt,in Page p) =>
                {
                    if(p.Index != 1)
                    {
                        rt.Hidden = true;
                    }
                }).Run();
        }
        protected override void OnUpdate()
        {
            var uiSystem = World.GetExistingSystem<ProcessUIEvents>();
            Entity incrementButtonEntity = uiSystem.GetEntityByUIName("PageIncrement");
            Entity decrementButtonEntity = uiSystem.GetEntityByUIName("PageDecrement");

            Entity eClicked = Entity.Null;

            Entities.ForEach((Entity entity, in UIState state) =>
            {
                if(state.IsClicked)
                {
                    eClicked = entity;
                }
            }).Run();

            if(eClicked == incrementButtonEntity || eClicked == decrementButtonEntity)
            {
                var pageIndex = GetSingleton<PageIndex>();
                if(eClicked == incrementButtonEntity)
                {
                    pageIndex.Value ++;
                }
                else
                {
                    pageIndex.Value --;
                }
                pageIndex.Value = math.min(math.max(pageIndex.Value, 1), MAX_INDEX);
                SetSingleton<PageIndex>(pageIndex);
                Entities
                .ForEach((ref RectTransform rt,in Page p) =>
                {
                    if(p.Index == pageIndex.Value)
                    {
                        rt.Hidden = false;
                    }
                    else
                    {
                        rt.Hidden = true;
                    }

                }).Run();
                Entity textEntity = uiSystem.GetEntityByUIName("PageIndex");
                TextLayout.SetEntityTextRendererString(EntityManager, textEntity, pageIndex.Value.ToString());
            }
        }
    }
}
