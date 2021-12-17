using UnityEngine;

namespace Shooter.Scripts
{
    public class GameMode : MonoBehaviour
    {
        private int _agentCount = 0;

        public static GameMode Instance { get; private set; }

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
    }
}
