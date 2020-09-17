using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class FumenSelectSceneTransitionData : ISceneTransitionData
{
    public MusicData musicData;
    public PlayerInfo[] playerInfos = {new PlayerInfo(), new PlayerInfo()};

    public void TestCheckParameter()
    {
    }
}


[Serializable]
public class PlayerInfo
{
    public string userName = "Guest";
    public float hiSpeed = 8.0f;

    public void TestCheckParameter()
    {
    }
}


public partial class MusicSelectManager : SingletonMonoBehaviour<MusicSelectManager>
{
    public MusicData musicData = new MusicData();

    [Serializable]
    public class KeyInputSetting
    {
        public Transform targetCanvasTransform;
        public IndexJsonReadManager targetJsonReadManager;
        public KeyCode[] keyCodes;
        public Animator[] frameWaveAnimController;
        public Text[] userInterFaceTexts;

        public void InitKeyInputSetting(PlayerInfo pInfo, int pId)
        {
            targetJsonReadManager = new IndexJsonReadManager(pInfo, pId, targetCanvasTransform);
        }
    }

    [SerializeField] private List<KeyInputSetting> keyInputSettings;
    private bool _isSelectMusic = false;

    private int _selectPlayerId;

    private FumenSelectSceneTransitionData _fumenSelectSceneData = new FumenSelectSceneTransitionData();


    private new void Awake()
    {
        base.Awake();
    }

    private async void Start()
    {
        musicData = new MusicData();

        //SceneLoadManagerからTitleSceneで受けとったデータをもらう
        if (!(SceneLoadManager.Instance.previewSceneTransitionData
            is TitleSceneManager.TitleSceneTransitionData titleTransitionData))
        {
            //もしTitleSceneから受け取れてなかったら自動で生成
            titleTransitionData = new TitleSceneManager.TitleSceneTransitionData();
        }

        //MainSceneに受け渡す用のデータにPlayerInfoを渡しておく。
        //上で生成されてれば初期値が入ったやつがくる
        _fumenSelectSceneData.playerInfos = titleTransitionData.playerInfos;
//        Debug.Log(_fumenSelectSceneData.playerInfos[0].hiSpeed);

        //IndexJsonReaderを初期化
        for (int i = 0; i < titleTransitionData.playerInfos.Length; i++)
        {
            keyInputSettings[i].InitKeyInputSetting(titleTransitionData.playerInfos[i], i);
            await keyInputSettings[i].targetJsonReadManager.Init();
        }
    }


    private void Update()
    {
        if (keyInputSettings[0].targetJsonReadManager.isSelectLevel &&
            keyInputSettings[1].targetJsonReadManager.isSelectLevel)
        {
            keyInputSettings[0].targetJsonReadManager
                .LoadScene(keyInputSettings[_selectPlayerId].targetJsonReadManager.focus);
        }

        if (Input.anyKey)
        {
            for (int i = 0; i < keyInputSettings.Count; i++)
            {
                if (Input.GetKeyDown(keyInputSettings[i].keyCodes[0]))
                {
                    if (_isSelectMusic)
                    {
                        return;
                    }

                    keyInputSettings[i].frameWaveAnimController[0].Play("Wave", 0, 0.0f);
                    keyInputSettings[i].targetJsonReadManager.ScrollUp();

/*
                    Debug.Log("上");
*/
                }
                else if (Input.GetKeyDown(keyInputSettings[i].keyCodes[1]))
                {
                    if (_isSelectMusic)
                    {
                        keyInputSettings[i].frameWaveAnimController[1].Play("Wave", 0, 0.0f);

                        keyInputSettings[0].targetJsonReadManager.Escape();
                        keyInputSettings[0].userInterFaceTexts[1].text = "下";
                        keyInputSettings[0].userInterFaceTexts[3].text = "遅";
                        keyInputSettings[0].userInterFaceTexts[4].text = "速";

                        keyInputSettings[1].targetJsonReadManager.Escape();
                        keyInputSettings[1].userInterFaceTexts[1].text = "下";
                        keyInputSettings[1].userInterFaceTexts[3].text = "遅";
                        keyInputSettings[1].userInterFaceTexts[4].text = "速";

                        _isSelectMusic = false;

                        return;
                    }


                    keyInputSettings[i].frameWaveAnimController[1].Play("Wave", 0, 0.0f);
                    keyInputSettings[i].targetJsonReadManager.ScrollDown();

/*
                    Debug.Log("下");
*/
                }
                else if (Input.GetKeyDown(keyInputSettings[i].keyCodes[2]))
                {
                    keyInputSettings[i].frameWaveAnimController[2].Play("Wave", 0, 0.0f);

                    if (_isSelectMusic)
                    {
                        keyInputSettings[i].targetJsonReadManager
                            .SelectMusic(keyInputSettings[_selectPlayerId].targetJsonReadManager.focus);
                        return;
                    }

                    //誰の選んだ楽曲なのかを管理
                    _selectPlayerId = i;

                    //曲が選択されたフラグを立てる
                    _isSelectMusic = true;
                    //下キーを戻るキーとして認識することにする
                    keyInputSettings[0].userInterFaceTexts[1].text = "戻";
                    keyInputSettings[0].userInterFaceTexts[3].text = "左";
                    keyInputSettings[0].userInterFaceTexts[4].text = "右";
                    keyInputSettings[0].targetJsonReadManager
                        .SelectMusic(keyInputSettings[i].targetJsonReadManager.focus);

                    keyInputSettings[1].userInterFaceTexts[1].text = "戻";
                    keyInputSettings[1].userInterFaceTexts[3].text = "左";
                    keyInputSettings[1].userInterFaceTexts[4].text = "右";
                    keyInputSettings[1].targetJsonReadManager
                        .SelectMusic(keyInputSettings[i].targetJsonReadManager.focus);

/*
                    Debug.Log("決定");
*/
                }
                else if (Input.GetKeyDown(keyInputSettings[i].keyCodes[3]))
                {
                    keyInputSettings[i].frameWaveAnimController[3].Play("Wave", 0, 0.0f);
                    _fumenSelectSceneData.playerInfos[i].hiSpeed =
                        Mathf.Clamp(_fumenSelectSceneData.playerInfos[i].hiSpeed - 0.5f, 0.5f, 10.0f);

                    //Ui反映
                    keyInputSettings[i].targetJsonReadManager.InputLeft(_fumenSelectSceneData.playerInfos[i].hiSpeed);

/*                    PlayerRegisterManager.Instance.hispeed[_playerId] =
                        Mathf.Clamp(PlayerRegisterManager.Instance.hispeed[_playerId] = -0.5f, 0.5f, 10.0f);*/


                }
                else if (Input.GetKeyDown(keyInputSettings[i].keyCodes[4]))
                {
                    keyInputSettings[i].frameWaveAnimController[4].Play("Wave", 0, 0.0f);

                    _fumenSelectSceneData.playerInfos[i].hiSpeed =
                        Mathf.Clamp(_fumenSelectSceneData.playerInfos[i].hiSpeed + 0.5f, 1.0f, 10.0f);
                    keyInputSettings[i].targetJsonReadManager.InputRight(_fumenSelectSceneData.playerInfos[i].hiSpeed);
/*
                    Debug.Log("右");
*/
                }
            }
        }
    }
}