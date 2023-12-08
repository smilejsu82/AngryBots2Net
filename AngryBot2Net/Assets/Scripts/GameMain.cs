using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameMain : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> prefabs;
    // public TMP_InputField nicknameInputField;
    // public Button btnConnect;
    
    void Start()
    {
        Debug.Log("<color=cyan>Start</color>");
        
        // this.btnConnect.onClick.AddListener(() =>
        // {
        //     this.nicknameInputField.gameObject.SetActive(false);
        //     this.btnConnect.gameObject.SetActive(false);
        //     
        //     NetworkManager.instance.Connect(nicknameInputField.text, (player) =>
        //     {
        //         this.CreatePlayer(player);
        //     });   
        // });
    }

    public void Init(Player player)
    {
        Debug.Log("<color=cyan>Init</color>");
        
        var pool = PhotonNetwork.PrefabPool as DefaultPool;
        foreach (GameObject prefab in this.prefabs)
        {
            pool.ResourceCache.Add(prefab.name, prefab);
        }
        
        this.CreatePlayer(player);
    }

    private void CreatePlayer(Player player)
    {
        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);   // 1 ~ 3
        Transform initPoint = points[idx];
        GameObject go = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Player"),
            initPoint.position, initPoint.rotation, 0);
        
        Damage damage =go.GetComponent<Damage>(); 
        damage.Init(player);
        //RPC 호출
        damage.pv.RPC("Init", RpcTarget.OthersBuffered, player);
        
        Debug.Log("플레이어가 생성되었습니다.");
        
    }
    
    
}
