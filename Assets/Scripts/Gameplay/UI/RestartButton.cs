using UnityEngine;
using UnityEngine.UI;
using VContainer;
using Core;

namespace Gameplay.UI
{
    [RequireComponent(typeof(Button))]
    public class RestartButton : MonoBehaviour
    {
        private Button _button;
        private IGameStateProvider _stateProvider;

        [Inject]
        public void Construct(IGameStateProvider stateProvider)
        {
            _stateProvider = stateProvider;
        }

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(RestartGame);
        }

        private void RestartGame()
        {
            _stateProvider.ChangeState(GameState.ReadyToPlay);
        }
    }
}