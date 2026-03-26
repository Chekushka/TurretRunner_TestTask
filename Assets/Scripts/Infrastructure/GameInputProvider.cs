using System;
using Core;
using VContainer.Unity;

namespace Infrastructure
{
    public interface IGameInputProvider
    {
        event Action OnTap;
    }

    public class GameInputProvider : IGameInputProvider, IStartable, IDisposable
    {
        private readonly IGameStateProvider _stateProvider;
        private readonly GameInput _gameInput;
        public event Action OnTap;

        public GameInputProvider(IGameStateProvider stateProvider)
        {
            _gameInput = new GameInput();
            _stateProvider = stateProvider;
        }

        public void Start()
        {
            _gameInput.Player.Touch.performed += _ => HandleTap();
            _gameInput.Enable();
        }

        private void HandleTap()
        {
            OnTap?.Invoke();
            if (_stateProvider.CurrentState == GameState.Menu)
            {
                _stateProvider.ChangeState(GameState.Gameplay);
            }
            else if (_stateProvider.CurrentState == GameState.ReadyToPlay)
            {
                _stateProvider.ChangeState(GameState.Gameplay);
            }
        }

        public void Dispose()
        {
            _gameInput.Disable();
        }
    }
}