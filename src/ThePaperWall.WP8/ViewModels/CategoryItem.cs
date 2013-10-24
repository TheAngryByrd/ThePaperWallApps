using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ThePaperWall.WP8.ViewModels
{
    public class CategoryItem : ReactiveObject, IComparable<CategoryItem>
    {
        public string Id { get; private set; }

        public string Name { get; set; }


        public CategoryItem(string id, string name)
        {
            // TODO: Complete member initialization
            Id = id;
            Name = name;
        }
        public int CompareTo(CategoryItem other)
        {
            return string.Compare(this.Name, other.Name);
        }
    }
}
