using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;
using UnityEngine;

public class Movement : MonoBehaviour, IPunObservable
{
    private PhotonView photonView;
    private CinemachineVirtualCamera virtualCamera;
    private Plane plane;
    private Ray ray;
    private CharacterController controller;
    private Animator anim;
    
    void Start()
    {
        this.photonView = this.GetComponent<PhotonView>();
        this.virtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
        this.controller = this.GetComponent<CharacterController>();
        this.anim = this.GetComponent<Animator>();
        
        if (this.photonView.IsMine)
        {
            this.virtualCamera.Follow = this.transform;
            this.virtualCamera.LookAt = this.transform;
        }

        this.plane = new Plane(this.transform.up, this.transform.position);
    }

    private void Update()
    {
        if (this.photonView.IsMine)
        {
            if (this.GetComponent<Damage>().hp <= 0) return;
            
            this.Move();
            this.Turn();
        }
        else
        {
            this.transform.position = Vector3.Lerp(this.transform.position, this.receivePos, Time.deltaTime * 10f);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, this.receiveRot, Time.deltaTime * 10f);
        }
    }

    private float H => Input.GetAxis("Horizontal");
    private float V => Input.GetAxis("Vertical");

    private void Move()
    {
        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraForward.y = 0;
        cameraRight.y = 0;

        //DrawArrow.ForDebug(this.transform.position, cameraForward * V, 0, Color.blue, ArrowType.Solid);
        //DrawArrow.ForDebug(this.transform.position, cameraRight * H, 0, Color.red, ArrowType.Solid);

        Vector3 moveDir = (cameraForward * V) + (cameraRight * H);
        moveDir.Set(moveDir.x, 0f, moveDir.z);
        
        //DrawArrow.ForDebug(this.transform.position, moveDir, 0, Color.yellow, ArrowType.Solid);

        this.controller.SimpleMove(moveDir * 10f);

        float forward = Vector3.Dot(moveDir, this.transform.forward);
        float strafe = Vector3.Dot(moveDir, transform.right);
        
        this.anim.SetFloat("Forward", forward); //-1 ~ 1
        this.anim.SetFloat("Strafe", strafe);

    }

    private void Turn()
    {
        this.ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float enter = 0f;
        if (this.plane.Raycast(ray, out enter))
        {
            Vector3 hitPoint = this.ray.GetPoint(enter);

            Vector3 startPosition = this.transform.position;
            Vector3 dir = (hitPoint - startPosition).normalized; //e - s
            //DrawArrow.ForDebug(startPosition, dir, 1f, Color.red, ArrowType.Solid);
            //방향 전환
            this.transform.localRotation = Quaternion.LookRotation(dir);
        }
    }

    private Vector3 receivePos;
    private Quaternion receiveRot;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)   //내꺼면 보낸다 
        {
            stream.SendNext(this.transform.position);
            stream.SendNext(this.transform.rotation);
        }
        else
        {
            //Remote Player가 받는다
            this.receivePos = (Vector3)stream.ReceiveNext();
            this.receiveRot = (Quaternion)stream.ReceiveNext();
        }
    }
}
