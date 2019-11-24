using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DemoAndDiscourse.Utils;
using FluentValidation;
using Grpc.Core;

namespace DemoAndDiscourse.Logic
{
    public static class Extensions
    {
        public static T UpdateObject<T>(this T destination, T source)
        {
            foreach (var property in typeof(T).GetProperties().Where(p => p.CanWrite))
            {
                var sourceValue = property.GetValue(source, null);
                var destinationValue = property.GetValue(destination, null);

                if (sourceValue is null || destinationValue != null && !string.IsNullOrEmpty(destinationValue.ToString())) continue;

                property.SetValue(destination, sourceValue, null);
            }

            return destination;
        }

        public static IObservable<T> AsObservable<T>(this IAsyncStreamReader<T> streamReader) where T : class =>
            Observable.FromAsync(async _ =>
                {
                    var hasNext = streamReader != null && await streamReader.MoveNext();
                    return hasNext ? streamReader.Current : null;
                })
                .Repeat()
                .TakeWhile(data => data.IsNotNullOrDefault());

        public static async Task ValidateOrThrowAsync<T>(this IValidator<T> validator, T objectToValidate)
        {
            try
            {
                await validator.ValidateAndThrowAsync(objectToValidate);
            }
            catch (ValidationException validationException)
            {
                var errorMessages = string.Join(" ", validationException.Errors.Select(e => e.ErrorMessage));
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Validation failed."), errorMessages);
            }
        }
    }
}