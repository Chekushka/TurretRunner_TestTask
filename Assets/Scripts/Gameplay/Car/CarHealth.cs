using UnityEngine;
using UnityEngine.UI;
using VContainer;
using Common;
using Core;
using Gameplay.VFX;

namespace Gameplay.Car
{
    [RequireComponent(typeof(CarVisuals))]
    public class CarHealth : MonoBehaviour, IDamageable
    {
        [Header("Settings")]
        [SerializeField] private Slider _healthSlider;

        private int _currentHealth;
        private IGameStateProvider _stateProvider;
        private CarVisuals _visuals;
        private GameSettings _settings;

        [Inject]
        public void Construct(IGameStateProvider stateProvider, GameSettings settings)
        {
            _stateProvider = stateProvider;
            _settings = settings;
        }
        
        private void Awake()
        {
            _visuals = GetComponent<CarVisuals>();
        }
        
        private void OnEnable() => _stateProvider.OnStateChanged += HandleStateChanged;
        private void OnDisable() => _stateProvider.OnStateChanged -= HandleStateChanged;
        
        private void HandleStateChanged(GameState newState)
        {
            if (newState == GameState.ReadyToPlay)
            {
                ResetCar();
            }
        }
        
        private void Start() => ResetCar();

        public void TakeDamage(int amount, bool isCritical = false)
        {
            if (_stateProvider.CurrentState != GameState.Gameplay || _currentHealth <= 0) return;

            _currentHealth -= amount;
            UpdateUI();
            _visuals.DamageVisualizer.PlayHitEffect();

            if (_currentHealth <= 0)
            {
                _visuals.SetDestructionVisuals();
                _stateProvider.ChangeState(GameState.Lost);
            }
        }
        
        private void ResetCar()
        {
            _currentHealth = _settings.CarMaxHealth;
            _visuals.ResetVisuals();
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (_healthSlider != null)
                _healthSlider.value = (float)_currentHealth / _settings.CarMaxHealth;
        }
    }
}