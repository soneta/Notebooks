using System.Linq;
using Soneta.Business;
using Soneta.Core;
using Soneta.Zadania.Budzetowanie;
using SonetaBudget.Attributes;
using SonetaBudget.Budget;
using SonetaBudget.UI.Extenders;

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
            => session.GetSonetaBudget().DefBudgetTags.AsQuery()
                .Where(x => !x.Locked)
                .OrderBy(x => x.Name)
                .Select(x => new BudgetTagAttribute(x))
                .ToArray();
    }
}
