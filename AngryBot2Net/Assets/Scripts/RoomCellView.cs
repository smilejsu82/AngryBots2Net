using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomCellView : MonoBehaviour
{
    public TMP_Text roomNameText;
    public TMP_Text playerCountText;
    public Button btn;
    
    public void Init(string roomName, int playerCount, int maxCount)
    {
        this.roomNameText.text = roomName;
        this.playerCountText.text = string.Format("{0}/{1}", playerCount, maxCount);
    }
}
