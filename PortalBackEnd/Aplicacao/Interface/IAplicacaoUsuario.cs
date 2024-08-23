using Dominio;
using Repositorio;

public interface IAplicacaoUsuario
{
    Task<int> CriarAsync(Usuario usuario);
    Task AtualizarAsync(Usuario usuario);
    Task AtualizarUsuarioLogado(string cpf, Usuario usuario);
    Task<Usuario> Logar(string cpf, string senha);
    Task AlterarSenhaAsync(int id, string senha, string novaSenha);
    Task DeletarAsync(int id);
    Task DeletarUsuarioLogado(string cpf);
    Task RestaurarAsync(int id);
    Task RestaurarPorCpfAsync(string cpf);
    Task<IEnumerable<Usuario>> ListarAsync(bool ativo);
    Task<Usuario> ObterAsync(int id);
    Task<Usuario> ObterPorCpfAsync(string cpf);
}