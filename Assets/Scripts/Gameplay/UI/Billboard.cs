using UnityEngine;

namespace Gameplay.UI
{
    public class Billboard : MonoBehaviour
    {
        private Transform _mainCameraTransform;

        private void Start()
        {
            if (Camera.main != null) 
                _mainCameraTransform = Camera.main.transform;
        }

        private void LateUpdate()
        {
            if (_mainCameraTransform == null) return;
            
            transform.LookAt(transform.position + _mainCameraTransform.rotation * Vector3.forward,
                _mainCameraTransform.rotation * Vector3.up);
        }
    }
}