// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using DataAuthorize;

namespace DataLayer.EfClasses.BusinessClasses
{
    public class PersonalData : OwnedByBase
    {
        public int PersonalDataId { get; set; }

        public string SecretToYou { get; set; }
    }
}