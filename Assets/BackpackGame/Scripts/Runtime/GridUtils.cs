// -----------------------------------------------------------------------
// <copyright file="GridUtils.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class GridUtils
    {
        public static readonly float gridSize = 1;

        public static readonly Dictionary<GridLayer, GridLayer> gridRequirementMappings = new Dictionary<GridLayer, GridLayer>() {
            { GridLayer.Wall, GridLayer.None },
            { GridLayer.Backpack, GridLayer.Wall},
            { GridLayer.Item, GridLayer.Backpack},
            { GridLayer.Diamond, GridLayer.Slot},
        };

        public static Vector2Int WorldPositionToGridPosition(Vector3 worldPosition)
        {
            var x = Mathf.RoundToInt(worldPosition.x / gridSize);
            var y = Mathf.RoundToInt(worldPosition.y / gridSize);
            return new Vector2Int(x, y);
        }

        public static Vector2Int WorldPositionToGridPositionLB(Vector3 worldPosition, Vector2Int gridShape)
        {
            worldPosition.x -= gridShape.x * gridSize * 0.5f;
            worldPosition.y -= gridShape.y * gridSize * 0.5f;

            var x = Mathf.RoundToInt(worldPosition.x / gridSize);
            var y = Mathf.RoundToInt(worldPosition.y / gridSize);
            return new Vector2Int(x, y);
        }

        public static Vector3 GridPositionToWorldPosition(Vector2Int gridPosition, float z = 0)
        {
            var x = (gridPosition.x * gridSize) + (0.5f * gridSize);
            var y = (gridPosition.y * gridSize) + (0.5f * gridSize);
            return new Vector3(x, y, z);
        }

        public static Vector3 SnapGrid(Vector3 worldPosition)
        {
            var grid = WorldPositionToGridPosition(worldPosition);
            return GridPositionToWorldPosition(grid, worldPosition.z);
        }

        public static Vector3 SnapGrid(Vector3 worldPosition, Vector2Int gridShape)
        {
            var grid = WorldPositionToGridPositionLB(worldPosition, gridShape);
            var rangeX = gridShape.x * gridSize;
            var rangeY = gridShape.y * gridSize;
            var leftBottom = GridPositionToWorldPosition(grid);
            var center = leftBottom + new Vector3(rangeX * 0.5f, rangeY * 0.5f, 0);
            center.x -= gridSize * 0.5f;
            center.y -= gridSize * 0.5f;
            center.z = worldPosition.z;
            return center;
        }

        public static int AngleToRotationIndex(float angle)
        {
            angle = angle % 360;
            if (angle < 0)
            {
                angle += 360;
            }

            var rotationIndex = Mathf.RoundToInt(angle / 90);
            rotationIndex = rotationIndex % 4;
            return rotationIndex;
        }

        public static float RotationIndexToAngle(int rotationIndex)
        {
            return rotationIndex * 90;
        }

        public static Vector3 SnapAngle(Vector3 localEulerAngles)
        {
            var index = AngleToRotationIndex(localEulerAngles.z);

            localEulerAngles.z = RotationIndexToAngle(index);
            return localEulerAngles;
        }

        public static bool CanHold(GridData first, GridData other, Vector2Int offset)
        {
            return first.MatchAll(other, offset, (a, b) => {

                UnityEngine.Debug.Log($"in can hold: a={a} b={b}");

                if (b == 0)
                {
                    return true;
                }

                if ((a & b) != 0)
                {
                    // a 已放置了与b相同的
                    // overlap
                    return false;
                }

                if (gridRequirementMappings.TryGetValue(b, out var required))
                {
                    if ((a & required) != required)
                    {
                        // a 不包含 required
                        // 不能放置
                        return false;
                    }
                }
                else
                {
                    // 未定义 requre
                    // 不能放置
                    return false;
                }

                return true;
            }, 0);
        }

        public static void Union(GridData first, GridData other, Vector2Int offset)
        {
            first.Operate(other, offset, (a, b) => a | b, 0);
        }

        public static void Subtract(GridData first, GridData other, Vector2Int offset)
        {
            first.Operate(other, offset, (a, b) => a & ~b, 0);
        }

        public static GridData GetRotated(GridData first, int rotationIndex)
        {
            if (rotationIndex == 0)
            {
                return first;
            }

            GridData rotated;

            int width = first.Width;
            int height = first.Height;

            if (rotationIndex == 1 || rotationIndex == 3)
            {
                rotated = new GridData(first.Height, first.Width);
            }
            else
            {
                // rotationIndex == 2
                rotated = new GridData(first.Width, first.Height);
            }

            int newWidth = rotated.Width;
            int newHeight = rotated.Height;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int newX, newY;

                    if (rotationIndex == 1)
                    {
                        newX = newHeight - 1 - y;
                        newY = x;
                    }
                    else if(rotationIndex == 3)
                    {
                        newX = y;
                        newY = newWidth - 1 - x;
                    }
                    else
                    {
                        // rotationIndex == 2
                        newX = newWidth - 1 - x;
                        newY = newHeight - 1 - y;
                    }

                    rotated[newX, newY] = first[x, y];
                }
            }

            return rotated;
        }
    }
}
