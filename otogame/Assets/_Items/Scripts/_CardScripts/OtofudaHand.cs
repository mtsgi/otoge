using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OtoFuda.Card
{
	public class OtofudaHand : MonoBehaviour
	{
		//PlayerManagerから引っ張ってくるPlayer情報。
		private PlayerManager _playerManager;
		private PlayerManager.Player[] _players;

		//HandEffectManagerから引っ張ってくる役効果一覧の辞書。
		private Dictionary<string,OtofudaHandEffectBase> _effectDictionary;
		
		//役判定クラス。
		private OtofudaHandEffectChecker cheker;
		

		private void Start()
		{
			//それぞれ初期化。
			_playerManager = PlayerManager.Instance;
			_players = _playerManager._players;

			_effectDictionary = OtofudaHandEffectManager.Instance.effectDictionary;
			
			cheker = new OtofudaHandEffectChecker();
		}
		
		//手札の効果を発動する。
		private void handEffectActivate(int playerNum)
		{
			//手札のエフェクトにチェックをかけて手札効果のタイプを取得。
			var effectType = cheker.handCheck(_players[playerNum].playerHand);
			//役なしであれば弾く。
			//effectTypeの0がNONEであれば役なしであることが確定している。
			
			if (effectType[0] == OtofudaHandEffectType.NONE)
			{
				return;
			}
			else
			{
				//格納されたEffectTypeの数だけ処理を実行。
				//辞書から参照してそれぞれのapplyhandeffectが実行する
				for (int i = 0; i < effectType.Count; i++)
				{
					_effectDictionary[effectType[i].ToString()].applyHandEffect(playerNum);
				}
			}
		}


		[ContextMenu("test")]
		private void test()
		{
			handEffectActivate(0);
		}
		
	}
	

}