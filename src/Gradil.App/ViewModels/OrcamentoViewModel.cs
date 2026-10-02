using System.Collections.ObjectModel;
using System.Globalization;
using Gradil.App.Comum;
using Gradil.Core.Contratos;
using Gradil.Core.Modelos;
using Gradil.Core.Regras;
using Microsoft.Extensions.Logging;

namespace Gradil.App.ViewModels;

public class OrcamentoViewModel : ViewModelBase
{
    private const double PixelsPorMetro = 48;
    private const double PostePassaDaTela = 12;
    private const int LimiteDeTelasNoDesenho = 40;
    private static readonly CultureInfo PtBr = new("pt-BR");

    private readonly IPedidoRepositorio _repositorio;
    private readonly ILogger<OrcamentoViewModel> _logger;

    private string _comprimento = string.Empty;
    private Altura _altura = Catalogo.Alturas[0];
    private Pintura _pintura = Catalogo.Pinturas[0];
    private Orcamento? _orcamento;
    private string? _erro;
    private string? _sucesso;
    private string? _erroNoHistorico;

    public OrcamentoViewModel(IPedidoRepositorio repositorio, ILogger<OrcamentoViewModel> logger)
    {
        _repositorio = repositorio;
        _logger = logger;

        ConfirmarCommand = new Comando(Confirmar, () => Orcamento is not null);
        CancelarCommand = new Comando(Limpar);

        CarregarPedidos();
    }

    public IReadOnlyList<Altura> Alturas => Catalogo.Alturas;
    public IReadOnlyList<Pintura> Pinturas => Catalogo.Pinturas;
    public ObservableCollection<Pedido> Pedidos { get; } = [];

    public Comando ConfirmarCommand { get; }
    public Comando CancelarCommand { get; }

    public string Comprimento
    {
        get => _comprimento;
        set { if (Alterar(ref _comprimento, value)) Recalcular(); }
    }

    public Altura Altura
    {
        get => _altura;
        set { if (Alterar(ref _altura, value)) Recalcular(); }
    }

    public Pintura Pintura
    {
        get => _pintura;
        set { if (Alterar(ref _pintura, value)) Recalcular(); }
    }

    public Orcamento? Orcamento
    {
        get => _orcamento;
        private set
        {
            if (!Alterar(ref _orcamento, value)) return;

            Notificar(nameof(TemOrcamento));
            Notificar(nameof(Desenho));
            Notificar(nameof(AlturaDoPoste));
            Notificar(nameof(CorDaCerca));
            Notificar(nameof(AvisoDoDesenho));
            ConfirmarCommand.AtualizarEstado();
        }
    }

    public bool TemOrcamento => Orcamento is not null;

    public string? Erro
    {
        get => _erro;
        private set => Alterar(ref _erro, value);
    }

    public string? Sucesso
    {
        get => _sucesso;
        private set => Alterar(ref _sucesso, value);
    }

    public string? ErroNoHistorico
    {
        get => _erroNoHistorico;
        private set => Alterar(ref _erroNoHistorico, value);
    }

    public IReadOnlyList<VaoDaCerca> Desenho
    {
        get
        {
            if (Orcamento is null) return [];

            var alturaDaTela = (double)Orcamento.Altura.Metros * PixelsPorMetro;
            var quantidade = Math.Min(Orcamento.Telas, LimiteDeTelasNoDesenho);

            return Enumerable.Range(0, quantidade)
                .Select(_ => new VaoDaCerca(alturaDaTela, AlturaDoPoste, CorDaCerca))
                .ToList();
        }
    }

    public double AlturaDoPoste => (double)Altura.Metros * PixelsPorMetro + PostePassaDaTela;
    public string CorDaCerca => Pintura.CorHex;

    public string? AvisoDoDesenho =>
        Orcamento != null && Orcamento.Telas > LimiteDeTelasNoDesenho
            ? $"Mostrando {LimiteDeTelasNoDesenho} de {Orcamento.Telas} telas"
            : null;

    private void Recalcular()
    {
        Sucesso = null;

        if (string.IsNullOrWhiteSpace(Comprimento))
        {
            Erro = null;
            Orcamento = null;
            return;
        }

        if (!TentarLerComprimento(Comprimento, out var metros))
        {
            Erro = "Informe um número maior que zero e até 10.000 (ex.: 12,5).";
            Orcamento = null;
            return;
        }

        Erro = null;
        Orcamento = CalculadoraGradil.Calcular(metros, Altura, Pintura);
    }

    private void Confirmar()
    {
        if (Orcamento is null) return;

        var pedido = new Pedido(DateTime.Now, Orcamento.ComprimentoDesejado, Orcamento.Altura.Metros, Orcamento.Pintura.Nome);

        try
        {
            _repositorio.Salvar(pedido);
        }
        catch (RepositorioException ex)
        {
            _logger.LogWarning(ex, "Confirmação do pedido falhou");
            Erro = ex.Message;
            return;
        }

        Pedidos.Insert(0, pedido);
        Limpar();
        Sucesso = $"Pedido confirmado em {pedido.DataConfirmacao:dd/MM/yyyy 'às' HH:mm}.";
    }

    private void Limpar()
    {
        Comprimento = string.Empty;
        Altura = Catalogo.Alturas[0];
        Pintura = Catalogo.Pinturas[0];
        Erro = null;
        Sucesso = null;
    }

    private void CarregarPedidos()
    {
        try
        {
            foreach (var pedido in _repositorio.ListarMaisRecentes())
                Pedidos.Add(pedido);
        }
        catch (RepositorioException ex)
        {
            _logger.LogWarning(ex, "Não foi possível carregar o histórico de pedidos");
            ErroNoHistorico = ex.Message;
        }
    }

    private static bool TentarLerComprimento(string texto, out decimal metros) =>
        decimal.TryParse(texto.Trim().Replace('.', ','), NumberStyles.Number, PtBr, out metros)
        && metros > 0
        && metros <= Catalogo.ComprimentoMaximo;
}
