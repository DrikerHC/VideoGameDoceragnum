using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using TMPro;
namespace Doceragnum
{
    [CreateAssetMenu(fileName = "NuevoDialogo", menuName = "Historia/Nodos/Dialogo")]
    public class DialogueNode : BaseNode
    {
        
        private string textoDialogoFinal;
        
        public LocalizedString miTextoLocalizado;

        public float ActionSpend;
        
        public List<Option> opciones;

        //El dialogeNode te aquesta funcio que s'encarrega basicament de
        //traduir l'ID del localization al text segons l'idioma
        public string MostrarTexto()
        {
            //Ho faig amb un funcio ya nativa, que es diu GetLocalizedString(), y retorna el text
            textoDialogoFinal = miTextoLocalizado.GetLocalizedString();
            Debug.Log("El nodo dice: " + textoDialogoFinal);
            //finalment aquesta funcio retorna al text final
            return textoDialogoFinal;
        }
        
        //Aquesta es la funcio que hereda de la clase BaseNode, en aquest cas no fa res mes que
        //fer la comprobacio de les Remaining Actions, despres es retorna a si mateixa,
        //de aquesta manera el story manager podre procesar-lo
        public override BaseNode EjecutarNodo(CharacterData n)
        {
            if (InGameDataManager.Data.CurrentPhase == 2 && F2Manager.Instance.RemainingActions == 0)
            {
                return InGameDataManager.PlayerData.NodoSinEnergia;
            }
            return this;
        }

        [ContextMenu("New sub")]
        public void CreateOption()
        {
            Option nod = new();
            nod.siguienteNodo = this;
            opciones.Add(nod);
        }
    }

    [System.Serializable]
    public class Option
    {
        //Variables de l'opcio
        public string textoBoton;
        public BaseNode siguienteNodo;
        public LocalizedString miOpcionLocalizada;
        public List<BaseConsecuencia> consecuencias;

        //Mateixa funcion per a traduir l'ID a text
        public string MostrarTexto()
        {
            textoBoton = miOpcionLocalizada.GetLocalizedString();
            Debug.Log("El boton dice: " + miOpcionLocalizada);
            return textoBoton;
        }
        
        //Funcio que es crida quan escolleixes una opcio per aplicar les consequencies de la mateixa
        public void AplicarConsecuencias(CharacterData npcActual, TextMeshProUGUI text)
        {
            //Comprobacio per si algun dels 2 es null
            if (npcActual == null || consecuencias == null)
            {
                Debug.Log("O el npc o las concecuencias son null");
            }

            //Aqui s'iteren totes les consequencies i es van aplicant una a una
            foreach (var consecuencia in consecuencias)
            {
                if (consecuencia != null)
                {
                    Debug.Log("Aplicando consecuencia");
                    text.text += consecuencia.Ejecutar(npcActual);
                }
                   
            }
        }
    }
}