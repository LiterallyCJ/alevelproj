using UnityEngine;
using UnityEngine.SceneManagement;                                  // Make sure that the script can access the SceneManager, so it can load the main game scene

public class mainMenu : MonoBehaviour
{
    public void startGame()
    {
        globals.player.coins = 10000;
        SceneManager.LoadSceneAsync("mainGame");                    // Load the scene which will contain the rest of the game
    }                                                               // Async added to make sure that the game waits for the scene to be loaded first

    public void quitGame()                                          // Later also save the game before quitting
    {
        Application.Quit();                                         // Close the game safely
    }
}

