using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using System.Reflection;
using System.ComponentModel;

public class globals : MonoBehaviour
{
    [System.Serializable]                                                           // Making the class serializable means I can store it as a binary
    public class playerStats                                                        // Class which will hold information that can't be calculated/mostly constant (e.g level)
    {                                                                               // Other stats such as health, damage, ect. can be calculated from the player's gear
        public int level = 0;
        public int coins = 0;
    }

    public class playerSettings                                                     // No need to serialise as this will be stored as a JSON file
    {
        public float volume = 1f;
    }

    // Create  a new instance of playerStats which can be accessed by other scripts
    public static playerStats player = new playerStats();

    void debug()                                                                    // Method that is ran so I can see what the player's stats are
    {
        Debug.Log($"Player loaded with stats: ");           

        PropertyInfo[] properties = typeof(playerStats).GetProperties();            // Get all attributes of the playerStats class as an array
        foreach (PropertyInfo property in properties)                               // Iterate through the attributes
        {
            Debug.Log($"{property}: {property.GetValue(player)}");                  // Log the attribute and its value
        }
    }

    void Start()                                                                    // Start is a function in every script that is ran once when the script is loaded
    {
        debug();
    }
}

