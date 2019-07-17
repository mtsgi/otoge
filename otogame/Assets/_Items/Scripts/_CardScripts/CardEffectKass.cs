﻿using System.Collections;
using System.Collections.Generic;
using OtoFuda.Card;
using UnityEngine;

public class CardEffectKass : OtofudaCardEffectBase
{
    //札を捨てた時、追加でデッキから札を1枚引く。(ただし手札の上限は5枚まで) 

    public override void handEffect()
    {
        base.handEffect();
        
        //いったん使用した手札の情報をNoneの音札に置き換える
        _targetPlayer.playerHand[handIndex] = PlayerManager.Instance.otofudaNone;
        
        //まずワンドロー
        if (_targetPlayer.playerDeck.Count != 0)
        {
            _targetPlayer.playerHand[handIndex] = _targetPlayer.playerDeck[0];
            _targetPlayer.playerDeck.RemoveAt(0);
        }
        
        //プレイヤーのデッキ枚数が0もしくはNone以外のカード(つまり手札枚数)が五枚であれば処理を終了
        if (_targetPlayer.playerDeck.Count == 0 || _targetPlayer.getActiveHandCount() == 5)
        {
            return;
        }
        
        //追加でドロー
        _targetPlayer.playerHand[_targetPlayer.getNoneHandIndex()] = _targetPlayer.playerDeck[0];
        _targetPlayer.playerDeck.RemoveAt(0);
        
    }
}
