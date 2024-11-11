using System.Collections.Generic;
using UnityEngine;
using PlayerManager;
using UnityEngine.UIElements;
using System;
using UnityEngine.Events;

public class GameController : MonoBehaviour
{
    public List<PlayerOperator> player1Characters; // List of Player 1's characters
    public List<PlayerOperator> player2Characters; // List of Player 2's characters
    private bool _isPlayer1Turn = true; // Track whose turn it is
    private int _characterSelectAction = 0;
    private int _targetIndex = 0;
    private int _moveSelected = 0;
    private GridManager _gridManager;
    private void Start()
    {
        player1Characters[0].NotifyPlayerSelectsMove();
        _gridManager = FindObjectOfType<GridManager>();
        NotifyPlayerChanged();
        if (_gridManager == null)
        {
            Debug.Log("GridManager not found in the scene!");
        }
    }
    public void SwitchTurn()
    {
        if (_isPlayer1Turn) StopOngoingAnimations();
        _isPlayer1Turn = !_isPlayer1Turn;
        _targetIndex = 0;
        _characterSelectAction = 0;
        NotifyPlayerChanged();
        if(_isPlayer1Turn) player1Characters[0].NotifyPlayerSelectsMove();
        Debug.Log($"Turn switched to {(_isPlayer1Turn ? "Player 1" : "Player 2")}");
    }

    public bool CheckCharaterOnPosition(Vector2Int moveCoordinate)
    {
        if (!_gridManager.CheckCoordinateValid(moveCoordinate)) return false;
        foreach (var character in player1Characters)
        {
            if (character.Position == moveCoordinate)
            {
                return false;
            }
        }

        // Check if the moveCoordinate matches any position in player2Characters
        foreach (var character in player2Characters)
        {
            if (character.Position == moveCoordinate)
            {
                return false;
            }
        }

        // If no character is found on the moveCoordinate, return true
        return true;
    }

    public void ModifyAllHealth(bool isPlayer1, int healIncreaseValue)
    {
        if (isPlayer1)
        {
            foreach (PlayerOperator player in player1Characters)
            {
                player.UpdateHealth(healIncreaseValue);
            }
            Debug.Log($"Player 1's characters' health increased by {healIncreaseValue}");
        }
        else
        {
            foreach (PlayerOperator player in player2Characters)
            {
                player.UpdateHealth(healIncreaseValue);
            }
            Debug.Log($"Player 2's characters' health increased by {healIncreaseValue}");
        }
    }

    public void ModifyHealthPlayer(bool isPlayer1, int healIncreaseValue, int targetIndex)
    {
        if (isPlayer1) player1Characters[targetIndex].UpdateHealth(healIncreaseValue);
        else player2Characters[targetIndex].UpdateHealth(healIncreaseValue);

        Debug.Log($"{(isPlayer1 ? "Player 1" : "Player 2")}'s character at index {targetIndex} health modified by {healIncreaseValue}");
    }

    public int CharacterSelectAction
    {
        get => _characterSelectAction;
        set => _characterSelectAction = value;
    }
    public void StopOngoingAnimations()
    {
        if(_characterSelectAction == 3)
            player1Characters[2].NotifyPlayerDoesntSelectMove();
        else
            player1Characters[_characterSelectAction].NotifyPlayerDoesntSelectMove();
        StopAnimationTarget();
    }
    public void NextCharacterSelectionAction()
    {
        if(IsPlayer1Turn)
        {
            player1Characters[_characterSelectAction].NotifyPlayerDoesntSelectMove();
            _characterSelectAction++;
            if (_characterSelectAction < 3)
                player1Characters[_characterSelectAction].NotifyPlayerSelectsMove();
        }
        else
        {
            _characterSelectAction++;
        }
    }

    public bool IsPlayer1Turn => _isPlayer1Turn;

    public int TargetIndex
    {
        get => _targetIndex;
        set => _targetIndex = value;
    }

    public int MoveSelected
    {
        get => _moveSelected;
        set => _moveSelected = value;
    }
    public void StartAnimationTarget()
    {
        if (IsPlayer1Turn)
        {
            player2Characters[_targetIndex].NotifyPlayerSelectsMove();
        }

    }

    public void StopAnimationTarget()
    {
        if (IsPlayer1Turn)
        {
            player2Characters[_targetIndex].NotifyPlayerDoesntSelectMove();
        }
    }
    public void NextTargetIndex()
    {
        StopAnimationTarget();
        _targetIndex = (_targetIndex + 1) % 3;
        StartAnimationTarget();
        Debug.Log($"Next target index: {_targetIndex}");
    }

    public void PreviousTargetIndex()
    {
        StopAnimationTarget();
        _targetIndex = (_targetIndex - 1 + 3) % 3; 
        StartAnimationTarget();
        Debug.Log($"Previous target index: {_targetIndex}");
    }

    public void NextMove()
    {
        _moveSelected = (_moveSelected + 1) % 6;
        NotifyMoveSelctedChanged(true);
        Debug.Log($"Next move index: {_targetIndex}");
    }

    public void PreviousMove()
    {
        _moveSelected = (_moveSelected - 1 + 6) % 6;
        NotifyMoveSelctedChanged(false);
        Debug.Log($"Previous move index: {_targetIndex}");
    }
    public Vector2Int GetPlayerPosition(bool _isPlayer1, int target)
    {
        if (_isPlayer1) return player1Characters[target].Position;
        return player2Characters[target].Position;
    }

    public UnityEvent<float> onMoveSelectedChanged;
    public UnityEvent onPlayerChanged;

    private void NotifyMoveSelctedChanged(bool positive)
    {
        if (positive)
            onMoveSelectedChanged?.Invoke(60);
        else
            onMoveSelectedChanged?.Invoke(-60);
    }

    private void NotifyPlayerChanged()
    {
        onPlayerChanged?.Invoke();
    }
}
