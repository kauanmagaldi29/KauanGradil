using Gradil.App.ViewModels;
using Gradil.Core.Modelos;
using Gradil.Core.Regras;
using Gradil.Tests.Fakes;
using Microsoft.Extensions.Logging.Abstractions;

namespace Gradil.Tests.ViewModels;

public class OrcamentoViewModelTests
{
    private readonly RepositorioEmMemoria _repositorio = new();

    private OrcamentoViewModel CriarViewModel() =>
        new(_repositorio, NullLogger<OrcamentoViewModel>.Instance);

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
    public void Entrada_invalida_mostra_erro_e_bloqueia_confirmacao(string digitado)
    {
        var vm = CriarViewModel();

        vm.Comprimento = digitado;

        Assert.Null(vm.Orcamento);
        Assert.NotNull(vm.Erro);
        Assert.False(vm.ConfirmarCommand.CanExecute(null));
    }

    [Fact]
    public void Trocar_a_altura_recalcula_o_orcamento()
    {
        var vm = CriarViewModel();
        vm.Comprimento = "10";

        vm.Altura = Catalogo.Alturas[2];

        Assert.Equal(24, vm.Orcamento!.Fixadores);
    }

    [Fact]
    public void Confirmar_salva_as_entradas_e_poe_o_pedido_no_topo_da_lista()
    {
        var vm = CriarViewModel();
        vm.Comprimento = "12,3";
        vm.Pintura = Catalogo.Pinturas[3];

        vm.ConfirmarCommand.Execute(null);

        var salvo = Assert.Single(_repositorio.Salvos);
        Assert.Equal(12.3m, salvo.Comprimento);
        Assert.Equal(1.03m, salvo.Altura);
        Assert.Equal("Verde", salvo.Pintura);
        Assert.Same(salvo, vm.Pedidos[0]);
        Assert.NotNull(vm.Sucesso);
        Assert.Equal(string.Empty, vm.Comprimento);
    }

    [Fact]
    public void Se_o_banco_falhar_o_orcamento_continua_na_tela_com_mensagem()
    {
        _repositorio.SimularFalha = true;
        var vm = CriarViewModel();
        vm.Comprimento = "10";

        vm.ConfirmarCommand.Execute(null);

        Assert.NotNull(vm.Orcamento);
        Assert.Equal("Não foi possível salvar o pedido.", vm.Erro);
        Assert.Empty(vm.Pedidos);
    }

    [Fact]
    public void Ao_abrir_carrega_os_pedidos_ja_confirmados()
    {
        _repositorio.Salvos.Add(new Pedido(new DateTime(2026, 9, 1), 5m, 1.03m, "Branca"));
        _repositorio.Salvos.Add(new Pedido(new DateTime(2026, 9, 2), 8m, 1.53m, "Preta"));

        var vm = CriarViewModel();

        Assert.Equal(2, vm.Pedidos.Count);
        Assert.Equal("Preta", vm.Pedidos[0].Pintura);
    }
}
