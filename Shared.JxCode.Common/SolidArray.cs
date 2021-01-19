using System;
using System.Collections;
using System.Collections.Generic;

namespace JxCode.Common
{
    /// <summary>
    /// 新增溢出项会覆盖旧项的固定大小数组
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SolidArray<T> : IList<T>, ICollection<T>, IEnumerable<T>
    {
        protected T[] array;
        protected int capacity;
        public int Capacity
        {
            get => this.capacity;
        }
        protected int count;
        public int Count
        {
            get => this.count;
        }

        public SolidArray(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            this.capacity = capacity;
            this.array = new T[capacity];
        }

        public SolidArray() : this(8)
        {
        }


        public T this[int index]
        {
            get
            {
                if (index >= count)
                {
                    throw new IndexOutOfRangeException();
                }
                return this.array[index];
            }
            set
            {
                if (index >= count)
                {
                    throw new IndexOutOfRangeException();
                }
                this.array[index] = value;
            }
        }

        public bool IsReadOnly => false;

        public void Add(T item)
        {
            if (this.count < this.capacity)
            {
                this.array[this.count] = item;
                this.count++;
            }
            else
            {
                Array.Copy(this.array, 1, this.array, 0, this.capacity - 1);
                this.array[this.capacity - 1] = item;
            }

        }

        public void Clear()
        {
            Array.Clear(this.array, 0, this.count);
            this.count = 0;
        }

        public bool Contains(T item)
        {
            return this.IndexOf(item) != -1;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Array.Copy(this.array, 0, array, arrayIndex, this.count);
        }

        protected struct Enumerator : IEnumerator<T>
        {
            SolidArray<T> list;
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


            public Enumerator(SolidArray<T> list)
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
                if(this.index + 1 == this.list.count)
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

        public IEnumerator<T> GetEnumerator()
        {
            return new Enumerator(this);
        }

        public int IndexOf(T item)
        {
            return Array.IndexOf(this.array, item, 0, this.count);
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
            if (index >= this.count)
            {
                throw new IndexOutOfRangeException();
            }
            if (index == this.count - 1)
            {
                this.array[this.count - 1] = default(T);
                --this.count;
            }
            else
            {
                Array.Copy(this.array, index + 1, this.array, index, this.count - 1 - index);
                this.array[this.count - 1] = default(T);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.array.GetEnumerator();
        }
    }
}