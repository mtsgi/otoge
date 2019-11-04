using System.Collections;
using System.Collections.Generic;
using OtoFuda.Card;
using OtoFuda.Fumen;
using UnityEngine;

public class CardEffect67910Seed : OtofudaCardEffectBase 
{
    //札を捨てた時、このターン相手の札効果を無効化し、短冊以外の役札をデッキからランダムに1枚引いてくる。
    //(この札の効果は無効化されないまた既に発動している効果は無効化できない) 
    [Header("その難易度が非アクティブになった時のposZ")]
    [SerializeField] private float inactivePosZ = 0.3f;
    
    
    private OtofudaCard drawCard;


/*    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            applyHandEffect(0, 0);
            Debug.Log("Effect");
        }
        
    }*/
    
    
    public override void handEffect()
    {
        //最初のワンドロー処理
        base.handEffect();
        
        //いったん使用した手札の情報をNoneの音札に置き換える
        _players[playerID].playerHand[handIndex] = PlayerManager.Instance.otofudaNone;
        

        //まずワンドロー
        if (_targetPlayer.playerDeck.Count != 0)
        {
            _players[playerID].playerHand[handIndex] = _targetPlayer.playerDeck[0];
            _players[playerID].playerHand[handIndex].setSprite(playerID,handIndex);
            _players[playerID].playerDeck.RemoveAt(0);
        }
        else
        {
            _players[playerID].playerHand[handIndex].setNone(playerID, handIndex);
        }
        


        var _noteObjects = FumenDataManager.Instance.mainNotes[playerID];
        var _moreDifObjects = FumenDataManager.Instance.moreDifficultNotes[playerID];
        
        getBetweenNotes();
        
/*
        _targetPlayer.noteCounters[(int)_targetPlayer.FumenState]
*/
        
/*        _targetPlayer.*/
/*
        _targetPlayer.isBarrier = true;
*/
    }

    private void getBetweenNotes()
    {

        //Debug.Log("<color=red>EFFECT!</color>");
        var notteObjectList = new List<NoteObject>();
        var _noteObjects = FumenDataManager.Instance.mainNotes[playerID];
        var _moreDifficultNoteObjects = FumenDataManager.Instance.moreDifficultNotes[playerID];
        
/*        Debug.Log(_noteObjects.Count);
        Debug.Log(_moreDifficultNoteObjects.Count);*/


        //全体で何ノーツ目かが引っ張ってこれればおk
        var noteCount = _targetPlayer.noteSimpleCount;
        
        for (int i = 0; i < _noteObjects.Count; i++)
        {
            //現在のノーツから次の音札ノーツまでのTransformを操作してユーザーから見えなくする。
            _noteObjects[i].posZ = inactivePosZ;
            if (_noteObjects[i].noteType == 5)
            {
                break;
            }
            
        } 
        
        // Debug.Log("StartCount");
        for (int i = 0; i < _moreDifficultNoteObjects.Count; i++)
        {
            //現在のノーツから次の音札ノーツまでのTransformを操作してユーザーから見えるようにする。
            _moreDifficultNoteObjects[i].posZ = 0;
            //Debug.Log(_moreDifficultNoteObjects[i].transform.position);
            
            if (_noteObjects[i].noteType == 5)
            {
                break;
            }
        }
        
        _targetPlayer.FumenState = PlayerFumenState.MORE_DIFFICULT;
        
        //   Debug.Log("EndCount");

    }
    
    private void Update()
    {
/*        if (Input.GetKeyDown(KeyCode.A))
        {
            applyHandEffect(0,3);
        }*/
    }
    
    
    
}
