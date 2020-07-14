using System;
using UnityEngine;

[Serializable]
public class FixedFpsSetting
{
    public bool useFixFps;
    public int fixFps = 60;
}

[Serializable]
public class LoggingSetting
{
    public bool useLogging = true;
    public float loggingInterval = 0.5f;
}

public class FpsTester : MonoBehaviour
{
    public float Fps { get; private set; }

    public FixedFpsSetting fixedFpsSetting = new FixedFpsSetting();
    public LoggingSetting loggingSetting = new LoggingSetting();

    float _mPrevTime;
    int _mFrameCount;

    private void Awake()
    {
        if (fixedFpsSetting.useFixFps)
        {
            Application.targetFrameRate = fixedFpsSetting.fixFps;
        }
    }

    private void Start()
    {
        Fps = 60.0f;
        _mPrevTime = 0.0f;
        _mFrameCount = 0;
    }

    void Update()
    {
        ++_mFrameCount;
        float diff_time = Time.realtimeSinceStartup - _mPrevTime;

        if (loggingSetting.loggingInterval > diff_time)
        {
            return;
        }

        Fps = ((float) _mFrameCount / diff_time);
        if (loggingSetting.useLogging)
        {
            Debug.Log(Fps);
        }

        _mFrameCount = 0;
        _mPrevTime = Time.realtimeSinceStartup;
    }
}