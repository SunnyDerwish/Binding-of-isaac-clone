using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject titleMenu;  // Drag your panel containing all title menu elements here
    public GameObject player;     // Drag your player GameObject here
    public Camera titleScreenCamera;
    public Camera mainCamera;
    public GameObject pauseMenu;
    private bool isPaused = false;

    void Start()
    {
        // Initially, let's disable player control
        player.GetComponent<PlayerController>().enabled = false;  

        titleScreenCamera.enabled = true;
        mainCamera.enabled = false;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void StartGame()
    {
        // Enable player controls
        player.GetComponent<PlayerController>().enabled = true;

        titleScreenCamera.enabled = false;
        mainCamera.enabled =  true;

        // Hide the title menu
        titleMenu.SetActive(false);
    }

    public void OpenOptions()
    {
        // Open your options panel or scene
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
    }
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
    }
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
