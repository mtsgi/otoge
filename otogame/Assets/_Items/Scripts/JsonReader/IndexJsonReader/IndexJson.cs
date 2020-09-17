using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace OtoFuda.Fumen.index
{
    [Serializable]
    public class IndexInfo
    {
        public List<IndexContent> contents = null;
    }


    [Serializable]
    public class IndexContent
    {
        public string song_id = string.Empty; // 楽曲ID
        public string name = string.Empty;
        public string artist = String.Empty;
        public float bpm = 0.0f;
        public string dispbpm = string.Empty; // 表記BPM
        private int[] _colorArray;

        public int[] ColorArray
        {
            get
            {
                if (_colorArray == null)
                {
                    _colorArray = new int[0];
                }

                if (_colorArray.Length != 3)
                {
                    var regex = new Regex(@"[0-9]+");
                    var matches = regex.Matches(color);
                    var array = new int[matches.Count];
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (int.TryParse(matches[i].Value, out var value))
                        {
                            array[i] = value;
                        }
                        else
                        {
                            Debug.Log($"color parse error:{matches[i].Value}");
                            return new int[3];
                        }
                    }

                    return array;
                }
                else
                {
                    return _colorArray;
                }
            }
        }

        public string color = "";

        public MusicJacket jacket = null;
        public int offset = 1000;
        public int raku = -1;
        public int easy = 1;
        public int normal = 3;
        public int hard = 7;
        public int extra = -1;
        public string author = string.Empty;
        public string comment = string.Empty;
        public float demo = 0.0f; // 楽曲デモ開始時間
        public bool coming = false;
    }

    [Serializable]
    public class MusicJacket
    {
        public string url = "";
        public Texture texture;
    }
}