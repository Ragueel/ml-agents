using System.Collections.Generic;
using Shooter.Scripts.Dummies;
using UnityEngine;

namespace Shooter.Scripts.GameModes
{
    public class ShooterGameMode : MonoBehaviour
    {
        [SerializeField] private Color[] _teamColors;
        [SerializeField] private BaseShooterDummy[] _dummyPrefabs;
        [SerializeField] private bool _isTrainMode = true;
        [SerializeField] private GameObject _endGameUI;

        private int _agentCount = 0;

        public static ShooterGameMode Instance { get; private set; }

        private List<BaseShooterDummy> _spawnedDummies = new List<BaseShooterDummy>();
        private int _shooterAgentsCount = 0;

        private void Awake()
        {
            Instance = this;
        }

        public void OnShooterAgentDeath()
        {
            _shooterAgentsCount--;

            if (_shooterAgentsCount <= 1)
            {
                var agents = FindObjectsOfType<ShooterAgent>();

                foreach (var shooterAgent in agents)
                {
                    shooterAgent.enabled = false;
                }

                Instantiate(_endGameUI, Vector3.zero, Quaternion.identity);
            }
        }

        public void OnTimeOut()
        {
            Instantiate(_endGameUI, Vector3.zero, Quaternion.identity);
            var agents = FindObjectsOfType<ShooterAgent>();

            foreach (var shooterAgent in agents)
            {
                Destroy(shooterAgent.gameObject);
            }
        }

        public void Reset()
        {
            if (_isTrainMode)
            {
                SpawnDummies();
            }
            else
            {
                _shooterAgentsCount = FindObjectsOfType<ShooterAgent>().Length;
                AssignColorsToAgents();
            }
        }

        private void AssignColorsToAgents()
        {
            var tankSkins = FindObjectsOfType<TankSkinController>();

            for (int i = 0; i < tankSkins.Length; i++)
            {
                var color = _teamColors[i % _teamColors.Length];

                tankSkins[i].SetColor(color);
            }
        }

        private void SpawnDummies()
        {
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
