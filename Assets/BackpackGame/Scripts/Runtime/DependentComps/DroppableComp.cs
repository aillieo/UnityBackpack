// -----------------------------------------------------------------------
// <copyright file="DroppableComp.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using System;
    using UnityEngine;

    [RequireComponent(typeof(Collider2D))]
    [DisallowMultipleComponent]
    public class DroppableComp : MonoBehaviour
    {
        public event Action<MouseEventData> OnDrop;

        public void HandleDrop(MouseEventData eventData)
        {
            OnDrop?.Invoke(eventData);
        }
    }
}
