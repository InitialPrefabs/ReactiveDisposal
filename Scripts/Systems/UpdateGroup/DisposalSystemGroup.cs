using Unity.Entities;

namespace Reactive {

    /// <summary>
    /// Trigger any kind of entity destruction.
    /// </summary>
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class DisposalTriggerGroup { }

    /// <summary>
    /// The system group that will actually run and schedule the destruction of whatever entity and dispose any kind of 
    /// unmanaged memory.
    /// </summary>
    [UpdateInGroup(typeof(PresentationSystemGroup)), UpdateAfter(typeof(DisposalTriggerGroup))]
    public class DisposalGroup : ComponentSystemGroup { }
}
