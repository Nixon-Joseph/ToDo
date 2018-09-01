using System;
using System.Collections.Generic;

namespace ToDo
{
    public static class Extensions
    {
        public static IList<T> Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
            return list;
        }

        /// <summary>
        /// Swaps two adjacent stack children
        /// Indeces MUST be adjacent
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="indexA"></param>
        /// <param name="indexB"></param>
        /// <returns></returns>
        public static System.Windows.Controls.StackPanel SwapChildren(this System.Windows.Controls.StackPanel stack, int indexA, int indexB)
        {
            var lesser = Math.Min(indexA, indexB);
            var greater = Math.Max(indexA, indexB);
            var tmp = stack.Children[lesser];
            stack.Children.RemoveAt(lesser);
            stack.Children.Insert(greater, tmp);
            return stack;
        }
    }
}
