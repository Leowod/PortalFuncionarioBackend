using Microsoft.AspNetCore.Mvc;
using Dominio;


namespace Api;


[Route("[Controller]")]
[ApiController]
public class UsuarioController : Controller
{
    private readonly IAplicacaoUsuario _aplicacaoUsuario;
    
    public UsuarioController(IAplicacaoUsuario aplicacaoUsuario)
    {
        _aplicacaoUsuario = aplicacaoUsuario;
    }

    [HttpGet("HealthCheck")]
    public IActionResult HealthCheckAsync()
    {
        return Ok(new { status = "Conexao bem-sucedida" });
    }

    [HttpGet("Obter/{usuarioId}")]
    public async Task<IActionResult> ObterUsuarioPorIdAsync([FromRoute] int usuarioId)
    {
        try
        {
            var usuario = await _aplicacaoUsuario.ObterAsync(usuarioId);

            var usuarioResposta = new UsuarioResposta()
            {
                Id = usuario.UsuarioId,
                Nome = usuario.Nome,
                Sobrenome = usuario.Sobrenome,
                Telefone = usuario.Telefone,
                Ativo = usuario.Ativo,
            };

            return Ok(usuarioResposta);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("Listar")]
    public async Task<IActionResult> ListarAsync([FromQuery] bool ativo)
    {
        try
        {
            var usuarios = await _aplicacaoUsuario.ListarAsync(ativo);
            var usuarioResposta = usuarios.Select(usuario => new UsuarioResposta()
            {
                Id = usuario.UsuarioId,
                Nome = usuario.Nome,
                Sobrenome = usuario.Sobrenome,
                Telefone = usuario.Telefone,
                Ativo = usuario.Ativo,
            }).ToList();

            return Ok(usuarioResposta);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }

    }

    [HttpPost("Criar")]
    public async Task<IActionResult> UsuarioCriarAsync([FromBody] UsuarioCriar usuario)
    {
        try
        {
            var criarUsuario = new Usuario()
            {
                Nome = usuario.Nome, 
                CPF = usuario.CPF,
                Senha = usuario.Senha,
                Telefone = usuario.Telefone,
                Sobrenome = usuario.Sobrenome
            };
            var usuarioCriado = await _aplicacaoUsuario.CriarAsync(criarUsuario);
            return Ok(usuarioCriado);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("Atualizar/{id}")]
    public async Task<IActionResult> UsuarioAtualizarAsync([FromBody] UsuarioAtualizar usuarioAtualizar)
    {
        try
        {
            Usuario usuario = new Usuario()
            {
                UsuarioId = usuarioAtualizar.Id,
                Nome = usuarioAtualizar.Nome,
                Sobrenome = usuarioAtualizar.Sobrenome,
                Senha = usuarioAtualizar.Senha,
                Telefone = usuarioAtualizar.Telefone,
            };
            await _aplicacaoUsuario.AtualizarAsync(usuario);

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // [HttpPut("AlterarSenha")]
    // public async Task<IActionResult> AlterarSenhaAsync([FromBody] UsuarioAlterarSenha usuarioAlterarSenha)
    // {
    //     Usuario usuario = new Usuario()
    //     {
    //         ID = usuarioAlterarSenha.Id,
    //     };

    //     usuario.AlterarSenha(usuarioAlterarSenha.Senha);

    //     try
    //     {
    //         await _aplicacaoUsuario.AlterarSenhaAsync(usuario, usuarioAlterarSenha.NovaSenha);
    //     }
    //     catch (Exception ex)
    //     {
    //         return BadRequest(ex.Message);
    //     }

    //     return Ok();
    // }

    [HttpPost("Logar")]
    public async Task<IActionResult> LogarAsync([FromBody] UsuarioAutenticar usuarioAutenticar)
    {
        try
        {
            var usuariosAtivos = await _aplicacaoUsuario.ListarAsync(true);
            List<Usuario> listaUsuarios = usuariosAtivos.Select(x => new Usuario()
            {
                UsuarioId = x.UsuarioId,
                Nome = x.Nome,
                Sobrenome = x.Sobrenome,
                Telefone = x.Telefone,
                Senha = x.Senha,
            }).ToList();

            var usuarioAutenticado = listaUsuarios.FirstOrDefault(x => x.CPF == usuarioAutenticar.CPF && x.Senha == usuarioAutenticar.Senha);

            if (usuarioAutenticado == null)
            {
                return Unauthorized();
            }
            return Ok(new { message = "Usuário logado com sucesso", user = usuarioAutenticado });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("Delete/{usuarioId}")]
    public async Task<IActionResult> DeletarAsync([FromRoute] int usuarioId)
    {
        try
        {
            await _aplicacaoUsuario.DeletarAsync(usuarioId);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("Restaurar/{usuarioId}")]
    public async Task<IActionResult> RestaurarAsync([FromRoute] int usuarioId)
    {
        try
        {
            await _aplicacaoUsuario.RestaurarAsync(usuarioId);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
