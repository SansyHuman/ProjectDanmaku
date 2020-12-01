using System.Collections;
using System.Collections.Generic;

using SansyHuman.Management;
using SansyHuman.UDE.Management;
using SansyHuman.UDE.Object;

using UnityEngine;

namespace SansyHuman.Item
{
    [DisallowMultipleComponent]
    public abstract class ItemBase : MonoBehaviour
    {
        public enum FallDirection
        {
            X, Y
        }

        [SerializeField]
        private FallDirection fallDirection = FallDirection.X;

        [SerializeField]
        private float acceleration = 1.0f;

        [SerializeField]
        [Tooltip("The maximum falling speed.")]
        private float maxSpeed = 1.0f;

        [SerializeField]
        [Tooltip("The drag speed to the player.")]
        private float dragSpeed = 5.0f;

        [SerializeField]
        [Tooltip("The threashold from the boarder of the main camera to destroy the item. The unit is the viewport unit.")]
        private float itemDestructionThreashold = 0.05f;

        private float velocity = 0.0f;
        private bool dragToPlayer = false;
        private Transform player;

        private Transform tr;
        private Camera mainCamera;

        /// <summary>
        /// Gets whether the item uses object pool. The default setting is false.
        /// If the item uses the object pool, override this property.
        /// </summary>
        public virtual bool UsesObjectPool => false;

        private void Awake()
        {
            tr = transform;
            mainCamera = Camera.main;
        }

        private void FixedUpdate()
        {
            float deltaTime = Time.deltaTime * UDETime.Instance.EnemyTimeScale;

            Vector3 pos = tr.position;
            Vector3 camPos = mainCamera.WorldToViewportPoint(pos);

            if (dragToPlayer)
            {
                Vector3 velocity = player.position - pos;
                velocity *= (dragSpeed / velocity.magnitude);
                pos += velocity * deltaTime;
                tr.position = pos;
                return;
            }

            velocity -= acceleration * deltaTime;
            if (velocity < -maxSpeed)
                velocity = -maxSpeed;


            switch (fallDirection)
            {
                case FallDirection.X:
                    if (camPos.x <= -itemDestructionThreashold)
                    {
                        RemoveItem();
                        return;
                    }
                    pos.x += velocity * deltaTime;
                    break;
                case FallDirection.Y:
                    if (camPos.y <= -itemDestructionThreashold)
                    {
                        RemoveItem();
                        return;
                    }
                    pos.y += velocity * deltaTime;
                    break;
            }

            tr.position = pos;
        }

        public virtual void Initialize(float initVelocity, Vector2 initPosition)
        {
            velocity = initVelocity;
            tr.position = initPosition;

            ItemManager.Instance.AddItem(this);
        }

        protected virtual void OnDisable()
        {
            ItemManager.Instance.RemoveItem(this);
            dragToPlayer = false;
            player = null;
        }

        /// <summary>
        /// Called when the item is destroyed. The default action is to destroy
        /// the item object. You can override it(for example, override to return the item
        /// to the object pool).
        /// </summary>
        public virtual void RemoveItem()
        {
            Destroy(gameObject);
        }

        public void DragToPlayer(Transform player)
        {
            dragToPlayer = true;
            this.player = player;

            ItemManager.Instance.MoveToDraggedItems(this);
        }
    }
}