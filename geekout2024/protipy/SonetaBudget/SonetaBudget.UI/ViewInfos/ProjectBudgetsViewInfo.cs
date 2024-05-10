using Soneta.Business;
using Soneta.Zadania;

namespace SonetaBudzet.UI.ViewInfos
{
    public class ProjectBudgetsViewInfo : ViewInfo
    {
        public ProjectBudgetsViewInfo()
        {
            // View wiążemy z odpowiednią definicją viewform.xml poprzez property ResourceName
            ResourceName = "ProjectBudgets";

            // Inicjowanie contextu
            InitContext += ProjectBudgetViewInfo_InitContext;

            // Tworzenie view zawierającego konkretne dane
            CreateView += ProjectBudgetViewInfo_CreateView;
        }

        void ProjectBudgetViewInfo_InitContext(object sender, ContextEventArgs args)
            => args.Context.Set(new WParams(args.Context));

        void ProjectBudgetViewInfo_CreateView(object sender, CreateViewEventArgs args)
        {
            if (!args.Context.Get<WParams>(out var parameters))
                return;

            args.View = ViewCreate(parameters);
        }

        public class WParams : ContextBase
        {
            public WParams(Context context) : base(context)
            {
            }
        }

        protected View ViewCreate(WParams pars)
        {
            View view = pars.Session.GetZadania().BudzetyProjektu.CreateView();
            return view;
        }
    }
}
