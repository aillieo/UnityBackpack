// -----------------------------------------------------------------------
// <copyright file="GridLayer.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using System;

    [Flags]
    public enum GridLayer
    {
        None = 0,
        Wall = 1,
        Backpack = 2,
        Item = 4,
        Slot = 8,
        Gem = 16,
        Shelf = 32,
    }
}
