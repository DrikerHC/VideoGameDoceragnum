using System.Collections.Generic;

namespace Assets.Scripts.Utils
{
    public static class DictionaryExtensions
    {
        public static bool TryGetKey<TKey, TValue>(this Dictionary<TKey, TValue> dict, TValue value, out TKey key)
        {
            foreach (KeyValuePair<TKey, TValue> pair in dict)
            {
                if (EqualityComparer<TValue>.Default.Equals(pair.Value, value))
                {
                    key = pair.Key;
                    return true;
                }
            }
            key = default;
            return false;
        }

        /// <summary>
        /// Retorna la primera clau associada al valor especificat.
        /// </summary>
        /// <exception cref="KeyNotFoundException">Es llança si el valor no existeix al diccionari.</exception>
        public static TKey GetKey<TKey, TValue>(this Dictionary<TKey, TValue> dict, TValue value)
        {
            foreach (KeyValuePair<TKey, TValue> pair in dict)
            {
                if (EqualityComparer<TValue>.Default.Equals(pair.Value, value))
                {
                    return pair.Key;
                }
            }
            throw new KeyNotFoundException($"El valor especificat '{value}' no s'ha trobat en cap clau del diccionari.");
        }
    }
}