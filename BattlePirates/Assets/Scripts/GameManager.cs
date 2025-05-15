using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public bool CanPlayerAttack;
    public GameStates GameState;
    public static GameManager GameManagerInstance;
    private GridManager _gridManager;
    public List<ShipBase> ShipList;
    private ShipManager _shipManager;
    private int _timer = 3600;
    [SerializeField] private Text TimerText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (GameManagerInstance != null)
        {
            Destroy(gameObject);
        }
        GameManagerInstance = this;
        GameState = GameStates.PreparationPhase;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        //GameState = GameStates.PreparationPhase;
        _shipManager = GameObject.Find("ShipManager").GetComponent<ShipManager>();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        if (ShipList.Count <= 0)
        {
            ShipList = _shipManager._ships;
        }
        if(scene.name == "PlayingPhase2" /*&& aimanager.hasplaced*/)
        {
            ChangeToAttackPhase();
        }

        if (GameState == GameStates.PlayerTurn)
        {
            _timer--;
            TimerText.text = (_timer / 60).ToString();
            if (_timer <= 0)
            {
                GameState = GameStates.AITurn;
                _timer = 3600;
            }
        }

        if (GameState == GameStates.AITurn)
        {
            _timer--;
            TimerText.text = (_timer / 60).ToString();
            if (_timer <= 0)
            {
                GameState = GameStates.PlayerTurn;
                _timer = 3600;
            }
        }

    }

    void ChangeToAttackPhase()
    {
        for (int i = 0; i < ShipList.Count; i++)
        {
            ShipList[i].gameObject.GetComponent<PlacementManager>().PlaceShip();
            Debug.Log("hoi");
            ShipList[i].gameObject.SetActive(false);
        }
        GameState = GameStates.PlayerTurn;
        CanPlayerAttack = true;
        foreach (ShipBase ship in _shipManager._ships)
        {
            ship.gameObject.SetActive(false);
        }
    }

    public static int BetterClamp(int Amount, int Min, int Max)
    {
        if(Amount < Min)
        {
            return Min;
        }

        if(Amount > Max) 
        { 
            return Max;
        }

        return Amount;
    }
}




public enum GameStates
{
    PreparationPhase,
    PlayerTurn,
    AITurn
}