using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class GetListRole
    {
        public GetListRole(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public AppDbContext _dbContext { get; set; }

        public List<UserRole> GetRoles()
        {
            return new List<UserRole>();
        }
    }
}
