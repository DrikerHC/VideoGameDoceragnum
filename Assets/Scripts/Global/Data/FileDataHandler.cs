using System;
using System.IO;
using UnityEngine;

namespace Doceragnum
{
    public class FileDataHandler
    {
        private string dataDirPath = "";
        private string dataFileName = "";

        public FileDataHandler(string dataDirPath, string dataFileName)
        {
            this.dataDirPath = dataDirPath;
            this.dataFileName = dataFileName;
        }

        public bool TryLoad(out PersistentGameData data)
        {
            data = default;
            string fullPath = Path.Combine(dataDirPath, dataFileName);

            if (!File.Exists(fullPath))
                return false;

            data = Load();
            return true;
        }

        public void DeleteSave()
        {
            string fullPath = Path.Combine(dataDirPath, dataFileName);

            if (!File.Exists(fullPath))
                return;

            try
            {
                File.Delete(fullPath);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occured when trying to delete save file: " + fullPath + "\n" + e);
            }
        }

        public PersistentGameData Load()
        {
           

            string fullPath = Path.Combine(dataDirPath, dataFileName);
            
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
                    
                    PersistentGameData loadedData = JsonUtility.FromJson<PersistentGameData>(dataToLoad);
                    return loadedData;
                }
                catch (Exception e)
                {
                    Debug.LogError("Error occured when trying to load file at path: "
                                   + fullPath + " and backup did not work.\n" + e);
                }
            }

            throw  new FileNotFoundException("File not found", fullPath);
        }
        
        public void Save(PersistentGameData data) 
        {
            string fullPath = Path.Combine(dataDirPath, dataFileName);
            try 
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                string dataToStore = JsonUtility.ToJson(data, true);
                

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
                Debug.LogError("Error occured when trying to save data to file: " + fullPath + "\n" + e);
            }
        }

    }
}