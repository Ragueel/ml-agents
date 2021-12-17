using System.Collections;
using UnityEngine;

namespace Shooter.Scripts.Shooting.Projectiles
{
    public class ProjectileController : MonoBehaviour
    {
        [SerializeField] private float _movementSpeed = 9f;
        private ProjectileData _projectileData;

        private float _maxLifeTime = 5f;

        public void Setup(ProjectileData projectileData)
        {
            _projectileData = projectileData;
        }

        private void Start()
        {
            StartCoroutine(Die());
        }

        private IEnumerator Die()
        {
            yield return new WaitForSeconds(_maxLifeTime);

            Destroy(gameObject);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.CompareTag("agent"))
            {
                other.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(_projectileData);
            }

            Destroy(gameObject);
        }

        private void FixedUpdate()
        {
            transform.position += transform.forward * _movementSpeed * Time.fixedDeltaTime;
        }
    }
}
