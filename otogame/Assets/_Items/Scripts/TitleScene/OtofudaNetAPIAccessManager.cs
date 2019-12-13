using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class OtofudaNetAPIAccessManager : SingletonMonoBehaviour<OtofudaNetAPIAccessManager>
{
    [SerializeField] private string apiUrl = "https://otofudanet-staging.herokuapp.com/api/v1/users/";
    //SecurityTokenKeyWard
    private string SecurityKeyWord = "otofuda";
    private readonly StringBuilder _accessTokenBuilder = new StringBuilder();
    

    public async UniTask<string> SendNfcId(string nfcId)
    {
        var uri = $"{apiUrl}{nfcId}?token={GenerateAccessToken()}";
        Debug.Log(uri);
        _accessTokenBuilder.Clear();
        return await SendWebRequestCoroutine(uri);
    }
    
    private async UniTask<string> SendWebRequestCoroutine(string uri)
    {
        using (var www = UnityWebRequest.Get(uri))
        {
            await www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
                return www.error;
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text);

                return www.downloadHandler.text;
            }
        }
    }
    
    private string GenerateAccessToken()
    {
        var date = DateTime.Today;
        _accessTokenBuilder.Append(date.Month);
        _accessTokenBuilder.Append(date.Day);
        _accessTokenBuilder.Append(SecurityKeyWord);

        var securityKey = _accessTokenBuilder.ToString();

        return CalculateMd5(securityKey);
    }
    
    //めんどくせぇコピペだけどこれでいいや
    private string CalculateMd5(string srcStr)
    {
        var md5 = MD5.Create();
        // md5ハッシュ値を求める
        var srcBytes = Encoding.UTF8.GetBytes(srcStr);
        var destBytes = md5.ComputeHash(srcBytes);

        // 求めたmd5値を文字列に変換する
        var md5hashBuilder = new StringBuilder();
        foreach (var curByte in destBytes)
        {
            md5hashBuilder.Append(curByte.ToString("x2"));
        }

        // 変換後の文字列を返す
        return md5hashBuilder.ToString();
    }



    public async UniTask<Texture> GetQRCodeImage(string uri)
    {
        var webRequest = UnityWebRequestTexture.GetTexture(uri);

        await webRequest.SendWebRequest();

        return DownloadHandlerTexture.GetContent(webRequest);
    }
}