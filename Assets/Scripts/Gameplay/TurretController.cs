using System;
using Common;
using Core;
using DG.Tweening;
using Infrastructure;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;

namespace Gameplay
{
    public class TurretController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform _turretHead;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private ParticleSystem _muzzleFlash;
        [SerializeField] private LineRenderer _laserLine;
        [SerializeField] private Bullet _bulletPrefab;

        [Header("Combat Settings")] 
        [SerializeField] private float _fireRate = 0.15f;
        [SerializeField] private int _damage = 34;
        [SerializeField] private float _range = 40f;
        [SerializeField] private float hitboxRadius = 0.5f;
        [SerializeField] private LayerMask _enemyLayer;
        
        [Header("Movement and Feel")]
        [SerializeField] private float _rotationSpeed = 15f;
        [SerializeField] private float _maxAngle = 70f;
        [SerializeField] private float _recoilDistance = 0.2f;
        [SerializeField] private Vector3 _punchScale = new Vector3(1.1f, 1.1f, 1.1f);

        private IGameStateProvider _stateProvider;
        private IGameInputProvider _inputProvider;
        private IObjectPool<Bullet> _bulletPool;
        
        private float _currentAngle;
        private float _nextFireTime;
        private Vector3 _initialHeadLocalPosition;

        [Inject]
        public void Construct(IGameStateProvider stateProvider, IGameInputProvider inputProvider)
        {
            _stateProvider = stateProvider;
            _inputProvider = inputProvider;
        }

        private void Awake()
        {
            _initialHeadLocalPosition = _turretHead.localPosition;
            _laserLine.enabled = false;
            
            _bulletPool = new ObjectPool<Bullet>(
                createFunc: () => Instantiate(_bulletPrefab, transform.position, Quaternion.identity),
                actionOnGet: bullet => bullet.gameObject.SetActive(true),
                actionOnRelease: bullet => bullet.gameObject.SetActive(false),
                actionOnDestroy: bullet => Destroy(bullet.gameObject),
                defaultCapacity: 20,
                maxSize: 30
            );
        }

        private void Update()
        {
            if (_stateProvider.CurrentState != GameState.Gameplay)
            {
                _laserLine.enabled = false;
                return;
            }
            
            HandleRotation();
            HandleShooting();
        }
        
        private void HandleRotation()
        {
            if (!_inputProvider.IsTouching) return;
            
            float delta = _inputProvider.SwipeDeltaX * _rotationSpeed * Time.deltaTime;
            _currentAngle = Mathf.Clamp(_currentAngle + delta, -_maxAngle, _maxAngle);
            
            _turretHead.localRotation = Quaternion.Euler(0f, _currentAngle, 0f);
        }

        private void HandleShooting()
        {
            if (_inputProvider.IsTouching)
            {
                DrawLaser();

                if (Time.time >= _nextFireTime)
                {
                    Shoot();
                    _nextFireTime = Time.time + _fireRate;
                }
            }
            else
            {
                _laserLine.enabled = false;
            }
        }

        private void Shoot()
        {
            PlayShootEffects();
            
            Vector3 hitPoint = _firePoint.position + _firePoint.forward * _range;
            IDamageable target = null;
            
            if(Physics.SphereCast(_firePoint.position, hitboxRadius, _firePoint.forward, out RaycastHit hit, _range, _enemyLayer))
            {
                hitPoint = hit.point;
                hit.collider.TryGetComponent(out target);
            }
            
            Bullet bullet = _bulletPool.Get();
            bullet.Init(_firePoint.position, hitPoint, target, _damage, _bulletPool);
        }
        
        private void PlayShootEffects()
        {
            if (_muzzleFlash != null) _muzzleFlash.Play();

            _turretHead.DOKill(true);
            _turretHead.localPosition = _initialHeadLocalPosition;
            
            _turretHead.DOLocalMoveZ(_initialHeadLocalPosition.z - _recoilDistance, _fireRate * 0.4f)
                .SetLoops(2, LoopType.Yoyo);
            _turretHead.DOPunchScale(_punchScale - Vector3.one, _fireRate * 0.8f, 10, 1);
        }

        private void DrawLaser()
        {
            _laserLine.enabled = true;
            _laserLine.SetPosition(0, transform.position);
            
            if(Physics.SphereCast(_firePoint.position, hitboxRadius, _firePoint.forward, out RaycastHit hit, _range, _enemyLayer))
            {
                _laserLine.SetPosition(1, hit.point);
            }
            else
            {
                _laserLine.SetPosition(1, _firePoint.position + _firePoint.forward * _range);
            }
        }
    }
}