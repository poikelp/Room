using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Housing.Component
{

    public struct Squarea : IComponentData
    {
        public int2 pos;
        public bool isLock;
    
    }

    public struct SquareaGenerator : IComponentData
    {
        public Entity Prefab;
        public int RoomSize;
    }

    public struct OverFurnitureEntity : IComponentData
    {
        public Entity Value;
    }
}