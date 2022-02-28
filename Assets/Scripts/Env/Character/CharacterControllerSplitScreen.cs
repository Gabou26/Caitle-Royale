using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterControllerSplitScreen : CharacterController2D
{
    [Header("Camera Controls")]
    [SerializeField] private Camera cameraPlayer;
    [SerializeField, Range(0f, 1f)] private float deadZone;
    [SerializeField, Range(1f, 6f)] private float cameraFollowingDistance;

    private Vector3 viewDirection = new Vector3(0f, 0f, -10f);
    
    private void LateUpdate()
    {
        cameraPlayer.transform.localPosition = viewDirection;
    }
    
    private void OnView(InputValue input)
    {
        switch (playerInput.currentControlScheme)
        {
            case "MouseKeyboard":
                Vector2 mouseWorldPosition = cameraPlayer.ScreenToViewportPoint(input.Get<Vector2>());

                mouseWorldPosition.x = (mouseWorldPosition.x - 0.5f) * cameraFollowingDistance;
                mouseWorldPosition.y = (mouseWorldPosition.y - 0.5f) * cameraFollowingDistance;

                viewDirection = deadZone < mouseWorldPosition.magnitude ?
                    new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, -10f) :
                    new Vector3(0f, 0f, -10f);
                
                break;
            case "Gamepad":
                viewDirection = new Vector3(0f, 0f, -10f);
                break;
        }
    }
}
