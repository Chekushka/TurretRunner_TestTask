using System;

namespace Core
{
    public class GameStateManager : IGameStateProvider
    {
        public GameState CurrentState { get; private set; } = GameState.ReadyToPlay;
        public event Action<GameState> OnStateChanged;

        public void ChangeState(GameState newState)
        {
            if (CurrentState == newState) return;
            
            CurrentState = newState;
            OnStateChanged?.Invoke(CurrentState);
        }
    }
}