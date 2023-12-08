using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;

public class PlayerCellView : MonoBehaviour
{
    public TMP_Text nicknameText;
    public GameObject masterMark;

    public void Init(Player player)
    {
        this.nicknameText.text = player.NickName;
        this.masterMark.SetActive(player.IsMasterClient);
    }
}
