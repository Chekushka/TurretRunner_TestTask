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
        [Header("References")]
        [SerializeField] private Slider _progressSlider;

        private const float DistanceMultiplier = 35f;
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

        private void Start()
        {
            if (_progressSlider != null)
                _progressSlider.value = 0f;
        }

        private void Update()
        {
            if (_stateProvider.CurrentState != GameState.Gameplay || _car == null) return;

            UpdateProgress();
        }

        private void UpdateProgress()
        {
            float currentZ = _car.transform.position.z;
            float totalZ = _roadGenerator.TotalLevelDistance * DistanceMultiplier;
            
            float progress = Mathf.Clamp01(currentZ / totalZ);
            
            if (_progressSlider != null)
                _progressSlider.value = progress;
        }
    }
}