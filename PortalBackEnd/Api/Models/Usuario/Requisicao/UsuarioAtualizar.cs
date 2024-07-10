namespace Api;

public class UsuarioAtualizar
{
    public string Nome { get; set; }
    public string Sobrenome { get; set; }
    public string Senha { get; set; }
    public string Telefone { get; set; }
    public int Id { get; internal set; }
}