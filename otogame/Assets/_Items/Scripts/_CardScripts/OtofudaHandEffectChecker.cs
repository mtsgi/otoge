using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OtoFuda.Card
{
    public class OtofudaHandEffectChecker
    {

        private OtofudaType otofudaType = new OtofudaType();

        private List<OtofudaHandEffectType> handeffectTypes = new List<OtofudaHandEffectType>();
        
        internal List<OtofudaHandEffectType> handCheck(OtofudaCardScriptableObject[] cards)
        {
            var cardCount = cards.Length;
            
            //手札枚数が2枚未満であれば効果は発生しないのではじく。
            if (cardCount < 2)
            {
                handeffectTypes.Add(OtofudaHandEffectType.NONE);
                return handeffectTypes;
            }
            
            //手札の情報をかき集める。
            OtofudaFlowerType[] flowers = new OtofudaFlowerType[cardCount];
            OtofudaPointType[] points = new OtofudaPointType[cardCount];
                        
            for (int i = 0; i < cardCount; i++)
            {
                flowers[i] = otofudaType.GetOtofudaFlowerFromMonth(cards[i].monthType);
                points[i] = cards[i].pointType;
            }
            
            if (cardCount >= 2)
            {
                //月見で一杯の判定
                if (checkhandValue(flowers,points,OtofudaFlowerType.KIKU,OtofudaPointType.SEED) &&
                    checkhandValue(flowers,points,OtofudaFlowerType.SUSUKI,OtofudaPointType.SHINE))
                {
                    handeffectTypes.Add(OtofudaHandEffectType.TSUKIMI);
                }
                //花見で一杯の判定
                if (checkhandValue(flowers,points,OtofudaFlowerType.KIKU,OtofudaPointType.SEED) &&
                    checkhandValue(flowers,points,OtofudaFlowerType.SAKURA,OtofudaPointType.SHINE))
                {
                    handeffectTypes.Add(OtofudaHandEffectType.HANAMI);
                }

            }
            
            

            
            //枚数によって役が固定されているので、forでいろいろ回すより弾いた方が速い
            if (cardCount >= 3)
            {

                //猪鹿蝶の判定
                if (checkhandValue(flowers,points,OtofudaFlowerType.BOTAN,OtofudaPointType.SEED) &&
                    checkhandValue(flowers,points,OtofudaFlowerType.HAGI,OtofudaPointType.SEED) &&
                    checkhandValue(flowers,points,OtofudaFlowerType.MOMIJI,OtofudaPointType.SEED))
                {
                    handeffectTypes.Add(OtofudaHandEffectType.INOSHIKACHOU);
                }

                //赤短の判定
                if (checkhandValue(flowers, points, OtofudaFlowerType.MATSU, OtofudaPointType.STRIP) &&
                    checkhandValue(flowers, points, OtofudaFlowerType.UME, OtofudaPointType.STRIP) &&
                    checkhandValue(flowers, points, OtofudaFlowerType.SAKURA, OtofudaPointType.STRIP))
                {
                    handeffectTypes.Add(OtofudaHandEffectType.AKATAN);
                }

                //青短の判定
                if (checkhandValue(flowers, points, OtofudaFlowerType.BOTAN, OtofudaPointType.STRIP) &&
                    checkhandValue(flowers, points, OtofudaFlowerType.KIKU, OtofudaPointType.STRIP) &&
                    checkhandValue(flowers, points, OtofudaFlowerType.MOMIJI, OtofudaPointType.STRIP))
                {
                    handeffectTypes.Add(OtofudaHandEffectType.AOTAN);
                }

                //三光の判定
                if (points.Select(x => x == OtofudaPointType.SHINE).Count() >= 3)
                {
                    handeffectTypes.Add(OtofudaHandEffectType.SANKOU);
                }

            }


            //枚数によって役が固定されているので、forでいろいろ回すより弾いた方が速い
            if (cardCount >= 4)
            {
                //四光の判定
                if (points.Select(x => x == OtofudaPointType.SHINE).Count() >= 4)
                {
                    //雨を含んだら雨四光
                    if (flowers.Contains(OtofudaFlowerType.YANAGI))
                    {
                        handeffectTypes.Add(OtofudaHandEffectType.AMESHIKOU);
                    }
                    //含まなかったらただの四光
                    else
                    {
                        handeffectTypes.Add(OtofudaHandEffectType.SHIKOU);
                    }
                }

            }

            //枚数によって役が固定されているので、forでいろいろ回すより弾いた方が速い
            if (cardCount == 5)
            {

                //五光の判定
                if (points.Select((x => x == OtofudaPointType.SHINE)).Count() == 5)
                {
                    handeffectTypes.Add(OtofudaHandEffectType.GOKOU);
                    return handeffectTypes;
                }

                //短冊の判定
                if (points.Select(x => x == OtofudaPointType.STRIP).Count() == 5)
                {
                    handeffectTypes.Add(OtofudaHandEffectType.TANZAKU);
                    return handeffectTypes;
                }

                //タネの判定
                if (points.Select(x => x == OtofudaPointType.SEED).Count() == 5)
                {
                    handeffectTypes.Add(OtofudaHandEffectType.TANE);
                    return handeffectTypes;
                }
            }

            return handeffectTypes;
        }


        //譜面を叩く部分でないので非同期処理でOK、のはず。
        private bool checkhandValue(OtofudaFlowerType[] _flower, OtofudaPointType[] _point,
            OtofudaFlowerType targetFlower, OtofudaPointType targetPosint)
        {
            //ターゲットが含まれていなかったらfalse
            if (!_flower.Contains(targetFlower))
            {
                return false;
            }
            var flowerIdx = Array.IndexOf(_flower,targetFlower);

            if (_point[flowerIdx] == targetPosint)
            {
                return true;
            }

            return false;
        }
    }

}

