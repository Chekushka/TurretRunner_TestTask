using Core;
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

        [Header("Wheels Visuals")] [SerializeField]
        private Transform[] _frontWheels;

        [SerializeField] private Transform[] _rearWheels;
        [SerializeField] private float _wheelRadius = 0.3f;

        private IGameStateProvider _stateProvider;

        private float _targetX;
        private float _currentVelocityX;
        private float _nextDriftTime;
        private bool _isFirstDriftSet;

        private float _currentWheelRollAngle;

        [Inject]
        public void Construct(IGameStateProvider stateProvider)
        {
            _stateProvider = stateProvider;
        }

        private void Awake()
        {
            var rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            rb.isKinematic = true;
            rb.interpolation = RigidbodyInterpolation.None;
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
        }

        private void Update()
        {
            if (_stateProvider.CurrentState != GameState.Gameplay) return;

            HandleDriftTimer();
            MoveAndRotateCar();
            AnimateWheels();
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
        }

        private void AnimateWheels()
        {
            float wheelCircumference = 2 * Mathf.PI * _wheelRadius;
            float degreesPerSecond = (_forwardSpeed / wheelCircumference) * 360f;

            _currentWheelRollAngle += degreesPerSecond * Time.deltaTime;

            float steerAngle = transform.rotation.eulerAngles.y;
            if (steerAngle > 180f) steerAngle -= 360f;

            float exaggeratedSteer = steerAngle * 1.5f;

            foreach (var wheel in _frontWheels)
            {
                wheel.localRotation = Quaternion.Euler(_currentWheelRollAngle, exaggeratedSteer, 0);
            }

            foreach (var wheel in _rearWheels)
            {
                wheel.localRotation = Quaternion.Euler(_currentWheelRollAngle, 0, 0);
            }
        }
    }
}