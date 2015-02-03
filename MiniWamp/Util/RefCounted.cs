using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperWare.Util
{
    public abstract class RefCounted
    {
        private readonly object _gate = new object();
        private int _count = 0;
        private bool _isDisposed;

        public RefCounted()
        {
            _count = 0;
            _isDisposed = false;
        }

        public int Count
        {
            get { return this._count; }
        }

        public void Increment()
        {
            lock (_gate) { _count++; }
        }

        public void Release()
        {
            lock (_gate) { _count--; if (_count < 1) Dispose(); }
        }

        public void Dispose()
        {
            if (!_isDisposed)
                DoDispose();
            _isDisposed = true;
        }

        protected abstract void DoDispose();

        public bool IsDisposed
        {
            get { return this._isDisposed; }
        }
    }
}
