// -----------------------------------------------------------------------
// <copyright file="BackpackManager.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using AillieoUtils;
    using System.Collections.Generic;
    using System.Linq;
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

        public Dictionary<Vector2Int, List<DraggableComp>> gridToItemLookup = new Dictionary<Vector2Int, List<DraggableComp>>();
        public Dictionary<DraggableComp, Vector2Int> attachedItems = new Dictionary<DraggableComp, Vector2Int>();

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

        public bool TryAttachItem(DraggableComp item)
        {
            var gridPosition = GridUtils.WorldPositionToGridPositionLB(item.transform.position, item.gridData.GetShape());
            var offset = gridPosition - wallGridsStart;

            if (GridUtils.CanHold(wallGrids, item.gridData, offset))
            {
                GridUtils.Union(wallGrids, item.gridData, offset);

                this.attachedItems.Add(item, gridPosition);

                for (var x = 0; x < item.gridData.Width; x++)
                {
                    for (var y = 0; y < item.gridData.Height; y++)
                    {
                        if (item.gridData[x, y] != 0)
                        {
                            var grid = gridPosition = new Vector2Int(x, y);
                            var list = this.gridToItemLookup.GetOrAdd(grid, () => new List<DraggableComp>());
                            list.Add(item);
                        }
                    }
                }

                return true;
            }

            return false;
        }

        public bool DetachItem(DraggableComp item)
        {
            if (!this.attachedItems.TryGetValue(item, out var gridLB))
            {
                return false;
            }

            Debug.Log("will detach " + item);

            var gridPosition = GridUtils.WorldPositionToGridPositionLB(item.transform.position, item.gridData.GetShape());
            var offset = gridPosition - wallGridsStart;

            GridUtils.Subtract(wallGrids, item.gridData, offset);

            this.attachedItems.Remove(item);

            for (var x = 0; x < item.gridData.Width; x++)
            {
                for (var y = 0; y < item.gridData.Height; y++)
                {
                    if (item.gridData[x, y] != 0)
                    {
                        var grid = gridPosition = new Vector2Int(x, y);
                        var list = this.gridToItemLookup.GetOrAdd(grid, () => new List<DraggableComp>());

                        var index = list.IndexOf(item);
                        if (index >= 0)
                        {
                            // 需要移除 index 后边的
                            for (int i = list.Count - 1; i > index; i--)
                            {
                                var item2 = list[i];
                                DetachItem(item2);
                                item2.SwitchSimulation(true);
                            }
                        }
                    }
                }
            }

            return true;
        }

        protected override void Awake()
        {
            base.Awake();

            // 初始化墙体
            for (var x = 0; x < this.wallGrids.Width; x++)
            {
                for (var y = 0; y < this.wallGrids.Height; y++)
                {
                    this.wallGrids[x, y] = GridLayer.Wall;
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

                        if (value == GridLayer.Wall)
                        {
                            Gizmos.DrawWireCube(worldPosition, Vector3.one * 0.5f);
                        }
                        else
                        {
                            var backup = Gizmos.color;
                            if ((value & GridLayer.Backpack) != 0)
                            {
                                Gizmos.color = Color.green;
                                Gizmos.DrawCube(worldPosition, Vector3.one * 0.5f);
                            }

                            if ((value & GridLayer.Item) != 0)
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
                this.selectedDraggable.OnRotateRequest();
            }
        }
    }
}
