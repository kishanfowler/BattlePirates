using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool CanPlayerAttack;
    public GameStates gameState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameState = GameStates.PreparationPhase;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum GameStates
{
    PreparationPhase,
    PlayerTurn,
    AITurn
}