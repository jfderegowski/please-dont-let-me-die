using DG.Tweening;
using EventButtonSystem.Runtime;
using FishNet.Managing;
using FishNet.Transporting;
using UISystem.UIAnimationSystem.New;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.FishNetNetworkHud
{
    public class NetworkHud : MonoBehaviour
    {
        [SerializeField] private ButtonWithEvents _serverButton;
        [SerializeField] private ButtonWithEvents _clientButton;

        [Header("Animation Settings")]
        [SerializeField] private Image _serverIndicator;
        [SerializeField] private Image _clientIndicator;
        [SerializeField] private Color _stoppedColor;
        [SerializeField] private Color _startingColor;
        [SerializeField] private Color _startedColor;
        [SerializeField] private Color _stoppingColor;
        [SerializeField] private float _animDuration = 0.5f;
        [SerializeField] private Ease _animEasy = Ease.Linear;
        
        [Inject] private NetworkManager _networkManager;

        private LocalConnectionState _serverState = LocalConnectionState.Stopped;
        private LocalConnectionState _clientState = LocalConnectionState.Stopped;
        private StateAnimations<LocalConnectionState> _serverIndicatorAnimations;
        private StateAnimations<LocalConnectionState> _clientIndicatorAnimations;
        
        private const string ANIM_ID_SERVER = "Server";
        private const string ANIM_ID_CLIENT = "Client";

        private void Awake() => InitializeAnimations();

        private void OnEnable()
        {
            _networkManager.ServerManager.OnServerConnectionState += OnServerConnectionState;
            _networkManager.ClientManager.OnClientConnectionState += OnClientConnectionState;
            
            _serverButton.onClick.AddListener(OnServerButtonClicked);
            _clientButton.onClick.AddListener(OnClientButtonClicked);
        }
        
        private void OnDisable()
        {
            _networkManager.ServerManager.OnServerConnectionState -= OnServerConnectionState;
            _networkManager.ClientManager.OnClientConnectionState -= OnClientConnectionState;
            
            _serverButton.onClick.RemoveListener(OnServerButtonClicked);
            _clientButton.onClick.RemoveListener(OnClientButtonClicked);
        }

        private void OnDestroy()
        {
            _serverIndicatorAnimations.Dispose();
            _clientIndicatorAnimations.Dispose();
        }

        private void OnClientConnectionState(ClientConnectionStateArgs clientConnectionStateArgs)
        {
            _clientState = clientConnectionStateArgs.ConnectionState;
            
            _clientIndicatorAnimations.UpdateState();
        }

        private void OnServerConnectionState(ServerConnectionStateArgs serverConnectionStateArgs)
        {
            _serverState = serverConnectionStateArgs.ConnectionState;
            
            _serverIndicatorAnimations.UpdateState();
        }
        
        private void OnServerButtonClicked()
        {
            if (_networkManager == null)
                return;

            if (_serverState != LocalConnectionState.Stopped)
                _networkManager.ServerManager.StopConnection(true);
            else
                _networkManager.ServerManager.StartConnection();
        }

        private void OnClientButtonClicked()
        {
            if (_networkManager == null)
                return;

            if (_clientState != LocalConnectionState.Stopped)
                _networkManager.ClientManager.StopConnection();
            else
                _networkManager.ClientManager.StartConnection();
        }

        #region Animations

        private void InitializeAnimations()
        {
            var serverStoppedAnim = new StateAnimation(ToStoppedServer, KillServerAnimation);
            var serverStartingAnim = new StateAnimation(ToStartingServer, KillServerAnimation);
            var serverStartedAnim = new StateAnimation(ToStartedServer, KillServerAnimation);
            var serverStoppingAnim = new StateAnimation(ToStoppingServer, KillServerAnimation);
            
            _serverIndicatorAnimations = new StateAnimations<LocalConnectionState>.Builder()
                .SetAnimation(LocalConnectionState.Stopped, serverStoppedAnim)
                .SetAnimation(LocalConnectionState.Starting, serverStartingAnim)
                .SetAnimation(LocalConnectionState.Started, serverStartedAnim)
                .SetAnimation(LocalConnectionState.Stopping, serverStoppingAnim)
                .Build(GetServerState);

            var clientStoppedAnim = new StateAnimation(ToStoppedClient, KillClientAnimation);
            var clientStartingAnim = new StateAnimation(ToStartingClient, KillClientAnimation);
            var clientStartedAnim = new StateAnimation(ToStartedClient, KillClientAnimation);
            var clientStoppingAnim = new StateAnimation(ToStoppingClient, KillClientAnimation);
            
            _clientIndicatorAnimations = new StateAnimations<LocalConnectionState>.Builder()
                .SetAnimation(LocalConnectionState.Stopped, clientStoppedAnim)
                .SetAnimation(LocalConnectionState.Starting, clientStartingAnim)
                .SetAnimation(LocalConnectionState.Started, clientStartedAnim)
                .SetAnimation(LocalConnectionState.Stopping, clientStoppingAnim)
                .Build(GetClientState);
        }

        #region Server Indicator
        
        private LocalConnectionState GetServerState() => _serverState;
        
        private void ToStoppedServer() => ToStopped(_serverIndicator, ANIM_ID_SERVER);
        
        private void ToStartingServer() => ToStarting(_serverIndicator, ANIM_ID_SERVER);
        
        private void ToStartedServer() => ToStarted(_serverIndicator, ANIM_ID_SERVER);
        
        private void ToStoppingServer() => ToStopping(_serverIndicator, ANIM_ID_SERVER);
        
        private void KillServerAnimation() => DOTween.Kill(ANIM_ID_SERVER);

        #endregion

        #region Client Indicator
        
        private LocalConnectionState GetClientState() => _clientState;
        
        private void ToStoppedClient() => ToStopped(_clientIndicator, ANIM_ID_CLIENT);
        
        private void ToStartingClient() => ToStarting(_clientIndicator, ANIM_ID_CLIENT);
        
        private void ToStartedClient() => ToStarted(_clientIndicator, ANIM_ID_CLIENT);
        
        private void ToStoppingClient() => ToStopping(_clientIndicator, ANIM_ID_CLIENT);
        
        private void KillClientAnimation() => DOTween.Kill(ANIM_ID_CLIENT);

        #endregion
        
        private void ToStopped(Image img, string animID) => Animate(img, _stoppedColor, animID);
        
        private void ToStarting(Image img, string animID) => Animate(img, _startingColor, animID);
        
        private void ToStarted(Image img, string animID) => Animate(img, _startedColor, animID);
        
        private void ToStopping(Image img, string animID) => Animate(img, _stoppingColor, animID);

        private void Animate(Image img, Color color, string animID)
        {
            img.DOColor(color, _animDuration)
                .SetEase(_animEasy)
                .SetUpdate(true)
                .SetId(animID);
        }
        
        #endregion
    }
}
