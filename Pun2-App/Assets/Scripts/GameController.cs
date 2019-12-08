using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    private const byte _EVENT_ID_CHANGE_COLOR = 1;

    #region UI

    [Header("UI")]

    [SerializeField]
    private Image _imgColor;

    [SerializeField]
    private Button _btnRed;

    [SerializeField]
    private Button _btnGreen;

    [SerializeField]
    private Button _btnBlue;

    #endregion


    #region [Unity] OnEnable

    private void OnEnable() {
        PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;

        _btnRed.onClick.AddListener(new UnityAction(OnBtnRedClick));
        _btnGreen.onClick.AddListener(new UnityAction(OnBtnGreenClick));
        _btnBlue.onClick.AddListener(new UnityAction(OnBtnBlueClick));
    }

    #endregion


    public void ChangeColor(Color color) {
        _imgColor.color = color;

        object[] datas = new object[] {
            color.r,
            color.g,
            color.b
        };
        PhotonNetwork.RaiseEvent(_EVENT_ID_CHANGE_COLOR, datas, RaiseEventOptions.Default, SendOptions.SendReliable);
    }

    public void OnEventReceived(EventData obj) {
        if (obj.Code == _EVENT_ID_CHANGE_COLOR) {
            object[] datas = (object[])obj.CustomData;
            float r = (float)datas[0];
            float g = (float)datas[1];
            float b = (float)datas[2];

            _imgColor.color = new Color(r, g, b, 1);
        }
    }


    #region Events: OnBtnRedClick, OnBtnGreenClick, OnBtnBlueClick

    private void OnBtnRedClick() {
        ChangeColor(Color.red);
    }

    private void OnBtnGreenClick() {
        ChangeColor(Color.green);
    }

    private void OnBtnBlueClick() {
        ChangeColor(Color.blue);
    }

    #endregion
    

    #region [Unity] OnDisable

    private void OnDisable() {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEventReceived;

        _btnRed.onClick.RemoveAllListeners();
        _btnGreen.onClick.RemoveAllListeners();
        _btnBlue.onClick.RemoveAllListeners();
    }

    #endregion

}