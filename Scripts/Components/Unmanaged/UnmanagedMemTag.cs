using Unity.Entities;

namespace ReactiveDisposal.Unmanaged {

    /// <summary>
    /// This is a tag component to ensure that any entities with unmanaged memory must be managed 
    /// by reactive systems.
    /// </summary>
    public struct UnmanagedMemTag : IComponentData { }
}
