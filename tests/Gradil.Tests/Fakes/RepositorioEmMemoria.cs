using System.IO;
using Gradil.Core.Contratos;
using Gradil.Core.Modelos;

namespace Gradil.Tests.Fakes;

public class RepositorioEmMemoria : IPedidoRepositorio
{
    public List<Pedido> Salvos { get; } = [];
    public bool SimularFalha { get; set; }

    public void Salvar(Pedido pedido)
    {
        if (SimularFalha)
            throw new RepositorioException("Não foi possível salvar o pedido.", new IOException("disco cheio"));

        Salvos.Add(pedido);
    }

    public IReadOnlyList<Pedido> ListarMaisRecentes() =>
        Salvos.OrderByDescending(p => p.DataConfirmacao).ToList();
}
