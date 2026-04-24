using FluentValidation;

namespace aca_scaling_api.Validation
{
    public sealed class SendMessageRequestValidator: AbstractValidator<SendMessageRequest>
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
