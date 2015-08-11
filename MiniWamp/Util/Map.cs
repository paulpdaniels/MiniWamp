using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperWare.Util
{
    /// <summary>
    /// A simple bidirectional map implementation, taken from this answer
    /// http://stackoverflow.com/a/10966684/2521865
    /// 
    /// The two indexes are implemented with ReadOnlyDictionaries as changing the value of one will corrupt the value of the other.
    /// Therefore users should always make sure to remove keys or use `Replace` in order to change the values
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class Map<T1, T2>
    {
        private Dictionary<T1, T2> _forward = new Dictionary<T1,T2>();
        private Dictionary<T2, T1> _backward = new Dictionary<T2,T1>();

        public Map()
        {
            Forward = new ReadOnlyDictionary<T1, T2>(_forward);
            Backward = new ReadOnlyDictionary<T2, T1>(_backward);
        }

        /// <summary>
        /// Adds a new value and key to both indices
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        public virtual void Add(T1 t1, T2 t2)
        {
            _forward.Add(t1, t2);
            _backward.Add(t2, t1);
        }

        /// <summary>
        /// Replaces the value and key for both indices with a new value
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        public virtual void Replace(T1 t1, T2 t2)
        {
            RemoveForward(t1);
            Add(t1, t2);
        }

        /// <summary>
        /// Removes an item using the forward index as the key
        /// </summary>
        /// <param name="t1"></param>
        /// <returns></returns>
        public virtual bool RemoveForward(T1 t1)
        {
            T2 t2;
            if (_forward.TryGetValue(t1, out t2))
            {
                return _forward.Remove(t1) && _backward.Remove(t2);
            }

            return false;
        }


        /// <summary>
        /// Removes an item, using the backward index as the key
        /// </summary>
        /// <param name="t2"></param>
        /// <returns></returns>
        public virtual bool RemoveBackward(T2 t2)
        {
            T1 t1;
            if (_backward.TryGetValue(t2, out t1))
            {
                return _backward.Remove(t2) && _forward.Remove(t1);
            }

            return false;
        }

        /// <summary>
        /// Gets the number of items present in this map
        /// </summary>
        public int Count
        {
            get
            {
                return this._forward.Count;
            }
        }

        /// <summary>
        /// Gets the forward index set
        /// </summary>
        public IReadOnlyDictionary<T1, T2> Forward
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the reverse index set
        /// </summary>
        public IReadOnlyDictionary<T2, T1> Backward
        {
            get;
            private set;
        }

    }
}
