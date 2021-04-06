using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace DynamicArrays
{
    public class DynamicArray<T> : IEnumerable<T>
    {
        private T[] _arr;
        private int _len; // length user thinks array is
        private int _capacity; // Actual array size

        public DynamicArray() : this(16)
        {

        }

        public DynamicArray(int capacity)
        {
            if (capacity < 0) throw new ArgumentException("Illegal Capacity: " + capacity);
            this._capacity = capacity;
            _arr = new T[16];
        }

        public int Size()
        {
            return _len;
        }

        public int Capacity()
        {
            return _capacity;
        }

        public bool IsEmpty()
        {
            return Size() == 0;
        }

        public T Get(int index)
        {
            if (index >= _len || index < 0) throw new IndexOutOfRangeException();
            return _arr[index];
        }

        public void Set(int index, T elem)
        {
            if (index >= _len || index < 0) throw new IndexOutOfRangeException();
            _arr[index] = elem;
        }

        public void Clear()
        {
            for (var i = 0; i < _len; i++) _arr[i] = default(T);
            _len = 0;
        }

        public void Add(T elem)
        {
            // Time to resize!
            if (_len + 1 >= _capacity)
            {
                ResizeArray();
            }

            _arr[_len++] = elem;
        }

        private void ResizeArray()
        {
            if (_capacity == 0) _capacity = 1;
            else _capacity *= 2; // double the size
            var newArr = new T[_capacity];
            for (var i = 0; i < _len; i++) newArr[i] = _arr[i];
            _arr = newArr; // arr has extra nulls padded
        }

        //This method can be improved, but for the simplicity it just calls add method 
        public void AddRange(T[] elements)
        {
            for (var i = 0; i < elements.Length; i++)
            {
                Add(elements[i]);
            }
        }

        // Removes an element at the specified index in this array.
        public T RemoveAt(int rmIndex)
        {
            if (rmIndex >= _len || rmIndex < 0) throw new IndexOutOfRangeException();
            var data = _arr[rmIndex];
            var newArr = new T[_len - 1];
            for (int i = 0, j = 0; i < _len; i++, j++)
            {
                if (i == rmIndex) j--; // Skip over rm_index by fixing j temporarily
                else newArr[j] = _arr[i];
            }
            _arr = newArr;
            _capacity = --_len;
            return data;
        }

        public bool Remove(object obj)
        {
            var index = IndexOf(obj);
            if (index == -1) return false;
            RemoveAt(index);
            return true;
        }

        public int IndexOf(object obj)
        {
            for (var i = 0; i < _len; i++)
            {
                if (obj == null)
                {
                    if (_arr[i] == null) return i;
                }
                else
                {
                    if (obj.Equals(_arr[i])) return i;
                }
            }
            return -1;
        }

        public bool Contains(object obj)
        {
            return IndexOf(obj) != -1;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        public IEnumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        public class Enumerator : IEnumerator<T>
        {
            private readonly DynamicArray<T> _dynamicArray;
            private int _index = -1;

            public Enumerator(DynamicArray<T> dynamicArray)
            {
                _dynamicArray = dynamicArray;
            }

            public T Current
            {
                get
                {
                    if (_index < 0) throw new InvalidOperationException("Enumerator Ended");
                    return _dynamicArray._arr[_index];
                }
            }

            object IEnumerator.Current => Current;

            public bool MoveNext()
            {
                _index += 1;
                return _index < _dynamicArray._len;
            }

            public void Reset()
            {
                _index = -1;
            }

            public void Dispose()
            {
                _index = -1;
            }
        }

        public override string ToString()
        {
            if (_len == 0) return "[]";
            else
            {
                var sb = new StringBuilder(_len).Append("[");
                for (var i = 0; i < _len - 1; i++) sb.Append(_arr[i] + ", ");
                return sb.Append(_arr[_len - 1] + "]").ToString();
            }
        }
    }
}
