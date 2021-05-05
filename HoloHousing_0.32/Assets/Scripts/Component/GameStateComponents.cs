using Unity.Entities;
using Unity.Mathematics;

namespace Housing.Component
{
    public struct SelectedSquareaEntity : IComponentData
    {
        public Entity Value;
    }
    public struct AlphaEntity : IComponentData
    {
        public Entity Value;
    }
    public struct isUIHidden : IComponentData
    {
        public bool Value;
    }
    public struct PageIndex : IComponentData
    {
        public int Value;
    }
}