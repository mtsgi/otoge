using System.Collections;
using System.Collections.Generic;
using OtoFuda.Card;
using UnityEngine;

public class CardEffectAprilSeed : OtofudaCardEffectBase 
{
    //札を捨てた時、このターン相手の札効果を無効化し、短冊以外の役札をデッキからランダムに1枚引いてくる。
    //(この札の効果は無効化されないまた既に発動している効果は無効化できない) 
    private OtofudaCard drawCard;
    public override void handEffect()
    {
        base.handEffect();
        if (_targetPlayer.playerDeck.Count == 0)
        {
            return;
        }

        var randIndex = 0;
        var deckCount = _targetPlayer.playerDeck.Count;
        var copyDeck = new List<OtofudaCard>();
        
        var stripList = new List<int>();
        Debug.Log("deck Count is "+ deckCount);
        
        for (int i = 0; i < deckCount; i++)
        {
            if (_targetPlayer.playerDeck[i]._pointType == OtofudaPointType.STRIP)
            {
                stripList.Add(i);
                Debug.Log("STRIP is "+i);
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
    }
    
}
