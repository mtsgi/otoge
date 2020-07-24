using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComboCounter
{
    private readonly TextMeshProUGUI _comboCountText;
    private readonly TextMeshProUGUI _comboInfoText;
    private int _comboCount;
    private readonly Vector3 _defaultTextScale;
    private readonly float _interactionScale = 1.1f;
    private readonly float _interactionTime = 0.05f;

    private float defaultAlpha;

    public ComboCounter(TextMeshProUGUI countText, TextMeshProUGUI infoText, float iScale, float iTime)
    {
        _comboCountText = countText;
        _comboInfoText = infoText;
        
        _interactionScale = iScale;
        _interactionTime = iTime;

        Debug.Log($"Scale{iScale}/Time{iTime}");
        _defaultTextScale = _comboCountText.rectTransform.localScale;

        defaultAlpha = countText.alpha;
        
        ComboCut();
    }

    public void ComboUp()
    {
        //カウント前のコンボ数が0であれば表示のTweenを行う
        if (_comboCount == 0)
        {
            var sequence = DOTween.Sequence()
                .Append(_comboCountText.DOFade(defaultAlpha, _interactionTime))
                .Append(_comboInfoText.DOFade(defaultAlpha, _interactionTime))
                .Play(); 
        }
        
        _comboCount += 1;
        if (_comboCountText != null)
        {
            _comboCountText.text = $"{_comboCount}";
            var sequence = DOTween.Sequence()
                .Append(_comboCountText.rectTransform.DOScale(_defaultTextScale * _interactionScale, _interactionTime))
                .Append(_comboCountText.rectTransform.DOScale(_defaultTextScale, _interactionTime))
                .Play();
        }
    }

    public void ComboCut()
    {
        _comboCount = 0;
        if (_comboCountText != null)
        {
            _comboCountText.text = $"{_comboCount}";
            var sequence = DOTween.Sequence()
                .Append(_comboCountText.DOFade(0.0f, _interactionTime))
                .Append(_comboInfoText.DOFade(0.0f, _interactionTime))
                .Play(); 
        }
    }
}