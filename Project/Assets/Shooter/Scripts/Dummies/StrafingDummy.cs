using UnityEngine;

namespace Shooter.Scripts.Dummies
{
    public class StrafingDummy : BaseShooterDummy
    {
        private float _movementTime = 1.5f;
        private float _timePassed = 0f;

        private Vector3 _leftPosition;
        private Vector3 _rightPosition;

        private int movementCounter = 0;
        private Vector3 _currentVelocity = Vector3.zero;


        public override void InitializeDummy()
        {
            base.InitializeDummy();

            var transform1 = transform;
            var right = transform1.right;
            var position = transform1.position;

            _leftPosition = position - right * 4f;
            _rightPosition = position + right * 4f;
        }

        protected override void TargetedUpdate()
        {
            base.TargetedUpdate();

            Vector3 target = movementCounter % 2 == 0 ? _leftPosition : _rightPosition;


            transform.position = Vector3.SmoothDamp(transform.position, target, ref _currentVelocity, 0.3f, 100.6f,
                Time.fixedDeltaTime);

            _timePassed += Time.fixedDeltaTime;

            if (_timePassed < _movementTime)
            {
                _timePassed = 0;
                movementCounter++;
            }
        }
    }
}
