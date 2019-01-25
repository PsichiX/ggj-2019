﻿using System.Collections.Generic;
using UnityEngine;

namespace GaryMoveOut
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private InputHandler m_inputHandler;
        [SerializeField]
        private float m_speed;
        [SerializeField]
        private Vector3 m_pickableOffset;

        private Rigidbody2D m_rigidBody;
        private HashSet<GameObject> m_interactibles = new HashSet<GameObject>();
        private bool m_lastAction = false;
        private bool m_lastUp = false;
        private Pickable m_pickedUp;

        private void Start()
        {
            m_rigidBody = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            if (m_inputHandler != null)
            {
                var dt = Time.fixedDeltaTime;
                var action = m_inputHandler.Action;
                var up = m_inputHandler.Up;
                if (action != m_lastAction)
                {
                    m_lastAction = action;
                    if (action)
                    {
                        // TODO: perform aim.
                    }
                    else
                    {

                        // TODO: perform throw.
                    }
                }
                if (up != m_lastUp)
                {
                    m_lastUp = up;
                    if (up)
                    {
                        PickUp();
                    }
                }
                if (m_inputHandler.Left)
                {
                    m_rigidBody.MovePosition(m_rigidBody.position + Vector2.left * m_speed * dt);
                }
                if (m_inputHandler.Right)
                {
                    m_rigidBody.MovePosition(m_rigidBody.position + Vector2.right * m_speed * dt);
                }
            }

            if (m_pickedUp != null && m_pickedUp.IsPickedUp)
            {
                m_pickedUp.transform.position = transform.position + m_pickableOffset;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Interactible")
            {
                m_interactibles.Add(other.transform.parent.gameObject);
                Debug.Log("TRIGGER ENTER: " + other.transform.parent.gameObject.name);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag == "Interactible")
            {
                m_interactibles.Remove(other.transform.parent.gameObject);
                Debug.Log("TRIGGER EXIT: " + other.transform.parent.gameObject.name);
            }
        }

        private void PickUp()
        {
            var pickable = GetInteractible<Pickable>();
            if (pickable != null)
            {
                m_pickedUp = pickable;
                m_pickedUp.PickUp();
            }
        }

        private T GetInteractible<T>() where T : MonoBehaviour
        {
            foreach (var go in m_interactibles)
            {
                var c = go.GetComponent<T>();
                if (c != null)
                {
                    return c;
                }
            }
            return null;
        }
    }
}
