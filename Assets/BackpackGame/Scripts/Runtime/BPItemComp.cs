namespace AillieoTech.Game
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Assertions;

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
                if (physicsCompValue == null)
                {
                    physicsCompValue = this.gameObject.GetComponent<Physics2DComp>();
                }

                return physicsCompValue;
            }
        }

        private GridDataComp gridDataValue;

        public GridDataComp gridData
        {
            get
            {
                if (gridDataValue == null)
                {
                    gridDataValue = this.gameObject.GetComponent<GridDataComp>();
                }

                return gridDataValue;
            }
        }

        private RotateComp rotateCompValue;

        public RotateComp rotateComp
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

        private DraggableComp draggableCompValue;
        private DraggableComp draggableComp
        {
            get
            {
                if (draggableCompValue == null)
                {
                    draggableCompValue = this.gameObject.GetComponent<DraggableComp>();
                }

                return draggableCompValue;
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
        }

        private void OnRotationIndexChanged()
        {
            UnityEngine.Debug.Log("OnRotationIndexChanged: " + this.name);
        }
    }
}
