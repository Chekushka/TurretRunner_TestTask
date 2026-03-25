using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
public class CarMovement : MonoBehaviour
{
    [Header("Forward Movement")]
    [SerializeField] private float _maxSpeed = 12f;
    [SerializeField] private float _acceleration = 5f;

    [Header("Drift Settings")] 
    [SerializeField] private float _roadWidth = 3.5f;
    [SerializeField] private float _driftSpeed = 2f;
    [SerializeField] private float _minDriftInterval = 1.5f;
    [SerializeField] private float _maxDriftInterval = 3f;
    
    private Rigidbody _rb;
    private IGameStateProvider _stateProvider;
    
    private float _currentForwardSpeed;
    private float _targetX;
    private float _nextDriftTime;
    private bool _isFirstDriftSet;

    [Inject]
    public void Construct(IGameStateProvider stateProvider)
    {
        _stateProvider = stateProvider;
    }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        _rb.useGravity = true;
        
        _targetX = 0f;
        _nextDriftTime = Time.time + Random.Range(_minDriftInterval, _maxDriftInterval);
    }
    
    private void OnEnable()
    {
        _stateProvider.OnStateChanged += HandleStateChanged;
    }

    private void OnDisable()
    {
        _stateProvider.OnStateChanged -= HandleStateChanged;
    }

    private void FixedUpdate()
    {
        if (_stateProvider.CurrentState != GameState.Gameplay)
        {
            StopCar();
            return;
        }

        HandleForwardMovement();
        if (_isFirstDriftSet)
            HandleDriftLogic();
    }
    
    private void HandleStateChanged(GameState newState)
    {
        if (newState == GameState.Gameplay)
        {
            _nextDriftTime = Time.time + Random.Range(_minDriftInterval, _maxDriftInterval) + 5f;
            _isFirstDriftSet = true;
        }
    }

    private void HandleForwardMovement()
    {
        _currentForwardSpeed = Mathf.MoveTowards(_currentForwardSpeed, _maxSpeed, _acceleration * Time.fixedDeltaTime);
    }

    private void HandleDriftLogic()
    {
        if (Time.time >= _nextDriftTime)
        {
            UpdateTargetX();
            _nextDriftTime = Time.time + Random.Range(_minDriftInterval, _maxDriftInterval);
        }
        
        float currentX = transform.position.x;
        float newXVelocity = (_targetX - currentX) * _driftSpeed;
        
        _rb.linearVelocity = new Vector3(newXVelocity, _rb.linearVelocity.y, _currentForwardSpeed);
    }

    private void UpdateTargetX()
    {
        float currentX = transform.position.x;
        float threshold = _roadWidth * 0.4f;

        if (Mathf.Abs(currentX) > threshold)
        {
            _targetX = -Mathf.Sign(currentX) * Random.Range(0.1f, _roadWidth * 0.5f);
        }
        else
        {
            _targetX = Random.Range(-_roadWidth * 0.8f, _roadWidth * 0.8f);
        }
    }


    private void StopCar()
    {
        _currentForwardSpeed = Mathf.MoveTowards(_currentForwardSpeed, 0, _acceleration * 2 * Time.fixedDeltaTime);
        _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, _currentForwardSpeed);
    }
}