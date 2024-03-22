// -----------------------------------------------------------------------
// <copyright file="GameObjectExtensions.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class GameObjectExtensions
    {
        public static void GetDirectChildrenComponents<T>(this GameObject gameObject, bool includeInactive, List<T> result)
            where T : Component
        {
            result.Clear();

            foreach (var child in gameObject.transform)
            {
                var childTransform = (Transform)child;
                if (childTransform.gameObject.activeInHierarchy || includeInactive)
                {
                    if (childTransform.TryGetComponent<T>(out var childComp))
                    {
                        result.Add(childComp);
                    }
                }
            }
        }

        public static void GetDirectChildrenComponents<T>(this GameObject gameObject, List<T> result)
            where T : Component
        {
            result.Clear();

            foreach (var child in gameObject.transform)
            {
                var childTransform = (Transform)child;
                if (childTransform.TryGetComponent<T>(out var childComp))
                {
                    result.Add(childComp);
                }
            }
        }
    }
}
