using System.Collections;
using System.Collections.Generic;
using OtoFuda.Card;
using UnityEngine;

public class CardEffectBlueStrip : OtofudaCardEffectBase 
{
    //札を捨てた時、このターン相手の札効果を無効化し、短冊以外の役札をデッキからランダムに1枚引いてくる。
    //(この札の効果は無効化されないまた既に発動している効果は無効化できない) 
    private OtofudaCardScriptableObject _drawCardScriptableObject;
    [SerializeField] private int addHpWeight = 20;
    public override void HandEffect()
    {
        //最初のワンドロー処理
        base.HandEffect();
        
        /*
        //ワンドロー処理の代わりに発動するものなのか、ワンドロー処理に追加で発動するものなのかを後で確認する。
       
        //いったん使用した手札の情報をNoneの音札に置き換える
        _targetPlayer.playerHand[handIndex] = PlayerManager.Instance.otofudaNone;

        //まずワンドロー
        if (_targetPlayer.playerDeck.Count != 0)
        {
            _players[playerID].playerHand[handIndex] = _targetPlayer.playerDeck[0];
            _players[playerID].playerHand[handIndex].SetSprite(playerID,handIndex);
            _players[playerID].playerDeck.RemoveAt(0);
        }
        else
        {
            OtofudaCardScriptableObject.setNone(playerID, handIndex);
        }
        
        _targetPlayer.playerHp += addHpWeight;
        _targetPlayer.playerHPSlider.value = Mathf.Clamp(_targetPlayer.playerHp,0,_targetPlayer.playerHPSlider.maxValue);
        */

/*
        _targetPlayer.isBarrier = true;
*/
    }
    
/*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            applyHandEffect(0,3);
        }
    }
    */

}
