using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Packets
{
    //요청 객체 
    public struct req_signup
    {
        public string id;
        public string password;
    }
    
    //응답 객체 
    public class res_signup
    {
        public int code;    
        public string message;       
    }

}
