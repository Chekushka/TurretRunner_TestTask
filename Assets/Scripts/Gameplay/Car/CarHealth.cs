using UnityEngine;
using UnityEngine.UI;
using VContainer;
using Common;
using Core;

namespace Gameplay.Car
{
    public class CarHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private int _maxHealth = 500;
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private DamageVisualizer _damageVisualizer;

        private int _currentHealth;
        private IGameStateProvider _stateProvider;

        [Inject]
        public void Construct(IGameStateProvider stateProvider)
        {
            _stateProvider = stateProvider;
        }

        private void Start()
        {
            _currentHealth = _maxHealth;
            UpdateUI();
        }

        public void TakeDamage(int amount)
        {
            if (_stateProvider.CurrentState != GameState.Gameplay) return;

            _currentHealth -= amount;
            UpdateUI();
            
            if (_damageVisualizer != null) _damageVisualizer.PlayHitEffect();

            if (_currentHealth <= 0)
            {
                _stateProvider.ChangeState(GameState.Lost);
            }
        }

        private void UpdateUI()
        {
            if (_healthSlider != null)
                _healthSlider.value = (float)_currentHealth / _maxHealth;
        }
    }
}