using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverSimple : MonoBehaviour
{
    public GameObject gameOverUI;
    public int maxHits = 3;
    private int hits = 0;

    public void TakeHit()
    {
        hits++;

        if (hits >= maxHits)
        {
            gameOverUI.SetActive(true);

            
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            
            Time.timeScale = 0f;
        }
    }

    public void RestartGame()
    {

        Debug.Log("BUTTON WERKT!");

        
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
