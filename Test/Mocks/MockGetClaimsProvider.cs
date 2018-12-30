// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using DataAuthorize;

namespace Test.Mocks
{
    public class MockGetClaimsProvider : IGetClaimsProvider
    {
        public MockGetClaimsProvider(string userId, int shopKey)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            ShopKey = shopKey;
        }

        public string UserId { get; }
        public int ShopKey { get; }

    }
}