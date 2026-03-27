using System;
using Common;
using Core;
using UnityEngine;
using UnityEngine.Pool;

namespace Gameplay
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField] private float _speed = 100f;
        [SerializeField] private TrailRenderer _trail;
        
        private IObjectPool<Bullet> _pool;
        private IDamageable _target;
        private int _damage;
        private Vector3 _destination;
        private bool _isTracking;
        private bool _isCritical;
        
        public void Init(Vector3 startPos, Vector3 hitPoint, IDamageable target, int damage, bool isCritical, IObjectPool<Bullet> pool)
        {
            transform.position = startPos;
            _destination = hitPoint;
            _target = target;
            _damage = damage;
            _isCritical = isCritical;
            _pool = pool;
            _isTracking = target != null;
            _trail.Clear();
            _trail.enabled = true;
        }

        private void Update()
        {
            if (_isTracking && _target is MonoBehaviour targetObj && targetObj.gameObject.activeInHierarchy)
            {
                _destination = targetObj.transform.position + Vector3.up * 0.5f;
            }
            
            transform.position = Vector3.MoveTowards(transform.position, _destination, _speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, _destination) < 0.1f)
            {
                HitTarget();
            }
        }

        private void HitTarget()
        {
            if (_isTracking && _target is MonoBehaviour targetObj && targetObj.gameObject.activeInHierarchy)
            {
                _target.TakeDamage(_damage, _isCritical);
            }
            
            _pool.Release(this);
        }
    }
}