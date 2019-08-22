using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using System;

#if ENABLE_VALIDATION
using static UnityEngine.Debug;
#endif

namespace ReactiveDisposal.Unmanaged.Systems {

    [UpdateInGroup(typeof(DisposalGroup))]
    public abstract class ReactiveDisposalSystem<T> : JobComponentSystem where T : struct, ISystemStateComponentData,
       IDisposable {

        [ExcludeComponent(typeof(UnmanagedMemTag))]
        public struct DisposalJob : IJobForEachWithEntity<T> {

            [ReadOnly]
            public EntityCommandBuffer CmdBuffer;

            public void Execute(Entity entity, int index, ref T c0) {
#if ENABLE_VALIDATION
                Log($"Disposing unmanaged memory of Type: {c0.GetType()} from Entity: {entity.Index}");
#endif
                c0.Dispose();
                CmdBuffer.RemoveComponent(entity, typeof(T));
            }
        }

        protected BeginPresentationEntityCommandBufferSystem cmdBufferSystem;

        protected override void OnCreate() {
            cmdBufferSystem = World.Active.GetOrCreateSystem<BeginPresentationEntityCommandBufferSystem>();
        }

        protected override void OnDestroy() {
            ScheduleDisposalJob(default).Complete();
        }

        protected JobHandle ScheduleDisposalJob(JobHandle inputDeps) {
            var job = new DisposalJob {
                CmdBuffer = cmdBufferSystem.CreateCommandBuffer()
            }.Schedule(this);

            cmdBufferSystem.AddJobHandleForProducer(job);
            return JobHandle.CombineDependencies(inputDeps, job);
        }
    }
}
