// -----------------------------------------------------------------------
// <copyright file="BPContainerComp.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using System.Collections.Generic;
    using UnityEngine;

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

        private RotateComp rotateComp
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
        }

        private void OnDisable()
        {
            this.draggableComp.OnDragBegin -= this.OnDragStart;
            this.draggableComp.OnDragEnd -= this.OnDragEnd;
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

            BackpackManager.Instance.wallComp.SetRenererVisible(false);
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
            }

            BackpackManager.Instance.wallComp.SetRenererVisible(true);
        }
    }
}
