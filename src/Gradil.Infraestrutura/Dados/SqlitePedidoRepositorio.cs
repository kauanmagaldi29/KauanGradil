using System.Globalization;
using Gradil.Core.Contratos;
using Gradil.Core.Modelos;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace Gradil.Infraestrutura.Dados;

public class SqlitePedidoRepositorio : IPedidoRepositorio
{
    private readonly string _connectionString;
    private readonly ILogger<SqlitePedidoRepositorio> _logger;

    public SqlitePedidoRepositorio(string caminhoDoBanco, ILogger<SqlitePedidoRepositorio> logger)
    {
        _connectionString = new SqliteConnectionStringBuilder { DataSource = caminhoDoBanco }.ToString();
        _logger = logger;
        CriarTabelaSeNaoExistir();
    }

    public void Salvar(Pedido pedido)
    {
        ArgumentNullException.ThrowIfNull(pedido);

        const string sql = """
            INSERT INTO Pedidos (DataConfirmacao, Comprimento, Altura, Pintura)
            VALUES ($data, $comprimento, $altura, $pintura);
            """;

        try
        {
            using var conexao = AbrirConexao();
            using var comando = conexao.CreateCommand();
            comando.CommandText = sql;
            comando.Parameters.AddWithValue("$data", pedido.DataConfirmacao.ToString("O", CultureInfo.InvariantCulture));
            comando.Parameters.AddWithValue("$comprimento", pedido.Comprimento);
            comando.Parameters.AddWithValue("$altura", pedido.Altura);
            comando.Parameters.AddWithValue("$pintura", pedido.Pintura);
            comando.ExecuteNonQuery();

            _logger.LogInformation("Pedido salvo: {Comprimento} m, altura {Altura} m, {Pintura}",
                pedido.Comprimento, pedido.Altura, pedido.Pintura);
        }
        catch (SqliteException ex)
        {
            _logger.LogError(ex, "Falha ao salvar pedido {@Pedido}", pedido);
            throw new RepositorioException("Não foi possível salvar o pedido.", ex);
        }
    }

    public IReadOnlyList<Pedido> ListarMaisRecentes()
    {
        const string sql = """
            SELECT DataConfirmacao, Comprimento, Altura, Pintura
            FROM Pedidos
            ORDER BY DataConfirmacao DESC, Id DESC;
            """;

        try
        {
            using var conexao = AbrirConexao();
            using var comando = conexao.CreateCommand();
            comando.CommandText = sql;

            var pedidos = new List<Pedido>();
            using var leitor = comando.ExecuteReader();
            while (leitor.Read())
            {
                pedidos.Add(new Pedido(
                    DateTime.Parse(leitor.GetString(0), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                    leitor.GetDecimal(1),
                    leitor.GetDecimal(2),
                    leitor.GetString(3)));
            }

            return pedidos;
        }
        catch (SqliteException ex)
        {
            _logger.LogError(ex, "Falha ao listar pedidos");
            throw new RepositorioException("Não foi possível carregar os pedidos.", ex);
        }
    }

    private void CriarTabelaSeNaoExistir()
    {
        const string sql = """
            CREATE TABLE IF NOT EXISTS Pedidos (
                Id              INTEGER PRIMARY KEY AUTOINCREMENT,
                DataConfirmacao TEXT    NOT NULL,
                Comprimento     NUMERIC NOT NULL,
                Altura          NUMERIC NOT NULL,
                Pintura         TEXT    NOT NULL
            );
            """;

        try
        {
            using var conexao = AbrirConexao();
            using var comando = conexao.CreateCommand();
            comando.CommandText = sql;
            comando.ExecuteNonQuery();
        }
        catch (SqliteException ex)
        {
            _logger.LogCritical(ex, "Não foi possível preparar o banco de dados");
            throw new RepositorioException("Não foi possível preparar o banco de dados.", ex);
        }
    }

    private SqliteConnection AbrirConexao()
    {
        var conexao = new SqliteConnection(_connectionString);
        conexao.Open();
        return conexao;
    }
}
