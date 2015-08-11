using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DapperWare.Util
{
    public class PrefixDictionary : IDictionary<string, string>
    {
        private Dictionary<string, string> _prefixes;

        public PrefixDictionary()
        {
            this._prefixes = new Dictionary<string, string>();
        }


        public void Add(string key, string value)
        {
            this._prefixes.Add(key, value);
            if (this.PrefixChanged != null)
            {
                this.PrefixChanged(this,
                    new NotifyPrefixesChangedEventArgs(NotifyCollectionChangedAction.Add, 
                        new KeyValuePair<string, string>(key, value)));
            }
        }

        public bool ContainsKey(string key)
        {
            return this._prefixes.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(string key)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out string value)
        {
            return this._prefixes.TryGetValue(key, out value);
        }

        public ICollection<string> Values
        {
            get { return this._prefixes.Values; }
        }

        public string this[string key]
        {
            get
            {
                return this._prefixes[key];
            }
            set
            {
                this._prefixes[key] = value;
            }
        }

        public void Add(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return this._prefixes.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException();

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException();

            if (array.Length - arrayIndex < this.Count)
                throw new ArgumentException("This array is not big enough");

            var i = arrayIndex;
            foreach (var item in this)
            {
                array[i++] = item;
            }


        }

        public int Count
        {
            get { return this._prefixes.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return this._prefixes.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public event EventHandler<NotifyPrefixesChangedEventArgs> PrefixChanged;
    }
}
