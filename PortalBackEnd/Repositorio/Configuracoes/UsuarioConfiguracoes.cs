

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Dominio;

public class UsuarioConfiguracoes : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("Usuario").HasKey(x => x.UsuarioId);
        builder.Property(nameof(Usuario.UsuarioId)).HasColumnName("UsuarioId");
        builder.Property(nameof(Usuario.Nome)).HasColumnName("Nome");
        builder.Property(nameof(Usuario.Sobrenome)).HasColumnName("Sobrenome");
        builder.Property(nameof(Usuario.CPF)).HasColumnName("CPF");        
        builder.Property(nameof(Usuario.Telefone)).HasColumnName("Telefone");
        builder.Property(nameof(Usuario.Endereco)).HasColumnName("Endereço");
        builder.Property(nameof(Usuario.Senha)).HasColumnName("Senha");
        builder.Property(nameof(Usuario.Ativo)).HasColumnName("Ativo");

    }
}