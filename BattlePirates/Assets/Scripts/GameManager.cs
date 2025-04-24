using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool CanPlayerAttack;
    public GameStates GameState;
    private ShipManager shipManager;
    public static GameManager GameManagerInstance;
    private GridManager gridManager;
    public List<ShipBase> ShipList;

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
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        shipManager = GameObject.Find("ShipManager").GetComponent<ShipManager>();
        gridManager = GameObject.Find("GridManager").GetComponent<GridManager>();
        if (ShipList.Count <= 0)
        {
            ShipList = shipManager._ships;
        }
        if(scene.name == "PlayingPhase2" /*&& aimanager.hasplaced*/)
        {
            ChangeToAttackPhase();
        }
    }

    void ChangeToAttackPhase()
    {
        for (int i = 0; i < ShipList.Count; i++)
        {
            var shipPos = ShipList[i].gameObject.transform.position;
            ShipList[i].gameObject.transform.position = new Vector3(shipPos.x + 6, shipPos.y,shipPos.z);
            Physics.SyncTransforms();
            ShipList[i].gameObject.GetComponent<PlacementManager>().TryPlaceShip();
            Debug.Log("hoi");
            ShipList[i].gameObject.SetActive(false);
        }
        GameState = GameStates.PlayerTurn;
        CanPlayerAttack = true;
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