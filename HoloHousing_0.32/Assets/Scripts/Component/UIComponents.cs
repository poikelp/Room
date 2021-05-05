using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;


namespace Housing.Component
{
    public struct uiImage : IComponentData
    {
        public Entity Value;
        public int2 size;
    }
    public struct Page : IComponentData
    {
        public int Index;
    }
    
    
}