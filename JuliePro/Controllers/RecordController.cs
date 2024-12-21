using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JuliePro.Data;
using JuliePro.Models;
using JuliePro.Services;
using JuliePro.Services.impl;
using NuGet.DependencyResolver;

namespace JuliePro.Controllers
{
    public class RecordController : Controller
    {
        private readonly IRecordService _service;
        private readonly IServiceBaseAsync<Discipline> _disciplineService;
        private readonly IServiceBaseAsync<Trainer> _trainerService;

        public RecordController(IRecordService service, IServiceBaseAsync<Discipline> disciplineService, IServiceBaseAsync<Trainer> trainerService)
        {
            _service = service;
            _disciplineService = disciplineService;
            _trainerService = trainerService;
        }

        // GET: Record
        public async Task<IActionResult> Index()
        {
            return View(await _service.GetAllAsync());
        }

        public async Task<IActionResult> TrainerIndex(int trainerId)
        {

            return View(await _service.GetAllByRecordsAsync(trainerId));
        }

        // GET: Record/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @record = await _service.GetByIdAsync(id.Value);

            if (@record == null)
            {
                return NotFound();
            }

            return View(@record);
        }

        // GET: Record/Create
        public async Task<IActionResult> Create()
        {
            return View(await _service.CreateVM());
        }

        // POST: Record/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RecordViewModel model)
        {
            ModelState.Remove(nameof(model.Trainers));
            ModelState.Remove(nameof(model.Displicines));

            if (ModelState.IsValid)
            {
                await _service.CreateAsync(model.Record);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Record/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            RecordViewModel recordVm = new RecordViewModel();

            if (id == null)
            {
                return NotFound();
            }

            recordVm.Record = await _service.GetByIdAsync(id.Value);

            if (recordVm == null)
            {
                return NotFound();
            }

            recordVm.Displicines = new SelectList(await _disciplineService.GetAllAsync(), "Id", "Name");
            recordVm.Trainers = new SelectList(await _trainerService.GetAllAsync(), "Id", "FullName");

            return View(recordVm);
        }

        // POST: Record/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RecordViewModel recordVm)
        {
            if (id != recordVm.Record.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _service.EditAsync(recordVm.Record);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!(await _service.ExistsAsync(recordVm.Record.Id)))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            recordVm.Displicines = new SelectList(await _disciplineService.GetAllAsync(), "Id", "Name");
            recordVm.Trainers = new SelectList(await _trainerService.GetAllAsync(), "Id", "FullName");

            return View(recordVm);
        }

        // GET: Record/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @record = await _service.GetByIdAsync(id.Value);

            if (@record == null)
            {
                return NotFound();
            }

            return View(@record);
        }

        // POST: Record/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @record = await _service.GetByIdAsync(id);

            if (@record != null)
            {
                await _service.DeleteAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
