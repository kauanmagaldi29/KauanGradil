using Gradil.Core.Modelos;

namespace Gradil.Core.Contratos;

public interface IPedidoRepositorio
{
    void Salvar(Pedido pedido);

    IReadOnlyList<Pedido> ListarMaisRecentes();
}
