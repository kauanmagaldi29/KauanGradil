using System.Windows.Input;

namespace Gradil.App.Comum;

public class Comando(Action executar, Func<bool>? podeExecutar = null) : ICommand
{
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parametro) => podeExecutar?.Invoke() ?? true;

    public void Execute(object? parametro) => executar();

    public void AtualizarEstado() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}
