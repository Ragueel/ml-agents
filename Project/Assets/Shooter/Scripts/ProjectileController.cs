using UnityEngine;

namespace Shooter.Scripts
{
    public class ProjectileController : MonoBehaviour
    {
        private ProjectileData _projectileData;

        public void Setup(ProjectileData projectileData)
        {
            _projectileData = projectileData;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.CompareTag("agent"))
            {
                other.collider.gameObject.GetComponent<BaseShooterAgent>()?.TakeDamage(_projectileData);
            }

            Destroy(gameObject);
        }
    }
}
