using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace DapperWare.Util
{
    public class NotifyPrefixesChangedEventArgs : EventArgs
    {
        public NotifyCollectionChangedAction Action { get; private set; }

        public KeyValuePair<string, string> Prefix { get; private set; }

        public NotifyPrefixesChangedEventArgs(NotifyCollectionChangedAction action, KeyValuePair<string, string> prefix)
        {
            Action = action;
            Prefix = prefix;
        }
    }
}
