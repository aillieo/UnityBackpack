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

        public static Vector3 SnapAngle(Vector3 localEulerAngles)
        {
            var index = AngleToRotationIndex(localEulerAngles.z);

            localEulerAngles.z = index * 90;
            return localEulerAngles;
        }

        public static bool CanHold(GridData first, GridData other, Vector2Int offset)
        {
            return first.MatchAll(other, offset, (a, b) => a == 0 || b == 0, 0);
        }

        public static void Union(GridData first, GridData other, Vector2Int offset)
        {
            first.Operate(other, offset, (a, b) => a | b, 0);
        }

        public static void Subtract(GridData first, GridData other, Vector2Int offset)
        {
            first.Operate(other, offset, (a, b) => a & ~b, 0);
        }

        public static T[] RotateArray<T>(T[] inputArray, int width, int height, bool clockwise)
        {
            var rotatedShape = new T[width * height];

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var index = (y * width) + x;
                    var newX = clockwise ? y : height - y - 1;
                    var newY = clockwise ? width - x - 1 : x;
                    var newIndex = (newX * height) + newY;
                    rotatedShape[newIndex] = inputArray[index];
                }
            }

            return rotatedShape;
        }
    }
}
