using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private float _timeElapsed;
    private bool _isPaused = true;  // Start the game in a paused state
    public TMP_Text timerText;     // Reference to the TMP Text component
    public GameObject gameCanvas;  // Canvas for the game elements
    public GameObject pauseCanvas; // Canvas for the pause menu
    public GameObject otherCanvas1; // Other canvas used in the game
    public GameObject otherCanvas2; // Another canvas used in the game
    public TMP_Text winner1Text; 
    public TMP_Text winner2Text; 
    private PlayerInput _playerInput;

    private List<Car> _cars = new List<Car>();
    [SerializeField] private const int TOTAL_LAPS = 3;
    public int winner = 0;

    void Start()
    {
        HideCursor();
        ActivatePauseCanvas();

        _playerInput = GetComponent<PlayerInput>();

        _playerInput.actions["Pause"].performed += ctx => TogglePause();
        _playerInput.actions["Reset"].performed += ctx => ResetGame();
        _playerInput.actions["Exit"].performed += ctx => Application.Quit();
        _cars.AddRange(FindObjectsOfType<Car>());
        Debug.Log(_cars.Count);
        ResetGame();

    }

    void Update()
    {
        // Update the timer only when the game is not paused
        if (!_isPaused)
        {
            UpdateTimer();
            UpdateCarOrder();
            CheckWinner();
        }
    }

    void TogglePause()
    {
        _isPaused = !_isPaused; 

        if (_isPaused)
        {
            Time.timeScale = 0f;
            ActivatePauseCanvas();
        }
        else
        {
            Time.timeScale = 1f; 
            ActivateGameCanvas();
        }
    }
    public void UpdateCarOrder()
    {
        // Sort the cars based on laps, section IDs, and their positions
        _cars.Sort((car1, car2) =>
        {
            // First, compare laps
            if (car1.Laps != car2.Laps)
                return car2.Laps.CompareTo(car1.Laps); // Higher lap count is better

            // If laps are equal, compare sections
            if(car2.CurrentSection == null) return -1;
            if(car1.CurrentSection == null) return 1; 
            if (car1.CurrentSection != car2.CurrentSection)
                return car2.CurrentSection.sectionId.CompareTo(car1.CurrentSection.sectionId);

            // If both are in the same section, compare their distance to the reference point
            float distance1 = Vector3.Distance(car1.transform.position, car1.CurrentSection.referencePoint);
            float distance2 = Vector3.Distance(car2.transform.position, car2.CurrentSection.referencePoint);
            return distance2.CompareTo(distance1); // Further to the reference point is better
        });

        // After sorting, you can determine positions or do something with the order
        for (int i = 0; i < _cars.Count; i++)
        {
            if(_cars[i].CurrentSection != null)
                Debug.Log($"Car {i + 1}: Laps: {_cars[i].Laps}, Section: {_cars[i].CurrentSection.sectionId}");
            _cars[i].SetPlace(i+1);
            _cars[i].UpdatePlaceUI();
        }
    }
    public void CheckWinner()
    {
        //Debug.Log("First car id: " + _cars[0].CarId.ToString());

        if(_cars[0].Laps == TOTAL_LAPS) 
        {
            winner = _cars[0].CarId;
            EndGame();
        }
         
    }

    private void ActivatePauseCanvas()
    {
        gameCanvas.SetActive(false);
        otherCanvas1.SetActive(false);
        otherCanvas2.SetActive(false);
        pauseCanvas.SetActive(true);
        if (winner == 1)
        {
            winner1Text.gameObject.SetActive(true);
        }
        else
        {
            winner1Text.gameObject.SetActive(false);
        }

        if (winner == 2)
        {
            winner2Text.gameObject.SetActive(true);
        }
        else
        {
            winner2Text.gameObject.SetActive(false);
        }

    }

    private void ActivateGameCanvas()
    {
        gameCanvas.SetActive(true);
        otherCanvas1.SetActive(true);
        otherCanvas2.SetActive(true);
        pauseCanvas.SetActive(false);
    }

    private void UpdateTimer()
    {
        _timeElapsed += Time.deltaTime;

        // Calculate minutes, seconds, and milliseconds
        int minutes = Mathf.FloorToInt(_timeElapsed / 60F);
        int seconds = Mathf.FloorToInt(_timeElapsed % 60F);

        // Format the time and update the TMP text
        string timeFormatted = string.Format("{0:00}:{1:00}", minutes, seconds);
        timerText.text = timeFormatted;  // Display on the TMP text component
    }

    private void ResetTimer()
    {
        _timeElapsed = 0f;
        timerText.text = "00:00";
    }

    private void HideCursor()
    {
        Cursor.visible = false;        
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void ResetGame()
    {   if (_isPaused) 
        {
            winner = 0;
            ResetTimer();
            for (int i = 0; i < _cars.Count; i++)
            {
                _cars[i].Restart();
            } 
            ActivatePauseCanvas();
        }
    }

    public void EndGame()
    {
        _isPaused = true;
        Time.timeScale = 0f;
        ActivatePauseCanvas();
    }
}
