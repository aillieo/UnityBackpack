// -----------------------------------------------------------------------
// <copyright file="GridUtils.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using UnityEngine;

    public static class GridUtils
    {
        public static readonly float gridSize = 1;

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

        public static Vector3 SnapAngle(Vector3 eulerAngles)
        {
            var index = AngleToRotationIndex(eulerAngles.z);

            eulerAngles.z = RotationIndexToAngle(index);
            return eulerAngles;
        }
    }
}
