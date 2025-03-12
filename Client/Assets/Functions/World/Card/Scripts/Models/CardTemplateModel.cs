using System;
using System.Collections.Generic;

namespace World.Card
{
    [Serializable]
    public class CardTemplateModel
    {
        public ushort Id;
        public List<AttributeModel> Attributes;
    }
}