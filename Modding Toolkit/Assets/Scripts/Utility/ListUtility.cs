using System;
using System.Collections;
using System.Collections.Generic;

public static class ListUtility
{
    public static bool IsValidIndex(this IList a, int index)
    {
        return index > -1 && index < a.Count;
    }
    public static bool IsValidInsert(this IList a, int index)
    {
        return index > -1 && index <= a.Count;
    }
    public static int RemoveRange<T>(this List<T> a, IList collection)
    {
        return a.RemoveAll(x => collection.Contains(x));
    }

    public static void ForEach<T>(this IEnumerable<T> list, Action<T> onItem)
    {
        foreach (T item in list)
            onItem.Invoke(item);
    }
    public static T GetBest<T, T2>(this IEnumerable<T> list, Func<T, T2> getScore, out T2 bestScore) where T2 : IComparable
    {
        (T item, T2 score)? best = null;

        foreach (T item in list)
        {
            T2 _score = getScore.Invoke(item);
            if (!best.HasValue || _score.CompareTo(best.Value.score) > 0)
                best = (item, _score);
        }

        bestScore = best.Value.score;
        return best.Value.item;
    }
    public static T GetBest<T>(this IEnumerable<T> list, Func<T, T, bool> isBetter)
    {
        bool bestAssigned = false;
        T best = default;

        foreach (T item in list)
        {
            if (!bestAssigned || isBetter.Invoke(item, best))
            {
                best = item;
                bestAssigned = true;
            }
        }

        return best;
    }
    public static List<T> Collapse<T>(this IEnumerable<IEnumerable<T>> lists)
    {
        List<T> items = new List<T>();
        foreach (IEnumerable<T> list in lists)
            items.AddRange(list);
        return items;
    }
}