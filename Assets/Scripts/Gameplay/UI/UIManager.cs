using UnityEngine;
using VContainer;
using Core;

namespace Gameplay.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject _readyToPlayPanel;
        [SerializeField] private GameOverUI _gameOverUI;

        private IGameStateProvider _stateProvider;

        [Inject]
        public void Construct(IGameStateProvider stateProvider)
        {
            _stateProvider = stateProvider;
        }

        private void OnEnable()
        {
            _stateProvider.OnStateChanged += HandleStateChanged;
            HandleStateChanged(_stateProvider.CurrentState); 
        }

        private void OnDisable()
        {
            _stateProvider.OnStateChanged -= HandleStateChanged;
        }

        private void HandleStateChanged(GameState state)
        {
            _readyToPlayPanel.SetActive(state == GameState.ReadyToPlay);
            
            if (state == GameState.Won || state == GameState.Lost)
            {
                _gameOverUI.ShowResult(state == GameState.Won);
            }
            else
            {
                _gameOverUI.gameObject.SetActive(false);
            }
        }
    }
}