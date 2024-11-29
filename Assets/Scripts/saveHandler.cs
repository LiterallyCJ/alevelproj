using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class saveHandler                                                         // Make sure saveHandler can't be instantiated somewhere else
{
    static string playerFile = Application.persistentDataPath + "/playerStats.itd";     // Find a filepath that will make sure that the data won't be lost
    static BinaryFormatter formatter = new BinaryFormatter();                           // Initialise a new BinaryFormatter which can convert data into and out of binary

    public static void save()
    {
        FileStream fs = new FileStream(playerFile, FileMode.OpenOrCreate);              // Create a FileStream to the player data path, which can open or create a file if there is none
        formatter.Serialize(fs, globals.player);                                        // Turn the player data into binary and write it into the file
        fs.Close();                                                                     // Close and dispose of the FileStream (to prevent memory leakage)
    }

    public static bool load()
    {
        if (!File.Exists(playerFile))                                                   // Check if the playerFile exists
        {
            Debug.LogError("No player stats file.");                                    // Debug with an error and return false
            return false;
        }

        FileStream fs = new FileStream(playerFile, FileMode.Open);                      // Open a FileStream which can read data inside files
        globals.player = formatter.Deserialize(fs) as globals.playerStats;              // Convert the binary into data saved as a playerStats class
        fs.Close();

        return true;                                                                    // Let the calling function know it was successful
    }
}
