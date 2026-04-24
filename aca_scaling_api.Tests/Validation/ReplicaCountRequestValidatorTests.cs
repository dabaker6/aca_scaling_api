using aca_scaling_api.Validation;
using FluentAssertions;
using Xunit;

namespace aca_scaling_api.Tests.Validation
{
    public class ReplicaCountRequestValidatorTests
    {
        private readonly ReplicaCountRequestValidator _validator;

        public ReplicaCountRequestValidatorTests()
        {
            _validator = new ReplicaCountRequestValidator();
        }

        [Fact]
        public async Task Validate_WithValidRevisionName_ReturnsSuccess()
        {
            var request = new ReplicaCountRequest("my-revision-1");

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validate_WithEmptyRevisionName_ReturnsFail()
        {
            var request = new ReplicaCountRequest(string.Empty);

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "Revision name must not be empty.");
        }

        [Fact]
        public async Task Validate_WithRevisionNameExceeding100Chars_ReturnsFail()
        {
            var longName = new string('a', 101);
            var request = new ReplicaCountRequest(longName);

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "Revision name must not exceed 100 characters.");
        }

        [Fact]
        public async Task Validate_WithRevisionNameAtMaxLength_ReturnsSuccess()
        {
            var maxLengthName = new string('a', 100);
            var request = new ReplicaCountRequest(maxLengthName);

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task Validate_WithInvalidCharacters_ReturnsFail()
        {
            var request = new ReplicaCountRequest("revision_with_underscores");

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.ErrorMessage == "Revision name can only contain alphanumeric characters and hyphens.");
        }

        [Fact]
        public async Task Validate_WithSpecialCharacters_ReturnsFail()
        {
            var request = new ReplicaCountRequest("revision@name!");

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeFalse();
        }

        [Fact]
        public async Task Validate_WithAlphanumericAndHyphens_ReturnsSuccess()
        {
            var request = new ReplicaCountRequest("my-revision-abc123");

            var result = await _validator.ValidateAsync(request);

            result.IsValid.Should().BeTrue();
        }
    }
}
