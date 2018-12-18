using System;

namespace DataAuthorize
{
    public interface IWhoWhen
    {
        string CreatedBy { get; }
        DateTime CreatedOn { get; }
        string UpdatedBy { get; }
        DateTime UpdatedOn { get; }

        void LogChange(bool add, string userId);
    }
}