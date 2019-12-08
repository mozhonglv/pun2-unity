using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LobbyInfoView : MonoBehaviour {

    #region Childs

    [SerializeField]
    private InputField _inputNickname;

    [SerializeField]
    private Button _btnConnect;

    [SerializeField]
    private Text _txtProgress;

    #endregion


    #region [Unity] OnEnable

    private void OnEnable() {
        string nickname = NetworkHub.Instance.GetNickname();
        if (!string.IsNullOrEmpty(nickname)) {
            _inputNickname.text = nickname;
        }

        SetState(false);

        NetworkHub.Instance.OnConnectNetwork += OnConnectedNetwork;
        NetworkHub.Instance.OnDisconnectNetwork += OnDisconnectNetwork;

        _inputNickname.onValueChanged.AddListener(new UnityAction<string>(OnInputNicknameValueChanged));
        _btnConnect.onClick.AddListener(new UnityAction(OnBtnConnectClick));
    }
    
    #endregion

    public void SetState(bool isOnConnect) {
        _inputNickname.gameObject.SetActive(!isOnConnect);
        _btnConnect.gameObject.SetActive(!isOnConnect);
        _txtProgress.gameObject.SetActive(isOnConnect);
    }


    #region Event: OnConnectedNetwork

    private void OnConnectedNetwork() {

    }

    #endregion

    #region Event: OnDisconnectNetwork

    private void OnDisconnectNetwork() {
        
    }

    #endregion

    #region Event: OnInputNicknameValueChanged

    private void OnInputNicknameValueChanged(string arg0) {
        NetworkHub.Instance.SetNickname(arg0);
    }

    #endregion

    #region Event: OnBtnConnectClick

    private void OnBtnConnectClick() {
        SetState(true);

        NetworkHub.Instance.Connect();
    }

    #endregion


    #region [Unity] OnDisable

    private void OnDisable() {
        if (NetworkHub.Instance) {
            NetworkHub.Instance.OnConnectNetwork -= OnConnectedNetwork;
            NetworkHub.Instance.OnDisconnectNetwork -= OnDisconnectNetwork;
        }

        _inputNickname.onValueChanged.RemoveAllListeners();
        _btnConnect.onClick.RemoveAllListeners();
    }

    #endregion

}