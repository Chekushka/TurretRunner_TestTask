using System;

public interface IGameStateProvider
{
    GameState CurrentState { get; }
    event Action<GameState> OnStateChanged;
    void ChangeState(GameState newState);
}

public enum GameState
{
    Menu,
    Gameplay,
    Won,
    Lost
}
