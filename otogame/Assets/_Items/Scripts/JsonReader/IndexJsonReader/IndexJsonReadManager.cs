using System;
using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen.index;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IndexJsonReadManager : MonoBehaviour
{
    [SerializeField] private Transform targetCanvasgameobject;
    [SerializeField] private string fileName = "index";
    [SerializeField,Range(0,1)] private int playerID;
    

    public int focus = 0, indexLength = 0;
    private GameObject _levelselect;
    private int selectLevelindex = 1;

    private bool isLevelSelectOpen;

    public bool isSelectLevel;
    
    //シーンを多重ロードするのを防ぐためのbool
    private bool isSceneLoadStart;
    
    // Start時にjson読み込み
    private void Start() 
    {
        //ハイスピ値の初期化。ユーザー登録するならここでいろいろする
        for (int i = 0; i < 2; i++)
        {
            MusicSelectManager.HISPEED[i] = 1.0f;
        }
        
        _levelselect = targetCanvasgameobject.Find("LevelSelect").gameObject;
        _levelselect.SetActive(false);
        draw();
    }

    private void Update() {
/*        if (Input.GetKeyUp(KeyCode.UpArrow)) {

        }
        else if (Input.GetKeyUp(KeyCode.DownArrow)) {

        }
        //難易度選択画面へ遷移
        else if (Input.GetKeyUp(KeyCode.Return)) {

        }
        else if (Input.GetKeyUp(KeyCode.Escape)) {
        }*/
    }

    public void ScrollUp()
    {
        if (focus == 0) focus = indexLength - 1;
        else focus -= 1;
        draw();
    }

    public void ScrollDown()
    {
        if (focus == indexLength - 1) focus = 0;
        else focus += 1;
        draw();
    }

    public void SelectMusic(int _focus)
    {
        _levelselect.SetActive(true);
        var textAsset = Resources.Load("FumenJsons/" + fileName) as TextAsset;
        var jsonText = textAsset.text;
        var _index = JsonUtility.FromJson<IndexInfo>(jsonText);
        IndexContent m = _index.index[_focus];
        
        if (isLevelSelectOpen && !isSelectLevel)
        {
/*            MusicSelectManager.musicID = m.id;
            MusicSelectManager.jsonFilePath = m.id + "/" + m.id + "/" + m.id;
            MusicSelectManager.BPM = m.bpm;

            isSceneLoadStart = true;
            Debug.Log(MusicSelectManager.jsonFilePath);
            
            SceneManager.LoadScene("JsonReadTestScene");*/


            MusicSelectManager.LEVELS[playerID] =
                (JsonReadManager.DIFFICULTY) Enum.ToObject(typeof(JsonReadManager.DIFFICULTY), selectLevelindex);

            isSelectLevel = true;
            Debug.Log("LEVEL SELECT!");
            return;
        }
        

        _levelselect.transform.Find("MusicName").gameObject.GetComponent<Text>().text = m.name;
        _levelselect.transform.Find("MusicArtist").gameObject.GetComponent<Text>().text = m.artist;

        _levelselect.transform.Find("MusicEasyNum").gameObject.GetComponent<Text>().text = m.easy.ToString();
        _levelselect.transform.Find("MusicNormalNum").gameObject.GetComponent<Text>().text = m.normal.ToString();
        _levelselect.transform.Find("MusicHardNum").gameObject.GetComponent<Text>().text = m.hard.ToString();
            
        _levelselect.transform.Find("MusicBPM").gameObject.GetComponent<Text>().text = "BPM " + m.dispbpm;
        _levelselect.transform.Find("MusicAuthor").gameObject.GetComponent<Text>().text = "譜面制作：" + m.author;

        //// 楽曲IDからTextureのパスを取得
        string jacketPath = "FumenJsons/" + m.id + "/" + m.id;
        _levelselect.transform.Find("MusicJacket").gameObject.GetComponent<RawImage>().texture = Resources.Load<Texture>(jacketPath);

        _levelselect.transform.Find("MusicComment").gameObject.GetComponent<Text>().text = m.comment;
        
        isLevelSelectOpen = true;
        
/*
        Debug.Log("SELECTED");
*/
    }

    public void LoadScene(int _focus)
    {
        _levelselect.SetActive(true);
        var textAsset = Resources.Load("FumenJsons/" + fileName) as TextAsset;
        var jsonText = textAsset.text;
        var _index = JsonUtility.FromJson<IndexInfo>(jsonText);
        IndexContent m = _index.index[_focus];

        if (isLevelSelectOpen && isSelectLevel)
        {
            MusicSelectManager.musicID = m.id;
            MusicSelectManager.jsonFilePath = m.id + "/" + m.id + "/" + m.id;
            MusicSelectManager.BPM = m.bpm;

            isSceneLoadStart = true;
            Debug.Log(MusicSelectManager.jsonFilePath);

            SceneManager.LoadScene("OtofudaMainScene");
        }
    }

    public void Escape()
    {
        _levelselect.SetActive(false);
        isLevelSelectOpen = false;
         isSelectLevel= false;
    }
    
    public void InputLeft()
    {
        if (isSelectLevel)
        {
            return;
        }
        
        if (isLevelSelectOpen)
        {
            if (selectLevelindex == 1)
            {
                _levelselect.transform.Find("SelectFrame").position=_levelselect.transform.Find("MusicEasyColor").position;
                _levelselect.transform.Find("SelectFrame").gameObject.GetComponent<Animator>().Play("Wave", 0, 0.0f);
                selectLevelindex -= 1;

            }
            else if (selectLevelindex == 2)
            {
                _levelselect.transform.Find("SelectFrame").position=_levelselect.transform.Find("MusicNormalColor").position;
                _levelselect.transform.Find("SelectFrame").gameObject.GetComponent<Animator>().Play("Wave", 0, 0.0f);
                selectLevelindex -= 1;
            }
/*
            Debug.Log("ENTER LEFT");
*/
            return;
        }
        
        MusicSelectManager.HISPEED[playerID] = Mathf.Clamp(MusicSelectManager.HISPEED[playerID] - 0.5f, 0.5f, 10.0f);
        targetCanvasgameobject.Find("HiSpeedText").GetComponent<Text>().text =
            string.Format("HI-SPEED : {0:0.0}", MusicSelectManager.HISPEED[playerID]);
/*
        Debug.Log("HI SPEED DOWN");
*/


    }
    
    public void InputRight()
    {
        if (isSelectLevel)
        {
            return;
        }
        
        if (isLevelSelectOpen)
        {
            _levelselect.transform.Find("SelectFrame").gameObject.GetComponent<Animator>().Play("Wave", 0, 0.0f);

            if (selectLevelindex == 1)
            {
                _levelselect.transform.Find("SelectFrame").position=_levelselect.transform.Find("MusicHardColor").position;
                selectLevelindex += 1;
            }
            else if (selectLevelindex == 0)
            {
                _levelselect.transform.Find("SelectFrame").position=_levelselect.transform.Find("MusicNormalColor").position;
                selectLevelindex += 1;
            }
/*
            Debug.Log("ENTER RIGHT");
*/

            return;
        }

        MusicSelectManager.HISPEED[playerID] = Mathf.Clamp(MusicSelectManager.HISPEED[playerID] + 0.5f, 1.0f, 10.0f);
        targetCanvasgameobject.Find("HiSpeedText").GetComponent<Text>().text =
            string.Format("HI-SPEED : {0:0.0}", MusicSelectManager.HISPEED[playerID]);
/*
        Debug.Log("HI SPEED UP");
*/



    }

    internal void serializeFumendata()
    {		
    }

    [ContextMenu("曲目を生成")]
    public void draw() 
    {
        var textAsset = Resources.Load("FumenJsons/" + fileName) as TextAsset;
        var jsonText = textAsset.text;
        var _index = JsonUtility.FromJson<IndexInfo>(jsonText);
        indexLength = _index.index.Count;

        GameObject music = (GameObject)Resources.Load("FumenJsons/Prefabs/Music");

        //選択中の楽曲情報
        for( int i=-2; i<=2; i++ ) {
            var focusing = targetCanvasgameobject.Find("Music" + i.ToString());
            int idx = focus + i;
            //先頭でループ
            if (idx == -2) idx = indexLength - 2;
            else if (idx == -1) idx = indexLength - 1;
            //末尾でループ
            else if (idx == indexLength) idx = 0;
            else if (idx == indexLength + 1) idx = 1;

            IndexContent m = _index.index[idx];
            focusing.transform.Find("MusicName").gameObject.GetComponent<Text>().text = m.name;
            focusing.transform.Find("MusicArtist").gameObject.GetComponent<Text>().text = m.artist;

            focusing.transform.Find("MusicEasyNum").gameObject.GetComponent<Text>().text = m.easy.ToString();
            focusing.transform.Find("MusicNormalNum").gameObject.GetComponent<Text>().text = m.normal.ToString();
            focusing.transform.Find("MusicHardNum").gameObject.GetComponent<Text>().text = m.hard.ToString();

            focusing.transform.Find("MusicColor").GetComponent<Image>().color = new Color(m.color[0]/255, m.color[1]/255, m.color[2]/255, .3f);

            //
            if ( i == 0 ) {
                focusing.transform.Find("MusicBPM").gameObject.GetComponent<Text>().text = "BPM " + m.dispbpm;
                focusing.transform.Find("MusicAuthor").gameObject.GetComponent<Text>().text = "譜面制作：" + m.author;

                //// 楽曲IDからTextureのパスを取得
                string jacketPath = "FumenJsons/" + m.id + "/" + m.id;
                focusing.transform.Find("MusicJacket").gameObject.GetComponent<RawImage>().texture = Resources.Load<Texture>(jacketPath);
            }
        }
        
        //foreach(IndexContent v in _index.index) {
        //music.name = v.name;
        //var _music = Instantiate(music,GameObject.Find("Canvas").transform.Find("MusicScroll/Viewport/Content"));
        //_music.GetComponent<RectTransform>().localPosition += new Vector3( 0, -180 - 120 * cnt, 0 );
        //cnt++;
        //// ボタンの色
        //_music.transform.Find("MusicColor").GetComponent<Image>().color = new Color(v.color[0]/255, v.color[1]/255, v.color[2]/255, .3f);

        //// 楽曲情報をボタンに表示
        //_music.transform.Find("MusicName").gameObject.GetComponent<Text>().text = v.name;
        //_music.transform.Find("MusicArtist").gameObject.GetComponent<Text>().text = v.artist;
        //_music.transform.Find("MusicBPM").gameObject.GetComponent<Text>().text = "BPM " + v.dispbpm;
        //_music.transform.Find("MusicAuthor").gameObject.GetComponent<Text>().text = "譜面制作：" + v.author;

        //_music.transform.Find("MusicEasyNum").gameObject.GetComponent<Text>().text = v.easy.ToString();
        //_music.transform.Find("MusicNormalNum").gameObject.GetComponent<Text>().text = v.normal.ToString();
        //_music.transform.Find("MusicHardNum").gameObject.GetComponent<Text>().text = v.hard.ToString();

        //// 楽曲IDからTextureのパスを取得
        //string jacketPath = "FumenJsons/" + v.id + "/" + v.id;
        //_music.transform.Find("MusicJacket").gameObject.GetComponent<RawImage>().texture = Resources.Load<Texture>(jacketPath) ;

        //// クリック時のイベントを設定
        //_music.GetComponent<Button>().onClick.AddListener( ()=> {
        //    Debug.Log("[楽曲選択]");
        //    Debug.Log("楽曲を選択しました:" + v.name);
        //} );
        //}

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
