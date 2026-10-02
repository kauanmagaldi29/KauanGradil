using Gradil.App.ViewModels;
using Gradil.Core.Regras;

namespace Gradil.Tests.ViewModels;

public class OrcamentoViewModelTests
{
    private static OrcamentoViewModel CriarViewModel() => new();

    [Theory]
    [InlineData("10,1")]
    [InlineData("10.1")]
    [InlineData(" 10,1 ")]
    public void Aceita_virgula_ou_ponto_no_comprimento(string digitado)
    {
        var vm = CriarViewModel();

        vm.Comprimento = digitado;

        Assert.NotNull(vm.Orcamento);
        Assert.Equal(5, vm.Orcamento!.Telas);
        Assert.Null(vm.Erro);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("0")]
    [InlineData("-2")]
    [InlineData("99999999999999")]
    public void Entrada_invalida_mostra_erro(string digitado)
    {
        var vm = CriarViewModel();

        vm.Comprimento = digitado;

        Assert.Null(vm.Orcamento);
        Assert.NotNull(vm.Erro);
    }

    [Fact]
    public void Trocar_a_altura_recalcula_o_orcamento()
    {
        var vm = CriarViewModel();
        vm.Comprimento = "10";

        vm.Altura = Catalogo.Alturas[2];

        Assert.Equal(24, vm.Orcamento!.Fixadores);
    }
}
