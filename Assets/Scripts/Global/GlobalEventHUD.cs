using System;

namespace Doceragnum
{
    public class GlobalEventHUD 
    {
        public static event Action<int, int, StoreItemSO.BonusType> OnValueChanged;
        public static event Action<string> OnMessage;
        public static event Action<int> OnDayChanged;
        public static event Action<int> OnPhaseChanged;
        
        public static void InvokeValueChanged(int previousValue, int newValue, StoreItemSO.BonusType type)
        {
            OnValueChanged?.Invoke(previousValue, newValue, type);
        }

        public static void InvokeMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
                OnMessage?.Invoke(message);
        }

        public static void InvokeDayChanged(int day)
        {
            OnDayChanged?.Invoke(day);
        }

        public static void InvokePhaseChanged(int phase)
        {
            OnPhaseChanged?.Invoke(phase);
        }
    }
}