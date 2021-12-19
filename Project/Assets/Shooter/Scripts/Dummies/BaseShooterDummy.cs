using Shooter.Scripts.Dummies.Aiming;
using Shooter.Scripts.GameModes;
using Shooter.Scripts.Shooting;
using Shooter.Scripts.Shooting.Projectiles;
using UnityEngine;

namespace Shooter.Scripts.Dummies
{
    public abstract class BaseShooterDummy : MonoBehaviour, IDamageable
    {
        [SerializeField] protected AgentStats _agentStats;
        [SerializeField] protected ProjectileController _projectilePrefab;
        [SerializeField] protected Transform _shootPoint;

        private Rigidbody _rb;
        private Collider _collider;
        private IShooterController _shooterController;
        private IAimController _aimController;
        protected GameObject _target;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();

            _aimController = new SimpleRotationAimController(transform);
            _shooterController = new SimpleProjectileShooterController(_projectilePrefab);
        }

        public virtual void InitializeDummy()
        {
            _target = FindObjectOfType<ShooterAgent>().gameObject;
            _agentStats.Reset();
        }

        protected virtual void FixedUpdate()
        {
            if (_target == null)
            {
                return;
            }

            TargetedUpdate();
        }

        protected virtual void TargetedUpdate()
        {
            AimAndShoot();
        }

        private void AimAndShoot()
        {
            _aimController.AimAt(_target.transform);
            _agentStats.LastShotTime += Time.fixedDeltaTime;

            if (_agentStats.CanShoot())
            {
                _agentStats.LastShotTime = 0;
                _shooterController.Shoot(new ShootParams
                {
                    ProjectileData = new ProjectileData
                    {
                        AuthorId = gameObject.GetHashCode(),
                        DamageAmount = _agentStats.DefaultDamageAmount
                    },
                    ShootDirection = _shootPoint.forward,
                    ShootPosition = _shootPoint.position,
                });
            }
        }

        public void TakeDamage(ProjectileData projectileData)
        {
            if (_target == null)
            {
                return;
            }

            var shooterAgent = _target.GetComponent<ShooterAgent>();

            shooterAgent.AddReward(Rewards.HitReward);

            _agentStats.CurrentHp -= projectileData.DamageAmount;

            if (_agentStats.IsDead())
            {
                shooterAgent.AddReward(Rewards.KillReward);
                print("Killed bot");
                ShooterGameMode.Instance.RemoveAgent();
                Destroy(gameObject);
            }
        }
    }
}
