// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace DataAuthorize
{
    public class GetClaimsFromUser : IGetClaimsProvider
    {
        public const string ShopIdClaimName = "ShopId";

        public string UserId { get; private set; }
        public string ShopKey { get; private set; }

        public GetClaimsFromUser(IHttpContextAccessor accessor)
        {
            UserId = accessor.HttpContext?.User.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            ShopKey = accessor.HttpContext?.User.Claims.SingleOrDefault(x => x.Type == ShopIdClaimName)?.Value;
        }
    }
}