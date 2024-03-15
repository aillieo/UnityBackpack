// -----------------------------------------------------------------------
// <copyright file="DraggableComp.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [RequireComponent(typeof(ItemDataComp))]
    public class DraggableComp : MonoBehaviour
    {
        // 该物体所属的图层
        public GridLayer itemBelongingLayer;
        // 可放置该物体的图层
        public GridLayer layerRequired;

        public bool isAttachedToBackpackGrid;

        private Vector3 dragStartOffset;

        private Collider2D colliderValue;

        private new Collider2D collider
        {
            get
            {
                if (colliderValue == null)
                {
                    colliderValue = this.gameObject.GetComponent<Collider2D>();
                }

                return colliderValue;
            }
        }

        private Rigidbody2D rigidbodyValue;

        private new Rigidbody2D rigidbody
        {
            get
            {
                if (rigidbodyValue == null)
                {
                    rigidbodyValue = this.gameObject.GetComponent<Rigidbody2D>();
                }

                return rigidbodyValue;
            }
        }

        private GridData gridDataValue;

        public GridData gridData
        {
            get
            {
                if (gridDataValue == null)
                {
                    gridDataValue = this.gameObject.GetComponent<ItemDataComp>().gridData;
                }

                return gridDataValue;
            }
        }

        private void OnEnable()
        {
            // this.transform.position = GridUtils.SnapGrid(this.transform.position, this.gridData.GetShape());
        }

        public void OnDragEnd(Vector3 screenPosition)
        {
            var attached = false;

            var wallGrids = BackpackManager.Instance.wallGrids;
            var gridPosition = GridUtils.WorldPositionToGridPositionLB(this.transform.position, this.gridData.GetShape());
            var offset = gridPosition - BackpackManager.Instance.wallGridsStart;

            bool predicate(int wall, int item)
            {
                if (item == 0)
                {
                    return true;
                }

                var wallLayer = (GridLayer)wall;
                if ((wallLayer & this.layerRequired) == 0)
                {
                    // 不能放置
                    return false;
                }

                if ((wallLayer & this.itemBelongingLayer) != 0)
                {
                    // overlap
                    return false;
                }

                return true;
            }

            if (wallGrids.MatchAll(this.gridData, offset, predicate, 0))
            {
                wallGrids.Operate(this.gridData, offset, (wall, item) =>
                {
                    if (item != 0)
                    {
                        return wall | (int)this.itemBelongingLayer;
                    }

                    return wall;
                }, 0);

                this.isAttachedToBackpackGrid = true;
                this.transform.position = GridUtils.SnapGrid(this.transform.position, this.gridData.GetShape());
                attached = true;
            }

            if (!attached)
            {
                this.SwitchSimulation(true);
            }
        }

        public void OnDrag(Vector3 screenPosition)
        {
            Vector3 position = BackpackManager.Instance.currentCamera.ScreenToWorldPoint(screenPosition);
            position.z = this.transform.position.z;
            this.transform.position = position - dragStartOffset;
        }

        public void OnDragStart(Vector3 screenPosition)
        {
            // 记录偏移值
            var dragStartWorld = BackpackManager.Instance.currentCamera.ScreenToWorldPoint(screenPosition);
            dragStartWorld.z = this.transform.position.z;
            dragStartOffset = dragStartWorld - this.transform.position;

            SwitchSimulation(false);

            if (this.isAttachedToBackpackGrid)
            {
                var wallGrids = BackpackManager.Instance.wallGrids;
                var gridPosition = GridUtils.WorldPositionToGridPositionLB(this.transform.position, this.gridData.GetShape());
                var offset = gridPosition - BackpackManager.Instance.wallGridsStart;

                wallGrids.Operate(this.gridData, offset, (wall, item) =>
                {
                    if (item != 0)
                    {
                        return wall & (~(int)this.itemBelongingLayer);
                    }

                    return wall;
                },
                    0);

                this.isAttachedToBackpackGrid = false;
            }

            this.FixRotation();
        }

        public void OnSimpleTap(Vector3 screenPosition)
        {
        }

        private void SwitchSimulation(bool isOn)
        {
            if (isOn)
            {
                this.rigidbody.velocity = Vector2.zero;
                this.rigidbody.angularVelocity = 0f;
                this.rigidbody.bodyType = RigidbodyType2D.Dynamic;

                this.collider.isTrigger = false;
            }
            else
            {
                this.rigidbody.bodyType = RigidbodyType2D.Kinematic;
                this.collider.isTrigger = true;
            }
        }

        private void FixRotation()
        {
            this.transform.localEulerAngles = GridUtils.SnapAngle(this.transform.localEulerAngles);
        }

        private void OnDrawGizmos()
        {
            // 绘制grids
            var rangeX = this.gridData.Width * GridUtils.gridSize;
            var rangeY = this.gridData.Height * GridUtils.gridSize;
            var leftBottom = this.transform.position - new Vector3(rangeX * 0.5f, rangeY * 0.5f, 0);
            for (var x = 0; x < this.gridData.Width; x++)
            {
                for (var y = 0; y < this.gridData.Height; y++)
                {
                    if (this.gridData[x, y] != 0)
                    {
                        var worldPosition = leftBottom + GridUtils.GridPositionToWorldPosition(new Vector2Int(x, y));
                        Gizmos.DrawWireCube(worldPosition, Vector3.one * 0.4f);
                    }
                }
            }
        }
    }
}
