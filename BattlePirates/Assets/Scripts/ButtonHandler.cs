using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class ButtonHandler : MonoBehaviour
{
    public UIDocument UIDocument;
    private Dictionary<string, Action> _buttonActions;
    private VisualElement _help;
    private VisualElement _helpScreen;
    private VisualElement _settingsPanel;
    private VisualElement _powerUpPanel;
    private Button _settingsButton;
    private VisualElement _splashScreen;
    private VisualElement _turnExplanationScreen;
    private VisualElement _confirmationScreen;
    private Button _actualQuitButton;
    private Button _cancelButton;
    private VisualElement _confirmationScreenForfeit;
    private Button _actualForfeitButton;
    private VisualElement _victoryScreen;
    private VisualElement _portraitElement;
    private VisualElement _defeatScreen;
    private AIManager _aiManager;
    private Button _powerUpButton;
    private List<VisualElement> _captainPortraits;
    private List<VisualElement> _powerUpElements;
    private int _currentCaptainIndex;
    private int _currentPowerUpIndex = 0;
    private List<AttackManager.SpecialAttacks> _powerUps;
    private GameManager _gameManager;
    private AttackManager _attackManager;
    private string _selectedPower;
    void Awake()
    {
        if (GameObject.Find("AttackManager") != null)
        {
            _attackManager = GameObject.Find("AttackManager").GetComponent<AttackManager>();
        }

        if (GameObject.Find("GameManager")!= null)
        {
            _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
        var root = UIDocument.rootVisualElement;
        _buttonActions = new Dictionary<string, Action>
        {
            { "Play", Play },
            { "SettingsButton", ToggleSettings },
            { "CloseSettingsButton", CloseSettings },
            { "Quit", QuitGame },
            { "HelpButton", Help},
            { "CloseHelpButton", CloseHelp},
            { "Forfeit", Forfeit},
            { "Again", Again},
            { "MainMenu", MainMenu},
            { "SplashScreen", SplashScreen},
            { "CloseExplanation", TurnExplanation},
            { "ActualQuitButton", ActualQuit},
            { "Cancel", Cancel},
            { "CancelForfeit", CancelForfeit},
            { "ActualForfeitButton", ActualForfeit},
            { "SelectPowerUp", SelectPowerUp},
            { "PreviousPowerButton", PreviousPowerUp},
            { "NextPowerButton", NextPowerUp},
            { "ActivatePowerUpButton", DoPowerUp},
        };
        _portraitElement = root.Q<VisualElement>("CaptainPortrait");
        _confirmationScreen = root.Q<VisualElement>("ConfirmationScreen");
        _confirmationScreenForfeit = root.Q<VisualElement>("ConfirmationScreenForfeit");
        _settingsPanel = root.Q<VisualElement>("SettingsPanel");
        _powerUpPanel = root.Q<VisualElement>("PowerUpChoice");
        _helpScreen = root.Q<VisualElement>("HelpScreen");
        _splashScreen = root.Q<VisualElement>("SplashScreen");
        _turnExplanationScreen = root.Q<VisualElement>("TurnExplanation");
        _victoryScreen = root.Q<VisualElement>("VictoryScreen");
        _defeatScreen = root.Q<VisualElement>("DefeatScreen");
        _powerUps = new List<AttackManager.SpecialAttacks>
        {
            AttackManager.SpecialAttacks.Mist,
            AttackManager.SpecialAttacks.Coin,
            AttackManager.SpecialAttacks.Plus,
            AttackManager.SpecialAttacks.Dutchman
        };
        if (SceneManager.GetActiveScene().name == "PlanningPhase2")
        {
            
            _captainPortraits = root.Query<VisualElement>(name: "CaptainPortrait").ToList();
            _powerUpElements = root.Query<VisualElement>(name: "PowerUp").ToList();
            for (int i = 0; i < _captainPortraits.Count; i++)
            {
                _captainPortraits[i].style.display = i == 0 ? DisplayStyle.Flex : DisplayStyle.None;
            }
            for (int i = 0; i < _powerUpElements.Count; i++)
            {
                _powerUpElements[i].style.display = i == 0 ? DisplayStyle.Flex : DisplayStyle.None;
            }
            _gameManager.ChosenPowerUp = _powerUps[_currentPowerUpIndex];
        }

        if (SceneManager.GetActiveScene().name == "PlayingPhase2")
        {
            var gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            _selectedPower = gameManager.ChosenPowerUp.ToString();
            Texture2D portraitTexture = Resources.Load<Texture2D>("CaptainPortraits/" + _selectedPower);
            if (portraitTexture == null)
            {
                Debug.LogError($"❌ Kon geen portrait vinden voor power-up {_selectedPower} in Resources/CaptainPortraits/");
                return;
            }

            // Zet de afbeelding op de VisualElement background
            _portraitElement.style.backgroundImage = new StyleBackground(portraitTexture);
        }
        
        foreach (var kvp in _buttonActions)
        {
            Button button = root.Q<Button>(kvp.Key);
            if (_actualForfeitButton != null)
            {
                _confirmationScreenForfeit.style.display = DisplayStyle.None;
            }
            if (_helpScreen != null)
            {
                _confirmationScreen.style.display = DisplayStyle.None;
                _helpScreen.style.display = DisplayStyle.None;
            }

            if (_victoryScreen != null)
            {
                _victoryScreen.style.display = DisplayStyle.None;
            }

            if (_defeatScreen != null)
            {
                _defeatScreen.style.display = DisplayStyle.None;
            }
            if (button != null)
            {
                string buttonName = kvp.Key;
                button.clicked += () => OnButtonClicked(buttonName);
            }
            if (_settingsPanel != null)
            {
                _settingsPanel.style.display = DisplayStyle.None;
            }
            // else
            // {
            //     Debug.Log($"Button met naam '{kvp.Key}' niet gevonden!");
            // }
        }

        root.RegisterCallback<ClickEvent>(evt => SplashScreen() );
    }

    

    private void DoPowerUp()
    {
        Debug.Log("DoPowerUp() gestart...");

        if (_attackManager == null)
        {
            Debug.LogError("⚠️ _attackManager is NULL in DoPowerUp!");
            return;
        }

        var powerUp = _gameManager.ChosenPowerUp;
        _attackManager.DoSpecialAttack(powerUp);
    }

    void ToggleNextPowerUp()
    {
        if (_powerUpElements.Count == 0) return;
        if (_captainPortraits.Count == 0) return;
        _powerUpElements[_currentPowerUpIndex].style.display = DisplayStyle.None;
        _captainPortraits[_currentCaptainIndex].style.display = DisplayStyle.None;
        _currentPowerUpIndex = (_currentPowerUpIndex + 1) % _powerUpElements.Count;
        _currentCaptainIndex = (_currentCaptainIndex + 1) % _captainPortraits.Count;
        _powerUpElements[_currentPowerUpIndex].style.display = DisplayStyle.Flex;
        _captainPortraits[_currentCaptainIndex].style.display = DisplayStyle.Flex;
        _gameManager.ChosenPowerUp = _powerUps[_currentPowerUpIndex];
    }

    private AttackManager.SpecialAttacks GetCurrentPowerup()
    {
        if (_powerUps == null || _powerUps.Count == 0)
        {
            Debug.LogError("PowerUps lijst is niet geïnitialiseerd!");
            return AttackManager.SpecialAttacks.Mist; // fallback waarde
        }

        return _powerUps[_currentPowerUpIndex];
    }

    void TogglePreviousPowerUp()
    {
        if (_powerUpElements.Count == 0) return;
        if (_captainPortraits.Count == 0) return;
        _powerUpElements[_currentPowerUpIndex].style.display = DisplayStyle.None;
        _captainPortraits[_currentCaptainIndex].style.display = DisplayStyle.None;
        _currentPowerUpIndex = (_currentPowerUpIndex - 1 + _powerUpElements.Count) % _powerUpElements.Count;
        _currentCaptainIndex = (_currentCaptainIndex - 1 + _captainPortraits.Count) % _captainPortraits.Count;
        _powerUpElements[_currentPowerUpIndex].style.display = DisplayStyle.Flex;
        _captainPortraits[_currentCaptainIndex].style.display = DisplayStyle.Flex;
        _gameManager.ChosenPowerUp = _powerUps[_currentPowerUpIndex];
        Debug.Log("Start met powerup:" + GetCurrentPowerup());
    }

    private void NextPowerUp()
    {
        ToggleNextPowerUp();
    }

    private void PreviousPowerUp()
    {
        TogglePreviousPowerUp();
    }

    private void SelectPowerUp()
    {
        _gameManager.PowerUpChosen = true;
        _powerUpPanel.style.display = DisplayStyle.None;
    }

    private void CancelForfeit()
    {
        _confirmationScreenForfeit.style.display = DisplayStyle.None;
    }

    private void ActualForfeit()
    {
        Debug.Log("Forfeit button clicked. Checking for confirmation");
        _aiManager = GameObject.Find("AIManager").GetComponent<AIManager>();
        if (_aiManager != null)
        {
            _aiManager.RemoveShips();
        }
        SceneManager.LoadScene("DefeatScreen");
        
    }

    private void Cancel()
    {
        _confirmationScreen.style.display = DisplayStyle.None;
    }

    private void ActualQuit()
    {
        Debug.Log("Quit button clicked. Closing Game");
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    private void SplashScreen()
    {
        if (_splashScreen != null)
        {
            _splashScreen.style.display = DisplayStyle.None;
        }
    }private void TurnExplanation()
    {
        if (_turnExplanationScreen != null)
        {
            _turnExplanationScreen.style.display = DisplayStyle.None;
        }
    }

    private void OnButtonClicked(string buttonName)
    {
        if (_buttonActions.TryGetValue(buttonName, out var action))
        {
            action.Invoke();
        }
    }
    private void MainMenu()
    {
        Debug.Log("Main Menu Button clicked, returning to main menu");
        SceneManager.LoadScene("Main Menu");
    }

    private void Again()
    {
        Debug.Log("Again button clicked, returning to planning phase");
        SceneManager.LoadScene("PlanningPhase2");
    }

    private void Forfeit()
    {
        _confirmationScreenForfeit.style.display = DisplayStyle.Flex;
    }

    public void ShowVictoryScreen()
    {
        GameObject.Find("AttackManager").gameObject.GetComponent<AttackManager>().enabled = false;
        _victoryScreen.style.display = DisplayStyle.Flex;
    }

    public void ShowDefeatScreen()
    {
        GameObject.Find("AttackManager").gameObject.GetComponent<AttackManager>().enabled = false;
        _defeatScreen.style.display = DisplayStyle.Flex;
    }
    private void Help()
    {
        Debug.Log("Help Button clicked. Opening Help Box");
        if (_helpScreen.style.display == DisplayStyle.None)
            _helpScreen.style.display = DisplayStyle.Flex;
    }

    private void CloseHelp()
    {
        _helpScreen.style.display = DisplayStyle.None;
    }
    private void QuitGame()
    {
        if (_confirmationScreen.style.display == DisplayStyle.None)
        {
            _confirmationScreen.style.display = DisplayStyle.Flex;
        }
    }
    private void Play()
    {
        Debug.Log("Play button clicked. Going to next scene");
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("Geen volgende scene beschikbaar. Controleer of alle scenes in de Build Settings zijn toegevoegd.");
        }
    }
    private void ToggleSettings()
    {
        if (_settingsPanel == null)
        {
            Debug.LogError("SettingsPanel niet Beschikbaar");
            return;
        }
    
        if (_settingsPanel.style.display == DisplayStyle.None)
        {
            _settingsPanel.style.display = DisplayStyle.Flex;
        }
        else
        {
            _settingsPanel.style.display = DisplayStyle.None;
        }
    }

    private void CloseSettings()
    {
        _settingsPanel.style.display = DisplayStyle.None;
    }
}
