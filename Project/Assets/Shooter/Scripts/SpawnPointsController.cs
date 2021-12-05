using UnityEngine;

namespace Shooter.Scripts
{
    public class SpawnPointsController : MonoBehaviour
    {
        [SerializeField] private Vector2 _rangeX;
        [SerializeField] private Vector2 _rangeZ;

        public static SpawnPointsController Instance;

        private void Awake()
        {
            Instance = this;
        }

        public Vector3 GetRandomSpawnPoint()
        {
            return new Vector3(Random.Range(_rangeX.x, _rangeX.y), 0, Random.Range(_rangeZ.x, _rangeZ.y));
        }
    }
}
