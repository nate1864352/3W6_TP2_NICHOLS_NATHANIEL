using JuliePro.Models;
using Microsoft.EntityFrameworkCore;

namespace JuliePro.Services
{
    public interface IRecordService : IServiceBaseAsync<Record>
    {
        public Task<RecordViewModel> CreateVM();
        public Task<RecordViewModel> GetAllByRecordsAsync(int trainerId);
    }
}
