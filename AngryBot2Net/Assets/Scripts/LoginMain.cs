using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LoginMain : MonoBehaviour
{
    [Header("Sign Up")] public TMP_InputField signupIdInputField;
    public TMP_InputField signupPasswordInputField;
    public Button btnSignUp;

    [Header("Sign In")] public TMP_InputField signinIdInputField;
    public TMP_InputField signinPasswordInputField;
    public Button btnSignIn;

    void Start()
    {
        this.btnSignUp.onClick.AddListener(() =>
        {
            string id = this.signupIdInputField.text;
            string pw = this.signupPasswordInputField.text;
            Debug.LogFormat("{0}, {1}", id, pw);
            StartCoroutine(RequestSignUp(id, pw));
        });
        this.btnSignIn.onClick.AddListener(() =>
        {
            string id = this.signinIdInputField.text;
            string pw = this.signinPasswordInputField.text;
            Debug.LogFormat("{0}, {1}", id, pw);
        });
    }

    private IEnumerator RequestSignUp(string id, string pw)
    {
        //요청 객체 만들기
        var req = new Packets.req_signup()
        {
            id = id,
            password = pw
        };
        //직렬화 
        string json = JsonConvert.SerializeObject(req);
        //바이트 배열로 만들어줘야함 (직렬화 문자열 -> 바이트 배열)
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
        
        using (UnityWebRequest www = UnityWebRequest.PostWwwForm("http://localhost:3000/signup", ""))
        {
            www.uploadHandler = new UploadHandlerRaw(bytes);
            www.SetRequestHeader("Content-Type", "application/json");
            yield return www.SendWebRequest();  //전송 
            //응답 
            if (www.result != UnityWebRequest.Result.Success)
            {
                //Error 
                Debug.LogErrorFormat("{0}", www.error);
            }
            else
            {
                Debug.LogFormat("{0}", www.downloadHandler.text);
            }
        }
        
    }

}
