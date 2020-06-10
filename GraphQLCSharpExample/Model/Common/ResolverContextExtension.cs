using System;
using HotChocolate.Resolvers;
using HotChocolate.Language;

namespace GraphQLCSharpExample.Model.Common
{
    public static class ResolverContextExtension
    {
        public static bool IsSingleField(
            this IResolverContext ctx, 
            string fieldName)
        {
            var selections = ctx.FieldSelection.SelectionSet.Selections;
            if (selections.Count == 1)
            {
                FieldNode? fieldNode = selections[0] as FieldNode;
                if (fieldNode != null)
                {
                    return fieldNode.Name.Value == fieldName;
                }
            }
            return false;
        }
    }
}
