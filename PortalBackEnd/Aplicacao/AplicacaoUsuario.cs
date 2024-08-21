using Dominio;
using Microsoft.EntityFrameworkCore;
using Repositorio;
using System;
using System.Threading.Tasks;


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
        usuarioDominio.Endereco = usuario.Endereco;

        await _usuarioRepositorio.AtualizarAsync(usuarioDominio);
    }

    public async Task AtualizarUsuarioLogado(string cpf, Usuario usuarioAtualizado)
    {
        if (string.IsNullOrEmpty(cpf))
            throw new ArgumentException("CPF não pode ser vazio!", nameof(cpf));

        var usuarioDominio = await _usuarioRepositorio.ObterPorCpfAsync(cpf);

        if (usuarioDominio == null)
            throw new Exception("Usuário não encontrado!");


        ValidarAlteracoesUsuario(usuarioAtualizado);

        usuarioDominio.Telefone = usuarioAtualizado.Telefone;
        usuarioDominio.Nome = usuarioAtualizado.Nome;
        usuarioDominio.Sobrenome = usuarioAtualizado.Sobrenome;

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

    public async Task DeletarUsuarioLogado(string cpf)
    {
        if (string.IsNullOrEmpty(cpf))
            throw new ArgumentException("CPF não pode ser vazio!", nameof(cpf));

        var usuarioDominio = await _usuarioRepositorio.ObterPorCpfAsync(cpf);

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

    public async Task<Usuario> ObterPorCpfAsync(string cpf)
    {
        if (string.IsNullOrEmpty(cpf))
            throw new ArgumentException("CPF não pode ser vazio!", nameof(cpf));

        var usuario = await _usuarioRepositorio.ObterPorCpfAsync(cpf);

        if (usuario == null)
            throw new Exception("Usuário não encontrado!");

        return usuario;
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

    public async Task RestaurarPorCpfAsync(string cpf)
    {
        if (string.IsNullOrEmpty(cpf))
            throw new ArgumentException("CPF não pode ser vazio!", nameof(cpf));

        var usuario = await _usuarioRepositorio.ObterPorCpfAsync(cpf);

        if (usuario == null)
            throw new Exception("Usuário não encontrado!");

        usuario.Ativo = true;

        await _usuarioRepositorio.AtualizarAsync(usuario);
    }


    public async Task<int> CriarAsync(Usuario usuario)
    {
        if (usuario == null)
            throw new Exception("Usuário não pode ser vazio!");

        if (string.IsNullOrEmpty(usuario.Nome))
            throw new Exception("Nome não pode ser vazio!");

        if (string.IsNullOrWhiteSpace(usuario.Nome))
            throw new Exception("Nome não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(usuario.Telefone))
            throw new Exception("Telefone não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(usuario.Sobrenome))
            throw new Exception("Sobrenome não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(usuario.Endereco))
            throw new Exception("Endereço não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(usuario.Senha))
            throw new Exception("Senha não pode ser vazia.");

        if (string.IsNullOrWhiteSpace(usuario.CPF))
            throw new Exception("CPF não pode ser vazio.");

        var usuarioCpf = await _usuarioRepositorio.ObterPorCpfAsync(usuario.CPF);

        if (usuarioCpf != null)
            throw new Exception("CPF já cadastrado.");


        return await _usuarioRepositorio.AdicionarAsync(usuario);
    }

    async void ValidarInformacoesUsuario(Usuario usuario)
    {
        if (string.IsNullOrWhiteSpace(usuario.Nome))
            throw new Exception("Nome não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(usuario.Telefone))
            throw new Exception("Telefone não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(usuario.Sobrenome))
            throw new Exception("Sobrenome não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(usuario.Endereco))
            throw new Exception("Endereço não pode ser vazio.");

        if (string.IsNullOrWhiteSpace(usuario.Senha))
            throw new Exception("Senha não pode ser vazia.");

        if (string.IsNullOrWhiteSpace(usuario.CPF))
            throw new Exception("CPF não pode ser vazio.");

        var usuarioCpf = await _usuarioRepositorio.ObterPorCpfAsync(usuario.CPF);

        if (usuarioCpf != null)
            throw new Exception("CPF já cadastrado.");

    }

    static void ValidarAlteracoesUsuario(Usuario usuario)
    {
        if (string.IsNullOrEmpty(usuario.Nome))
            throw new Exception("Nome não pode ser vazio.");

        if (string.IsNullOrEmpty(usuario.Telefone))
            throw new Exception("Telefone não pode ser vazio.");

        if (string.IsNullOrEmpty(usuario.Sobrenome))
            throw new Exception("Sobrenome não pode ser vazio.");
    }
}