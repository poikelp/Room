using Unity.Entities;
using Unity.Mathematics;

namespace Housing.Component
{
    public struct Player : IComponentData {}
    public struct CharacterState : IComponentData
    {
        public CharacterStates Value;
    }
    public struct MoveData :IComponentData
    {
        public int2 goal;
        public int nextPointIndex;
    }
    public struct MovePoints : IBufferElementData
    {
        public float3 Value;
    }
    public struct ElapsedTime : IComponentData
    {
        public float Value;
    }
}
