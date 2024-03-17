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
    [RequireComponent(typeof(RotateComp))]
    [DisallowMultipleComponent]
    public class DraggableComp : MonoBehaviour
    {
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

        private RotateComp rotateCompValue;

        private RotateComp rotateComp
        {
            get
            {
                if (rotateCompValue == null)
                {
                    rotateCompValue = this.gameObject.GetComponent<RotateComp>();
                }

                return rotateCompValue;
            }
        }

        private void OnEnable()
        {
            // this.transform.position = GridUtils.SnapGrid(this.transform.position, this.gridData.GetShape());
        }

        public void OnDragEnd(Vector3 screenPosition)
        {
            var attached = BackpackManager.Instance.TryAttachItem(this);

            if (attached)
            {
                this.transform.position = GridUtils.SnapGrid(this.transform.position, this.gridData.GetShape());
            }
            else
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
            this.rotateComp.FixRotation();

            var detached = BackpackManager.Instance.DetachItem(this);
            if (!detached)
            {
                //this.rotateComp.FixRotation();
            }
        }

        public void OnSimpleTap(Vector3 screenPosition)
        {
        }

        public void OnRotateRequest()
        {
            rotateComp.Rotate();
        }

        public void SwitchSimulation(bool isOn)
        {
            UnityEngine.Debug.Log("SwitchSimulation " + this.name + " " + isOn);

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
