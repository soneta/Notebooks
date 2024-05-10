using Soneta.Business;
using SonetaBudget.Budget;
using SonetaBudget.Models.Database;

[assembly: NewRow(typeof(BudgetTag))]

namespace SonetaBudget.Models.Database
{
    public class BudgetTag : SonetaBudgetModule.BudgetTagRow
    {
    }
}
