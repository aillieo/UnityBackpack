// -----------------------------------------------------------------------
// <copyright file="BackpackManager.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using AillieoUtils;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Assertions;

    public class BackpackManager : Singleton<BackpackManager>
    {
        private WallComp wallCompValue;
        public WallComp wallComp
        {
            get 
            {
                if (wallCompValue == null)
                {
                    wallCompValue = UnityEngine.Object.FindObjectOfType<WallComp>();
                }

                return wallCompValue;
            }
        }

        public Transform wallNode => wallComp.transform;

        public Dictionary<Vector2Int, BPContainerComp> gridToContainerLookup = new Dictionary<Vector2Int, BPContainerComp>();
        public Dictionary<BPContainerComp, Vector2Int> attachedContainers = new Dictionary<BPContainerComp, Vector2Int>();

        public Dictionary<Vector2Int, BPItemComp> gridToItemLookup = new Dictionary<Vector2Int, BPItemComp>();
        public Dictionary<BPItemComp, Vector2Int> attachedItems = new Dictionary<BPItemComp, Vector2Int>();

        public bool TryAttachContainer(BPContainerComp container)
        {
            var containerGridData = container.gridData;
            var wallGridData = wallComp.gridData;

            var containerMin = containerGridData.GetWorldGridStart();
            var containerMax = containerMin + containerGridData.GetWorldShape();
            var wallMin = wallGridData.GetWorldGridStart();
            var wallMax = wallMin + wallGridData.GetWorldShape();

            bool canHold = true;

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

                    gridToContainerLookup[wallLocal] = container;
                }
            }

            attachedContainers[container] = wallGridData.WorldGridToLocalGrid(containerMin);

            return true;
        }

        public bool TryAttachItem(BPItemComp item)
        {
            var itemGridData = item.gridData;
            var wallGridData = wallComp.gridData;

            var itemMin = itemGridData.GetWorldGridStart();
            var itemMax = itemMin + itemGridData.GetWorldShape();
            var wallMin = wallGridData.GetWorldGridStart();
            var wallMax = wallMin + wallGridData.GetWorldShape();

            bool canHold = true;

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

                    gridToItemLookup[wallLocal] = item;
                }
            }

            attachedItems[item] = wallGridData.WorldGridToLocalGrid(itemMin);

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
            var wallGridData = wallComp.gridData;

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

                    // 检查是否有子物体 有的话 先detach
                    if (gridToItemLookup.TryGetValue(wallLocal, out var item))
                    {
                        DetachItem(item);
                        item.physicsComp.SwitchSimulation(true);
                    }

                    wallGridData.gridData[wallLocal.x, wallLocal.y] &= (~GridLayer.Backpack);

                    Assert.IsTrue(gridToContainerLookup.Remove(wallLocal));
                }
            }

            Assert.IsTrue(attachedContainers.Remove(container));

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
            var wallGridData = wallComp.gridData;

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
                    wallGridData.gridData[wallLocal.x, wallLocal.y] &= (~GridLayer.Item);

                    Assert.IsTrue(gridToItemLookup.Remove(wallLocal));
                }
            }

            Assert.IsTrue(attachedItems.Remove(item));

            return true;
        }
    }
}
