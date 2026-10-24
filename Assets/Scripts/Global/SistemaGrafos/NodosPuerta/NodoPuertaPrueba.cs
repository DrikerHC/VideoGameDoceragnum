using System.Collections.Generic;
using UnityEngine;

namespace Doceragnum
{
    [CreateAssetMenu(fileName = "NuevaPuerta", menuName = "Historia/Nodos/Puerta")]
    public class PuertaNode : BaseNode
    {
        [Header("Configuración de Salto")]
        public BaseNode nodoExito;
        public BaseNode nodoFallo;

        [Header("Requisitos del NPC")]
        public List<Requisito> requisitos;

        public override BaseNode EjecutarNodo(CharacterData npcActual)
        {
            // Si no hi ha requisits pasem al node exit directament
            if (requisitos == null || requisitos.Count == 0) 
                return nodoExito;

            foreach (var req in requisitos)
            {
                // En aquesta funcio anem comprovant els requisits
                float valorEnNPC = npcActual.GetVariable(req.nombreVariable);

                // Fem la comprobacio 
                if (!CumpleCondicion(valorEnNPC, req))
                {
                    // Si alguna de les comprobacions no es compleix retornem "nodoFallo"
                    return nodoFallo;
                }
            }

            // Si arribem aqui es que totos el requisits han estat acomplits y retornem "nodoExito"
            return nodoExito;
        }

        private bool CumpleCondicion(float valorActual, Requisito req)
        {
            switch (req.comparacion)
            {
                case Requisito.TipoComparacion.MayorOIgual:
                    return valorActual >= req.valorNecesario;
                case Requisito.TipoComparacion.MenorOIgual:
                    return valorActual <= req.valorNecesario;
                case Requisito.TipoComparacion.Igual:
                    return valorActual == req.valorNecesario;
                default:
                    return false;
            }
        }
    }
}