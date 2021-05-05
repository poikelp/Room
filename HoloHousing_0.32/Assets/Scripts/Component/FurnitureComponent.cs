using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Housing.Component
{
    public struct Furniture : IComponentData
    {
        public int2 Size;
        public bool isFlip;
        public Entity Sprite;
        public byte ID;
    }
    public struct MoveFurnitureTag : IComponentData {}
    public struct InRoomPos : IComponentData {
        public int2 Value;
    }

    public struct FurnitureGenerator : IComponentData
    {
        public Entity Prefab;
    }
    public struct UnderData : IComponentData
    {
        public int2 Top;
        public int2 Bottom;
    }
}
