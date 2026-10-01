using Gradil.Core.Regras;

namespace Gradil.Core.Modelos;

public sealed record ItemOrcamento(string Componente, int Quantidade);

public sealed record Orcamento(
    decimal ComprimentoDesejado,
    Altura Altura,
    Pintura Pintura,
    int Telas,
    int Postes,
    int Fixadores,
    int Parafusos)
{
    public decimal ComprimentoVendido => Telas * Catalogo.ComprimentoDaTela;
    public decimal Diferenca => ComprimentoVendido - ComprimentoDesejado;
    public bool TemDiferenca => Diferenca > 0;

    public IReadOnlyList<ItemOrcamento> Itens =>
    [
        new("Tela", Telas),
        new("Poste", Postes),
        new("Fixador", Fixadores),
        new("Parafuso", Parafusos),
    ];
}
