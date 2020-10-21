using GraphQL.Api.GraphQL.Mutations;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace GraphQL.Api.GraphQL
{
    public class ConferenceSchema : Schema
    {
        public ConferenceSchema(IServiceProvider provider) : base(provider)
        {
            Query = provider.GetRequiredService<ConferenceQuery>();
            Mutation = provider.GetRequiredService<ConferenceMutation>();
        }
    }
}
