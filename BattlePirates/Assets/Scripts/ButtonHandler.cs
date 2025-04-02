using System;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.InputSystem.HID;
using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour
{
    public UIDocument uiDocument;
    private Dictionary<string, System.Action> m_ButtonActions;
    private VisualElement m_Help;
    private VisualElement m_HelpScreen;
    private VisualElement m_SettingsPanel;
    private Button m_SettingsButton;
    void Start()
    {
        var root = uiDocument.rootVisualElement;
        m_ButtonActions = new Dictionary<string, Action>
        {
            { "Play", Play },
            { "Settings", ToggleSettings },
            { "Quit", QuitGame },
            { "HelpButton", Help},
            { "Forfeit", Forfeit},
            { "Again", Again},
            { "MainMenu", MainMenu}
        };
        m_SettingsPanel = root.Q<VisualElement>("SettingsPanel");
        m_HelpScreen = root.Q<VisualElement>("HelpScreen");
        foreach (var kvp in m_ButtonActions)
        {
            Button button = root.Q<Button>(kvp.Key);
            if (m_HelpScreen != null)
            {
                m_HelpScreen.style.display = DisplayStyle.None;
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
        Debug.Log("Forfeit button clicked. Checking for confirmation");
        SceneManager.LoadScene("DefeatScreen");
    }

    private void Help()
    {
        Debug.Log("Help Button clicked. Opening Help Box");
        if (m_HelpScreen.style.display == DisplayStyle.None)
            m_HelpScreen.style.display = DisplayStyle.Flex; // Of wat je gebruikt (bijv. Grid)
        else
            m_HelpScreen.style.display = DisplayStyle.None;
    }

    private void OnButtonClicked(string buttonName)
    {
        if (m_ButtonActions.TryGetValue(buttonName, out var action))
        {
            action.Invoke();
        }
    }

    private void QuitGame()
    {
        Debug.Log("Quit button clicked. Closing Game");
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
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
        if (m_SettingsPanel == null)
        {
            Debug.LogError("SettingsPanel niet Beschikbaar");
            return;
        }

        if (m_SettingsPanel.style.display == DisplayStyle.None)
        {
            m_SettingsPanel.style.display = DisplayStyle.Flex;
        }
        else
        {
            m_SettingsPanel.style.display = DisplayStyle.None;
        }
    }
}
