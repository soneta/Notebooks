using Soneta.Business;
using Soneta.Tools;
using SonetaBudget.Budget;
using SonetaBudget.Models.DefBudgetTags;
using SonetaBudget.UI.Extenders;

[assembly: Worker(typeof(DefBudgetTagsExtender))]

namespace SonetaBudget.UI.Extenders
{
    internal class DefBudgetTagsExtender
    {
        public ViewInfo Collection
        {
            get
            {
                var viewInfo = new ViewInfo();

                viewInfo.CreateView += (sender, args) =>
                {
                    args.View = args.Session.GetSonetaBudget().DefBudgetTags.CreateView();
                    args.View.NewRows = new[] {
                        new NewRowAttribute("Informacyjny".Translate(), typeof(DefBudgetInformationTag)) { Default = true },
                        new NewRowAttribute("Ostrzegający".Translate(), typeof(DefBudgetWarningTag)),
                        new NewRowAttribute("Błędny".Translate(), typeof(DefBudgetErrorTag))
                    };
                };

                return viewInfo;
            }
        }
    }
}
