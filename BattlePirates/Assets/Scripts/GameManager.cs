using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject DutchManPrefab;
    [SerializeField] private int TimerTime;
    private List<ShipBase> _shipList = new();
    private int _timer;
    private Text _timerText;
    private bool _doOnce = false;
    private bool _ghostShipSpawned;
    public bool TimerHasReset = false;
    public AttackManager.SpecialAttacks ChosenPowerUp;
    public bool PowerUpChosen = false;
    public int Turns;
    public bool CanPlayerAttack;
    public GridManager GridManager;
    public GridManager AIGridManager;
    public ShipManager ShipManager;
    public GameStates GameState {get; private set;}
    public static GameManager GameManagerInstance;
    public List<GameObject> CaptainPortraits;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _shipList = new List<ShipBase>();
        _timer = TimerTime * 60;
        if (GameManagerInstance != null)
        {
            Destroy(gameObject);
        }
        GameManagerInstance = this;
        GameState = GameStates.PreparationPhase;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        GridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
    }

    private void Start()
    {
        ShipManager = ShipManager.ShipManagerInstance;
        if (_shipList.Count <= 0)
        {
            _shipList = ShipManager._ships;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(scene.name == "PlayingPhase2")
        {
            // Dit is in een andere scene maar hij heeft wel dezelfe naam
            GridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
            AIGridManager = GameObject.Find("AIGridManager").GetComponent<GridManager>();
            ShipManager = GameObject.Find("ShipManager").GetComponent<ShipManager>();
            _timerText = GameObject.Find("TimerText").GetComponent<Text>();
            _doOnce = true;
            if (_shipList.Count <= 0)
            {
                _shipList = ShipManager._ships;
            }
        }
    }
    
    private void FixedUpdate()
    {
        if (GameState == GameStates.PlayerTurn)
        {
            if (!TimerHasReset)
            {
                ResetTimer();
            }
            _timer--;
            if (_timerText != null) _timerText.text = "00:" + (_timer / 60).ToString();
            if (_timer <= 0)
            {
                GameState = GameStates.AITurn;
                TimerHasReset = false;
            }
        }

        if (GameState == GameStates.AITurn)
        {
            if (!TimerHasReset)
            {
                ResetTimer();
            }
            _timer--;
            if (_timerText != null) _timerText.text = "00:" + (_timer / 60).ToString();
            if (_timer <= 0)
            {
                GameState = GameStates.PlayerTurn;
                TimerHasReset = false;
            }
        }

        if (SceneManager.GetActiveScene().name == "PlanningPhase2")
        {
            if (PowerUpChosen && ChosenPowerUp == AttackManager.SpecialAttacks.Dutchman && !_ghostShipSpawned)
            {
                SpawnGhostShip();
            }
        }
        if (_doOnce)
        {
            if(GridManager.GridGenDone)
            {
                _doOnce=false;
                ChangeToAttackPhase();
            }
        }

        if (SceneManager.GetActiveScene().name == "PlayingPhase2")
        {
            if (PowerUpChosen && ChosenPowerUp == AttackManager.SpecialAttacks.Dutchman)
            {
                var _attackManager = GameObject.Find("AttackManager").GetComponent<AttackManager>();
                _attackManager.CanPlayerSpecialAttack = false;
            }
        }
    }

    

    void ChangeToAttackPhase()
    {
        for (int i = 0; i < _shipList.Count; i++)
        {
            _shipList[i].gameObject.GetComponent<PlacementManager>().CheckForOccupy(_shipList[i], AIGridManager);
        }
        
        GameState = GameStates.PlayerTurn;
        CanPlayerAttack = true;
    }

    void SpawnGhostShip()
    {
        var ship = Instantiate(DutchManPrefab, new Vector3(-8, 6, -1), quaternion.identity);
        ship.GetComponent<ShipBase>().IsPlayerShip = true;
        _ghostShipSpawned = true;
    }

    private void ResetTimer() 
    {
        _timer = TimerTime * 60;
        TimerHasReset = true;
        Turns++;
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

    public void SetGameState(GameStates Gamestate)
    {
        GameState = Gamestate;
    }
}


public enum GameStates
{
    PreparationPhase,
    PlayerTurn,
    AITurn
}