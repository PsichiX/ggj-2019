using System;
using UnityEngine;

namespace GaryMoveOut.Items
{
    public class ItemCatcher : MonoBehaviour
    {
        private GameplayEvents gameplayEvents;
        private BoxCollider2D boxCollider2D;
        private Floor floor;
        private GamePhases.GameplayPhase gamePhase;

        private void Start()
        {
            gameplayEvents = GameplayEvents.GetGameplayEvents();
            gameplayEvents.GameplayPhaseChanged += UpdateGamePhase;
        }
        private void OnDestroy()
        {
            gameplayEvents.GameplayPhaseChanged -= UpdateGamePhase;
        }

        private void UpdateGamePhase(GamePhases.GameplayPhase newGamePhase)
        {
            if (newGamePhase == GamePhases.GameplayPhase.Evacuation || newGamePhase == GamePhases.GameplayPhase.DeEvacuation)
            {
                gamePhase = newGamePhase;
            }
        }

        public void Setup(Vector2 center, Vector3 size, Floor floor)
        {
            boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
            boxCollider2D.size = size;
            boxCollider2D.isTrigger = true;
            gameObject.transform.position = center;
            this.floor = floor;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (gamePhase == GamePhases.GameplayPhase.DeEvacuation)
            {
                var item = other.GetComponentInParent<Item>();
                if (item != null && floor != null)
                {
                    Debug.Log("Added " + item.name + "to floor " + floor.Type);
                    if (floor.AddItem(item))
                    {
                        if (gameplayEvents == null)
                        {
                            gameplayEvents = GameplayEvents.GetGameplayEvents();
                        }
                        gameplayEvents.CallItemAddedToFloor(item);
                    }
                }
            }
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (gamePhase == GamePhases.GameplayPhase.Evacuation)
            {
                var item = other.GetComponentInParent<Item>();
                if (item != null && floor != null)
                {
                    Debug.Log("Removed " + item.name + "from floor " + floor.Type);
                    floor.RemoveItem(item);
                }
            }
        }

    }
}