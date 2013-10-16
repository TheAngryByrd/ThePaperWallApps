using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThePaperWall.Core.Framework
{
    public class SortableObservableCollection<T> : ObservableCollection<T> where T : IComparable<T>
    {
        public SortableObservableCollection()
        {
        }

        public SortableObservableCollection(IEnumerable<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");
            foreach (T item in list)
                Add(item);
        }

        public new void Add(T item)
        {
            base.Add(item);
            MoveItemIntoSortedList(item);
        }

        private void MoveItemIntoSortedList(T item)
        {
            MoveItem(Count - 1, GetBinarySearchIndex(item, 0, Count - 1));
        }

        private int GetBinarySearchIndex(T item, int low, int high)
        {
            if (high < low)
                return low;
            int mid = low + ((high - low) / 2);
            if (base[mid].CompareTo(item) > 0)
                return GetBinarySearchIndex(item, low, mid - 1);
            if (base[mid].CompareTo(item) < 0)
                return GetBinarySearchIndex(item, mid + 1, high);
            return mid;
        }
    }

}
