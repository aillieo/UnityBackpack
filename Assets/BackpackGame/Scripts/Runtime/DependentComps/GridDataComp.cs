// -----------------------------------------------------------------------
// <copyright file="GridDataComp.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class GridDataComp : MonoBehaviour
    {
        [SerializeField]
        private GridData grids;

        public GridData gridData => this.grids;

        public Vector2Int GetWorldShape()
        {
            var rotationIndex = GridUtils.AngleToRotationIndex(this.transform.eulerAngles.z);

            if (rotationIndex == 1 || rotationIndex == 3)
            {
                return new Vector2Int(this.grids.Height, this.grids.Width);
            }
            else
            {
                return new Vector2Int(this.grids.Width, this.grids.Height);
            }
        }

        public Vector2Int GetWorldGridStart()
        {
            var shape = this.GetWorldShape();

            var rangeX = shape.x * GridUtils.gridSize;
            var rangeY = shape.y * GridUtils.gridSize;
            var leftBottom = this.transform.position - new Vector3(rangeX * 0.5f, rangeY * 0.5f, 0);

            var offset = GridUtils.WorldPositionToGridPosition(leftBottom);

            return offset;
        }

        public GridLayer GetWorldValue(Vector2Int worldGrid, GridLayer valueOutOfRange = default)
        {
            var localGrid = this.WorldGridToLocalGrid(worldGrid);

            if (localGrid.x >= 0 && localGrid.x < this.gridData.Width && localGrid.y >= 0 && localGrid.y < this.gridData.Height)
            {
                return this.gridData[localGrid.x, localGrid.y];
            }
            else
            {
                return valueOutOfRange;
            }
        }

        public Vector2Int WorldGridToLocalGrid(Vector2Int worldGrid)
        {
            var offset = this.GetWorldGridStart();
            var rotationIndex = GridUtils.AngleToRotationIndex(this.transform.eulerAngles.z);

            var width = this.grids.Width;
            var height = this.grids.Height;

            var tempX = worldGrid.x - offset.x;
            var tempY = worldGrid.y - offset.y;

            int localX;
            int localY;

            if (rotationIndex == 1)
            {
                localX = tempY;
                localY = height - 1 - tempX;
            }
            else if (rotationIndex == 3)
            {
                localX = width - 1 - tempY;
                localY = tempX;
            }
            else if (rotationIndex == 2)
            {
                localX = width - 1 - tempX;
                localY = height - 1 - tempY;
            }
            else
            {
                localX = tempX;
                localY = tempY;
            }

            return new Vector2Int(localX, localY);
        }

        private void OnDrawGizmos()
        {
            // 绘制wallGrids
            var start = this.GetWorldGridStart();
            var shape = this.GetWorldShape();

            for (var x = 0; x < shape.x; x++)
            {
                for (var y = 0; y < shape.y; y++)
                {
                    var worldGrid = start + new Vector2Int(x, y);
                    var worldPosition = GridUtils.GridPositionToWorldPosition(worldGrid);

                    var value = this.GetWorldValue(worldGrid);
                    if (value != 0)
                    {
                        if (value == GridLayer.Wall)
                        {
                            Gizmos.DrawWireCube(worldPosition, Vector3.one * 0.5f);
                        }
                        else
                        {
                            var backup = Gizmos.color;
                            if ((value & GridLayer.Backpack) != 0)
                            {
                                Gizmos.color = Color.green;
                                Gizmos.DrawCube(worldPosition, Vector3.one * 0.85f);
                            }

                            if ((value & GridLayer.Item) != 0)
                            {
                                Gizmos.color = Color.red;
                                Gizmos.DrawCube(worldPosition, Vector3.one * 0.9f);
                            }

                            Gizmos.color = backup;
                        }
                    }
                }
            }
        }
    }
}
