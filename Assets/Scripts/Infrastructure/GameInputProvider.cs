using System;
using Core;
using UnityEngine;
using VContainer.Unity;

namespace Infrastructure
{
    public interface IGameInputProvider
    {
        event Action OnTap;
        bool IsTouching { get; }
        float SwipeDeltaX { get; }
    }

    public class GameInputProvider : IGameInputProvider, IStartable, ITickable, IDisposable
    {
        private readonly IGameStateProvider _stateProvider;
        private readonly GameInput _gameInput;
        
        public event Action OnTap;
        
        public bool IsTouching { get; private set; }
        public float SwipeDeltaX { get; private set; }

        public GameInputProvider(IGameStateProvider stateProvider)
        {
            _gameInput = new GameInput();
            _stateProvider = stateProvider;
        }

        public void Start()
        {
            _gameInput.Player.TouchPress.performed += _ => HandleTap();
            _gameInput.Enable();
        }

        public void Tick()
        {
            IsTouching = _gameInput.Player.TouchPress.IsPressed();

            if (IsTouching)
            {
                Vector2 delta = _gameInput.Player.Delta.ReadValue<Vector2>();
                SwipeDeltaX = delta.x;
            }
            else
            {
                SwipeDeltaX = 0f;
            }
        }

        private void HandleTap()
        {
            OnTap?.Invoke();
            
            if (_stateProvider.CurrentState == GameState.Menu)
            {
                _stateProvider.ChangeState(GameState.ReadyToPlay);
            }
            else if (_stateProvider.CurrentState == GameState.ReadyToPlay)
            {
                _stateProvider.ChangeState(GameState.Gameplay);
            }
        }

        public void Dispose()
        {
            _gameInput.Player.TouchPress.performed -= _ => HandleTap();
            _gameInput.Disable();
            _gameInput.Dispose();
        }
    }
}