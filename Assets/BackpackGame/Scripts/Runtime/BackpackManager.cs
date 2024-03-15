// -----------------------------------------------------------------------
// <copyright file="BackpackManager.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using AillieoUtils;
    using UnityEngine;

    public class BackpackManager : SingletonMonoBehaviour<BackpackManager>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureInstance()
        {
            var _ = BackpackManager.Instance;
        }

        public DraggableComp selectedDraggable;

        public Camera currentCamera;

        private MouseEvents mouseEventsValue;

        public GridData wallGrids = new GridData(20, 10);
        public Vector2Int wallGridsStart = new Vector2Int(-10, -5);

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

        protected override void Awake()
        {
            base.Awake();

            // 初始化墙体
            for (var x = 0; x < this.wallGrids.Width; x++)
            {
                for (var y = 0; y < this.wallGrids.Height; y++)
                {
                    this.wallGrids[x, y] = (int)GridLayer.Wall;
                }
            }
        }

        private void OnEnable()
        {
            this.currentCamera = Camera.main;

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

        private Collider2D[] raycastHits = new Collider2D[16];

        public DraggableComp FindDraggable(Vector2 screenPosition)
        {
            var worldPoint = this.currentCamera.ScreenToWorldPoint(screenPosition);
            var result = Physics2D.OverlapPointNonAlloc(worldPoint, this.raycastHits, -1, -100, 100);
            UnityEngine.Debug.Log("result = " + result);
            if (result > 0)
            {
                // 处理击中物体的逻辑
                for (var i = 0; i < result; i++)
                {
                    var collider = this.raycastHits[i];
                    if (collider != null)
                    {
                        var go = collider.gameObject;
                        if (go.TryGetComponent<DraggableComp>(out var draggable))
                        {
                            UnityEngine.Debug.Log("draggable = " + draggable);
                            return draggable;
                        }
                    }
                }
            }

            return null;
        }

        private void OnDragStart(Vector3 screenPosition)
        {
            var draggable = this.FindDraggable(screenPosition);
            if (draggable != null)
            {
                this.selectedDraggable = draggable;
                draggable.OnDragStart(screenPosition);
            }
        }

        private void OnDrag(Vector3 screenPosition)
        {
            if (this.selectedDraggable != null)
            {
                this.selectedDraggable.OnDrag(screenPosition);
            }
        }

        private void OnDragEnd(Vector3 screenPosition)
        {
            if (this.selectedDraggable != null)
            {
                this.selectedDraggable.OnDragEnd(screenPosition);
            }

            this.selectedDraggable = null;
        }

        private void OnDrawGizmos()
        {
            // 绘制wallGrids
            for (var x = 0; x < this.wallGrids.Width; x++)
            {
                for (var y = 0; y < this.wallGrids.Height; y++)
                {
                    var value = this.wallGrids[x, y];
                    if (value != 0)
                    {
                        var worldPosition =
                            GridUtils.GridPositionToWorldPosition(new Vector2Int(x, y) + this.wallGridsStart);

                        if (value == (int)GridLayer.Wall)
                        {
                            Gizmos.DrawWireCube(worldPosition, Vector3.one * 0.5f);
                        }
                        else
                        {
                            var backup = Gizmos.color;
                            if ((value & (int)GridLayer.Backpack) != 0)
                            {
                                Gizmos.color = Color.green;
                                Gizmos.DrawCube(worldPosition, Vector3.one * 0.5f);
                            }

                            if ((value & (int)GridLayer.Item) != 0)
                            {
                                Gizmos.color = Color.red;
                                Gizmos.DrawCube(worldPosition, Vector3.one * 0.25f);
                            }

                            Gizmos.color = backup;
                        }
                    }
                }
            }
        }

        private void OnRightMouseClick(Vector3 screenPosition)
        {
            if (this.selectedDraggable != null)
            {
                if (this.selectedDraggable.gameObject.TryGetComponent<RotateComp>(out var rotateComp))
                {
                    rotateComp.Rotate();
                }
            }
        }
    }
}
