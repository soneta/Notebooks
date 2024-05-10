using Soneta.Business;
using SonetaBudget.Budget;
using SonetaBudget.Models.Database.Config;
using System.Collections.Generic;
using System.Linq;

namespace SonetaBudget.UI.Helpers
{
    internal static class BudgetTagHelper
    {
        public static IEnumerable<DefBudgetTag> GetBudgetTagDefinitions(Session session)
            => session.GetSonetaBudget().DefBudgetTags.AsQuery()
                .Where(x => !x.Locked)
                .OrderBy(x => x.Type)
                .ThenBy(x => x.Name);
    }
}
