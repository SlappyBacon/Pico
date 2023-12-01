using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pico.MiscTypes
{
    /// <summary>
    /// Queue of objects.
    /// </summary>
    /// <typeparam name="T">Type.</typeparam>
    class ItemQueue<T>
    {
        object _lock = new object();
        T[] _items;
        T[] Items { get { return _items; } }

        int _count = 0;
        int _maxCount;
        public int Count { get { return _count; } }
        public int MaxCount { get { return _maxCount; } }
        public bool IsEmpty { get { return Count < 1; } }
        
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="maxCount">Maximum queue size.</param>
        public ItemQueue(int maxCount = 100)
        {
            if (maxCount <= 0) maxCount = 100;
            _maxCount = maxCount;
            _items = new T[MaxCount];
        }
        /// <summary>
        /// Adds object to back of queue.
        /// </summary>
        /// <param name="item">Object to add.</param>
        /// <returns></returns>
        public bool Add(T item)
        {
            lock (_lock)
            {
                if (Count >= MaxCount) return false;
                
                Items[Count] = item;
                
                _count++;
                
                return true;
            }
        }

        /// <summary>
        /// Pops object from front of queue.
        /// </summary>
        /// <param name="item">Popped object.</param>
        /// <returns></returns>
        public bool Pop(out T item)
        {
            lock (_lock)
            {
                if (IsEmpty)
                {
                    item = default(T);
                    return false;
                }

                item = Items[0];

                Shift(1);    //Decrements Count

                return true;
            }
        }

        /// <summary>
        /// Pops objects from front of queue.
        /// </summary>
        /// <param name="amount">Amount of objects to pop.</param>
        /// <param name="items">Popped objects.</param>
        /// <returns></returns>
        public bool Pop(int amount, out T[] items)
        {
            if (amount < 1)
            {
                items = default(T[]);
                return false;
            }

            lock (_lock)
            {
                if (amount > Count) amount = Count;
                
                if (IsEmpty)
                {
                    items = default(T[]);
                    return false;
                }

                items = new T[amount];
                for (int i = 0; i < amount; i++)
                {
                    items[i] = Items[i];
                }

                Shift(amount);    //Decrements Count

                return true;
            }
        }

        /// <summary>
        /// Pops all objects from queue.
        /// </summary>
        /// <param name="items">Popped objects.</param>
        /// <returns></returns>
        public bool PopAll(out T[] items)
        {
            if (Pop(Count, out items))
            {
                return true;
            }
            else return false;
        }


        void Shift(int amount = 1)
        {
            if (amount < 1) return;
            
            lock (_lock)
            {
                if (IsEmpty) return;
                
                if (amount > Count) amount = Count;
                
                for (int i = 0; i < Count - 1; i++)
                {
                    var newIndex = i + amount;
                    if (newIndex >= Count)
                    {
                        //Out of array range
                        Items[i] = default(T);
                    }
                    else
                    {
                        //In array range
                        Items[i] = Items[newIndex];
                    }
                }
                
                _count -= amount;
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                for (int i = 0; i < Count - 1; i++)
                {
                    Items[i] = default(T);
                }
                _count = 0;
            }
        }
    }
}
