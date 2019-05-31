using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen.index;
using UnityEngine;
using UnityEngine.UI;

public class IndexJsonReadManager : MonoBehaviour
{
    [SerializeField] private string fileName = "index";
    
    internal void serializeFumendata()
    {		
    }

    [ContextMenu("曲目を生成")]
    public void test()
    {
        var textAsset = Resources.Load ("FumenJsons/"+fileName) as TextAsset;
        var jsonText = textAsset.text;
        var _index = JsonUtility.FromJson<IndexInfo>(jsonText);

        GameObject music = (GameObject)Resources.Load("FumenJsons/Prefabs/Music");

        float cnt = 0f;
        foreach(IndexContent v in _index.index) {
            music.name = v.name;
            var _music = Instantiate(music,GameObject.Find("Canvas").transform.Find("MusicScroll/Viewport/Content"));
            _music.GetComponent<RectTransform>().localPosition += new Vector3( 0, -180 - 120 * cnt, 0 );
            cnt++;
            // ボタンの色
            _music.transform.Find("MusicColor").GetComponent<Image>().color = new Color(v.color[0]/255, v.color[1]/255, v.color[2]/255, .2f);
            Debug.Log(v.color[0]+" "+v.color[1]+" "+v.color[2]);
            // 楽曲情報
            _music.transform.Find("MusicName").gameObject.GetComponent<Text>().text = v.name;
            _music.transform.Find("MusicArtist").gameObject.GetComponent<Text>().text = v.artist;
            _music.transform.Find("MusicBPM").gameObject.GetComponent<Text>().text = "BPM " + v.bpm.ToString();
            _music.transform.Find("MusicAuthor").gameObject.GetComponent<Text>().text = "譜面制作：" + v.author;

            _music.transform.Find("MusicEasyNum").gameObject.GetComponent<Text>().text = v.easy.ToString();
            _music.transform.Find("MusicNormalNum").gameObject.GetComponent<Text>().text = v.normal.ToString();
            _music.transform.Find("MusicHardNum").gameObject.GetComponent<Text>().text = v.hard.ToString();

            // クリック時
            _music.GetComponent<Button>().onClick.AddListener( ()=> {
                Debug.Log("楽曲を選択しました:" + v.name);
            } );
        }
        
        //Debug.Log(_index.index.Count);
        //Debug.Log(_index.index[0].name);
        //Debug.Log(_index.index[0].artist);
        //Debug.Log(_index.index[0].bpm);
        //Debug.Log(_index.index[0].color[0]+" "+_index.index[0].color[1]+" "+_index.index[0].color[2]);
        //Debug.Log(_index.index[0].offset);
        //Debug.Log(_index.index[0].raku);
        //Debug.Log(_index.index[0].easy);
        //Debug.Log(_index.index[0].normal);
        //Debug.Log(_index.index[0].hard);
        //Debug.Log(_index.index[0].extra);
        //Debug.Log(_index.index[0].author);
        //Debug.Log(_index.index[0].comment);
    }
}
