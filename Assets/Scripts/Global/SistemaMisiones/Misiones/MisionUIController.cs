using Doceragnum;
using TMPro;
using UnityEngine;

public class MisionUIController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private Mision MisionReference;
    [SerializeField] private TextMeshProUGUI _NombreMisionUI;
    [SerializeField] private TextMeshProUGUI _PasoActualMisionUI;
    
    public void init()
    {
        _NombreMisionUI.text = MisionReference.NameCharacterMision + " - " + MisionReference.MisionName;
        if(MisionReference.GetLastStep()>0)
            _PasoActualMisionUI.text = MisionReference.Pasos[MisionReference.GetLastStep()].NamePaso;
        Debug.Log("Nombre paso en teoria: "+MisionReference.Pasos[MisionReference.GetLastStep()].NamePaso);
    }

    public void EstablecerReferencia( Mision mision)
    {
        MisionReference = mision;
    }
}
