using System.Collections;
using System.Collections.Generic;

using SansyHuman.Item;
using SansyHuman.UDE.Management;
using UnityEngine;

namespace SansyHuman.Management
{
    public class ItemManager : UDESingleton<ItemManager>
    {
        [SerializeField]
        [Tooltip("Power item. It should use object pool.")]
        private ItemBase powerItem;

        [SerializeField]
        [Tooltip("The minimum power. The size of the power item is (1 + 0.5 * log(Power / Base Power Unit))")]
        private float basePowerUnit = 0.01f;

        private HashSet<ItemBase> notDraggedItems;
        private HashSet<ItemBase> draggedItems;

        protected override void Awake()
        {
            base.Awake();

            notDraggedItems = new HashSet<ItemBase>();
            draggedItems = new HashSet<ItemBase>();
        }

        // Internal use.
        internal void AddItem(ItemBase item)
        {
            notDraggedItems.Add(item);
        }

        // Internal use.
        internal void MoveToDraggedItems(ItemBase item)
        {
            notDraggedItems.Remove(item);
            draggedItems.Add(item);
        }

        // Internal use.
        internal void RemoveItem(ItemBase item)
        {
            notDraggedItems.Remove(item);
            draggedItems.Remove(item);
        }

        public void DestroyAllItems()
        {
            ItemBase[] nd = new ItemBase[notDraggedItems.Count];
            notDraggedItems.CopyTo(nd);

            ItemBase[] d = new ItemBase[draggedItems.Count];
            draggedItems.CopyTo(d);

            foreach (var e in nd)
                e.RemoveItem();

            foreach (var e in d)
                e.RemoveItem();

            notDraggedItems.Clear();
            draggedItems.Clear();
        }

        /// <summary>
        /// Summons the power item. The power item should use object pool.
        /// </summary>
        /// <param name="power">The power of the item</param>
        /// <param name="initVelocity">Initial velocity of the item</param>
        /// <param name="initPosition">Initial position of the item</param>
        public void SummonPowerItem(float power, float initVelocity, Vector2 initPosition)
        {
            Power item = ObjectPool.Instance.GetObject(powerItem) as Power;
            item.PowerPoint = power;

            Transform itemTr = item.transform;
            float scale = 1 + 0.5f * Mathf.Log10(power / basePowerUnit);
            itemTr.localScale = new Vector3(scale, scale, scale);

            item.Initialize(initVelocity, initPosition);
        }

        public IEnumerator<ItemBase> GetUndraggedItems()
        {
            return notDraggedItems.GetEnumerator();
        }
    }
}