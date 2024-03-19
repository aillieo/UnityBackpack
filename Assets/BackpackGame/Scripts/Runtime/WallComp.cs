namespace AillieoTech.Game
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Assertions;

    [RequireComponent(typeof(GridDataComp))]
    [RequireComponent(typeof(DroppableComp))]
    [DisallowMultipleComponent]
    public class WallComp : MonoBehaviour
    {
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

        private void OnDrawGizmos()
        {
            // 绘制wallGrids
            var start = this.gridData.GetWorldGridStart();
            var shape = this.gridData.GetWorldShape();

            for (var x = 0; x < shape.x; x++)
            {
                for (var y = 0; y < shape.y; y++)
                {
                    var worldGrid = start + new Vector2Int(x, y);
                    var worldPosition = GridUtils.GridPositionToWorldPosition(worldGrid);
 
#if UNITY_EDITOR
                    //if (this.gridToItemLookup.TryGetValue(new Vector2Int(x, y), out var list))
                    //{
                    //    UnityEditor.Handles.Label(worldPosition, $"{list.Count}");
                    //}
#endif
                }
            }
        }
    }
}
