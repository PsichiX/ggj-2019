﻿using System;
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
        private GameplayEvents gameplayEvents;
        private Vector3 currentUfoPos;
        private Sequence sequence;
		private AudioSource ufoAudio;

        public override CatastrophyType Type { get { return CatastrophyType.UFO; } }
        public override EvecuationDirection EvacuationDirection { get { return EvecuationDirection.Down; } }

        public override void Initialize()
        {
            gameplayEvents = GameplayEvents.GetGameplayEvents();
            gameplayEvents.AttachToEvent(GamePhases.GameplayPhase.TruckStart, OnTruckStart);
            gameplayEvents.AttachToEvent(GamePhases.GameplayPhase.Summary, JustDispose);
            gameplayEvents.AttachToEvent(GamePhases.GameplayPhase.GameOver, JustDispose);

            ufo = GameObject.Instantiate(ufoPrefab, ufoStartPos, Quaternion.identity);
            ufo.transform.Rotate(new Vector3(0f, 180f, 0f));
			ufoAudio = ufo.GetComponent<AudioSource>();
			gameplayManager = GameplayManager.GetGameplayManager();
        }

		private void JustDispose(object obj)
		{
			Dispose();
		}

        private void OnTruckStart(object obj)
        {
            OnFinish();
            DOVirtual.DelayedCall(2f, () =>
            {
                Dispose();
            });
        }

        public override void Dispose()
        {
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
						ufoAudio.Play();

						gameplayManager.AddMultiCameraTarget(ufo);
                    }));
                    sequence.Append(DOVirtual.DelayedCall(1f, () =>
                    {
						

						for (int i = 0; i < floor.items.Count; i++)
                        {
                            var item = floor.items[i];
							if (item == null)
							{
								return;
							}
                            item.gameObject.transform.SetParent(null);
                            var time = UnityEngine.Random.Range(1.5f, 2.5f);
                            item.gameObject.transform.DOMove(ufo.transform.position, time).SetEase(Ease.InOutExpo).OnComplete(() =>
                            {
                                GameObject.Destroy(item.gameObject);
                            });
                        }
                        floor.items.Clear();

                        for (int i = 0; i < floor.segments.Count; i++)
                        {
                            var segment = floor.segments[i];
                            segment.gameObject.transform.SetParent(null);
                            var time = UnityEngine.Random.Range(1.5f, 2.5f);
                            segment.gameObject.transform.DOMove(ufo.transform.position, time).SetEase(Ease.InOutExpo).OnComplete(() =>
                            {
                                GameObject.Destroy(segment.gameObject);
                            });
                        }
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