using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using SceneController;
using Unity.Mathematics.Geometry;
using UnityEngine;
using Utils;

namespace Doceragnum
{
    public class InGameDataManager : Singleton<InGameDataManager>
    {
        [SerializeField] private FaseManager _FaseManager;
        [SerializeField] private PlayerData _PlayerData;
        [SerializeField] private GameData _gameData;
        [SerializeField] private CharacterPool _characterPool;
        [SerializeField] private string _fileName;
        public static CharacterPool CharacterPool;
        public static GameData Data;
        public static FaseManager FaseManager;
        public static PlayerData PlayerData;

        public List<Mision> misionesDelJuego;
        private Dictionary<string, Mision> registroMisiones = new();

        private FileDataHandler _dataHandler;

        public override void Awake()
        {
            base.Awake();
            _dataHandler = new FileDataHandler(Application.persistentDataPath, _fileName);
            Data = _gameData;
            FaseManager = _FaseManager;
            PlayerData = _PlayerData;
            CharacterPool = _characterPool;

            foreach (var m in misionesDelJuego)
            {
                if (!string.IsNullOrEmpty(m.misionID) && !registroMisiones.ContainsKey(m.misionID))
                {
                    registroMisiones.Add(m.misionID, m);
                }
            }

        }

        public void ResetToNewGame()
        {
            ApplyNewGameDefaults();
            _dataHandler.DeleteSave();
        }

        public void LoadGame()
        {
            if (!_dataHandler.TryLoad(out PersistentGameData persistentGameData))
            {
                Debug.LogWarning("No save file found.");
                return;
            }

            ApplyNewGameDefaults();
            ApplyPersistentData(persistentGameData);
        }

        private void ApplyNewGameDefaults()
        {
            var snapshot = PersistentGameData.CreateNewGameSnapshot(
                _gameData, _PlayerData, misionesDelJuego, _gameData.Inventory);
            ApplyPersistentData(snapshot);
        }

        private void ApplyPersistentData(PersistentGameData persistentGameData)
        {
            persistentGameData.ApplyTo(_gameData, _PlayerData, misionesDelJuego, _gameData.Inventory);
            GlobalEventHUD.InvokeDayChanged(_gameData.DayCount);
            GlobalEventHUD.InvokePhaseChanged(_gameData.CurrentPhase);
        }

        public void SaveGame()
        {
            var persistentGameData =
                new PersistentGameData(_gameData, _PlayerData, misionesDelJuego, _gameData.Inventory);
            _dataHandler.Save(persistentGameData);
        }


        public void EvPlay(VideojuegoSO videojuego)
        {
            if (videojuego.isBought)
                Play(videojuego);
            else
                BuyGame(videojuego);
        }

        private void BuyGame(VideojuegoSO videojuego)
        {
            if (!TrySpendMoney(videojuego.PrecioJuego))
                return;

            videojuego.isBought = true;
        }

        public void BuyItem(StoreItemSO item)
        {
            if (TryGetPurchaseBlockReason(item, out string blockReason))
            {
                GlobalEventHUD.InvokeMessage(blockReason);
                return;
            }

            if (!TrySpendMoney(item.ItemPrice))
                return;

            CompletePurchase(item);
            ApplyItemBonus(item);
        }

        public void PayBill(BillSO bill)
        {
            if (!CanPayBill(bill))
            {
                GlobalEventHUD.InvokeMessage("No tienes facturas pendientes por pagar.");
                return;
            }

            int amount = GetBillPaymentAmount(bill);
            if (!TrySpendMoney(amount))
                return;

            bill.DayLastPayment = Data.DayCount;
        }

        public static bool IsBillDueOnDay(BillSO bill, int day)
        {
            if (day < bill.DayStartPayment)
                return false;

            return (day - bill.DayStartPayment) % bill.Frequency == 0;
        }

        public static bool CanPayBill(BillSO bill)
        {
            if (bill == null)
                return false;

            return GetBillOutstandingDebt(bill, Data.DayCount) > 0;
        }

        public static List<BillSO> GetPayableUnpaidBills()
        {
            var payableBills = new List<BillSO>();

            if (Data?.Inventory?._bills == null)
                return payableBills;

            foreach (var bill in Data.Inventory._bills)
            {
                if (bill != null && CanPayBill(bill))
                    payableBills.Add(bill);
            }

            return payableBills;
        }

        public static int GetBillPaymentAmount(BillSO bill)
        {
            return GetBillOutstandingDebt(bill, Data.DayCount);
        }

        public static bool CanAffordOutstandingBills()
        {
            return PlayerData.DineroActual >= GetOutstandingBillDebt();
        }

        public static int GetOutstandingBillDebt()
        {
            int dayCount = Data.DayCount;
            int total = 0;

            if (Data?.Inventory?._bills == null)
                return 0;

            foreach (var bill in Data.Inventory._bills)
            {
                if (bill == null)
                    continue;

                total += GetBillOutstandingDebt(bill, dayCount);
            }

            return total;
        }

        private static int GetBillOutstandingDebt(BillSO bill, int dayCount)
        {
            int debt = 0;

            for (int day = bill.DayStartPayment; day <= dayCount; day++)
            {
                if (!IsBillDueOnDay(bill, day))
                    continue;

                if (day <= bill.DayLastPayment)
                    continue;

                debt += Mathf.RoundToInt(bill.Price);
            }

            return debt;
        }

        private bool TryGetPurchaseBlockReason(StoreItemSO item, out string reason)
        {
            if (item.isBuyOnce)
            {
                if (item.isBought)
                {
                    reason = "Ya has comprado este articulo.";
                    return true;
                }

                reason = null;
                return false;
            }

            if (item.QuantityPerFrequency <= item.QuantityConsumed)
            {
                reason = "Has alcanzado el limite de compras.";
                return true;
            }

            if (item.LastUse + item.Frequency < Data.DayCount)
            {
                reason = "Este articulo no esta disponible ahora.";
                return true;
            }

            reason = null;
            return false;
        }

        private void CompletePurchase(StoreItemSO item)
        {
            if (item.isBuyOnce)
            {
                item.isBought = true;
                return;
            }

            item.LastUse = Data.DayCount;
            item.QuantityConsumed++;
        }

        private bool TrySpendMoney(int amount)
        {
            if (PlayerData.DineroActual < amount)
            {
                GlobalEventHUD.InvokeMessage("No tienes suficiente dinero.");
                return false;
            }

            int prev = PlayerData.DineroActual;
            PlayerData.DineroActual -= amount;
            GlobalEventHUD.InvokeValueChanged(prev, PlayerData.DineroActual, StoreItemSO.BonusType.MONEY);
            return true;
        }

        private void ApplyItemBonus(StoreItemSO item)
        {
            switch (item.type)
            {
                case StoreItemSO.BonusType.HAPPINESS:
                    ApplyStatChange(
                        ref PlayerData.felicidadActual,
                        item.InitialBonus,
                        0,
                        PlayerData.felicidadMax,
                        StoreItemSO.BonusType.HAPPINESS);
                    break;
                case StoreItemSO.BonusType.MONEY:
                    ApplyStatChange(
                        ref PlayerData.DineroActual,
                        item.InitialBonus,
                        int.MinValue,
                        int.MaxValue,
                        StoreItemSO.BonusType.MONEY);
                    break;
                case StoreItemSO.BonusType.REPUTATION:
                    ApplyStatChange(
                        ref PlayerData.ReputacionActual,
                        item.InitialBonus,
                        -PlayerData.ReputacionMax,
                        PlayerData.ReputacionMax,
                        StoreItemSO.BonusType.REPUTATION);
                    break;
            }
        }

        private static void ApplyStatChange(
            ref int stat,
            int delta,
            int min,
            int max,
            StoreItemSO.BonusType type)
        {
            int prev = stat;
            stat = Mathf.Clamp(stat + delta, min, max);
            GlobalEventHUD.InvokeValueChanged(prev, stat, type);
        }

        private void Play(VideojuegoSO videojuego)
        {
            if (_gameData.LasDayPlayed >= _gameData.DayCount)
            {
                GlobalEventHUD.InvokeMessage("Ya has jugado hoy.");
                return;
            }

            if (videojuego.DiasJugables <= videojuego.DiasJugados)
            {
                GlobalEventHUD.InvokeMessage("No te quedan dias para jugar a este juego.");
                return;
            }

            int prev = PlayerData.felicidadActual;
            PlayerData.felicidadActual =
                (int)Mathf.Clamp(PlayerData.felicidadActual + videojuego.BonusActual, 0, 100);
            if (prev != PlayerData.felicidadActual)
                GlobalEventHUD.InvokeValueChanged(prev, PlayerData.felicidadActual,
                    StoreItemSO.BonusType.HAPPINESS);
            videojuego.BonusActual -= videojuego.BonusActual * videojuego.DecreaseDiario;
            videojuego.DiasJugados++;
            _gameData.LasDayPlayed = _gameData.DayCount;
        }

        public float GetProgresoMision(string id)
        {
            if (registroMisiones.TryGetValue(id, out Mision m))
            {
                // Si la misión ya está cumplida, devolvemos un valor alto (ej: 99) 
                // o el número total de pasos.
                if (m.IsCumplida) return m.Pasos.Count + 1;

                // Si no, buscamos el índice del paso actual
                for (int i = 0; i < m.Pasos.Count; i++)
                {
                    if (!m.Pasos[i].IsCumplido)
                    {
                        Debug.Log("Mision: " + m.misionID + " Pasos: " + i);
                        return i;
                    }
                }

                Debug.Log("Mision: " + m.misionID + " Pasos: " + m.Pasos.Count);
                return m.Pasos.Count;
            }

            Debug.Log("No existe esta mision");
            return -1; // No existe esa misión
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }
    }
}