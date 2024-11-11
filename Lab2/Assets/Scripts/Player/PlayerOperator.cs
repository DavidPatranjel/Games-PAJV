using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Game.Commands;
namespace PlayerManager
{
    public class PlayerOperator : MonoBehaviour
    {
        private const int MAX_HEALTH = 10;
        [SerializeField] private int _health = 10;
        [SerializeField] private Vector2Int _position;
        private GameController _gameController;
        private static readonly HashSet<Vector2Int> MoveVectorsSet = new HashSet<Vector2Int>
        {
            new Vector2Int(0, -1),    // LEFT
            new Vector2Int(0, 1),     // RIGHT
            new Vector2Int(-1, 0),    // LEFT_UP
            new Vector2Int(1, -1),    // LEFT_DOWN
            new Vector2Int(-1, 1),     // RIGHT_UP
            new Vector2Int(1, 0)      // RIGHT_DOWN
        };
        [SerializeField] private bool _isPlayer1 = true;

    
        private void Start()
        {
            NotifyMapPlayerMoved(_position);
            NotifyPlayerHealthChanged();
            _gameController = FindObjectOfType<GameController>();
            if (_gameController == null)
            {
                Debug.LogError("GameController not found in the scene!");
            }
        }
        public void Move(MoveCoordinate moveCoordinate)
        {
                Vector2Int newPosition;
                if (!IsValidMove(moveCoordinate, out newPosition)) return;
                Vector2Int oldposition = _position;
                _position = newPosition;
                Debug.Log("Old ps:" + oldposition + "New ps: " + _position);
                NotifyMapPlayerMoved(oldposition);
            

        }
        public void MassHeal(int healIncreaseValue)
        {
            _gameController.ModifyAllHealth(_isPlayer1, healIncreaseValue);
        }
        public void MassAttack(int attackStrength)
        {
            _gameController.ModifyAllHealth(!_isPlayer1, -attackStrength);
        }
        public void SimpleAttack(int attackStrength, int targetIndex)
        {
            _gameController.ModifyHealthPlayer(!_isPlayer1, -attackStrength, targetIndex);
        }
        public bool IsValidMove(MoveCoordinate moveCoordinate, out Vector2Int newPosition)
        {
            newPosition = Vector2Int.zero;
            if (MoveCoordinateDictionary.MoveVectors.TryGetValue(moveCoordinate, out Vector2Int moveVector))
            {
                newPosition = moveVector + _position;
                return _gameController.CheckCharaterOnPosition(newPosition);
            }
            else
            {
                Debug.LogWarning("Invalid move coordinate.");
                return false;
            }
        }

        public bool IsNeighbour(int target)
        {
            Vector2Int enemyPosition = _gameController.GetPlayerPosition(!_isPlayer1, target);
            Vector2Int differencePos = _position - enemyPosition;
            return MoveVectorsSet.Contains(differencePos);
        }

        public UnityEvent<int> onPlayerHealthChanged;
        public UnityEvent<Vector2Int, Vector2Int, bool> onMapPlayerMoved;
        public UnityEvent onPlayerSelectsMove;
        public UnityEvent onPlayerDoesntSelectMove;


        public void UpdateHealth(int healIncreaseValue)
        {
            _health = Mathf.Min(_health + healIncreaseValue, MAX_HEALTH);
            if (_health < 0) Debug.LogWarning("Player dead");
            NotifyPlayerHealthChanged();
        }
        private void NotifyPlayerHealthChanged()
        {
            onPlayerHealthChanged?.Invoke(_health);
        }
        private void NotifyMapPlayerMoved(Vector2Int oldpos)
        {
            onMapPlayerMoved?.Invoke(_position, oldpos, _isPlayer1);
        }
        public void NotifyPlayerSelectsMove()
        {
            onPlayerSelectsMove?.Invoke();
        }
        public void NotifyPlayerDoesntSelectMove()
        {
            onPlayerDoesntSelectMove?.Invoke();
        }
        public Vector2Int Position => _position;

    }

    public enum MoveCoordinate
    {
        LEFT,
        LEFT_UP,
        RIGHT_UP,
        RIGHT,
        RIGHT_DOWN,
        LEFT_DOWN
    }


    public static class MoveCoordinateDictionary
    {
        public static readonly Dictionary<MoveCoordinate, Vector2Int> MoveVectors = new Dictionary<MoveCoordinate, Vector2Int>
    {
        { MoveCoordinate.LEFT, new Vector2Int(0, -1) },
        { MoveCoordinate.RIGHT, new Vector2Int(0, 1) },
        { MoveCoordinate.LEFT_UP, new Vector2Int(-1, 0) },
        { MoveCoordinate.LEFT_DOWN, new Vector2Int(1, -1) },
        { MoveCoordinate.RIGHT_UP, new Vector2Int(-1, 1) },
        { MoveCoordinate.RIGHT_DOWN, new Vector2Int(1, 0) }

    };
    }
}