using System;
using System.Windows.Input;

namespace MulticastChat;

public class AutoEventCommandBase : ICommand
{
    private readonly Action<object> _action;
    private readonly Func<object, bool> _predicate;

    public AutoEventCommandBase(Action<object> action, Func<object, bool> predicate)
    {
        _action = action;
        _predicate = predicate;
    }

    public bool CanExecute(object parameter) => _predicate(parameter);

    public void Execute(object parameter) => _action(parameter);

    public event EventHandler CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }
}