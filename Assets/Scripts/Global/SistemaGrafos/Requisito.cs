using System;

namespace Doceragnum
{
    // Usamos [Serializable] para que aparezca en el inspector de Unity
    [Serializable] 
    public struct Requisito 
    {
        public string nombreVariable; // El nombre en la "libreta" (ej: "afecto")
        public float valorNecesario;  // El número que pedimos (ej: 50)
        
        // Opcional: Podrías añadir un enum para comparar (Mayor, Menor, Igual)
        public enum TipoComparacion { MayorOIgual, MenorOIgual, Igual }
        public TipoComparacion comparacion;
    }
}