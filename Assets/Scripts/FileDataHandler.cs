using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

/*
 * Shouldn't touch this class
 */
public class FileDataHandler
{
    private string dataPath = "";
    private string dataFileName = "";

    private bool encryptData = false;
    private string codeWord = "sss";

    public FileDataHandler(string dataPath, string dataFileName, bool _encrypt)
    {
        this.dataPath = dataPath;
        this.dataFileName = dataFileName;
        this.encryptData = _encrypt;
    }

    public void Save(GameData _data)
    {
        string fullPath = Path.Combine(dataPath, dataFileName);

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            string dataToStore = JsonUtility.ToJson(_data);
            if (encryptData)
                dataToStore = EncryptDecrypt(dataToStore);
            using (FileStream stream = new FileStream(fullPath, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error" + fullPath + " " + e.Message);
        }
    }

    public GameData Load()
    {
        string fullPath = Path.Combine(dataPath, dataFileName);
        GameData loadData = null;
        if (File.Exists(fullPath))
        {
            try
            {
                string dataToLoad = "";
                using (FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                if (encryptData)
                {

                    dataToLoad = EncryptDecrypt(dataToLoad);
                }
                loadData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch
            {
            }
        }
        return loadData;
    }

    public void Delete()
    {

        string fullPath = Path.Combine(dataPath, dataFileName);
        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }
    }

    private string EncryptDecrypt(string _data)
    {
        string modifiedData = "";
        for (int i = 0; i < _data.Length; i++)
        {
            modifiedData += (char)(_data[i] ^ codeWord[i % codeWord.Length]);
        }
        return modifiedData;
    }
}
