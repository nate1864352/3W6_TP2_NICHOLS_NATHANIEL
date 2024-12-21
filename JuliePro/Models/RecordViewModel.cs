using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace JuliePro.Models
{
    public class RecordViewModel
    {
        [Display(Name = "Displicines")] public SelectList Displicines { get; set; }
        [Display(Name = "Trainers")] public SelectList Trainers { get; set; }
        public Record Record {  get; set; }
        public Trainer Trainer { get; set; }
        public IEnumerable<Record> Records { get; set; }

        public RecordViewModel(Trainer trainer, IEnumerable<Record> records)
        {
            Trainer = trainer;
            Records = records;
        }

        public RecordViewModel()
        {

        }
    }
}
