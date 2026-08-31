using UnityEngine;

namespace Core.GameManagment
{
    public class Score : MonoBehaviour
    {
        private static Score Instance { get; set; }
        public int Count { get; private set; }
        [SerializeField] private GameObject player;

        public static int CurrentCount => Instance != null ? Instance.Count : 0;
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
        }

        private void Update()
        {
            Count = (int)player.transform.position.z;
        }
    }
}