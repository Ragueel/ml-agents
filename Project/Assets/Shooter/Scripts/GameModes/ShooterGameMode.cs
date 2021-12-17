using UnityEngine;

namespace Shooter.Scripts.GameModes
{
    public class ShooterGameMode : MonoBehaviour
    {
        private int _agentCount = 0;

        public static ShooterGameMode Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        public void Reset()
        {
            _agentCount = 2;
        }

        public void RemoveAgent()
        {
            _agentCount--;
        }

        public bool IsFinished()
        {
            return _agentCount == 1;
        }

        public void FinishEpisode()
        {
            var agents = FindObjectsOfType<ShooterAgent>();
            foreach (var baseShooterAgent in agents)
            {
                baseShooterAgent.EndEpisode();
            }
        }
    }
}
