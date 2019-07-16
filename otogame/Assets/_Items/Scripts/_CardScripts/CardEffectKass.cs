using System.Collections;
using System.Collections.Generic;
using OtoFuda.Card;
using UnityEngine;

public class CardEffectKass : OtofudaCardEffectBase
{
    //札を捨てた時、追加でデッキから札を1枚引く。(ただし手札の上限は5枚まで) 

    public override void handEffect()
    {
        base.handEffect();
        if (_targetPlayer.playerDeck.Count == 0 && _targetPlayer.playerHand.Length == 5)
        {
            return;
        }
        _targetPlayer.playerHand[handIndex] = _targetPlayer.playerDeck[0];
        _targetPlayer.playerDeck.RemoveAt(0);
        
    }
}
