using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class OtofudaAPIJSON
{
    public string status = "";
    public UserData data = new UserData();
}

[Serializable]
public class UserData
{
    public string name = "";
    public string nfcid = "";
    public string public_uid = "";
    public bool registered = false;
    public bool slowfast = false;
    public int hispeed = 0;
    public string qr = "";
}
