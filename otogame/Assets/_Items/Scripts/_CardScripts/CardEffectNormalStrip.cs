using System.Collections;
using System.Collections.Generic;
using OtoFuda.Card;
using UnityEngine;

public class CardEffectNormalStrip : OtofudaCardEffectBase 
{
    //札を捨てた時、このターン相手の札効果を無効化し、短冊以外の役札をデッキからランダムに1枚引いてくる。
    //(この札の効果は無効化されないまた既に発動している効果は無効化できない) 
    private OtofudaCard drawCard;
    
    public override void handEffect()
    {
        //最初のワンドロー処理
        base.handEffect();
        
        //ワンドロー処理の代わりに発動するものなのか、ワンドロー処理に追加で発動するものなのかを後で確認する。
       
        //いったん使用した手札の情報をNoneの音札に置き換える
        _targetPlayer.playerHand[handIndex] = PlayerManager.Instance.otofudaNone;

        /*
        //まずワンドロー
        if (_targetPlayer.playerDeck.Count != 0)
        {
            _targetPlayer.playerHand[handIndex] = _targetPlayer.playerDeck[0];
            _targetPlayer.playerDeck.RemoveAt(0);
        }
        */

        
        //デッキ枚数が0、もしくは手札枚数が枚であれば処理終了
        if (_targetPlayer.playerDeck.Count == 0 || _targetPlayer.getActiveHandCount() == 5)
        {
            return;
        }

        var randIndex = 0;
        var deckCount = _targetPlayer.playerDeck.Count;
        var copyDeck = new List<OtofudaCard>();
        
        var stripIndexList = new List<int>();
        Debug.Log("deck Count is "+ deckCount);
        
        //デッキ内のStripの数を数える
        for (int i = 0; i < deckCount; i++)
        {
            var targetPlayerDeckCard = _targetPlayer.playerDeck[i];
            if (targetPlayerDeckCard._pointType == OtofudaPointType.STRIP)
            {
                var monthType = targetPlayerDeckCard._monthType;
                if (monthType == OtofudaMonthType.January || monthType == OtofudaMonthType.FEBRUARY ||
                    monthType == OtofudaMonthType.MARCH || monthType == OtofudaMonthType.JULY ||
                    monthType == OtofudaMonthType.SEPTEMBER || monthType == OtofudaMonthType.OCTOBER)
                {
                    stripIndexList.Add(i);
                }

                // Debug.Log("STRIP is "+i);
            }
        }
        
        //stripのインデックスリストから参照してくる
        randIndex = Random.Range(0, stripIndexList.Count);
        _targetPlayer.playerHand[handIndex] = _targetPlayer.playerDeck[randIndex];
        _targetPlayer.playerDeck.RemoveAt(randIndex);
        

/*
        _targetPlayer.isBarrier = true;
*/
    }
    
}
