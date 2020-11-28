using System.Collections.Generic;
using UnityEngine;

namespace OtoFuda.Card
{
    [CreateAssetMenu(menuName = "音札/CardSetList")]
    public class OtofudaCardSetList : ScriptableObject
    {
        public List<OtofudaCardScriptableObject> otofudaCardScriptableObjects;
    }
}