using Gradil.Core.Modelos;

namespace Gradil.Core.Regras;

public static class CalculadoraGradil
{
    public static Orcamento Calcular(decimal comprimento, Altura altura, Pintura pintura)
    {
        if (comprimento <= 0 || comprimento > Catalogo.ComprimentoMaximo)
            throw new ArgumentOutOfRangeException(nameof(comprimento), "O comprimento deve ser maior que zero e no máximo 10.000 m.");
        ArgumentNullException.ThrowIfNull(altura);
        ArgumentNullException.ThrowIfNull(pintura);

        var telas = (int)Math.Ceiling(comprimento / Catalogo.ComprimentoDaTela);

        var postes = telas + 1;

        return new Orcamento(
            comprimento,
            altura,
            pintura,
            telas,
            postes,
            Fixadores: telas * altura.FixadoresPorTela,
            Parafusos: postes * Catalogo.ParafusosPorPoste);
    }
}
