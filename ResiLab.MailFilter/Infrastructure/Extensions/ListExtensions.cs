using System.Collections.Generic;

namespace ResiLab.MailFilter.Infrastructure.Extensions {
    public static class ListExtensions {
        public static void AddOrIgnore<T>(this List<T> list, T item) {
            if (item == null) {
                return;
            }

            if (list.Contains(item) == false) {
                list.Add(item);
            }
        }
    }
}