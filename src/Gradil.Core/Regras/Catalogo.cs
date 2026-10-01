using Gradil.Core.Modelos;

namespace Gradil.Core.Regras;

public static class Catalogo
{
    public const decimal ComprimentoDaTela = 2.5m;
    public const int ParafusosPorPoste = 4;

    public const decimal ComprimentoMaximo = 10_000m;

    public static IReadOnlyList<Altura> Alturas { get; } =
    [
        new(1.03m, FixadoresPorTela: 3),
        new(1.53m, FixadoresPorTela: 4),
        new(2.03m, FixadoresPorTela: 6),
    ];

    public static IReadOnlyList<Pintura> Pinturas { get; } =
    [
        new("Sem pintura", "#8A969E"),
        new("Branca", "#FFFFFF"),
        new("Preta", "#222222"),
        new("Verde", "#2E7D4F"),
    ];
}
