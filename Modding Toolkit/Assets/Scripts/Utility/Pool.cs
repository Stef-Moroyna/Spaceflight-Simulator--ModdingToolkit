using System;
using System.Collections.Generic;

public class Pool<T>
{
    Func<T> createNew;
    Action<T> onReset;
    public List<T> Items { get; }
    public int Index { get; private set; }

    public Pool(Func<T> createNew, Action<T> onReset)
    {
        this.createNew = createNew;
        this.onReset = onReset;
        Items = new List<T>();
    }

    // Gets a item from the pool
    public T GetItem()
    {
        if (Index == Items.Count)
            Items.Add(createNew.Invoke());

        Index++;
        return Items[Index - 1];
    }

    // Resets all items in the pool
    public void Reset()
    {
        foreach (T a in Items)
            onReset.Invoke(a);

        ResetIndex();
    }
    public void ResetIndex()
    {
        Index = 0;
    }
}