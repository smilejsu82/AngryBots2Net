using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Fire : MonoBehaviour
{
    private PhotonView pv;
    public Transform firePose;
    public GameObject bulletPrefab;
    private ParticleSystem muzzleFlash;
    void Start()
    {
        this.pv = this.GetComponent<PhotonView>();
        this.muzzleFlash = firePose.Find("MuzzleFlash").GetComponent<ParticleSystem>();
    }
    
    void Update()
    {
        if (this.GetComponent<Damage>().hp <= 0) return;
        
        if (this.pv.IsMine && Input.GetMouseButtonDown(0))
        {
            var info = new PhotonMessageInfo(PhotonNetwork.LocalPlayer, 0, this.pv);
            this.FireBullet(info);
            this.pv.RPC("FireBullet", RpcTarget.Others);
        }
    }

    [PunRPC]
    void FireBullet(PhotonMessageInfo info)
    {
        Debug.LogFormat("FireBullet: <color=lime>{0}, {1}</color>", info.Sender, info.photonView);
        if(!muzzleFlash.isPlaying) this.muzzleFlash.Play(true);
        GameObject bulletGo = Instantiate(bulletPrefab, this.firePose.position, firePose.rotation);
        bulletGo.GetComponent<Bullet>().Init(info.Sender);
    }
}
