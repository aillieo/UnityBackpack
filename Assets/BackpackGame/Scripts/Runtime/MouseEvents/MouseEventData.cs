// -----------------------------------------------------------------------
// <copyright file="MouseEventData.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using UnityEngine;

    public class MouseEventData
    {
        public Camera camera;
        public Vector3 screenPosition;
        public DraggableComp draggable;
        public DroppableComp droppable;
    }
}
