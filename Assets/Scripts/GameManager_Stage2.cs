using UnityEngine;
using UnityEngine.UI; // For Sliders
using TMPro; 
using System.Collections; 
using System.Collections.Generic; // For the AI's list of moves
using UnityEngine.SceneManagement;

// -----------------------------------------------------------------
// Enums (Move, SpecialMove, RageCardCategory) are REMOVED
// because your Level 1 GameManager.cs already has them!
// This fixes the compiler errors.
// -----------------------------------------------------------------


// -----------------------------------------------------------------
// SCRIPT NAME: GameManager_Stage2.cs
// -----------------------------------------------------------------
public class GameManager_Stage2 : MonoBehaviour
{
    [Header("Game Settings")]
    public int startingHealth = 20;
    public string mainMenuSceneName = "MainMenu";
    public int maxRage = 10;
    
    [Header("Timing Settings")]
    public float roundDuration = 30f;
    public float delayBeforeNextRound = 2.0f;
    public float gameOverPanelDelay = 1.5f;
    public float bonkEffectDuration = 0.5f;
    public float damageTextDuration = 1.0f;
    public float enemyRageCardRevealDuration = 2.5f;
    [Tooltip("How long to pause to show the RPS choices before battle")]
    public float rpsRevealDuration = 1.0f; 
    
    [Header("Special Move Cooldowns")]
    public int attackCooldownSetting = 2;
    public int defenseCooldownSetting = 2;
    public int boostCooldownSetting = 3;
    public int healCooldownSetting = 3;

    
    [Header("Special Move Effects")]
    public int healAmount = 5;
    public int boostRageAmount = 5;
    
    [Header("Rage Card Effects")]
    public int rageHealthSacrifice = 2;
    public int rageSacrificeDamage = 2;


    [Header("UI References")]
    // Health & Rage Sliders
    public Slider playerHealthSlider;
    public Slider enemyHealthSlider;
    public Slider playerRageSlider;
    public Slider enemyRageSlider;

    // Timer & Round Text
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI roundNumberText; // NEW: Round number text

    // Split Damage Info Text
    public TextMeshProUGUI rockDamageText;
    public TextMeshProUGUI paperDamageText;
    public TextMeshProUGUI scissorsDamageText;

    // NEW: Replaced the old container with direct button references
    [Header("Player RPS Choice (Buttons)")]
    [Tooltip("Drag the Player's Rock Button GameObject here")]
    public GameObject playerRockButton;
    [Tooltip("Drag the Player's Paper Button GameObject here")]
    public GameObject playerPaperButton;
    [Tooltip("Drag the Player's Scissors Button GameObject here")]
    public GameObject playerScissorsButton;

    // Feedback UI
    public TextMeshProUGUI playerChoiceText;
    public TextMeshProUGUI enemyChoiceText;
    public TextMeshProUGUI winnerText;
    
    // Game Over Panel
    [Header("Game Over Panels")]
    public GameObject playerWinPanel; // NEW
    public GameObject playerLosePanel; // NEW
    
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
    
    // Enemy Rage Card UI
    [Header("Enemy Rage Card UI")]
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
    public TextMeshProUGUI playerDamageText;
    public TextMeshProUGUI enemyDamageText;
    
    // ---
    // NEW: Enemy Special Move Indicators (Replaced Old System)
    // ---
    [Header("Enemy Special Move Indicators")]
    [Header("Enemy Special UI (Attack)")]
    public GameObject enemyAttackIcon;
    public GameObject enemyAttackCooldownIcon;
    public TextMeshProUGUI enemyAttackCooldownText;

    [Header("Enemy Special UI (Defense)")]
    public GameObject enemyDefenseIcon;
    public GameObject enemyDefenseCooldownIcon;
    public TextMeshProUGUI enemyDefenseCooldownText;

    [Header("Enemy Special UI (Boost Rage)")]
    public GameObject enemyBoostIcon;
    public GameObject enemyBoostCooldownIcon;
    public TextMeshProUGUI enemyBoostCooldownText;

    [Header("Enemy Special UI (Heal)")]
    public GameObject enemyHealIcon;
    public GameObject enemyHealCooldownIcon;
    public TextMeshProUGUI enemyHealCooldownText;
    // ---
    
    [Header("Enemy RPS Choice Indicators")]
    [Tooltip("Image/Icon to show when enemy chooses Rock")]
    public GameObject enemyRockIndicator;
    [Tooltip("Image/Icon to show when enemy chooses Paper")]
    public GameObject enemyPaperIndicator;
    [Tooltip("Image/Icon to show when enemy chooses Scissors")]
    public GameObject enemyScissorsIndicator;
    
    // NEW: Hit Effect UI
    [Header("Hit Effect UI")]
    [Tooltip("The GameObject with the Hammer's Animator")]
    public GameObject hammerImageObject;
    [Tooltip("The impact effect image for the Player")]
    public GameObject playerImpactImage;
    [Tooltip("The impact effect image for the Enemy")]
    public GameObject enemyImpactImage;
    
    [Header("Audio")]
    [Tooltip("The AudioSource to play game sounds")]
    public AudioSource gameAudioSource;
    [Tooltip("The 'bonk' sound effect")]
    public AudioClip bonkSoundClip;
    [Tooltip("The audio clip to play when the player wins")]
    public AudioClip winSoundClip;
    [Tooltip("The audio clip to play when the player loses")]
    public AudioClip loseSoundClip;
    
    [Header("Animation References")]
    public Animator playerHealthAnimator;
    public Animator enemyHealthAnimator;
    public Animator playerRageAnimator;
    public Animator enemyRageAnimator;


    // --- Private Game State Variables ---
    private Move playerMove;
    private Move enemyMove;
    private SpecialMove playerSpecialMove;
    private SpecialMove enemySpecialMove;
    
    private int playerHealth;
    private int enemyHealth;
    private int playerRage;
    private int enemyRage;
    
    private int roundNumber = 0; // NEW: Round counter

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
    private bool playerHasDoubleDamage = false;
    private bool playerHasIgnoreDefense = false;
    private int  playerDamageSacrifice = 0;
    private bool playerHasBlockAllDamage = false;
    private bool playerHasIgnoreAttack = false;
    private bool playerHealsOnWin = false;
    
    private bool enemyHasDoubleDamage = false;
    private bool enemyHasIgnoreDefense = false;
    private int  enemyDamageSacrifice = 0;
    private bool enemyHasBlockAllDamage = false;
    private bool enemyHasIgnoreAttack = false;
    private bool enemyHealsOnWin = false;
    
    // Animation Trigger Name
    private const string SHAKE_TRIGGER = "Trigger";
    // NEW: Hit Animation Triggers
    private const string PLAYER_HIT_TRIGGER = "PlayerHit";
    private const string ENEMY_HIT_TRIGGER = "EnemyHit";

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
        
        // Hook up the 3 player RPS buttons
        if (playerRockButton != null) playerRockButton.GetComponent<Button>().onClick.AddListener(OnPlayerChoseRock);
        if (playerPaperButton != null) playerPaperButton.GetComponent<Button>().onClick.AddListener(OnPlayerChosePaper);
        if (playerScissorsButton != null) playerScissorsButton.GetComponent<Button>().onClick.AddListener(OnPlayerChoseScissors);
    }

    void InitializeGame()
    {
        isGameOver = false;
        playerHealth = startingHealth;
        enemyHealth = startingHealth;
        playerRage = 0;
        enemyRage = 0;
        roundNumber = 0; // NEW: Reset round number

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
        if (rageCardPanel != null) rageCardPanel.SetActive(false);
        if (enemyRageCardPanel != null) enemyRageCardPanel.SetActive(false);
        
        // NEW: Hide both game over panels
        if (playerWinPanel != null) playerWinPanel.SetActive(false);
        if (playerLosePanel != null) playerLosePanel.SetActive(false);
        
        if (playerDamageText != null) playerDamageText.gameObject.SetActive(false);
        if (enemyDamageText != null) enemyDamageText.gameObject.SetActive(false);
        
        // NEW: Hide hit effect UI
        if (hammerImageObject != null) hammerImageObject.SetActive(false);
        if (playerImpactImage != null) playerImpactImage.SetActive(false);
        if (enemyImpactImage != null) enemyImpactImage.SetActive(false);
        
        HideAllEnemyRPSIndicators(); 
        UpdateEnemySpecialMoveUI(); // Set enemy special UI to initial state
        
        HideAllPlayerRageCards();
        HideAllEnemyRageCards(); 
        
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
        // NEW: Increment and display round number
        roundNumber++;
        if (roundNumberText != null) roundNumberText.text = $"Round: {roundNumber}";
        
        // 1. Reset all temporary round effects
        ResetRoundEffects();
        
        // 2. Check for Player Rage Event
        if (playerRage >= maxRage)
        {
            yield return StartCoroutine(ShowPlayerRageCards());
        }
        
        // 3. Check for Enemy Rage Event
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
        ShowAllPlayerRPSButtons(); 
        UpdateSpecialMoveUI(); 
        UpdateEnemySpecialMoveUI();
        
        // Make sure all text objects are active at round start
        ToggleAllSpecialUI(true);
        ToggleAllEnemySpecialUI(true);
        
        ShowAllEnemyRPSIndicators(); 
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
    
    // This function is called by the RPS buttons to lock in choices
    private void LockInChoicesAndReveal(Move chosenPlayerMove)
    {
        if (!isRoundActive || isGameOver) return;
        
        isRoundActive = false; // Stop the timer
        
        // --- 1. Lock in ALL choices ---
        playerMove = chosenPlayerMove;
        
        enemySpecialMove = GetEnemySpecialMove(); // <-- AI UPGRADE
        
        // -----------------------------------------------------------------
        // AI UPGRADE: This line is changed to pass the special move
        // -----------------------------------------------------------------
        enemyMove = GetEnemyMove(enemySpecialMove); // <-- AI UPGRADE
        
        // --- 2. Hide Player Special Move controls & Show Enemy's choice ---
        // Player lock-in is handled by OnPlayerChose... functions
        LockInEnemySpecialMove(enemySpecialMove); // Show only the enemy's chosen move

        // --- 3. REVEAL RPS CHOICES ---
        
        // Show RPS indicators
        ShowPlayerRPSIndicator(playerMove);
        ShowEnemyRPSIndicator(enemyMove);
        
        // --- 4. Start the Battle Coroutine ---
        StartCoroutine(ResolveBattleAfterDelay());
    }

    // This Coroutine contains the *actual* battle logic
    private IEnumerator ResolveBattleAfterDelay()
    {
        // --- 1. Wait for the reveal animation ---
        yield return new WaitForSeconds(rpsRevealDuration);

        // --- 1.5. Hide ALL Special UI (now that battle starts) ---
        ToggleAllSpecialUI(false); 
        ToggleAllEnemySpecialUI(false); 
        
        // --- 2. Apply Instant Effects & Set Cooldowns ---
        ApplyAndSetCooldowns();

        // --- 3. Determine RPS Winner ---
        string winner = DetermineWinner(playerMove, enemyMove);

        // --- 4. Calculate Damage & Rage (Battle) ---
        if (winner == "Player")
        {
            // Player Won
            int baseDamage = GetDamageAmount(playerMove);
            int bonusDamage = (playerSpecialMove == SpecialMove.Attack && !enemyHasIgnoreAttack) ? 2 : 0; 
            int damageReduction = (enemySpecialMove == SpecialMove.Defense && !playerHasIgnoreDefense) ? 2 : 0; 
            int sacrificeDamage = playerDamageSacrifice;
            
            int totalDamage = Mathf.Max(0, baseDamage + bonusDamage - damageReduction + sacrificeDamage);
            
            if (playerHealsOnWin)
            {
                int heal = GetDamageAmount(playerMove);
                playerHealth = Mathf.Min(startingHealth, playerHealth + heal);
                SafeSetTrigger(playerHealthAnimator, SHAKE_TRIGGER); 
                Debug.Log($"Player converted win to {heal} heal!");
            }
            else
            {
                // Normal win
                if (playerHasDoubleDamage) totalDamage *= 2; 
                if (enemyHasBlockAllDamage) totalDamage = 0; 
                
                enemyHealth -= totalDamage;
                if (totalDamage > 0) {
                    SafeSetTrigger(enemyHealthAnimator, SHAKE_TRIGGER); 
                    IncreaseRage(ref enemyRage, totalDamage, false);
                    StartCoroutine(ShowHitEffect(false)); // NEW: false = enemy got hit
                    StartCoroutine(ShowDamageText(enemyDamageText, totalDamage)); 
                }
            }
        }
        else if (winner == "Enemy")
        {
            // Enemy Won
            int baseDamage = GetDamageAmount(enemyMove);
            int bonusDamage = (enemySpecialMove == SpecialMove.Attack && !playerHasIgnoreAttack) ? 2 : 0; 
            int damageReduction = (playerSpecialMove == SpecialMove.Defense && !enemyHasIgnoreDefense) ? 2 : 0; 
            int sacrificeDamage = enemyDamageSacrifice;
            
            int totalDamage = Mathf.Max(0, baseDamage + bonusDamage - damageReduction + sacrificeDamage);

            if (enemyHealsOnWin)
            {
                int heal = GetDamageAmount(enemyMove);
                enemyHealth = Mathf.Min(startingHealth, enemyHealth + heal);
                SafeSetTrigger(enemyHealthAnimator, SHAKE_TRIGGER); 
                Debug.Log($"Enemy converted win to {heal} heal!");
            }
            else
            {
                // Normal win
                if (enemyHasDoubleDamage) totalDamage *= 2; 
                if (playerHasBlockAllDamage) totalDamage = 0; 
                
                playerHealth -= totalDamage;
                if (totalDamage > 0) {
                    SafeSetTrigger(playerHealthAnimator, SHAKE_TRIGGER); 
                    IncreaseRage(ref playerRage, totalDamage, true);
                    StartCoroutine(ShowHitEffect(true)); // NEW: true = player got hit
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
            // NEW: Show the correct win/lose panel
            bool playerWonMatch = (playerHealth > 0);
            StartCoroutine(ShowGameOverPanel(gameOverPanelDelay, playerWonMatch));
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
            SafeSetTrigger(playerHealthAnimator, SHAKE_TRIGGER); 
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
            SafeSetTrigger(enemyHealthAnimator, SHAKE_TRIGGER); 
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

    // RPS buttons NOW call the new reveal function
    public void OnPlayerChoseRock() { LockInChoicesAndReveal(Move.Rock); }
    public void OnPlayerChosePaper() { LockInChoicesAndReveal(Move.Paper); }
    public void OnPlayerChoseScissors() { LockInChoicesAndReveal(Move.Scissors); }

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
    
    private void SafeSetTrigger(Animator animator, string triggerName)
    {
        if (animator != null && animator.gameObject.activeInHierarchy)
        {
            animator.SetTrigger(triggerName);
        }
    }
    
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

    // -----------------------------------------------------------------
    // REVERTED: This is your original Level 1 code for the UI
    // -----------------------------------------------------------------
    private void LockInSpecialMove(SpecialMove chosenMove)
    {
        if (chosenMove != SpecialMove.Attack)
        {
            if (attackButtonObject != null) attackButtonObject.SetActive(false);
            if (attackCooldownImage != null)
            {
                // Show cooldown image if it's on CD (attackCooldown > 0)
                // OR if it was available (attackCooldown == 0) to show the greyed-out icon
                attackCooldownImage.SetActive(true); 
            }
            if (attackCooldownText != null) attackCooldownText.gameObject.SetActive(false); 
        }

        if (chosenMove != SpecialMove.Defense)
        {
            if (defenseButtonObject != null) defenseButtonObject.SetActive(false);
            if (defenseCooldownImage != null)
            {
                defenseCooldownImage.SetActive(true); // Show greyed-out/cooldown icon
            }
            if (defenseCooldownText != null) defenseCooldownText.gameObject.SetActive(false);
        }

        if (chosenMove != SpecialMove.BoostRage)
        {
            if (boostButtonObject != null) boostButtonObject.SetActive(false);
            if (boostCooldownImage != null)
            {
                boostCooldownImage.SetActive(true); // Show greyed-out/cooldown icon
            }
            if (boostCooldownText != null) boostCooldownText.gameObject.SetActive(false);
        }

        if (chosenMove != SpecialMove.Heal)
        {
            if (healButtonObject != null) healButtonObject.SetActive(false);
            if (healCooldownImage != null)
            {
                healCooldownImage.SetActive(true); // Show greyed-out/cooldown icon
            }
            if (healCooldownText != null) healCooldownText.gameObject.SetActive(false);
        }
    }
    
    private void ShowAllPlayerRPSButtons(bool animate = false)
    {
        if (playerRockButton != null) playerRockButton.SetActive(true);
        if (playerPaperButton != null) playerPaperButton.SetActive(true);
        if (playerScissorsButton != null) playerScissorsButton.SetActive(true);

        if (animate)
        {
            SafeSetTrigger(playerRockButton.GetComponent<Animator>(), SHAKE_TRIGGER);
            SafeSetTrigger(playerPaperButton.GetComponent<Animator>(), SHAKE_TRIGGER);
            SafeSetTrigger(playerScissorsButton.GetComponent<Animator>(), SHAKE_TRIGGER);
        }
    }
    
    private void HideAllPlayerRPSButtons()
    {
        if (playerRockButton != null) playerRockButton.SetActive(false);
        if (playerPaperButton != null) playerPaperButton.SetActive(false);
        if (playerScissorsButton != null) playerScissorsButton.SetActive(false);
    }

    void InstantLoss()
    {
        Debug.Log("Player ran out of time! Instant Loss.");
        
        HideAllPlayerRPSButtons();
        ToggleAllSpecialUI(false); 
        ToggleAllEnemySpecialUI(false); 
        
        HideAllEnemyRPSIndicators(); 
        
        if (rageCardPanel != null) rageCardPanel.SetActive(false);
        if (enemyRageCardPanel != null) enemyRageCardPanel.SetActive(false);

        isGameOver = true;
        playerHealth = 0; 
        SafeSetTrigger(playerHealthAnimator, SHAKE_TRIGGER); 
        UpdateHealthAndRageUI(); 
        winnerText.text = "Out of time! You lose!";
        winnerText.color = Color.red;
        StartCoroutine(ShowGameOverPanel(gameOverPanelDelay, false)); // NEW: false = player lost
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

    // -----------------------------------------------------------------
    // AI UPGRADE: This function is replaced
    // -----------------------------------------------------------------
    private SpecialMove GetEnemySpecialMove()
    {
        // --- Stage 2 AI Logic ---
        List<SpecialMove> availableMoves = new List<SpecialMove>();
        if (enemyAttackCooldown == 0) availableMoves.Add(SpecialMove.Attack);
        if (enemyDefenseCooldown == 0) availableMoves.Add(SpecialMove.Defense);
        if (enemyBoostCooldown == 0) availableMoves.Add(SpecialMove.BoostRage);
        if (enemyHealCooldown == 0) availableMoves.Add(SpecialMove.Heal);

        // --- Priority Checks ---
        
        // 1. FINISHER: If player is very low (<= 30%) and Attack is ready, 80% chance to attack.
        if (playerHealth <= (startingHealth * 0.3) && availableMoves.Contains(SpecialMove.Attack))
        {
            if (random.Next(0, 100) < 80) // 80% chance
            {
                Debug.Log("AI: Player is weak! Attacking!");
                return SpecialMove.Attack;
            }
        }

        // 2. PANIC HEAL: If AI is low (<= 40%) and Heal is ready, 90% chance to heal.
        if (enemyHealth <= (startingHealth * 0.4) && availableMoves.Contains(SpecialMove.Heal))
        {
            if (random.Next(0, 100) < 90) // 90% chance
            {
                Debug.Log("AI: Low health! Healing!");
                return SpecialMove.Heal;
            }
        }

        // 3. PANIC DEFENSE: If AI is low (<= 40%), Heal is on CD, but Defense is ready, 70% chance to defend.
        if (enemyHealth <= (startingHealth * 0.4) && 
            !availableMoves.Contains(SpecialMove.Heal) && 
            availableMoves.Contains(SpecialMove.Defense))
        {
            if (random.Next(0, 100) < 70) // 70% chance
            {
                Debug.Log("AI: Low health, no heal! Defending!");
                return SpecialMove.Defense;
            }
        }
        
        // 4. STRATEGIC BOOST: If health is decent (>= 70%) and Boost is ready, 50% chance to boost.
        if (enemyHealth >= (startingHealth * 0.7) && availableMoves.Contains(SpecialMove.BoostRage))
        {
            if (random.Next(0, 100) < 50) // 50% chance
            {
                Debug.Log("AI: Feeling good! Boosting rage!");
                return SpecialMove.BoostRage;
            }
        }

        // 5. DEFAULT/RANDOM: If no priority move was chosen, pick from available moves + "None"
        availableMoves.Add(SpecialMove.None);
        availableMoves.Add(SpecialMove.None); // Bias towards doing nothing
        
        int randomIndex = random.Next(0, availableMoves.Count);
        SpecialMove chosenMove = availableMoves[randomIndex];
        
        Debug.Log($"AI: Default random choice: {chosenMove}");
        return chosenMove;
    }

    // -----------------------------------------------------------------
    // AI UPGRADE: This function is replaced and now takes an argument
    // -----------------------------------------------------------------
    private Move GetEnemyMove(SpecialMove chosenSpecial)
    {
        // --- Stage 2 AI Logic ---
        
        // Check for aggressive "must-win" states
        bool isAggressive = (chosenSpecial == SpecialMove.Attack) || 
                            enemyHasDoubleDamage || 
                            (enemyDamageSacrifice > 0);
                            
        // Check for defensive "don't-care" states
        bool isDefensive = (chosenSpecial == SpecialMove.Defense) || 
                           enemyHasBlockAllDamage || 
                           enemyHealsOnWin;

        // 1. AGGRESSIVE STRATEGY: Pick the move with the highest damage.
        if (isAggressive)
        {
            Debug.Log("AI: Aggressive state! Picking highest damage move.");
            if (rockDamage >= paperDamage && rockDamage >= scissorsDamage)
            {
                return Move.Rock; // Rock is strongest or tied for strongest
            }
            if (paperDamage >= rockDamage && paperDamage >= scissorsDamage)
            {
                return Move.Paper; // Paper is strongest
            }
            return Move.Scissors; // Scissors must be strongest
        }

        // 2. DEFENSIVE STRATEGY: Pick the move with the lowest damage (to save high-damage moves).
        if (isDefensive)
        {
            Debug.Log("AI: Defensive state! Picking lowest damage move.");
            if (rockDamage <= paperDamage && rockDamage <= scissorsDamage)
            {
                return Move.Rock; // Rock is weakest or tied for weakest
            }
            if (paperDamage <= rockDamage && paperDamage <= scissorsDamage)
            {
                return Move.Paper; // Paper is weakest
            }
            return Move.Scissors; // Scissors must be weakest
        }

        // 3. NEUTRAL STRATEGY (Heal, Boost, None): 
        // 70% chance to pick highest damage, 30% chance to pick random.
        // This makes the AI *generally* smart but not perfectly predictable.
        if (random.Next(0, 100) < 70) // 70% chance to be smart
        {
            Debug.Log("AI: Neutral state! 70% chance for high damage.");
            if (rockDamage >= paperDamage && rockDamage >= scissorsDamage)
            {
                return Move.Rock;
            }
            if (paperDamage >= rockDamage && paperDamage >= scissorsDamage)
            {
                return Move.Paper;
            }
            return Move.Scissors;
        }
        
        // 4. FALLBACK: 30% chance for pure random (the original "Stage 1" logic)
        Debug.Log("AI: Neutral state! 30% chance for random.");
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
        
        if (isPlayer)
        {
            SafeSetTrigger(playerRageAnimator, SHAKE_TRIGGER); 
        }
        else
        {
            SafeSetTrigger(enemyRageAnimator, SHAKE_TRIGGER); 
        }
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
        
        // Special UI stays active
        
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
                    SafeSetTrigger(playerHealthAnimator, SHAKE_TRIGGER); 
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
    
    private IEnumerator TriggerEnemyRageCard()
    {
        isRoundActive = false; // Stop timer
        
        // Special UI stays active
        
        int offeredOffensiveID = random.Next(1, 5);
        int offeredDefensiveID = random.Next(1, 5);
        
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
        
        HideAllEnemyRageCards();
        
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
        
        if (enemyRageCardPanel != null) enemyRageCardPanel.SetActive(true);
        ApplyEnemyRageEffect(chosenCategory, chosenCardID);
        
        enemyRage = 0;
        UpdateHealthAndRageUI();
        
        yield return new WaitForSeconds(enemyRageCardRevealDuration);
        
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
                    SafeSetTrigger(enemyHealthAnimator, SHAKE_TRIGGER); 
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
    
    // This function now also hides the text of un-chosen moves
    // and correctly shows the "greyed out" image for available-but-not-chosen moves.
    private void LockInEnemySpecialMove(SpecialMove chosenMove)
    {
        // Hide non-chosen moves. The chosen one is already active.
        if (chosenMove != SpecialMove.Attack)
        {
            if (enemyAttackIcon != null) enemyAttackIcon.SetActive(false);
            if (enemyAttackCooldownIcon != null)
            {
                // Show cooldown image if it's on CD (enemyAttackCooldown > 0)
                // OR if it was available (enemyAttackCooldown == 0) to show the greyed-out icon
                enemyAttackCooldownIcon.SetActive(true);
            }
            if (enemyAttackCooldownText != null) enemyAttackCooldownText.gameObject.SetActive(false);
        }
        if (chosenMove != SpecialMove.Defense)
        {
            if (enemyDefenseIcon != null) enemyDefenseIcon.SetActive(false);
            if (enemyDefenseCooldownIcon != null)
            {
                enemyDefenseCooldownIcon.SetActive(true); // Show greyed-out/cooldown icon
            }
            if (enemyDefenseCooldownText != null) enemyDefenseCooldownText.gameObject.SetActive(false);
        }
        if (chosenMove != SpecialMove.BoostRage)
        {
            if (enemyBoostIcon != null) enemyBoostIcon.SetActive(false);
            if (enemyBoostCooldownIcon != null)
            {
                enemyBoostCooldownIcon.SetActive(true); // Show greyed-out/cooldown icon
            }
            if (enemyBoostCooldownText != null) enemyBoostCooldownText.gameObject.SetActive(false);
        }
        if (chosenMove != SpecialMove.Heal)
        {
 
            if (enemyHealIcon != null) enemyHealIcon.SetActive(false);
            if (enemyHealCooldownIcon != null)
            {
                enemyHealCooldownIcon.SetActive(true); // Show greyed-out/cooldown icon
            }
            if (enemyHealCooldownText != null) enemyHealCooldownText.gameObject.SetActive(false);
        }
    }
    
    private void HideAllEnemyRPSIndicators()
    {
        if (enemyRockIndicator != null) enemyRockIndicator.SetActive(false);
        if (enemyPaperIndicator != null) enemyPaperIndicator.SetActive(false);
        if (enemyScissorsIndicator != null) enemyScissorsIndicator.SetActive(false);
    }
    
    private void ShowAllEnemyRPSIndicators(bool animate = false)
    {
        // Set them active first
        if (enemyRockIndicator != null) enemyRockIndicator.SetActive(true);
        if (enemyPaperIndicator != null) enemyPaperIndicator.SetActive(true);
        if (enemyScissorsIndicator != null) enemyScissorsIndicator.SetActive(true);

        if (animate)
        {
            // Trigger the "Animate In" (which is your "Trigger")
            SafeSetTrigger(enemyRockIndicator.GetComponent<Animator>(), SHAKE_TRIGGER);
            SafeSetTrigger(enemyPaperIndicator.GetComponent<Animator>(), SHAKE_TRIGGER);
            SafeSetTrigger(enemyScissorsIndicator.GetComponent<Animator>(), SHAKE_TRIGGER);
        }
    }
    
    // Triggers animations on the *other* two buttons
    private void ShowPlayerRPSIndicator(Move move)
    {
        // This will trigger the "animate out" on the two buttons NOT chosen
        if (move != Move.Rock) SafeSetTrigger(playerRockButton.GetComponent<Animator>(), SHAKE_TRIGGER);
        if (move != Move.Paper) SafeSetTrigger(playerPaperButton.GetComponent<Animator>(), SHAKE_TRIGGER);
        if (move != Move.Scissors) SafeSetTrigger(playerScissorsButton.GetComponent<Animator>(), SHAKE_TRIGGER);
    }
    
    // Triggers animations on the *other* two images
    private void ShowEnemyRPSIndicator(Move move)
    {
        // This will trigger the "animate out" on the two images NOT chosen
        if (move != Move.Rock) SafeSetTrigger(enemyRockIndicator.GetComponent<Animator>(), SHAKE_TRIGGER);
        if (move != Move.Paper) SafeSetTrigger(enemyPaperIndicator.GetComponent<Animator>(), SHAKE_TRIGGER);
        if (move != Move.Scissors) SafeSetTrigger(enemyScissorsIndicator.GetComponent<Animator>(), SHAKE_TRIGGER);
    }
    
    private IEnumerator ShowDamageText(TextMeshProUGUI textElement, int damage)
    {
        if (textElement == null || damage <= 0)
        {
            yield break; 
        }
        
        textElement.text = $"-{damage} HP";
        textElement.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(damageTextDuration);
        
        textElement.gameObject.SetActive(false);
    }
    
    // This function now toggles the text's GameObject
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

        // Also toggle the text GameObjects
        if (attackCooldownText != null) attackCooldownText.gameObject.SetActive(show);
        if (defenseCooldownText != null) defenseCooldownText.gameObject.SetActive(show);
        if (boostCooldownText != null) boostCooldownText.gameObject.SetActive(show);
        if (healCooldownText != null) healCooldownText.gameObject.SetActive(show);
    }

    // This function now toggles the text's GameObject
    private void ToggleAllEnemySpecialUI(bool show)
    {
        if (enemyAttackIcon != null) enemyAttackIcon.SetActive(show);
        if (enemyAttackCooldownIcon != null) enemyAttackCooldownIcon.SetActive(show);
        if (enemyDefenseIcon != null) enemyDefenseIcon.SetActive(show);
        if (enemyDefenseCooldownIcon != null) enemyDefenseCooldownIcon.SetActive(show);
        if (enemyBoostIcon != null) enemyBoostIcon.SetActive(show);
        if (enemyBoostCooldownIcon != null) enemyBoostCooldownIcon.SetActive(show);
        if (enemyHealIcon != null) enemyHealIcon.SetActive(show);
        if (enemyHealCooldownIcon != null) enemyHealCooldownIcon.SetActive(show);

        // Also toggle the text GameObjects
        if (enemyAttackCooldownText != null) enemyAttackCooldownText.gameObject.SetActive(show);
        if (enemyDefenseCooldownText != null) enemyDefenseCooldownText.gameObject.SetActive(show);
        if (enemyBoostCooldownText != null) enemyBoostCooldownText.gameObject.SetActive(show);
        if (enemyHealCooldownText != null) enemyHealCooldownText.gameObject.SetActive(show);
    }

    // -----------------------------------------------------------------
    // REVERTED: This is your original Level 1 code for the UI
    // -----------------------------------------------------------------
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

    // This function now shows "Ready" when CD is 0
    private void UpdateEnemySpecialMoveUI()
    {
        bool isAttackReady = (enemyAttackCooldown == 0);
        if (enemyAttackIcon != null) enemyAttackIcon.SetActive(isAttackReady);
        if (enemyAttackCooldownIcon != null) enemyAttackCooldownIcon.SetActive(!isAttackReady);
        if (enemyAttackCooldownText != null) enemyAttackCooldownText.text = isAttackReady ? "Ready" : $"CD: {enemyAttackCooldown} Rnd";

        bool isDefenseReady = (enemyDefenseCooldown == 0);
        if (enemyDefenseIcon != null) enemyDefenseIcon.SetActive(isDefenseReady);
        if (enemyDefenseCooldownIcon != null) enemyDefenseCooldownIcon.SetActive(!isDefenseReady);
        if (enemyDefenseCooldownText != null) enemyDefenseCooldownText.text = isDefenseReady ? "Ready" : $"CD: {enemyDefenseCooldown} Rnd";

        bool isBoostReady = (enemyBoostCooldown == 0);
        if (enemyBoostIcon != null) enemyBoostIcon.SetActive(isBoostReady);
        if (enemyBoostCooldownIcon != null) enemyBoostCooldownIcon.SetActive(!isBoostReady);
        if (enemyBoostCooldownText != null) enemyBoostCooldownText.text = isBoostReady ? "Ready" : $"CD: {enemyBoostCooldown} Rnd";

        bool isHealReady = (enemyHealCooldown == 0);
        if (enemyHealIcon != null) enemyHealIcon.SetActive(isHealReady);
        if (enemyHealCooldownIcon != null) enemyHealCooldownIcon.SetActive(!isHealReady);
        if (enemyHealCooldownText != null) enemyHealCooldownText.text = isHealReady ? "Ready" : $"CD: {enemyHealCooldown} Rnd";
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

    // Replaced BonkEffect with this
    private IEnumerator ShowHitEffect(bool playerGotHit)
    {
        // NEW: Play bonk sound
        if (gameAudioSource != null && bonkSoundClip != null)
        {
            gameAudioSource.PlayOneShot(bonkSoundClip);
        }
        
        if (hammerImageObject == null)
        {
            yield break; // Don't do anything else if hammer isn't set
        }

        hammerImageObject.SetActive(true);
        Animator hammerAnimator = hammerImageObject.GetComponent<Animator>();

        if (playerGotHit)
        {
            if (playerImpactImage != null) playerImpactImage.SetActive(true);
            if (hammerAnimator != null) SafeSetTrigger(hammerAnimator, PLAYER_HIT_TRIGGER);
        }
        else // Enemy got hit
        {
            if (enemyImpactImage != null) enemyImpactImage.SetActive(true);
            if (hammerAnimator != null) SafeSetTrigger(hammerAnimator, ENEMY_HIT_TRIGGER);
        }

        yield return new WaitForSeconds(bonkEffectDuration); // Re-use the duration
        
        // Hide all effects
        hammerImageObject.SetActive(false);
        if (playerImpactImage != null) playerImpactImage.SetActive(false);
        if (enemyImpactImage != null) enemyImpactImage.SetActive(false);
    }


    private IEnumerator RoundResetDelay()
    {
        yield return new WaitForSeconds(delayBeforeNextRound);
        
        // Call the Coroutine to start the next round
        StartCoroutine(StartNewRound());
    }
    
    private bool CheckForGameOver()
    {
        if (isGameOver) return true;

        if (playerHealth <= 0 || enemyHealth <= 0)
        {
            isGameOver = true;
            HideAllPlayerRPSButtons();
            
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

    // NEW: This function now takes a boolean to show the correct panel
    private IEnumerator ShowGameOverPanel(float delay, bool didPlayerWin)
    {
        yield return new WaitForSeconds(delay);
        
        if (didPlayerWin)
        {
            if (playerWinPanel != null) playerWinPanel.SetActive(true);
            
            // ADD THIS:
            if (gameAudioSource != null && winSoundClip != null)
            {
                gameAudioSource.PlayOneShot(winSoundClip);
            }
        }
        else
        {
            if (playerLosePanel != null) playerLosePanel.SetActive(true);
            
            // ADD THIS:
            if (gameAudioSource != null && loseSoundClip != null)
            {
                gameAudioSource.PlayOneShot(loseSoundClip);
            }
        }
    }
}