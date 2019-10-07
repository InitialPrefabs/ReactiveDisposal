using System;
using Unity.Entities;

namespace ReactiveDisposal.Unmanaged.Systems
{
    public abstract class ReactiveDisposalSystem<T> : ComponentSystem 
        where T : struct, ISystemStateComponentData, IDisposable {

        protected void DisposeOnMainThread() {
            Entities.WithAll<T>().WithNone<UnmanagedMemTag>().ForEach((Entity entity, ref T c0) => {
                c0.Dispose();

                PostUpdateCommands.RemoveComponent<T>(entity);
            });
        }
    }
}
