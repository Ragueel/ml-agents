using UnityEngine;

namespace Shooter.Scripts.Dummies
{
    public abstract class BaseShooterDummy : MonoBehaviour
    {
        [SerializeField] protected AgentStats _agentStats;
        [SerializeField] protected ProjectileController _projectilePrefab;

        [SerializeField] protected Transform _shootPoint;

        private Rigidbody _rb;
        private Collider _collider;
        private IShooterController _shooterController;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();

            _shooterController = new SimpleProjectileShooterController(_projectilePrefab);
        }

        public void InitializeDummy()
        {
        }
    }
}
