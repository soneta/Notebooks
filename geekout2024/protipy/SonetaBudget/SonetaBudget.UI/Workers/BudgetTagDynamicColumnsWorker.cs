using System.Collections.Generic;
using System.Linq;
using Soneta.Business;
using Soneta.Business.Licence;
using Soneta.Zadania.Budzetowanie;
using SonetaBudget.Budget;
using SonetaBudget.UI.Models;
using SonetaBudget.UI.Workers;

[assembly: Worker(typeof(BudgetTagDynamicColumnsWorker), typeof(BudzetProjektu), Contexts = new object[] { LicencjeModułu.PRJ })]

namespace SonetaBudget.UI.Workers
{
    internal class BudgetTagDynamicColumnsWorker
    {
        public BudgetTagDynamicColumnsWorker(BudzetProjektu projectBudget, Context context)
            => InitializeColumns(projectBudget, context);

        public IList<string> Columns { get; set; } = new List<string>();

        private void InitializeColumns(BudzetProjektu projectBudget, Context context)
        {
            if (!context.Get<ProjectBudgetTags.BudgetTagParams>(out var param))
                return;
            
            var tags = projectBudget.Session.GetSonetaBudget().BudgetTags.WgBudget[projectBudget];

            foreach (var tag in param.Tags)
                Columns.Add(tags.FirstOrDefault(x => x.Definition.ID == tag.ID)?.Description);
        }
    }
}
