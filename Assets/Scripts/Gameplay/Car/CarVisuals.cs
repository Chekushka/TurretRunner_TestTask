using Gameplay.VFX;
using UnityEngine;

namespace Gameplay.Car
{
    public class CarVisuals : MonoBehaviour
    {
        [Header("Wheels")]
        [SerializeField] private Transform[] _wheels;
        [SerializeField] private float _wheelRotationSpeed = 300f;

        [Header("Effects")]
        [SerializeField] private ParticleSystem _explosionParticles;
        [SerializeField] private ParticleSystem _fireParticles;
        [SerializeField] private DamageVisualizer _damageVisualizer;

        public void AnimateWheels(float moveSpeed)
        {
            foreach (var wheel in _wheels)
            {
                wheel.Rotate(Vector3.right * (moveSpeed * _wheelRotationSpeed * Time.deltaTime));
            }
        }

        public void SetDestructionVisuals()
        {
            if (_explosionParticles != null)
            {
                _explosionParticles.gameObject.SetActive(true);
                _explosionParticles.Play();
            }
            if (_fireParticles != null)
            {
                _fireParticles.gameObject.SetActive(true);
                _fireParticles.Play();
            }

            if (_damageVisualizer != null)
                _damageVisualizer.SetDeadColor();

            foreach (var wheel in _wheels) wheel.gameObject.SetActive(false);
        }

        public void ResetVisuals()
        {
            if (_fireParticles != null)
            {
                _fireParticles.Stop();
                _fireParticles.gameObject.SetActive(false);
            }
            if (_explosionParticles != null)
                _fireParticles.gameObject.SetActive(false);
            
            if (_damageVisualizer != null) _damageVisualizer.ResetColor();
            foreach (var wheel in _wheels) wheel.gameObject.SetActive(true);
        }

        public DamageVisualizer DamageVisualizer => _damageVisualizer;
    }
}