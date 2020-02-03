using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System.IO;
using UnityEngine;

/**
 * Level Data,EnemiesDamages Files Controler
 */
public static class LevelDataManager
{
    private static readonly string LevelDataPath = Path.Combine(Application.streamingAssetsPath, "LevelData");
    public static void SaveLevelData(string WhichLevel)
    {
        if (Directory.Exists(LevelDataPath))
        {
            string LevelDataFilePath = Path.Combine(LevelDataPath, WhichLevel + ".json");
            using (StreamWriter LevelDataStreamer = new StreamWriter(LevelDataFilePath))
            {
                string JsonData = BsonExtensionMethods.ToJson(new LevelData());
                LevelDataStreamer.Write(JsonData);
            }
        }
    }
    public static void SaveEnemiesDamages(string EnemiesDamages)
    {
        if (Directory.Exists(LevelDataPath))
        {
            string EnemiesDamagesPath = Path.Combine(LevelDataPath, EnemiesDamages + ".json");
            using (StreamWriter EnemiesDamagesWriter = new StreamWriter(EnemiesDamagesPath))
            {
                string JsonData = BsonExtensionMethods.ToJson(new EnemyDamages());
                EnemiesDamagesWriter.Write(JsonData);
            }
        }
    }
    public static bool LoadEnemiesDamage(string EnemiesDamage, out EnemyDamages EnemyDamages)
    {
        if (Directory.Exists(LevelDataPath))
        {
            string EnemiesDamagesPath = Path.Combine(LevelDataPath, EnemiesDamage + ".json");
            if (File.Exists(EnemiesDamagesPath))
            {
                using (StreamReader EnemiesDamagesReader = new StreamReader(EnemiesDamagesPath))
                {
                    string JsonData = EnemiesDamagesReader.ReadToEnd();
                    EnemyDamages = BsonSerializer.Deserialize<EnemyDamages>(JsonData);
                }
                return true;
            }
        }
        EnemyDamages = null;
        return false;
    }

    public static bool LoadLevelData(string WhichLevel, out LevelData LevelData)
    {
        if (Directory.Exists(LevelDataPath))
        {
            string LevelDataFilePath = Path.Combine(LevelDataPath, WhichLevel + ".json");
            if (File.Exists(LevelDataFilePath))
            {
                using (StreamReader LevelDataStreamer = new StreamReader(LevelDataFilePath))
                {
                    string JsonData = LevelDataStreamer.ReadToEnd();
                    LevelData = BsonSerializer.Deserialize<LevelData>(JsonData);
                }
                return true;
            }
        }
        LevelData = null;
        return false;
    }
}