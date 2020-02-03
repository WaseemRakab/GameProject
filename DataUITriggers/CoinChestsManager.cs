using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * Managing Coin Chests In-Game, SaveGameControling , Etc..
 * 
 */
public class CoinChestsManager : MonoBehaviour
{
    private Player LaraPlayer;

    public int CoinChests;

    public LevelRestartedOrNextLevel _OnLevelRestarted;
    public ScriptableDataUI _DataUI;
    private void Awake()
    {
        GameObject.FindGameObjectWithTag("Player")?.TryGetComponent(out LaraPlayer);
        CoinChestsDataRefresh();
    }

    private void CoinChestsDataRefresh()
    {
        LaraPlayer.HasBeenInCoinChests = new bool[CoinChests];
        if (_OnLevelRestarted.OnLevelRestarted || _OnLevelRestarted.OnNextLevel)
        {
            _DataUI._DataUI.CoinChestsOpened = new bool[CoinChests];
        }
        else
        {
            GameObject[] CoinChestObjects = GameObject.FindGameObjectsWithTag("CoinChest");
            List<CoinChest> CoinChests = (from chest in CoinChestObjects select chest.GetComponent<CoinChest>()).ToList();
            CoinChests.Sort();//Sorting by CoinChestID, Implemented in class
            for (int i = 0; i < CoinChests.Count; ++i)
            {
                if (_DataUI._DataUI.CoinChestsOpened[i] == true)
                {
                    LaraPlayer.GetComponent<Player>().HasBeenInCoinChests[i] = true;
                    CoinChests[i].OnChestOpened();
                }
                _DataUI._DataUI.CoinChestsOpened[i] = false;
            }
        }
    }
}