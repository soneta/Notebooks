using Soneta.Business;
using SonetaBudget.Budget;
using SonetaBudget.Models.Database.Config;
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
                        new NewRowAttribute(typeof(DefBudgetTag)) { Default = true }
                    };
                };

                return viewInfo;
            }
        }
    }
}
