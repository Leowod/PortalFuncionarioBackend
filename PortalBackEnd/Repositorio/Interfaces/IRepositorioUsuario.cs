using Dominio;

public interface IRepositorioUsuario
{
    Task<int> AdicionarAsync(Usuario usuario);
    Task AtualizarAsync(Usuario usuario);
    Task<Usuario> ObterIdAsync(int id);
    Task<IEnumerable<Usuario>> ListarAsync(bool ativo);
    Task<Usuario> ObterPorCpfAsync(string cpf);
}
