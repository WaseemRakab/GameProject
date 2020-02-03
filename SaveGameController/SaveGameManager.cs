using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

/**
 * SaveGame Handler, Getting Files,Saving Files
 */
public static class SaveGameManager
{
    public static void LoadNewGameData(ScriptablePlayerStats playerStats, ScriptableWeapon weaponStats, ScriptableSpawnObjects spawnObjects, ScriptableDataUI dataUI, ScriptablePlayTime playTime, ScriptableEnemyStats enemyStats)
    {
        playerStats._PlayerStats = new PlayerStats();
        weaponStats._Weapon = new Weapon();
        spawnObjects._SpawnObjects = new SpawnObjects();
        dataUI._DataUI = new DataUI();
        playTime._PlayTime = new PlayTime();
        enemyStats._EnemyStats = new EnemyStats();
    }
    public static List<SaveGame> RetrieveSaveGames()//Get All SaveGames From Bson Files
    {
        int MaxSaveGames = 0;
        List<SaveGame> SaveGames = new List<SaveGame>(3);
        string SaveGamesDirectory = Path.Combine(Application.dataPath, "Data", "SaveGames");
        //Get the Full Path , where Bson Save Games Are located
        if (Directory.Exists(SaveGamesDirectory))//Checking if the directory exists
        {
            string[] SaveGamesPaths = Directory.GetFiles(SaveGamesDirectory, "*.bson", SearchOption.TopDirectoryOnly);//Gets All Files With Extension of Bson
            for (int i = 0; i < SaveGamesPaths.Length && MaxSaveGames < 3; ++i)
            {
                if (GetSaveGame(SaveGamesPaths[i], out SaveGame saveGame) && Path.GetFileNameWithoutExtension(SaveGamesPaths[i]) == saveGame._SaveFileName)
                {
                    SaveGames.Add(saveGame);
                    MaxSaveGames++;
                }
                else
                {
                    RemoveCorruptedSaveGame(SaveGamesPaths[i]);
                }
            }
        }
        else
        {
            Directory.CreateDirectory(SaveGamesDirectory);//If Directory Not Exists it Will Be Created for Future SaveGames
        }
        return SaveGames;
    }
    private static void RemoveCorruptedSaveGame(string SaveGamePath)
    {
        try { File.Delete(SaveGamePath); }
        catch (IOException e) { Debug.LogError("Cannot Delete ThatSaveGame\n" + e.StackTrace); }
        catch (UnauthorizedAccessException e) { Debug.LogError("Cannot Delete ThatSaveGame\n" + e.StackTrace); }
    }

    public static void RemoveSaveGameSlot(string SaveGameFileName)
    {
        SaveGameFileName += ".bson";
        string SaveGameFilePath = Path.Combine(Application.dataPath, "Data", "SaveGames", SaveGameFileName);
        if (File.Exists(SaveGameFilePath))
        {
            try { File.Delete(SaveGameFilePath); }
            catch (IOException e) { Debug.LogError("Cannot Delete ThatSaveGame\n" + e.StackTrace); }
            catch (UnauthorizedAccessException e) { Debug.LogError("Cannot Delete ThatSaveGame\n" + e.StackTrace); }
        }
    }
    private static bool GetSaveGame(string SaveGamePath, out SaveGame SaveGame)
    {
        using (FileStream saveGameStreamReader = new FileStream(SaveGamePath, FileMode.Open))
        using (BsonBinaryReader binReader = new BsonBinaryReader(saveGameStreamReader))
        {
            try
            {
                SaveGame = BsonSerializer.Deserialize<SaveGame>(binReader);
            }
            catch (EndOfStreamException)//File Corrupted
            {
                binReader.Close();
                saveGameStreamReader.Close();
                SaveGame = null;
                return false;
            }
            catch (FormatException)//File Corrupted
            {
                binReader.Close();
                saveGameStreamReader.Close();
                SaveGame = null;
                return false;
            }
        }
        return true;
    }
    public static void SaveGameData(string saveGameFileName, SaveGame saveGame)
    {
        saveGameFileName += ".bson";
        string saveGameFilePath = Path.Combine(Application.dataPath, "Data", "SaveGames", saveGameFileName);
        using (FileStream saveGameFileStreamWriter = new FileStream(saveGameFilePath, FileMode.Create))
        using (BsonBinaryWriter binWriter = new BsonBinaryWriter(saveGameFileStreamWriter))
        {
            BsonSerializer.Serialize(binWriter, saveGame);
        }
    }
    public static void OverWriteSaveGame(string saveGameFileName, string PreviousSaveFileName, SaveGame saveGame)
    {
        PreviousSaveFileName += ".bson";
        saveGameFileName += ".bson";
        string previousSaveGameFilePath = Path.Combine(Application.dataPath, "Data", "SaveGames", PreviousSaveFileName);
        string newSaveGameFilePath = Path.Combine(Application.dataPath, "Data", "SaveGames", saveGameFileName);
        if (File.Exists(previousSaveGameFilePath))
        {
            File.Move(previousSaveGameFilePath, newSaveGameFilePath);//Rename FileName
            using (FileStream saveGameFileStreamWriter = new FileStream(newSaveGameFilePath, FileMode.Create))
            using (BsonBinaryWriter binWriter = new BsonBinaryWriter(saveGameFileStreamWriter))
            {
                BsonSerializer.Serialize(binWriter, saveGame);
            }
        }
    }
    public static void SaveRecentGame(SaveGame RecentSave)
    {
        string RecentGameFilePath = Path.Combine(Application.streamingAssetsPath, "RecentGame.bson");
        using (FileStream saveGameFileStreamWriter = new FileStream(RecentGameFilePath, FileMode.Create))
        using (BsonBinaryWriter binWriter = new BsonBinaryWriter(saveGameFileStreamWriter))
        {
            BsonSerializer.Serialize(binWriter, RecentSave);
        }
    }
    public static SaveGame LoadRecentGame()
    {
        string RecentGameFilePath = Path.Combine(Application.streamingAssetsPath, "RecentGame.bson");
        if (File.Exists(RecentGameFilePath))
            if (GetSaveGame(RecentGameFilePath, out SaveGame LoadedGame))
                return LoadedGame;
        return null;
    }
    public static void DeleteRecentGame()
    {
        string RecentGameFilePath = Path.Combine(Application.streamingAssetsPath, "RecentGame.bson");
        if (File.Exists(RecentGameFilePath))
        {
            try { File.Delete(RecentGameFilePath); }
            catch (IOException e) { Debug.LogError("Cannot Delete ThatSaveGame\n" + e.StackTrace); }
            catch (UnauthorizedAccessException e) { Debug.LogError("Cannot Delete ThatSaveGame\n" + e.StackTrace); }
        }
    }
    public static SaveGame LoadSaveGame(string loadGameFileName)
    {
        loadGameFileName += ".bson";
        string loadGameFilePath = Path.Combine(Application.dataPath, "Data", "SaveGames", loadGameFileName);
        if (File.Exists(loadGameFilePath))
            if (GetSaveGame(loadGameFilePath, out SaveGame LoadedGame))
                return LoadedGame;
        return null;
    }
    public static void SetSaveGamesToUI(List<SaveGame> saveGamesToUI, TMP_Text[] saveGamesFileNamesData, TMP_Text[] saveGamesDateData, TMP_Text[] saveGamesLevelData, TMP_Text[] saveGamesStageData, TMP_Text[] saveGamesCashData, GameObject[] emptySlotObjects, GameObject[] saveGamesData)
    {
        for (int i = 0; i < saveGamesToUI.Count; ++i)
        {
            int SlotToSave = saveGamesToUI[i].WhichSlot;
            saveGamesFileNamesData[SlotToSave].text = saveGamesToUI[i]._SaveFileName;
            saveGamesDateData[SlotToSave].text = saveGamesToUI[i]._DateTime;
            saveGamesLevelData[SlotToSave].text = saveGamesToUI[i]._PlayerLevel.LevelNumber.ToString();
            saveGamesStageData[SlotToSave].text = saveGamesToUI[i]._PlayerLevel.StageNumber.ToString();
            saveGamesCashData[SlotToSave].text = saveGamesToUI[i]._PlayerStats.MyCash.ToString() + " $";
            emptySlotObjects[SlotToSave].SetActive(false);
            saveGamesData[SlotToSave].SetActive(true);
        }
    }
    public static void SetLoadGamesToUI(List<SaveGame> loadGamesToUI, TMP_Text[] loadGamesFileNamesData, TMP_Text[] loadGamesDateData, TMP_Text[] loadGamesLevelData, TMP_Text[] loadGamesStageData, TMP_Text[] loadGamesCashData, GameObject[] emptySlotObjects, GameObject[] loadGamesData)
    {
        SetSaveGamesToUI(
             loadGamesToUI, loadGamesFileNamesData,
             loadGamesDateData, loadGamesLevelData,
             loadGamesStageData, loadGamesCashData,
             emptySlotObjects, loadGamesData);
    }
}