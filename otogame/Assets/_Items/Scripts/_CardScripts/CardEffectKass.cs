using System.Collections;
using System.Collections.Generic;
using OtoFuda.Card;
using UnityEngine;

public class CardEffectKass : OtofudaCardEffectBase
{
    //札を捨てた時、追加でデッキから札を1枚引く。(ただし手札の上限は5枚まで)
    //つまり計二回ドロー

    public override void HandEffect()
    {
        base.HandEffect();

        //いったん使用した手札の情報をNoneの音札に置き換える

        //使用したカードを破棄する
        _deckController.DiscardHand(_handIndex);
        //ドローする
        _deckController.Draw();
        
    }
}