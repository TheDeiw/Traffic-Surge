using UnityEngine;
using System.Collections.Generic;
using Gameplay.Traffic;

namespace Gameplay.RoadGeneration
{
    public class RoadSpawner : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject startChunkPrefab;

        [SerializeField] private GameObject[] chunkPrefabs;

        [Header("Settings")]
        [SerializeField] private int initialChunksCount = 5;
        [SerializeField] private float chunkLength = 50f;

        [Header("References")]
        [SerializeField] private Transform player;

        private readonly Dictionary<GameObject, Queue<GameObject>> _chunkPools = new Dictionary<GameObject, Queue<GameObject>>();
        private readonly Queue<GameObject> _activeChunks = new Queue<GameObject>();
        private float _spawnZ = 0f;


        private void Start()
        {
            InitializeChunkPools();
            SpawnChunk(startChunkPrefab);

            for (int i = 0; i < initialChunksCount; i++)
            {
                SpawnRandomChunk();
            }
        }

        private void Update()
        {
            if (player.position.z > (_spawnZ - (chunkLength * initialChunksCount)))
            {
                SpawnRandomChunk();
                RemoveOldChunk();
            }
        }

        /// <summary>
        /// Creates empty queues for each prefab in the dictionary.
        /// </summary>
        private void InitializeChunkPools()
        {
            foreach (var chunkPrefab in chunkPrefabs)
            {
                if (!_chunkPools.ContainsKey(chunkPrefab))
                {
                    _chunkPools.Add(chunkPrefab, new Queue<GameObject>());
                }

                if (startChunkPrefab != null && !_chunkPools.ContainsKey(startChunkPrefab))
                {
                    _chunkPools.Add(startChunkPrefab, new Queue<GameObject>());
                }
            }
        }

        private void SpawnRandomChunk()
        {
            int randomIndex = Random.Range(0, chunkPrefabs.Length);
            GameObject randomChunkPrefab = chunkPrefabs[randomIndex];
            SpawnChunk(randomChunkPrefab);
        }

        /// <summary>
        /// Retrieves a chunk from the pool (or instantiates a new one) and places it on the road.
        /// </summary>
        private void SpawnChunk(GameObject prefab)
        {
            GameObject chunk;
            if (_chunkPools[prefab].Count > 0)
            {
                chunk = _chunkPools[prefab].Dequeue();
                chunk.SetActive(true);
            }
            else
            {
                chunk = GameObject.Instantiate(prefab);
                Chunk chunkInfo = chunk.AddComponent<Chunk>();
                chunkInfo.OriginalPrefab = prefab;
            }
            chunk.transform.position = new Vector3(0, 0, _spawnZ);
            _spawnZ += chunkLength;
            ObstacleSpawner obstacleSpawner = chunk.GetComponent<ObstacleSpawner>();
            if (obstacleSpawner)
            {
                obstacleSpawner.SpawnContent();
            }
            _activeChunks.Enqueue(chunk);
        }

        /// <summary>
        /// Deactivates the oldest chunk and returns it to its specific pool.
        /// </summary>
        private void RemoveOldChunk()
        {
            GameObject oldChunk = _activeChunks.Dequeue();
            oldChunk.SetActive(false);

            Chunk chunkInfo = oldChunk.GetComponent<Chunk>();
            _chunkPools[chunkInfo.OriginalPrefab].Enqueue(oldChunk);
        }
    }
}