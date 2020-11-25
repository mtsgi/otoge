using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Leap.Unity;
using OtoFuda.Fumen;
using OtoFuda.RythmSystem;
using UnityEngine;

namespace OtoFuda.Fumen
{
    //todo あんまし綺麗じゃないけど、いったんこれで。悩ましい
    [Serializable]
    public class TimingInformationList
    {
        public List<NoteTimingInformation>[] DefaultTimings = new List<NoteTimingInformation>[5];
        public List<NoteTimingInformation>[] MoreEasyTimings = new List<NoteTimingInformation>[5];
        public List<NoteTimingInformation>[] MoreDifficultTimings = new List<NoteTimingInformation>[5];

        public TimingInformationList()
        {
            for (int i = 0; i < 5; i++)
            {
                DefaultTimings[i] = new List<NoteTimingInformation>();
                MoreEasyTimings[i] = new List<NoteTimingInformation>();
                MoreDifficultTimings[i] = new List<NoteTimingInformation>();
            }
        }
    }

    [Serializable]
    public class NoteTimingInformation
    {
        public NoteTimingInformation(NoteObject noteEntity, int noteType, float reachTime)
        {
            _noteEntity = noteEntity;
            _noteType = noteType;
            _reachTime = reachTime;
        }

        public NoteObject _noteEntity;
        public int _noteType;
        public float _reachTime;
    }

    //todo Playerが今どのステートなのか？も管理する。例えば、難易度変更の効果を受けた後、_fumenDataManager.currentTargetTimings を対象のものにするなど

    public class FumenDataManager : MonoBehaviour
    {
        //todo 見にくいのでこれ外に出してほしい
        public enum MusicDataMode
        {
            Debug,
            Game,
            AutoPlay
        }

        public MusicDataMode musicDataMode;
        public MusicDataObject debugMusicData;

        private List<NoteObject> _defaultNotes = new List<NoteObject>();
        private List<NoteObject> _moreEasyNotes = new List<NoteObject>();
        private List<NoteObject> _moreDifficultNotes = new List<NoteObject>();

        private List<NoteObject> _defaultBeatLineNotes = new List<NoteObject>();
        private List<NoteObject> _moreEasyBeatLineNotes = new List<NoteObject>();
        private List<NoteObject> _moreDifficultBeatLineNotes = new List<NoteObject>();

        public Transform notesRootTransform;

        [SerializeField] private FumenFlowManager[] _fumenFlowManager;
        [SerializeField] private PlayerKeyInputManager[] _playerKeyInputManagers;
        public PlayerKeyInputManager[] PlayerKeyInputManagers => _playerKeyInputManagers;

        public MusicData _musicData = new MusicData();
        private PlayerInfo[] _playerInfos = new PlayerInfo[2];

        //実寸/10で定義する
        internal float laneLength = 0.5f;
        public float[] _highSpeed = {8.0f, 8.0f};

        private TimingInformationList _timingInformationList = new TimingInformationList();
        private TimingInformationList _beatLineTimingInformation = new TimingInformationList();

        private List<NoteTimingInformation>[] _currentStateTimingInformation;

        public int playerId;

        private AudioSource _audioSource;


        [SerializeField] private JudgeProfile judgeProfile;
        [SerializeField] private JudgeTextController judgeTextController;
        [SerializeField] private PlayerManager _playerManager;

        private int[,] _noteCounters;

        private new void Awake()
        {
        }

        protected void Start()
        {
            Init();
            FumenStart(musicDataMode);
        }

        protected void Update()
        {
            _fumenFlowManager[0].FumenUpdate();
            _playerKeyInputManagers[0].KeyInputUpdate();
        }

        public void Init()
        {
            Debug.Log("FumenDataManagerAwake");

            //AudioSourceを拾う
            _audioSource = SoundManager.Instance.gameObject.GetComponents<AudioSource>()[0];

            //初期化
            _timingInformationList = new TimingInformationList();
            _defaultNotes = new List<NoteObject>();
            _moreEasyNotes = new List<NoteObject>();
            _moreDifficultNotes = new List<NoteObject>();

            //各難易度の各レーンのノーツ情報を初期化する
            _noteCounters = new int[(int) PlayerFumenState.End, 5];
            for (var i = 0; i < (int) PlayerFumenState.End; i++)
            {
                for (var k = 0; k < 5; k++)
                {
                    _noteCounters[i, k] = 0;
                }
            }


            var fumenSelectSceneData =
                SceneLoadManager.Instance.previewSceneTransitionData as FumenSelectSceneTransitionData;

            Debug.Log($"{SceneLoadManager.Instance.testTypeName}");

            if (musicDataMode == MusicDataMode.Debug)
            {
                SetDebugMusicData();
            }
            else if (musicDataMode == MusicDataMode.Game)
            {
                if (fumenSelectSceneData != null)
                {
                    SetFumenSelectSceneMusicData(fumenSelectSceneData);
                }

                //ハイスピ周りはPlayerManagerに渡したい。
                //というかこれMonoにしたくない
                for (int i = 0; i < _playerInfos.Length; i++)
                {
                    _highSpeed[i] = _playerInfos[i].hiSpeed;
                }
            }
            else if (musicDataMode == MusicDataMode.AutoPlay)
            {
                SetAutoPlayMusicData();
            }

            _musicData.TestCheckParameter();


            //譜面をセットアップする。
            //resultがFalseなら譜面情報がまだ登録されていないのでreturn
            SetUpFumen(out var result);

            /*
            if (!result)
            {
                return;
            }
            */

            //デフォルトのノーツのタイミングリストを現在のステートとする
            _currentStateTimingInformation = _timingInformationList.DefaultTimings;

            //todo keyInputManagerもFumenFlowManagerもSingletonをやめたい。ここのfor文が消えるはず
            //InputManagerを初期化
            for (int i = 0; i < _playerKeyInputManagers.Length; i++)
            {
                _playerKeyInputManagers[i].Init(_audioSource,
                    judgeProfile, judgeTextController,
                    _playerManager,
                    _timingInformationList, _currentStateTimingInformation,
                    _noteCounters);
            }

            for (int i = 0; i < _fumenFlowManager.Length; i++)
            {
                _fumenFlowManager[i].Init(_audioSource,
                    judgeProfile, judgeTextController,
                    _playerManager,
                    _timingInformationList, _currentStateTimingInformation,
                    _noteCounters);

                _fumenFlowManager[i].SetBeatLineTimings(_beatLineTimingInformation);
            }
        }

        private void SetUpFumen(out bool setUpResult)
        {
            var fumenJsonReader = new FumenJsonReader(_musicData, musicDataMode);
            var fumenInfo = fumenJsonReader.Serialize();

            Debug.Log(fumenInfo);
            //fumenInfoが空であればfumenが登録されていないということなので、return
            if (fumenInfo == null)
            {
                setUpResult = false;
                return;
            }

            //各難易度のノーツのオブジェクトを生成する
            var playerDefaultDifficultyNumber = (int) _musicData.levels[playerId];
            _defaultNotes = GenerateSortedNotes(fumenInfo, (GameDifficulty) (playerDefaultDifficultyNumber), 0);
            _moreDifficultNotes =
                GenerateSortedNotes(fumenInfo, (GameDifficulty) (playerDefaultDifficultyNumber + 1), 1);
            _moreEasyNotes = GenerateSortedNotes(fumenInfo, (GameDifficulty) (playerDefaultDifficultyNumber - 1), 1);

            //生成されたノーツのオブジェクトから各レーンごとにタイミングだけを格納したListの配列を作成する
            _timingInformationList.DefaultTimings = GenerateSortedTimings(_defaultNotes);
            _timingInformationList.MoreDifficultTimings = GenerateSortedTimings(_moreDifficultNotes);
            _timingInformationList.MoreEasyTimings = GenerateSortedTimings(_moreEasyNotes);

            //デフォルトノーツの情報からEOFを探す
            var eofNoteObject = _defaultNotes.First(x => x.noteType == 99);

            //譜面の小節線を生成する
            _defaultBeatLineNotes = GenerateBeatLines(fumenInfo, (GameDifficulty) (playerDefaultDifficultyNumber),
                0, eofNoteObject);
            _moreDifficultBeatLineNotes =
                GenerateBeatLines(fumenInfo, (GameDifficulty) (playerDefaultDifficultyNumber + 1),
                    1, eofNoteObject);
            _moreEasyBeatLineNotes = GenerateBeatLines(fumenInfo, (GameDifficulty) (playerDefaultDifficultyNumber - 1),
                1, eofNoteObject);

            //小節線のタイミング情報を格納する
            _beatLineTimingInformation.DefaultTimings = GenerateSortedTimings(_defaultBeatLineNotes);
            _beatLineTimingInformation.MoreDifficultTimings = GenerateSortedTimings(_moreDifficultNotes);
            _beatLineTimingInformation.MoreEasyTimings = GenerateSortedTimings(_moreEasyNotes);
            setUpResult = true;
        }

        private List<NoteObject> GenerateSortedNotes(FumenInfo fumenInfo, GameDifficulty difficulty, float zOffset)
        {
            var fumenNoteGenerator = new FumenNoteGenerator(notesRootTransform, _musicData, laneLength);

            var defaultNoteObjects =
                fumenNoteGenerator.GenerateNotesData(fumenInfo, difficulty, playerId, _highSpeed[playerId], zOffset);
            defaultNoteObjects.Sort((x, y) => x.reachFrame.CompareTo(y.reachFrame));

            return defaultNoteObjects;
        }

        private List<NoteObject> GenerateBeatLines(FumenInfo fumenInfo, GameDifficulty difficulty,
            float zOffset, NoteObject eofNoteObject)
        {
            var fumenNoteGenerator = new FumenNoteGenerator(notesRootTransform, _musicData, laneLength);
            var result = fumenNoteGenerator.BeatLineGenerate(fumenInfo, difficulty, playerId,
                _highSpeed[playerId], zOffset, eofNoteObject);
            result.Sort((x, y) => x.reachFrame.CompareTo(y.reachFrame));
            return result;
        }


        private List<NoteTimingInformation>[] GenerateSortedTimings(ReadonlyList<NoteObject> noteObjects)
        {
            var timings = new List<NoteTimingInformation>[5];
            for (int i = 0; i < timings.Length; i++)
            {
                timings[i] = new List<NoteTimingInformation>();
            }


            for (int i = 0; i < noteObjects.Count; i++)
            {
                var noteObject = noteObjects[i];
                var lane = noteObject.lane;

                if (lane <= 0) continue;
                var type = noteObject.noteType;
                var reach = noteObject.reachFrame;
                //todo 今はいいけどこれ微妙では？普通にNoteObject単体の参照を別の構造体也なんなりに渡す、でいいはず
                //ノーツの実体とタイプ情報、到達時間を持った配列を作り出す
                timings[lane - 1].Add(new NoteTimingInformation(noteObject, type, reach));
            }

            for (int i = 0; i < timings.Length; i++)
            {
                timings[i].Sort((x, y) => x._reachTime.CompareTo(y._reachTime));
            }

            return timings;
        }


        public virtual void SetDebugMusicData()
        {
            _musicData = debugMusicData.GetMusicData();
            _playerInfos = new PlayerInfo[_fumenFlowManager.Length];
            for (int i = 0; i < _playerInfos.Length; i++)
            {
                _playerInfos[i] = new PlayerInfo();
            }

            Debug.Log("Test Mode");
        }

        public virtual void SetFumenSelectSceneMusicData(FumenSelectSceneTransitionData fumenSelectSceneData)
        {
            _musicData = fumenSelectSceneData.musicData;
            _playerInfos = fumenSelectSceneData.playerInfos;
        }

        public virtual void SetAutoPlayMusicData()
        {
            _playerInfos = new PlayerInfo[_fumenFlowManager.Length];
            for (int i = 0; i < _playerInfos.Length; i++)
            {
                _playerInfos[i] = new PlayerInfo();
            }

            Debug.Log("Test Mode");
        }

        public void FumenStart(MusicDataMode mode)
        {
            //musicIdが空っぽだったら抜ける
            if (string.IsNullOrEmpty(_musicData.musicId))
            {
                Debug.LogError("楽曲を開始しようとしましたが、Music Id が空っぽでした");
                return;
            }

            if (mode == FumenDataManager.MusicDataMode.Game || mode == FumenDataManager.MusicDataMode.Debug)
            {
                var path = "Musics/" + _musicData.musicId;
                Debug.Log(path);
                var audioClip = Resources.Load(path, typeof(AudioClip)) as AudioClip;
                SoundManager.Instance.gameObject.GetComponents<AudioSource>()[0].clip = audioClip;
                if (audioClip != null) SoundManager.Instance._soundListSettings[0].soundName = audioClip.name;
                SoundManager.Instance.InitDictionary();
            }
            else if (mode == MusicDataMode.AutoPlay)
            {
                SoundManager.Instance.PlayAudioFromExternal(_musicData.musicId);
            }

            StartCoroutine(FumenStartWait());
        }

        public void Check()
        {
//            Debug.Log(mainNotes[0].Count);
        }

        private IEnumerator FumenStartWait()
        {
            var waitSec = (60 / _musicData.bpm);
            var waitForBlankRhythm = new WaitForSeconds(waitSec);
            yield return new WaitForSeconds(2);
            SoundManager.Instance.PlaySound(1).Play();
            yield return waitForBlankRhythm;
            SoundManager.Instance.PlaySound(1).Play();
            yield return waitForBlankRhythm;
            SoundManager.Instance.PlaySound(1).Play();
            yield return waitForBlankRhythm;
            SoundManager.Instance.PlaySound(1).Play();
            yield return waitForBlankRhythm;

            /*for (int i = 0; i < _fumenFlowManager.Length; i++)
            {
                _fumenFlowManager[i].StartFumenFlow(mainNotes[i], moreDifficultNotes[i]);
            }*/

            for (int i = 0; i < _fumenFlowManager.Length; i++)
            {
                _fumenFlowManager[i].StartFumenFlow(_defaultNotes, _moreDifficultNotes,
                    _defaultBeatLineNotes, _moreDifficultBeatLineNotes, _moreEasyBeatLineNotes);
            }

            MusicManager.Instance.Startmusic(0);
        }
    }
}