using Dominio;
using Microsoft.EntityFrameworkCore;


public class RepositorioUsuario : IRepositorioUsuario
{
    private readonly Contexto _contexto;

    public RepositorioUsuario(Contexto contexto)
    {
        _contexto = contexto;
    }

    public async Task<int> AdicionarAsync(Usuario usuario)
    {
        await _contexto.Usuarios.AddAsync(usuario);
        await _contexto.SaveChangesAsync();

        return usuario.UsuarioId;
    }


    public async Task AtualizarAsync(Usuario usuario)
    {
        _contexto.Usuarios.Update(usuario);
        await _contexto.SaveChangesAsync();
    }

    public async Task<IEnumerable<Usuario>> ListarAsync(bool ativo)
    {
        return await _contexto.Usuarios
            .Where(x => x.Ativo == ativo)
            .ToListAsync();
    }

    public async Task<Usuario> ObterIdAsync(int usuarioId)
    {
        return await _contexto.Usuarios
            .Where(x => x.UsuarioId == usuarioId)
            .FirstOrDefaultAsync();
    }

    public async Task<Usuario> ObterPorCpfAsync(string cpf)
    {
        if (string.IsNullOrEmpty(cpf))
            throw new ArgumentException("CPF não pode ser vazio!", cpf);

        var user = _contexto.Usuarios.FirstOrDefault(u => u.CPF == cpf);
        return user;
    }
}