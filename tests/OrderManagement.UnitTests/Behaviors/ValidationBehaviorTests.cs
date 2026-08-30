using FluentValidation;
using MediatR;
using OrderManagement.Application.Behaviors;

namespace OrderManagement.UnitTests.Behaviors;

public sealed class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_WithValidRequest_InvokesNextBehavior()
    {
        var nextWasCalled = false;
        var behavior = new ValidationBehavior<TestRequest, string>([new TestRequestValidator()]);

        var response = await behavior.Handle(
            new TestRequest("valid"),
            _ =>
            {
                nextWasCalled = true;
                return Task.FromResult("handled");
            },
            CancellationToken.None);

        Assert.True(nextWasCalled);
        Assert.Equal("handled", response);
    }

    [Fact]
    public async Task Handle_WithInvalidRequest_ThrowsAndDoesNotInvokeNextBehavior()
    {
        var nextWasCalled = false;
        var behavior = new ValidationBehavior<TestRequest, string>([new TestRequestValidator()]);

        var exception = await Assert.ThrowsAsync<ValidationException>(() => behavior.Handle(
            new TestRequest(string.Empty),
            _ =>
            {
                nextWasCalled = true;
                return Task.FromResult("handled");
            },
            CancellationToken.None));

        Assert.False(nextWasCalled);
        Assert.Contains(exception.Errors, error => error.PropertyName == nameof(TestRequest.Value));
    }

    private sealed record TestRequest(string Value) : IRequest<string>;

    private sealed class TestRequestValidator : AbstractValidator<TestRequest>
    {
        internal TestRequestValidator()
        {
            RuleFor(request => request.Value).NotEmpty();
        }
    }
}
