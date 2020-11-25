using TMPro;
using UnityEngine;

namespace OtoFuda.Fumen
{
    //判定をenumで管理
    public enum Judge
    {
        Perfect = 0,
        Good = 1,
        Bad = 2,
        Miss = 3,
        None = 4
    }

    public class JudgeTextController : MonoBehaviour
    {
        //コンボカウンター用
        private ComboCounter _comboCounter;
        [SerializeField] private TextMeshProUGUI comboCountText;
        [SerializeField] private TextMeshProUGUI comboInfoText;

        //判定を表示する用のテキスト
        public Animator[] judgeTextAnimators;

        [SerializeField] float comboInteractionScale = 1.05f;
        [SerializeField] float comboInteractionTime = 0.05f;


        private void Start()
        {
            if (comboCountText != null)
            {
                _comboCounter = new ComboCounter(comboCountText, comboInfoText,
                    comboInteractionScale, comboInteractionTime);
            }
        }

        public void ShowJudgeAnimation(Judge judge)
        {
            judgeTextAnimators[(int) judge].Play("Judge", 0, 0.0f);
        }

        public void ComboUp()
        {
            _comboCounter?.ComboUp();
        }

        public void ComboUp(Judge judge)
        {
            _comboCounter?.ComboUp();
            if (judge != Judge.None)
            {
                ShowJudgeAnimation(judge);
            }
        }

        public void ComboCut()
        {
            _comboCounter?.ComboCut();
        }

        public void ComboCut(Judge judge)
        {
            _comboCounter?.ComboCut();
            if (judge != Judge.None)
            {
                ShowJudgeAnimation(judge);
            }
        }
    }
}