using System.Collections.Generic;
using UnityEngine;


namespace Game.Commands
{
    public interface ICommand
    {
        void Execute();
    }

    public class CommandMaker : MonoBehaviour
    {
        private static Queue<ICommand> _commands = new Queue<ICommand>();

        public static void ExecuteAllCommands()
        {
            while (_commands.Count > 0)
            {
                ICommand command = _commands.Dequeue();
                command.Execute();
            }
        }

        public static void InsertCommand(ICommand command)
        {
            _commands.Enqueue(command);
        }
    }
}