using PlayerManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Commands
{
    public class SimpleAttackCommand : ICommand
    {
        [SerializeField] private int _attackStrength = 4;

        private PlayerOperator _playerOperator;
        private int _targetIndex;

        public SimpleAttackCommand(PlayerOperator playerOperator, int targetIndex)
        {
            _playerOperator = playerOperator;
            _targetIndex = targetIndex;
        }

        public void Execute()
        {
            _playerOperator.SimpleAttack(_attackStrength, _targetIndex);
        }
    }
}
