using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Denhub.API.Utils {
    /// <summary>
    /// This filter enriches swagger docs endpoints with authorization related symbology and information (eg. padlocks)
    /// </summary>
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasAuthorize =
                context.MethodInfo.DeclaringType is not null &&
                (context.MethodInfo.DeclaringType.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any()
                 || context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any());

            if (hasAuthorize)
            {
                operation.Responses.Add("401", new OpenApiResponse
                {
                    Description = "Unauthorized"
                });
                operation.Responses.Add("403", new OpenApiResponse
                {
                    Description = "Forbidden"
                });

                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new()
                    {
                        [
                            new OpenApiSecurityScheme
                            {
                                Type = SecuritySchemeType.Http,
                                Scheme = "basic",
                                Reference = new OpenApiReference { Id = "BasicAuth", Type = ReferenceType.SecurityScheme }
                            }
                        ] = System.Array.Empty<string>()
                    }
                };
            }
        }
    }

}