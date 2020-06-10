using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UniRx;
using UniRx.Async;
using UnityEngine;
using UnityEngine.SceneManagement;


//SingletonMonoBehaviourを継承していて尚且つTDataを実装している必要がある
public class SceneLoadManager : SingletonMonoBehaviour<SceneLoadManager>
{
    public ISceneTransitionData previewSceneTransitionData;

    private new void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
    }
    
    // シーンにデータを渡してからシーンを読み込む
    public void Load(GameSceneDefine gameScene, ISceneTransitionData entrySceneTransitionData, bool testFlag = false)
    {
        // SceneManager.sceneLoaded に登録するローカル関数
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Instance.previewSceneTransitionData = entrySceneTransitionData;

            if (testFlag)
            {
                Instance.previewSceneTransitionData.TestCheckParameter();
            }
        }

        // シーンにデータを渡すローカル関数を登録しておく
        SceneManager.sceneLoaded += OnSceneLoaded;

        var sceneName = gameScene.ToString();
        SceneManager.LoadScene(sceneName);
    }

    public async void LoadWithTransition(GameSceneDefine gameScene, ISceneTransitionData entrySceneTransitionData,
        Func<UniTask> loadStart, 
        Func<UniTask> loadEnd,
        bool testFlag = false)
    {
        var sceneName = gameScene.ToString();
        
        //SceneLoad時に実行される関数
        async void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            //LOadが終わったらアクションを破棄しよう
            SceneManager.sceneLoaded -= OnSceneLoaded;
            
            //自分自身にもらったデータを与えておく
            previewSceneTransitionData = entrySceneTransitionData;
            
            //テストフラグがオンだったらデータの構造体のなかで定義されてるテスト関数を実行
            if (testFlag)
            {
                previewSceneTransitionData.TestCheckParameter();
            }
            
            //Load終了Fadeを実行
            await loadEnd.Invoke();
        }

        // シーンにデータを渡すローカル関数を登録しておく
        SceneManager.sceneLoaded += OnSceneLoaded;

        
        //Fade処理をかける
        await loadStart.Invoke();

        await SceneManager.LoadSceneAsync(sceneName);
    }
}

public interface ISceneTransitionData
{
    void TestCheckParameter();
}