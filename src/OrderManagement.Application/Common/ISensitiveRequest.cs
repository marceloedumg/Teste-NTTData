namespace OrderManagement.Application.Common;

/// <summary>
/// Marca requisições cujos dados não podem aparecer nos logs, como senhas e tokens.
/// </summary>
public interface ISensitiveRequest;
