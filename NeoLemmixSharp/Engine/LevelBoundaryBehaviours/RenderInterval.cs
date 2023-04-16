using System;
using System.Collections;
using System.Collections.Generic;

namespace NeoLemmixSharp.Engine.LevelBoundaryBehaviours;

public sealed class RenderInterval
{
    public int PixelStart;
    public int PixelLength;

    public int ScreenStart;
    public int ScreenLength;

    public bool Overlaps(int otherPixelStart, int otherPixelLength)
    {
        return PixelStart < otherPixelStart + otherPixelLength &&
               otherPixelStart < PixelStart + PixelLength;
    }
}

public sealed class SimpleList : IReadOnlyList<RenderInterval>
{
    private readonly RenderInterval[] _array;

    public int Count { get; private set; }

    public SimpleList(int maxSize, int initialSize)
    {
        _array = new RenderInterval[maxSize];
        for (var i = 0; i < _array.Length; i++)
        {
            _array[i] = new RenderInterval();
        }

        Count = initialSize;
    }

    public RenderInterval this[int index] => _array[index];

    public void SetSize(int newSize)
    {
        Count = newSize;
    }

    public void SetData(int index, int pixelStart, int pixelLength, int screenStart, int screenLength)
    {
        var item = _array[index];
        item.PixelStart = pixelStart;
        item.PixelLength = pixelLength;
        item.ScreenStart = screenStart;
        item.ScreenLength = screenLength;
    }

    public Enumerator GetEnumerator() => new(this);

    IEnumerator<RenderInterval> IEnumerable<RenderInterval>.GetEnumerator() => new Enumerator(this);
    IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);

    public struct Enumerator : IEnumerator<RenderInterval>
    {
        private readonly SimpleList _list;
        private int _index;
        private RenderInterval? _current;

        public RenderInterval Current => _current!;

        public Enumerator(SimpleList list)
        {
            _list = list;
            _index = 0;
            _current = default;
        }

        public bool MoveNext()
        {
            var localList = _list;

            if (_index < localList.Count)
            {
                _current = localList._array[_index];
                _index++;
                return true;
            }

            _index = _list.Count + 1;
            _current = default;
            return false;
        }

        public void Reset()
        {
            _index = 0;
            _current = default;
        }

        object IEnumerator.Current => Current!;

        void IDisposable.Dispose() { }
    }
}