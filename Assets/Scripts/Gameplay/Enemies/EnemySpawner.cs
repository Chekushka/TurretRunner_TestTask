using System.Collections.Generic;
using Core;
using Gameplay.Car;
using Gameplay.Environment;
using Gameplay.VFX;
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

        [Header("References")] 
        [SerializeField] private Enemy _enemyPrefab;
        [SerializeField] private PooledParticle _deathParticlePrefab;
        [SerializeField] private DamagePopup _damagePopupPrefab;

        private IGameStateProvider _stateProvider;
        private CarMovement _car;
        private CarHealth _carHealth;
        private RoadGenerator _roadGenerator;
        private IObjectPool<Enemy> _enemyPool;
        private IObjectPool<PooledParticle> _deathParticlePool;
        private IObjectPool<DamagePopup> _damagePopupPool;
        private readonly List<Enemy> _activeEnemiesList = new();

        private float _nextChunkZ;
        private int _currentActiveCount;

        [Inject]
        public void Construct(IGameStateProvider stateProvider, CarMovement car, CarHealth carHealth, RoadGenerator roadGenerator)
        {
            _stateProvider = stateProvider;
            _car = car;
            _carHealth = carHealth;
            _roadGenerator = roadGenerator;
        }

        private void Awake()
        {
            _enemyPool = new ObjectPool<Enemy>(
                createFunc: () => Instantiate(_enemyPrefab, transform),
                actionOnGet: enemy =>
                {
                    enemy.gameObject.SetActive(true);
                    _activeEnemiesList.Add(enemy);
                    _currentActiveCount++;
                },
                actionOnRelease: enemy =>
                {
                    enemy.gameObject.SetActive(false);
                    _activeEnemiesList.Remove(enemy);
                    _currentActiveCount--;
                },
                actionOnDestroy: enemy => Destroy(enemy.gameObject),
                defaultCapacity: _maxActiveEnemies,
                maxSize: _maxActiveEnemies + 10
            );
            
            _deathParticlePool = new ObjectPool<PooledParticle>(
                createFunc: () => {
                    var particle = Instantiate(_deathParticlePrefab, transform);
                    particle.Init(_deathParticlePool);
                    return particle;
                },
                actionOnGet: particle => particle.gameObject.SetActive(true),
                actionOnRelease: particle => particle.gameObject.SetActive(false),
                actionOnDestroy: particle => Destroy(particle.gameObject),
                defaultCapacity: 20,
                maxSize: 30
            );
            
            _damagePopupPool = new ObjectPool<DamagePopup>(
                createFunc: () => {
                    var popup = Instantiate(_damagePopupPrefab, transform);
                    popup.Init(_damagePopupPool);
                    return popup;
                },
                actionOnGet: popup => popup.gameObject.SetActive(true),
                actionOnRelease: popup => popup.gameObject.SetActive(false),
                actionOnDestroy: popup => Destroy(popup.gameObject),
                defaultCapacity: 20,
                maxSize: 30
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
                ResetSpawner();
            }
        }
        
        private void ResetSpawner()
        {
            for (int i = _activeEnemiesList.Count - 1; i >= 0; i--)
            {
                _enemyPool.Release(_activeEnemiesList[i]);
            }

            _nextChunkZ = _car.transform.position.z + 20f; 
            SpawnInitialEnemies();
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
                enemy.Init(_car.transform, _carHealth, _enemyPool, _deathParticlePool, _damagePopupPool);
            }

            _nextChunkZ += _chunkLenght;
        }
    }
}