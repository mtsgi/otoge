using System.Collections;
using System.Collections.Generic;
using OtoFuda.Card;
using UnityEngine;

namespace OtoFuda.Card
{
	public class OtofudaHandEffect_AOTAN : OtofudaHandEffectBase {
		public override void handEffect()
		{
			base.handEffect();
			Debug.Log("青短");
		}

		private void Update()
		{
		}
	}

}

