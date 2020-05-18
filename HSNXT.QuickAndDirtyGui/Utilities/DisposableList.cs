using System;
using System.Collections.ObjectModel;

namespace HSNXT.QuickAndDirtyGui
{
    public sealed class DisposableList<T> : Collection<T>, IDisposable where T : IDisposable
    {
        public void Dispose()
        {
            foreach (var item in this) item.Dispose();
        }

        protected override void RemoveItem(int index)
        {
            this[index].Dispose();
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, T item)
        {
            var existingItem = this[index];
            if (!ReferenceEquals(existingItem, item)) existingItem.Dispose();
            base.SetItem(index, item);
        }

        protected override void ClearItems()
        {
            Dispose();
            base.ClearItems();
        }
    }
}