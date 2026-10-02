namespace Gradil.Core.Contratos;

public class RepositorioException(string mensagem, Exception causa) : Exception(mensagem, causa);
