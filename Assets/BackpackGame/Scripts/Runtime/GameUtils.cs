// -----------------------------------------------------------------------
// <copyright file="GameUtils.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class GameUtils
    {
        private static readonly Collider2D[] raycastHits = new Collider2D[16];

        public static T SelectComponent<T>(Vector2 screenPosition, Camera camera = null)
            where T : Component
        {
            if (camera == null)
            {
                camera = Camera.main;
            }

            var worldPoint = camera.ScreenToWorldPoint(screenPosition);
            var result = Physics2D.OverlapPointNonAlloc(worldPoint, raycastHits, -1, -100, 100);
            UnityEngine.Debug.Log("result = " + result);
            if (result > 0)
            {
                // 处理击中物体的逻辑
                for (var i = 0; i < result; i++)
                {
                    var collider = raycastHits[i];
                    if (collider != null)
                    {
                        var go = collider.gameObject;
                        if (go.TryGetComponent<T>(out var comp))
                        {
                            return comp;
                        }
                    }
                }
            }

            return null;
        }

        public static IEnumerator<Vector2Int> EnumerateRange(Vector2Int leftBottom, Vector2Int rightTop)
        {
            for (var x = leftBottom.x; x < rightTop.x; x++)
            {
                for (var y = leftBottom.y; y < rightTop.y; y++)
                {
                    yield return new Vector2Int(x, y);
                }
            }
        }

        public static Vector3 GetUIPosition(Vector3 worldPosition, RectTransform parent, Canvas canvas = null, Camera worldCamera = null)
        {
            if (worldCamera == null)
            {
                worldCamera = Camera.main;
            }

            if (canvas == null)
            {
                canvas = parent.gameObject.GetComponentInParent<Canvas>();
            }

            Vector3 screenPos = worldCamera.WorldToScreenPoint(worldPosition);
            Vector3 uiPos = default;
            switch (canvas.renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(
                        parent,
                        screenPos,
                        null,
                        out uiPos);
                    break;
                case RenderMode.ScreenSpaceCamera:
                case RenderMode.WorldSpace:
                    RectTransformUtility.ScreenPointToWorldPointInRectangle(
                        parent,
                        screenPos,
                        canvas.worldCamera,
                        out uiPos);
                    break;
            }

            uiPos.z = 0;
            return uiPos;
        }
    }
}
