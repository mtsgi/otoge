using System.Collections;
using System.Collections.Generic;
using OtoFuda.Card;
using UnityEngine;

public class CardEffect45Seed : OtofudaCardEffectBase 
{
    //札を捨てた時、このターン相手の札効果を無効化し、短冊以外の役札をデッキからランダムに1枚引いてくる。
    //(この札の効果は無効化されないまた既に発動している効果は無効化できない) 
    private OtofudaCard drawCard;
    public override void handEffect()
    {
        //最初のワンドロー処理
        base.handEffect();
        
        //いったん使用した手札の情報をNoneの音札に置き換える
        _targetPlayer.playerHand[handIndex] = PlayerManager.Instance.otofudaNone;
        
        //ワンドロー処理の代わりに発動するものなのか、ワンドロー処理に追加で発動するものなのかを後で確認する。
        //まずワンドロー
/*        if (_targetPlayer.playerDeck.Count != 0)
        {
            _players[playerID].playerHand[handIndex] = _targetPlayer.playerDeck[0];
            _players[playerID].playerHand[handIndex].setSprite(playerID,handIndex);
            _players[playerID].playerDeck.RemoveAt(0);
        }
        else
        {
            _players[playerID].playerHand[handIndex].setNone(playerID, handIndex);
        }*/

        
        //デッキ枚数が0、もしくは手札枚数が枚であれば処理終了
        if (_targetPlayer.playerDeck.Count == 0 || _targetPlayer.getActiveHandCount() == 5)
        {
            return;
        }

        var randIndex = 0;
        var deckCount = _targetPlayer.playerDeck.Count;
        var copyDeck = new List<OtofudaCard>();
        
        var stripList = new List<int>();
        Debug.Log("deck Count is "+ deckCount);
        
        //デッキ内のStripの数を数える
        for (int i = 0; i < deckCount; i++)
        {
            if (_targetPlayer.playerDeck[i]._pointType == OtofudaPointType.STRIP)
            {
                stripList.Add(i);
               // Debug.Log("STRIP is "+i);
            }
        }


        //Stripのリストの個数とデッキ枚数が同じだった場合、デッキの中身は全部Stripであることが自明になる
        if (stripList.Count == deckCount)
        {
            Debug.Log("Only Strip!");
        }
        else
        {
            do
            {
                randIndex = Random.Range(0, copyDeck.Count);
            } while (stripList.Contains(randIndex));
            
            
            _targetPlayer.playerHand[handIndex] = _targetPlayer.playerDeck[randIndex];
            _targetPlayer.playerDeck.RemoveAt(randIndex);
        }

/*
        _targetPlayer.isBarrier = true;
*/
    }
    
}
