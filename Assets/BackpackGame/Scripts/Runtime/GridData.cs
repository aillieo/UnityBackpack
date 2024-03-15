namespace AillieoTech.Game
{
    using System;
    using System.Text;
    using UnityEngine;

    [Serializable]
    public class GridData
    {
        public GridData(int width, int height)
        {
            this.width = width;
            this.data = new int[width * height];
        }

        [SerializeField]
        private int width;

        [SerializeField]
        private int[] data;

        public int Width
        {
            get => width;
        }

        public int Height
        {
            get => data.Length / width;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            var height = this.data.Length / this.width;

            for (var x = 0; x < this.width; x++)
            {
                for (var y = height - 1; y >= 0 ; y--)
                {
                    stringBuilder.Append(this[x, y]);
                    stringBuilder.Append(',');
                }

                stringBuilder.AppendLine();
            }

            return stringBuilder.ToString();
        }

        public int this[int x, int y]
        {
            get => data[x + y * width];
            set => data[x + y * width] = value;
        }

        public void Operate(GridData other, Vector2Int offset, Func<int, int, int> operation, int valueOutOfRange)
        {
            var height = this.Height;
            var otherWidth = other.Width;
            var otherHeight = other.Height;

            for (var x = 0; x < this.Width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var otherX = x - offset.x;
                    var otherY = y - offset.y;

                    if (otherX >= 0 && otherX < otherWidth && otherY >= 0 && otherY < otherHeight)
                    {
                        this[x, y] = operation(this[x, y], other[otherX, otherY]);
                    }
                    else
                    {
                        this[x, y] = operation(this[x, y], valueOutOfRange);
                    }
                }
            }
        }

        public bool MatchAll(GridData other, Vector2Int offset, Func<int, int, bool> operation, int valueOutOfRange)
        {
            var height = this.Height;
            var otherWidth = other.Width;
            var otherHeight = other.Height;

            for (var x = 0; x < this.Width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var otherX = x - offset.x;
                    var otherY = y - offset.y;

                    if (otherX >= 0 && otherX < otherWidth && otherY >= 0 && otherY < otherHeight)
                    {
                        if (!operation(this[x, y], other[otherX, otherY]))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!operation(this[x, y], valueOutOfRange))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public bool MatchAny(GridData other, Vector2Int offset, Func<int, int, bool> operation, int valueOutOfRange)
        {
            var height = this.Height;
            var otherWidth = other.Width;
            var otherHeight = other.Height;

            for (var x = 0; x < this.Width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var otherX = x - offset.x;
                    var otherY = y - offset.y;

                    if (otherX >= 0 && otherX < otherWidth && otherY >= 0 && otherY < otherHeight)
                    {
                        if (operation(this[x, y], other[otherX, otherY]))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (operation(this[x, y], valueOutOfRange))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public Vector2Int GetShape()
        {
            return new Vector2Int(this.Width, this.Height);
        }
    }
}
