using System;
using System.Reactive.Linq;
using DemoAndDiscourse.Utils;
using Grpc.Core;

namespace DemoAndDiscourse.Logic
{
    public static class Extensions
    {
        public static IObservable<T> AsObservable<T>(this IAsyncStreamReader<T> streamReader) where T : class =>
            Observable.FromAsync(async _ =>
                {
                    var hasNext = streamReader != null && await streamReader.MoveNext();
                    return hasNext ? streamReader.Current : null;
                })
                .Repeat()
                .TakeWhile(data => data.IsNotNullOrDefault());
    }
}