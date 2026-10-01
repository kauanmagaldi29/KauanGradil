namespace Gradil.Core.Modelos;

public sealed record Altura(decimal Metros, int FixadoresPorTela)
{
    public override string ToString() => $"{Metros:0.00} m";
}
