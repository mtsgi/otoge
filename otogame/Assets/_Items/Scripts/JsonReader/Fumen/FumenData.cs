using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OtoFuda.Fumen
{
   //BPMは別から参照

   //BPMは一秒間の拍数。
   //n/n拍子の時
   [Serializable]
   public class Note: MonoBehaviour
   {
      public int noteType = 0;
      public int lane = 0;

      public List<NotesInfo> endNotes = new List<NotesInfo>();
      public int option = 0;
      public float reachFrame = 0.0f;

      private void Update()
      {
          if (Input.GetKeyUp(KeyCode.A))
          {
              
          }
      }


      public Note(NotesInfo notesInfo, float _bpm, float _beat)
      {
          //ノーツのタイプ情報
          this.noteType = notesInfo.type;
          //レーン情報
          this.lane = notesInfo.lane;

          //endノーツが含まれていた場合、さらに生成してmainNotesの中につっこむ
          if (noteType == 2)
          {
              for (int i = 0; i < notesInfo.end.Count; i++)
              {
                  FumenDataManager.Instance.mainNotes.Add(new Note(notesInfo.end[i], _bpm, _beat));
                  this.endNotes.Add(notesInfo.end[i]);
              }
          }

          //オプション情報
          this.option = notesInfo.option;


          //一小節あたりの長さ(単位：フレーム)
          //60 秒/_bpm (拍)で 1拍 あたり何秒なのかを算出。
          //これに60をかけて1フレームあたり何秒なのかを算出する。
          //これにbeat(拍子数)をかけることで一小節あたりのフレーム数を計算する。
          var measureLength = (3600 / _bpm) * _beat;
          
          //一小節あたりのフレーム数がわかれば何フレーム目で到達するノーツなのかを算出できる。
          this.reachFrame = measureLength * ((float) notesInfo.measure - 1) +
                            measureLength * ((float) notesInfo.position / (float) notesInfo.split);


          GameObject noteGameObject;
          var spawnPos = Vector3.zero;
          var _laneLength = FumenDataManager.Instance.laneLength;

          switch (noteType)
          {
              
              case 0:
                  break;
              case 1:
                  noteGameObject = (GameObject) Resources.Load("NoteObjects/Prefabs/NormalNote");
                  spawnPos = new Vector3(lane, reachFrame * _laneLength, 0);
                  Instantiate(noteGameObject, spawnPos, Quaternion.identity);
                  break;
              case 2:
                  noteGameObject = (GameObject) Resources.Load("NoteObjects/Prefabs/NormalNote");
                  spawnPos = new Vector3(lane, reachFrame * _laneLength, 0);
                  Instantiate(noteGameObject, spawnPos, Quaternion.identity);
                  break;
              case 3:
                  noteGameObject = (GameObject) Resources.Load("NoteObjects/Prefabs/Flick_L");
                  spawnPos = new Vector3(lane, reachFrame * _laneLength, 0);
                  Instantiate(noteGameObject, spawnPos, Quaternion.identity);
                  break;
              case 4:
                  noteGameObject = (GameObject) Resources.Load("NoteObjects/Prefabs/Flick_R");
                  spawnPos = new Vector3(lane, reachFrame * _laneLength, 0);
                  Instantiate(noteGameObject, spawnPos, Quaternion.identity);
                  break;
              case 5:
                  noteGameObject = (GameObject) Resources.Load("NoteObjects/Prefabs/OtofuadNote");
                  spawnPos = new Vector3(3, reachFrame * _laneLength, 0);
                  Instantiate(noteGameObject, spawnPos, Quaternion.identity);
                  break;

              case 99:
                  break;
              
          }
      }
      
      
      
      









   }
   
   
}

