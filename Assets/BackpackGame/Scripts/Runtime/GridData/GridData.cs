// -----------------------------------------------------------------------
// <copyright file="GridData.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using System;
    using System.Text;

    [Serializable]
    public class GridData : BaseGridData<GridLayer>
    {
        public GridData(int width, int height)
            : base(width, height)
        {
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            var width = this.Width;
            var height = this.Height;

            for (var x = 0; x < width; x++)
            {
                for (var y = height - 1; y >= 0; y--)
                {
                    stringBuilder.Append(this[x, y]);
                    stringBuilder.Append(',');
                }

                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }
    }
}
