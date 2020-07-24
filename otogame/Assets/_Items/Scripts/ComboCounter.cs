using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComboCounter
{
    private readonly TextMeshProUGUI _comboText;
    private int _comboCount;
    private readonly Vector3 _defaultTextScale;
    private readonly float _interactionScale = 1.1f;
    private readonly float _interactionTime = 0.05f;


    public ComboCounter(TextMeshProUGUI text, float iScale, float iTime)
    {
        _comboText = text;
        _interactionScale = iScale;
        _interactionTime = iTime;
        
        Debug.Log($"Scale{iScale}/Time{iTime}");
        _defaultTextScale = _comboText.rectTransform.localScale;
    }

    public void ComboUp()
    {
        _comboCount += 1;
        if (_comboText != null)
        {
            _comboText.text = $"{_comboCount}";
            var sequence = DOTween.Sequence()
                .Append(_comboText.rectTransform.DOScale(_defaultTextScale * _interactionScale, _interactionTime))
                .Append(_comboText.rectTransform.DOScale(_defaultTextScale, _interactionTime))
                .Play();
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