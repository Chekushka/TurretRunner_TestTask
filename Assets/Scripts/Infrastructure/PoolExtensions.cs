using System.Collections.Generic;
using UnityEngine.Pool;

namespace Infrastructure
{
    public static class PoolExtensions
    {
        /// <summary>
        /// Creates the specified number of objects in the pool in advance to prevent freezes during gameplay.
        /// </summary>
        public static void Prewarm<T>(this IObjectPool<T> pool, int count) where T : class
        {
            List<T> tempContainer = new List<T>(count);
            
            for (int i = 0; i < count; i++)
            {
                tempContainer.Add(pool.Get());
            }
            
            foreach (var item in tempContainer)
            {
                pool.Release(item);
            }
        }
    }
}