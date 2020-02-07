using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using System;

namespace ReactiveDisposal.Unmanaged.Systems {

    /// <summary>
    /// The base class to implement which allows for scheduling the batch release of memory on a per entity basis.
    /// </summary>
    /// <typeparam name="T">struct that implements ISystemStateComponentData and IDisposable</typeparam>
    [UpdateInGroup(typeof(DisposalGroup))]
    public abstract class ReactiveDisposalJobSystem<T> : JobComponentSystem where T : struct, ISystemStateComponentData,
       IDisposable {

        private struct DisposalJob : IJobForEachWithEntity<T> {

            public EntityCommandBuffer.Concurrent CmdBuffer;

            public void Execute(Entity entity, int index, ref T c0) {
                c0.Dispose();
                CmdBuffer.RemoveComponent(index, entity, typeof(T));
            }
        }

        protected BeginPresentationEntityCommandBufferSystem cmdBufferSystem;
        protected EntityQuery disposalGroup;

        protected override void OnCreate() {
            cmdBufferSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BeginPresentationEntityCommandBufferSystem>();
            disposalGroup = GetEntityQuery(new EntityQueryDesc {
                All = new ComponentType[] { typeof(T) },
                None = new ComponentType[] { typeof(UnmanagedMemTag) }
            });
        }

        protected override void OnDestroy() {
            ScheduleDisposalJob(default).Complete();
        }

        /// <summary>
        /// Convenience function to schedule the disposal job. The subsequent job can be chained from previous jobs or
        /// to subsequent jobs.
        /// </summary>
        /// <param name="inputDeps">The previously scheduled job's dependency.</param>
        /// <returns>A JobHandle with the information of when to run the job.</returns>
        protected JobHandle ScheduleDisposalJob(JobHandle inputDeps) {
            var cmdBuffer   = cmdBufferSystem.CreateCommandBuffer().ToConcurrent();
            var disposalJob = new DisposalJob {
                CmdBuffer = cmdBuffer
            }.Schedule(disposalGroup, inputDeps);

            cmdBufferSystem.AddJobHandleForProducer(disposalJob);
            return disposalJob;
        }
    }
}
