using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboCounter
{
    private Text comboText;
    private int comboCount;

    public ComboCounter(Text text)
    {
        comboText = text;
    }

    public void ComboUp()
    {
        comboCount += 1;
        if (comboText != null)
        {
            comboText.text = $"{comboCount}";
        }
    }

    public void ComboCut()
    {
        comboCount = 0;
        if (comboText != null)
        {
            comboText.text = $"{comboCount}";
        }
    }
}