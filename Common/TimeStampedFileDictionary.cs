using System;
using System.Collections.Generic;

using System.Text;

namespace Common
{
    public class TimeStampedFileDictionary<T> where T : class
    {
        Dictionary<string, Tuple<DateTime, T>> dictionary = new Dictionary<string,Tuple<DateTime,T>>();

        public void AddElement(string path, T element)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(path);
            DateTime lastAccessTime = fi.LastWriteTimeUtc;

            if (dictionary.ContainsKey(path))
                dictionary.Remove(path);

            dictionary.Add(path, new Tuple<DateTime,T>(lastAccessTime, element));
        }

        public T GetElement(string path)
        {
            System.IO.FileInfo fi = new System.IO.FileInfo(path);
            DateTime lastAccessTime = fi.LastWriteTimeUtc;

            T ret = null;

            Tuple<DateTime, T> t;
            if (dictionary.TryGetValue(path, out t))
            {
                if (t.Item1.CompareTo(lastAccessTime) == 0)
                    ret = t.Item2;
            }

            return ret;
        }

        public void RemoveIfExists(T element)
        {
            List<string> keysToRemove = new List<string>();
            Dictionary<string, Tuple<DateTime, T>>.Enumerator e = dictionary.GetEnumerator();
            while(e.MoveNext())
                if (e.Current.Value.Item2 == element)
                    keysToRemove.Add(e.Current.Key);
            foreach (string key in keysToRemove)
                dictionary.Remove(key);
        }

        public IEnumerable<T> GetValues()
        {
            foreach (Tuple<DateTime, T> t in dictionary.Values)
                yield return t.Item2;
        }

        public List<string> GetKeys()
        {
            return new List<string>(dictionary.Keys);
        }
    }
}
