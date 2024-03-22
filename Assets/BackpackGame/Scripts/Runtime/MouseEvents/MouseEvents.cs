// -----------------------------------------------------------------------
// <copyright file="MouseEvents.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using System;
    using UnityEngine;

    public class MouseEvents : MonoBehaviour
    {
        public event Action<Vector3> OnMouseDown;

        public event Action<Vector3> OnMouseDragStart;

        public event Action<Vector3> OnMouseDrag;

        public event Action<Vector3> OnMouseDragEnd;

        public event Action<Vector3> OnMouseClick;

        public event Action<Vector3> OnMouseUp;

        public event Action<Vector3> OnRightMouseClick;

        private Vector3 lastMousePosition;
        private bool isDragging = false;
        private Vector3 leftMouseDownPosition;
        private Vector3 rightMouseDownPosition;
        private float leftMouseDownTime;
        private float rightMouseDownTime;

        private const float dragThreshold = 10;
        private const float clickThreshold = 1.6f;

        private void Update()
        {
            // 鼠标左键
            if (Input.GetMouseButtonDown(0))
            {
                this.leftMouseDownPosition = Input.mousePosition;
                this.lastMousePosition = Input.mousePosition;
                this.leftMouseDownTime = Time.time;
                this.OnMouseDown?.Invoke(Input.mousePosition);
            }

            if (Input.GetMouseButton(0))
            {
                if (!this.isDragging)
                {
                    var distance = Vector3.Distance(this.leftMouseDownPosition, Input.mousePosition);
                    if (distance > dragThreshold)
                    {
                        this.isDragging = true;
                        this.OnMouseDragStart?.Invoke(Input.mousePosition);
                    }
                }
                else
                {
                    var distance = Vector3.Distance(this.lastMousePosition, Input.mousePosition);
                    if (distance > Vector3.kEpsilon)
                    {
                        this.OnMouseDrag?.Invoke(Input.mousePosition);
                    }
                }

                this.lastMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (this.isDragging)
                {
                    this.OnMouseDragEnd?.Invoke(Input.mousePosition);
                }
                else
                {
                    var pressedTime = Time.time - this.leftMouseDownTime;
                    if (pressedTime < clickThreshold)
                    {
                        this.OnMouseClick?.Invoke(Input.mousePosition);
                    }
                }

                this.isDragging = false;
                this.OnMouseUp?.Invoke(Input.mousePosition);
            }

            // 鼠标右键
            if (Input.GetMouseButtonDown(1))
            {
                this.rightMouseDownPosition = Input.mousePosition;
                this.rightMouseDownTime = Time.time;
            }

            if (Input.GetMouseButtonUp(1))
            {
                var pressedTime = Time.time - this.rightMouseDownTime;
                var distance = Vector3.Distance(this.rightMouseDownPosition, Input.mousePosition);
                UnityEngine.Debug.Log($"pressedTime < clickThreshold = {pressedTime < clickThreshold} distance < dragThreshold = {distance < dragThreshold}");
                if (pressedTime < clickThreshold && distance < dragThreshold)
                {
                    this.OnRightMouseClick?.Invoke(Input.mousePosition);
                }
            }
        }
    }
}
