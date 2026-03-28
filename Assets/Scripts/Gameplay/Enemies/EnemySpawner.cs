using System;
using System.Collections.Generic;
using Core;
using Gameplay.Car;
using Gameplay.Environment;
using Gameplay.VFX;
using Infrastructure;
using UnityEngine;
using UnityEngine.Pool;
using VContainer;
using Random = UnityEngine.Random;

namespace Gameplay.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Spawn Zone Settings")] 
        [SerializeField] private float _chunkLenght = 40f;
        [SerializeField] private float _spawnAheadOffset = 50f;

        [Header("References")] 
        [SerializeField] private Enemy _enemyPrefab;
        [SerializeField] private PooledParticle _deathParticlePrefab;
        [SerializeField] private DamagePopup _damagePopupPrefab;

        private IGameStateProvider _stateProvider;
        private CarMovement _car;
        private CarHealth _carHealth;
        private RoadGenerator _roadGenerator;
        private GameSettings _settings;
        private IObjectPool<Enemy> _enemyPool;
        private IObjectPool<PooledParticle> _deathParticlePool;
        private IObjectPool<DamagePopup> _damagePopupPool;
        private readonly List<Enemy> _activeEnemiesList = new();

        private float _nextChunkZ;
        private const float DistanceMultiplier = 70f;

        [Inject]
        public void Construct(IGameStateProvider stateProvider, CarMovement car, CarHealth carHealth, RoadGenerator roadGenerator, GameSettings settings)
        {
            _stateProvider = stateProvider;
            _car = car;
            _carHealth = carHealth;
            _roadGenerator = roadGenerator;
            _settings = settings;
        }

        private void Awake()
        {
            _enemyPool = new ObjectPool<Enemy>(
                createFunc: () => Instantiate(_enemyPrefab, transform),
                actionOnGet: enemy =>
                {
                    enemy.gameObject.SetActive(true);
                    _activeEnemiesList.Add(enemy);
                },
                actionOnRelease: enemy =>
                {
                    enemy.gameObject.SetActive(false);
                    _activeEnemiesList.Remove(enemy);
                },
                actionOnDestroy: enemy => Destroy(enemy.gameObject),
                defaultCapacity: _settings.MaxActiveEnemies,
                maxSize: _settings.MaxActiveEnemies + 10
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

        private void Start()
        {
            _damagePopupPool.Prewarm(10);
            _deathParticlePool.Prewarm(10);
            _enemyPool.Prewarm(10);
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
            else if (state == GameState.Won)
            {
                ClearAllEnemies();
            }
        }
        
        private void ResetSpawner()
        {
            ClearAllEnemies();
            
            _nextChunkZ = 20f; 
            SpawnInitialEnemies();
        }
        
        private void ClearAllEnemies()
        {
            for (int i = _activeEnemiesList.Count - 1; i >= 0; i--)
            {
                _enemyPool.Release(_activeEnemiesList[i]);
            }
        }

        private void Update()
        {
            if (_stateProvider.CurrentState != GameState.Gameplay) return;
            float distanceToFinish = (_roadGenerator.TotalLevelDistance - 1) * DistanceMultiplier - _car.transform.position.z;
            if(distanceToFinish < 10f) return;
            if (_car.transform.position.z + _spawnAheadOffset >= _nextChunkZ)
            {
                SpawnChunk();
            }
        }

        private void SpawnInitialEnemies()
        {
            for (int i = 0; i < 3; i++)
                SpawnChunk();
        }

        private void SpawnChunk()
        {
            int enemiesToSpawn = Mathf.Min(_settings.EnemiesPerChunk, _settings.MaxActiveEnemies - _activeEnemiesList.Count);

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                float randomX = Random.Range(-_settings.RoadWidth, _settings.RoadWidth);
                float randomZ = Random.Range(_nextChunkZ, _nextChunkZ + _chunkLenght);
                
                float randomRotationY = Random.Range(0f, 360f);

                Vector3 spawnPos = new Vector3(randomX, 0f, randomZ);
                Quaternion spawnRotation = Quaternion.Euler(0, randomRotationY, 0);

                Enemy enemy = _enemyPool.Get();
                enemy.transform.position = spawnPos;
                enemy.transform.SetPositionAndRotation(spawnPos, spawnRotation);
                enemy.Init(_car.transform, _carHealth, _enemyPool, _deathParticlePool, _damagePopupPool, _settings);
            }

            _nextChunkZ += _chunkLenght;
        }
    }
}