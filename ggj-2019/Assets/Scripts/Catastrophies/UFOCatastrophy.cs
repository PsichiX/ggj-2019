using DG.Tweening;
using UnityEngine;
using static GaryMoveOut.GameplayManager;

namespace GaryMoveOut.Catastrophies
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Catastrophies/UFO")]
    public class UFOCatastrophy : BaseCatastrophy
    {
        [SerializeField] private GameObject ufoPrefab;
        [SerializeField] private Vector3 ufoStartPos = new Vector3(20f, 300f, 0f);
        [SerializeField] private Vector3 ufoIdlePos = new Vector3(60f, 100f, 0f);
        [SerializeField] private Vector3 ufoAboveFloorPos = new Vector3(2f, 6f, 0f);
        [SerializeField] private float ufoFlightDuration = 4f;
        [SerializeField] private float ufoTakeOffDelay = 2.5f;

        private GameObject ufo;
        private GameplayManager gameplayManager;
        private Vector3 currentUfoPos;
        private Sequence sequence;
		private AudioSource ufoAudio;

        public override CatastrophyType Type { get { return CatastrophyType.UFO; } }
        public override EvecuationDirection EvacuationDirection { get { return EvecuationDirection.Down; } }

        public override void Initialize()
        {
            ufo = GameObject.Instantiate(ufoPrefab, ufoStartPos, Quaternion.identity);
            ufo.transform.Rotate(new Vector3(0f, 180f, 0f));
            if (ufoAudio == null)
            {
                ufoAudio = ufo.GetComponent<AudioSource>();
            }
            if (gameplayManager == null)
            {
                gameplayManager = GameplayManager.GetGameplayManager();
            }
        }

        public override void Dispose()
        {
            OnFinish();
            GameObject.Destroy(ufo);
        }

        public override void DestroyFloor(Building building, int floorIndex)
        {
			if (ufo == null)
			{
				Initialize();
			}
            Debug.LogWarning($"Nightmare out of SPACE on the {floorIndex} floor!");

            if (building.Floors.TryGetValue(floorIndex, out Floor floor))
            {
                currentUfoPos = floor.gameObject.transform.position + ufoAboveFloorPos;
                ufo.transform.DOMove(currentUfoPos, ufoFlightDuration).SetEase(Ease.InCirc).OnComplete(() =>
                {
                    sequence = DOTween.Sequence();
                    sequence.Append(DOVirtual.DelayedCall(1f, () =>
                    {
						if (ufo != null)
						{
							ufoAudio.Play();
							gameplayManager.AddMultiCameraTarget(ufo);
						}

                    }));
                    sequence.Append(DOVirtual.DelayedCall(1f, () =>
                    {
						while(floor.transform.childCount > 0)
                        {
                            var child = floor.transform.GetChild(0);
                            child.gameObject.transform.SetParent(null);
                            var time = UnityEngine.Random.Range(0.7f, 2.5f);
                            child.gameObject.transform.DOMove(ufo.transform.position, time).SetEase(Ease.InExpo).OnComplete(() =>
                            {
                                GameObject.Destroy(child.gameObject);
                            });
                        }
                        floor.items.Clear();
                        floor.segments.Clear();

                    }));
                    sequence.Append(DOVirtual.DelayedCall(ufoTakeOffDelay, OnFinish));
                });
            }
        }

        private void OnFinish()
        {
            if (sequence != null)
            {
                sequence.Kill();
            }
            gameplayManager.RemoveMultiCameraTarget(ufo);
            currentUfoPos += ufoIdlePos;
			if (ufo != null)
			{
				ufo.transform.DOMove(currentUfoPos, ufoFlightDuration).SetEase(Ease.InOutExpo);
			}
        }
    }
}