using System;
using System.Collections.Generic;
using Doceragnum;
using UnityEngine;

[Serializable]
public struct PersistentGameData
{
    [System.Serializable]
    public struct GameDataValues
    {
        public int difficulty;

        public int Seed;
        public int NpcCount;

        public int DayCount;
        public int CurrentPhase;
        public int LastDayPlayed;

        public GameDataValues(GameData gameData)
        {
            difficulty = (int)gameData.difficulty;
            Seed = gameData.Seed;
            NpcCount = gameData.NpcCount;
            DayCount = gameData.DayCount;
            CurrentPhase = gameData.CurrentPhase;
            LastDayPlayed = gameData.LasDayPlayed;
        }

        public void ToGameData(GameData gameData)
        {
            gameData.difficulty = (GameData.Difficulty)difficulty;
            gameData.Seed = Seed;
            gameData.NpcCount = NpcCount;
            gameData.DayCount = DayCount;
            gameData.CurrentPhase = CurrentPhase;
            gameData.LasDayPlayed =  LastDayPlayed;
        }
    }

    [System.Serializable]
    public struct PlayerDataValues
    {
        public string nombreUsuario;
        public int felicidadMax;
        public int felicidadActual;
        public int ReputacionMax;
        public int ReputacionActual;
        public int DineroActual;
        public int estamina;
        public Vector3 position;

        public PlayerDataValues(PlayerData playerData)
        {
            nombreUsuario = playerData.nombreUsuario;
            felicidadMax = playerData.felicidadMax;
            felicidadActual = playerData.felicidadActual;
            ReputacionMax = playerData.ReputacionMax;
            ReputacionActual = playerData.ReputacionActual;
            DineroActual = playerData.DineroActual;
            estamina = playerData.estamina;
            position = playerData.position;
        }

        public void ToPlayerData(PlayerData playerData)
        {
            playerData.nombreUsuario = nombreUsuario;
            playerData.felicidadMax = felicidadMax;
            playerData.felicidadActual = felicidadActual;
            playerData.ReputacionMax = ReputacionMax;
            playerData.ReputacionActual = ReputacionActual;
            playerData.DineroActual = DineroActual;
            playerData.estamina = estamina;
            playerData.position = position;
        }
    }

    [System.Serializable]
    public struct InventoryValues
    {
        [System.Serializable]
        public struct VideojuegoValues
        {
            public string nombreDelVideojuego;
            public bool isBought;
            public float BonusActual;
            public int DiasJugados;

            public VideojuegoValues(VideojuegoSO videojuegoSo)
            {
                nombreDelVideojuego = videojuegoSo.nombreDelVideojuego;
                isBought = videojuegoSo.isBought;
                BonusActual = videojuegoSo.BonusActual;
                DiasJugados = videojuegoSo.DiasJugados;
            }

            public void ToVideojuegoSo(VideojuegoSO videojuegoSo)
            {
                videojuegoSo.isBought = isBought;
                videojuegoSo.BonusActual = BonusActual;
                videojuegoSo.DiasJugados = DiasJugados;
            }
        }

        [System.Serializable]
        public struct StoreItemData
        {
            public string ItemName;
            public int type;
            public bool isBought;
            public int QuantityConsumed;
            public int LastUse;

            public StoreItemData(StoreItemSO storeItemSo)
            {
                ItemName = storeItemSo.ItemName;
                type = (int)storeItemSo.type;
                isBought = storeItemSo.isBought;
                QuantityConsumed = storeItemSo.QuantityConsumed;
                LastUse = storeItemSo.LastUse;
            }

            public void ToStoreItemSO(StoreItemSO storeItemSo)
            {
                storeItemSo.type = (StoreItemSO.BonusType)type;
                storeItemSo.isBought = isBought;
                storeItemSo.QuantityConsumed = QuantityConsumed;
                storeItemSo.LastUse = LastUse;
            }
        }

        [System.Serializable]
        public struct BillData
        {
            public string Name;
            public int DayLastPayment;

            public BillData(BillSO billSo)
            {
                Name = billSo.Name;
                DayLastPayment = billSo.DayLastPayment;
            }

            public void ToBillSO(BillSO billSo)
            {
                billSo.DayLastPayment = DayLastPayment;
            }
        }

        public List<VideojuegoValues> videojuegos;
        public List<StoreItemData> items;
        public List<BillData> bills;

        public InventoryValues(InventorySO inventorySo)
        {
            videojuegos = new List<VideojuegoValues>();
            if (inventorySo?._videojuegos != null)
            {
                foreach (var videojuego in inventorySo._videojuegos)
                {
                    if (videojuego != null)
                        videojuegos.Add(new VideojuegoValues(videojuego));
                }
            }

            items = new List<StoreItemData>();
            if (inventorySo?._items != null)
            {
                foreach (var item in inventorySo._items)
                {
                    if (item != null)
                        items.Add(new StoreItemData(item));
                }
            }

            bills = new List<BillData>();
            if (inventorySo?._bills != null)
            {
                foreach (var bill in inventorySo._bills)
                {
                    if (bill != null)
                        bills.Add(new BillData(bill));
                }
            }
        }
    }

    [System.Serializable]
    public struct MisionValues
    {
        public string MissionId;
        public List<bool> Steps;

        public MisionValues(Mision mision)
        {
            MissionId = mision.misionID;
            Steps = new List<bool>();

            if (mision.Pasos == null)
                return;

            foreach (var step in mision.Pasos)
                Steps.Add(step.IsCumplido);
        }

        public void ToMisionData(Mision mision)
        {
            if (mision.Pasos == null || Steps == null)
                return;

            bool isComplete = true;
            int count = Mathf.Min(mision.Pasos.Count, Steps.Count);

            for (int i = 0; i < count; i++)
            {
                mision.Pasos[i].IsCumplido = Steps[i];
                if (!mision.Pasos[i].IsCumplido)
                    isComplete = false;
            }

            for (int i = count; i < mision.Pasos.Count; i++)
            {
                mision.Pasos[i].IsCumplido = false;
                isComplete = false;
            }

            mision.IsCumplida = isComplete && count == mision.Pasos.Count;
        }
    }

    public GameDataValues GameData;
    public PlayerDataValues PlayerData;
    public List<MisionValues> Misions;
    public InventoryValues Inventory;

    public PersistentGameData(GameData gameData, PlayerData playerData, List<Mision> misions, InventorySO inventorySo)
    {
        GameData = new GameDataValues(gameData);
        PlayerData = new PlayerDataValues(playerData);
        Misions = new List<MisionValues>();
        Inventory = new InventoryValues(inventorySo);

        foreach (var mision in misions)
            Misions.Add(new MisionValues(mision));
    }

    public static PersistentGameData CreateNewGameSnapshot(
        GameData gameData,
        PlayerData playerData,
        List<Mision> missions,
        InventorySO inventorySo)
    {
        var snapshot = new PersistentGameData
        {
            GameData = CreateNewGameGameData(gameData),
            PlayerData = CreateNewGamePlayerData(playerData),
            Misions = new List<MisionValues>(),
            Inventory = CreateNewGameInventory(inventorySo)
        };

        if (missions != null)
        {
            foreach (var mission in missions)
            {
                if (mission != null)
                    snapshot.Misions.Add(CreateNewGameMission(mission));
            }
        }

        return snapshot;
    }

    private static GameDataValues CreateNewGameGameData(GameData gameData)
    {
        var values = new GameDataValues(gameData);
        values.DayCount = gameData.diaInicial;
        values.CurrentPhase = gameData.faseInicial;
        values.LastDayPlayed = 0;
        return values;
    }

    private static PlayerDataValues CreateNewGamePlayerData(PlayerData playerData)
    {
        var values = new PlayerDataValues(playerData);
        values.DineroActual = playerData.dineroInicial;
        values.felicidadActual = playerData.felicidadInicial;
        values.ReputacionActual = playerData.reputacionInicial;
        values.estamina = playerData.estaminaInicial;
        values.position = Vector3.zero;
        return values;
    }

    private static MisionValues CreateNewGameMission(Mision mission)
    {
        var missionValues = new MisionValues
        {
            MissionId = mission.misionID,
            Steps = new List<bool>()
        };

        if (mission.Pasos == null)
            return missionValues;

        for (int i = 0; i < mission.Pasos.Count; i++)
            missionValues.Steps.Add(false);

        return missionValues;
    }

    private static InventoryValues CreateNewGameInventory(InventorySO inventorySo)
    {
        var inventoryValues = new InventoryValues
        {
            videojuegos = new List<InventoryValues.VideojuegoValues>(),
            items = new List<InventoryValues.StoreItemData>(),
            bills = new List<InventoryValues.BillData>()
        };

        if (inventorySo?._videojuegos != null)
        {
            foreach (var videojuego in inventorySo._videojuegos)
            {
                if (videojuego == null)
                    continue;

                inventoryValues.videojuegos.Add(new InventoryValues.VideojuegoValues
                {
                    nombreDelVideojuego = videojuego.nombreDelVideojuego,
                    isBought = videojuego.PrecioJuego == 0,
                    BonusActual = videojuego.BonusInicial,
                    DiasJugados = 0
                });
            }
        }

        if (inventorySo?._items != null)
        {
            foreach (var item in inventorySo._items)
            {
                if (item == null)
                    continue;

                inventoryValues.items.Add(new InventoryValues.StoreItemData
                {
                    ItemName = item.ItemName,
                    type = (int)item.type,
                    isBought = false,
                    QuantityConsumed = 0,
                    LastUse = 0
                });
            }
        }

        if (inventorySo?._bills != null)
        {
            foreach (var bill in inventorySo._bills)
            {
                if (bill == null)
                    continue;

                inventoryValues.bills.Add(new InventoryValues.BillData
                {
                    Name = bill.Name,
                    DayLastPayment = 0
                });
            }
        }

        return inventoryValues;
    }

    public void ApplyTo(GameData gameData, PlayerData playerData, List<Mision> misions, InventorySO inventorySo)
    {
        GameData.ToGameData(gameData);
        PlayerData.ToPlayerData(playerData);
        Inventory.ApplyTo(inventorySo);
        ApplyMissions(misions);
    }

    private void ApplyMissions(List<Mision> misions)
    {
        if (misions == null || Misions == null)
            return;

        foreach (var savedMission in Misions)
        {
            foreach (var mission in misions)
            {
                if (mission != null && mission.misionID == savedMission.MissionId)
                {
                    savedMission.ToMisionData(mission);
                    break;
                }
            }
        }
    }
}

public static class InventoryValuesExtensions
{
    public static void ApplyTo(this PersistentGameData.InventoryValues inventoryValues, InventorySO inventorySo)
    {
        if (inventorySo == null)
            return;

        ApplyVideojuegos(inventoryValues.videojuegos, inventorySo._videojuegos);
        ApplyStoreItems(inventoryValues.items, inventorySo._items);
        ApplyBills(inventoryValues.bills, inventorySo._bills);
    }

    private static void ApplyVideojuegos(
        List<PersistentGameData.InventoryValues.VideojuegoValues> saved,
        VideojuegoSO[] videojuegos)
    {
        if (saved == null || videojuegos == null)
            return;

        foreach (var savedGame in saved)
        {
            foreach (var videojuego in videojuegos)
            {
                if (videojuego != null && videojuego.nombreDelVideojuego == savedGame.nombreDelVideojuego)
                {
                    savedGame.ToVideojuegoSo(videojuego);
                    break;
                }
            }
        }
    }

    private static void ApplyStoreItems(
        List<PersistentGameData.InventoryValues.StoreItemData> saved,
        StoreItemSO[] items)
    {
        if (saved == null || items == null)
            return;

        foreach (var savedItem in saved)
        {
            foreach (var item in items)
            {
                if (item != null && item.ItemName == savedItem.ItemName)
                {
                    savedItem.ToStoreItemSO(item);
                    break;
                }
            }
        }
    }

    private static void ApplyBills(
        List<PersistentGameData.InventoryValues.BillData> saved,
        BillSO[] bills)
    {
        if (saved == null || bills == null)
            return;

        foreach (var savedBill in saved)
        {
            foreach (var bill in bills)
            {
                if (bill != null && bill.Name == savedBill.Name)
                {
                    savedBill.ToBillSO(bill);
                    break;
                }
            }
        }
    }
}