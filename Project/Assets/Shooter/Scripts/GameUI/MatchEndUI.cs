using UnityEngine;
using UnityEngine.UI;

namespace Shooter.Scripts.GameUI
{
    public class MatchEndUI : MonoBehaviour
    {
        [SerializeField] private Button _restartBtn;
        [SerializeField] private Button _exitBtn;

        private void Start()
        {
            _restartBtn.onClick.AddListener(RestartClicked);
            _exitBtn.onClick.AddListener(ExitClicked);
        }

        private void RestartClicked()
        {
        }

        private void ExitClicked()
        {
        }
    }
}
