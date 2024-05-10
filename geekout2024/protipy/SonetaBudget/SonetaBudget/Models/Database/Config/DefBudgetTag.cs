using Soneta.Business;
using SonetaBudget.Budget;
using SonetaBudget.Models.Database.Config;

[assembly: NewRow(typeof(DefBudgetTag))]

namespace SonetaBudget.Models.Database.Config
{
    public class DefBudgetTag : SonetaBudgetModule.DefBudgetTagRow
    {
    }
}
