using System;
using System.Collections;
using System.Collections.Generic;

namespace JxCode.Common
{
    public class SolidStack<T> : IEnumerable
    {
        private T[] data;
        private int count = 0;
        private int capicity = 0;
        public SolidStack(int size)
        {
            if(size <= 0)
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
            if(this.count == 0)
            {
                throw new IndexOutOfRangeException();
            }
            T tmp = this.data[this.count - 1];
            this.data[this.count - 1] = default(T);
            --this.count;
            return tmp;
        }
        public int Count => this.count;
        public void CopyTo(Array array, int index) => data.CopyTo(array, index);
        IEnumerator IEnumerable.GetEnumerator() => data.GetEnumerator();
    }
}
