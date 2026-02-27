using UnityEngine;

namespace AbstractPixel.Utility
{
    public interface ICopyable<T> where T : class
    {
        public void CopyReferencesFrom(T source);

    }
}
