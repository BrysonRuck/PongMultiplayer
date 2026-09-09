using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

/*
 * SessionManager is intentionally light in the starter project.
 * Local Pong starts immediately. The GameManager, NetworkManager, and button
 * references mark where you will start a Host or Client session.
 */

public class SessionManager : MonoBehaviour
{
    [Header("Multiplayer")]
    [SerializeField] GameManager gameManager;
    
    // This does not already exist in the scene, you need to add it and reference it
    [SerializeField] NetworkManager networkManager;

    [Header("Multiplayer UI")]
    [SerializeField] Button startHostButton;
    [SerializeField] Button startClientButton;
    [SerializeField] Canvas sessionUI;

    void Awake()
    {
        sessionUI.gameObject.SetActive(true);

        startHostButton.onClick.AddListener(StartHost);
        startClientButton.onClick.AddListener(StartClient);
    }

    void StartHost()
    {
        Debug.Log("Starting Host...");

        if (networkManager.StartHost())
        {
            Debug.Log("Host started successfully.");
            sessionUI.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Failed to start Host.");
        }
    }

    void StartClient()
    {
        Debug.Log("Starting Client...");

        if (networkManager.StartClient())
        {
            Debug.Log("Client started successfully.");
            sessionUI.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Failed to start Client.");
        }
    }
}