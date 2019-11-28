using System;
using System.Collections.ObjectModel;

namespace HSNXT.QuickAndDirtyGui
{
    public class DisposableCollection<T> : Collection<T>, IDisposable where T : IDisposable
    {
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            foreach (var item in this) item.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
        }


        protected override void ClearItems()
        {
            Dispose();
            base.ClearItems();
        }
    }
}