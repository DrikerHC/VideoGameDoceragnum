using System;
using Doceragnum;
using UnityEngine;

public class MisionTableManager : MonoBehaviour
{
    [SerializeField] private GameObject MisionPrefab;
    [SerializeField] private Transform content;
    
    private void OnEnable()
    {
        foreach (Mision Mision in InGameDataManager.Instance.misionesDelJuego)
        {
            if (Mision.Pasos[0].IsCumplido && !Mision.IsCumplida)
            {
                GameObject M = Instantiate(MisionPrefab, content);
                Debug.Log("La mision esta acabada: " + Mision.MisionName);
                M.GetComponent<MisionUIController>().EstablecerReferencia(Mision);
                M.GetComponent<MisionUIController>().init();

            }
            else
            {
                Debug.Log("La mision esta acabada: " + Mision.MisionName);
            }
        } ;
    }
    
    private void OnDisable()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }
}
