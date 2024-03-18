
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
        private const float clickThreshold = 0.1f;

        private void Update()
        {
            // 鼠标左键
            if (Input.GetMouseButtonDown(0))
            {
                leftMouseDownPosition = Input.mousePosition;
                lastMousePosition = Input.mousePosition;
                leftMouseDownTime = Time.time;
                OnMouseDown?.Invoke(Input.mousePosition);
            }

            if (Input.GetMouseButton(0))
            {
                if (!isDragging)
                {
                    var distance = Vector3.Distance(leftMouseDownPosition, Input.mousePosition);
                    if (distance > dragThreshold)
                    {
                        isDragging = true;
                        OnMouseDragStart?.Invoke(Input.mousePosition);
                    }
                }
                else
                {
                    var distance = Vector3.Distance(lastMousePosition, Input.mousePosition);
                    if (distance > Vector3.kEpsilon)
                    {
                        OnMouseDrag?.Invoke(Input.mousePosition);
                    }
                }

                lastMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (isDragging)
                {
                    OnMouseDragEnd?.Invoke(Input.mousePosition);
                }
                else
                {
                    var pressedTime = Time.time - leftMouseDownTime;
                    if (pressedTime < clickThreshold)
                    {
                        OnMouseClick?.Invoke(Input.mousePosition);
                    }
                }

                isDragging = false;
                this.OnMouseUp?.Invoke(Input.mousePosition);
            }

            // 鼠标右键
            if (Input.GetMouseButtonDown(1))
            {
                rightMouseDownPosition = Input.mousePosition;
                rightMouseDownTime = Time.time;
            }
            
            if (Input.GetMouseButtonUp(1))
            {
                var pressedTime = Time.time - rightMouseDownTime;
                var distance = Vector3.Distance(rightMouseDownPosition, Input.mousePosition);
                UnityEngine.Debug.Log($"pressedTime < clickThreshold = {pressedTime < clickThreshold} distance < dragThreshold = {distance < dragThreshold}");
                if (pressedTime < clickThreshold && distance < dragThreshold)
                {
                    OnRightMouseClick?.Invoke(Input.mousePosition);
                }
            }
        }
    }
}
