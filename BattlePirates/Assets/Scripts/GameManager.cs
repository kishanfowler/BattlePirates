using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool CanPlayerAttack;
    public GameStates GameState;
    private ShipManager shipManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //GameState = GameStates.PreparationPhase;
        shipManager = GameObject.Find("ShipManager").GetComponent<ShipManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.K))
        {
            ChangeToAttackPhase();
        }
    }

    void ChangeToAttackPhase()
    {
        GameState = GameStates.PlayerTurn;
        CanPlayerAttack = true;
        foreach (ShipBase ship in shipManager._ships)
        {
            ship.gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }

}




public enum GameStates
{
    PreparationPhase,
    PlayerTurn,
    AITurn
}