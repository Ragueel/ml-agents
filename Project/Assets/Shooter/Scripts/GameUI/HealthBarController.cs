using UnityEngine;

namespace Shooter.Scripts.GameUI
{
    public class HealthBarController : MonoBehaviour
    {
        [SerializeField] private Transform _healthBar;

        public void SetHealth(float health)
        {
            _healthBar.localScale = new Vector3(Mathf.Clamp01(health), 1f, 1f);
        }
    }
}
