using Microsoft.AspNetCore.Mvc;
using Dominio;
using System.Text.RegularExpressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


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
                Endereco = usuario.Endereco,
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
                Endereco = usuario.Endereco,
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
                Endereco = usuario.Endereco,
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
                Endereco = usuarioAtualizar.Endereco,
            };

            await _aplicacaoUsuario.AtualizarAsync(usuario);

            var usuarioResposta = new UsuarioResposta()
            {
                Id = usuario.UsuarioId,
                Nome = usuario.Nome,
                Sobrenome = usuario.Sobrenome,
                Telefone = usuario.Telefone,
                Endereco = usuario.Endereco,
                Ativo = usuario.Ativo,
            };

            return Ok(usuarioResposta);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("Atualiza")]
    public async Task<IActionResult> AtualizarUsuario([FromBody] UsuarioAtualizar usuarioAtualizado)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized("Usuário não autorizado.");
        }

        if (!int.TryParse(userId, out var id))
        {
            return BadRequest("ID do usuário inválido.");
        }

        var usuarioExistente = await _aplicacaoUsuario.ObterAsync(id);

        if (usuarioExistente == null)
        {
            return NotFound("Usuário não encontrado.");
        }

        usuarioExistente.Nome = usuarioAtualizado.Nome;
        usuarioExistente.Sobrenome = usuarioAtualizado.Sobrenome;
        usuarioExistente.Telefone = usuarioAtualizado.Telefone;
        usuarioExistente.Endereco = usuarioAtualizado.Endereco;

        try
        {
            await _aplicacaoUsuario.AtualizarAsync(usuarioExistente);
            return Ok(usuarioExistente);
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
            await _aplicacaoUsuario.AlterarSenhaAsync(usuarioAlterarSenha.Id, usuarioAlterarSenha.Senha, usuarioAlterarSenha.NovaSenha);
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
            return BadRequest("Preencha todos os campos.");
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

    // [HttpDelete("Delete")]
    // [Authorize]
    // public async Task<IActionResult> DeletarUsuarioLogado()
    // {

    //     var cpf = User.FindFirst(ClaimTypes.Name)?.Value;

    //     if (cpf == null)
    //     {
    //         return Unauthorized("Usuário não autenticado.");
    //     }

    //     try
    //     {
    //         await _aplicacaoUsuario.DeletarUsuarioLogado(cpf);
    //         return Ok("Usuário desativado com sucesso.");
    //     }
    //     catch (Exception ex)
    //     {
    //         return BadRequest(ex.Message);
    //     }
    // }



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

    [HttpGet("Restaurar/{cpf}")]
    public async Task<IActionResult> RestaurarPorCpfAsync([FromRoute] string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
        {
            return BadRequest(new { mensagem = "CPF não pode ser nulo ou vazio." });
        }

        if (!IsValidCpfFormat(cpf))
        {
            return BadRequest(new { mensagem = "Formato de CPF inválido." });
        }

        try
        {
            var usuario = await _aplicacaoUsuario.ObterPorCpfAsync(cpf);

            if (usuario == null)
            {
                return NotFound(new { mensagem = "Usuário não encontrado." });
            }

            if (!usuario.Ativo)
            {
                await _aplicacaoUsuario.RestaurarPorCpfAsync(cpf);
                return Ok(new { mensagem = "Usuário restaurado com sucesso." });
            }

            return BadRequest(new { mensagem = "Usuário já está ativo." });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception: " + ex.Message);
            return StatusCode(500, new { mensagem = "Erro ao verificar CPF. Tente novamente mais tarde." });
        }
    }

    private static bool IsValidCpfFormat(string cpf)
    {
        return Regex.IsMatch(cpf, @"^\d{3}\.\d{3}\.\d{3}-\d{2}$");
    }

}