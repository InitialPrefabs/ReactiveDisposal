using Unity.Entities;

namespace ReactiveDisposal {

    /// <summary>
    /// The system group that will actually run and schedule the destruction of whatever entity and dispose any kind of 
    /// unmanaged memory.
    /// </summary>
    [UpdateInGroup(typeof(LateSimulationSystemGroup))]
    public class DisposalGroup : ComponentSystemGroup { }
}
