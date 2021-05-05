using Unity.Entities;
using Unity.Mathematics;
using Unity.Tiny;
using Unity.Tiny.Input;
using Unity.Tiny.UI;
using Unity.Transforms;
using System.Collections.Generic;
using Housing.Component;

namespace Housing
{
    [UpdateInGroup(typeof(FurnitureSystemGroup))]
    [UpdateAfter(typeof(GetSquareaSystem))]
    public class SetFurnitureSystem : SystemBase
    {
        private int2 bottomPos;
        private int2 topPos;
        private Translation squareaTrans;
        private List<Squarea> areaSquareas;
        private bool canPut;
        private Entity _squareaEntity;
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<MoveFurnitureTag>();
        }
        
        protected override void OnUpdate()
        {
            Entity squareaEntity = GetSingleton<SelectedSquareaEntity>().Value;
            
            
            Entity eClicked = Entity.Null;
            Entity ePressed = Entity.Null;
            var uiSystem = World.GetExistingSystem<ProcessUIEvents>();
            Entity cmpButtonEntity = uiSystem.GetEntityByUIName("Complete");
            Entities.ForEach((Entity entity, in UIState state) =>
            {
                if(state.IsClicked)
                {
                    eClicked = entity;
                }
                if(state.IsHighlight || state.IsPressed)
                {
                    ePressed = entity;
                }
            }).Run();
            if(ePressed != null && ePressed != Entity.Null)
            {
                return;
            }

            float4 red = new float4(1,0,0,1);
            float4 green = new float4(0,1,0,1);
            float4 white = new float4(1,1,1,1);

            var input = World.GetExistingSystem<InputSystem>();
            Entity moveEntity = GetSingletonEntity<MoveFurnitureTag>();
            Furniture furniture = GetComponent<Furniture>(moveEntity);

            if(squareaEntity != Entity.Null)
            {
                bottomPos = GetComponent<Squarea>(squareaEntity).pos;
                int2 sizeOffset = furniture.Size - new int2(1,1);
                topPos = bottomPos + sizeOffset;

                squareaTrans = GetComponent<Translation>(squareaEntity);
                float3 topSquareaPos = squareaTrans.Value;
            
                areaSquareas = new List<Squarea>();
                int safeCount = 0;

                Entities
                    .WithoutBurst()
                    .ForEach((in Squarea squ, in Translation trans) =>
                    {
                        if(bottomPos.x <= squ.pos.x && squ.pos.x <= topPos.x &&
                            bottomPos.y <= squ.pos.y && squ.pos.y <= topPos.y)
                        {
                            areaSquareas.Add(squ);
                        
                            if(!squ.isLock)
                            {
                                safeCount ++;
                            }
                            if(squ.pos.x == topPos.x && squ.pos.y == topPos.y)
                            {
                                topSquareaPos = trans.Value;
                            }
                        }
                    }).Run();

                Translation _trans = GetComponent<Translation>(moveEntity);
                _trans.Value = (squareaTrans.Value + topSquareaPos) / 2;
                SetComponent(moveEntity, _trans);
                canPut = safeCount == furniture.Size.x*furniture.Size.y;
            }

            bool isPush = (eClicked == cmpButtonEntity) || (!input.IsTouchSupported() && input.GetMouseButtonDown(0) && squareaEntity != Entity.Null);
            
            Entities
                .WithoutBurst()
                .ForEach((ref Squarea squ, ref OverFurnitureEntity over, 
                    ref SpriteRenderer sr, ref Renderer2D r2) =>
                {
                    if(bottomPos.x <= squ.pos.x && squ.pos.x <= topPos.x &&
                        bottomPos.y <= squ.pos.y && squ.pos.y <= topPos.y)
                    {
                        if(canPut)
                        {
                            sr.Color = green;
                            r2.OrderInLayer = 2;
                            if(isPush)
                            {
                                squ.isLock = true;
                                over.Value = moveEntity;
                            }
                        }
                        else
                        {
                            sr.Color = red;
                            r2.OrderInLayer = 100;
                        }
                    }
                    else
                    {
                        sr.Color = white;
                        r2.OrderInLayer = 1;
                    }
                    if(isPush)
                    {
                        sr.Color.w = 0;
                    }
                }).Run();
            if(canPut && isPush)
            {
                EntityManager.RemoveComponent<MoveFurnitureTag>(moveEntity);
                EntityManager.AddComponentData<InRoomPos>(moveEntity, new InRoomPos
                {
                    Value = bottomPos
                });
                var renderer = GetComponent<Renderer2D>(moveEntity);
                short order = (short)(100 - squareaTrans.Value.y*10);
                renderer.OrderInLayer = order;
                SetComponent(moveEntity, renderer);
                
                var cmpTrans = GetComponent<RectTransform>(cmpButtonEntity);
                cmpTrans.Hidden = true;
                SetComponent<RectTransform>(cmpButtonEntity, cmpTrans);
                
            }
            _squareaEntity = squareaEntity;
        }
    }
}
