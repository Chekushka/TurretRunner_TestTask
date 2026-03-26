using System;

namespace Core
{
    public interface IGameStateProvider
    {
        GameState CurrentState { get; }
        event Action<GameState> OnStateChanged;
        void ChangeState(GameState newState);
    }
}