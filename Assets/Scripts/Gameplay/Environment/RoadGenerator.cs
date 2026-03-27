using System.Collections.Generic;
using Core;
using Gameplay.Car;
using UnityEngine;
using VContainer;

namespace Gameplay.Environment
{
    public class RoadGenerator : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private RoadSegment _startSegmentPrefab;
        [SerializeField] private RoadSegment _middleSegmentPrefab;
        [SerializeField] private RoadSegment _finishSegmentPrefab;
        
        private IGameStateProvider _stateProvider;
        private GameSettings _settings;
        private CarMovement _car;

        private readonly Queue<RoadSegment> _activeSegments = new();
        private float _nextSpawnZ;
        private bool _finishSpawned;
        private float _actualFinishZ;
        
        public float ActualFinishZ => _actualFinishZ;
        public float TotalLevelDistance => _settings.LevelDistance;

        [Inject]
        public void Construct(IGameStateProvider stateProvider, GameSettings gameSettings)
        {
            _stateProvider = stateProvider;
            _settings = gameSettings;
        }

        public void SetCar(CarMovement car) => _car = car;

        private void OnEnable()
        {
            _stateProvider.OnStateChanged += HandleStateChanged;
            if (_stateProvider.CurrentState == GameState.ReadyToPlay)
                ResetAndGenerateRoad();
        }

        private void OnDisable() => _stateProvider.OnStateChanged -= HandleStateChanged;

        private void HandleStateChanged(GameState newState)
        {
            if (newState == GameState.ReadyToPlay)
            {
                ResetAndGenerateRoad();
            }
        }

        public void ResetAndGenerateRoad()
        {
            ClearExistingRoad();

            _nextSpawnZ = 0f;
            _finishSpawned = false;
            _actualFinishZ = 9999f;
            
            SpawnSpecificSegment(_startSegmentPrefab);
            
            for (int i = 0; i < _settings.RoadSegmentsInReserve; i++)
            {
                SpawnMiddleSegment();
            }
        }

        private void Update()
        {
            if (_stateProvider.CurrentState != GameState.Gameplay || _car == null) return;
            
            
            if (_activeSegments.Count > 0)
            {
                var firstSegment = _activeSegments.Peek();
                if (_car.transform.position.z > firstSegment.transform.position.z + firstSegment.Length)
                {
                    HandleSegmentCycling();
                }
            }
        }

        private void HandleSegmentCycling()
        {
            var oldSegment = _activeSegments.Dequeue();
            
            if (_nextSpawnZ < _settings.LevelDistance)
            {
                MoveSegmentToFront(oldSegment);
            }
            else if (!_finishSpawned)
            {
                SpawnFinishSequence();
                Destroy(oldSegment.gameObject);
            }
            else
            {
                Destroy(oldSegment.gameObject);
            }
        }

        private void SpawnFinishSequence()
        {
            _finishSpawned = true;
            
            var segment = Instantiate(_finishSegmentPrefab, new Vector3(0, 0, _nextSpawnZ), Quaternion.identity, transform);
            
            _actualFinishZ = _nextSpawnZ + (segment.Length / 2f);
            
            _nextSpawnZ += segment.Length;
            _activeSegments.Enqueue(segment);
        }

        private void MoveSegmentToFront(RoadSegment segment)
        {
            segment.transform.position = new Vector3(0, 0, _nextSpawnZ);
            _nextSpawnZ += segment.Length;
            _activeSegments.Enqueue(segment);
            
            if (segment.TryGetComponent<EnvironmentRandomizer>(out var randomizer))
            {
                randomizer.Randomize();
            }
        }

        private void SpawnMiddleSegment()
        {
            var segment = Instantiate(_middleSegmentPrefab, new Vector3(0, 0, _nextSpawnZ), Quaternion.identity, transform);
            MoveSegmentToFront(segment);
        }

        private void SpawnSpecificSegment(RoadSegment prefab)
        {
            var segment = Instantiate(prefab, new Vector3(0, 0, _nextSpawnZ), Quaternion.identity, transform);
            _nextSpawnZ += segment.Length;
            _activeSegments.Enqueue(segment);
        }

        private void ClearExistingRoad()
        {
            while (_activeSegments.Count > 0)
            {
                var segment = _activeSegments.Dequeue();
                if (segment != null) Destroy(segment.gameObject);
            }
        }
    }
}