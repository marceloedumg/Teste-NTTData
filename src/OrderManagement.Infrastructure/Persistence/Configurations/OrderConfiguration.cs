using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderManagement.Domain.Orders;

namespace OrderManagement.Infrastructure.Persistence.Configurations;

/// <summary>
/// Define o mapeamento relacional da raiz do agregado sem adicionar atributos de EF ao domínio.
/// </summary>
internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(order => order.Id);

        builder.Property(order => order.CustomerId)
            .IsRequired();

        builder.Property(order => order.Status)
            // Texto torna o banco legível e evita que reordenações futuras do enum alterem o significado salvo.
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(order => order.CreatedAt)
            // SQLite não preserva DateTimeKind; a conversão restaura UTC ao materializar a entidade.
            .HasConversion(
                createdAt => createdAt,
                storedValue => DateTime.SpecifyKind(storedValue, DateTimeKind.Utc))
            .IsRequired();

        // TotalAmount é derivado dos itens no domínio e não deve virar uma fonte de verdade duplicada.
        builder.Ignore(order => order.TotalAmount);

        builder.HasIndex(order => order.CustomerId);
        builder.HasIndex(order => order.CreatedAt);

        builder.HasMany(order => order.Items)
            .WithOne()
            .HasForeignKey(item => item.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(order => order.Items)
            // O EF escreve no campo privado para não precisar expor uma coleção mutável no agregado.
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
