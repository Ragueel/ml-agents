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

            Vector3 direction = (_target.transform.position - position).normalized;
            position += direction * (Time.fixedDeltaTime * _agentStats.MovementSpeed);

            if (Vector3.Distance(_target.transform.position, position) > 3f)
            {
                transform.position = position;
            }
        }
    }
}
