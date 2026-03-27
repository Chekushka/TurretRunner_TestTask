using System;
using System.Collections.Generic;
using Core;
using Gameplay.Car;
using UnityEngine;
using VContainer;

namespace Gameplay.Environment
{
    public class RoadGenerator : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _levelDistance = 200f;
        [SerializeField] private int _segmentsInReserve = 5;

        [Header("Prefabs")]
        [SerializeField] private RoadSegment _startSegment;
        [SerializeField] private RoadSegment _middleSegmentPrefab;
        [SerializeField] private RoadSegment _finishSegment;

        public float LevelDistance => _levelDistance;
        private IGameStateProvider _stateProvider;
        private readonly Queue<RoadSegment> _activeSegments = new();
        private IObjectResolver _container;
        private CarMovement _car;
    
        private float _nextSpawnZ;
        private bool _finishSpawned;

        [Inject]
        public void Construct(IGameStateProvider stateProvider)
        {
            _stateProvider = stateProvider;
        }
        public void SetCar(CarMovement car) => _car = car;

        private void OnEnable()
        {
            _stateProvider.OnStateChanged += HandleStateChanged;
            if(_stateProvider.CurrentState == GameState.ReadyToPlay)
                ResetAndGenerateRoad();
        }

        private void OnDisable()
        {
            _stateProvider.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState newState)
        {
            if (newState == GameState.ReadyToPlay)
            {
                ResetAndGenerateRoad();
            }
        }

        private void ResetAndGenerateRoad()
        {
            ClearExistingRoad();

            _nextSpawnZ = 0f;
            _finishSpawned = false;

            SpawnSpecificSegment(_startSegment);
        
            for (int i = 0; i < _segmentsInReserve; i++)
            {
                SpawnMiddleSegment();
            }
        }

        private void ClearExistingRoad()
        {
            while (_activeSegments.Count > 0)
            {
                var segment = _activeSegments.Dequeue();
                if (segment != null)
                {
                    Destroy(segment.gameObject);
                }
            }
        }

        private void Update()
        {
            if (_stateProvider.CurrentState != GameState.Gameplay) return;
            
            if (_car.transform.position.z > _activeSegments.Peek().transform.position.z + _activeSegments.Peek().Length)
            {
                HandleSegmentCycling();
            }
        }

        private void HandleSegmentCycling()
        {
            var oldSegment = _activeSegments.Dequeue();
        
            if (_nextSpawnZ < _levelDistance)
            {
                MoveSegmentToFront(oldSegment);
            }
            else if (!_finishSpawned)
            {
                SpawnSpecificSegment(_finishSegment);
                _finishSpawned = true;
                Destroy(oldSegment.gameObject);
            }
            else
            {
                Destroy(oldSegment.gameObject);
            }
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
    }
}