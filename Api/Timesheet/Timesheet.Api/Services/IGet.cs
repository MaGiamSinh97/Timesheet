using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timesheet.Api.Services
{
    public interface IGet<T> where T: class, new()
    {
        Task<T> GetAsync(int id);
    }
}
