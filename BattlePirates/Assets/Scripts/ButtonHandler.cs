using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour
{
    public UIDocument uiDocument;
    private Dictionary<string, Action> _ButtonActions;
    private VisualElement _Help;
    private VisualElement _HelpScreen;
    private VisualElement _SettingsPanel;
    private VisualElement _PowerUpPanel;
    private Button _SettingsButton;
    private VisualElement _SplashScreen;
    private VisualElement _ConfirmationScreen;
    private Button _ActualQuitButton;
    private Button _CancelButton;
    private VisualElement _ConfirmationScreenForfeit;
    private Button _ActualForfeitButton;
    private VisualElement _VictoryScreen;
    private AIManager _aiManager;
    private Button _powerUpButton;
    private List<VisualElement> _captainPortraits;
    private List<VisualElement> _powerUpElements;
    private int _currentCaptainIndex;
    private int _currentPowerUpIndex = 0;
    private List<AttackManager.SpecialAttacks> _powerUps;
    private GameManager _gameManager;
    private AttackManager _attackManager;
    void Awake()
    {
        if (GameObject.Find("AttackManager") != null)
        {
            _attackManager = GameObject.Find("AttackManager").GetComponent<AttackManager>();
        }
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        var root = uiDocument.rootVisualElement;
        _ButtonActions = new Dictionary<string, Action>
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
            { "ActualQuitButton", ActualQuit},
            { "Cancel", Cancel},
            { "CancelForfeit", CancelForfeit},
            { "ActualForfeitButton", ActualForfeit},
            { "SelectPowerUp", SelectPowerUp},
            { "PreviousPowerButton", PreviousPowerUp},
            { "NextPowerButton", NextPowerUp},
            { "ActivatePowerUpButton", DoPowerUp},
        };
        
        _ConfirmationScreen = root.Q<VisualElement>("ConfirmationScreen");
        _ConfirmationScreenForfeit = root.Q<VisualElement>("ConfirmationScreenForfeit");
        _SettingsPanel = root.Q<VisualElement>("SettingsPanel");
        _PowerUpPanel = root.Q<VisualElement>("PowerUpChoice");
        _HelpScreen = root.Q<VisualElement>("HelpScreen");
        _SplashScreen = root.Q<VisualElement>("SplashScreen");
        _VictoryScreen = root.Q<VisualElement>("VictoryScreen");
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
        
        foreach (var kvp in _ButtonActions)
        {
            Button button = root.Q<Button>(kvp.Key);
            if (_ActualForfeitButton != null)
            {
                _ConfirmationScreenForfeit.style.display = DisplayStyle.None;
            }
            if (_HelpScreen != null)
            {
                _ConfirmationScreen.style.display = DisplayStyle.None;
                _HelpScreen.style.display = DisplayStyle.None;
            }

            if (_VictoryScreen != null)
            {
                _VictoryScreen.style.display = DisplayStyle.None;
            }
            if (button != null)
            {
                string buttonName = kvp.Key;
                button.clicked += () => OnButtonClicked(buttonName);
            }
            if (_SettingsPanel != null)
            {
                _SettingsPanel.style.display = DisplayStyle.None;
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

        var powerUp = GetCurrentPowerup();
        Debug.Log("⚡ Activating powerup: " + powerUp);
        _attackManager.SpecialAttack(powerUp);
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
        Debug.Log("Start met powerup:" + GetCurrentPowerup());
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
        _PowerUpPanel.style.display = DisplayStyle.None;
    }

    private void CancelForfeit()
    {
        _ConfirmationScreenForfeit.style.display = DisplayStyle.None;
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
        _ConfirmationScreen.style.display = DisplayStyle.None;
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
        if (_SplashScreen != null)
        {
            _SplashScreen.style.display = DisplayStyle.None;
        }
    }

    private void OnButtonClicked(string buttonName)
    {
        if (_ButtonActions.TryGetValue(buttonName, out var action))
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
        _ConfirmationScreenForfeit.style.display = DisplayStyle.Flex;
    }

    public void ShowVictoryScreen()
    {
        GameObject.Find("AttackSystem").gameObject.GetComponent<AttackManager>().enabled = false;
        _VictoryScreen.style.display = DisplayStyle.Flex;
    }
    private void Help()
    {
        Debug.Log("Help Button clicked. Opening Help Box");
        if (_HelpScreen.style.display == DisplayStyle.None)
            _HelpScreen.style.display = DisplayStyle.Flex;
    }

    private void CloseHelp()
    {
        _HelpScreen.style.display = DisplayStyle.None;
    }
    private void QuitGame()
    {
        if (_ConfirmationScreen.style.display == DisplayStyle.None)
        {
            _ConfirmationScreen.style.display = DisplayStyle.Flex;
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
        if (_SettingsPanel == null)
        {
            Debug.LogError("SettingsPanel niet Beschikbaar");
            return;
        }
    
        if (_SettingsPanel.style.display == DisplayStyle.None)
        {
            _SettingsPanel.style.display = DisplayStyle.Flex;
        }
        else
        {
            _SettingsPanel.style.display = DisplayStyle.None;
        }
    }

    private void CloseSettings()
    {
        _SettingsPanel.style.display = DisplayStyle.None;
    }
}
