using System;
using VContainer.Unity;

public interface IGameInputProvider
{
    event Action OnTap;
}

public class GameGameInputProvider : IGameInputProvider, IStartable, IDisposable
{
    private readonly IGameStateProvider _stateProvider;
    private readonly GameInput _gameInput;
    public event Action OnTap;

    public GameGameInputProvider(IGameStateProvider stateProvider)
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
    }

    public void Dispose()
    {
        _gameInput.Disable();
    }
}