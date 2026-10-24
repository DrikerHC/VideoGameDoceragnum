using System;
using Doceragnum;

namespace Phase_3
{
    public class ComputerF3 : Interactable
    {
        public event Action OnComputerF3Interact;
        public event Action OnComputerF3Disconnect;
        public override void Interact()
        {
            print("entered ComputerF3");
            OnComputerF3Interact?.Invoke();
        }

        public void Disconnect()
        {
            print("disconnected ComputerF3");
            OnComputerF3Disconnect?.Invoke();
        }


    }
}