using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using HeadHunterApp.Models;
using RestSharp;

namespace HeadHunterApp.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            VacanciesModel model = Get(120000, 15000);
            return View(model);
        }

        private const string site = "https://api.hh.ru";
        private const string vacancyRes = "/vacancies";
        private readonly int vacanciesOnPage = 100;
        private int pageCount = 0;
        private readonly IRestClient client = new RestClient(site);

        public VacanciesModel Get(int more, int lower)
        {
            VacanciesModel model = new VacanciesModel(more, lower);
            IRestResponse response = Get(pageCount);
            int pagesCount = (int)JObject.Parse(response.Content)["pages"];
            JArray vacancies = JObject.Parse(response.Content)["items"] as JArray;
            for (int i = pageCount; i < pagesCount; i++)
            {
                foreach (var vacancy in vacancies)
                {
                    if (vacancy["salary"].Type == JTokenType.Null)
                        continue;
                    var salaryFrom = vacancy["salary"]["from"];
                    var salaryTo = vacancy["salary"]["to"];
                    var salaryCurr = vacancy["salary"]["currency"];
                    var salaryFromType = salaryFrom.Type;
                    var salaryToType = salaryTo.Type;
                    double salary = -1;
                    if ((string)salaryCurr != "RUR")
                        continue;
                    else if (salaryFromType != JTokenType.Null && salaryToType != JTokenType.Null)
                        salary = ((double)salaryFrom + (double)salaryTo) / 2;
                    else if (salaryFromType == JTokenType.Null && salaryToType != JTokenType.Null)
                        salary = (double)salaryTo;
                    else if (salaryFromType != JTokenType.Null && salaryToType == JTokenType.Null)
                        salary = (double)salaryFrom;
                    if (salary >= more)
                    {
                        var profession = (string)vacancy["name"];
                        if (!model.VacanciesBiggerOrEvenThanNumber.Contains(profession))
                        {
                            model.VacanciesBiggerOrEvenThanNumber.Add(profession);
                        }
                        var details = JObject.Parse(GetDetails((string)vacancy["id"]).Content);
                        JArray keySkills = details["key_skills"] as JArray;
                        if (keySkills.HasValues)
                        {
                            foreach (var keySkill in keySkills)
                            {
                                var skill = (string)keySkill["name"];
                                if (model.SkillsBiggerOrEvenThanNumber.Contains(skill))
                                    continue;
                                model.SkillsBiggerOrEvenThanNumber.Add(skill);
                            }
                        }
                    }
                    else if (salary > 0 && salary < lower)
                    {
                        var profession = (string)vacancy["name"];
                        if (!model.VacanciesLowerOrEvenThanNumber.Contains(profession))
                        {
                            model.VacanciesLowerOrEvenThanNumber.Add(profession);
                        }
                        var details = JObject.Parse(GetDetails((string)vacancy["id"]).Content);
                        JArray keySkills = details["key_skills"] as JArray;
                        if (keySkills.HasValues)
                        {
                            foreach (var keySkill in keySkills)
                            {
                                var skill = (string)keySkill["name"];
                                if (model.SkillsLowerOrEvenThanNumber.Contains(skill))
                                    continue;
                                model.SkillsLowerOrEvenThanNumber.Add(skill);
                            }
                        }
                    }
                }
                response = Get(pageCount + i + 1);
                vacancies = JObject.Parse(response.Content)["items"] as JArray;
            }
            return model;
        }

        private IRestResponse Get(int page)
        {
            IRestRequest request = new RestRequest(string.Format("{0}?page={1}&per_page={2}", vacancyRes, page, vacanciesOnPage), Method.GET);
            request.AddParameter("only_with_salary", "true");
            return client.Execute(request);
        }

        private IRestResponse GetDetails(string id)
        {
            IRestRequest request = new RestRequest(string.Format("{0}/{1}", vacancyRes, id), Method.GET);
            return client.Execute(request);
        }
    }
}
