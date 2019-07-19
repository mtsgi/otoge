using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using OtoFuda.RythmSystem;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace OtoFuda.Fumen
{
    
    //ノーツ本体を定義するスクリプト
    
   //BPMは別から参照

   //BPMは一秒間の拍数。
   //n/n拍子の時
   [Serializable]
   public class NoteObject : MonoBehaviour
   {
      public int noteType = 0;
      public int lane = 0;
      public int playerID;

      public List<NotesInfo> endNotes = new List<NotesInfo>();
      //endnoteが全体で何個目のノーツかを格納
      private int endNotesNum;

      public int _endNotesNum
      {
          get { return endNotesNum; }
          set { endNotesNum = value; }
      }
       
      public int option = 0;
      public float reachFrame = 0.0f;
      
      private float _laneLength = 0.0f;
      public float _highSpeed = 1.0f;
      
      private bool isPlayingGame;
      
      private int frameConnt;

      private float defPosX;
      
      //Z座標はノーツの難易度が変化するとともに変わるのでinternalとかにしておく
      internal float posZ;
      
      private Stopwatch _stopwatch = new Stopwatch();

      private AudioSource _audioSource;
      
      private void Start()
      {
          _highSpeed = FumenDataManager.Instance.highSpeed[playerID];
          _laneLength = FumenDataManager.Instance.laneLength;
          defPosX = transform.position.x;
          posZ = transform.position.z;
      }

      private void LateUpdate()
      {
          
          if (isPlayingGame)
          {
              transform.transform.position = new Vector3(defPosX, (reachFrame - _audioSource.time) * _laneLength * _highSpeed, posZ);

/*              frameConnt++;
              Debug.Log(frameConnt);
              if (frameConnt == 60)
              {
                  frameConnt = 0;
                  Debug.Log("Count！");
              }*/
          }
      }
      
       public void setNoteObject(int _type, int _lane, int _end, int _option, float _reach ,int _playerID)
       {
           noteType = _type;
           lane = _lane;
           endNotes = new List<NotesInfo>();

           endNotesNum = _end;
          
           option = _option;
           reachFrame = _reach;
           playerID = _playerID;

       }

       public void changeFumenState()
       {
//           Debug.Log("stateChange");
           isPlayingGame = !isPlayingGame;
           _stopwatch.Start();
           _audioSource = SoundManager.Instance.gameObject.GetComponents<AudioSource>()[0];
       }
       
   }
   
   
}

