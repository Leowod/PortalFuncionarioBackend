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
    public async Task<ActionResult<UsuarioResposta>> ObterUsuarioPorIdAsync([FromRoute] int usuarioId)
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

    [HttpPost("Cadastrar")]
    public async Task<IActionResult> UsuarioCadastrarAsync([FromBody] UsuarioCriar usuario)
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
    public async Task<IActionResult> UsuarioAtualizarAsync([FromRoute] int id, [FromBody] UsuarioAtualizar usuarioAtualizar)
    {
        try
        {
            var usuario = new Usuario()
            {
                UsuarioId = id,
                Nome = usuarioAtualizar.Nome,
                Sobrenome = usuarioAtualizar.Sobrenome,
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


    [HttpPut("AlterarSenha")]
    public async Task<IActionResult> AlterarSenhaAsync([FromBody] UsuarioAlterarSenha usuarioAlterarSenha)
    {
        if (usuarioAlterarSenha.NovaSenha == null)
        {
            return BadRequest("A nova senha não pode ser nula.");
        }

        try
        {
            Usuario usuario = new Usuario()
            {
                UsuarioId = usuarioAlterarSenha.Id
            };

            await _aplicacaoUsuario.AlterarSenhaAsync(usuario, usuarioAlterarSenha.NovaSenha);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }


    [HttpPost("Logar")]
    public async Task<IActionResult> LogarAsync([FromBody] UsuarioAutenticar usuarioAutenticar)
    {
        if (usuarioAutenticar.CPF == null || usuarioAutenticar.Senha == null)
        {
            return BadRequest("CPF e senha não podem ser nulos.");
        }

        try
        {
            var usuarioAutenticado = await _aplicacaoUsuario.Logar(usuarioAutenticar.CPF, usuarioAutenticar.Senha);

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

