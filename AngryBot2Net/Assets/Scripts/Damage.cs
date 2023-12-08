using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;

public class Damage : MonoBehaviour
{
    [SerializeField] private int maxHp;
    [SerializeField] private Renderer[] renderers;  //player(Skinned Mesh Renderer), gun (Mesh Renderer)
    public int hp;
    private Player player;  //나 
    public TMP_Text debugText;
    public PhotonView pv;
    private Animator anim;
    private CharacterController cc;

    private readonly int hashDie = Animator.StringToHash("Die");
    private readonly int hashRespawn = Animator.StringToHash("Respawn");
    private void Awake()
    {
        this.pv = this.GetComponent<PhotonView>();
        this.anim = this.GetComponent<Animator>();
        this.cc = this.GetComponent<CharacterController>();
    }

    [PunRPC]
    public void Init(Player player)
    {
        this.hp = this.maxHp;
        this.player = player;
        
        Debug.LogFormat("<color=cyan>Init: {0}</color>", this.player);

        this.debugText.text = this.player.NickName + "\n" + this.hp + "/" + this.maxHp;
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.LogFormat(other.gameObject.name);
        if (this.hp > 0 && other.collider.CompareTag("BULLET"))
        {

            if (this.pv.IsMine)
            {
                this.Hit(20);
                this.pv.RPC("Hit", RpcTarget.OthersBuffered, 20);
                
                
                if (this.hp <= 0)
                {
                    var bullet = other.gameObject.GetComponent<Bullet>();
                    PlayerDie(bullet.Owner, this.player);
                    var pv = this.GetComponent<PhotonView>();
                    pv.RPC("PlayerDie", RpcTarget.Others, bullet.Owner, this.player);
                    
                    //나혼자 실행 
                    StartCoroutine(this.PlayDieAnimationAndRespawn());
                }
            }
            
        }
    }
    
    [PunRPC]
    private void Hit(int damage){
        this.hp -= damage;
        
        Debug.LogFormat("Hit: {0}", this.player); //?
        
        this.debugText.text = this.player.NickName + "\n" + this.hp + "/" + this.maxHp;
    }

    [PunRPC]
    void PlayerDie(Player killer, Player victim)
    {
        Debug.LogFormat("<color=red>{0}님이 {1}를 처치 했습니다.</color>", 
            killer.NickName, victim.NickName);
    }

    private IEnumerator PlayDieAnimationAndRespawn()
    {
        this.SetCharacterControllerVisible(false);
        this.pv.RPC(nameof(SetCharacterControllerVisible), RpcTarget.Others, false);
        
        this.anim.SetBool(this.hashRespawn, false); //Die -> Movement (Idle) 못들어오게
        this.anim.SetTrigger(this.hashDie); //죽는 애니메이션 실행 1.96초 
        yield return new WaitForSeconds(3.0f);  //왜 3초?
        Debug.Log("죽는 애니메이션 실행 완료");
        
        this.anim.SetBool(this.hashRespawn, true);  //리소폰 활성화
        this.SetPlayerVisible(false);   //안보이게 하기
        //RPC 
        this.pv.RPC(nameof(this.SetPlayerVisible), RpcTarget.Others, false);
        
        yield return new WaitForSeconds(1.5f);  //잠시후
        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);   //exclusive (1 ~ 3)  ??? 왜 (0 ~ 3까지가 아니라..) 
        transform.position = points[idx].position;  //위치 재조정 

        this.SetHp(100);
        this.pv.RPC(nameof(this.SetHp), RpcTarget.Others, 100);

        this.SetPlayerVisible(true);    //보여주기 
        this.pv.RPC(nameof(this.SetPlayerVisible), RpcTarget.Others, true);
        
        this.SetCharacterControllerVisible(true);
        this.pv.RPC(nameof(SetCharacterControllerVisible), RpcTarget.Others, true); 
    }

    [PunRPC]
    private void SetCharacterControllerVisible(bool enabled)
    {
        this.cc.enabled = enabled;
    }

    [PunRPC]
    private void SetHp(int hp)
    {
        if (hp >= this.maxHp)
            this.hp = this.maxHp; //현재 체력 올려주기
        else
            this.hp = hp;
        
        this.debugText.text = this.player.NickName + "\n" + this.hp + "/" + this.maxHp;
    }

    

    [PunRPC]
    private void SetPlayerVisible(bool isVisible)
    {
        foreach (var renderer in this.renderers)
        {
            renderer.enabled = isVisible;
        }
    }
}
