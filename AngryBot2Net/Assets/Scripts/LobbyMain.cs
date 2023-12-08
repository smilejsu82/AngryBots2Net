using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMain : MonoBehaviour
{
    public TMP_InputField nicknameInputField;
    public TMP_InputField roomNameInputField;
    public Button btnJoin;
    public Button btnCreateRoom;
    public Button btnExitRoom;
    public GameObject uiLoadingGo;

    private Dictionary<string, RoomCellView> dic = new Dictionary<string, RoomCellView>();
    public GameObject roomCellViewPrefab;
    public Transform content;

    public GameObject scrollviewGo; //로비 방목록 스크롤뷰 
    public UIPlayersPanel uiPlayersPanel;
    void Start()
    {
        NetworkManager.instance.onPlayerEnteredRoomAction = (newPlayer) =>
        {
            this.uiPlayersPanel.AddPlayerCellView(newPlayer);
        };
        
        NetworkManager.instance.onJoinedRoomAction = () =>
        {
            Debug.LogFormat("룸에 입장 했습니다. {0}, {1}/{2}", 
                PhotonNetwork.CurrentRoom.Name, 
                PhotonNetwork.CurrentRoom.PlayerCount, 
                PhotonNetwork.CurrentRoom.MaxPlayers);

            //스크롤뷰 안보이기 
            this.scrollviewGo.SetActive(false);
            //UIPlayersPanel보이기 
            this.uiPlayersPanel.gameObject.SetActive(true);
            this.uiPlayersPanel.Init(PhotonNetwork.CurrentRoom.Name);

            List<Player> players = new List<Player>();
            if (!PhotonNetwork.IsMasterClient)  //내가 마스터가 아니면 
            {
                var master = PhotonNetwork.CurrentRoom.Players.Values.ToList().Find(x=> x.IsMasterClient);
                
                players.Add(master);
                
                foreach (var pair in PhotonNetwork.CurrentRoom.Players)
                {
                    var player = pair.Value;
                    if (player.IsMasterClient == false)
                    {
                        Debug.LogFormat("{0} -> {1}",player.NickName, player.IsMasterClient);
                        players.Add(player);    
                    }
                }
                
                for (int i = 0; i < players.Count; i++)
                {
                    var player = players[i];    //0 : master
                    this.uiPlayersPanel.AddPlayerCellView(player);
                }
            }
            else
            {
                //마스터면 
                this.uiPlayersPanel.AddPlayerCellView(PhotonNetwork.LocalPlayer);
            }

        };
        
        NetworkManager.instance.onJoinedLobbyAction = () =>
        {
            Debug.LogFormat("로비에 입장 했습니다. LocalPlayer: {0}", PhotonNetwork.LocalPlayer); 
            this.uiLoadingGo.SetActive(false);
            
            //스크롤뷰 보이기 
            this.scrollviewGo.SetActive(true);
            //UIPlayersPanel안보이기 
            this.uiPlayersPanel.gameObject.SetActive(false);
            
        };
        
        NetworkManager.instance.onRoomListUpdateAction = (list) =>
        {
            foreach (var info in list)
            {
                Debug.LogFormat("=> {0}, {1}", info.Name, info.RemovedFromList);
                if (info.RemovedFromList)
                {
                    //지우기
                    // if (this.dic.TryGetValue(info.Name, out var cellView))
                    // {
                    //     Destroy(cellView.gameObject);
                    // }
                    
                    if (this.dic.ContainsKey(info.Name))//리스트에 있음 
                    {
                        var cellView = this.dic[info.Name];
                        Destroy(cellView.gameObject);

                        this.dic.Remove(info.Name);
                    }
                }
                else
                {
                    if (this.dic.ContainsKey(info.Name) == false)
                    {
                        var go = Instantiate(this.roomCellViewPrefab, content);
                        var cellView = go.GetComponent<RoomCellView>();
                        //이벤트 달기 
                        cellView.btn.onClick.AddListener(() =>
                        {
                            Debug.LogFormat("RoomName: <color=yellow>{0}</color>", info.Name);

                            Debug.LogFormat("NetworkClientState: {0}", PhotonNetwork.NetworkClientState);
                                
                            this.OnEnterRoom(info);
                        });
                        cellView.Init(info.Name, info.PlayerCount, info.MaxPlayers);
                    
                        this.dic.Add(info.Name, cellView);
                    }    
                }

                
            }
        };
        
        
        this.btnJoin.onClick.AddListener(() =>
        {
            //무작위로 추출한 룸 입장 
            this.JoinRandomRoom();
        });
        
        this.btnCreateRoom.onClick.AddListener(() =>
        {
            //로비 접속후 방 만들기 
            this.CreateRoom();
        });
        
        this.btnExitRoom.onClick.AddListener(() =>
        {
            NetworkManager.instance.LeaveRoom();
        });
    }

    private void OnEnterRoom(RoomInfo info)
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = info.MaxPlayers;
        options.IsOpen = info.IsOpen;
        options.IsVisible = info.IsVisible;
        
        Debug.LogFormat("info.Name: {0}", info.Name);
        
        NetworkManager.instance.JoinOrCreateRoom(info.Name, options);   //비동기 
    }

    private void CreateRoom()
    {
        string roomName = this.roomNameInputField.text;
        if (string.IsNullOrEmpty(roomName))
        {
            Debug.Log("룸 이름을 입력해야 합니다.");
            return;
        }

        RoomOptions opt = new RoomOptions();
        opt.MaxPlayers = 20;
        opt.IsOpen = true;
        opt.IsVisible = true;

        NetworkManager.instance.CreateRoom(roomName, opt);
    }

    private void JoinRandomRoom()
    {
        string nickname = this.nicknameInputField.text;
        if (string.IsNullOrEmpty(nickname))
        {
            Debug.Log("닉네임을 입력해주세요.");
        }
        else
        {
            //서버에 요청 
            this.uiLoadingGo.SetActive(true);
            NetworkManager.instance.Connect(nickname);
        }
    }

}


// ,
// (player) =>
// {
//     Debug.LogFormat("룸 입장됨: {0}" , player);
//
//     if (PhotonNetwork.IsMasterClient)
//     {
//         //씬전환
//         NetworkSceneManager.instance.LoadLevel("GameScene", () =>
//         {
//             GameMain gameMain = GameObject.FindObjectOfType<GameMain>();
//             Debug.LogFormat("gameMain: <color=lime>{0}</color>", gameMain);
//             gameMain.Init(player);    //Player생성
//         });
//     }
//     else
//     {
//         //다른플레이어 
//         Scene scene = SceneManager.GetActiveScene();
//         Debug.LogFormat("scene : {0}", scene.name);    //LobbyScene(x)  -> GameScene (0)    
//                         
//         //나는 이미 GameScene 
//         GameMain gameMain = GameObject.FindObjectOfType<GameMain>();
//         Debug.LogFormat("gameMain: <color=lime>{0}</color>", gameMain);
//         gameMain.Init(player);    //Player생성
//     }
//
// },
// (message) =>
//     {
//         //응답을 받음 
//         this.uiLoadingGo.SetActive(false);
//         Debug.Log(message);
//     }
//     );
