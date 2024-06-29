using System.Collections.Generic;

#nullable enable
namespace BeamNG.RemoteControlUltra.Collections
{
    public sealed class StackList<T> : List<T>
    {
        public StackList() : base() { }
        public StackList(IEnumerable<T> collection) : base(collection) { }
        public StackList(int capacity) : base(capacity) { }

        public T? Peek()
        {
            if (Count > 0)
                return base[Count - 1];
            else
                return default;
        }

        public T? Pop()
        {
            if (Count > 0)
            {
                T item = base[Count - 1];
                RemoveAt(Count - 1);
                return item;
            }
            else
                return default;
        }

        public void Push(T item)
            => Add(item);
    }
}
