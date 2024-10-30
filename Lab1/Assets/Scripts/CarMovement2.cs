using UnityEngine;
using UnityEngine.EventSystems;


public class CarMovement2 : Car
{
    public Lab1p2 playerControl;

    private void Awake()
    {
        playerControl = new Lab1p2();
        START_POSITION = new Vector3(90, 1, 770);
        START_ROTATION = Quaternion.Euler(0, 180, 0);
        _carId = 2;

    }
    private void OnEnable()
    {
        move = playerControl.Player.Move;
        move.Enable();
    }

    private void OnDisable()
    {
        move.Disable();
    }


}
