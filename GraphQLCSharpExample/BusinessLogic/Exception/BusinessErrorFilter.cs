using System;
using System.Collections.Generic;
using HotChocolate;

namespace GraphQLCSharpExample.BusinessLogic.Exception
{
    public class BusinessErrorFilter : IErrorFilter
    {
        public IError OnError(IError error)
        {
            BusinessException? be = error.Exception as BusinessException;
            if (be != null)
            {
                var builder = ErrorBuilder
                    .New()
                    .SetCode($"BUSINESS:{be.Code}")
                    .SetMessage(be.Message);
                foreach (KeyValuePair<string, object> pair in be.Fields)
                {
                    builder.SetExtension(pair.Key, pair.Value);
                }
                return builder.Build();
            }
            return error;
        }
    }
}
