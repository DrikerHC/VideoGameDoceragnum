using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Doceragnum
{
    public class MenuRequisitsManager : MonoBehaviour
    {
        public List<BaseRequisite> Requisites;
        public Button BotonLast;
        public Button BotonNext;
        public int _ActualPage;
        [SerializeField] private TextMeshProUGUI _TituloL;
        [SerializeField] private TextMeshProUGUI _TituloR;
        [SerializeField] private TextMeshProUGUI _DescripcionL;
        [SerializeField] private TextMeshProUGUI _DescripcionR;
        void Start()
        {
            _ActualPage = 0;
            SetPages();
        }

        public void OnEnable()
        {
            BotonLast.onClick.AddListener(LastPage);
            BotonNext.onClick.AddListener(NextPage);
        }
        public void OnDisable()
        {
            BotonLast.onClick.RemoveListener(LastPage);
            BotonNext.onClick.RemoveListener(NextPage);
        }
        private void LastPage()
        {
            if (_ActualPage <= 0)
                return;

            _ActualPage -= 2;
            SetPages();
        }

        private void NextPage()
        {
            if (_ActualPage >= Requisites.Count - 2)
                return;

            _ActualPage += 2;
            SetPages();
        }

        private void SetPages()
        {
            _TituloL.text = Requisites[_ActualPage].GetName();
            _DescripcionL.text = Requisites[_ActualPage].GetDesc();
            _TituloR.text = Requisites[_ActualPage + 1].GetName();
            _DescripcionR.text = Requisites[_ActualPage + 1].GetDesc();
        }

    }
}


