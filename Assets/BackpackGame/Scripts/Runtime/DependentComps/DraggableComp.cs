// -----------------------------------------------------------------------
// <copyright file="DraggableComp.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using System;
    using UnityEngine;

    [RequireComponent(typeof(Collider2D))]
    [DisallowMultipleComponent]
    public class DraggableComp : MonoBehaviour
    {
        public event Action<MouseEventData> OnDragBegin;

        public event Action<MouseEventData> OnDrag;

        public event Action<MouseEventData> OnDragEnd;

        private Vector3 dragStartOffset;

        public void HandleDragEnd(MouseEventData eventData)
        {
            this.OnDragEnd?.Invoke(eventData);
        }

        public void HandleDrag(MouseEventData eventData)
        {
            Vector3 position = eventData.camera.ScreenToWorldPoint(eventData.screenPosition);
            position.z = this.transform.position.z;
            this.transform.position = position - this.dragStartOffset;

            this.OnDrag?.Invoke(eventData);
        }

        public void HandleDragStart(MouseEventData eventData)
        {
            // 记录偏移值
            var dragStartWorld = eventData.camera.ScreenToWorldPoint(eventData.screenPosition);
            dragStartWorld.z = this.transform.position.z;
            this.dragStartOffset = dragStartWorld - this.transform.position;

            this.OnDragBegin?.Invoke(eventData);
        }
    }
}
