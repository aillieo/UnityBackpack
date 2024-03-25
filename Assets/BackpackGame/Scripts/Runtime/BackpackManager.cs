// -----------------------------------------------------------------------
// <copyright file="BackpackManager.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Assertions;

#if AILLIEO_UNITY_SINGLETON
    public class BackpackManager : AillieoUtils.Singleton<BackpackManager>
    {
#else
    public class BackpackManager
    {
        public static readonly BackpackManager Instance = new BackpackManager();
#endif

        public event Action<bool> OnChangeSlotVisibilityRequested;

        private WallComp wallCompValue;

        public WallComp wallComp
        {
            get
            {
                if (this.wallCompValue == null)
                {
                    this.wallCompValue = UnityEngine.Object.FindObjectOfType<WallComp>();
                }

                return this.wallCompValue;
            }
        }

        public Transform wallNode => this.wallComp.transform;

        public readonly Dictionary<Vector2Int, BPContainerComp> gridToContainerLookup = new Dictionary<Vector2Int, BPContainerComp>();
        public readonly Dictionary<BPContainerComp, Vector2Int> attachedContainers = new Dictionary<BPContainerComp, Vector2Int>();

        public readonly Dictionary<Vector2Int, BPItemComp> gridToItemLookup = new Dictionary<Vector2Int, BPItemComp>();
        public readonly Dictionary<BPItemComp, Vector2Int> attachedItems = new Dictionary<BPItemComp, Vector2Int>();

        public bool TryAttachContainer(BPContainerComp container)
        {
            var containerGridData = container.gridData;
            var wallGridData = this.wallComp.gridData;

            var containerMin = containerGridData.GetWorldGridStart();
            var containerMax = containerMin + containerGridData.GetWorldShape();
            var wallMin = wallGridData.GetWorldGridStart();
            var wallMax = wallMin + wallGridData.GetWorldShape();

            var canHold = true;

            for (var x = containerMin.x; x < containerMax.x; x++)
            {
                for (var y = containerMin.y; y < containerMax.y; y++)
                {
                    if (containerGridData.GetWorldValue(new Vector2Int(x, y)) == 0)
                    {
                        continue;
                    }

                    if (x < wallMin.x || x >= wallMax.x || y < wallMin.y || y >= wallMax.y)
                    {
                        // 超出wall边界了
                        canHold = false;
                        break;
                    }

                    if ((wallGridData.GetWorldValue(new Vector2Int(x, y)) & GridLayer.Backpack) > 0)
                    {
                        // overlap
                        canHold = false;
                        break;
                    }
                }
            }

            if (!canHold)
            {
                return false;
            }

            for (var x = containerMin.x; x < containerMax.x; x++)
            {
                for (var y = containerMin.y; y < containerMax.y; y++)
                {
                    if (containerGridData.GetWorldValue(new Vector2Int(x, y)) == 0)
                    {
                        continue;
                    }

                    var worldGrid = new Vector2Int(x, y);
                    var wallLocal = wallGridData.WorldGridToLocalGrid(worldGrid);
                    wallGridData.gridData[wallLocal.x, wallLocal.y] |= GridLayer.Backpack;

                    this.gridToContainerLookup[wallLocal] = container;
                }
            }

            this.attachedContainers[container] = wallGridData.WorldGridToLocalGrid(containerMin);

            return true;
        }

        public bool TryAttachItem(BPItemComp item)
        {
            var itemGridData = item.gridData;
            var wallGridData = this.wallComp.gridData;

            var itemMin = itemGridData.GetWorldGridStart();
            var itemMax = itemMin + itemGridData.GetWorldShape();
            var wallMin = wallGridData.GetWorldGridStart();
            var wallMax = wallMin + wallGridData.GetWorldShape();

            var canHold = true;

            for (var x = itemMin.x; x < itemMax.x; x++)
            {
                for (var y = itemMin.y; y < itemMax.y; y++)
                {
                    if (itemGridData.GetWorldValue(new Vector2Int(x, y)) == 0)
                    {
                        continue;
                    }

                    if (x < wallMin.x || x >= wallMax.x || y < wallMin.y || y >= wallMax.y)
                    {
                        // 超出wall边界了
                        canHold = false;
                        break;
                    }

                    if ((wallGridData.GetWorldValue(new Vector2Int(x, y)) & GridLayer.Backpack) == 0)
                    {
                        // 没有背包
                        canHold = false;
                        break;
                    }

                    if ((wallGridData.GetWorldValue(new Vector2Int(x, y)) & GridLayer.Item) > 0)
                    {
                        // overlap
                        canHold = false;
                        break;
                    }
                }
            }

            if (!canHold)
            {
                return false;
            }

            for (var x = itemMin.x; x < itemMax.x; x++)
            {
                for (var y = itemMin.y; y < itemMax.y; y++)
                {
                    if (itemGridData.GetWorldValue(new Vector2Int(x, y)) == 0)
                    {
                        continue;
                    }

                    var worldGrid = new Vector2Int(x, y);
                    var wallLocal = wallGridData.WorldGridToLocalGrid(worldGrid);
                    wallGridData.gridData[wallLocal.x, wallLocal.y] |= GridLayer.Item;

                    this.gridToItemLookup[wallLocal] = item;
                }
            }

            this.attachedItems[item] = wallGridData.WorldGridToLocalGrid(itemMin);

            return true;
        }

        public bool DetachContainer(BPContainerComp container)
        {
            if (!this.attachedContainers.TryGetValue(container, out var gridLB))
            {
                return false;
            }

            Debug.Log("will detach " + container);

            var containerGridData = container.gridData;
            var wallGridData = this.wallComp.gridData;

            var containerMin = containerGridData.GetWorldGridStart();
            var containerMax = containerMin + containerGridData.GetWorldShape();
            var wallMin = wallGridData.GetWorldGridStart();
            var wallMax = wallMin + wallGridData.GetWorldShape();

            for (var x = containerMin.x; x < containerMax.x; x++)
            {
                for (var y = containerMin.y; y < containerMax.y; y++)
                {
                    if (containerGridData.GetWorldValue(new Vector2Int(x, y)) == 0)
                    {
                        continue;
                    }

                    var worldGrid = new Vector2Int(x, y);
                    var wallLocal = wallGridData.WorldGridToLocalGrid(worldGrid);

                    wallGridData.gridData[wallLocal.x, wallLocal.y] &= ~GridLayer.Backpack;

                    Assert.IsTrue(this.gridToContainerLookup.Remove(wallLocal));
                }
            }

            Assert.IsTrue(this.attachedContainers.Remove(container));

            return true;
        }

        public bool DetachItem(BPItemComp item)
        {
            if (!this.attachedItems.TryGetValue(item, out var gridLB))
            {
                return false;
            }

            Debug.Log("will detach " + item);

            var itemGridData = item.gridData;
            var wallGridData = this.wallComp.gridData;

            var itemMin = itemGridData.GetWorldGridStart();
            var itemMax = itemMin + itemGridData.GetWorldShape();
            var wallMin = wallGridData.GetWorldGridStart();
            var wallMax = wallMin + wallGridData.GetWorldShape();

            for (var x = itemMin.x; x < itemMax.x; x++)
            {
                for (var y = itemMin.y; y < itemMax.y; y++)
                {
                    if (itemGridData.GetWorldValue(new Vector2Int(x, y)) == 0)
                    {
                        continue;
                    }

                    var worldGrid = new Vector2Int(x, y);
                    var wallLocal = wallGridData.WorldGridToLocalGrid(worldGrid);
                    wallGridData.gridData[wallLocal.x, wallLocal.y] &= ~GridLayer.Item;

                    Assert.IsTrue(this.gridToItemLookup.Remove(wallLocal));
                }
            }

            Assert.IsTrue(this.attachedItems.Remove(item));

            return true;
        }

        public bool TryAttachGem(GemComp gem, SlotComp slot)
        {
            var item = slot.GetComponentInParent<BPItemComp>();
            if (item == null)
            {
                throw new InvalidOperationException();
            }

            if (!this.attachedItems.TryGetValue(item, out var lb))
            {
                // 自由状态的item 不允许安装gem
                return false;
            }

            var oldGem = new List<GemComp>();
            slot.gameObject.GetDirectChildrenComponents(oldGem);
            if (oldGem.Count > 0)
            {
                // 已经安装的有宝石了
                return false;
            }

            gem.transform.SetParent(slot.transform, false);
            gem.transform.localPosition = Vector3.zero;
            gem.transform.localEulerAngles = Vector3.zero;
            return true;
        }

        public bool DettachGem(GemComp gem)
        {
            var slot = gem.GetComponentInParent<SlotComp>();
            if (slot == null)
            {
                return false;
            }

            gem.transform.SetParent(this.wallNode, true);
            return true;
        }

        public void RequestChangeSlotVisibility(bool visible)
        {
            this.OnChangeSlotVisibilityRequested?.Invoke(visible);
        }
    }
}
