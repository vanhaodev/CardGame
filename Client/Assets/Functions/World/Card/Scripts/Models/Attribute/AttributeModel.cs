using System;
using System.Collections.Generic;
using System.Linq;

namespace World.TheCard
{
    [Serializable]
    public class AttributeModel
    {
        public AttributeType Type;
        public int Value;

        public static Dictionary<AttributeType, int> ToDictionary(List<AttributeModel> attributes)
        {
            return attributes
                .GroupBy(a => a.Type)
                .OrderBy(g => (int)g.Key) // Sort theo thứ tự khai báo enum
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(a => a.Value)
                );
        }
    }
}