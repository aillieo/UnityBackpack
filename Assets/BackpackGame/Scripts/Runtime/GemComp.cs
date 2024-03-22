// -----------------------------------------------------------------------
// <copyright file="GemComp.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using UnityEngine;

    [RequireComponent(typeof(Physics2DComp))]
    [RequireComponent(typeof(DraggableComp))]
    [DisallowMultipleComponent]
    public class GemComp : MonoBehaviour
    {
        private DraggableComp draggableCompValue;

        private DraggableComp draggableComp
        {
            get
            {
                if (this.draggableCompValue == null)
                {
                    this.draggableCompValue = this.gameObject.GetComponent<DraggableComp>();
                }

                return this.draggableCompValue;
            }
        }

        private Physics2DComp physicsCompValue;

        public Physics2DComp physicsComp
        {
            get
            {
                if (this.physicsCompValue == null)
                {
                    this.physicsCompValue = this.gameObject.GetComponent<Physics2DComp>();
                }

                return this.physicsCompValue;
            }
        }

        private void OnEnable()
        {
            this.draggableComp.OnDragBegin += this.OnDragStart;
            this.draggableComp.OnDragEnd += this.OnDragEnd;
        }

        private void OnDisable()
        {
            this.draggableComp.OnDragBegin -= this.OnDragStart;
            this.draggableComp.OnDragEnd -= this.OnDragEnd;
        }

        private void OnDragStart(MouseEventData eventData)
        {
            this.physicsComp.SwitchSimulation(false);
        }

        private void OnDragEnd(MouseEventData eventData)
        {
            var attached = false;

            if (eventData.droppable != null && eventData.droppable.TryGetComponent<SlotComp>(out var slot))
            {
                if (BackpackManager.Instance.TryAttachGem(this, slot))
                {
                    attached = true;
                }
            }

            if (!attached)
            {
                this.physicsComp.SwitchSimulation(true);
            }
        }
    }
}
