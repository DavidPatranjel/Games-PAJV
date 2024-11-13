using Game.Commands;
using PlayerManager;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    public PlayerControlls controls;
    public GameController gameController;
    private PlayerOperator currentPlayerOperator;
    private bool isSimpleAttackMode = false;
    private bool isMoveMode = false;
    
    public TMP_Text timerText; // Reference to UI Text to display the timer
    private float roundTime = 20f; // Duration of each round in seconds
    private Coroutine timerCoroutine;
    private void Awake()
    {
        controls = new PlayerControlls();
        controls.Controlls.Exit.performed += _ => Application.Quit();

        // Binding actions to keys
        controls.Controlls.Move.performed += _ => InitiateMove();       // S for Move
        controls.Controlls.MassHeal.performed += _ => ChooseMove(1);   // W for Mass Heal
        controls.Controlls.MassAttack.performed += _ => ChooseMove(2); // D for Mass Attack
        controls.Controlls.Attack.performed += _ => InitiateSimpleAttack(); // A to start Simple Attack

        controls.Controlls.ArrowR.performed += _ => SelectNextTarget();
        controls.Controlls.ArrowL.performed += _ => SelectPreviousTarget();
        controls.Controlls.Select.performed += _ => ConfirmTarget();
    }
    private void Start()
    {
        StartTurn();
    }
    private void Update()
    {
        if (roundTime >= 0.0f)
        {
            roundTime -= Time.deltaTime;
            timerText.text = $"Time: {roundTime:F0}s";
        }

        if(roundTime <= 0.0f)
        {
            Debug.Log("Time's up! Executing moves.");
            gameController.StopOngoingAnimations();
            NotifyDoesntPlayerChoseMove();
            RunAllCommands();
            EndTurn();
        }

    }

    private void OnEnable()
    {
        controls.Controlls.Enable();
    }

    private void OnDisable()
    {
        controls.Controlls.Disable();
    }

    private void StartTurn()
    {
        if (gameController.IsPlayer1Turn)
        {
            roundTime = 20.0f;  // Only reset timer at the start of Player 1's turn
        }
        Debug.Log($"Starting turn for {(gameController.IsPlayer1Turn ? "Player 1" : "Player 2")}");
        GetCharacter();

        if (!gameController.IsPlayer1Turn)
        {
            StartCoroutine(StartStartCoroutine());
            //StartCoroutine(SelectRandomMovesForPlayer2Coroutine());
        }
    }
    private IEnumerator StartStartCoroutine()
    {
        StartCoroutine(SelectRandomMovesForPlayer2Coroutine());
        yield return new WaitForSeconds(1);
    }
    private IEnumerator SelectRandomMovesForPlayer2Coroutine()
    {
        float delayTime = 0.5f; // Set the delay time between moves, adjust as necessary

        for (int i = 0; i < 3; i++)
        {
            int randomMoveType = Random.Range(0, 4); // Random move type (0: Move, 1: Mass Heal, 2: Mass Attack)
            Debug.Log("PC Player " + i.ToString() + " action " + randomMoveType.ToString());

            if (randomMoveType == 1 || randomMoveType == 2)
            {
                ChooseMove(randomMoveType);
            }
            else if (randomMoveType == 0)
            {
                int moveID = Random.Range(0, 5);
                Debug.Log("Moving: " + moveID.ToString());
                MoveCoordinate moveCoordinate = GetMoveCoordinate(moveID);
                RunPlayerMovement(currentPlayerOperator, moveCoordinate);
            }
            else if (randomMoveType == 3)
            {
                int targetID = Random.Range(0, 2);
                Debug.Log("Target: " + targetID.ToString());
                RunPlayerSimpleAttack(currentPlayerOperator, targetID);
            }

            gameController.NextCharacterSelectionAction();
            if (gameController.CharacterSelectAction < 3)
            {
                GetCharacter();
            }

            // Wait for the specified delay time before the next character's move
            yield return new WaitForSeconds(delayTime);
        }

        RunAllCommands();
        EndTurn();
    }
    public void ChooseMove(int moveType)
    {
        if (isSimpleAttackMode) return; // Avoid any move changes while in Simple Attack mode
        if (isMoveMode) return;

        switch (moveType)
        {
            case 1:
                Debug.Log("Chosen action: Mass Heal");
                RunPlayerMassHeal(currentPlayerOperator);
                break;
            case 2:
                Debug.Log("Chosen action: Mass Attack");
                RunPlayerMassAttack(currentPlayerOperator);
                break;
        }

        // Advance to the next character or end turn
        gameController.NextCharacterSelectionAction();
        if (gameController.CharacterSelectAction < 3)
        {
            GetCharacter();
        }
        else
        {
            RunAllCommands();
            EndTurn();
        }
    }

    private void GetCharacter()
    {
        Debug.Log("Getting character id: " + gameController.CharacterSelectAction);
        currentPlayerOperator = gameController.IsPlayer1Turn
            ? gameController.player1Characters[gameController.CharacterSelectAction]
            : gameController.player2Characters[gameController.CharacterSelectAction];
        Debug.Log($"Selected character {gameController.CharacterSelectAction + 1} for {(gameController.IsPlayer1Turn ? "Player 1" : "Player 2")}");
    }

    private void SelectNextTarget()
    {
        if (isSimpleAttackMode)
        {
            gameController.NextTargetIndex();
            Debug.Log($"Target index changed to: {gameController.TargetIndex}");
        }
        else if (isMoveMode)
        {
            gameController.NextMove();
            Debug.Log($"Move changed to: {gameController.MoveSelected}");
        }

    }

    private void SelectPreviousTarget()
    {
        if (isSimpleAttackMode)
        {
            gameController.PreviousTargetIndex();
            Debug.Log($"Target index changed to: {gameController.TargetIndex}");
        }
        else if (isMoveMode)
        {
            gameController.PreviousMove();
            Debug.Log($"Move changed to: {gameController.MoveSelected}");
        }
    }

    private void ConfirmTarget()
    {
        if (isSimpleAttackMode)
        {
            gameController.StopAnimationTarget();
            Debug.Log($"Simple Attack confirmed on target index: {gameController.TargetIndex}");
            RunPlayerSimpleAttack(currentPlayerOperator, gameController.TargetIndex);
            isSimpleAttackMode = false; // Exit Simple Attack mode after confirming

            // Advance to the next character or end turn
            gameController.NextCharacterSelectionAction();
            if (gameController.CharacterSelectAction < 3)
            {
                GetCharacter();
            }
            else
            {
                RunAllCommands();
                EndTurn();
            }
        }
        else if (isMoveMode)
        {
            Debug.Log($"Move confirmed: {gameController.MoveSelected}");
            MoveCoordinate moveCoordinate = GetMoveCoordinate(gameController.MoveSelected);

            RunPlayerMovement(currentPlayerOperator, moveCoordinate);
            NotifyDoesntPlayerChoseMove();
            isMoveMode = false; // Exit Move mode after confirming

            // Advance to the next character or end turn
            gameController.NextCharacterSelectionAction();
            if (gameController.CharacterSelectAction < 3)
            {
                GetCharacter();
            }
            else
            {
                RunAllCommands();
                EndTurn();
            }
        }
    }

    MoveCoordinate GetMoveCoordinate(int moveSelected)
    {
        switch (moveSelected)
        {
            case 0: return MoveCoordinate.LEFT;
            case 1: return MoveCoordinate.LEFT_UP;
            case 2: return MoveCoordinate.RIGHT_UP;
            case 3: return MoveCoordinate.RIGHT;
            case 4: return MoveCoordinate.RIGHT_DOWN;
            case 5: return MoveCoordinate.LEFT_DOWN;
            default:
                Debug.LogError("Invalid MoveSelected value.");
                return MoveCoordinate.LEFT; // Default or error handling
        }
    }
    private void InitiateMove()
    {
        isMoveMode = true; // Enter Move mode
        gameController.MoveSelected = 0;
        NotifyPlayerChoseMove();
        Debug.Log("Move mode activated. Use arrow keys to select a target.");
    }
    private void InitiateSimpleAttack()
    {
        isSimpleAttackMode = true; // Enter Simple Attack mode
        gameController.StartAnimationTarget();
        Debug.Log("Simple Attack mode activated. Use arrow keys to select a target.");
    }

    private void EndTurn()
    {
        Debug.Log($"End of turn for {(gameController.IsPlayer1Turn ? "Player 1" : "Player 2")}");
        gameController.SwitchTurn();
        StartTurn(); 
    }

    private void RunPlayerMovement(PlayerOperator playerOperator, MoveCoordinate moveCoordinate)
    {
        if (playerOperator == null) return;
        if (playerOperator.IsValidMove(moveCoordinate, out _))
        {
            ICommand command = new MoveCommand(playerOperator, moveCoordinate);
            CommandMaker.InsertCommand(command);
            Debug.Log("Move command added to queue.");
        }
    }

    private void RunPlayerMassHeal(PlayerOperator playerOperator)
    {
        if (playerOperator == null) return;
        ICommand command = new MassHealCommand(playerOperator);
        CommandMaker.InsertCommand(command);
        Debug.Log("Mass Heal command added to queue.");
    }

    private void RunPlayerMassAttack(PlayerOperator playerOperator)
    {
        if (playerOperator == null) return;
        ICommand command = new MassAttackCommand(playerOperator);
        CommandMaker.InsertCommand(command);
        Debug.Log("Mass Attack command added to queue.");
    }

    private void RunPlayerSimpleAttack(PlayerOperator playerOperator, int targetIndex)
    {
        if (playerOperator == null) return;
        if (playerOperator.IsNeighbour(targetIndex))
        {
            ICommand command = new SimpleAttackCommand(playerOperator, targetIndex);
            CommandMaker.InsertCommand(command);
            Debug.Log("Simple Attack command added to queue.");
        }
    }

    private void RunAllCommands()
    {
        CommandMaker.ExecuteAllCommands();
        Debug.Log("All commands executed.");
    }

    public UnityEvent onPlayerChoseMove;
    public UnityEvent onPlayerDoesntChoseMove;

    public void NotifyPlayerChoseMove ()
    {
        onPlayerChoseMove?.Invoke();
    }
    public void NotifyDoesntPlayerChoseMove()
    {
        onPlayerDoesntChoseMove?.Invoke();
    }
}
