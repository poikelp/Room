using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Tiny;
using urlopener;
using System.Collections.Generic;
using System;
using Housing.Component;

namespace Housing
{
    /// <summary>
    /// URLのクエリパラメータから部屋を再現する
    /// </summary>
    /// <remarks>
    /// TinyではSystem.Convertが使えない
    /// </remarks>

    [UpdateAfter(typeof(GenerateSquareaSystem))]
    public class LoadRoomSystem : SystemBase
    {
        protected override void OnStartRunning()
        {
            base.OnStartRunning();

            // クエリパラメータの取得、変換
            string query = URLOpener.GetQuery();
            string bits = "";
            foreach(var c in query)
            {
                var value = Array.IndexOf(this.ConversionTable, c);
                bits += value / 32;
                value %= 32;
                bits += value / 16;
                value %= 16;
                bits += value / 8;
                value %= 8;
                bits += value / 4;
                value %= 4;
                bits += value / 2;
                bits += value % 2;
            }
            int octet = 0;
            int bitPosition = 0;
            List<byte> bytes = new List<byte>();
            foreach(var b in bits)
            {
                bitPosition++;
                if(b=='1')
                {
                    octet += 0b1000_0000 >> (bitPosition-1);
                }
                if(bitPosition == 8)
                {
                    bytes.Add((byte)octet);
                    octet = 0;
                    bitPosition = 0;
                }
            }
            List<bool> flipList = new List<bool>();
            List<byte> idList = new List<byte>();
            List<int2> posList = new List<int2>();
            for(int i=0; i<bytes.Count; i+=2)
            {
                bool flip = (bytes[i] & 0b1000_0000) == 0b1000_0000;
                byte id = (byte)(bytes[i] % 0b1000_0000);
                int2 pos = new int2(bytes[i+1]%16, bytes[i+1]/16);
                flipList.Add(flip);
                idList.Add(id);
                posList.Add(pos);
            }

            // Entityの生成予約
            var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);
            var generator = GetSingleton<FurnitureGenerator>();
            var prefabSR = GetComponent<SpriteRenderer>(generator.Prefab);
            var renderer = GetComponent<Renderer2D>(generator.Prefab);
            for(int i=0;i<flipList.Count;i++)
            {
                byte id = idList[i];
                Furniture furniture = new Furniture();
                InRoomPos inRoomPos = new InRoomPos{Value = posList[i]};
                Entities.WithoutBurst().ForEach((in Furniture f) =>
                {
                    if(f.ID == id)furniture = f;
                }).Run();
                furniture.isFlip = flipList[i];
                if(furniture.isFlip)
                {
                    var tmp = furniture.Size.x;
                    furniture.Size.x = furniture.Size.y;
                    furniture.Size.y = tmp;
                }
                prefabSR.Sprite = furniture.Sprite;
                int2 top = furniture.Size - new int2(1,1) + posList[i];
                float3 topWorld = RoomPos.P2W(top);
                float3 bottomWorld = RoomPos.P2W(posList[i]);
                float3 center = (topWorld + bottomWorld) / 2;
                Translation trans = new Translation{Value = center};
                quaternion q = quaternion.EulerXYZ(0,flipList[i]?math.PI:0,0);
                Rotation rot = new Rotation{Value=q};
                UnderData under = new UnderData{Top = top,Bottom = posList[i]};
                renderer.OrderInLayer = (short)(100 - trans.Value.y*10);
                var prefab = cmdBuffer.Instantiate(generator.Prefab);
                cmdBuffer.SetComponent(prefab, furniture);
                cmdBuffer.SetComponent(prefab, prefabSR);
                cmdBuffer.SetComponent(prefab, trans);
                cmdBuffer.SetComponent(prefab, rot);
                cmdBuffer.SetComponent(prefab, renderer);
                cmdBuffer.AddComponent(prefab, inRoomPos);
                cmdBuffer.AddComponent(prefab, under);

            }
            cmdBuffer.Playback(EntityManager);
            cmdBuffer.Dispose();
            
        }

        // OnStartRunningで生成予約し生成されたEntityへの参照を付与
        protected override void OnUpdate()
        {
            List<Entity> entityList = new List<Entity>();
            List<UnderData> underList = new List<UnderData>();
            var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);
            Entities
                .WithoutBurst() 
                .ForEach((Entity entity, in UnderData data) =>
                {
                    entityList.Add(entity);
                    underList.Add(data);
                    cmdBuffer.RemoveComponent<UnderData>(entity);
                }).Run();
            for(int i=0;i<entityList.Count;i++)
            {
                int2 bottomPos = underList[i].Bottom;
                int2 topPos = underList[i].Top;
                Entity entity = entityList[i];
                Entities.ForEach((ref OverFurnitureEntity over, ref Squarea squ) =>
                {
                    if(bottomPos.x <= squ.pos.x && squ.pos.x <= topPos.x &&
                        bottomPos.y <= squ.pos.y && squ.pos.y <= topPos.y)
                    {
                        squ.isLock = true;
                        over.Value = entity;
                    }
                }).ScheduleParallel();
            }
            cmdBuffer.Playback(EntityManager);
            cmdBuffer.Dispose();
        }

        private char[] ConversionTable = new char[]
        {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J',
            'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T',
            'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd',
            'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n',
            'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x',
            'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7',
            '8', '9', '-', '_'
        };
    }
}
