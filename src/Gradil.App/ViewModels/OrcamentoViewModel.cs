using System.Globalization;
using Gradil.App.Comum;
using Gradil.Core.Modelos;
using Gradil.Core.Regras;

namespace Gradil.App.ViewModels;

public class OrcamentoViewModel : ViewModelBase
{
    private static readonly CultureInfo PtBr = new("pt-BR");

    private string _comprimento = string.Empty;
    private Altura _altura = Catalogo.Alturas[0];
    private Pintura _pintura = Catalogo.Pinturas[0];
    private Orcamento? _orcamento;
    private string? _erro;

    public IReadOnlyList<Altura> Alturas => Catalogo.Alturas;
    public IReadOnlyList<Pintura> Pinturas => Catalogo.Pinturas;

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
        }
    }

    public bool TemOrcamento => Orcamento is not null;

    public string? Erro
    {
        get => _erro;
        private set => Alterar(ref _erro, value);
    }

    private void Recalcular()
    {
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

    private static bool TentarLerComprimento(string texto, out decimal metros) =>
        decimal.TryParse(texto.Trim().Replace('.', ','), NumberStyles.Number, PtBr, out metros)
        && metros > 0
        && metros <= Catalogo.ComprimentoMaximo;
}
