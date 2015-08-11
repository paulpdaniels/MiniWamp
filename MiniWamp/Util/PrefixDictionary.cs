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
        }

        public override void Add(string key, string value)
        {
            base.Add(key, value);
            OnPrefixChanged(this, new KeyValuePair<string, string>(key, value));
        }

        /// <summary>
        /// Used to raise when a new prefix has been added to this dictionary
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="value"></param>
        protected virtual void OnPrefixChanged(object sender, KeyValuePair<string, string> value)
        {
            if (this.PrefixChanged != null)
            {
                this.PrefixChanged(sender,
                    new NotifyPrefixesChangedEventArgs(NotifyCollectionChangedAction.Add, value));
            }
        }

        public event EventHandler<NotifyPrefixesChangedEventArgs> PrefixChanged;

        /// <summary>
        /// Maps a given URI -> CURIE if a prefix mapping has been declared
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Maps a given URI -> CURIE if a prefix mapping has been declared 
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
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
