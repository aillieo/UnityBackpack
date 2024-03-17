// -----------------------------------------------------------------------
// <copyright file="ItemDataComp.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using UnityEngine;

    [DisallowMultipleComponent]
    public class ItemDataComp : MonoBehaviour
    {
        [SerializeField]
        private GridData grids;

        public GridData gridData => this.grids;
    }
}
