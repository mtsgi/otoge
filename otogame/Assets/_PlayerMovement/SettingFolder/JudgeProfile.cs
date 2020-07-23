using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class JudgeProfile : ScriptableObject
{
    public float perfectThreshold;
    public float goodThreshold;
    public float badThreshold;
    public float missThreshold;

    public float comboInteractionScale = 1.05f;
    public float comboInteractionTime = 0.05f;
}