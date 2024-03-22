// -----------------------------------------------------------------------
// <copyright file="BPItemComp.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using UnityEngine;

    [RequireComponent(typeof(Physics2DComp))]
    [RequireComponent(typeof(GridDataComp))]
    [RequireComponent(typeof(RotateComp))]
    [RequireComponent(typeof(DraggableComp))]
    [DisallowMultipleComponent]
    public class BPItemComp : MonoBehaviour
    {
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

        private GridDataComp gridDataValue;

        public GridDataComp gridData
        {
            get
            {
                if (this.gridDataValue == null)
                {
                    this.gridDataValue = this.gameObject.GetComponent<GridDataComp>();
                }

                return this.gridDataValue;
            }
        }

        private RotateComp rotateCompValue;

        public RotateComp rotateComp
        {
            get
            {
                if (this.rotateCompValue == null)
                {
                    this.rotateCompValue = this.gameObject.GetComponent<RotateComp>();
                }

                return this.rotateCompValue;
            }
        }

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

        private void OnEnable()
        {
            this.draggableComp.OnDragBegin += this.OnDragStart;
            this.draggableComp.OnDragEnd += this.OnDragEnd;
            this.rotateComp.OnRotationIndexChanged += this.OnRotationIndexChanged;
        }

        private void OnDisable()
        {
            this.draggableComp.OnDragBegin -= this.OnDragStart;
            this.draggableComp.OnDragEnd -= this.OnDragEnd;
            this.rotateComp.OnRotationIndexChanged -= this.OnRotationIndexChanged;
        }

        public void OnDragEnd(MouseEventData eventData)
        {
            var attached = BackpackManager.Instance.TryAttachItem(this);

            if (attached)
            {
                this.transform.position = GridUtils.SnapGrid(this.transform.position, this.gridData.GetWorldShape());
            }
            else
            {
                this.physicsComp.SwitchSimulation(true);
            }

            BackpackManager.Instance.wallComp.SetRenererVisible(false);
        }

        public void OnDragStart(MouseEventData eventData)
        {
            this.transform.SetParent(BackpackManager.Instance.wallNode, true);

            this.physicsComp.SwitchSimulation(false);
            this.rotateComp.FixRotation();

            var detached = BackpackManager.Instance.DetachItem(this);
            if (!detached)
            {
                //this.rotateComp.FixRotation();
            }

            BackpackManager.Instance.wallComp.SetRenererVisible(true);
        }

        private void OnRotationIndexChanged()
        {
            UnityEngine.Debug.Log("OnRotationIndexChanged: " + this.name);
        }
    }
}
