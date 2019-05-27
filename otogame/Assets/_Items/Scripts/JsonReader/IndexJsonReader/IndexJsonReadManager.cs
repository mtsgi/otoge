using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen.index;
using UnityEngine;

public class IndexJsonReadManager : MonoBehaviour
{
    [SerializeField] private string fileName = "index";
    
    internal void serializeFumendata()
    {		
    }

    [ContextMenu("test")]
    public void test()
    {
        var textAsset = Resources.Load ("FumenJsons/"+fileName) as TextAsset;
        var jsonText = textAsset.text;
        var _index = JsonUtility.FromJson<IndexInfo>(jsonText);		
        
        Debug.Log(_index.index.Count);
        Debug.Log(_index.index[0].name);
        Debug.Log(_index.index[0].artist);
        Debug.Log(_index.index[0].bpm);
        Debug.Log(_index.index[0].color[0]+" "+_index.index[0].color[1]+" "+_index.index[0].color[2]);
        Debug.Log(_index.index[0].offset);
        Debug.Log(_index.index[0].raku);
        Debug.Log(_index.index[0].easy);
        Debug.Log(_index.index[0].normal);
        Debug.Log(_index.index[0].hard);
        Debug.Log(_index.index[0].extra);
        Debug.Log(_index.index[0].author);
        Debug.Log(_index.index[0].comment);    }
}
