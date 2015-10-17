﻿using DapperWare.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DapperWare
{
    public class WampSubscription<T> : IWampSubscription<T>
    {
        private IWampSession _session;
        private RefCountedDisposable _subscription;
        private CompositeDisposable _subjects;

        public WampSubscription(IWampSession session, string topic)
        {
            this.Topic = topic;
            this._session = session;
            this._subscription = new RefCountedDisposable(Disposable.Create(() => { 
                if (this._session != null)
                    this._session.Unsubscribe(this.Topic);
            }));
            this._subjects = new CompositeDisposable();
        }

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<WampSubscriptionMessageEventArgs<T>> Event;
        

        public void HandleEvent(Newtonsoft.Json.Linq.JToken jToken)
        {
            OnEvent(this, new WampSubscriptionMessageEventArgs<T>(Topic, jToken.ToObject<T>()));
        }

        protected virtual void OnEvent(object sender, WampSubscriptionMessageEventArgs<T> args)
        {
            if (this.Event != null)
            {
                Event(sender, args);
            }
        }

        public string Topic
        {
            get;
            private set;
        }

        public void Dispose()
        {
            this._subjects.Dispose();
            this._subscription.Dispose();
        }

        public IWampSubject<T> CreateSubject()
        {
            var subject = new WampSubject<T>(this, this._subscription.GetDisposable());
            this._subjects.Add(subject);
            return subject;
        }

        IWampSubject IWampSubscription.CreateSubject()
        {
            return CreateSubject();
        }
    }
}



