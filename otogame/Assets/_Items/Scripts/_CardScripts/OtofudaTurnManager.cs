/*
using System.Collections;
using System.Collections.Generic;
using OtoFuda.Card;
using UnityEngine;

public class OtofudaTurnManager : SingletonMonoBehaviour<OtofudaTurnManager>
{

    internal bool[] playerEffectStandby;
    internal OtofudaCardEffectBase[] effects;
    
    
    internal void runCoroutine()
    {
        StopCoroutine(otofudaTurnCorouitine());
        StartCoroutine(otofudaTurnCorouitine());
        
        
    }
    
    
    private IEnumerator otofudaTurnCorouitine()
    {
        while (true)
        {
            if (playerEffectStandby[0] && playerEffectStandby[1])
            {
                

            }
            
            
            yield return null;
        }
    }
}
*/
