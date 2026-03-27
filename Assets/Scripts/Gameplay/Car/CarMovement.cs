using Core;
using DG.Tweening;
using Gameplay.Environment;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace Gameplay.Car
{
    [RequireComponent(typeof(Rigidbody))]
    public class CarMovement : MonoBehaviour
    {
        [Header("Forward Movement")] [SerializeField]
        private float _forwardSpeed = 10f;

        [SerializeField] private float _roadWidth = 3.5f;

        [Header("Drift Settings")] [SerializeField]
        private float _driftSmoothTime = 0.8f;

        [SerializeField] private float _maxTiltAngle = 12f;
        [SerializeField] private float _tiltSpeed = 6f;
        [SerializeField] private float _minDriftInterval = 3f;
        [SerializeField] private float _maxDriftInterval = 5f;

        private IGameStateProvider _stateProvider;
        private RoadGenerator _roadGenerator;
        private CarVisuals _visuals;

        private float _targetX;
        private float _currentVelocityX;
        private float _nextDriftTime;
        private bool _isFirstDriftSet;
        
        private Vector3 _startPosition;
        private Quaternion _startRotation;

        private float _currentWheelRollAngle;

        [Inject]
        public void Construct(IGameStateProvider stateProvider, RoadGenerator roadGenerator)
        {
            _stateProvider = stateProvider;
            _roadGenerator = roadGenerator;
        }

        private void Awake()
        {
            var rb = GetComponent<Rigidbody>();
            _visuals = GetComponent<CarVisuals>();
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.None;
            _startPosition = transform.position;
            _startRotation = transform.rotation;
            _roadGenerator.SetCar(this);
        }

        private void OnEnable()
        {
            _stateProvider.OnStateChanged += HandleStateChanged;
        }

        private void OnDisable()
        {
            _stateProvider.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState newState)
        {
            if (newState == GameState.Gameplay)
            {
                _targetX = 0f;
                _nextDriftTime = Time.time + Random.Range(_minDriftInterval, _maxDriftInterval) + 5f;
                _isFirstDriftSet = true;
            }
            else if (newState == GameState.ReadyToPlay)
            {
                transform.DOKill(); 
                transform.position = _startPosition;
                transform.rotation = _startRotation;
            }
        }

        private void Update()
        {
            if (_stateProvider.CurrentState != GameState.Gameplay) return;

            HandleDriftTimer();
            MoveAndRotateCar();
            CheckFinishLine();
        }

        private void HandleDriftTimer()
        {
            if (!_isFirstDriftSet) return;

            if (Time.time >= _nextDriftTime)
            {
                SetNewTargetX();
                _nextDriftTime = Time.time + Random.Range(_minDriftInterval, _maxDriftInterval);
            }
        }

        private void SetNewTargetX()
        {
            float currentX = transform.position.x;
            float threshold = _roadWidth * 0.4f;

            if (Mathf.Abs(currentX - currentX) > threshold)
            {
                _targetX = Mathf.Sign(currentX) * Random.Range(0.1f, _roadWidth * 0.5f);
            }
            else
            {
                _targetX = Random.Range(-_roadWidth * 0.4f, _roadWidth * 0.4f);
            }
        }

        private void MoveAndRotateCar()
        {
            float currentX = transform.position.x;
            float newX = Mathf.SmoothDamp(currentX, _targetX, ref _currentVelocityX, _driftSmoothTime);
            float newZ = transform.position.z + _forwardSpeed * Time.deltaTime;

            transform.position = new Vector3(newX, transform.position.y, newZ);

            float distanceToTarget = _targetX - currentX;
            float targetAngle = Mathf.Clamp(distanceToTarget * 8f, -_maxTiltAngle, _maxTiltAngle);

            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _tiltSpeed);
            _visuals.AnimateWheels(_forwardSpeed);
        }
        
        private void CheckFinishLine()
        {
            if (transform.position.z >= _roadGenerator.LevelDistance)
            {
                TriggerFinishSequence();
            }
        }
        
        private void TriggerFinishSequence()
        {
            _stateProvider.ChangeState(GameState.Won);
            Vector3 targetPos = transform.position + new Vector3(0, 0, 10f);
            
            transform.DOMove(targetPos, 1.5f).SetEase(Ease.OutCubic);
            transform.DORotate(new Vector3(0, 90f, 0), 1.5f, RotateMode.Fast)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => 
                {
                    // TODO: Add particles
                    Debug.Log("Finish Animation Complete");
                });
        }
    }
}