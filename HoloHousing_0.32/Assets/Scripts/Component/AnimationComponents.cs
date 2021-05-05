using Unity.Entities;

namespace Housing.Component
{
    public struct AnimSpritesBuffer : IBufferElementData
    {
        public Entity Sprite;
    }
    public struct PlayerRunAnimTag : IComponentData {}
    public struct PlayerWaitAnimTag : IComponentData {}
    
    public struct SpriteAnimationData : IComponentData
    {
        public float ElapsedTime;
    }
}
