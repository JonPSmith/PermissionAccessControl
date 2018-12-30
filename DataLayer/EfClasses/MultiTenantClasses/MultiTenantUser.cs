// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAuthorize;

namespace DataLayer.EfClasses.MultiTenantClasses
{
    public class MultiTenantUser : IShopKey
    {
        [Required] //This means SQL will throw an error if we doing fill it in
        [MaxLength(40)] //A guid string is 36 characters long
        [Key]
        public string UserId { get; set; }
        public int ShopKey { get; private set; }
        public void SetShopKey(int shopKey)
        {
            ShopKey = shopKey;
        }

        //---------------------------------------------
        //relationships

        [ForeignKey(nameof(ShopKeyBase.ShopKey))]
        public Shop WorksAt { get; set; }
    }
}