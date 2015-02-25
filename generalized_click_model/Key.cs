using System;
using System.Linq;
namespace Clik
{
    class Key
    {
        public int QueryId;
        public int[] URLs;

        public Key()
        {
        }

        public Key(Query q)
        {
            QueryId = q.QueryId;
            URLs = (int[])q.URLs.Clone();
        }

        bool EqualURLs(int[] other)
        {
            for (int i = 0; i < Constants.Ranks; i++)
                if (URLs[i] != other[i])
                    return false;
            return true;
        }

        public bool Equals(Key other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.QueryId == QueryId && EqualURLs(other.URLs);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Key)) return false;
            return Equals((Key) obj);
        }

        int URLsHashCode()
        {
            if(URLs == null)
                return 0;
            else
            {
                unchecked
                {
                    int res = 1234567;
                    for (int i = 0; i < URLs.Length; i++)
                        res = (res*397) ^ URLs[i];
                    return res;
                }
            }
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (QueryId*397) ^ URLsHashCode();
            }
        }
    }
}