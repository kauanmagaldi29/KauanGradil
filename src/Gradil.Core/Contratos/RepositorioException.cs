namespace Gradil.Core.Contratos;

// Esconde os detalhes do banco (SqliteException etc.) de quem usa o repositório.
// A tela só precisa saber que algo deu errado e mostrar uma mensagem amigável.
public class RepositorioException(string mensagem, Exception causa) : Exception(mensagem, causa);
