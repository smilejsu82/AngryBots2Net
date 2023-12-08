using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PhotonManager : MonoBehaviourPunCallbacks, IInRoomCallbacks
{
    private readonly string version = "1.0";
    public TMP_InputField nicknameInputField;
    private string nickname;

    public Button btnConnectToMasterServer;
    
    public Button btnJoinLobby;
    public Button btnLeaveLobby;

    public GameObject roomCreateGo;
    public TMP_InputField roomNameInputField;
    public Button createRoomBtn;

    public GameObject joinRoomGo;
    public TMP_Text joinedRoomNameText;
    public TMP_Text playerCountText;

    public Button btnLeaveRoom;
    public Button randomJoinRoomBtn;
    
    private void Awake()
    {
        Debug.Log("Awake");
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = this.version;
        Debug.Log(PhotonNetwork.SendRate);  //default: 30 
        
    }

    private void Start()
    {
        Debug.Log("Start");
        this.btnJoinLobby.gameObject.SetActive(false);
        this.btnLeaveLobby.gameObject.SetActive(false);
        
        this.btnConnectToMasterServer.onClick.AddListener(() =>
        {
            this.nickname = this.nicknameInputField.text;
            PhotonNetwork.NickName = this.nickname;
            
            if (string.IsNullOrEmpty(this.nickname))
            {
                Debug.Log("<color=red>닉네임을 입력 해야 합니다.</color>");
            }
            else
            {
                //마스터 서버 접속 
                PhotonNetwork.ConnectUsingSettings();
            }
        });
        
        this.btnJoinLobby.onClick.AddListener(() =>
        {
            Debug.Log("btnJoinLobby clicked");
            
            PhotonNetwork.JoinLobby();
        });
        this.btnLeaveLobby.onClick.AddListener(() =>
        {
            Debug.Log("btnLeaveLobby clicked");
            PhotonNetwork.LeaveLobby();
        });

        this.roomCreateGo.SetActive(false);
        this.createRoomBtn.onClick.AddListener(() =>
        {
            string roomName = this.roomNameInputField.text;
            if (string.IsNullOrEmpty(roomName))
            {
                Debug.LogFormat("<color=red>방 이름을 입력 해야 합니다.</color>");
            }
            else
            {
                Debug.LogFormat("방을 생성합니다.");
                this.CreateRoom(roomName);
            }
        });
        
        this.btnLeaveRoom.onClick.AddListener(() =>
        {
            Debug.Log("btnLeaveRoom clicked");
            PhotonNetwork.LeaveRoom();
        });
        
        this.randomJoinRoomBtn.onClick.AddListener(() =>
        {
            PhotonNetwork.JoinRandomRoom();
        });
    }

    public override void OnConnectedToMaster()
    {
        Debug.LogFormat("마스터 서버에 접속 : {0}", PhotonNetwork.NickName);
        Debug.LogFormat("PhotonNetwork.InLobby: {0}", PhotonNetwork.InLobby);
        
        this.nicknameInputField.gameObject.SetActive(false);
        this.btnConnectToMasterServer.gameObject.SetActive(false);
        
        this.btnJoinLobby.gameObject.SetActive(true);
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비 접속 성공");
        Debug.LogFormat("PhotonNetwork.InLobby: {0}", PhotonNetwork.InLobby);
        
        this.roomCreateGo.SetActive(true);
        this.btnLeaveLobby.gameObject.SetActive(true);
        this.btnJoinLobby.gameObject.SetActive(false);
        //만들어져있는 임의 룸에 입장을 시도, 룸이 없다면 Failed 
        //PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogFormat("{0}, {1}", returnCode, message);
        //this.CreateRoom("Test123");
    }

    private void CreateRoom(string roomName)
    {
        Debug.LogFormat("CreateRoom");
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 20;
        options.IsOpen = true;
        options.IsVisible = true;
        PhotonNetwork.CreateRoom(roomName, options);
    }

    //룸 생성 완료  
    public override void OnCreatedRoom()
    {
        Debug.Log("룸 생성완료");
        Debug.LogFormat("PhotonNetwork.CurrentRoom.Name: {0}", PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("룸 입장");
        this.roomCreateGo.SetActive(false);
        this.btnLeaveLobby.gameObject.SetActive(false);
        this.joinRoomGo.SetActive(true);
        this.roomNameInputField.text = "";
        
        Debug.LogFormat("PhotonNetwork.CurrentRoom.PlayerCount: {0}", PhotonNetwork.CurrentRoom.PlayerCount);

        foreach (var pair in PhotonNetwork.CurrentRoom.Players)
        {
            Player player = pair.Value;
            Debug.LogFormat("pair.Key: {0}", pair.Key);
            Debug.LogFormat("NickName: {0}, ActorNumber: {1}", player.NickName, player.ActorNumber);
        }

        this.playerCountText.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        this.joinedRoomNameText.text = PhotonNetwork.CurrentRoom.Name;

        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);
        
        GameObject go = PhotonNetwork.Instantiate("Player", points[idx].position, points[idx].rotation);
        

    }

    //룸 생성 실패 
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogFormat("{0}, {1}", returnCode, message);
    }

    public override void OnLeftLobby()
    {
        Debug.Log("로비에서 나갔습니다.");
        
        this.btnLeaveLobby.gameObject.SetActive(false);
        this.btnJoinLobby.gameObject.SetActive(true);
        this.roomCreateGo.SetActive(false);
        this.roomNameInputField.text = "";
    }

    public override void OnLeftRoom()
    {
        //Debug.LogFormat("{0}에서 나갔습니다.", PhotonNetwork.CurrentRoom.Name);    //CurrentRoom
        Debug.Log("룸에서 나갔습니다");
        
        //마스터 서버에 접속 다시 할거라서 ~ 
        this.joinRoomGo.SetActive(false);
    }
    
    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("<color=yellow>{0}</color>님이 입장 하셨습니다.", newPlayer.NickName);
        this.playerCountText.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
    }
    
    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.LogFormat("<color=yellow>{0}</color>님이 퇴장 하셨습니다.", otherPlayer.NickName);
        this.playerCountText.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
    }
}
