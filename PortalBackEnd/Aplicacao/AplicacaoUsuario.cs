using Dominio;
using Repositorio;


public class AplicacaoUsuario : IAplicacaoUsuario
{
    private readonly IRepositorioUsuario _usuarioRepositorio;

    public AplicacaoUsuario(IRepositorioUsuario usuarioRepositorio)
    {
        _usuarioRepositorio = usuarioRepositorio;
    }

    public async Task AtualizarAsync(Usuario usuario)
    {
        var usuarioDominio = await _usuarioRepositorio.ObterIdAsync(usuario.UsuarioId);

        if (usuarioDominio == null)
            throw new Exception("Usuário não encontrado!");

        ValidarAlteracoesUsuario(usuario);

        usuarioDominio.Telefone = usuario.Telefone;
        usuarioDominio.Nome = usuario.Nome;
        usuarioDominio.Sobrenome = usuario.Sobrenome;

        await _usuarioRepositorio.AtualizarAsync(usuarioDominio);
    }


    public async Task DeletarAsync(int id)
    {
        var usuarioDominio = await _usuarioRepositorio.ObterIdAsync(id);

        if (usuarioDominio == null)
            throw new Exception("Usuário não encontrado!");

        if (usuarioDominio.Ativo == false)
            throw new Exception("Usuário já deletado");

        usuarioDominio.Deletar();

        await _usuarioRepositorio.AtualizarAsync(usuarioDominio);
    }

    public async Task<Usuario> Logar(string cpf, string senha)
    {
        var usuariosAtivos = await _usuarioRepositorio.ListarAsync(true);

        var usuario = usuariosAtivos.FirstOrDefault(x => x.CPF == cpf && x.Senha == senha);

        if (usuario == null)
            throw new Exception("Credenciais inválidas!");

        return usuario;
    }

    public async Task AlterarSenhaAsync(Usuario usuario, string novaSenha)
    {
        var usuarioDominio = await _usuarioRepositorio.ObterIdAsync(usuario.UsuarioId);

        if (usuarioDominio == null)
            throw new Exception("Usuário não encontrado!");

        usuarioDominio.Senha = novaSenha;

        await _usuarioRepositorio.AtualizarAsync(usuarioDominio);
    }

    public async Task<IEnumerable<Usuario>> ListarAsync(bool ativo)
    {
        return await _usuarioRepositorio.ListarAsync(ativo);
    }

    public async Task<Usuario> ObterAsync(int id)
    {
        var usuarioDominio = await _usuarioRepositorio.ObterIdAsync(id);

        if (usuarioDominio == null)
            throw new Exception("Usuário não encontrado!");

        return usuarioDominio;
    }

    public async Task RestaurarAsync(int id)
    {
        var usuarioDominio = await _usuarioRepositorio.ObterIdAsync(id);

        if (usuarioDominio == null)
            throw new Exception("Usuário não encontrado!");

        if (usuarioDominio.Ativo == true)
            throw new Exception("Usuário já ativo");

        usuarioDominio.Restaurar();

        await _usuarioRepositorio.AtualizarAsync(usuarioDominio);
    }

    public async Task<int> CriarAsync(Usuario usuario)
    {
        if (usuario == null)
            throw new Exception("Usuário não pode ser vazio!");

        if (string.IsNullOrEmpty(usuario.Nome))
            throw new Exception("Nome não pode ser vazio!");

        ValidarInformacoesUsuario(usuario);

        return await _usuarioRepositorio.AdicionarAsync(usuario);
    }

    private static void ValidarInformacoesUsuario(Usuario usuario)
    {
        if (string.IsNullOrEmpty(usuario.Nome))
            throw new Exception("Nome não pode ser vazio.");

        if (string.IsNullOrEmpty(usuario.Telefone))
            throw new Exception("Telefone não pode ser vazio.");

        if (string.IsNullOrEmpty(usuario.Sobrenome))
            throw new Exception("Sobrenome não pode ser vazio.");

        if (string.IsNullOrEmpty(usuario.CPF))
            throw new Exception("CPF não pode ser vazio.");
    }


    private static void ValidarAlteracoesUsuario(Usuario usuario)
    {
        if (string.IsNullOrEmpty(usuario.Nome))
            throw new Exception("Nome não pode ser vazio.");

        if (string.IsNullOrEmpty(usuario.Telefone))
            throw new Exception("Telefone não pode ser vazio.");

        if (string.IsNullOrEmpty(usuario.Sobrenome))
            throw new Exception("Sobrenome não pode ser vazio.");
    }
}
