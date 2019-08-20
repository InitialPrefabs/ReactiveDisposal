using UnityEngine;
using Unity.Entities;

namespace Reactive.Unmanaged {

    /// <summary>
    /// Allows a MonoBehaviour to construct Blob structures with the correct minimal archetype. Blobs 
    /// are not managed the MonoBehaviour.
    /// </summary>
    public abstract class BaseBlob<T> : MonoBehaviour, IConvertGameObjectToEntity where T : struct {

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) {
            dstManager.AddComponent(entity, typeof(UnmanagedMemTag));
            AttachToEntity(entity, dstManager, conversionSystem);
        }

        /// <summary>
        /// Must be called in AttachToEntity so that the BlobAssetReference is contained by a ISystemStateComponentData.
        /// </summary>
        protected abstract BlobAssetReference<T> ConstructBlob();

        protected abstract void AttachToEntity(Entity e, EntityManager dstManager, GameObjectConversionSystem conversionSystem);
    }
}
