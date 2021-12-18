using Shooter.Scripts.Shooting.Projectiles;
using UnityEngine;

namespace Shooter.Scripts.Dummies
{
    public class AvoidingDummy : BaseShooterDummy
    {
        [SerializeField] private float _checkDistance = 2f;
        [SerializeField] private float _strafeSpeed = 25f;
        [SerializeField] private float _checkRadius = 1f;

        protected override void TargetedUpdate()
        {
            base.TargetedUpdate();

            Ray ray = new Ray(_shootPoint.position, _shootPoint.forward);

            if (Physics.SphereCast(ray, _checkRadius, out RaycastHit hit, _checkDistance))
            {
                if (hit.collider.CompareTag("Projectile"))
                {
                    var projectileController = hit.collider.gameObject.GetComponent<ProjectileController>();

                    if (projectileController.GetOwnerId() == GetHashCode())
                    {
                        return;
                    }

                    transform.position += transform.right * (_strafeSpeed * Time.fixedDeltaTime);
                }
            }
        }
    }
}
