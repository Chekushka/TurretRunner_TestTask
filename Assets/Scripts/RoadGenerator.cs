using System.Collections.Generic;
using UnityEngine;
using VContainer;

public class RoadGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _levelDistance = 200f;
    [SerializeField] private int _segmentsInReserve = 5;

    [Header("Prefabs")]
    [SerializeField] private RoadSegment _startSegment;
    [SerializeField] private RoadSegment _middleSegmentPrefab;
    [SerializeField] private RoadSegment _finishSegment;
    [SerializeField] private Transform _carTransform;

    private IGameStateProvider _stateProvider;
    private readonly Queue<RoadSegment> _activeSegments = new();
    
    private float _nextSpawnZ;
    private float _distanceCovered;
    private bool _finishSpawned;

    [Inject]
    public void Construct(IGameStateProvider stateProvider)
    {
        _stateProvider = stateProvider;
    }

    private void Start()
    {
        SpawnSpecificSegment(_startSegment);
        
        for (int i = 0; i < _segmentsInReserve; i++)
        {
            SpawnMiddleSegment();
        }
    }

    private void Update()
    {
        if (_stateProvider.CurrentState != GameState.Gameplay) return;
        
        if (_carTransform.position.z > _activeSegments.Peek().transform.position.z + _activeSegments.Peek().Length)
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