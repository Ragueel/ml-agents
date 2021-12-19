using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Shooter.Scripts.GameUI
{
    public class MainMenuUIController : MonoBehaviour
    {
        [SerializeField] private Button _playBtn;
        [SerializeField] private Button _exitBtn;


        private void Start()
        {
            _playBtn.onClick.AddListener(OnPlayClick);
            _exitBtn.onClick.AddListener(OnExitClick);
        }

        private void OnExitClick()
        {
            Application.Quit();
        }

        private void OnPlayClick()
        {
            SceneManager.LoadScene(GameScenes.GamePlay);
        }
    }
}
