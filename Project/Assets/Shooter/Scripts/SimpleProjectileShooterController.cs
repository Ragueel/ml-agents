using UnityEngine;

namespace Shooter.Scripts
{
    public class SimpleProjectileShooterController : IShooterController
    {
        private ProjectileController _projectileControllerPrefab;

        public SimpleProjectileShooterController(ProjectileController projectileControllerPrefab)
        {
            _projectileControllerPrefab = projectileControllerPrefab;
        }

        public void Shoot(ShootParams shootParams)
        {
            var projectile = Object.Instantiate(_projectileControllerPrefab, shootParams.ShootPosition,
                Quaternion.identity);
            projectile.transform.forward = shootParams.ShootDirection;
            projectile.Setup(shootParams.ProjectileData);
        }
    }
}
