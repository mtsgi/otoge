using UnityEngine;

public class FpsTester : MonoBehaviour
{
    public float FPS { get; private set; }

    static readonly float INTERVAL = 0.5f;
    float m_prev_time;
    int m_frame_count;

    void Start()
    {
        FPS = 60.0f;
        m_prev_time = 0.0f;
        m_frame_count = 0;
    }

    void Update()
    {
        ++m_frame_count;
        float diff_time = Time.realtimeSinceStartup - m_prev_time;
        if (INTERVAL > diff_time)
        {
            return;
        }

        FPS = ((float) m_frame_count / diff_time);
        Debug.Log(FPS);
        m_frame_count = 0;
        m_prev_time = Time.realtimeSinceStartup;
    }
}