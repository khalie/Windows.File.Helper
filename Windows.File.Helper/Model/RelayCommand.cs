using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Windows.File.Helper.Model
{
  public class RelayCommand : ICommand
  {
    private readonly Action _execute;
    private readonly Func<bool> _canExecute;

    public RelayCommand(Action execute, Func<bool> canExecute)
    {
      _execute = execute;
      _canExecute = canExecute;
    }
    public bool CanExecute(object parameter)
    {
      return _canExecute();
    }

    public event EventHandler CanExecuteChanged;
    public void RaiseCanExecuteChanged()
    {
      if (null != this.CanExecuteChanged)
      { 
        this.CanExecuteChanged(this, new EventArgs());
      }
    }

    public void Execute(object parameter)
    {
      _execute();
    }
  }
}
