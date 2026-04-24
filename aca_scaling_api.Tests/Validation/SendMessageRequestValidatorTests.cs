using aca_scaling_api.Validation;
using FluentAssertions;
using Xunit;

namespace aca_scaling_api.Tests.Validation
{
    public class SendMessageRequestValidatorTests
    {
        private readonly SendMessageRequestValidator _validator;

        public SendMessageRequestValidatorTests()
        {
            _validator = new SendMessageRequestValidator();
        }

        [Fact]
        public async Task Validate_WithValidMessageCount_ReturnsSuccess()
        {
            // Arrange
            var request = new SendMessageRequest(100);

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validate_WithZeroMessageCount_ReturnsSuccess()
        {
            // Arrange
            var request = new SendMessageRequest(0);

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validate_WithMessageCountAtMaxLimit_ReturnsSuccess()
        {
            // Arrange
            var request = new SendMessageRequest(5000);

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validate_WithMessageCountAboveMaxLimit_ReturnsFail()
        {
            // Arrange
            var request = new SendMessageRequest(5001);

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors[0].ErrorMessage.Should().Contain("Message Count must be less than 5000");
        }

        [Fact]
        public async Task Validate_WithLargeMessageCount_ReturnsFail()
        {
            // Arrange
            var request = new SendMessageRequest(10000);

            // Act
            var result = await _validator.ValidateAsync(request);

            // Assert
            result.IsValid.Should().BeFalse();
        }
    }
}
