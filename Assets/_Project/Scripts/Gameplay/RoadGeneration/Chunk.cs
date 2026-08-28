using UnityEngine;

namespace Gameplay.RoadGeneration
{
    /// <summary>
    /// Component attached to instantiated road chunks to keep track of their original prefab.
    /// This is required to return the chunk to the correct object pool.
    /// </summary>
    public class Chunk : MonoBehaviour
    {
        public GameObject OriginalPrefab { get; set; }
    }

}
