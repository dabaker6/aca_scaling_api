using FluentValidation;

namespace aca_scaling_api.Validation
{
    internal sealed class ReplicaCountRequestValidator: AbstractValidator<ReplicaCountRequest>
    {
        public ReplicaCountRequestValidator() 
        {
            RuleFor(request => request.revisionName)
                .NotEmpty().WithMessage("Revision name must not be empty.")
                .MaximumLength(100).WithMessage("Revision name must not exceed 100 characters.")
                .Matches("^[a-zA-Z0-9-]+$").WithMessage("Revision name can only contain alphanumeric characters and hyphens.");            
        }
    }
}
