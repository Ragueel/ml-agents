using UnityEngine;

namespace Shooter.Scripts
{
    public class TankSkinController : MonoBehaviour
    {
        [SerializeField] private Renderer[] _renderers;

        public void SetColor(Color color)
        {
            foreach (var renderer1 in _renderers)
            {
                renderer1.materials[0].color = color;
            }
        }
    }
}
