// -----------------------------------------------------------------------
// <copyright file="SellBoxComp.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using UnityEngine;

    [RequireComponent(typeof(DroppableComp))]
    [DisallowMultipleComponent]
    public class SellBoxComp : MonoBehaviour
    {
        private DroppableComp droppableCompValue;

        private DroppableComp droppableComp
        {
            get
            {
                if (this.droppableCompValue == null)
                {
                    this.droppableCompValue = this.gameObject.GetComponent<DroppableComp>();
                }

                return this.droppableCompValue;
            }
        }

        private void OnEnable()
        {
            this.droppableComp.OnDrop += this.OnDrop;
        }

        private void OnDisable()
        {
            this.droppableComp.OnDrop -= this.OnDrop;
        }

        private void OnDrop(MouseEventData eventData)
        {
            if (eventData.draggable != null)
            {
                GameObject.Destroy(eventData.draggable.gameObject);
            }
        }
    }
}
