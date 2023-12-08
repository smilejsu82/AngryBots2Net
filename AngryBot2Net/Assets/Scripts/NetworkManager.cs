using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;


public class NetworkManager : MonoBehaviourPunCallbacks
{
    private readonly string version = "1.0.0";
    public static NetworkManager instance;  //싱글톤
    
    public Action onJoinedRoomAction;   //룸에 접속 했을때 
    public Action<string> onJoinRandomFailedAction; //룸에 접속 실패시 
    
    public Action onJoinedLobbyAction; //로비 접속 했을때 

    public Action<Player> onPlayerEnteredRoomAction;  //다른 플레이어가 룸에 들어왔을때
    
    private void Awake()
    {
        NetworkManager.instance = this; //인스턴스
        DontDestroyOnLoad(this.gameObject);
    }

    public void Connect(string nickname)
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = version;
        PhotonNetwork.NickName = nickname;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoom(string roomName, RoomOptions opt)
    {
        PhotonNetwork.CreateRoom(roomName, opt);
    }

    public void JoinOrCreateRoom(string roomName, RoomOptions options)
    {
        Debug.LogFormat("info.Name: {0}, options: {1}", roomName, options);
        //options.MaxPlayers,options.IsOpen,options.IsVisible
        
        Debug.LogFormat("NetworkClientState: {0}", PhotonNetwork.NetworkClientState);
        
        PhotonNetwork.JoinOrCreateRoom(roomName, options, TypedLobby.Default);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("마스터 서버 접속 됨");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("로비에 접속됨");
        //PhotonNetwork.JoinRandomOrCreateRoom();
        //PhotonNetwork.JoinRandomRoom();
        this.onJoinedLobbyAction();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("룸에 입장");
        
        Debug.LogFormat("내꺼 Player: {0}", PhotonNetwork.LocalPlayer);

        // foreach (var pair in PhotonNetwork.CurrentRoom.Players)
        // {
        //     Debug.LogFormat("=> {0}", pair.Value);
        // }

        // if(this.onJoinedRoomAction != null)
        //     this.onJoinedRoomAction(PhotonNetwork.LocalPlayer);
        
        this.onJoinedRoomAction?.Invoke();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("룸 생성 실패 : " + message);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("룸 입장 실패 : " + message);
        this.onJoinRandomFailedAction(message);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.LogFormat("{0}님이 룸에 입장 했습니다.", newPlayer.NickName);   
        
        Debug.LogFormat("원격 Player: {0}", newPlayer);

        this.onPlayerEnteredRoomAction(newPlayer);
    }

    public Action<List<RoomInfo>> onRoomListUpdateAction; 
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.LogFormat("***** OnRoomListUpdate *****");
        
        foreach (var info in roomList)
        {
            Debug.LogFormat("{0}, {1}/{2}, <color=yellow>{3}</color>", 
                info.Name, info.PlayerCount, info.MaxPlayers, info.RemovedFromList);
        }
        
        this.onRoomListUpdateAction(roomList);
    }

    public override void OnLeftRoom()
    {
        Debug.Log("룸을 떠났습니다.");
    }

    public void LeaveRoom()
    {
        Debug.LogFormat("PhotonNetwork.InRoom: {0}", PhotonNetwork.InRoom);
        
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            Debug.Log("룸에 없습니다.");
        }
    }
}
