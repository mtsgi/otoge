using System.Collections;
using System.Collections.Generic;
using OtoFuda.Card;
using UnityEngine;

namespace OtoFuda.Card
{
	public class OtofudaHandEffect_TANE : OtofudaHandEffectBase {
		public override void handEffect()
		{
			base.handEffect();
			Debug.Log("タネ");
		}

		private void Update()
		{
		}
	}

}

