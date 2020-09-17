using System;
using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen.index;
using UniRx.Async;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IndexJsonReadManager
{
    private const string GetFumenDataUri = "https://otofuda.microcms.io/api/v1/songs?limit=100";
    private const string GetFumenDataApiKey = "91c69bf8-3df5-445f-81e7-30b54ab4a7d4";

    private readonly Transform _targetCanvasTransform;
    private string fileName = "index";
    private int _playerId;


    public int focus = 0, indexLength = 0;
    private GameObject _levelselect;
    private int selectLevelindex = 1;

    private bool isLevelSelectOpen;

    public bool isSelectLevel;

    //シーンを多重ロードするのを防ぐためのbool
    private bool isSceneLoadStart;

    private PlayerInfo _playerInfo = new PlayerInfo();

    private IndexInfo _indexInfo = new IndexInfo();

    public IndexJsonReadManager(PlayerInfo playerInfo, int playerId, Transform targetCanvas)
    {
        _playerInfo = playerInfo;
        _playerId = playerId;
        _targetCanvasTransform = targetCanvas;

//        Init();
    }


    public async UniTask Init()
    {
        _levelselect = _targetCanvasTransform.Find("LevelSelect").gameObject;
        _levelselect.SetActive(false);
        var loadTask = LoadIndexJson();
        await loadTask;
//        Debug.Log(loadTask.Result);
        _indexInfo = Utf8Json.JsonSerializer.Deserialize<IndexInfo>(loadTask.Result);
        Debug.Log(_indexInfo.contents[0].song_id);

        var generateTask = TextureGenerate(_indexInfo);
        await generateTask;

        _targetCanvasTransform.Find("HiSpeedText").GetComponent<Text>().text =
            $"HI-SPEED : {_playerInfo.hiSpeed:0.0}";

        Draw();
    }

    public void ScrollUp()
    {
        if (focus == 0) focus = indexLength - 1;
        else focus -= 1;
        Draw();
    }

    public void ScrollDown()
    {
        if (focus == indexLength - 1) focus = 0;
        else focus += 1;
        Draw();
    }

    public void SelectMusic(int _focus)
    {
        _levelselect.SetActive(true);
        /*var textAsset = Resources.Load("FumenJsons/" + fileName) as TextAsset;*/
        /*var jsonText = textAsset.text;*/
        /*var _index = JsonUtility.FromJson<IndexInfo>(jsonText);*/
        IndexContent m = _indexInfo.contents[_focus];

        if (isLevelSelectOpen && !isSelectLevel)
        {
/*            MusicSelectManager.musicID = m.id;
            MusicSelectManager.jsonFilePath = m.id + "/" + m.id + "/" + m.id;
            MusicSelectManager.BPM = m.bpm;

            isSceneLoadStart = true;
            Debug.Log(MusicSelectManager.jsonFilePath);
            
            SceneManager.LoadScene("JsonReadTestScene");*/


            MusicSelectManager.Instance.musicData.levels[_playerId] =
                (GameDifficulty) Enum.ToObject(typeof(GameDifficulty), selectLevelindex);

            isSelectLevel = true;
            Debug.Log("LEVEL SELECT!");
            //難易度情報の送信
            OtofudaSerialPortManager.Instance.SendDifficultyColor(_playerId,
                (GameDifficulty) Enum.ToObject(typeof(GameDifficulty), selectLevelindex));

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
        string jacketPath = "FumenJsons/" + m.song_id + "/" + m.song_id;
        _levelselect.transform.Find("MusicJacket").gameObject.GetComponent<RawImage>().texture = m.jacket.texture;
        /*Resources.Load<Texture>(jacketPath);*/

        _levelselect.transform.Find("MusicComment").gameObject.GetComponent<Text>().text = m.comment;

        isLevelSelectOpen = true;

/*
        Debug.Log("SELECTED");
*/
    }

    private async UniTask<string> LoadIndexJson()
    {
        //ウェブリクエストを生成
        var request = UnityEngine.Networking.UnityWebRequest.Get(GetFumenDataUri);
        request.SetRequestHeader("X-API-KEY", GetFumenDataApiKey);
        //通信待ち
        await request.SendWebRequest();
        //エラーが発生したか
        if (request.isHttpError || request.isNetworkError)
        {
            //エラー内容
            Debug.Log(request.error);
            return "";
        }

        return request.downloadHandler.text;
    }

    public void LoadScene(int _focus)
    {
        _levelselect.SetActive(true);
        /*var textAsset = Resources.Load("FumenJsons/" + fileName) as TextAsset;*/
        /*var jsonText = textAsset.text;*/
        /*var _index = JsonUtility.FromJson<IndexInfo>(jsonText);*/
        IndexContent m = _indexInfo.contents[_focus];

        if (isLevelSelectOpen && isSelectLevel)
        {
            MusicSelectManager.Instance.musicData.musicId = m.song_id;
            MusicSelectManager.Instance.musicData.jsonFilePath = m.song_id + "/" + m.song_id + "/" + m.song_id;
            MusicSelectManager.Instance.musicData.bpm = m.bpm;
            MusicSelectManager.Instance.musicData.offset = m.offset;

            isSceneLoadStart = true;
            Debug.Log(MusicSelectManager.Instance.musicData.jsonFilePath);

            //このタイミングでユーザーのデータ渡すとか
            var transitionData = new FumenSelectSceneTransitionData();
            transitionData.musicData = MusicSelectManager.Instance.musicData;

/*            SceneManager.LoadScene("OtofudaMainScene");*/
            SceneLoadManager.Instance.Load(GameSceneDefine._03_MainScene, transitionData, true);
            Debug.Log("SceneLoad");
        }
    }

    public void Escape()
    {
        _levelselect.SetActive(false);
        isLevelSelectOpen = false;
        isSelectLevel = false;
    }

    public void InputLeft(float newHiSpeed)
    {
        if (isSelectLevel)
        {
            return;
        }

        if (isLevelSelectOpen)
        {
            if (selectLevelindex == 1)
            {
                _levelselect.transform.Find("SelectFrame").position =
                    _levelselect.transform.Find("MusicEasyColor").position;
                _levelselect.transform.Find("SelectFrame").gameObject.GetComponent<Animator>().Play("Wave", 0, 0.0f);
                selectLevelindex -= 1;
            }
            else if (selectLevelindex == 2)
            {
                _levelselect.transform.Find("SelectFrame").position =
                    _levelselect.transform.Find("MusicNormalColor").position;
                _levelselect.transform.Find("SelectFrame").gameObject.GetComponent<Animator>().Play("Wave", 0, 0.0f);
                selectLevelindex -= 1;
            }

/*
            Debug.Log("ENTER LEFT");
*/
            return;
        }

        _targetCanvasTransform.Find("HiSpeedText").GetComponent<Text>().text = $"HI-SPEED : {newHiSpeed:0.0}";
/*
        Debug.Log("HI SPEED DOWN");
*/
    }

    public void InputRight(float newHiSpeed)
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
                _levelselect.transform.Find("SelectFrame").position =
                    _levelselect.transform.Find("MusicHardColor").position;
                selectLevelindex += 1;
            }
            else if (selectLevelindex == 0)
            {
                _levelselect.transform.Find("SelectFrame").position =
                    _levelselect.transform.Find("MusicNormalColor").position;
                selectLevelindex += 1;
            }
/*
            Debug.Log("ENTER RIGHT");
*/

            return;
        }

/*        PlayerRegisterManager.Instance.hispeed[_playerId] =
            Mathf.Clamp(PlayerRegisterManager.Instance.hispeed[_playerId] +0.5f, 1.0f, 10.0f);*/
        _targetCanvasTransform.Find("HiSpeedText").GetComponent<Text>().text = $"HI-SPEED : {newHiSpeed:0.0}";
    }

    private void Draw()
    {
        /*var textAsset = Resources.Load("FumenJsons/" + fileName) as TextAsset;
        var jsonText = textAsset.text;
        var _index = JsonUtility.FromJson<IndexInfo>(jsonText);*/
        var index = _indexInfo;
        indexLength = index.contents.Count;

        var music = (GameObject) Resources.Load("FumenJsons/Prefabs/Music");

        //選択中の楽曲情報
        for (int i = -2; i <= 2; i++)
        {
            var focusing = _targetCanvasTransform.Find("Music" + i.ToString());
            int idx = focus + i;
            //先頭でループ
            if (idx == -2) idx = indexLength - 2;
            else if (idx == -1) idx = indexLength - 1;
            //末尾でループ
            else if (idx == indexLength) idx = 0;
            else if (idx == indexLength + 1) idx = 1;

            IndexContent m = index.contents[idx];
            focusing.transform.Find("MusicName").gameObject.GetComponent<Text>().text = m.name;
            focusing.transform.Find("MusicArtist").gameObject.GetComponent<Text>().text = m.artist;

            focusing.transform.Find("MusicEasyNum").gameObject.GetComponent<Text>().text = m.easy.ToString();
            focusing.transform.Find("MusicNormalNum").gameObject.GetComponent<Text>().text = m.normal.ToString();
            focusing.transform.Find("MusicHardNum").gameObject.GetComponent<Text>().text = m.hard.ToString();

            if (m.ColorArray != null && m.ColorArray.Length == 3)
            {
                focusing.transform.Find("MusicColor").GetComponent<Image>().color =
                    new Color(m.ColorArray[0] / 255.0f, m.ColorArray[1] / 255.0f, m.ColorArray[2] / 255.0f, .3f);
                //
                if (i == 0)
                {
                    focusing.transform.Find("MusicBPM").gameObject.GetComponent<Text>().text = "BPM " + m.dispbpm;
                    focusing.transform.Find("MusicAuthor").gameObject.GetComponent<Text>().text = "譜面制作：" + m.author;

                    //// 楽曲IDからTextureのパスを取得
                    string jacketPath = "FumenJsons/" + m.song_id + "/" + m.song_id;
                    focusing.transform.Find("MusicJacket").gameObject.GetComponent<RawImage>().texture =
                        m.jacket.texture;
                    /*Resources.Load<Texture>(jacketPath);*/

                    OtofudaSerialPortManager.Instance.SendFumenColor(_playerId, m.ColorArray);
                }
            }
        }
    }

    private async UniTask TextureGenerate(IndexInfo indexInfo)
    {
        var contents = indexInfo.contents;
        for (int i = 0; i < contents.Count; i++)
        {
            contents[i].jacket.texture = await GetTexture(contents[i].jacket.url);
        }
    }

    private async UniTask<Texture> GetTexture(string uri)
    {
        var r = UnityWebRequestTexture.GetTexture(uri);
        await r.SendWebRequest(); // UnityWebRequestをawaitできる
        return DownloadHandlerTexture.GetContent(r);
    }
}