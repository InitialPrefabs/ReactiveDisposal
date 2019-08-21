using NUnit.Framework;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace ReactiveDisposal.Unmanaged.Systems.Tests {

    public class BlobDisposalTests : TestsFixture {

        struct Data {
            public BlobPtr<int> IntPtr;
            public BlobArray<float> FloatArray;
        }

        struct BlobData : ISystemStateComponentData, IDisposable {

            public BlobAssetReference<Data> BlobAsset;

            public void Dispose() {
                if (BlobAsset.IsCreated) {
                    BlobAsset.Dispose();
                }
            }
        }

        class DataDisposalSystem : ReactiveDisposalSystem<BlobData> {

            protected override JobHandle OnUpdate(JobHandle inputDeps) {
                return ScheduleDisposalJob(inputDeps);
            }
        }


        BlobAssetReference<Data> ConstructBlob() {
            var builder = new BlobBuilder(Allocator.Temp);
            ref var root = ref builder.ConstructRoot<Data>();

            ref var intPtrValue = ref builder.Allocate(ref root.IntPtr);
            intPtrValue = 2;

            var floatArray = builder.Allocate(ref root.FloatArray, 3);
            floatArray[0] = 1.0f;
            floatArray[1] = 2.0f;
            floatArray[2] = 3.0f;

            var blobAsset = builder.CreateBlobAssetReference<Data>(Allocator.Persistent);
            builder.Dispose();

            Assert.IsNotNull(blobAsset.Value.IntPtr);
            Assert.IsNotNull(blobAsset.Value.FloatArray);

            return blobAsset;
        }

        DataDisposalSystem dataDisposalSystem;
        EntityQuery blobQuery, blobWithTagQuery;
        BlobAssetReference<Data> blobData;

        public override void SetUp() {
            base.SetUp();
            dataDisposalSystem = world.CreateSystem<DataDisposalSystem>();

            blobQuery         = EmptySystem.GetEntityQuery(ComponentType.ReadOnly<BlobData>());
            blobWithTagQuery  = EmptySystem.GetEntityQuery(ComponentType.ReadOnly<UnmanagedMemTag>(),
                ComponentType.ReadOnly<BlobData>());

            var entity = manager.CreateEntity();
            manager.AddComponentData(entity, new BlobData { BlobAsset = blobData = ConstructBlob() });
            manager.AddComponent(entity, typeof(UnmanagedMemTag));
        }

        [Test]
        public void BlobIsDisposed() {
            using (var entities = blobWithTagQuery.ToEntityArray(Allocator.TempJob)) {
                Assert.AreEqual(1, entities.Length);
                Assert.IsTrue(manager.HasComponent<UnmanagedMemTag>(entities[0]));
                Assert.IsTrue(manager.HasComponent<BlobData>(entities[0]));
                manager.DestroyEntity(entities[0]);
            }

            Assert.AreEqual(0, blobWithTagQuery.CalculateEntityCount());
            Assert.AreEqual(1, blobQuery.CalculateEntityCount());

            dataDisposalSystem.Update();
            manager.CompleteAllJobs();

            // Force update the buffer system
            world.GetExistingSystem<BeginPresentationEntityCommandBufferSystem>().Update();

            Assert.AreEqual(0, blobWithTagQuery.CalculateEntityCount());
            Assert.AreEqual(0, blobQuery.CalculateEntityCount());

            Assert.Catch<InvalidOperationException>(() => {
                var ptr = blobData.Value;
            });
        }

        [Test]
        public void BlobRemains() {
            using (var entities = blobWithTagQuery.ToEntityArray(Allocator.TempJob)) {
                Assert.AreEqual(1, entities.Length);
                Assert.IsTrue(manager.HasComponent<UnmanagedMemTag>(entities[0]));
                Assert.IsTrue(manager.HasComponent<BlobData>(entities[0]));
            }

            dataDisposalSystem.Update();
            manager.CompleteAllJobs();

            world.GetExistingSystem<BeginPresentationEntityCommandBufferSystem>().Update();

            Assert.AreEqual(1, blobWithTagQuery.CalculateEntityCount());
            Assert.AreEqual(1, blobQuery.CalculateEntityCount());
        }
    }
}
