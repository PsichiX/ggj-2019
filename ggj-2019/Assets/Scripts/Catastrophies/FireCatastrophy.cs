using System.Collections.Generic;
using UnityEngine;
using static GaryMoveOut.GameplayManager;

namespace GaryMoveOut.Catastrophies
{
    [CreateAssetMenu(menuName = "ScriptableObjects/Catastrophies/Fire")]
    public class FireCatastrophy : BaseCatastrophy
    {
        public List<GameObject> firePrefabs;

        public override CatastrophyType Type { get { return CatastrophyType.Fire; } }
        public override EvecuationDirection EvacuationDirection
        {
            get
            {
                //return EvecuationDirection.Down;
                var rnd = Random.Range(0, 3);
                return (EvecuationDirection)rnd;
            }
        }

        public override void Initialize()
        {
        }

        public override void Dispose()
        {
        }

        public override void DestroyFloor(Building building, int floorIndex)
        {
            Debug.LogWarning($"Fire on the {floorIndex} dance floor!");

            if (building.Floors.TryGetValue(floorIndex, out Floor floor))
            {
                for(int i = 1; i < floor.segments.Count - 1; i++)
                {
					var fire = Instantiate(firePrefabs[Random.Range(0, firePrefabs.Count)], floor.segments[i].transform);
					Vector3 pos = fire.transform.position;
					pos.z = Random.Range(-0.7f, 0.2f);
					fire.transform.position = pos;
				}
            }
        }
    }
}