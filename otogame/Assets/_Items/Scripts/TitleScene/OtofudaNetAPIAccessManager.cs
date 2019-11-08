using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class OtofudaNetAPIAccessManager : SingletonMonoBehaviour<OtofudaNetAPIAccessManager>
{
    [SerializeField] private string apiUrl = "https://otofudanet-staging.herokuapp.com/api/v1/users/";
    public string testId = "senshudaigakuisgod";
    public Text testText;

    [ContextMenu("test")]
    public void Test()
    {
        StartCoroutine(SendWebRequestCoroutine(apiUrl + testId));
    }


    public void SendNfcId(string nfcId)
    {
        StartCoroutine(SendWebRequestCoroutine(apiUrl + nfcId));
    }
    
    private IEnumerator SendWebRequestCoroutine(string uri)
    {
        using (var www = UnityWebRequest.Get(uri))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                Debug.Log(www.downloadHandler.text);
                var getData = new OtofudaAPIJSON();
                getData = JsonUtility.FromJson<OtofudaAPIJSON>(www.downloadHandler.text);

                var accessCode = getData.data.public_uid; 
                Debug.Log(accessCode);
                testText.text = accessCode.ToUpper();
            }
        }
    }
    
    
    
}