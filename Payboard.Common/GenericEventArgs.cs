using System;

namespace Payboard.Integrations
{
    public class GenericEventArgs<T> : EventArgs
    {
        public GenericEventArgs(T item)
        {
            Item = item;
        }

        public T Item { get; private set; }
    }

    public class GenericEventArgs<T1, T2> : EventArgs
    {
        public GenericEventArgs(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public T1 Item1 { get; private set; }
        public T2 Item2 { get; private set; }
    }
}