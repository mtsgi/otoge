using System.Collections;
using System.IO;
using JetBrains.Annotations;
using OtoFuda.Fumen;
using UnityEngine;

//FumenJsonを読み込むだけのクラス。
//それ以上のことはしちゃだめ
public class FumenJsonReader
{
    private readonly string _jsonPath = "";


    //譜面の読み込み設定を突っ込む
    public FumenJsonReader([NotNull] MusicData musicData, FumenDataManager.MusicDataMode mode)
    {
        var fileName = musicData.jsonFilePath;

        if (mode == FumenDataManager.MusicDataMode.Game || mode == FumenDataManager.MusicDataMode.Debug)
        {
            _jsonPath = "FumenJsons/" + fileName;
        }
        //AutoPlayModeのときはfileNameがjsonのパスそのものになる
        else if (mode == FumenDataManager.MusicDataMode.AutoPlay)
        {
            _jsonPath = fileName;
        }
    }

    //FumenInfoを素で渡してあげる。
    //受け取った側が難易度を指定して受け取る
    public FumenInfo Serialize()
    {
        if (string.IsNullOrEmpty(_jsonPath))
        {
            return null;
        }

        //pathからjsonを丸ッとよみこむ
        var jsonText = "";
        using (var fs = new FileStream(_jsonPath, FileMode.Open))
        {
            using (var sr = new StreamReader(fs))
            {
                jsonText = sr.ReadToEnd();
            }
        }

        if (string.IsNullOrEmpty(jsonText))
        {
            Debug.LogError("Jsonの読み込みに失敗しました");
            return null;
        }

        var fumen = Utf8Json.JsonSerializer.Deserialize<FumenInfo>(jsonText);
        return fumen;
    }
}