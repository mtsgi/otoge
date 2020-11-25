using System.Collections.Generic;
using OtoFuda.Fumen;
using UnityEngine;

public abstract class FumenJudgeBehaviour : MonoBehaviour
{
    //todo FumenJudgeBehaviourを管理するもう一つ上の層のクラスが必要。一旦はInspectorからそれぞれ登録する。
    private JudgeProfile _judgeProfile;
    public JudgeProfile JudgeProfile => _judgeProfile;

    private JudgeTextController _judgeTextController;
    protected JudgeTextController JudgeController => _judgeTextController;

    protected AudioSource _audioSource;

    private PlayerManager _playerManager;
    public PlayerManager PlayerManager => _playerManager;

    //

    protected int[,] _cacheNoteCounters;

    protected List<NoteTimingInformation>[] _cacheDefaultTimings;
    protected List<NoteTimingInformation>[] _cacheMoreEasyTimings;
    protected List<NoteTimingInformation>[] _cacheMoreDifficultTimings;

    protected List<NoteTimingInformation>[] _cacheCurrentTimings;

    protected bool isInitialized;

    public virtual void Init(AudioSource audioSource,
        JudgeProfile profile, JudgeTextController judgeTextController,
        PlayerManager playerManager,
        TimingInformationList timingInformationList,
        List<NoteTimingInformation>[] currentStateTimingInformation,
        int[,] noteCounters)
    {
        _audioSource = audioSource;

        _judgeProfile = profile;
        _judgeTextController = judgeTextController;

        _playerManager = playerManager;

        _cacheDefaultTimings = timingInformationList.DefaultTimings;
        _cacheMoreDifficultTimings = timingInformationList.MoreDifficultTimings;
        _cacheMoreEasyTimings = timingInformationList.MoreEasyTimings;

        _cacheCurrentTimings = currentStateTimingInformation;

        _cacheNoteCounters = noteCounters;

        isInitialized = true;
    }
}