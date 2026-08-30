namespace OrderManagement.Application.Common;

/// <summary>
/// Representa a ausência de um recurso esperado pelo caso de uso sem introduzir conceitos HTTP.
/// </summary>
public sealed class NotFoundException(string resourceName, object key)
    : Exception($"{resourceName} '{key}' was not found.");
