using DG.Tweening;
using UnityEngine;
using static GaryMoveOut.GameplayManager;

namespace GaryMoveOut.Catastrophies
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Catastrophies/Earth Quake")]
    public class EarthQuakeCatastrophy : BaseCatastrophy
    {
        [SerializeField] private GameObject dustStorm;
        [SerializeField] private float dustStormDuration = 5f;
        [SerializeField] private Vector3 dustStormOffset = new Vector3(0f, 0f, -6f);
        [SerializeField] private float floorFallTime = 0.4f;
        [SerializeField] private AudioClip[] sounds;

        public override CatastrophyType Type { get { return CatastrophyType.EarthQuake; } }
        public override EvecuationDirection EvacuationDirection { get { return EvecuationDirection.Up; } }

        private BuildingSegmentsDatabase buildingsDatabase;
		private AudioSource audioSource;

        public override void Initialize()
        {
            if (audioSource == null)
            {
                audioSource = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioSource>();
            }
            if (buildingsDatabase == null)
            {
                buildingsDatabase = Resources.Load<BuildingSegmentsDatabase>("Databases/BuildingSegmentsDatabase");
            }
        }

        public override void Dispose()
        {
        }

        public override void DestroyFloor(Building building, int floorIndex)
        {
            Debug.LogWarning($"Eaaaarthhhh Quuuuaaaakeeeee on the {floorIndex} floor!");

			var players = GameObject.FindGameObjectsWithTag("Player");
			foreach (var p in players)
			{
                var playerController = p.GetComponent<PlayerController>();
                if (playerController.m_inputBlocked == false && playerController.m_isJumping == false)
				{
					p.transform.parent = building.root;
				}
			}

            var endPos = building.root.transform.position - new Vector3(0f, building.SegmentSize.Height, 0f);
            building.root.transform.DOMove(endPos, floorFallTime).SetEase(Ease.OutBounce).OnComplete(() => DeparentPlayers(players));

            var buildingWidth = building.SegmentSize.Width * building.Config.floorSegmentsCount;
            var position = building.root.transform.position;
            position = new Vector3(position.x + buildingWidth * 0.5f, 0f, 0f) + dustStormOffset;
            var dust = Instantiate(dustStorm, position, Quaternion.identity);
            Destroy(dust.gameObject, dustStormDuration);

            audioSource.PlayOneShot(sounds[Random.Range(0, sounds.Length)]);
            DOVirtual.DelayedCall(floorFallTime, () =>
            {
				building.DestroyFloor(floorIndex);
            });
        }

		private void DeparentPlayers(GameObject[] players)
		{
			foreach (var p in players)
			{
				p.transform.SetParent(null);
			}
		}
    }
}