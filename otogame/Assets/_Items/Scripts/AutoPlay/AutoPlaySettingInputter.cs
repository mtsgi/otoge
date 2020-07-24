using System.Collections;
using System.Collections.Generic;
using System.IO;
using SFB;
using UnityEngine;
using UnityEngine.UI;

public class AutoPlaySettingInputter : MonoBehaviour
{
    [Header("譜面Json選択")] [SerializeField] private Text fumenJsonNameText;
    [SerializeField] private Button fumenJsonSelectButton;
    private string fumenJsonName = "";

    [Header("音源ファイル選択")] [SerializeField] private Text musicNameText;
    [SerializeField] private Button musicSelectButton;
    private string musicName = "";

    [Header("Bpm設定")] [SerializeField] private InputField bpmInput;
    private float bpm;

    [Header("拍子設定")] [SerializeField] private InputField beatInput;
    private float beat;

    [Header("オフセット設定")] [SerializeField] private InputField offsetInput;
    private int offset;

    [Header("難易度設定")] [SerializeField] private Dropdown difficultyDropDown;
    public GameDifficulty[] levels = {GameDifficulty.Hard, GameDifficulty.Normal};

    private void Start()
    {
        var jsonFilter = new ExtensionFilter[1];
        jsonFilter[0] = new ExtensionFilter(
            "JsonFiles",
            "json"
        );

        var musicFilter = new ExtensionFilter[1];
        musicFilter[0] = new ExtensionFilter(
            "MusicFiles",
            "wav", "mp3", "ogg"
        );

        fumenJsonSelectButton.onClick.AddListener(() =>
            OpenSelectJsonDialog(jsonFilter, fumenJsonNameText, ref fumenJsonName));
        musicSelectButton.onClick.AddListener(() =>
            OpenSelectJsonDialog(musicFilter, musicNameText, ref musicName));

        bpmInput.onValueChanged.AddListener(_ => SetInputTextToFloat(bpmInput, ref bpm));
        beatInput.onValueChanged.AddListener(_ => SetInputTextToFloat(beatInput, ref beat));
        offsetInput.onValueChanged.AddListener(_ => SetInputTextToInt(offsetInput, ref offset));

        InitializeDropDown();
        difficultyDropDown.onValueChanged.AddListener(_ => SetLevel());

        SetInputTextToFloat(bpmInput, ref bpm);
        SetInputTextToFloat(beatInput, ref beat);
        SetInputTextToInt(offsetInput, ref offset);
    }

    private void InitializeDropDown()
    {
        difficultyDropDown.options = new List<Dropdown.OptionData>();
        for (int i = 0; i < (int) GameDifficulty.End; i++)
        {
            difficultyDropDown.options.Add(new Dropdown.OptionData()
            {
                text = ((GameDifficulty) i).ToString()
            });
        }

        difficultyDropDown.value = (int) GameDifficulty.Hard;
    }

    public void OpenSelectJsonDialog(ExtensionFilter[] extensions, Text text, ref string fileName)
    {
        var openFolder = "";
        if (!string.IsNullOrEmpty(fileName) && Directory.Exists(Path.GetDirectoryName(fileName)))
        {
            openFolder = Path.GetDirectoryName(fileName);
        }

        var paths = StandaloneFileBrowser.OpenFilePanel("Select Json",
            $"{openFolder}", extensions, false);

        if (paths.Length != 0)
        {
            fileName = paths[0];
            text.text = Path.GetFileName(paths[0]);
            Debug.Log($"Load File {fileName}");
            return;
        }

        fileName = "";
    }

    public void SetInputTextToFloat(InputField field, ref float targetFloat)
    {
        if (float.TryParse(field.text, out var value))
        {
            targetFloat = value;
        }
        else
        {
            return;
        }
    }

    public void SetInputTextToInt(InputField field, ref int targetFloat)
    {
        if (int.TryParse(field.text, out var value))
        {
            targetFloat = value;
        }
        else
        {
            return;
        }
    }

    public void SetLevel()
    {
        for (int i = 0; i < levels.Length; i++)
        {
            levels[i] = (GameDifficulty) difficultyDropDown.value;
        }
    }

    private string GetPathWithoutExtension(string path)
    {
        var extension = Path.GetExtension(path);
        if (string.IsNullOrEmpty(extension))
        {
            return path;
        }

        return path.Replace(extension, string.Empty);
    }

    public AutoPlaySetting GetAutoPlaySetting()
    {
        var autoPlaySetting = new AutoPlaySetting();

        autoPlaySetting.jsonFilePath = fumenJsonName;

        autoPlaySetting.bpm = bpm;
        autoPlaySetting.beat = beat;
        autoPlaySetting.offset = offset;

        for (int i = 0; i < autoPlaySetting.levels.Length; i++)
        {
            autoPlaySetting.levels[i] = levels[i];
            Debug.Log("Get");
        }

        autoPlaySetting.musicFilePath = musicName;

        return autoPlaySetting;
    }
}