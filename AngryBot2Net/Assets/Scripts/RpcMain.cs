using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class RpcMain : MonoBehaviour
{
    private ChatPlayer localPlayer;
    
    public static int DateTimeToInt(DateTime dateTime)
    {
        // Unix 타임스탬프는 1970년 1월 1일부터 경과한 초를 나타냅니다.
        DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        // DateTime을 초 단위로 변환한 후 int로 변환합니다.
        return (int)(dateTime.ToUniversalTime() - unixEpoch).TotalSeconds;
    }

    public static DateTime IntToDateTime(int unixTimestamp)
    {
        // Unix 타임스탬프는 1970년 1월 1일부터 경과한 초를 나타냅니다.
        DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        
        // Unix 타임스탬프를 초 단위로 DateTime으로 변환합니다.
        return unixEpoch.AddSeconds(unixTimestamp);
    }
    
    public static string FormatDateTime(DateTime dateTime)
    {
        // 원하는 형식의 문자열을 지정합니다.
        string format = "yyyy-MM-dd tt hh:mm";

        // DateTime을 지정한 형식 문자열로 포맷합니다.
        string formattedDateTime = dateTime.ToString(format);

        return formattedDateTime;
    }
    
    void Start()
    {
        // Debug.Log(DateTime.Now);
        //
        // // 현재 로컬 시간을 Unix 타임스탬프로 변환
        // var timestamp = DateTimeToInt(DateTime.Now);
        // Debug.LogFormat("Unix Timestamp: {0}", timestamp);
        //
        // // Unix 타임스탬프를 DateTime으로 변환하여 포맷
        // var datetime = IntToDateTime(timestamp);
        // var formattedDateTime = FormatDateTime(datetime.ToLocalTime());
        // Debug.LogFormat("Formatted DateTime: {0}", formattedDateTime);
        
        // NetworkManager.instance.Connect("", (player) => { 
        //     GameObject go = PhotonNetwork.Instantiate("ChatPlayer", Vector3.zero, Quaternion.identity);
        //     var chatPlayer = go.GetComponent<ChatPlayer>();
        //     Debug.LogFormat("chatPlayer: {0}", chatPlayer);
        //     if (chatPlayer.IsMine)
        //     {
        //         this.localPlayer = chatPlayer;
        //     }
        // }, null);
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Send", GUILayout.Width(200), GUILayout.Height(80)))
        {

            int timestamp = RpcMain.DateTimeToInt(DateTime.Now);

            var info = new PhotonMessageInfo(PhotonNetwork.LocalPlayer, timestamp, this.localPlayer.pv);
            
            this.localPlayer.ChatMessage(PhotonNetwork.NickName, this.localPlayer.message, info);
            
            this.localPlayer.pv.RPC("ChatMessage", RpcTarget.Others, 
                PhotonNetwork.NickName, localPlayer.message);
        }
    }
    
    
    
}
