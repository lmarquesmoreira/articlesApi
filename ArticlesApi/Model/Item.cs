using System.Collections.Generic;

namespace ArticlesApi.Model
{
    public class Item
    {
        public string id { get; set; }
        public string text { get; set; }
        public string location { get; set; }
        public string fromEndpoint { get; set; }
    }

    public class ItemComparer : EqualityComparer<Item>
    {
        public override bool Equals(Item x, Item y)
        {
            var isEqual = false;

            if (x.id == y.id)
            {
                isEqual = true;
            }
            return isEqual;
        }


        public override int GetHashCode(Item s)
        {
            return base.GetHashCode();
        }
    }
}