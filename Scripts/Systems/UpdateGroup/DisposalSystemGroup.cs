using Unity.Entities;

namespace ReactiveDisposal {

    /// <summary>
    /// Trigger any kind of entity destruction in this group.
    /// </summary>
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public class DisposalTriggerGroup : ComponentSystemGroup { }

    /// <summary>
    /// The system group that will actually run and schedule the destruction of whatever entity and dispose any kind of 
    /// unmanaged memory.
    /// </summary>
    [UpdateInGroup(typeof(PresentationSystemGroup)), UpdateAfter(typeof(DisposalTriggerGroup))]
    public class DisposalGroup : ComponentSystemGroup { }
}
