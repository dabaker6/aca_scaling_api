using FluentValidation;

namespace aca_scaling_api.Validation
{
    internal sealed class SendMessageRequestValidator: AbstractValidator<SendMessageRequest>
    {
        public SendMessageRequestValidator() 
        {
            int maxMessageCount = 5000;
            RuleFor(request => request.MessageCount)
                .LessThanOrEqualTo(maxMessageCount)
                .WithMessage($"Message Count must be less than {maxMessageCount}");            
        }
    }
}
