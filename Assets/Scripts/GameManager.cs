using UnityEngine;
using UnityEngine.UI; // For Sliders
using TMPro; 
using System.Collections; 
using System.Collections.Generic; // For the AI's list of moves
using UnityEngine.SceneManagement;

public enum Move
{
    None,
    Rock,
    Paper,
    Scissors
}

public enum SpecialMove
{
    None,
    Attack,
    Defense,
    BoostRage,
    Heal
}

public enum RageCardCategory
{
    Offensive,
    Defensive
}

public class GameManager : MonoBehaviour
{
    [Header("Game Settings")]
    public int startingHealth = 20;
    public string mainMenuSceneName = "MainMenu";
    public int maxRage = 10;
    public float nextRoundDelay = 2.0f;
    public float roundDuration = 30f;
    
    [Header("Special Move Cooldowns")]
    public int attackCooldownSetting = 2;
    public int defenseCooldownSetting = 2;
    public int boostCooldownSetting = 3;
    public int healCooldownSetting = 3;

    
    [Header("Special Move Effects")]
    public int healAmount = 5;
    public int boostRageAmount = 5;
    
    [Header("Rage Card Effects")]
    [Tooltip("Health sacrificed for Offensive Card 4")]
    public int rageHealthSacrifice = 2;
    [Tooltip("Damage gained for Offensive Card 4")]
    public int rageSacrificeDamage = 2;


    [Header("UI References")]
    // Health & Rage Sliders
    public Slider playerHealthSlider;
    public Slider enemyHealthSlider;
    public Slider playerRageSlider;
    public Slider enemyRageSlider;

    // Timer Text
    public TextMeshProUGUI timerText;

    // Split Damage Info Text
    public TextMeshProUGUI rockDamageText;
    public TextMeshProUGUI paperDamageText;
    public TextMeshProUGUI scissorsDamageText;

    // Control Containers
    public GameObject playerControlsContainer; 

    // Feedback UI
    public TextMeshProUGUI playerChoiceText;
    public TextMeshProUGUI enemyChoiceText;
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI bonkText;
    
    // Game Over Panel
    public GameObject gameOverPanel;

    // Special Move UI
    [Header("Special UI (Attack)")]
    public GameObject attackButtonObject;
    public GameObject attackCooldownImage;
    public TextMeshProUGUI attackCooldownText;

    [Header("Special UI (Defense)")]
    public GameObject defenseButtonObject;
    public GameObject defenseCooldownImage;
    public TextMeshProUGUI defenseCooldownText;

    [Header("Special UI (Boost Rage)")]
    public GameObject boostButtonObject;
    public GameObject boostCooldownImage;
    public TextMeshProUGUI boostCooldownText;

    [Header("Special UI (Heal)")]
    public GameObject healButtonObject;
    public GameObject healCooldownImage;
    public TextMeshProUGUI healCooldownText;
    
    // Player Rage Card UI
    [Header("Player Rage Card UI")]
    [Tooltip("The parent panel for the PLAYER'S Rage Card choice")]
    public GameObject rageCardPanel;
    
    [Header("Player Offensive Card GameObjects (Buttons)")]
    public GameObject offensiveCard1_Button; // ID 1
    public GameObject offensiveCard2_Button; // ID 2
    public GameObject offensiveCard3_Button; // ID 3
    public GameObject offensiveCard4_Button; // ID 4
    
    [Header("Player Defensive Card GameObjects (Buttons)")]
    public GameObject defensiveCard1_Button; // ID 1
    public GameObject defensiveCard2_Button; // ID 2
    public GameObject defensiveCard3_Button; // ID 3
    public GameObject defensiveCard4_Button; // ID 4
    
    // NEW: Enemy Rage Card UI
    [Header("Enemy Rage Card UI")]
    [Tooltip("The parent panel to show the ENEMY'S chosen card")]
    public GameObject enemyRageCardPanel;
    [Header("Enemy Offensive Card Images")]
    public GameObject enemyOffensiveCard1_Image; // ID 1
    public GameObject enemyOffensiveCard2_Image; // ID 2
    public GameObject enemyOffensiveCard3_Image; // ID 3
    public GameObject enemyOffensiveCard4_Image; // ID 4
    [Header("Enemy Defensive Card Images")]
    public GameObject enemyDefensiveCard1_Image; // ID 1
    public GameObject enemyDefensiveCard2_Image; // ID 2
    public GameObject enemyDefensiveCard3_Image; // ID 3
    public GameObject enemyDefensiveCard4_Image; // ID 4
    
    // Damage Feedback
    [Header("Damage Feedback UI")]
    [Tooltip("Text to show damage taken by the player")]
    public TextMeshProUGUI playerDamageText;
    [Tooltip("Text to show damage taken by the enemy")]
    public TextMeshProUGUI enemyDamageText;
    
    // Enemy Special Indicators
    [Header("Enemy Special Move Indicators")]
    [Tooltip("Image/Icon to show when enemy uses Attack")]
    public GameObject enemyAttackIndicator;
    [Tooltip("Image/Icon to show when enemy uses Defense")]
    public GameObject enemyDefenseIndicator;
    [Tooltip("Image/Icon to show when enemy uses Boost")]
    public GameObject enemyBoostIndicator;
    [Tooltip("Image/Icon to show when enemy uses Heal")]
    public GameObject enemyHealIndicator;


    // --- Private Game State Variables ---
    private Move playerMove;
    private Move enemyMove;
    private SpecialMove playerSpecialMove;
    private SpecialMove enemySpecialMove;
    
    private int playerHealth;
    private int enemyHealth;
    private int playerRage;
    private int enemyRage;

    // Randomized damage
    private int rockDamage;
    private int paperDamage;
    private int scissorsDamage;

    // Game Flow
    private bool isGameOver = false;
    private System.Random random = new System.Random();
    
    // Timer & Round State
    private float currentRoundTimer;
    private bool isRoundActive = false; 
    private bool playerHasChosenRageCard = false; 

    // Player Cooldowns
    private int attackCooldown = 0;
    private int defenseCooldown = 0;
    private int boostCooldown = 0; 
    private int healCooldown = 0; 
    
    // Enemy Cooldowns
    private int enemyAttackCooldown = 0;
    private int enemyDefenseCooldown = 0;
    private int enemyBoostCooldown = 0; 
    private int enemyHealCooldown = 0; 
    
    // Temporary Rage Card Effects
    // Player
    private bool playerHasDoubleDamage = false;
    private bool playerHasIgnoreDefense = false;
    private int  playerDamageSacrifice = 0;
    private bool playerHasBlockAllDamage = false;
    private bool playerHasIgnoreAttack = false;
    private bool playerHealsOnWin = false;
    
    // Enemy
    private bool enemyHasDoubleDamage = false;
    private bool enemyHasIgnoreDefense = false;
    private int  enemyDamageSacrifice = 0;
    private bool enemyHasBlockAllDamage = false;
    private bool enemyHasIgnoreAttack = false;
    private bool enemyHealsOnWin = false;

    // --- Game Flow Functions ---

    void Start()
    {
        InitializeGame();
        
        // Hook up all 8 player card button listeners
        if (offensiveCard1_Button != null) offensiveCard1_Button.GetComponent<Button>().onClick.AddListener(() => OnPlayerChoseOffensiveCard(1));
        if (offensiveCard2_Button != null) offensiveCard2_Button.GetComponent<Button>().onClick.AddListener(() => OnPlayerChoseOffensiveCard(2));
        if (offensiveCard3_Button != null) offensiveCard3_Button.GetComponent<Button>().onClick.AddListener(() => OnPlayerChoseOffensiveCard(3));
        if (offensiveCard4_Button != null) offensiveCard4_Button.GetComponent<Button>().onClick.AddListener(() => OnPlayerChoseOffensiveCard(4));
        
        if (defensiveCard1_Button != null) defensiveCard1_Button.GetComponent<Button>().onClick.AddListener(() => OnPlayerChoseDefensiveCard(1));
        if (defensiveCard2_Button != null) defensiveCard2_Button.GetComponent<Button>().onClick.AddListener(() => OnPlayerChoseDefensiveCard(2));
        if (defensiveCard3_Button != null) defensiveCard3_Button.GetComponent<Button>().onClick.AddListener(() => OnPlayerChoseDefensiveCard(3));
        if (defensiveCard4_Button != null) defensiveCard4_Button.GetComponent<Button>().onClick.AddListener(() => OnPlayerChoseDefensiveCard(4));
    }

    void InitializeGame()
    {
        isGameOver = false;
        playerHealth = startingHealth;
        enemyHealth = startingHealth;
        playerRage = 0;
        enemyRage = 0;

        // Init player cooldowns
        attackCooldown = 0;
        defenseCooldown = 0;
        boostCooldown = 0;
        healCooldown = 0;
        
        // Init enemy cooldowns
        enemyAttackCooldown = 0;
        enemyDefenseCooldown = 0;
        enemyBoostCooldown = 0;
        enemyHealCooldown = 0;

        // Setup UI
        if (bonkText != null) bonkText.gameObject.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (rageCardPanel != null) rageCardPanel.SetActive(false);
        
        // NEW: Hide enemy rage panel
        if (enemyRageCardPanel != null) enemyRageCardPanel.SetActive(false);
        
        if (playerDamageText != null) playerDamageText.gameObject.SetActive(false);
        if (enemyDamageText != null) enemyDamageText.gameObject.SetActive(false);
        HideAllEnemyIndicators();
        
        HideAllPlayerRageCards();
        HideAllEnemyRageCards(); // NEW
        
        if (playerHealthSlider != null) { playerHealthSlider.maxValue = startingHealth; }
        if (enemyHealthSlider != null) { enemyHealthSlider.maxValue = startingHealth; }
        if (playerRageSlider != null) { playerRageSlider.maxValue = maxRage; }
        if (enemyRageSlider != null) { enemyRageSlider.maxValue = maxRage; }
        UpdateHealthAndRageUI(); 

        // Start the first round
        StartCoroutine(StartNewRound()); 
    }

    void Update()
    {
        if (isGameOver || !isRoundActive) return;

        // Timer Logic
        if (currentRoundTimer > 0)
        {
            currentRoundTimer -= Time.deltaTime;
            timerText.text = Mathf.CeilToInt(currentRoundTimer).ToString();
        }
        else
        {
            isRoundActive = false;
            timerText.text = "0";
            OnTimerEnd();
        }
    }

    IEnumerator StartNewRound()
    {
        // 1. Reset all temporary round effects
        ResetRoundEffects();
        
        // 2. Check for Player Rage Event
        if (playerRage >= maxRage)
        {
            yield return StartCoroutine(ShowPlayerRageCards());
        }
        
        // 3. Check for Enemy Rage Event (NOW A COROUTINE)
        if (enemyRage >= maxRage)
        {
            yield return StartCoroutine(TriggerEnemyRageCard());
        }
        
        // 4. Proceed with normal round setup
        
        // Cooldown Tick Down
        attackCooldown = Mathf.Max(0, attackCooldown - 1);
        defenseCooldown = Mathf.Max(0, defenseCooldown - 1);
        boostCooldown = Mathf.Max(0, boostCooldown - 1);
        healCooldown = Mathf.Max(0, healCooldown - 1);
        
        enemyAttackCooldown = Mathf.Max(0, enemyAttackCooldown - 1);
        enemyDefenseCooldown = Mathf.Max(0, enemyDefenseCooldown - 1);
        enemyBoostCooldown = Mathf.Max(0, enemyBoostCooldown - 1);
        enemyHealCooldown = Mathf.Max(0, enemyHealCooldown - 1);
        
        // Reset choices
        playerMove = Move.None;
        playerSpecialMove = SpecialMove.None;
        enemyMove = Move.None;
        enemySpecialMove = SpecialMove.None;

        // Randomize damage
        RandomizeMoveDamage();
        UpdateDamageInfoUI();

        // Show controls
        if (playerControlsContainer != null) playerControlsContainer.SetActive(true);
        UpdateSpecialMoveUI(); 
        
        HideAllEnemyIndicators();
        if (playerDamageText != null) playerDamageText.gameObject.SetActive(false);
        if (enemyDamageText != null) enemyDamageText.gameObject.SetActive(false);

        // Reset feedback text
        winnerText.text = "Choose your Special and Move!";
        winnerText.color = Color.white;
        playerChoiceText.text = "Player: ...";
        enemyChoiceText.text = "Enemy: ...";
        
        // Start the timer
        currentRoundTimer = roundDuration;
        isRoundActive = true; 
    }

    void OnTimerEnd()
    {
        InstantLoss();
    }

    private void PlayRound()
    {
        if (!isRoundActive || isGameOver) return;
        
        isRoundActive = false; // Stop the timer
        
        // Hide all controls
        if (playerControlsContainer != null) playerControlsContainer.SetActive(false);
        ToggleAllSpecialUI(false); 


        // --- 1. Get Enemy Choices ---
        enemySpecialMove = GetEnemySpecialMove();
        enemyMove = GetEnemyMove();
        
        ShowEnemyIndicator(enemySpecialMove);
        
        // --- 2. Apply Instant Effects & Set Cooldowns ---
        ApplyAndSetCooldowns();

        // --- 3. Determine RPS Winner ---
        string winner = DetermineWinner(playerMove, enemyMove);

        // --- 4. Calculate Damage & Rage (Battle) ---
        if (winner == "Player")
        {
            // Player Won
            int baseDamage = GetDamageAmount(playerMove);
            int bonusDamage = (playerSpecialMove == SpecialMove.Attack) ? 2 : 0;
            int damageReduction = (enemySpecialMove == SpecialMove.Defense && !playerHasIgnoreDefense) ? 2 : 0; 
            int sacrificeDamage = playerDamageSacrifice;
            
            int totalDamage = Mathf.Max(0, baseDamage + bonusDamage - damageReduction + sacrificeDamage);
            
            if (playerHealsOnWin)
            {
                int heal = GetDamageAmount(playerMove);
                playerHealth = Mathf.Min(startingHealth, playerHealth + heal);
                Debug.Log($"Player converted win to {heal} heal!");
            }
            else
            {
                // Normal win
                if (playerHasDoubleDamage) totalDamage *= 2; 
                if (enemyHasBlockAllDamage) totalDamage = 0; 
                
                enemyHealth -= totalDamage;
                if (totalDamage > 0) {
                    IncreaseRage(ref enemyRage, totalDamage, false);
                    StartCoroutine(ShowBonkEffect());
                    StartCoroutine(ShowDamageText(enemyDamageText, totalDamage)); 
                }
            }
        }
        else if (winner == "Enemy")
        {
            // Enemy Won
            int baseDamage = GetDamageAmount(enemyMove);
            int bonusDamage = (enemySpecialMove == SpecialMove.Attack && !playerHasIgnoreAttack) ? 2 : 0; 
            int damageReduction = (playerSpecialMove == SpecialMove.Defense) ? 2 : 0;
            int sacrificeDamage = enemyDamageSacrifice;
            
            int totalDamage = Mathf.Max(0, baseDamage + bonusDamage - damageReduction + sacrificeDamage);

            if (enemyHealsOnWin)
            {
                int heal = GetDamageAmount(enemyMove);
                enemyHealth = Mathf.Min(startingHealth, enemyHealth + heal);
                Debug.Log($"Enemy converted win to {heal} heal!");
            }
            else
            {
                // Normal win
                if (enemyHasDoubleDamage) totalDamage *= 2; 
                if (playerHasBlockAllDamage) totalDamage = 0; 
                
                playerHealth -= totalDamage;
                if (totalDamage > 0) {
                    IncreaseRage(ref playerRage, totalDamage, true);
                    StartCoroutine(ShowBonkEffect());
                    StartCoroutine(ShowDamageText(playerDamageText, totalDamage)); 
                }
            }
        }
        else {
            Debug.Log("It's a draw! No damage dealt.");
        }

        // --- 5. Update UI & Check for Game Over ---
        UpdateUI(winner);
        UpdateHealthAndRageUI(); 
        
        if (CheckForGameOver()) {
            StartCoroutine(ShowGameOverPanel(nextRoundDelay));
        } else {
            StartCoroutine(RoundResetDelay()); 
        }
    }
    
    private void ApplyAndSetCooldowns()
    {
        // Player
        if (playerSpecialMove == SpecialMove.Heal)
        {
            playerHealth = Mathf.Min(startingHealth, playerHealth + healAmount);
            healCooldown = healCooldownSetting;
        }
        else if (playerSpecialMove == SpecialMove.BoostRage)
        {
            IncreaseRage(ref playerRage, boostRageAmount, true);
            boostCooldown = boostCooldownSetting;
        }
        else if (playerSpecialMove == SpecialMove.Attack)
        {
            attackCooldown = attackCooldownSetting;
        }
        else if (playerSpecialMove == SpecialMove.Defense)
        {
            defenseCooldown = defenseCooldownSetting;
        }
        
        // Enemy
        if (enemySpecialMove == SpecialMove.Heal)
        {
            enemyHealth = Mathf.Min(startingHealth, enemyHealth + healAmount);
            enemyHealCooldown = healCooldownSetting;
        }
        else if (enemySpecialMove == SpecialMove.BoostRage)
        {
            IncreaseRage(ref enemyRage, boostRageAmount, false);
            enemyBoostCooldown = boostCooldownSetting;
        }
        else if (enemySpecialMove == SpecialMove.Attack)
        {
            enemyAttackCooldown = attackCooldownSetting;
        }
        else if (enemySpecialMove == SpecialMove.Defense)
        {
            enemyDefenseCooldown = defenseCooldownSetting;
        }
    }
    

    // --- Public Button Functions ---
    public void OnPlayerChoseAttack() 
    {
        if (attackCooldown > 0 || !isRoundActive || playerSpecialMove != SpecialMove.None) return;
        playerSpecialMove = SpecialMove.Attack;
        LockInSpecialMove(SpecialMove.Attack);
    }

    public void OnPlayerChoseDefense() 
    { 
        if (defenseCooldown > 0 || !isRoundActive || playerSpecialMove != SpecialMove.None) return;
        playerSpecialMove = SpecialMove.Defense; 
        LockInSpecialMove(SpecialMove.Defense);
    }
    
    public void OnPlayerChoseBoost()
    {
        if (boostCooldown > 0 || !isRoundActive || playerSpecialMove != SpecialMove.None) return;
        playerSpecialMove = SpecialMove.BoostRage;
        LockInSpecialMove(SpecialMove.BoostRage);
    }

    public void OnPlayerChoseHeal()
    {
        if (healCooldown > 0 || !isRoundActive || playerSpecialMove != SpecialMove.None) return;
        playerSpecialMove = SpecialMove.Heal;
        LockInSpecialMove(SpecialMove.Heal);
    }

    // RPS buttons NOW call PlayRound() directly.
    public void OnPlayerChoseRock() { if (!isRoundActive) return; playerMove = Move.Rock; PlayRound(); }
    public void OnPlayerChosePaper() { if (!isRoundActive) return; playerMove = Move.Paper; PlayRound(); }
    public void OnPlayerChoseScissors() { if (!isRoundActive) return; playerMove = Move.Scissors; PlayRound(); }

    // GAME OVER Buttons
    public void OnRestart() { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void OnBackToMainMenu()
    {
        if (!string.IsNullOrEmpty(mainMenuSceneName))
            SceneManager.LoadScene(mainMenuSceneName);
        else
            Debug.LogError("Main Menu Scene Name is not set!");
    }
    
    // --- Rage Card Button Functions ---
    public void OnPlayerChoseOffensiveCard(int cardID)
    {
        ApplyPlayerRageEffect(RageCardCategory.Offensive, cardID);
        if (rageCardPanel != null) rageCardPanel.SetActive(false);
        playerHasChosenRageCard = true; 
    }
    
    public void OnPlayerChoseDefensiveCard(int cardID)
    {
        ApplyPlayerRageEffect(RageCardCategory.Defensive, cardID);
        if (rageCardPanel != null) rageCardPanel.SetActive(false);
        playerHasChosenRageCard = true; 
    }
    

    // --- Helper & Logic Functions ---
    
    private void ResetRoundEffects()
    {
        playerHasDoubleDamage = false;
        playerHasIgnoreDefense = false;
        playerDamageSacrifice = 0;
        playerHasBlockAllDamage = false;
        playerHasIgnoreAttack = false;
        playerHealsOnWin = false;
        
        enemyHasDoubleDamage = false;
        enemyHasIgnoreDefense = false;
        enemyDamageSacrifice = 0;
        enemyHasBlockAllDamage = false;
        enemyHasIgnoreAttack = false;
        enemyHealsOnWin = false;
    }

    private void LockInSpecialMove(SpecialMove chosenMove)
    {
        if (chosenMove != SpecialMove.Attack && attackButtonObject != null)
        {
            attackButtonObject.SetActive(false);
            if (attackCooldown == 0) attackCooldownImage.SetActive(true); 
        }
        if (chosenMove != SpecialMove.Defense && defenseButtonObject != null)
        {
            defenseButtonObject.SetActive(false);
            if (defenseCooldown == 0) defenseCooldownImage.SetActive(true);
        }
        if (chosenMove != SpecialMove.BoostRage && boostButtonObject != null)
        {
            boostButtonObject.SetActive(false);
            if (boostCooldown == 0) boostCooldownImage.SetActive(true);
        }
        if (chosenMove != SpecialMove.Heal && healButtonObject != null)
        {
            healButtonObject.SetActive(false);
            if (healCooldown == 0) healCooldownImage.SetActive(true);
        }
    }

    void InstantLoss()
    {
        Debug.Log("Player ran out of time! Instant Loss.");
        
        if (playerControlsContainer != null) playerControlsContainer.SetActive(false);
        ToggleAllSpecialUI(false); 
        HideAllEnemyIndicators(); 
        
        // NEW: Also hide rage panels on instant loss
        if (rageCardPanel != null) rageCardPanel.SetActive(false);
        if (enemyRageCardPanel != null) enemyRageCardPanel.SetActive(false);

        isGameOver = true;
        playerHealth = 0; 
        UpdateHealthAndRageUI(); 
        winnerText.text = "Out of time! You lose!";
        winnerText.color = Color.red;
        StartCoroutine(ShowGameOverPanel(nextRoundDelay));
    }

    // --- Damage & AI Logic ---
    void RandomizeMoveDamage()
    {
        rockDamage = random.Next(0, 4);
        paperDamage = random.Next(0, 4);
        scissorsDamage = random.Next(0, 4);
    }

    private int GetDamageAmount(Move winningMove)
    {
        switch (winningMove)
        {
            case Move.Rock: return rockDamage;
            case Move.Paper: return paperDamage;
            case Move.Scissors: return scissorsDamage;
            default: return 0;
        }
    }

    private SpecialMove GetEnemySpecialMove()
    {
        List<SpecialMove> availableMoves = new List<SpecialMove>();
        if (enemyAttackCooldown == 0) availableMoves.Add(SpecialMove.Attack);
        if (enemyDefenseCooldown == 0) availableMoves.Add(SpecialMove.Defense);
        if (enemyBoostCooldown == 0) availableMoves.Add(SpecialMove.BoostRage);
        if (enemyHealCooldown == 0) availableMoves.Add(SpecialMove.Heal);
        
        availableMoves.Add(SpecialMove.None);
        availableMoves.Add(SpecialMove.None);
        
        if (enemyHealth < (startingHealth / 2) && availableMoves.Contains(SpecialMove.Heal))
        {
            return SpecialMove.Heal;
        }
        
        int randomIndex = random.Next(0, availableMoves.Count);
        SpecialMove chosenMove = availableMoves[randomIndex];
        return chosenMove;
    }

    private Move GetEnemyMove()
    {
        int roll = random.Next(1, 4);
        switch (roll)
        {
            case 1: return Move.Rock;
            case 2: return Move.Paper;
            case 3: return Move.Scissors;
            default: return Move.Rock;
        }
    }

    private string DetermineWinner(Move pMove, Move eMove)
    {
        if (pMove == eMove) return "Draw";
        if ((pMove == Move.Rock && eMove == Move.Scissors) ||
            (pMove == Move.Paper && eMove == Move.Rock) ||
            (pMove == Move.Scissors && eMove == Move.Paper))
        {
            return "Player";
        }
        return "Enemy";
    }

    private void IncreaseRage(ref int rageMeter, int damageTaken, bool isPlayer)
    {
        if (isGameOver) return;
        
        rageMeter = Mathf.Min(maxRage, rageMeter + damageTaken);
    }
    
    // --- Rage Card Logic ---
    
    private void HideAllPlayerRageCards()
    {
        if (offensiveCard1_Button != null) offensiveCard1_Button.SetActive(false);
        if (offensiveCard2_Button != null) offensiveCard2_Button.SetActive(false);
        if (offensiveCard3_Button != null) offensiveCard3_Button.SetActive(false);
        if (offensiveCard4_Button != null) offensiveCard4_Button.SetActive(false);
        if (defensiveCard1_Button != null) defensiveCard1_Button.SetActive(false);
        if (defensiveCard2_Button != null) defensiveCard2_Button.SetActive(false);
        if (defensiveCard3_Button != null) defensiveCard3_Button.SetActive(false);
        if (defensiveCard4_Button != null) defensiveCard4_Button.SetActive(false);
    }
    
    // NEW: Helper to hide all 8 enemy cards
    private void HideAllEnemyRageCards()
    {
        if (enemyOffensiveCard1_Image != null) enemyOffensiveCard1_Image.SetActive(false);
        if (enemyOffensiveCard2_Image != null) enemyOffensiveCard2_Image.SetActive(false);
        if (enemyOffensiveCard3_Image != null) enemyOffensiveCard3_Image.SetActive(false);
        if (enemyOffensiveCard4_Image != null) enemyOffensiveCard4_Image.SetActive(false);
        if (enemyDefensiveCard1_Image != null) enemyDefensiveCard1_Image.SetActive(false);
        if (enemyDefensiveCard2_Image != null) enemyDefensiveCard2_Image.SetActive(false);
        if (enemyDefensiveCard3_Image != null) enemyDefensiveCard3_Image.SetActive(false);
        if (enemyDefensiveCard4_Image != null) enemyDefensiveCard4_Image.SetActive(false);
    }
    
    private IEnumerator ShowPlayerRageCards()
    {
        isRoundActive = false; // Stop timer
        playerHasChosenRageCard = false; // Set flag to pause
        
        if (playerControlsContainer != null) playerControlsContainer.SetActive(false);
        ToggleAllSpecialUI(false);
        
        int offeredOffensiveCardID = random.Next(1, 5); 
        int offeredDefensiveCardID = random.Next(1, 5); 
        
        HideAllPlayerRageCards();
        
        switch(offeredOffensiveCardID)
        {
            case 1: if (offensiveCard1_Button != null) offensiveCard1_Button.SetActive(true); break;
            case 2: if (offensiveCard2_Button != null) offensiveCard2_Button.SetActive(true); break;
            case 3: if (offensiveCard3_Button != null) offensiveCard3_Button.SetActive(true); break;
            case 4: if (offensiveCard4_Button != null) offensiveCard4_Button.SetActive(true); break;
        }
        
        switch(offeredDefensiveCardID)
        {
            case 1: if (defensiveCard1_Button != null) defensiveCard1_Button.SetActive(true); break;
            case 2: if (defensiveCard2_Button != null) defensiveCard2_Button.SetActive(true); break;
            case 3: if (defensiveCard3_Button != null) defensiveCard3_Button.SetActive(true); break;
            case 4: if (defensiveCard4_Button != null) defensiveCard4_Button.SetActive(true); break;
        }

        if (rageCardPanel != null) rageCardPanel.SetActive(true);
        
        playerRage = 0;
        UpdateHealthAndRageUI();
        
        while (!playerHasChosenRageCard)
        {
            yield return null;
        }
    }
    
    private void ApplyPlayerRageEffect(RageCardCategory category, int cardID)
    {
        Debug.Log($"Player applied {category} Card, ID: {cardID}");
        if (category == RageCardCategory.Offensive)
        {
            switch(cardID)
            {
                case 1: playerHasDoubleDamage = true; break;
                case 2: playerHasIgnoreDefense = true; break;
                case 3: attackCooldown = 0; break; 
                case 4: 
                    playerHealth -= rageHealthSacrifice;
                    playerDamageSacrifice = rageSacrificeDamage;
                    break;
            }
        }
        else // Defensive
        {
            switch(cardID)
            {
                case 1: playerHasBlockAllDamage = true; break;
                case 2: playerHasIgnoreAttack = true; break;
                case 3: defenseCooldown = 0; break; 
                case 4: playerHealsOnWin = true; break;
            }
        }
    }
    
    // NEW: Converted to Coroutine
    private IEnumerator TriggerEnemyRageCard()
    {
        isRoundActive = false; // Stop timer
        
        // Hide game controls
        if (playerControlsContainer != null) playerControlsContainer.SetActive(false);
        ToggleAllSpecialUI(false);
        
        // 1. Generate the two "offers" for the AI
        int offeredOffensiveID = random.Next(1, 5);
        int offeredDefensiveID = random.Next(1, 5);
        
        // 2. AI decides which of the two offers to pick
        RageCardCategory chosenCategory;
        int chosenCardID;
        
        if (enemyHealth < (startingHealth / 2))
        {
            chosenCategory = RageCardCategory.Defensive;
            chosenCardID = offeredDefensiveID;
            Debug.Log($"Enemy is low health. Chose: {GetCardDescription(chosenCategory, chosenCardID)}");
        }
        else
        {
            chosenCategory = RageCardCategory.Offensive;
            chosenCardID = offeredOffensiveID;
            Debug.Log($"Enemy is healthy. Chose: {GetCardDescription(chosenCategory, chosenCardID)}");
        }
        
        // 3. Hide all 8 enemy card images
        HideAllEnemyRageCards();
        
        // 4. Show the ONE chosen card
        if (chosenCategory == RageCardCategory.Offensive)
        {
            switch(chosenCardID)
            {
                case 1: if (enemyOffensiveCard1_Image != null) enemyOffensiveCard1_Image.SetActive(true); break;
                case 2: if (enemyOffensiveCard2_Image != null) enemyOffensiveCard2_Image.SetActive(true); break;
                case 3: if (enemyOffensiveCard3_Image != null) enemyOffensiveCard3_Image.SetActive(true); break;
                case 4: if (enemyOffensiveCard4_Image != null) enemyOffensiveCard4_Image.SetActive(true); break;
            }
        }
        else // Defensive
        {
            switch(chosenCardID)
            {
                case 1: if (enemyDefensiveCard1_Image != null) enemyDefensiveCard1_Image.SetActive(true); break;
                case 2: if (enemyDefensiveCard2_Image != null) enemyDefensiveCard2_Image.SetActive(true); break;
                case 3: if (enemyDefensiveCard3_Image != null) enemyDefensiveCard3_Image.SetActive(true); break;
                case 4: if (enemyDefensiveCard4_Image != null) enemyDefensiveCard4_Image.SetActive(true); break;
            }
        }
        
        // 5. Show the parent panel and apply effect
        if (enemyRageCardPanel != null) enemyRageCardPanel.SetActive(true);
        ApplyEnemyRageEffect(chosenCategory, chosenCardID);
        
        // 6. Reset rage
        enemyRage = 0;
        UpdateHealthAndRageUI();
        
        // 7. Wait for the player to see the card
        yield return new WaitForSeconds(nextRoundDelay);
        
        // 8. Hide the panel and resume
        if (enemyRageCardPanel != null) enemyRageCardPanel.SetActive(false);
    }
    
    private void ApplyEnemyRageEffect(RageCardCategory category, int cardID)
    {
        Debug.Log($"Enemy applied {category} Card, ID: {cardID}");
        if (category == RageCardCategory.Offensive)
        {
            switch(cardID)
            {
                case 1: enemyHasDoubleDamage = true; break;
                case 2: enemyHasIgnoreDefense = true; break;
                case 3: enemyAttackCooldown = 0; break; 
                case 4: 
                    enemyHealth -= rageHealthSacrifice;
                    enemyDamageSacrifice = rageSacrificeDamage;
                    break;
            }
        }
        else // Defensive
        {
            switch(cardID)
            {
                case 1: enemyHasBlockAllDamage = true; break;
                case 2: enemyHasIgnoreAttack = true; break;
                case 3: enemyDefenseCooldown = 0; break; 
                case 4: enemyHealsOnWin = true; break;
            }
        }
    }

    // Helper to get card names (for AI logging)
    private string GetCardDescription(RageCardCategory category, int cardID)
    {
        if (category == RageCardCategory.Offensive)
        {
            switch(cardID)
            {
                case 1: return "Double Damage";
                case 2: return "Ignore Defense";
                case 3: return "Reset Attack CD";
                case 4: return "Sacrifice Health for Damage";
                default: return "Offensive Card";
            }
        }
        else // Defensive
        {
            switch(cardID)
            {
                case 1: return "Block All Damage";
                case 2: return "Ignore Bonus Attack";
                case 3: return "Reset Defense CD";
                case 4: return "Convert Win to Heal";
                default: return "Defensive Card";
            }
        }
    }


    // --- UI & Coroutine Functions ---
    
    private void HideAllEnemyIndicators()
    {
        if (enemyAttackIndicator != null) enemyAttackIndicator.SetActive(false);
        if (enemyDefenseIndicator != null) enemyDefenseIndicator.SetActive(false);
        if (enemyBoostIndicator != null) enemyBoostIndicator.SetActive(false);
        if (enemyHealIndicator != null) enemyHealIndicator.SetActive(false);
    }
    
    private void ShowEnemyIndicator(SpecialMove move)
    {
        HideAllEnemyIndicators();
        
        switch(move)
        {
            case SpecialMove.Attack:
                if (enemyAttackIndicator != null) enemyAttackIndicator.SetActive(true);
                break;
            case SpecialMove.Defense:
                if (enemyDefenseIndicator != null) enemyDefenseIndicator.SetActive(true);
                break;
            case SpecialMove.BoostRage:
                if (enemyBoostIndicator != null) enemyBoostIndicator.SetActive(true);
                break;
            case SpecialMove.Heal:
                if (enemyHealIndicator != null) enemyHealIndicator.SetActive(true);
                break;
            case SpecialMove.None:
                break;
        }
    }
    
    private IEnumerator ShowDamageText(TextMeshProUGUI textElement, int damage)
    {
        if (textElement == null || damage <= 0)
        {
            yield break; 
        }
        
        textElement.text = $"-{damage} HP";
        textElement.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(1.0f);
        
        textElement.gameObject.SetActive(false);
    }
    
    private void ToggleAllSpecialUI(bool show)
    {
        if (attackButtonObject != null) attackButtonObject.SetActive(show);
        if (attackCooldownImage != null) attackCooldownImage.SetActive(show);
        if (defenseButtonObject != null) defenseButtonObject.SetActive(show);
        if (defenseCooldownImage != null) defenseCooldownImage.SetActive(show);
        if (boostButtonObject != null) boostButtonObject.SetActive(show);
        if (boostCooldownImage != null) boostCooldownImage.SetActive(show);
        if (healButtonObject != null) healButtonObject.SetActive(show);
        if (healCooldownImage != null) healCooldownImage.SetActive(show);

        if (!show)
        {
            if (attackCooldownText != null) attackCooldownText.text = "";
            if (defenseCooldownText != null) defenseCooldownText.text = "";
            if (boostCooldownText != null) boostCooldownText.text = "";
            if (healCooldownText != null) healCooldownText.text = "";
        }
    }

    private void UpdateSpecialMoveUI()
    {
        bool isAttackReady = (attackCooldown == 0);
        if (attackButtonObject != null) attackButtonObject.SetActive(isAttackReady);
        if (attackCooldownImage != null) attackCooldownImage.SetActive(!isAttackReady);
        if (attackCooldownText != null) attackCooldownText.text = isAttackReady ? "Ready" : $"CD: {attackCooldown} Rnd";

        bool isDefenseReady = (defenseCooldown == 0);
        if (defenseButtonObject != null) defenseButtonObject.SetActive(isDefenseReady);
        if (defenseCooldownImage != null) defenseCooldownImage.SetActive(!isDefenseReady);
        if (defenseCooldownText != null) defenseCooldownText.text = isDefenseReady ? "Ready" : $"CD: {defenseCooldown} Rnd";

        bool isBoostReady = (boostCooldown == 0);
        if (boostButtonObject != null) boostButtonObject.SetActive(isBoostReady);
        if (boostCooldownImage != null) boostCooldownImage.SetActive(!isBoostReady);
        if (boostCooldownText != null) boostCooldownText.text = isBoostReady ? "Ready" : $"CD: {boostCooldown} Rnd";

        bool isHealReady = (healCooldown == 0);
        if (healButtonObject != null) healButtonObject.SetActive(isHealReady);
        if (healCooldownImage != null) healCooldownImage.SetActive(!isHealReady);
        if (healCooldownText != null) healCooldownText.text = isHealReady ? "Ready" : $"CD: {healCooldown} Rnd";
    }

    private void UpdateUI(string winner)
    {
        playerChoiceText.text = $"Player: {playerSpecialMove} / {playerMove}";
        enemyChoiceText.text = $"Enemy: {enemySpecialMove} / {enemyMove}";

        if (winner == "Player") {
            winnerText.text = "Player Wins Round!";
            winnerText.color = Color.green;
        }
        else if (winner == "Enemy") {
            winnerText.text = "Enemy Wins Round!";
            winnerText.color = Color.red;
        }
        else {
            winnerText.text = "It's a Draw!";
            winnerText.color = Color.gray;
        }
    }

    void UpdateDamageInfoUI()
    {
        if (rockDamageText != null) rockDamageText.text = $"Rock: {rockDamage}";
        if (paperDamageText != null) paperDamageText.text = $"Paper: {paperDamage}";
        if (scissorsDamageText != null) scissorsDamageText.text = $"Scissors: {scissorsDamage}";
    }
    
    private void UpdateHealthAndRageUI()
    {
        if (playerHealthSlider != null) playerHealthSlider.value = playerHealth;
        if (enemyHealthSlider != null) enemyHealthSlider.value = enemyHealth;
        if (playerRageSlider != null) playerRageSlider.value = playerRage;
        if (enemyRageSlider != null) enemyRageSlider.value = enemyRage;
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

    private IEnumerator RoundResetDelay()
    {
        // Wait for the delay
        yield return new WaitForSeconds(nextRoundDelay);
        
        // Call the Coroutine to start the next round
        StartCoroutine(StartNewRound());
    }
    
    private bool CheckForGameOver()
    {
        if (isGameOver) return true;

        if (playerHealth <= 0 || enemyHealth <= 0)
        {
            isGameOver = true;
            if (playerControlsContainer != null) playerControlsContainer.SetActive(false);
            
            if (playerHealth <= 0)
            {
                if (winnerText.text != "Out of time! You lose!")
                {
                    winnerText.text = "Enemy Wins the Match!";
                    winnerText.color = Color.red;
                }
            }
            else
            {
                winnerText.text = "Player Wins the Match!";
                winnerText.color = Color.green;
            }
            return true;
        }
        return false;
    }

    private IEnumerator ShowGameOverPanel(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }
}