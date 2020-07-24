using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class KeyBeamController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] beamRenderers;
    [SerializeField] private float fadeTime = 0.05f;
    [SerializeField] private Color defaultColor = Color.white ;

    private List<KeyBeam> keyBeams;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        keyBeams = new List<KeyBeam>();

        for (int i = 0; i < beamRenderers.Length; i++)
        {
            beamRenderers[i].color = defaultColor;
            var keyBeam = new KeyBeam(beamRenderers[i], fadeTime, defaultColor);
            keyBeams.Add(keyBeam);
        }
    }

    public void BeamOnAt(int index)
    {
        if (index <= keyBeams.Count)
        {
            keyBeams[index].BeamOn();
        }
    }

    public void BeamOffAt(int index)
    {
        if (index <= keyBeams.Count)
        {
            keyBeams[index].BeamOff();
        }
    }

    public void BeamOnAll()
    {
        for (int i = 0; i < keyBeams.Count; i++)
        {
            keyBeams[i].BeamOn();
        }
    }

    public void BeamOffAll()
    {
        for (int i = 0; i < keyBeams.Count; i++)
        {
            keyBeams[i].BeamOff();
        }
    }

    [ContextMenu("AutoSet")]
    public void AutoSet()
    {
        var sr = GetComponentsInChildren<SpriteRenderer>();
        beamRenderers = new SpriteRenderer[sr.Length];
        for (int i = 0; i < beamRenderers.Length; i++)
        {
            beamRenderers[i] = sr[i];
        }
    }
}

public class KeyBeam
{
    private SpriteRenderer _keyBeamSpriteRenderer;
    private float _fadeTime = 0.05f;
    private Color _defaultColor;
    private Color _offColor;

    public KeyBeam(SpriteRenderer keyBeamSpriteRenderer , float fade, Color defColor)
    {
        _keyBeamSpriteRenderer = keyBeamSpriteRenderer;
        _fadeTime = fade;
        _defaultColor = defColor;

        _offColor = _defaultColor;
        _offColor.a = 0.0f;
    }


    public void BeamOn()
    {
        var seq = DOTween.Sequence()
            .Append(_keyBeamSpriteRenderer.DOColor(_defaultColor, _fadeTime))
            .Play();
    }

    public void BeamOff()
    {
        var seq = DOTween.Sequence()
            .Append(_keyBeamSpriteRenderer.DOColor(_offColor, _fadeTime))
            .Play();
    }
}