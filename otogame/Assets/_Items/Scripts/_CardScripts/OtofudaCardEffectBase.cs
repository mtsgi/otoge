using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OtoFuda.Card
{
    public class OtofudaCardEffectBase : MonoBehaviour {


        private OtofudaHandEffectType _handEffectCheck(OtofudaCardBase[] otofudaCards)
        {
            return 0;
        }


        private OtofudaHandEffectType check( OtofudaCardBase[] cards )
        {
            var cardCount = cards.Length;
            //手札枚数が三枚未満であれば効果は発生しないのではじく。
            if (cardCount < 3)
            {
                return OtofudaHandEffectType.NONE;
            }

            //枚数によって役が固定されているので、forでいろいろ回すより弾いた方が速い
            if (cardCount == 3)
            {
                List<OtofudaFlowerType> flowers = new List<OtofudaFlowerType>();
                List<OtofudaPointType> points = new List<OtofudaPointType>();
                for (int i = 0; i < cardCount; i++)
                {
                    flowers[i] = cards[i]._flowerType;
                    points[i] = cards[i]._pointType;
                }

                //猪鹿蝶の判定
                if (flowers[0] == OtofudaFlowerType.BOTAN && points[0] == OtofudaPointType.SEED &&
                    flowers[1] == OtofudaFlowerType.HAGI && points[1] == OtofudaPointType.SEED &&
                    flowers[2] == OtofudaFlowerType.MOMIJI && points[2] == OtofudaPointType.SEED)
                {
                    return OtofudaHandEffectType.INOSHIKACHOU;
                }
                
                //赤短の判定
                if (flowers[0] == OtofudaFlowerType.MATSU && points[0] == OtofudaPointType.STRIP &&
                    flowers[1] == OtofudaFlowerType.UME && points[1] == OtofudaPointType.STRIP &&
                    flowers[2] == OtofudaFlowerType.SAKURA && points[2] == OtofudaPointType.STRIP)
                {
                    return OtofudaHandEffectType.AKATAN;
                }
                
                //青短の判定
                if (flowers[0] == OtofudaFlowerType.MATSU && points[0] == OtofudaPointType.STRIP &&
                    flowers[1] == OtofudaFlowerType.KIKU && points[1] == OtofudaPointType.STRIP &&
                    flowers[2] == OtofudaFlowerType.MOMIJI && points[2] == OtofudaPointType.STRIP)
                {
                    return OtofudaHandEffectType.AOTAN;
                }
                
                //三光の判定
                if (points[0] == OtofudaPointType.SHINE &&
                    points[1] == OtofudaPointType.SHINE &&
                    points[2] == OtofudaPointType.SHINE)
                {
                    return OtofudaHandEffectType.SANKOU;
                }
                
            }
            
            
            //枚数によって役が固定されているので、forでいろいろ回すより弾いた方が速い
            if (cardCount == 4)
            {
                List<OtofudaFlowerType> flowers = new List<OtofudaFlowerType>();
                List<OtofudaPointType> points = new List<OtofudaPointType>();
                for (int i = 0; i < cardCount; i++)
                {
                    flowers[i] = cards[i]._flowerType;
                    points[i] = cards[i]._pointType;
                }
                
                //四光の判定
                if (points[0] == OtofudaPointType.SHINE &&
                    points[1] == OtofudaPointType.SHINE &&
                    points[2] == OtofudaPointType.SHINE &&
                    points[3] == OtofudaPointType.SHINE )
                {
                    //雨を含んだら雨四光
                    if (flowers.Contains(OtofudaFlowerType.YANAGI))
                    {
                        return OtofudaHandEffectType.AMESHIKOU;
                    }
                    return OtofudaHandEffectType.SHIKOU;
                }

            }
            
            //枚数によって役が固定されているので、forでいろいろ回すより弾いた方が速い
            if (cardCount == 5)
            {
                List<OtofudaFlowerType> flowers = new List<OtofudaFlowerType>();
                List<OtofudaPointType> points = new List<OtofudaPointType>();
                for (int i = 0; i < cardCount; i++)
                {
                    flowers[i] = cards[i]._flowerType;
                    points[i] = cards[i]._pointType;
                }
                
                //四光の判定
                if (points[0] == OtofudaPointType.SHINE &&
                    points[1] == OtofudaPointType.SHINE &&
                    points[2] == OtofudaPointType.SHINE &&
                    points[3] == OtofudaPointType.SHINE &&
                    points[4] == OtofudaPointType.SHINE )
                {
                    return OtofudaHandEffectType.GOKOU;                    
                }
                
                //短冊の判定
                if (points[0] == OtofudaPointType.STRIP &&
                    points[1] == OtofudaPointType.STRIP &&
                    points[2] == OtofudaPointType.STRIP &&
                    points[3] == OtofudaPointType.STRIP &&
                    points[4] == OtofudaPointType.STRIP )
                {
                    return OtofudaHandEffectType.TANZAKU;                
                }
                
                //短冊の判定
                if (points[0] == OtofudaPointType.SEED &&
                    points[1] == OtofudaPointType.SEED &&
                    points[2] == OtofudaPointType.SEED &&
                    points[3] == OtofudaPointType.SEED &&
                    points[4] == OtofudaPointType.SEED )
                {
                    return OtofudaHandEffectType.TANE;                    
                }

            }

        }
        
    }
    
    
    

}

