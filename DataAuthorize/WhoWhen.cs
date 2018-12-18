// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;

namespace DataAuthorize
{
    public class WhoWhen : IWhoWhen
    {
        [Required] //This means SQL will throw an error if we doing fill it in
        [MaxLength(40)] //A guid string is 36 characters long
        public string CreatedBy { get; private set; }
        public DateTime CreatedOn { get; private set; }

        [Required] //This means SQL will throw an error if we doing fill it in
        [MaxLength(40)] //A guid string is 36 characters long
        public string UpdatedBy { get; private set; }
        public DateTime UpdatedOn { get; private set; }

        /// <summary>
        /// This logs the changes.
        /// </summary>
        /// <param name="add"></param>
        /// <param name="userId"></param>
        public void LogChange(bool add, string userId)
        {
            var time = DateTime.UtcNow;
            if (add)
            {
                CreatedBy = userId;
                CreatedOn = time;
            }

            //I always change the updated info, even on Create, so they are filled in
            UpdatedBy = userId; 
            UpdatedOn = time;
        }
    }
}