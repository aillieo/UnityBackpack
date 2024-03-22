// -----------------------------------------------------------------------
// <copyright file="DragDropHandler.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using AillieoUtils;
    using UnityEngine;

    public class DragDropHandler : SingletonMonoBehaviour<DragDropHandler>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureInstance()
        {
            var _ = DragDropHandler.Instance;
        }

        private Camera currentCamera;

        private readonly MouseEventData current = new MouseEventData();

        private MouseEvents mouseEventsValue;

        private MouseEvents mouseEvents
        {
            get
            {
                if (this.mouseEventsValue == null)
                {
                    this.mouseEventsValue = this.gameObject.AddComponent<MouseEvents>();
                }

                return this.mouseEventsValue;
            }
        }

        private void OnEnable()
        {
            this.current.camera = Camera.main;

            this.mouseEvents.OnMouseDragStart += this.OnDragStart;
            this.mouseEvents.OnMouseDrag += this.OnDrag;
            this.mouseEvents.OnMouseDragEnd += this.OnDragEnd;
            this.mouseEvents.OnRightMouseClick += this.OnRightMouseClick;
        }

        private void OnDisable()
        {
            this.mouseEvents.OnMouseDragStart -= this.OnDragStart;
            this.mouseEvents.OnMouseDrag -= this.OnDrag;
            this.mouseEvents.OnMouseDragEnd -= this.OnDragEnd;
            this.mouseEvents.OnRightMouseClick -= this.OnRightMouseClick;
        }

        private void OnDragStart(Vector3 screenPosition)
        {
            this.current.screenPosition = screenPosition;

            var draggable = GameUtils.SelectComponent<DraggableComp>(screenPosition, this.currentCamera);
            if (draggable != null)
            {
                this.current.draggable = draggable;
                draggable.HandleDragStart(this.current);
            }
        }

        private void OnDrag(Vector3 screenPosition)
        {
            this.current.screenPosition = screenPosition;

            if (this.current.draggable != null)
            {
                this.current.draggable.HandleDrag(this.current);
            }
        }

        private void OnDragEnd(Vector3 screenPosition)
        {
            this.current.screenPosition = screenPosition;

            if (this.current.draggable != null)
            {
                var draggable = this.current.draggable;
                var droppable = GameUtils.SelectComponent<DroppableComp>(screenPosition, this.currentCamera);
                UnityEngine.Debug.Log("droppable = " + droppable);
                if (droppable != null)
                {
                    this.current.droppable = droppable;
                    droppable.HandleDrop(this.current);
                }

                draggable.HandleDragEnd(this.current);
            }

            this.current.draggable = null;
            this.current.droppable = null;
        }

        private void OnRightMouseClick(Vector3 screenPosition)
        {
            this.current.screenPosition = screenPosition;

            if (this.current.draggable != null)
            {
                if (this.current.draggable.gameObject.TryGetComponent<RotateComp>(out var rotate))
                {
                    rotate.Rotate();
                }
            }
        }
    }
}
