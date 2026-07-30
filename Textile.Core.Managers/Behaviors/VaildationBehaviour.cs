using FluentValidation;
using MediatR;
using Textile.Core.Entities.Exceptions;

namespace Textile.Core.Managers.Behaviors
{

    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {

        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);

            var errorsDictionary = _validators
                                        .Select(x => x.Validate(context))
                                        .SelectMany(x => x.Errors)
                                        .Where(x => x != null && x.Severity == Severity.Error)
                                        .GroupBy(
                                            x => x.PropertyName.Substring(x.PropertyName.IndexOf('.') + 1), //Fluent API adds . in property name. This code is to clean up that property name
                                            x => x.ErrorMessage, (propertyName, errorMessage) => new
                                            {
                                                Key = propertyName,
                                                Values = errorMessage.Distinct().ToArray()
                                            })
                                        .ToDictionary(x => x.Key, x => x.Values);


            var warningsDictionary = _validators
                                       .Select(x => x.Validate(context))
                                       .SelectMany(x => x.Errors)
                                       .Where(x => x != null && x.Severity == Severity.Warning)
                                       .GroupBy(
                                           x => x.PropertyName.Substring(x.PropertyName.IndexOf('.') + 1), //Fluent API adds . in property name. This code is to clean up that property name
                                           x => x.ErrorMessage, (propertyName, errorMessage) => new
                                           {
                                               Key = propertyName,
                                               Values = errorMessage.Distinct().ToArray()
                                           })
                                       .ToDictionary(x => x.Key, x => x.Values);


            if (errorsDictionary.Any() || warningsDictionary.Any())
                throw new TextTileValidationException(errorsDictionary, warningsDictionary);

            return await next();
        }
    }
}



