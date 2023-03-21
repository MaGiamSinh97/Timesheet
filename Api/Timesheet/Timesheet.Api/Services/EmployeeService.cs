using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Timesheet.Api.Helpers;
using Timesheet.Infrastructure.Persistence;

namespace Timesheet.Api.Services
{
    public class EmployeeService : IGetAll<Core.Employee>, IGet<Core.Employee>
    {
        private readonly TimesheetContext context;

        public EmployeeService(TimesheetContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Core.Employee>> GetAllAsync(IEnumerable<int> ids)
        {
            return await this.context.Employees.Where(t => ids.Contains(t.Id)).ToListAsync();
        }

        public async Task<IEnumerable<Core.Employee>> GetAllAsync()
        {
            return await this.context.Employees.Include("Timesheets").AsNoTracking().ToListAsync();
        }

        public async System.Threading.Tasks.Task<int> Add(Timesheet.Core.Employee employee)
        {
            await this.context.AddAsync(employee);
            return await this.context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task<int> Update(Timesheet.Core.Employee employee)
        {
            this.context.Entry(employee).State = EntityState.Modified;
            return await this.context.SaveChangesAsync();
        }

        public async System.Threading.Tasks.Task<int> Delete(int Id)
        {
            var employee = this.context.Employees.Find(Id);
            if (employee != null)
            {
                this.context.Employees.Entry(employee).State = EntityState.Deleted;
                return await this.context.SaveChangesAsync();
            }
            return 0;
        }

        public async Task<Core.Employee> GetAsync(int id)
        {
            return await this.context.Employees.AsNoTracking().SingleOrDefaultAsync(t => t.Id == id);
        }  

        public bool CheckDuplicate(string Ldap)
        {
            return this.context.Employees.Any(t => t.KnoxId == Ldap);
        }
        public async Task<Core.Employee> FindAsync(string ldap)
        {
            return await this.context.Employees.Where(t => t.KnoxId == ldap).FirstOrDefaultAsync();
        }

        public HashSalt EncryptPassword(string password)
        {
            byte[] salt = new byte[128 / 8]; // Generate a 128-bit salt using a secure PRNG
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            string encryptedPassw = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));
            return new HashSalt { Hash = encryptedPassw, Salt = salt };
        }
        public bool VerifyPassword(string enteredPassword, byte[] salt, string storedPassword)
        {
            string encryptedPassw = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: enteredPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8
            ));
            return encryptedPassw == storedPassword;
        }
    }
}
