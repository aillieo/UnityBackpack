namespace AillieoTech.Game
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Assertions;

    [RequireComponent(typeof(Physics2DComp))]
    [RequireComponent(typeof(GridDataComp))]
    [RequireComponent(typeof(RotateComp))]
    [RequireComponent(typeof(DraggableComp))]
    [DisallowMultipleComponent]
    public class BPContainerComp : MonoBehaviour
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
            var attached = BackpackManager.Instance.TryAttachContainer(this);

            if (attached)
            {
                this.transform.position = GridUtils.SnapGrid(this.transform.position, this.gridData.GetWorldShape());

                // 处理子节点
                var list = new List<BPItemComp>();
                this.gameObject.GetDirectChildrenComponents(list);
                foreach (var childComp in list)
                {
                    childComp.transform.SetParent(BackpackManager.Instance.wallNode, true);

                    var childAttached = BackpackManager.Instance.TryAttachItem(childComp);
                    if (childAttached)
                    {
                        childComp.transform.position = GridUtils.SnapGrid(childComp.transform.position, childComp.gridData.GetWorldShape());
                    }
                    else
                    {
                        childComp.physicsComp.SwitchSimulation(true);
                    }
                }
            }
            else
            {
                // 处理子节点
                var list = new List<BPItemComp>();
                this.gameObject.GetDirectChildrenComponents(list);
                foreach (var childComp in list)
                {
                    childComp.transform.SetParent(BackpackManager.Instance.wallNode, true);
                    childComp.physicsComp.SwitchSimulation(true);
                }

                this.physicsComp.SwitchSimulation(true);
            }
        }

        public void OnDragStart(MouseEventData eventData)
        {
            this.transform.SetParent(BackpackManager.Instance.wallNode, true);

            this.physicsComp.SwitchSimulation(false);
            this.rotateComp.FixRotation();

            // 处理子节点
            foreach (var pair in BackpackManager.Instance.gridToContainerLookup)
            {
                if (pair.Value == this)
                {
                    if (BackpackManager.Instance.gridToItemLookup.TryGetValue(pair.Key, out var item))
                    {
                        BackpackManager.Instance.DetachItem(item);
                        item.transform.SetParent(this.transform, true);
                    }
                }
            }

            var detached = BackpackManager.Instance.DetachContainer(this);
            if (!detached)
            {
                //this.rotateComp.FixRotation();
            }
        }

        private void OnRotationIndexChanged()
        {
            UnityEngine.Debug.Log("OnRotationIndexChanged: " + this.name);

            // 处理子节点
            //var list = new List<BPItemComp>();
            //this.gameObject.GetDirectChildrenComponents(list);
            //foreach (var childComp in list)
            //{
            //    childComp.rotateComp.RecalculateRotationIndex();
            //}
        }
    }
}
