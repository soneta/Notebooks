using System.ComponentModel;
using Soneta.Business;
using Soneta.Types;
using Soneta.Zadania.Budzetowanie;
using SonetaBudget.Budget;
using SonetaBudget.Models.Database;
using SonetaBudget.Workers;

[assembly: Worker(typeof(BudgetTagWorker), typeof(BudzetProjektu))]

namespace SonetaBudget.Workers
{
    public class BudgetTagWorker
    {
        [Context]
        [Browsable(false)]
        public BudzetProjektu ProjectBudget { get; set; }


        [Caption("Tag budżetu")]
        public BudgetTag BudgetTag
            => ProjectBudget.Session.GetSonetaBudget().BudgetTags.WgBudget[ProjectBudget].GetFirst();

        [SqlResolving(
        RelationField = "Budget",
        ParentTableSubRow = "BudgetTags")]
        [Caption("Tag budżetu - atrybut")]
        public BudgetTag BudgetTagBySqlResolving
            => ProjectBudget.Session.GetSonetaBudget().BudgetTags.WgBudget[ProjectBudget].GetFirst();
    }
}
