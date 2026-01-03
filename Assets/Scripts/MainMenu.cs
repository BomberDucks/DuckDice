using UnityEngine;
using UnityEngine.SceneManagement; // Indispensable pour changer de scène

public class MainMenu : MonoBehaviour
{
    
    public void PlayGame()
    {
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

        
    }

    
    public void QuitGame()
    {
        Debug.Log("Le jeu se ferme ! (Visible uniquement en Build)");
        Application.Quit();
    }
}