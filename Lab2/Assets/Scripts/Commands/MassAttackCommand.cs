using PlayerManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Commands
{
    public class MassAttackCommand : ICommand
    {
        [SerializeField] private int _attackStrength = 2;

        private PlayerOperator _playerOperator;

        public MassAttackCommand(PlayerOperator playerOperator)
        {
            _playerOperator = playerOperator;
        }

        public void Execute()
        {
            _playerOperator.MassAttack(_attackStrength);
        }
    }
}
