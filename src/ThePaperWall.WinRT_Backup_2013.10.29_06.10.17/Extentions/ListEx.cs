using System;
using System.Collections.Generic;

public static class ListEx
    {
        public static void ForEach<T>(this List<T> This, Action<T> action)
        {
            foreach(var item in This)
            {
                action(item);
            }
        }
    }

