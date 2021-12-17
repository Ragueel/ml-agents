using Shooter.Scripts.Shooting.Projectiles;

namespace Shooter.Scripts
{
    public interface IDamageable
    {
        void TakeDamage(ProjectileData projectileData);
    }
}
