using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NetworkSceneManager : MonoBehaviour
{
    public static NetworkSceneManager instance;
    
    private void Awake()
    {
        //다른씬에 또 있다면 지우고 현재 인스턴스를 하나로 유지 
        if (NetworkSceneManager.instance != null && NetworkSceneManager.instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            NetworkSceneManager.instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void LoadLevel(string sceneName, System.Action callback)
    {
        StartCoroutine(this.CoLoadLevel(sceneName, callback));
    }

    private IEnumerator CoLoadLevel(string sceneName, System.Action callback)
    {
        PhotonNetwork.LoadLevel(sceneName);
        while (true)
        {
            Debug.LogFormat("{0}%", PhotonNetwork.LevelLoadingProgress);    //1.0
            if (PhotonNetwork.LevelLoadingProgress >= 1.0f)
            {
                break;
            }
            yield return null;
        }
        Debug.Log("씬 로딩 완료");
        callback();
    }
}
