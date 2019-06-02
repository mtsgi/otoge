using System.Collections;
using System.Collections.Generic;
using OtoFuda.Fumen;
using OtoFuda.RythmSystem;
using UnityEngine;
using UnityEngine.UI;

public class InputKeyJudge : MonoBehaviour
{
    //ユーザーのキーの配列。多分10になったりする
    public KeyCode[] playerKeys = new KeyCode[5];
    //判定を表示する用のテキスト
    public Text judgeText;

    private FumenDataManager _fumenDataManager;

    private AudioSource _audioSource;

    //各レーンのノーツ数を格納する配列
    private int[] noteCount = new int[10];

    [SerializeField] private GameObject[] laneLight;

    private bool isKeyInput;
    
    private void Start()
    {
        for (int i = 0; i < laneLight.Length; i++)
        {
            laneLight[i].SetActive(false);
        }
        
        _audioSource = SoundManager.Instance.gameObject.GetComponents<AudioSource>()[0];
        _fumenDataManager = FumenDataManager.Instance;
    }
    


    [ContextMenu("test")]
    private void test()
    {
        for (int i = 0; i < FumenDataManager.Instance.timings.Length; i++)
        {
            for (int k = 0; k < FumenDataManager.Instance.timings[i].Count; k++)
            {
                Debug.Log(FumenDataManager.Instance.timings[i] [ noteCount[i] ].reachTime);
                noteCount[i]++;
            }
        }
    }

    
    //Updateでキー入力するより遅延が少ない、らしい(？)
    private void OnGUI()
    {
        
        //推したとき        
        if (Event.current.type == EventType.KeyDown)
        {
            if (Input.GetKeyDown(Event.current.keyCode))
            {
                for (int i = 0; i < playerKeys.Length; i++)
                {
                    if (Event.current.keyCode == playerKeys[i])
                    {
                        checkJudge(i);
                    }

                }
            }
            return;
        }
        
        //離したとき
        if (Input.GetKeyUp(Event.current.keyCode))
        {
            for (int i = 0; i < playerKeys.Length; i++)
            {
                if (Event.current.keyCode == playerKeys[i])
                {
                    laneLight[i].SetActive(false);
                }
            }
            return;
        }
        

        
        //判定ラインの通過を見る
        for (int i = 0; i < _fumenDataManager.timings.Length; i++)
        {
            for (int k = 0; k < _fumenDataManager.timings[i].Count; k++)
            {
                if (noteCount[i] != _fumenDataManager.timings[i].Count)
                {
                    if (_audioSource.time - _fumenDataManager.timings[i][ noteCount[i] ].reachTime >= 0.12f)
                    {
                        judgeText.text = "MISS";
                        Debug.Log("Miss");
                        noteCount[i]++;
                    }
                }

            }
        }

    }
    
    
    
    private void checkJudge(int targetLane)
    {
        laneLight[targetLane].SetActive(true);

        //レーンのカウント数が最大数と同じであればはじく
        if (_fumenDataManager.timings[targetLane].Count == noteCount[targetLane])
        {
            return;
        }
        
        //入力された時点での楽曲の再生時間と、そのレーンのノーツの到達時間の差を見る
        var time = _audioSource.time;
        var targetTime = _fumenDataManager.timings[targetLane][noteCount[targetLane]].reachTime;
        /*Debug.Log(time-targetTime);*/
        
        //ここの判定幅要修正
        //外部からまとめて指定できるようにしたい
        if (time - targetTime >= -0.15f && time - targetTime <= 0.15f)
        {
            noteCount[targetLane]++;
            judgeText.text = "PERFECT";
            Debug.Log("Perfect");
        }
        else if (-0.2f <= time - targetTime && time - targetTime <= 0.2f)
        {
            noteCount[targetLane]++;
            judgeText.text = "GOOD";
            Debug.Log("Good");
        }
        else if (time - targetTime >= -0.6f && time - targetTime <= 0.6f)
        {
            noteCount[targetLane]++;
            judgeText.text = "BAD";
            Debug.Log("Bad");
        }
        else
        {
            
        }
    }
}