using Core;
using Unity.Cinemachine;
using UnityEngine;
using VContainer;

public class CameraController : MonoBehaviour
{
   private CinemachineCamera _cmCamera;
   private IGameStateProvider _stateProvider;

   [Inject]
   public void Construct(IGameStateProvider stateProvider)
   {
      _stateProvider = stateProvider;
   }

   private void Awake()
   {
      _cmCamera = GetComponent<CinemachineCamera>();
   }
}