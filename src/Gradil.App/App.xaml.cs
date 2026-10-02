using System.IO;
using System.Windows;
using Gradil.App.ViewModels;
using Gradil.App.Views;
using Gradil.Core.Contratos;
using Gradil.Infraestrutura.Dados;
using Gradil.Infraestrutura.Logs;
using Microsoft.Extensions.Logging;

namespace Gradil.App;

public partial class App : Application
{
    private ILoggerFactory? _logs;
    private ILogger<App>? _logger;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var pastaDoApp = AppContext.BaseDirectory;
        _logs = ConfiguracaoDeLogs.Criar(Path.Combine(pastaDoApp, "logs"));
        _logger = _logs.CreateLogger<App>();

        _logger.LogInformation("Aplicação iniciada");

        try
        {
            var repositorio = new SqlitePedidoRepositorio(
                Path.Combine(pastaDoApp, "gradil.db"),
                _logs.CreateLogger<SqlitePedidoRepositorio>());

            var viewModel = new OrcamentoViewModel(repositorio, _logs.CreateLogger<OrcamentoViewModel>());

            MainWindow = new MainWindow { DataContext = viewModel };
            MainWindow.Show();
        }
        catch (RepositorioException ex)
        {
            _logger.LogCritical(ex, "Falha ao iniciar");
            MessageBox.Show(
                $"{ex.Message}\n\nVerifique se a pasta do programa permite gravação.",
                "Gradil", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown(1);
        }
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _logger?.LogInformation("Aplicação encerrada");
        _logs?.Dispose();
        base.OnExit(e);
    }
}
