using System.Collections;
using System.Collections.Generic;
using OtoFuda.Card;
using UnityEngine;

namespace OtoFuda.Card
{
	public class OtofudaHandEffect_AKATAN : OtofudaHandEffectBase {
		public override void handEffect()
		{
			base.handEffect();
			Debug.Log("test!");
		}

		private void Update()
		{
		}
	}

}

