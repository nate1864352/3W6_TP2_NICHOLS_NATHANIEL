using JuliePro.Data;
using JuliePro.Models;
using JuliePro.Utility;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace JuliePro.Services.impl
{
    public class TrainerService : ServiceBaseEF<Trainer>, ITrainerService
    {
        private IImageFileManager fileManager;

        public TrainerService(JulieProDbContext dbContext, IImageFileManager fileManager) : base(dbContext)
        {
            this.fileManager = fileManager;
        }

        public async Task<Trainer> CreateAsync(Trainer model, IFormCollection form)
        {
            model.Photo = await fileManager.UploadImage(form, false, null);

            return await base.CreateAsync(model);
        }

        public async Task EditAsync(Trainer model, IFormCollection form)
        {
            var old = await _dbContext.Trainers.Where(x=>x.Id == model.Id).Select(x=>x.Photo).FirstOrDefaultAsync();
            model.Photo = await fileManager.UploadImage(form, true, old);
            await this.EditAsync(model);
        }

        public async Task<TrainerSearchViewModel> GetAllAsync(TrainerSearchViewModelFilter filter)
        {
            filter.VerifyProperties();//mets à null les éléments qui sont vides. 

            var result = new TrainerSearchViewModel(filter);

            //TODO Faire les filtres et utilisez les paramètres de pagination.
            result.Items = await this._dbContext.Trainers.
                            Where(x => filter.SearchNameText == null || (x.FirstName.Contains(filter.SearchNameText) || x.LastName.Contains(filter.SearchNameText)))
                           .Where(x => filter.SelectedDisciplineId == null || x.Discipline_Id == filter.SelectedDisciplineId)
                           .Where(x => filter.SelectedCertificationId == null || x.TrainerCertifications.Where(x => x.Certification_Id == filter.SelectedCertificationId).Any())
                           .Where(x => filter.SelectedCertificationCenter == null || x.TrainerCertifications.Where(x => x.Certification.CertificationCenter == filter.SelectedCertificationCenter).Any())
                           .Where(x => filter.SelectedGender == null || x.Genre == filter.SelectedGender)
                           .ToPaginatedAsync(result.SelectedPageSize != filter.SelectedPageSize ? 0 : result.SelectedPageIndex, result.SelectedPageSize);

            //TODO ajouter les éléments dans selectList
            result.AvailablePageSizes = new SelectList(new List<int>() { 9, 12, 18, 21 });
            result.Disciplines = new SelectList(new List<Discipline>(await _dbContext.Disciplines.Select(x => x).ToListAsync()), "Id", "Name");
            result.Certifications = new SelectList(await _dbContext.Certifications.Select(x => x).ToListAsync(), "Id", "FullTitle");
            result.CertificationCenters = new SelectList(new List<string>(_dbContext.Certifications.Select(x => x.CertificationCenter).Distinct().ToList()));

            return result;
        }
    }
}
