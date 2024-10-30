using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public abstract class Car : MonoBehaviour
{
    private const float MOVE_SPEED = 100f;
    private const float TURN_SPEED = 100f;
    private const int NO_PLAYERS = 2;
    protected int _carId = 0;
    [SerializeField]
    private int _laps = 0;
    [SerializeField]
    private int _place = 1;
    [SerializeField]
    public RoadSection _currentSection; 
    private readonly string[] PLACE_STRING_UI  = {"", "1st", "2nd"};
    protected Vector3 START_POSITION; // Example starting position
    protected Quaternion START_ROTATION;

    protected Rigidbody rb;
    protected Vector2 moveInput = Vector2.zero;

    protected InputAction move;
    public TMP_Text placeText;
    public TMP_Text lapText;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        UpdatePlaceUI();
        moveInput = move.ReadValue<Vector2>();

        // Calculate forward and backward movement
        Vector3 moveDirection = transform.forward * moveInput.y * MOVE_SPEED * Time.deltaTime;

        // Calculate left and right rotation
        float turn = moveInput.x * TURN_SPEED * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0, turn, 0);

        // Apply movement and rotation
        rb.MovePosition(rb.position + moveDirection);
        rb.MoveRotation(rb.rotation * turnRotation);
    }
    public void IncrementLaps()
    {
        _laps++;
        Debug.Log("Laps: " + _laps);
    }
    public void UpdatePlaceUI()
    {
        // Update the UI with the corresponding place string
        if (placeText != null && _place > 0 && _place <= NO_PLAYERS)
        {
            placeText.text = PLACE_STRING_UI[(_place)]; // Convert place to an index
        }
        if(lapText != null)
            lapText.text = _laps.ToString();
    }

    public void SetCurrentSection(RoadSection section)
    {
        _currentSection = section; // Update the current section
    }

    public void SetPlace(int place)
    {
        _place = place;
    }

    public void Restart()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        rb.position = START_POSITION;
        rb.rotation = START_ROTATION;
        _currentSection = null;
        _laps = 0;
    }

    public int Laps => _laps; 
    public RoadSection CurrentSection => _currentSection; 
    public int CarId => _carId;
}
