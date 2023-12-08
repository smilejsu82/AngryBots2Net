using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayersPanel : MonoBehaviour
{
    public Transform[] emptyAnchors;
    public Button btnPlay;
    public Button btnLeave;
    public TMP_Text roomNameText;
    public TMP_Text playerCountText;
    
    private List<PlayerCellView> list = new List<PlayerCellView>();
    
    public GameObject playerCellViewPrefab;
    
    public void Init(string roomName)
    {
        this.btnPlay.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        this.roomNameText.text = roomName;
        
        this.UpdatePlayerCountText();
    }

    public void AddPlayerCellView(Player player)
    {
        int index = this.list.Count;    //0 , 1
        var anchor = this.emptyAnchors[index];
        var go = Instantiate(this.playerCellViewPrefab, anchor);
        var playerCellView = go.GetComponent<PlayerCellView>();
        
        playerCellView.Init(player);
        
        list.Add(playerCellView);
        
        this.UpdatePlayerCountText();
    }

    private void UpdatePlayerCountText()
    {
        var currentRoom = PhotonNetwork.CurrentRoom;
        this.playerCountText.text = string.Format("{0}/{1}", currentRoom.PlayerCount, currentRoom.MaxPlayers);
    }
}
