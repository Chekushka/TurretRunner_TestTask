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
        private IGameStateProvider _stateProvider;
        private RoadGenerator _roadGenerator;
        private CarVisuals _visuals;
        private GameSettings _settings;

        private float _targetX;
        private float _currentVelocityX;
        private float _nextDriftTime;
        private bool _isFirstDriftSet;
        
        private Vector3 _startPosition;
        private Quaternion _startRotation;

        private float _currentWheelRollAngle;

        [Inject]
        public void Construct(IGameStateProvider stateProvider, RoadGenerator roadGenerator, GameSettings settings)
        {
            _stateProvider = stateProvider;
            _roadGenerator = roadGenerator;
            _settings = settings;
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
                _nextDriftTime = Time.time + Random.Range(_settings.MinDriftInterval, _settings.MaxDriftInterval) + 5f;
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
                _nextDriftTime = Time.time + Random.Range(_settings.MinDriftInterval, _settings.MaxDriftInterval);
            }
        }

        private void SetNewTargetX()
        {
            var roadWidth = _settings.RoadWidth;
            float currentX = transform.position.x;
            float threshold = roadWidth * 0.4f;

            if (Mathf.Abs(currentX - currentX) > threshold)
            {
                _targetX = Mathf.Sign(currentX) * Random.Range(0.1f, roadWidth * 0.5f);
            }
            else
            {
                _targetX = Random.Range(-roadWidth * 0.4f, roadWidth * 0.4f);
            }
        }

        private void MoveAndRotateCar()
        {
            float currentX = transform.position.x;
            float newX = Mathf.SmoothDamp(currentX, _targetX, ref _currentVelocityX, _settings.DriftSmoothTime);
            float newZ = transform.position.z + _settings.CarForwardSpeed * Time.deltaTime;

            transform.position = new Vector3(newX, transform.position.y, newZ);

            float distanceToTarget = _targetX - currentX;
            var tiltAngle = _settings.MaxTiltAngle;
            float targetAngle = Mathf.Clamp(distanceToTarget * 8f, -tiltAngle, tiltAngle);

            Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * _settings.CarTiltSpeed);
            _visuals.AnimateWheels(_settings.CarForwardSpeed);
        }
        
        private void CheckFinishLine()
        {
            if (transform.position.z >= _roadGenerator.ActualFinishZ - 60f && _stateProvider.CurrentState == GameState.Gameplay)
            {
                TriggerFinishSequence();
            }
        }
        
        private void TriggerFinishSequence()
        {
            _stateProvider.ChangeState(GameState.Won);
            float driveDistance = 5f; 
            Vector3 finalPos = new Vector3(0, 0, transform.position.z + driveDistance);
            
            Sequence finishSeq = DOTween.Sequence();
            finishSeq.Append(transform.DOMoveZ(finalPos.z, 2f).SetEase(Ease.OutCubic));
            finishSeq.Join(transform.DORotate(new Vector3(0, 70f, 0), 2f).SetEase(Ease.OutQuad));
            
            finishSeq.OnComplete(() => {
                //TODO: Add particles
            });
        }
    }
}