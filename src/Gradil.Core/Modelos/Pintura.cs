namespace Gradil.Core.Modelos;

public sealed record Pintura(string Nome, string CorHex)
{
    public override string ToString() => Nome;
}
