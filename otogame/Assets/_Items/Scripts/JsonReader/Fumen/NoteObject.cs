using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

      public List<NotesInfo> endNotes = new List<NotesInfo>();
      //endnoteが全体で何個目のノーツかを格納
      public int endNotesNum;
       
      public int option = 0;
      public float reachFrame = 0.0f;

      private void Update()
      {
          if (Input.GetKeyUp(KeyCode.A))
          {
              
          }
      }

       public void setNoteObject(int _type, int _lane, int _end, int _option, float _reach)
       {
           noteType = _type;
           lane = _lane;
           endNotes = new List<NotesInfo>();

           endNotesNum = _end;
          
           option = _option;
           reachFrame = _reach;
           
       }
   }
   
   
}

