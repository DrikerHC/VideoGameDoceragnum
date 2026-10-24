using Doceragnum;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BotonJuegoController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI NombreVideoJuego;
    [SerializeField] private Button BotonJuego;
    [SerializeField] public VideojuegoSO DataJuego;
    [SerializeField] private Button PlayJuego;
    private void Awake()
    {
        PlayJuego.onClick.AddListener(EvPlay);
    }

    public void init()
    {
        NombreVideoJuego.text = DataJuego.nombreDelVideojuego;
        BotonJuego.GetComponent<Image>().sprite = DataJuego.SpriteVideojuego;
        UpdateButtonText();
    }

    private void UpdateButtonText()
    {
        if (DataJuego.isBought)
        {
            PlayJuego.GetComponentInChildren(typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>().text = "Jugar";
        }
        else
        {
            PlayJuego.GetComponentInChildren(typeof(TextMeshProUGUI)).GetComponent<TextMeshProUGUI>().text = "Comprar";   
        }
    }

    private void OnDestroy()
    {
        PlayJuego.onClick.RemoveListener(EvPlay);
    }

    public void EvPlay()
    {
        print("clicked");
        InGameDataManager.Instance.EvPlay(DataJuego);
        UpdateButtonText();
    }
}