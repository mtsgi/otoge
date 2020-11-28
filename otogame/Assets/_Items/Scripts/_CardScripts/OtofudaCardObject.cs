using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace OtoFuda.Card
{
    public class OtofudaCardObject : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;
        public OtofudaCard Otofuda;

        private Sequence drawSequence;
        private Vector3 cacheDefaultScale;
        private Vector3 cacheDrawScale;
        private float scaleRatio = 1.2f;
        private float scaleDuration = 0.25f;

        private void Start()
        {
            cacheDefaultScale = this.transform.localScale;
            cacheDrawScale = cacheDefaultScale * scaleRatio;

            drawSequence = DOTween.Sequence()
                .Append(transform.DOScale(cacheDrawScale, scaleDuration))
                .Append(transform.DOScale(cacheDefaultScale, scaleDuration / 2))
                .Pause()
                .SetAutoKill(false)
                .SetLink(gameObject);
        }


        public void SetCard(OtofudaCard card)
        {
            Otofuda = card;
            SetSprite();
            drawSequence.Restart();
        }

        public void SetSprite()
        {
            _renderer.sprite = Otofuda.cardPicture;
        }


        [ContextMenu("Assign Sprite")]
        private void AssignSprite()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }
    }
}