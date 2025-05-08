using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool CanPlayerAttack;
    public GameStates GameState;
    private ShipManager _shipManager;
    private int _timer = 3600;
    [SerializeField] private Text TimerText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //GameState = GameStates.PreparationPhase;
        _shipManager = GameObject.Find("ShipManager").GetComponent<ShipManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.K))
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
        GameState = GameStates.PlayerTurn;
        CanPlayerAttack = true;
        foreach (ShipBase ship in _shipManager._ships)
        {
            ship.gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
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