using UnityEngine;
using System.Collections.Generic;

namespace Gameplay.Traffic
{
    public class TrafficManager : MonoBehaviour
    {
        public static TrafficManager Instance { get; private set; }

        [SerializeField] private GameObject[] carPrefabs;
        private List<GameObject> _freeCars = new List<GameObject>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            InitializePool();
        }

        private void InitializePool()
        {
            foreach (var prefab in carPrefabs)
            {
                if (prefab == null)
                {
                    Debug.LogWarning("A null prefab was found in the carPrefabs array.");
                    continue;
                }

                GameObject go = Instantiate(prefab);
                go.SetActive(false);
                _freeCars.Add(go);
                Debug.Log("Car added to the pool.");
            }
        }

        public GameObject GetRandomFreeCar()
        {
            if (_freeCars.Count == 0)
            {
                Debug.LogWarning("No free cars available in the pool.");
                return null;
            }

            int randomIndex = Random.Range(0, _freeCars.Count);
            GameObject carToSpawn = _freeCars[randomIndex];
            _freeCars.RemoveAt(randomIndex);
            carToSpawn.SetActive(true);
            return carToSpawn;
        }

        public void ReturnCarToPool(GameObject car)
        {
            if (!car) return;

            car.SetActive(false);
            _freeCars.Add(car);
        }
    }
}

