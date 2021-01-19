using System;
using System.Collections;
using System.Collections.Generic;

namespace JxCode.Common
{
    public class SolidStack<T> : IEnumerable<T>, IEnumerable
    {
        protected T[] array;

        protected int capacity = 0;
        public int Capacity => capacity;

        protected int size;
        public int Size => size;

        protected int head = 0;
        protected int tail = -1;

        public int Count
        {
            get
            {
                if (this.head > this.tail)
                {
                    return 0;
                }
                return this.tail - this.head + 1;
            }
        }

        public SolidStack(int size)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            this.size = size;

            this.capacity = 8;
            this.array = new T[this.capacity];
        }

        protected void SetCapacity(int capacity)
        {
            T[] arr = new T[capacity];

            var count = this.Count;

            Array.Copy(this.array, this.head, arr, 0, count);
            this.array = arr;

            this.capacity = capacity;
            this.head = 0;
            this.tail = count - 1;
        }

        public void Push(T item)
        {
            if (this.tail + 1 == this.capacity)
            {
                this.SetCapacity(this.Count * 2);
            }

            ++this.tail;
            this.array[this.tail] = item;

            if (this.Count > this.size)
            {
                this.array[this.head] = default(T);
                ++this.head;
            }

        }

        public T Pop()
        {
            if (this.Count == 0)
            {
                throw new IndexOutOfRangeException();
            }

            T t = this.array[this.tail];

            this.array[this.tail] = default(T);
            --this.tail;

            return t;
        }

        public T Peek(int level)
        {
            if (this.Count == 0)
            {
                throw new IndexOutOfRangeException();
            }
            int pos = this.Count - level - 1;
            if (pos < 0 || pos >= this.Count)
            {
                throw new IndexOutOfRangeException();
            }
            return this.array[this.head + pos];
        }

        public bool TryPeek(int level, ref T target)
        {
            if (this.Count == 0)
            {
                return false;
            }
            int pos = this.Count - level - 1;
            if (pos < 0 || pos >= this.Count)
            {
                return false;
            }
            target = this.array[pos];
            return true;
        }
        public T PeekTop()
        {
            return this.Peek(0);
        }
        public bool TryPeekTop(ref T obj)
        {
            return this.TryPeek(0, ref obj);
        }
        public void Clear()
        {
            Array.Clear(this.array, this.head, this.Count);
        }

         IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        protected struct Enumerator : IEnumerator<T>
        {
            SolidStack<T> stack;
            T current;
            int index;

            public T Current
            {
                get
                {
                    return this.current;
                }
            }
            object IEnumerator.Current
            {
                get
                {
                    return this.current;
                }
            }


            public Enumerator(SolidStack<T> stack)
            {
                this.stack = stack;
                this.current = default(T);
                this.index = -1;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (this.index + 1 == this.stack.Count)
                {
                    return false;
                }
                ++this.index;
                this.current = this.stack.Peek(this.index);
                return true;
            }

            public void Reset()
            {
                this.index = -1;
                this.current = default(T);
            }
        }
    }
}
