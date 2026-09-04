using UnityEngine;

namespace Gameplay.Traffic
{
    public class ObstacleSpawner : MonoBehaviour
    {
        [Header("Spawn Points")]
        [SerializeField] private Transform[] lanePoints;

        [Header("Settings")]
        [Range(0f, 1f)]
        [SerializeField] private float chanceToSpawnSomething = 0.8f;

        [SerializeField] private float zRandomOffset = 7f;

        // public void OnEnable()
        // {
        //     SpawnContent();
        // }

        public void SpawnContent()
        {
            for (var i = 0; i < lanePoints.Length; i++)
            {
                if (Random.value > chanceToSpawnSomething)
                    continue;

                Vector3 spawnPosition = lanePoints[i].position;
                spawnPosition.z += Random.Range(-zRandomOffset, zRandomOffset);

                SpawnCar(spawnPosition, i);
            }
        }

        private void SpawnCar(Vector3 spawnPosition, int index)
        {
            GameObject carObj = TrafficManager.Instance.GetRandomFreeCar();
            if (carObj == null) return;

            // if (!carObj.TryGetComponent<TrafficCar>(out TrafficCar carScript))
            // {
            //     carScript = carObj.AddComponent<TrafficCar>();
            // }
            TrafficCar carScript = carObj.GetComponent<TrafficCar>();

            bool isOncoming = index < (lanePoints.Length / 2);

            float speed = Random.Range(5f, 12f);

            CarBehavior behavior;
            if ((index == 0 || index == lanePoints.Length - 1) && Random.value < 0.2f)
            {
                behavior = CarBehavior.Parking;
            }
            else if (Random.value < 0.2f)
            {
                behavior = CarBehavior.LaneChange;
            }
            else
            {
                behavior = CarBehavior.Standard;
            }

            carScript.Init(spawnPosition, isOncoming, speed, behavior);
        }
    }
}