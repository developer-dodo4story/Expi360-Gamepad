using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class LocalizationData
{
    //public LocalizationItem[] items;
    public List<LocalizationItem> items;

    public LocalizationData()
    {
        this.items = new List<LocalizationItem>();
    }

    public void Sort()
    {
        items.Sort(new StringComparer());
    }
}

public class StringComparer : IComparer<LocalizationItem>
{
    public int Compare(LocalizationItem x, LocalizationItem y)
    {
        string s1 = x.key as string;
        string s2 = y.key as string;
        //negate the return value to get the reverse order
        return String.Compare(s1, s2);

    }
}