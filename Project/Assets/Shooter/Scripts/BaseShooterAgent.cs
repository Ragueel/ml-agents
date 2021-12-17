using System.Linq;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Shooter.Scripts
{
    public class BaseShooterAgent : Agent
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

            _agentStats.Reset();
            GameMode.Instance.Reset();
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
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
            _rb.AddTorque(new Vector3(0, rotation, 0), ForceMode.Force);


            var discreteActions = actions.DiscreteActions;

            bool isShoot = discreteActions[0] > 0;
            if (isShoot)
            {
                print("Isshoot");
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

            if (GameMode.Instance.IsFinished())
            {
                AddReward(2f);
                EndEpisode();
            }
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

        public void AddHitBonus()
        {
            AddReward(Rewards.HitReward);
        }

        public void AddKillBonus()
        {
            AddReward(Rewards.KillReward);
        }

        private static BaseShooterAgent GetAgentWithId(int id)
        {
            var agents = GameObject.FindGameObjectsWithTag("agent").Select(a => a.GetComponent<BaseShooterAgent>());

            foreach (var baseShooterAgent in agents)
            {
                if (baseShooterAgent.AgentId == id)
                {
                    return baseShooterAgent;
                }
            }

            return null;
        }

        private void OnDrawGizmos()
        {
            if (_raycastObsCollector != null)
            {
                _raycastObsCollector.CollectFor(new ObservationCollectionData
                {
                    ForwardVector = transform.forward,
                    Position = transform.position
                });
            }
        }

        public void TakeDamage(ProjectileData projectileData)
        {
            var agent = GetAgentWithId(projectileData.AuthorId);

            if (agent)
            {
                agent.AddHitBonus();
            }

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

        private void Die(DieData dieData)
        {
            SetReward(-Rewards.KillReward);

            _rb.isKinematic = true;
            _collider.enabled = false;

            var agent = GetAgentWithId(dieData.KillerId);

            if (agent)
            {
                agent.AddKillBonus();
            }

            GameMode.Instance.RemoveAgent();
        }
    }

    public struct RaycastData
    {
        public float Distance;
        public int HitType;
    }

    public static class Constants
    {
        public static class HitTypes
        {
            public const int None = 0;
            public const int Agent = 1;
            public const int Projectile = 2;
            public const int Obstacle = 4;
        }
    }

    public class RaycastObservationCollector
    {
        private int _segments = 20;

        private float _sphereCastRadius = 0.5f;

        public static float CalculateAngle(Vector3 v, Vector3 forward, Vector3 axis, bool clockwise = false)
        {
            Vector3 right;
            if (clockwise)
            {
                right = Vector3.Cross(forward, axis);
                forward = Vector3.Cross(axis, right);
            }
            else
            {
                right = Vector3.Cross(axis, forward);
                forward = Vector3.Cross(right, axis);
            }

            return Mathf.Atan2(Vector3.Dot(v, right), Vector3.Dot(v, forward)) * Mathf.Rad2Deg;
        }

        private Vector3[] GetRaycastVectors(Vector3 forwardVector)
        {
            Vector3[] directions = new Vector3[_segments];
            float offsetAngle = Vector3.Angle(Vector3.right, forwardVector);

            float anglePerSegment = 180f / _segments;
            float tempAngle = 0;

            for (int i = 0; i < _segments; i++)
            {
                float x;
                float y;

                float angle = (tempAngle + offsetAngle - 90);
                if (forwardVector.z < 0f)
                {
                    angle *= -1f;
                }

                x = Mathf.Cos(angle * Mathf.Deg2Rad);
                y = Mathf.Sin(angle * Mathf.Deg2Rad);
                Vector3 direction = new Vector3(x, 0, y);

                directions[i] = direction;

                tempAngle += anglePerSegment;
            }

            return directions;
        }

        public RaycastData[] CollectFor(ObservationCollectionData observationCollectionData)
        {
            var vectors = GetRaycastVectors(observationCollectionData.ForwardVector);

            Debug.DrawRay(observationCollectionData.Position, observationCollectionData.ForwardVector * 5, Color.green);

            foreach (var vector3 in vectors)
            {
                Debug.DrawLine(observationCollectionData.Position,
                    observationCollectionData.Position + vector3 * 5,
                    Color.red);
            }

            RaycastData[] resultRaycastData = new RaycastData[_segments];

            for (int i = 0; i < _segments; i++)
            {
                var raycastData = new RaycastData();
                var checkDirection = vectors[i];

                Vector3 offsetedOrigin = observationCollectionData.Position + checkDirection;
                Ray ray = new Ray(offsetedOrigin, checkDirection);

                if (Physics.SphereCast(ray, _sphereCastRadius, out RaycastHit hit))
                {
                    if (hit.collider.CompareTag("agent"))
                    {
                        raycastData.HitType = Constants.HitTypes.Agent;
                    }

                    if (hit.collider.CompareTag("Projectile"))
                    {
                        raycastData.HitType = Constants.HitTypes.Projectile;
                    }

                    if (hit.collider.CompareTag("Obstacle"))
                    {
                        raycastData.HitType = Constants.HitTypes.Obstacle;
                    }

                    raycastData.Distance = Vector3.Distance(hit.point, observationCollectionData.Position);
                }

                resultRaycastData[i] = raycastData;
            }

            return resultRaycastData;
        }
    }

    public class ObservationCollectionData
    {
        public Vector3 Position;
        public Vector3 ForwardVector;
    }
}
