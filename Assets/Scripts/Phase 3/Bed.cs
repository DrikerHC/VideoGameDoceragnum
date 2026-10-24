using System;
using Doceragnum;

namespace Phase_3
{
    public class Bed : Interactable
    {
        public event Action OnEndDay;
        public override void Interact()
        {
            OnEndDay?.Invoke();
        }


    }
}