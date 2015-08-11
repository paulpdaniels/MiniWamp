using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperWare.Util
{
    public class PrefixDictionary : Map<string, string>
    {
        public PrefixDictionary()
        {
            //this._prefixes = new Dictionary<string, string>();
        }


        public override void Add(string key, string value)
        {
            base.Add(key, value);
            OnPrefixChanged(this, new KeyValuePair<string, string>(key, value));
        }

        protected virtual void OnPrefixChanged(object sender, KeyValuePair<string, string> value)
        {
            if (this.PrefixChanged != null)
            {
                this.PrefixChanged(sender,
                    new NotifyPrefixesChangedEventArgs(NotifyCollectionChangedAction.Add, value));
            }
        }

        public event EventHandler<NotifyPrefixesChangedEventArgs> PrefixChanged;

        public string Shrink(string topic)
        {

            if (this.Count > 0)
            {
                for (var i = topic.Length; i > 0; --i)
                {
                    var sub = topic.Substring(0, i);
                    if (this.Backward.ContainsKey(sub))
                    {
                        return this.Backward[sub] + ":" + topic.Substring(i);
                    }
                }
            }

            return topic;
        }

        public string Unshrink(string topic)
        {
            if (this.Count > 0)
            {
                var semi = topic.IndexOf(':');
                var sub = topic.Substring(0, semi);

                if (this.Forward.ContainsKey(sub))
                {
                    return this.Forward[sub] + topic.Substring(semi + 1);
                }
            }

            return topic;
        }
    }
}
