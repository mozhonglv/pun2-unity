using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkHub : MonoBehaviourPunCallbacks {

    #region Events:

    public delegate void NetworkHubConnectEvent();

    public event NetworkHubConnectEvent OnConnectNetwork;
    public event NetworkHubConnectEvent OnDisconnectNetwork;

    #endregion

    public const string PLAYER_NICKNAME_PREF = "_nickname";

    #region Singleton Instance

    private static NetworkHub _instance;
    public static NetworkHub Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<NetworkHub>();
            }
            return _instance;

        }
    }

    #endregion

    #region Configuration

    [Header("Configuration")]

    [Tooltip("This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).")]
    [SerializeField]
    private string _gameVersion = "0.0.1";

    [Tooltip("This makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically")]
    [SerializeField]
    private bool _isAutomaticallySyncScene = true;

    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    [SerializeField]
    private byte _roomMaxPlayerCount = 2;

    #endregion

    /// <summary>
    /// Keep track of the current process. Since connection is asynchronous and is based on several callbacks from Photon,
    /// we need to keep track of this to properly adjust the behavior when we receive call back by Photon.
    /// Typically this is used for the OnConnectedToMaster() callback.
    /// </summary>
    private bool _isConnecting = false;


    #region [Unity] Awake

    private void Awake() {
        if (_instance) {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    #region [Unity] Start

    private void Start() {
        PhotonNetwork.GameVersion = _gameVersion;
        PhotonNetwork.AutomaticallySyncScene = _isAutomaticallySyncScene;
    }

    #endregion


    #region [Network] Connect

    public void Connect() {
        _isConnecting = true;
        // TODO: Start Connect Timer and if its finished dispatch connect fail event.

        if (PhotonNetwork.IsConnected) {
            PhotonNetwork.JoinRandomRoom();
        } else {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    #endregion

    #region [Network] LeaveLobby

    public void LeaveLobby() {
        PhotonNetwork.LeaveLobby();
    }

    #endregion

    #region [Network] LeaveRoom

    public void LeaveRoom() {
        PhotonNetwork.LeaveRoom();
    }

    #endregion


    #region [Photon] OnConnectedToMaster

    public override void OnConnectedToMaster() {
        Debug.Log("OnConnectedToMaster()");

        OnConnectNetwork?.Invoke();

        if (_isConnecting) {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    #endregion

    #region [Photon] OnDisconnected

    public override void OnDisconnected(DisconnectCause cause) {
        Debug.LogWarningFormat("OnDisconnected() reason {0}", cause);

        OnDisconnectNetwork?.Invoke();
    }

    #endregion

    #region [Photon] Room: OnJoinedRoom

    public override void OnJoinedRoom() {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1) {
            PhotonNetwork.LoadLevel("Game");
        }

        // TODO: Dispatch OnJoinedRoom Event
        Debug.Log("OnJoinedRoom()");
    }

    #endregion

    #region [Photon] Room: OnLeftRoom

    public override void OnLeftRoom() {
        // TODO: Dispatch OnLeftRoom Event

        Debug.Log("OnLeftRoom()");
    }

    #endregion

    #region [Photon] Room: OnJoinRandomFailed

    public override void OnJoinRandomFailed(short returnCode, string message) {
        Debug.Log("OnJoinRandomFailed() was called by PUN. No random room available, so we create one");

        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = _roomMaxPlayerCount });

        // TODO: Dispatch OnJoinRandomFailed Event
    }

    #endregion


    #region Nickname: Get, Set

    public string GetNickname() {
        string nickname = string.Empty;

        if (PlayerPrefs.HasKey(PLAYER_NICKNAME_PREF)) {
            nickname = PlayerPrefs.GetString(PLAYER_NICKNAME_PREF);
        }

        return nickname;
    }

    public void SetNickname(string nickname) {
        if (string.IsNullOrEmpty(nickname)) {
            nickname = "";
        }

        PhotonNetwork.NickName = nickname;
        PlayerPrefs.SetString(PLAYER_NICKNAME_PREF, nickname);
    }

    #endregion

}