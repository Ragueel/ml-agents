using System;
using UnityEngine;

namespace Shooter.Scripts
{
    [Serializable]
    public class AgentStats
    {
        [SerializeField] private int _maxHp = 20;
        [SerializeField] private float _shootDelay = 0.2f;
        [SerializeField] private int _defaultDamageAmount = 5;
        [SerializeField] private float _movementSpeed = 10f;
        [SerializeField] private float _rotationSpeed = 10f;

        public float LastShotTime { get; set; }
        public int CurrentHp { get; set; }

        public int DefaultDamageAmount => _defaultDamageAmount;

        public float ShootDelay => _shootDelay;

        public float MovementSpeed => _movementSpeed;

        public float RotationSpeed => _rotationSpeed;

        public bool IsDead()
        {
            return CurrentHp <= 0;
        }

        public bool CanShoot()
        {
            return LastShotTime > ShootDelay;
        }

        public void Reset()
        {
            CurrentHp = _maxHp;
            LastShotTime = 0f;
        }

        public void Tick()
        {
            LastShotTime += Time.deltaTime;
        }
    }
}
