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
    private Button _SettingsButton;
    private VisualElement _SplashScreen;
    private VisualElement _ConfirmationScreen;
    private Button _ActualQuitButton;
    private Button _CancelButton;
    private VisualElement _ConfirmationScreenForfeit;
    private Button _ActualForfeitButton;
    void Start()
    {
        var root = uiDocument.rootVisualElement;
        _ButtonActions = new Dictionary<string, Action>
        {
            { "Play", Play },
            { "Settings", ToggleSettings },
            { "Quit", QuitGame },
            { "HelpButton", Help},
            { "Forfeit", Forfeit},
            { "Again", Again},
            { "MainMenu", MainMenu},
            { "SplashScreen", SplashScreen},
            { "ActualQuitButton", ActualQuit},
            { "Cancel", Cancel},{ "CancelForfeit", CancelForfeit},
            { "ActualForfeitButton", ActualForfeit}
        };
        _ConfirmationScreen = root.Q<VisualElement>("ConfirmationScreen");
        _ConfirmationScreenForfeit = root.Q<VisualElement>("ConfirmationScreenForfeit");
        _SettingsPanel = root.Q<VisualElement>("SettingsPanel");
        _HelpScreen = root.Q<VisualElement>("HelpScreen");
        _SplashScreen = root.Q<VisualElement>("SplashScreen");
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
            if (button != null)
            {
                string buttonName = kvp.Key;
                button.clicked += () => OnButtonClicked(buttonName);
            }
            else
            {
                Debug.Log($"Button met naam '{kvp.Key}' niet gevonden!");
            }
        }

        root.RegisterCallback<ClickEvent>(evt => SplashScreen() );
    }

    private void CancelForfeit()
    {
        _ConfirmationScreenForfeit.style.display = DisplayStyle.None;
    }

    private void ActualForfeit()
    {
        Debug.Log("Forfeit button clicked. Checking for confirmation");
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
        _SplashScreen.style.display = DisplayStyle.None;
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

    private void Help()
    {
        Debug.Log("Help Button clicked. Opening Help Box");
        if (_HelpScreen.style.display == DisplayStyle.None)
            _HelpScreen.style.display = DisplayStyle.Flex; // Of wat je gebruikt (bijv. Grid)
        else
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
}
