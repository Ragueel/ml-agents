using UnityEngine;

namespace Shooter.Scripts.Dummies
{
    public class FollowingDummy : BaseShooterDummy
    {
        protected override void TargetedUpdate()
        {
            base.TargetedUpdate();

            MoveToAgent();
        }

        private void MoveToAgent()
        {
            var position = transform.position;

            Vector3 direction = (position - _target.transform.position).normalized;
            position += direction * (Time.fixedDeltaTime * _agentStats.MovementSpeed);

            transform.position = position;
        }
    }
}
