using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace yari.ViewModels
{
    public class ActionCommand : ICommand
    {
        private Action action;

        public Action Action
        {
            get { return action; }
            set { action = value; }
        }

        public ActionCommand(Action action)
        {
            this.action = action;
        }
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            action();
        }
    }
}
