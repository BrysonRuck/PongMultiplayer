using TMPro;
using Unity.Netcode;
using UnityEngine;

/*
 * GameManager owns the local match rules: scoring, win checks, and ball resets.
 * In the starter project, everything happens in one Unity player. This is
 * the script you will convert so the server owns shared game state and the
 * score is synchronized to every client.
 */

public class GameManager : NetworkBehaviour
{
    [SerializeField] Transform ball;
    [SerializeField] float startSpeed = 3f;
    [SerializeField] Vector3 startPosition = new(0f, 0.25f, 0f);
    [SerializeField] TextMeshProUGUI leftPlayerScoreText;
    [SerializeField] TextMeshProUGUI rightPlayerScoreText;

    NetworkVariable<int> _leftPlayerScore = new NetworkVariable<int>();
    NetworkVariable<int> _rightPlayerScore = new NetworkVariable<int>();

    const int ScoreToWin = 11;

    bool gameStarted = false;

    public override void OnNetworkSpawn()
    {
        _leftPlayerScore.OnValueChanged += OnLeftScoreChanged;
        _rightPlayerScore.OnValueChanged += OnRightScoreChanged;

        if (!IsServer)
            return;

        NetworkManager.OnClientConnectedCallback += OnClientConnected;
        CheckPlayersReady();
    }
    void OnLeftScoreChanged(int oldScore, int newScore)
    {
        leftPlayerScoreText.text = newScore.ToString();
    }

    void OnRightScoreChanged(int oldScore, int newScore)
    {
        rightPlayerScoreText.text = newScore.ToString();
    }
    public override void OnNetworkDespawn()
    {
        _leftPlayerScore.OnValueChanged -= OnLeftScoreChanged;
        _rightPlayerScore.OnValueChanged -= OnRightScoreChanged;

        if (NetworkManager != null)
            NetworkManager.OnClientConnectedCallback -= OnClientConnected;
    }

    void OnClientConnected(ulong clientId)
    {
        Debug.Log($"Client connected: {clientId}");

        CheckPlayersReady();
    }

    void CheckPlayersReady()
    {
        if (gameStarted)
            return;

        int playerCount = NetworkManager.ConnectedClients.Count;

        Debug.Log($"Players connected: {playerCount}");

        if (playerCount >= 2)
        {
            gameStarted = true;

            Debug.Log("Both players connected! Starting game.");

            UpdateScore();
            StartGame();
        }
    }

    public void StartGame()
    {
        if (!IsServer)
            return;

        float direction = Random.value < 0.5f ? -1f : 1f;
        ResetBall(direction);
    }

    public void OnGoalScored(PaddleSide scoringSide)
    {
       
        if (!IsServer)
            return;
        if (scoringSide == PaddleSide.Left)
        {
            _leftPlayerScore.Value++;
            Debug.Log($"Left player scored: {_leftPlayerScore.Value}");

            if (_leftPlayerScore.Value == ScoreToWin)
            {
                Debug.Log("Left player wins!");
            }
            else
            {
                ResetBall(1f);
            }
        }
        else if (scoringSide == PaddleSide.Right)
        {
            _rightPlayerScore.Value++;
            Debug.Log($"Right player scored: {_rightPlayerScore.Value}");

            if (_rightPlayerScore.Value == ScoreToWin)
            {
                Debug.Log("Right player wins!");
            }
            else
            {
                ResetBall(-1f);
            }
        }

        UpdateScore();
    }


    void UpdateScore()
    {
        rightPlayerScoreText.text = _rightPlayerScore.Value.ToString();
        leftPlayerScoreText.text = _leftPlayerScore.Value.ToString();
    }

    void ResetBall(float directionSign)
    {
        if (!IsServer)
            return;

        directionSign = Mathf.Sign(directionSign);

        Vector3 newVelocity =
            new Vector3(directionSign, 0f, 0f) * startSpeed;

        newVelocity =
            Quaternion.Euler(
                0f,
                Random.Range(-20f, 20f),
                0f
            ) * newVelocity;

        Rigidbody ballRigidbody = ball.GetComponent<Rigidbody>();

        ballRigidbody.position = startPosition;
        ballRigidbody.linearVelocity = newVelocity;
        ballRigidbody.angularVelocity = Vector3.zero;
    }
}