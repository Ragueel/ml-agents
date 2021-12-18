using Shooter.Scripts.Shooting.Projectiles;
using UnityEngine;

namespace Shooter.Scripts
{
    public class RaycastObservationCollector
    {
        private int _segments = 50;
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

            float anglePerSegment = 360f / _segments;
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

            RaycastData[] resultRaycastData = new RaycastData[_segments];

            for (int i = 0; i < _segments; i++)
            {
                var raycastData = new RaycastData();
                var checkDirection = vectors[i];

                Vector3 offsetedOrigin = observationCollectionData.Position + checkDirection + Vector3.up * 0.6f;
                Ray ray = new Ray(offsetedOrigin, checkDirection);

                raycastData.Direction = checkDirection;
                raycastData.CheckPosition = offsetedOrigin;

                if (Physics.SphereCast(ray, _sphereCastRadius, out RaycastHit hit, 500,
                    observationCollectionData.Layer))
                {
                    if (hit.collider.CompareTag("agent"))
                    {
                        raycastData.HitType = Constants.HitTypes.Agent;
                    }

                    if (hit.collider.CompareTag("Projectile"))
                    {
                        var projectileController = hit.collider.gameObject.GetComponent<ProjectileController>();

                        if (projectileController.GetOwnerId() != 0)
                        {
                            raycastData.HitType = Constants.HitTypes.Projectile;
                        }
                        else
                        {
                            raycastData.HitType = Constants.HitTypes.None;
                        }
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
}
