using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ChatPlayer : MonoBehaviour
{
    public PhotonView pv;
    public string message;
    public bool IsMine => pv.IsMine;

    private void Awake()
    {
        this.pv = this.GetComponent<PhotonView>();
    }
    
    [PunRPC]
    public void ChatMessage(string nickname, string msg, PhotonMessageInfo info)
    {
        if (this.pv.IsMine) nickname = "나";

        string formatDateTime = RpcMain.FormatDateTime(RpcMain.IntToDateTime(info.SentServerTimestamp).ToLocalTime());
        
        Debug.LogFormat("[{0}] : {1}, {2}, => {3}, {4}, {5}", nickname, msg, this.pv.IsMine, info.Sender, info.photonView, formatDateTime);
    }

}
