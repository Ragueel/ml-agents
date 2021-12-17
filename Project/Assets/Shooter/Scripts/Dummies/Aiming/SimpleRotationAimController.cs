using UnityEngine;

namespace Shooter.Scripts.Dummies.Aiming
{
    public class SimpleRotationAimController : IAimController
    {
        private Transform _parentTransform;

        public SimpleRotationAimController(Transform parentTransform)
        {
            _parentTransform = parentTransform;
        }

        public void AimAt(Transform target)
        {
            Vector3 difference = _parentTransform.position - target.position;
            _parentTransform.forward = difference.normalized;
        }
    }
}
