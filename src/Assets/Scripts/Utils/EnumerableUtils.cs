using System;
using System.Collections.Generic;

#nullable enable
namespace BeamNG.RemoteControlUltra.Utils
{
    public static class EnumerableUtils
    {
        public static T? MinItem<T>(this IEnumerable<T> enumerable, Func<T, float> selector)
        {
            T? minItem = default;
            float minValue = float.MaxValue;

            foreach (T item in enumerable)
            {
                float val = selector(item);
                if (val < minValue)
                {
                    minItem = item;
                    minValue = val;
                }
            }

            return minItem;
        }
    }
}
