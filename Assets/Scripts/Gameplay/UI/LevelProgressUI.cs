using UnityEngine;
using UnityEngine.UI;
using VContainer;
using Core;
using Gameplay.Environment;
using Gameplay.Car;

namespace Gameplay.UI
{
    public class LevelProgressUI : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Slider _progressSlider;

        private const float DistanceMultiplier = 70f;
        private float _totalDistance;
        private IGameStateProvider _stateProvider;
        private RoadGenerator _roadGenerator;
        private CarMovement _car;

        [Inject]
        public void Construct(IGameStateProvider stateProvider, RoadGenerator roadGenerator, CarMovement car)
        {
            _stateProvider = stateProvider;
            _roadGenerator = roadGenerator;
            _car = car;
        }
        
        private void OnEnable() => _stateProvider.OnStateChanged += HandleStateChanged;

        private void OnDisable() => _stateProvider.OnStateChanged -= HandleStateChanged;

        private void HandleStateChanged(GameState newState)
        {
            if (newState == GameState.ReadyToPlay)
            {
               ResetProgress();
            }
        }

        private void Start()
        {
           ResetProgress();
        }

        private void Update()
        {
            if (_stateProvider.CurrentState != GameState.Gameplay || _car == null) return;

            UpdateProgress();
        }

        private void UpdateProgress()
        {
            float currentZ = _car.transform.position.z;
            float progress = Mathf.Clamp01(currentZ / _totalDistance);

            if (_progressSlider != null)
                _progressSlider.value = progress;
        }

        private void ResetProgress()
        {
            if (_progressSlider != null)
            {
                _progressSlider.value = 0f;
                _totalDistance = (_roadGenerator.TotalLevelDistance + 1) * DistanceMultiplier - DistanceMultiplier / 2;
            }
        }
    }
}