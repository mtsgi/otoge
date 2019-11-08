using System.Collections;
using System.Collections.Generic;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class OtofudaNetAPIAccessManager : SingletonMonoBehaviour<OtofudaNetAPIAccessManager>
{
    [SerializeField] private string apiUrl = "https://otofudanet-staging.herokuapp.com/api/v1/users/";
    

    public async UniTask<string> SendNfcId(string nfcId)
    {
        return await SendWebRequestCoroutine(apiUrl + nfcId);
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
    
    
    
}