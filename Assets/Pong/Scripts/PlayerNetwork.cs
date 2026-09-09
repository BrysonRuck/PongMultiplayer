using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] Paddle paddle;

    [SerializeField] Key moveUpKey = Key.W;
    [SerializeField] Key moveDownKey = Key.S;

    void Update()
    {
       
        if (!IsOwner)
            return;

        float direction = 0f;

        if (Keyboard.current[moveUpKey].isPressed)
            direction += 1f;

        if (Keyboard.current[moveDownKey].isPressed)
            direction -= 1f;

        if (direction != 0f)
            MovePaddleServerRpc(direction);
    }

    [ServerRpc]
    void MovePaddleServerRpc(float direction)
    {
        paddle.Move(direction);
    }

    public override void OnNetworkSpawn()
    {
   
        if (OwnerClientId == NetworkManager.ServerClientId)
        {
            paddle.SetSide(PaddleSide.Left);
        }
        else
        {
            paddle.SetSide(PaddleSide.Right);
        }
    }
}