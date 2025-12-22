using ChartAPI.DataAccess.Interfaces;
using ChartAPI.Domain.Entities;

namespace ChartAPI.DataAccess.SQLite.Repositories
{
    public class EmployeeRepository : BaseRepository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(IConfiguration config)
            : base(config, tableName: "EmpInfo9933", dbFileName: "ManHourData.db")
        {
            
        }

        public override Task DeleteAsync(IEnumerable<Employee> models)
        {
            throw new NotImplementedException();
        }

        public override Task InsertAsync(IEnumerable<Employee> models)
        {
            throw new NotImplementedException();
        }
    }
}
