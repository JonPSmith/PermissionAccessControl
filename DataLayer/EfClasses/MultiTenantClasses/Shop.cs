// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using DataAuthorize;

namespace DataLayer.EfClasses.MultiTenantClasses
{
    public class Shop : IShopKey
    {
        [Key]
        public int ShopKey { get; private set; }
        public void SetShopKey(int shopKey)
        {
            ShopKey = shopKey;
        }

        public string Name { get; set; }


    }
}