using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HeadHunterApp.Models
{
    public class VacanciesModel
    {
        public List<string> VacanciesBiggerOrEvenThanNumber { get; set; }
        public List<string> SkillsBiggerOrEvenThanNumber { get; set; }
        public List<string> VacanciesLowerOrEvenThanNumber { get; set; }
        public List<string> SkillsLowerOrEvenThanNumber { get; set; }
        public int MoreSalary { get; set; }
        public int LowerSalary { get; set; }

        public VacanciesModel(int more, int lower)
        {
            MoreSalary = more;
            LowerSalary = lower;
            VacanciesBiggerOrEvenThanNumber = new List<string>();
            SkillsBiggerOrEvenThanNumber = new List<string>();
            VacanciesLowerOrEvenThanNumber = new List<string>();
            SkillsLowerOrEvenThanNumber = new List<string>();
        }

    }
}
