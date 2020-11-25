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
}