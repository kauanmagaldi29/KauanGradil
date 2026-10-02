using System.IO;
using Gradil.Core.Modelos;
using Gradil.Infraestrutura.Dados;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging.Abstractions;

namespace Gradil.Tests.Infraestrutura;
public class SqlitePedidoRepositorioTests : IDisposable
{
    private readonly string _arquivo = Path.Combine(Path.GetTempPath(), $"gradil-teste-{Guid.NewGuid()}.db");

    private SqlitePedidoRepositorio CriarRepositorio() =>
        new(_arquivo, NullLogger<SqlitePedidoRepositorio>.Instance);

    [Fact]
    public void Grava_e_le_o_pedido_sem_perder_precisao()
    {
        var repositorio = CriarRepositorio();
        var pedido = new Pedido(new DateTime(2026, 10, 3, 14, 30, 0), 10.1m, 1.53m, "Verde");

        repositorio.Salvar(pedido);

        Assert.Equal(pedido, Assert.Single(repositorio.ListarMaisRecentes()));
    }

    [Fact]
    public void Lista_do_mais_recente_para_o_mais_antigo()
    {
        var repositorio = CriarRepositorio();
        repositorio.Salvar(new Pedido(new DateTime(2026, 10, 1), 5m, 1.03m, "Branca"));
        repositorio.Salvar(new Pedido(new DateTime(2026, 10, 3), 7m, 2.03m, "Preta"));
        repositorio.Salvar(new Pedido(new DateTime(2026, 10, 2), 6m, 1.53m, "Verde"));

        var datas = repositorio.ListarMaisRecentes().Select(p => p.DataConfirmacao.Day);

        Assert.Equal(new[] { 3, 2, 1 }, datas);
    }

    [Fact]
    public void Os_dados_continuam_la_depois_de_reabrir()
    {
        CriarRepositorio().Salvar(new Pedido(DateTime.Now, 5m, 1.03m, "Branca"));

        Assert.Single(CriarRepositorio().ListarMaisRecentes());
    }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
        File.Delete(_arquivo);
    }
}
