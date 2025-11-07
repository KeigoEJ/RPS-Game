using UnityEngine;
using TMPro; 
using System.Collections; 
using UnityEngine.SceneManagement; // NEW: Required for loading scenes

public enum Move
{
    None,
    Rock,
    Paper,
    Scissors
}

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    [Tooltip("The starting health for both player and enemy")]
    public int startingHealth = 10;
    // NEW: Add a field for your main menu scene's name
    [Tooltip("The exact name of your Main Menu scene")]
    public string mainMenuSceneName = "MainMenu";


    [Header("UI References")]
    [Tooltip("Drag your TextMeshPro UI element for showing player's choice")]
    public TextMeshProUGUI playerChoiceText;

    [Tooltip("Drag your TextMeshPro UI element for showing enemy's choice")]
    public TextMeshProUGUI enemyChoiceText;

    [Tooltip("Drag your TextMeshPro UI element for showing the round winner")]
    public TextMeshProUGUI winnerText;
    
    [Tooltip("Drag your TextMeshPro UI element for showing player's health")]
    public TextMeshProUGUI playerHealthText;
    
    [Tooltip("Drag your TextMeshPro UI element for showing enemy's health")]
    public TextMeshProUGUI enemyHealthText;
    
    [Tooltip("Drag your TextMeshPro UI element for the BONK text")]
    public TextMeshProUGUI bonkText;
    
    [Tooltip("Drag the parent GameObject that holds your player control buttons")]
    public GameObject playerControlsContainer;

    // NEW: Reference to the Game Over panel
    [Tooltip("Drag your full-screen Game Over panel object here")]
    public GameObject gameOverPanel;


    private Move playerMove;
    private Move enemyMove;
    private System.Random random = new System.Random();
    
    private int playerHealth;
    private int enemyHealth;
    private bool isGameOver = false;

    void Start()
    {
        playerHealth = startingHealth;
        enemyHealth = startingHealth;
        isGameOver = false;

        if (bonkText != null)
        {
            bonkText.gameObject.SetActive(false);
        }

        if (playerControlsContainer != null)
        {
            playerControlsContainer.SetActive(true);
        }

        // NEW: Make sure the game over panel is hidden on start
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        UpdateHealthUI();
        winnerText.text = "Choose your move!";
        winnerText.color = Color.white;
        playerChoiceText.text = "Player chose: ---";
        enemyChoiceText.text = "Enemy chose: ---";
    }

    public void OnPlayerChoseRock()
    {
        PlayRound(Move.Rock);
    }

    public void OnPlayerChosePaper()
    {
        PlayRound(Move.Paper);
    }

    public void OnPlayerChoseScissors()
    {
        PlayRound(Move.Scissors);
    }

    // Core Game Logic
    private void PlayRound(Move chosenMove)
    {
        if (isGameOver)
        {
            return;
        }
        
        playerMove = chosenMove;
        enemyMove = GetEnemyMove();
        string winner = DetermineWinner(playerMove, enemyMove);

        if (winner == "Player")
        {
            int damage = GetDamageAmount(playerMove);
            enemyHealth -= damage;
            Debug.Log($"Player wins the round! Enemy takes {damage} damage.");
            StartCoroutine(ShowBonkEffect());
        }
        else if (winner == "Enemy")
        {
            int damage = GetDamageAmount(enemyMove);
            playerHealth -= damage;
            Debug.Log($"Enemy wins the round! Player takes {damage} damage.");
            StartCoroutine(ShowBonkEffect());
        }
        else
        {
            Debug.Log("It's a draw! No damage dealt.");
        }

        UpdateUI(winner);
        UpdateHealthUI();
        CheckForGameOver();
    }

    private Move GetEnemyMove()
    {
        int roll = random.Next(1, 4);
        switch (roll)
        {
            case 1:
                return Move.Rock;
            case 2:
                return Move.Paper;
            case 3:
                return Move.Scissors;
            default:
                return Move.Rock;
        }
    }

    private string DetermineWinner(Move pMove, Move eMove)
    {
        if (pMove == eMove)
        {
            return "Draw";
        }
        if ((pMove == Move.Rock && eMove == Move.Scissors) ||
            (pMove == Move.Paper && eMove == Move.Rock) ||
            (pMove == Move.Scissors && eMove == Move.Paper))
        {
            return "Player";
        }
        return "Enemy";
    }

    private int GetDamageAmount(Move winningMove)
    {
        switch (winningMove)
        {
            case Move.Rock:
                return 1;
            case Move.Paper:
                return 2;
            case Move.Scissors:
                return 3;
            default:
                return 0;
        }
    }


    private void UpdateUI(string winner)
    {
        if (playerChoiceText == null || enemyChoiceText == null || winnerText == null)
        {
            Debug.LogError("Core UI References are not set in the GameManager Inspector!");
            return;
        }

        playerChoiceText.text = $"Player chose: {playerMove}";
        enemyChoiceText.text = $"Enemy chose: {enemyMove}";

        winnerText.text = $"{winner} Wins!";

        if (winner == "Player")
        {
            winnerText.color = Color.green;
        }
        else if (winner == "Enemy")
        {
            winnerText.color = Color.red;
        }
        else // Draw
        {
            winnerText.text = "It's a Draw!";
            winnerText.color = Color.gray;
        }
    }
    
    private void UpdateHealthUI()
    {
        if (playerHealthText == null || enemyHealthText == null)
        {
             Debug.LogError("Health UI References are not set in the GameManager Inspector!");
            return;
        }
        
        playerHealthText.text = $"Player Health: {Mathf.Max(0, playerHealth)}";
        enemyHealthText.text = $"Enemy Health: {Mathf.Max(0, enemyHealth)}";
    }

    private IEnumerator ShowBonkEffect()
    {
        if (bonkText != null)
        {
            bonkText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f); 
            bonkText.gameObject.SetActive(false);
        }
    }
    
    private void CheckForGameOver()
    {
        if (playerHealth <= 0 || enemyHealth <= 0)
        {
            isGameOver = true;
            
            if (playerControlsContainer != null)
            {
                playerControlsContainer.SetActive(false);
            }
            
            if (playerHealth <= 0)
            {
                winnerText.text = "Enemy Wins the Match!";
                winnerText.color = Color.red;
            }
            else
            {
                winnerText.text = "Player Wins the Match!";
                winnerText.color = Color.green;
            }

            // NEW: Call a coroutine to show the panel after a short delay
            StartCoroutine(ShowGameOverPanel(1.5f));
        }
    }

    // NEW: Coroutine to show the game over panel after a delay
    private IEnumerator ShowGameOverPanel(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    // --- NEW: Public Functions for Game Over Panel Buttons ---

    /// <summary>
    /// Call this from your "Restart" button's OnClick() event.
    /// </summary>
    public void OnRestart()
    {
        Debug.Log("Restarting game...");
        // Reloads the currently active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Call this from your "Main Menu" button's OnClick() event.
    /// </summary>
    public void OnBackToMainMenu()
    {
        Debug.Log($"Returning to main menu: {mainMenuSceneName}");
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogError("Main Menu Scene Name is not set in the GameManager Inspector!");
        }
    }
}