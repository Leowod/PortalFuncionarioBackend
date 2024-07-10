using Dominio;
using Repositorio;

namespace Aplicacao;

public class AplicacaoUsuario : IAplicacaoUsuario
{
    private readonly IRepositorioUsuario _usuarioRepositorio;

    public AplicacaoUsuario(IRepositorioUsuario usuarioRepositorio)
    {
        _usuarioRepositorio = usuarioRepositorio;
    }

    public Task AlterarSenhaAsync(Usuario usuario, string novaSenha)
    {
        throw new NotImplementedException();
    }

    public async Task AtualizarAsync(Usuario usuario)
    {
        var usuarioDominio = await _usuarioRepositorio.ObterIdAsync(usuario.UsuarioId);

        if (usuarioDominio == null)
            throw new Exception("Usuário não encontrado!");

        ValidarInformacoesUsuario(usuario);

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

        await _usuarioRepositorio.AtualizarAsync(usuarioDominio); ;
    }

    public Task<IEnumerable<Usuario>> ListarAsync(bool ativo)
    {
        throw new NotImplementedException();
    }

    public async Task<Usuario> Logar(string nomeUsuario, string senha)
    {
        // if (!string.IsNullOrEmpty(nomeUsuario) || !string.IsNullOrEmpty(senha))
        //     throw new Exception("Credenciais inválidas!");

        // var usuarioDominio = await _usuarioRepositorio.OberNomeUsuarioAsync(nomeUsuario);

        // if (usuarioDominio == null)
        //     throw new Exception("Nome de Usuario invalido");

        // if (usuarioDominio.Senha != senha)
        //     throw new Exception("Senha inválida!");

        // return usuarioDominio;
        throw new NotImplementedException();
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
        if (usuario.CPF == null)
            throw new Exception("CPF não pode ser vazio!");

        if (string.IsNullOrEmpty(usuario.Telefone))
            throw new Exception("Favor informar um número de telefone.");

        if (string.IsNullOrEmpty(usuario.Sobrenome))
            throw new Exception("Sobrenome não pode ser vazio.");
    }
}
