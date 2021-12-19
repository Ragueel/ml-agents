using UnityEngine;

namespace Shooter.Scripts.GameUI
{
    public class BillBoard : MonoBehaviour
    {
        private Transform _cameraTransform;

        private void Start()
        {
            _cameraTransform = Camera.main.transform;
        }


        private void Update()
        {
            if (_cameraTransform == null)
            {
                return;
            }

            transform.forward = _cameraTransform.forward;
        }
    }
}
