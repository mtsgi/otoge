using System.Collections;
using System.Collections.Generic;
using OtoFuda.Card;
using UnityEngine;

namespace OtoFuda.Card
{
	public class OtofudaHandEffect_GOKOU : OtofudaHandEffectBase {
		public override void handEffect()
		{
			base.handEffect();
			Debug.Log("五光");
		}

		private void Update()
		{
		}
	}

}

