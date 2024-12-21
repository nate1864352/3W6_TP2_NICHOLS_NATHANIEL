using JuliePro.Data;
using JuliePro.Models;
using JuliePro.Utility;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace JuliePro.Services.impl
{
    public class RecordService : ServiceBaseEF<Record>, IRecordService
    {
        public RecordService(JulieProDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<RecordViewModel> CreateVM()
        {
            RecordViewModel recordViewModel = new RecordViewModel();

            recordViewModel.Displicines = new SelectList(await _dbContext.Disciplines.ToListAsync(), "Id", "Name");
            recordViewModel.Trainers = new SelectList(await _dbContext.Trainers.ToListAsync(), "Id", "FullName");

            return recordViewModel;
        }

        public async Task<RecordViewModel> GetAllByRecordsAsync(int trainerId)
        {
            var trainer = await _dbContext.Trainers.FindAsync(trainerId);
            var model = await _dbContext.Records.Where(x => x.Trainer.Records.Any(y => y.Trainer_Id == trainerId)).ToListAsync();

            return new RecordViewModel(trainer, model);
        }
    }
}
