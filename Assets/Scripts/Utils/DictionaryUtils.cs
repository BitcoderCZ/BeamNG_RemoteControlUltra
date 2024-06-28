using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable
namespace BeamNG.RemoteControlUltra.Utils
{
    public static class DictionaryUtils
    {
        public static void Swap<TKey, TValue>(IDictionary<TKey, TValue> dict, TKey a, TKey b)
        {
            bool hasA = dict.TryGetValue(a, out TValue aVal);
            bool hasB = dict.TryGetValue(b, out TValue bVal);

            if (hasA && hasB)
            {
                dict[a] = bVal;
                dict[b] = aVal;
            }
            else if (hasA)
            {
                dict.Remove(a);
                dict.Add(b, aVal);
            }
            else if (hasB)
            {
                dict.Remove(b);
                dict.Add(a, bVal);
            }
        }
    }
}
