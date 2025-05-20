using UnityEngine;

public class Mist : MonoBehaviour
{
    private GameManager _gameManager;
    private int _startingTurn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        _startingTurn = _gameManager.Turns;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_gameManager.Turns == _startingTurn + 3)
        {
            Destroy(gameObject);
        }
    }
}
