using System;
using System.Collections;
using System.Collections.Generic;

namespace JxCode.Common
{
    public class SolidStack<T> : IEnumerable
    {
        private T[] data;
        private int count = 0;
        public int Count
        {
            get
            {
                return this.count;
            }
        }

        private int capicity = 0;
        public int Capicity
        {
            get
            {
                return this.capicity;
            }
        }

        public SolidStack(int size)
        {
            if (size <= 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            this.capicity = size;
            this.data = new T[size];
        }

        public void Push(T t)
        {
            if (this.count == this.capicity)
            {
                Array.Copy(this.data, 1, this.data, 0, this.capicity - 1);
                this.data[this.capicity - 1] = t;
            }
            else
            {
                this.data[this.count] = t;
                ++this.count;
            }
        }
        public T Pop()
        {
            if (this.count == 0)
            {
                throw new IndexOutOfRangeException();
            }
            T tmp = this.data[this.count - 1];
            this.data[this.count - 1] = default(T);
            --this.count;
            return tmp;
        }
        public T Peek(int level)
        {
            if (this.count == 0)
            {
                throw new IndexOutOfRangeException();
            }
            int pos = this.count - level - 1;
            if (pos < 0 || pos >= this.count)
            {
                throw new IndexOutOfRangeException();
            }
            return this.data[pos];
        }
        public bool TryPeek(int level, ref T target)
        {
            if (this.count == 0)
            {
                return false;
            }
            int pos = this.count - level - 1;
            if (pos < 0 || pos >= this.count)
            {
                return false;
            }
            target = this.data[pos];
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
            Array.Clear(this.data, 0, this.count);
            this.count = 0;
        }

        IEnumerator IEnumerable.GetEnumerator() => data.GetEnumerator();

    }
}
