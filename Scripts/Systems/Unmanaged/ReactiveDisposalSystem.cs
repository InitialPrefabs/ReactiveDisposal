using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using System;

namespace Reactive.Unmanaged.Systems {

    [UpdateInGroup(typeof(DisposalGroup))]
    public abstract class ReactiveDisposalSystem : JobComponentSystem {

        [ExcludeComponent(typeof(UnmanagedMemTag))]
        public struct DisposalJob<T> : IJobForEachWithEntity<T> where T : struct, ISystemStateComponentData, IDisposable {

            [ReadOnly]
            public EntityCommandBuffer CmdBuffer;

            public void Execute(Entity entity, int index, ref T c0) {
                c0.Dispose();
                CmdBuffer.RemoveComponent(entity, typeof(T));
            }
        }

        protected BeginPresentationEntityCommandBufferSystem cmdBufferSystem;

        protected override void OnCreate() {
            cmdBufferSystem = World.Active.GetOrCreateSystem<BeginPresentationEntityCommandBufferSystem>();
        }

        protected JobHandle ScheduleDisposalJob<T>(JobHandle inputDeps) where T : struct, ISystemStateComponentData, IDisposable {
            var job = new DisposalJob<T> {
                CmdBuffer = cmdBufferSystem.CreateCommandBuffer()
            }.Schedule(this);

            cmdBufferSystem.AddJobHandleForProducer(job);
            return JobHandle.CombineDependencies(inputDeps, job);
        }
    }
}
