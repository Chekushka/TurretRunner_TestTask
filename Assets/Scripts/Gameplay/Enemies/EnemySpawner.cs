using Core;
using Gameplay.Car;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;
using Random = UnityEngine.Random;

namespace Gameplay.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Spawn Zone Settings")] [SerializeField]
        private float _chunkLenght = 40f;

        [SerializeField] private int _enemiesPerChunk = 5;
        [SerializeField] private float _spawnAheadOffset = 50f;
        [SerializeField] private float _roadWidth = 3f;

        [Header("Limits")] [SerializeField] private int _maxActiveEnemies = 20;

        [Header("References")] [SerializeField]
        private Enemy _enemyPrefab;

        private IGameStateProvider _stateProvider;
        private CarMovement _car;
        private CarHealth _carHealth;
        private IObjectPool<Enemy> _enemyPool;

        private float _nextChunkZ;
        private int _currentActiveCount;

        [Inject]
        public void Construct(IGameStateProvider stateProvider, CarMovement car, CarHealth carHealth)
        {
            _stateProvider = stateProvider;
            _car = car;
            _carHealth = carHealth;
        }

        private void Awake()
        {
            _enemyPool = new ObjectPool<Enemy>(
                createFunc: () => Instantiate(_enemyPrefab, transform),
                actionOnGet: enemy =>
                {
                    enemy.gameObject.SetActive(true);
                    _currentActiveCount++;
                },
                actionOnRelease: enemy =>
                {
                    enemy.gameObject.SetActive(false);
                    _currentActiveCount--;
                },
                actionOnDestroy: enemy => Destroy(enemy.gameObject),
                defaultCapacity: _maxActiveEnemies,
                maxSize: _maxActiveEnemies + 10
            );
        }

        private void OnEnable()
        {
            _stateProvider.OnStateChanged += HandleStateChanged;
            if(_stateProvider.CurrentState == GameState.ReadyToPlay)
                HandleStateChanged(GameState.ReadyToPlay);
        }
       
        private void OnDisable() => _stateProvider.OnStateChanged -= HandleStateChanged;

        private void HandleStateChanged(GameState state)
        {
            if (state == GameState.ReadyToPlay)
            {
                _nextChunkZ = _car.transform.position.z + 20f;
                SpawnInitialEnemies();
            }
        }

        private void Update()
        {
            if (_stateProvider.CurrentState != GameState.Gameplay) return;

            if (_car.transform.position.z + _spawnAheadOffset >= _nextChunkZ)
            {
                SpawnChunk();
            }
        }

        private void SpawnInitialEnemies()
        {
            SpawnChunk();
        }

        private void SpawnChunk()
        {
            int enemiesToSpawn = Mathf.Min(_enemiesPerChunk, _maxActiveEnemies - _currentActiveCount);

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                float randomX = Random.Range(-_roadWidth, _roadWidth);
                float randomZ = Random.Range(_nextChunkZ, _nextChunkZ + _chunkLenght);
                
                float randomRotationY = Random.Range(0f, 360f);

                Vector3 spawnPos = new Vector3(randomX, 0f, randomZ);
                Quaternion spawnRotation = Quaternion.Euler(0, randomRotationY, 0);

                Enemy enemy = _enemyPool.Get();
                enemy.transform.position = spawnPos;
                enemy.transform.SetPositionAndRotation(spawnPos, spawnRotation);
                enemy.Init(_car.transform, _carHealth, _enemyPool);
            }

            _nextChunkZ += _chunkLenght;
        }
    }
}