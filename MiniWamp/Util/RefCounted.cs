using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DapperWare.Util
{
    public class Disposable : IDisposable
    {
        public static readonly IDisposable Empty = new Disposable(() => { });
        private Action _action;
        private bool _isDisposed;

        public static IDisposable Create(Action action)
        {
            return new Disposable(action);
        }

        private Disposable(Action action)
        {
            this._action = action;
            this._isDisposed = false;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                this._isDisposed = true;
                this._action();
            }
        }
    }

    public class RefCountedDisposable : IDisposable
    {

        private readonly object _gate = new object();
        private int _count = 0;
        private bool _isDisposed;
        private IDisposable _disposable;

        public RefCountedDisposable(IDisposable disposable)
        {
            _count = 0;
            _isDisposed = false;
            _disposable = disposable;
        }

        public IDisposable GetDisposable()
        {
            lock (_gate)
            {
                if (_disposable == null)
                    return Disposable.Empty;
                else
                {
                    _count++;
                    return new InnerDisposable(this);
                }
            }
        }

        public void Release()
        {
            var disposable = default(IDisposable);
            lock (_gate)
            {
                if (_disposable != null) {
                    _count--;

                    System.Diagnostics.Debug.Assert(_count >= 0);
                    if (_isDisposed)
                    {
                        if (_count == 0)
                        {
                            disposable = this._disposable;
                            _disposable = null;
                        }
                    }
                }
            }

            if (disposable != null)
                disposable.Dispose();
        }



        public void Dispose()
        {
            var disposable = default(IDisposable);
            lock (_gate)
            {
                if (_disposable != null)
                {
                    if (_isDisposed)
                    {
                        _isDisposed = true;
                        if (_count == 0)
                        {
                            disposable = this._disposable;
                            _disposable = null;
                        }
                    }
                }
            }

            if (disposable != null)
                disposable.Dispose();
        }

        sealed class InnerDisposable : IDisposable
        {
            private RefCountedDisposable _parent;

            public InnerDisposable(RefCountedDisposable parent)
            {
                this._parent = parent;
            }

            public void Dispose()
            {
                var parent = Interlocked.Exchange(ref _parent, null);
                if (parent != null)
                    parent.Release();
            }
        }


        public bool IsDisposed
        {
            get { return this._disposable == null; }
        }
    }
}
