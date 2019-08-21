using System.Linq;
using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

namespace ReactiveDisposal {

    public class EmptySystem : JobComponentSystem {
        protected override JobHandle OnUpdate(JobHandle dep) { return dep; }

        new public EntityQuery GetEntityQuery(params EntityQueryDesc[] queriesDesc) =>
            base.GetEntityQuery(queriesDesc);

        new public EntityQuery GetEntityQuery(params ComponentType[] componentTypes) =>
            base.GetEntityQuery(componentTypes);

        new public EntityQuery GetEntityQuery(NativeArray<ComponentType> componentTypes) =>
            base.GetEntityQuery(componentTypes);
    }

    public abstract class TestsFixture {

        protected World previousWorld;
        protected World world;
        protected EntityManager manager;
        protected EntityManager.EntityManagerDebug managerDebug;

        public EmptySystem EmptySystem {
            get => World.Active.GetOrCreateSystem<EmptySystem>();
        }

        public EntityQueryBuilder Entities {
            get => new EntityQueryBuilder();
        }

        [SetUp]
        public virtual void SetUp() {
            previousWorld = World.Active;
            world = World.Active = new World("Test World");

            manager = world.EntityManager;
            managerDebug = new EntityManager.EntityManagerDebug(manager);
        }

        [TearDown]
        public virtual void TearDown() {
            if (manager != null && manager.IsCreated) {
                while (world.Systems.ToArray().Length > 0) {
                    world.DestroySystem(world.Systems.ToArray()[0]);
                }

                managerDebug.CheckInternalConsistency();

                world.Dispose();
                world = null;

                World.Active = previousWorld;
                previousWorld = null;
                manager = null;
            }
        }
    }
}
