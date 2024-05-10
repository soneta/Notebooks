using System.Linq;
using Soneta.Business;
using Soneta.Core;
using Soneta.Zadania.Budzetowanie;
using SonetaBudget.Attributes;
using SonetaBudget.Budget;
using SonetaBudget.UI.Extenders;
using SonetaBudget.UI.Helpers;

[assembly: Worker(typeof(BudgetTagsExtender))]

namespace SonetaBudget.UI.Extenders
{
    internal class BudgetTagsExtender : ContextBase
    {
        public BudgetTagsExtender(Context context) : base(context)
        {
        }

        [Context]
        public IBudzetowany Budget { get; set; }

        public ViewInfo Collection
        {
            get
            {
                var viewInfo = new ViewInfo();

                viewInfo.CreateView += (sender, args) =>
                {
                    args.View = args.Session.GetSonetaBudget().BudgetTags.WgBudget[(BudzetProjektu)Budget.BudzetPodstawowy].CreateView();
                    args.View.NewRows = GetNewRows(args.Session);
                };

                return viewInfo;
            }
        }

        private NewRowAttribute[] GetNewRows(Session session)
            => BudgetTagHelper.GetBudgetTagDefinitions(session)
                .Select(x => new BudgetTagAttribute(x))
                .ToArray();
    }
}
