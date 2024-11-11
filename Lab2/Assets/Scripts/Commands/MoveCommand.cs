using System.Collections;
using UnityEngine;
using PlayerManager;

namespace Game.Commands
{
    public class MoveCommand : ICommand
    {
        private PlayerOperator _playerOperator;
        private MoveCoordinate _moveCoordinate;

        public MoveCommand(PlayerOperator playerOperator, MoveCoordinate moveCoordinate)
        {
            _playerOperator = playerOperator;
            _moveCoordinate = moveCoordinate;
        }

        public void Execute()
        {
            _playerOperator.Move(_moveCoordinate);
        }
    }
}
