using Gradil.Core.Modelos;
using Gradil.Core.Regras;

namespace Gradil.Tests.Regras;

public class CalculadoraGradilTests
{
    private static readonly Altura Baixa = Catalogo.Alturas[0];
    private static readonly Pintura SemPintura = Catalogo.Pinturas[0];

    // decimal não é aceito em atributos, então os comprimentos entram como double.
    [Theory]
    [InlineData(10.0, 4, 5)]
    [InlineData(10.1, 5, 6)]
    [InlineData(2.5, 1, 2)]
    [InlineData(0.5, 1, 2)]
    public void Telas_arredondam_para_cima_e_ha_um_poste_a_mais(double comprimento, int telas, int postes)
    {
        var orcamento = CalculadoraGradil.Calcular((decimal)comprimento, Baixa, SemPintura);

        Assert.Equal(telas, orcamento.Telas);
        Assert.Equal(postes, orcamento.Postes);
    }

    [Theory]
    [InlineData(0, 12)]
    [InlineData(1, 16)]
    [InlineData(2, 24)]
    public void Fixadores_dependem_da_altura(int indiceDaAltura, int fixadores)
    {
        var orcamento = CalculadoraGradil.Calcular(10m, Catalogo.Alturas[indiceDaAltura], SemPintura);

        Assert.Equal(fixadores, orcamento.Fixadores);
    }

    [Fact]
    public void Cada_poste_leva_quatro_parafusos()
    {
        var orcamento = CalculadoraGradil.Calcular(10m, Baixa, SemPintura);

        Assert.Equal(orcamento.Postes * 4, orcamento.Parafusos);
    }

    [Fact]
    public void Sem_diferenca_quando_o_comprimento_e_multiplo_da_tela()
    {
        var orcamento = CalculadoraGradil.Calcular(10m, Baixa, SemPintura);

        Assert.Equal(10m, orcamento.ComprimentoVendido);
        Assert.False(orcamento.TemDiferenca);
    }

    [Fact]
    public void Mostra_a_diferenca_quando_vende_mais_do_que_o_cliente_precisa()
    {
        var orcamento = CalculadoraGradil.Calcular(10.1m, Baixa, SemPintura);

        Assert.Equal(12.5m, orcamento.ComprimentoVendido);
        Assert.Equal(2.4m, orcamento.Diferenca);
        Assert.True(orcamento.TemDiferenca);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    [InlineData(10_000.01)]
    public void Recusa_comprimento_zero_negativo_ou_acima_do_maximo(double comprimento)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => CalculadoraGradil.Calcular((decimal)comprimento, Baixa, SemPintura));
    }
}
