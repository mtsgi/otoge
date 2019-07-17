using System.Collections;
using System.Collections.Generic;
using OtoFuda.Card;
using UnityEngine;

namespace OtoFuda.Card
{
	public class OtofudaHandEffect_ : OtofudaHandEffectBase {
		public override void handEffect()
		{
			base.handEffect();
			Debug.Log("猪鹿蝶");
		}

		private void Update()
		{
		}
	}

}

