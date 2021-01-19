using System;
using System.Collections;
using System.Collections.Generic;

namespace JxCode.Common
{
    /// <summary>
    /// 逻辑上新增溢出项会覆盖旧项的固定大小列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SolidList<T> : IList<T>, ICollection<T>, IEnumerable<T>
    {
        protected T[] array;

        protected int capacity;
        public int Capacity
        {
            get => this.capacity;
        }

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

        protected int version = 0;

        public SolidList(int size)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            this.size = size;

            this.capacity = 8;
            this.array = new T[this.capacity];
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= this.Count)
                {
                    throw new IndexOutOfRangeException();
                }
                return this.array[this.head + index];
            }
            set
            {
                if (index < 0 || index >= this.Count)
                {
                    throw new IndexOutOfRangeException();
                }
                this.array[this.head + index] = value;
            }
        }

        public bool IsReadOnly => false;


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

        public void Add(T item)
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

            ++this.version;
        }

        public void Clear()
        {
            Array.Clear(this.array, this.head, this.Count);
            ++this.version;
        }

        public bool Contains(T item)
        {
            return this.IndexOf(item) != -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(this.array, this.head, array, arrayIndex, this.Count);
        }

        public int IndexOf(T item)
        {
            return Array.IndexOf(this.array, item, this.head, this.Count);
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            int index = this.IndexOf(item);
            if (index == -1)
            {
                return false;
            }
            this.RemoveAt(index);
            return true;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= this.Count)
            {
                throw new IndexOutOfRangeException();
            }
            if (index == this.Count - 1)
            {
                this.array[this.tail] = default(T);
                --this.tail;
            }
            else
            {
                Array.Copy(this.array, this.head + index + 1, this.array, index, this.Count - 1 - index);
                this.array[this.Count - 1] = default(T);
            }
            ++this.version;
        }


        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }


        protected struct Enumerator : IEnumerator<T>
        {
            SolidList<T> list;
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


            public Enumerator(SolidList<T> list)
            {
                this.list = list;
                this.current = default(T);
                this.index = -1;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (this.index + 1 == this.list.Count)
                {
                    return false;
                }
                ++this.index;
                this.current = this.list[this.index];
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