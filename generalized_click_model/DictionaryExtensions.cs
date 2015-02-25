using System.Collections.Generic;

namespace Clik
{
    public static class DictionaryExtensions
    {
        public static void Inc<TKey>(this Dictionary<TKey,int> dic, TKey key, int diff = 1)
         {
             if (!dic.ContainsKey(key))
                 dic[key] = 0;
             dic[key]+=diff;
         }

         public static void Inc<TKey>(this Dictionary<TKey, long> dic, TKey key)
         {
             if (!dic.ContainsKey(key))
                 dic[key] = 0;
             dic[key]++;
         }
    }
}