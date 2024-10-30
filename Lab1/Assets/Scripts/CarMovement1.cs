using UnityEngine;
using UnityEngine.EventSystems;


public class CarMovement1 : Car
{
    public Lab1 playerControl;

    private void Awake()
    {
        playerControl = new Lab1();
        START_POSITION = new Vector3(120, 1, 770);
        START_ROTATION = Quaternion.Euler(0, 180, 0);
        _carId = 1;
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
