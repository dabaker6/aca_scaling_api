using aca_scaling_api.Contracts;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace aca_scaling_api.Tests.Contracts
{
    public class HttpContextCorrelationExtensionsTests
    {

        [Fact]
        public void GetCorrelationId_WhenCorrelationIdStoredInItems_ReturnsIt()
        {
            var context = new DefaultHttpContext();
            context.Items[CorrelationConstants.ItemKey] = "my-correlation-id";

            var result = context.GetCorrelationId();

            result.Should().Be("my-correlation-id");
        }

        [Fact]
        public void GetCorrelationId_WhenItemsAreEmpty_ReturnsTraceIdentifier()
        {
            var context = new DefaultHttpContext();
            context.TraceIdentifier = "trace-abc-123";

            var result = context.GetCorrelationId();

            result.Should().Be("trace-abc-123");
        }

        [Fact]
        public void GetCorrelationId_WhenCorrelationIdIsWhitespace_ReturnsTraceIdentifier()
        {
            var context = new DefaultHttpContext();
            context.Items[CorrelationConstants.ItemKey] = "   ";
            context.TraceIdentifier = "trace-abc-123";

            var result = context.GetCorrelationId();

            result.Should().Be("trace-abc-123");
        }

        [Fact]
        public void GetCorrelationId_WhenCorrelationIdIsNull_ReturnsTraceIdentifier()
        {
            var context = new DefaultHttpContext();
            context.Items[CorrelationConstants.ItemKey] = null;
            context.TraceIdentifier = "trace-abc-123";

            var result = context.GetCorrelationId();

            result.Should().Be("trace-abc-123");
        }

        [Fact]
        public void GetCorrelationId_WhenItemKeyIsNotString_ReturnsTraceIdentifier()
        {
            var context = new DefaultHttpContext();
            context.Items[CorrelationConstants.ItemKey] = 42;
            context.TraceIdentifier = "trace-abc-123";

            var result = context.GetCorrelationId();

            result.Should().Be("trace-abc-123");
        }
    }
}
