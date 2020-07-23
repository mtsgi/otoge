using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboCounter
{
    private readonly Text _comboText;
    private int _comboCount;

    public ComboCounter(Text text)
    {
        _comboText = text;
    }

    public void ComboUp()
    {
        _comboCount += 1;
        if (_comboText != null)
        {
            _comboText.text = $"{_comboCount}";
        }
    }

    public void ComboCut()
    {
        _comboCount = 0;
        if (_comboText != null)
        {
            _comboText.text = $"{_comboCount}";
        }
    }
}