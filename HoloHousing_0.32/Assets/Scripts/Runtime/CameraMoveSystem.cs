using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Tiny.Rendering;
using Unity.Tiny.Input;
using Unity.Tiny.Text;
using Unity.Tiny.UI;
using Unity.Transforms;
using Housing.Component;

namespace Housing
{

    public class CameraMoveSystem : SystemBase
    {
        private float fingerDistance;
        private float2 defaultPos2 = new float2(0,5);
        private float3 defaultPos3 = new float3(0,5,-1);
        private float2 scalePerDistance = new float2(1.33f, 1);
        private float3 diff = float3.zero;

        private float moveSpeed = 0.01f;
        protected override void OnCreate()
        {
            base.OnCreate();
            RequireSingletonForUpdate<Camera>();
        }
        protected override void OnUpdate()
        {
            var input = World.GetExistingSystem<InputSystem>();
            var scroll = input.GetInputScrollDelta();
            
            var cam = GetSingleton<Camera>();
            Entity camEntity = GetSingletonEntity<Camera>();
            cam.fov = math.min(math.max(3, cam.fov += scroll.y*0.1f),6);

            float2 zoomCenter = input.GetInputPosition();


            //モバイルで拡大時UIを押しても家具の移動がされ続ける不具合があるためモバイルでの拡大機能を一時停止
            // if(input.IsTouchSupported())
            // {
            //     if(input.TouchCount() == 2)
            //     {
            //         float2 point0 = new float2(input.GetTouch(0).x, input.GetTouch(0).y);
            //         float2 point1 = new float2(input.GetTouch(1).x, input.GetTouch(1).y);
            //         float distance = math.distance(point0, point1);
                    
            //         if(input.GetTouch(1).phase != TouchState.Began)
            //         {
            //             if(distance > fingerDistance)
            //             {
            //                 cam.fov = math.min(math.max(3, cam.fov -= 0.1f),6);
            //             }
            //             else if(distance < fingerDistance)
            //             {
            //                 cam.fov = math.min(math.max(3, cam.fov += 0.1f),6);
            //             }
            //         }
            //         fingerDistance = distance;
            //         zoomCenter = (point0 + point1)/2;
            //     }
            // }
            SetSingleton<Camera>(cam);

            Translation trans = GetComponent<Translation>(camEntity);
            if(!scroll.Equals(float2.zero))
            {
                diff.x = math.min(math.max(-scalePerDistance.x*(6-cam.fov),diff.x),scalePerDistance.x*(6-cam.fov));
                diff.y = math.min(math.max(-scalePerDistance.y*(6-cam.fov),diff.y),scalePerDistance.y*(6-cam.fov));
                trans.Value = defaultPos3 + diff;
            }
            if((input.GetMouseButton(0) || input.TouchCount() == 1) && !HasSingleton<MoveFurnitureTag>())
            {
                var delta = input.GetInputDelta();
                if(!delta.Equals(float2.zero))
                {
                    
                    var hoge = -delta * moveSpeed;
                    diff.x = math.min(math.max(-scalePerDistance.x*(6-cam.fov),diff.x+hoge.x),scalePerDistance.x*(6-cam.fov));
                    diff.y = math.min(math.max(-scalePerDistance.y*(6-cam.fov),diff.y+hoge.y),scalePerDistance.y*(6-cam.fov));
                    trans.Value = defaultPos3 + diff;
                }
                
            }
            SetComponent(camEntity, trans);


            // var uiSys = World.GetExistingSystem<ProcessUIEvents>();
            // var hoge = uiSys.GetEntityByUIName("hoge");
            // TextLayout.SetEntityTextRendererString(EntityManager, hoge, scroll.ToString());
        }
    }
}
