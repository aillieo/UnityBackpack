namespace AillieoTech.Game
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    [Serializable]
    public class BaseGridData<T> : IEnumerable<KeyValuePair<Vector2Int, T>>
    {
        public BaseGridData(int width, int height)
        {
            this.width = width;
            this.data = new T[width * height];
        }

        [SerializeField]
        private int width;

        [SerializeField]
        private T[] data;

        public int Width
        {
            get => width;
        }

        public int Height
        {
            get => data.Length / width;
        }

        public T this[int x, int y]
        {
            get => data[x + y * width];
            set => data[x + y * width] = value;
        }

        public void Operate(BaseGridData<T> other, Vector2Int offset, Func<T, T, T> operation, T valueOutOfRange)
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

        public bool MatchAll(BaseGridData<T> other, Vector2Int offset, Func<T, T, bool> operation, T valueOutOfRange)
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

        public bool MatchAny(BaseGridData<T> other, Vector2Int offset, Func<T, T, bool> operation, T valueOutOfRange)
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.data.GetEnumerator();
        }

        IEnumerator<KeyValuePair<Vector2Int, T>> IEnumerable<KeyValuePair<Vector2Int, T>>.GetEnumerator()
        {
            for (var x = 0; x < this.Width; x++)
            {
                for (var y = 0; y < this.Height; y++)
                {
                    var grid = new Vector2Int(x, y);
                    var value = this[x, y];
                    yield return new KeyValuePair<Vector2Int, T>(grid, value);
                }
            }
        }
    }
}
