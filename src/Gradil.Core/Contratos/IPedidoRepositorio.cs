using Gradil.Core.Modelos;

namespace Gradil.Core.Contratos;

public interface IPedidoRepositorio
{
    /// <exception cref="RepositorioException">Quando não for possível gravar.</exception>
    void Salvar(Pedido pedido);

    /// <summary>Pedidos confirmados, do mais recente para o mais antigo.</summary>
    /// <exception cref="RepositorioException">Quando não for possível ler.</exception>
    IReadOnlyList<Pedido> ListarMaisRecentes();
}
