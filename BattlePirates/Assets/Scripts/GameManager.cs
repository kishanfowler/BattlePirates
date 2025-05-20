using System.Collections.Generic;
using System.Runtime.Serialization;
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
    [SerializeField] private int TimerTime;
    private int _timer;
    private Text TimerText;
    private bool DoOnce = false;
    private GridManager _AIGridManager;
    public bool TimerHasReset = false;
    public int Turns;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _timer = TimerTime * 60;
        if (GameManagerInstance != null)
        {
            Destroy(gameObject);
        }
        GameManagerInstance = this;
        GameState = GameStates.PreparationPhase;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        _shipManager = GameObject.Find("ShipManager").GetComponent<ShipManager>();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        if (ShipList.Count <= 0)
        {
            ShipList = _shipManager._ships;
        }
        if(scene.name == "PlayingPhase2")
        {
            _gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
            _AIGridManager = GameObject.Find("AIGridManager").GetComponent<GridManager>();
            TimerText = GameObject.Find("TimerText").GetComponent<Text>();
            DoOnce = true;
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
            TimerText.text = "00:" + (_timer / 60).ToString();
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
            TimerText.text = "00:" + (_timer / 60).ToString();
            if (_timer <= 0)
            {
                GameState = GameStates.PlayerTurn;
                TimerHasReset = false;
            }
        }

        if (DoOnce)
        {
            if(_gridManager.GridGenDone)
            {
                DoOnce=false;
                ChangeToAttackPhase();
            }
        }
    }

    void ChangeToAttackPhase()
    {
        for (int i = 0; i < ShipList.Count; i++)
        {
            ShipList[i].gameObject.GetComponent<PlacementManager>().CheckForOccupy(ShipList[i], _AIGridManager);
        }
        GameState = GameStates.PlayerTurn;
        CanPlayerAttack = true;
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
}




public enum GameStates
{
    PreparationPhase,
    PlayerTurn,
    AITurn
}