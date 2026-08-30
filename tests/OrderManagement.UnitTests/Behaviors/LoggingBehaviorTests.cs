using MediatR;
using Microsoft.Extensions.Logging;
using OrderManagement.Application.Authentication;
using OrderManagement.Application.Behaviors;

namespace OrderManagement.UnitTests.Behaviors;

public sealed class LoggingBehaviorTests
{
    [Fact]
    public async Task Handle_WithSensitiveRequest_OmitsCredentialsAndTokenFromLogs()
    {
        var logger = new RecordingLogger<LoggingBehavior<LoginCommand, LoginResponse>>();
        var behavior = new LoggingBehavior<LoginCommand, LoginResponse>(logger);

        var response = await behavior.Handle(
            new LoginCommand("sensitive@example.com", "secret-password"),
            _ => Task.FromResult(new LoginResponse("secret-token", DateTime.UtcNow)),
            CancellationToken.None);

        Assert.Equal("secret-token", response.AccessToken);
        Assert.Contains(logger.Entries, entry => entry.Message.Contains("payload omitted"));
        Assert.Contains(logger.Entries, entry => entry.Message.Contains("response omitted"));
        Assert.All(logger.Entries, entry =>
        {
            Assert.DoesNotContain("sensitive@example.com", entry.Message);
            Assert.DoesNotContain("secret-password", entry.Message);
            Assert.DoesNotContain("secret-token", entry.Message);
        });
    }

    [Fact]
    public async Task Handle_WithRegularRequest_LogsRequestAndResponse()
    {
        var logger = new RecordingLogger<LoggingBehavior<TestRequest, string>>();
        var behavior = new LoggingBehavior<TestRequest, string>(logger);

        var response = await behavior.Handle(
            new TestRequest("visible-value"),
            _ => Task.FromResult("visible-response"),
            CancellationToken.None);

        Assert.Equal("visible-response", response);
        Assert.Contains(logger.Entries, entry => entry.Message.Contains("visible-value"));
        Assert.Contains(logger.Entries, entry => entry.Message.Contains("visible-response"));
    }

    [Fact]
    public async Task Handle_WhenNextThrows_LogsWarningAndRethrowsSameException()
    {
        var logger = new RecordingLogger<LoggingBehavior<TestRequest, string>>();
        var behavior = new LoggingBehavior<TestRequest, string>(logger);
        var expectedException = new InvalidOperationException("expected failure");

        var actualException = await Assert.ThrowsAsync<InvalidOperationException>(() => behavior.Handle(
            new TestRequest("value"),
            _ => Task.FromException<string>(expectedException),
            CancellationToken.None));

        Assert.Same(expectedException, actualException);
        var warning = Assert.Single(logger.Entries, entry => entry.Level == LogLevel.Warning);
        Assert.Same(expectedException, warning.Exception);
        Assert.Contains(nameof(TestRequest), warning.Message);
    }

    private sealed record TestRequest(string Value) : IRequest<string>;

    /// <summary>
    /// Captura a mensagem já formatada para validar conteúdo e redaction sem depender de um provedor externo.
    /// </summary>
    private sealed class RecordingLogger<T> : ILogger<T>
    {
        internal List<LogEntry> Entries { get; } = [];

        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter) =>
            Entries.Add(new LogEntry(logLevel, formatter(state, exception), exception));
    }

    private sealed record LogEntry(LogLevel Level, string Message, Exception? Exception);
}
