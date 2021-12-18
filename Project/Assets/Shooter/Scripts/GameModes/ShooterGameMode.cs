using System.Collections.Generic;
using Shooter.Scripts.Dummies;
using UnityEngine;

namespace Shooter.Scripts.GameModes
{
    public class ShooterGameMode : MonoBehaviour
    {
        [SerializeField] private BaseShooterDummy[] _dummyPrefabs;
        [SerializeField] private bool _isTrainMode = true;

        private int _agentCount = 0;

        public static ShooterGameMode Instance { get; private set; }

        private List<BaseShooterDummy> _spawnedDummies = new List<BaseShooterDummy>();

        private void Awake()
        {
            Instance = this;
        }

        public void Reset()
        {
            if (!_isTrainMode)
            {
                return;
            }

            foreach (var baseShooterDummy in _spawnedDummies)
            {
                if (baseShooterDummy != null)
                {
                    Destroy(baseShooterDummy.gameObject);
                }
            }

            _spawnedDummies.Clear();

            int dummyCount = Random.Range(1, 4);

            for (int i = 0; i < dummyCount; i++)
            {
                var randomDummy = _dummyPrefabs[Random.Range(0, _dummyPrefabs.Length)];

                var tempDummy = Instantiate(randomDummy,
                    SpawnPointsController.Instance.GetRandomSpawnPoint(),
                    Quaternion.identity);

                tempDummy.InitializeDummy();

                _spawnedDummies.Add(tempDummy);
            }

            _agentCount = dummyCount + 1;
        }

        public void RemoveAgent()
        {
            _agentCount--;

            if (_agentCount == 1)
            {
                var agent = FindObjectOfType<ShooterAgent>();

                agent.EndEpisode();
            }
        }

        public bool IsFinished()
        {
            return _agentCount == 1;
        }
    }
}
