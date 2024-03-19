// -----------------------------------------------------------------------
// <copyright file="GameUtils.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
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
    }
}
