using PlayerManager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Commands
{
    public class MassHealCommand : ICommand
    {
        [SerializeField] private int _healIncreaseValue = 2;
        private PlayerOperator _playerOperator;
        public MassHealCommand(PlayerOperator playerOperator)
        {
            _playerOperator = playerOperator;
        }


        public void Execute()
        {
            _playerOperator.MassHeal(_healIncreaseValue);
        }
    }
}
