using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperWare.Util
{
    public class Map<T1, T2>
    {
        private Dictionary<T1, T2> _forward = new Dictionary<T1,T2>();
        private Dictionary<T2, T1> _backward = new Dictionary<T2,T1>();

        public Map()
        {
            Forward = new ReadOnlyDictionary<T1, T2>(_forward);
            Backward = new ReadOnlyDictionary<T2, T1>(_backward);
        }


        public virtual void Add(T1 t1, T2 t2)
        {
            _forward.Add(t1, t2);
            _backward.Add(t2, t1);
        }

        public virtual bool RemoveForward(T1 t1)
        {
            T2 t2;
            if (_forward.TryGetValue(t1, out t2))
            {
                return _forward.Remove(t1) && _backward.Remove(t2);
            }

            return false;
        }

        public virtual bool RemoveBackward(T2 t2)
        {
            T1 t1;
            if (_backward.TryGetValue(t2, out t1))
            {
                return _backward.Remove(t2) && _forward.Remove(t1);
            }

            return false;
        }

        public int Count
        {
            get
            {
                return this._forward.Count;
            }
        }

        public IDictionary<T1, T2> Forward { get; private set; }
        public IDictionary<T2, T1> Backward { get; private set; }

    }
}
