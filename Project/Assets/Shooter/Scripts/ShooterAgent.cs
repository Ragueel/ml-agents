﻿using Shooter.Scripts.GameModes;
using Shooter.Scripts.Shooting;
using Shooter.Scripts.Shooting.Projectiles;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Shooter.Scripts
{
    public class ShooterAgent : Agent, IDamageable
    {
        [SerializeField] private int _agentId;
        [SerializeField] private AgentStats _agentStats;
        [SerializeField] private ProjectileController _projectilePrefab;

        [SerializeField] private Transform _shootPoint;

        private RaycastObservationCollector _raycastObsCollector;

        private Rigidbody _rb;
        private Collider _collider;
        public int AgentId => _agentId;
        private IShooterController _shooterController;
        private float _timePassed = 0;

        private void Start()
        {
            _raycastObsCollector = new RaycastObservationCollector();
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();

            _shooterController = new SimpleProjectileShooterController(_projectilePrefab);
        }

        public override void OnEpisodeBegin()
        {
            if (transform.localPosition.y < 0)
            {
                _rb.velocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }

            transform.localPosition = SpawnPointsController.Instance.GetRandomSpawnPoint();

            Debug.Log($"Episode begin {_agentId}");

            _timePassed = 0f;
            _rb.isKinematic = false;
            _collider.enabled = true;

            _agentStats.Reset();

            ShooterGameMode.Instance.Reset();
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            _timePassed += Time.fixedDeltaTime;
            if (_timePassed > 60f)
            {
                _timePassed = 0f;

                AddReward(-1f);
                EndEpisode();

                return;
            }

            if (_agentStats.IsDead())
            {
                return;
            }

            var contActions = actions.ContinuousActions;

            Vector3 controlSignal = Vector3.zero;

            controlSignal.x = contActions[0];
            controlSignal.z = contActions[1];

            float rotation = contActions[2];

            _rb.AddForce(controlSignal * _agentStats.MovementSpeed, ForceMode.Force);
            _rb.AddTorque(new Vector3(0, rotation * _agentStats.RotationSpeed, 0), ForceMode.Force);

            var discreteActions = actions.DiscreteActions;

            bool isShoot = discreteActions[0] > 0;


            if (isShoot)
            {
                print($"Isshoot: {discreteActions[0]}");
            }

            if (_agentStats.CanShoot() &&
                isShoot)
            {
                _agentStats.LastShotTime = 0f;

                _shooterController.Shoot(new ShootParams
                {
                    ShootPosition = _shootPoint.position,
                    ShootDirection = _shootPoint.forward,
                    ProjectileData = new ProjectileData
                    {
                        AuthorId = _agentId,
                        DamageAmount = _agentStats.DefaultDamageAmount
                    }
                });
            }

            _agentStats.Tick();
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var contActionsOut = actionsOut.ContinuousActions;
            contActionsOut[0] = Input.GetAxis("Horizontal");
            contActionsOut[1] = Input.GetAxis("Vertical");
            contActionsOut[2] = Input.GetAxis("Horizontal_2");
            var discreteActions = actionsOut.DiscreteActions;
            discreteActions[0] = Input.GetButton("Fire1") ? 1 : 0;
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            var localPosition = transform.localPosition;
            sensor.AddObservation(localPosition);

            var forward = transform.forward;
            sensor.AddObservation(forward.x);
            sensor.AddObservation(forward.z);

            sensor.AddObservation(_rb.velocity.x);
            sensor.AddObservation(_rb.velocity.z);

            // Agent Stats
            sensor.AddObservation(_agentStats.IsDead());
            sensor.AddObservation(_agentStats.CanShoot());

            var raycastData = _raycastObsCollector.CollectFor(new ObservationCollectionData
            {
                ForwardVector = forward,
                Position = localPosition
            });

            foreach (var data in raycastData)
            {
                sensor.AddObservation(data.Distance);
                sensor.AddObservation(data.HitType);
            }
        }

        private void OnDrawGizmos()
        {
            if (_raycastObsCollector != null)
            {
                var collectedData = _raycastObsCollector.CollectFor(new ObservationCollectionData
                {
                    ForwardVector = transform.forward,
                    Position = transform.position
                });

                foreach (var raycastData in collectedData)
                {
                    if (raycastData.HitType == Constants.HitTypes.Agent)
                    {
                        Gizmos.color = Color.red;
                    }
                    else if (raycastData.HitType == Constants.HitTypes.Projectile)
                    {
                        Gizmos.color = Color.blue;
                    }
                    else
                    {
                        Gizmos.color = Color.white;
                    }

                    Debug.DrawRay(raycastData.CheckPosition, raycastData.Direction * raycastData.Distance);
                    Gizmos.DrawWireSphere(raycastData.CheckPosition + raycastData.Direction * raycastData.Distance,
                        0.5f);
                }
            }
        }

        public void TakeDamage(ProjectileData projectileData)
        {
            _agentStats.CurrentHp -= projectileData.DamageAmount;

            AddReward(-Rewards.HitReward);

            if (_agentStats.IsDead())
            {
                Die(new DieData
                {
                    KillerId = projectileData.AuthorId
                });
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.collider.CompareTag("Obstacle"))
            {
                AddReward(-0.3f);
            }
        }

        private void Die(DieData dieData)
        {
            SetReward(-Rewards.KillReward);

            _rb.isKinematic = true;
            _collider.enabled = false;

            EndEpisode();
        }
    }
}
