namespace OrderManagement.UnitTests.Common;

/// <summary>
/// Relógio determinístico usado para testar CreatedAt sem depender do horário da máquina.
/// </summary>
internal sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => utcNow;
}
