namespace Dominio;

public class Usuario
{
    public int UsuarioId { get; set; }
    public string? CPF { get; set; }
    public string? Telefone { get; set; }
    public string? Senha { get; set; }
    public bool Ativo { get; set; }
    public string? Nome { get; set; }
    public string? Sobrenome { get; set; }



    public Usuario()
    {
        Ativo = true;
    }

    public void Deletar()
    {
        Ativo = false;
    }

    public void Restaurar()
    {
        Ativo = true;
    }

}