using UnityEngine;
using UnityEngine.Pool;

namespace Gameplay.VFX
{
    [RequireComponent(typeof(ParticleSystem))]
    public class PooledParticle : MonoBehaviour
    {
        private ParticleSystem _particleSystem;
        private IObjectPool<PooledParticle> _pool;

        private void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
            var main = _particleSystem.main;
            main.stopAction = ParticleSystemStopAction.Callback;
        }

        public void Init(IObjectPool<PooledParticle> pool)
        {
            _pool = pool;
        }

        public void Play(Vector3 position, Color color)
        {
            transform.position = position;
            
            var main = _particleSystem.main;
            main.startColor = color;

            _particleSystem.Play(true);
        }
        
        private void OnParticleSystemStopped()
        {
            if (gameObject.activeInHierarchy)
            {
                _pool?.Release(this);
            }
        }
    }
}