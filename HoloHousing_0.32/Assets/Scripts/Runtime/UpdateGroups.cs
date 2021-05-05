using Unity.Entities;
using Unity.Tiny.UI;
namespace Housing
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(ClearUIState))]
    public class MyUISystemGroup : ComponentSystemGroup {}    

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class CharacterSystemGroup : ComponentSystemGroup {} 

    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(MyUISystemGroup))]
    [UpdateBefore(typeof(ClearUIState))]
    public class FurnitureSystemGroup : ComponentSystemGroup {}
}