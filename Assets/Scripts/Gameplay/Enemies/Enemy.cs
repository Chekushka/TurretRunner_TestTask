using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Pool;
using Common;
using Core;
using Gameplay.VFX;
using JetBrains.Annotations;
using VContainer;

namespace Gameplay.Enemies
{
    public class Enemy : MonoBehaviour, IDamageable
    {
        [Header("References")]
        [SerializeField] private Animator _animator;
        [SerializeField] private DamageVisualizer _damageVisualizer;
        
        [Header("UI")]
        [SerializeField] private GameObject _healthCanvasObject;
        [SerializeField] private Slider _healthSlider;

        private static readonly int SpeedHash = Animator.StringToHash("Speed");
        private static readonly int HitHash = Animator.StringToHash("Hit");

        private Transform _targetTransform;
        private IDamageable _targetHealth;
        private GameSettings _settings;
        private IObjectPool<Enemy> _pool;
        private IObjectPool<PooledParticle> _deathParticlePool;
        private IObjectPool<DamagePopup> _damagePopupPool;
        
        private int _currentHealth;
        private EnemyState _currentState;
        private float _stunEndTime;
       

        public void Init(Transform targetTransform, IDamageable targetHealth, IObjectPool<Enemy> pool, IObjectPool<PooledParticle> deathParticlePool, [CanBeNull] IObjectPool<DamagePopup> damagePopupPool, GameSettings settings)
        {
            _targetTransform = targetTransform;
            _targetHealth = targetHealth;
            _pool = pool;
            _deathParticlePool = deathParticlePool;
            _damagePopupPool = damagePopupPool;
            
            _settings =  settings;
            
            _currentHealth = _settings.EnemyMaxHealth;
            _currentState = EnemyState.Idle;
            
            _healthCanvasObject.SetActive(false);
        }

        private void Update()
        {
            if (_currentState == EnemyState.Dead || _targetTransform == null) return;

            if (transform.position.z < _targetTransform.position.z - 15f)
            {
                ReturnToPool();
                return;
            }

            HandleLogic();
        }

        private void HandleLogic()
        {
            if (_currentState == EnemyState.Stunned)
            {
                if (Time.time >= _stunEndTime) _currentState = EnemyState.Idle; 
                else return; 
            }

            float distance = Vector3.Distance(transform.position, _targetTransform.position);

            if (distance <= _settings.AttackRadius)
            {
                KamikazeAttack();
            }
            else if (distance <= _settings.DetectionRadius)
            {
                ChangeState(EnemyState.Running);
                MoveTowardsTarget();
            }
            else
            {
                ChangeState(EnemyState.Idle);
            }
        }

        private void KamikazeAttack()
        {
            _targetHealth?.TakeDamage(_settings.EnemyDamageToCar);
            Die();
        }

        private void ChangeState(EnemyState newState)
        {
            if (_currentState == newState) return;
            _currentState = newState;

            switch (_currentState)
            {
                case EnemyState.Idle:
                    _animator.SetFloat(SpeedHash, 0f);
                    break;
                case EnemyState.Running:
                    _animator.SetFloat(SpeedHash, 1f);
                    break;
                case EnemyState.Stunned:
                    _animator.SetTrigger(HitHash);
                    _animator.SetFloat(SpeedHash, 0f);
                    break;
            }
        }

        private void MoveTowardsTarget()
        {
            Vector3 direction = (_targetTransform.position - transform.position).normalized;
            direction.y = 0;
            
            transform.position += direction * (_settings.EnemySpeed * Time.deltaTime);
            
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, 
                    Quaternion.LookRotation(direction), Time.deltaTime * 10f);
            }
        }

        public void TakeDamage(int amount, bool isCritical = false)
        {
            if (_currentState == EnemyState.Dead) return;

            _currentHealth -= amount;
            if (_damagePopupPool != null)
            {
                DamagePopup popup = _damagePopupPool.Get();
                popup.Setup(amount, transform.position, isCritical);
            }
            UpdateHealthUI();
            
            if (_damageVisualizer != null) _damageVisualizer.PlayHitEffect();

            if (_currentHealth <= 0)
            {
                Die();
            }
            else
            {
                ApplyStun();
            }
        }

        private void UpdateHealthUI()
        {
            if (!_healthCanvasObject.activeSelf) _healthCanvasObject.SetActive(true);
            _healthSlider.value = (float)_currentHealth / _settings.EnemyMaxHealth;
        }

        private void ApplyStun()
        {
            ChangeState(EnemyState.Stunned);
            _stunEndTime = Time.time + _settings.StunDuration;
        }

        private void Die()
        {
            _currentState = EnemyState.Dead;
            
            if (_deathParticlePool != null)
            {
                PooledParticle effect = _deathParticlePool.Get();
                Color stickmanColor = _damageVisualizer.GetOriginalColor();
                effect.Play(transform.position, stickmanColor);
            }

            ReturnToPool();
        }

        private void ReturnToPool()
        {
            _pool.Release(this);
        }
    }
}