// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAuthorize;

namespace DataLayer.EfClasses.MultiTenantClasses
{
    public class StockInfo : IShopKey
    {
        public int StockInfoId { get; set; }
        public string Name { get; set; }
        public int NumInStock { get; set; }
        public int ShopKey { get; private set; }

        public void SetShopKey(int shopKey)
        {
            ShopKey = shopKey;
        }

        [MaxLength(40)]
        public string DistrictManagerId { get; set; }

        //---------------------------------------------
        //relationships

        [ForeignKey(nameof(ShopKey))]
        public Shop AtShop { get; set; }

        public override string ToString()
        {
            return $"{nameof(StockInfoId)}: {StockInfoId}, {nameof(Name)}: {Name}, {nameof(NumInStock)}: {NumInStock}, {nameof(ShopKey)}: {ShopKey}, {nameof(DistrictManagerId)}: {DistrictManagerId}";
        }
    }
}