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
        [SerializeField] private Vector3 ufoIdlePos = new Vector3(20f, 100f, 0f);
        [SerializeField] private Vector3 ufoAboveFloorPos = new Vector3(0f, 10f, 0f);

        private GameObject ufo;
        private GameplayManager gameplayManager;
        private Vector3 currentUfoPos;


        public override CatastrophyType Type { get { return CatastrophyType.UFO; } }
        public override EvecuationDirection EvacuationDirection { get { return EvecuationDirection.Down; } }


        public override void Initialize()
        {
            ufo = GameObject.Instantiate(ufoPrefab, ufoStartPos, Quaternion.identity);
            gameplayManager = GameplayManager.GetGameplayManager();
        }

        public override void Dispose()
        {
            GameObject.Destroy(ufo);
        }

        public override void DestroyFloor(Building building, int floorIndex)
        {
            Debug.LogWarning($"Nightmare out of SPACE on the {floorIndex} floor!");

            if (building.Floors.TryGetValue(floorIndex, out Floor floor))
            {
                currentUfoPos = floor.gameObject.transform.position + ufoAboveFloorPos;
                ufo.transform.DOMove(currentUfoPos, 2f).SetEase(Ease.InCirc).OnComplete(() => 
                {
                    var sequence = DOTween.Sequence();
                    sequence.Append(DOVirtual.DelayedCall(1f, () =>
                    {
                        // to do: play ufo beam
                        gameplayManager.AddMultiCameraTarget(ufo);
                    }));
                    sequence.Append(DOVirtual.DelayedCall(1f, () =>
                    {
                        for(int i = 0; i < floor.items.Count; i++)
                        {
                            floor.items[i].gameObject.transform.SetParent(null);
                            var time = UnityEngine.Random.Range(1.5f, 2.5f);
                            floor.items[i].gameObject.transform.DOMove(ufo.transform.position, time).SetEase(Ease.InOutExpo).OnComplete(() => 
                            {
                                GameObject.Destroy(floor.items[i].gameObject);
                            });
                        }
                        floor.items.Clear();

                        for (int i = 0; i < floor.segments.Count; i++)
                        {
                            floor.segments[i].gameObject.transform.SetParent(null);
                            var time = UnityEngine.Random.Range(1.5f, 2.5f);
                            floor.segments[i].gameObject.transform.DOMove(ufo.transform.position, time).SetEase(Ease.InOutExpo).OnComplete(() =>
                            {
                                GameObject.Destroy(floor.segments[i].gameObject);
                            });
                        }
                        floor.segments.Clear();

                    }));
                    sequence.Append(DOVirtual.DelayedCall(3f, () =>
                    {
                        gameplayManager.RemoveMultiCameraTarget(ufo);
                        currentUfoPos += ufoIdlePos;
                        ufo.transform.DOMove(currentUfoPos, 2f).SetEase(Ease.InOutExpo);
                    }));
                });
            }
        }
    }
}