using Soneta.Business;
using Soneta.Business.UI;
using Soneta.Types;
using Soneta.Zadania;
using SonetaBudget.Models.Database.Config;
using SonetaBudget.UI.Helpers;
using SonetaBudget.UI.UIElements.Builders;
using System.Collections.Generic;
using System.Linq;

namespace SonetaBudget.UI.Models
{
    internal class ProjectBudgetTags
    {
        private ViewInfo collection;

        public ProjectBudgetTags(Session session)
            => Session = session;

        public Session Session { get; }

        public BudgetTagParams Params { get; private set; }

        public ViewInfo Collection
            => collection ??= CreateViewInfo();

        public void RenderBlock(StackContainer stack)
            => ProjectBudgetTagsBuilder.DefaultComponent(stack, Params?.Tags).CreateComponent();

        public string RenderKey
            => AggregateBudgetTags(Params?.Tags);

        private ViewInfo CreateViewInfo()
        {
            var result = new ViewInfo();

            result.InitContext += (sender, args) =>
            {
                if (Params is null)
                    Params = new BudgetTagParams(args.Context);

                args.Context.Set(Params);
            };

            result.CreateView += (sender, args) =>
            {
                args.View = Session.GetZadania().BudzetyProjektu.CreateView();
            };

            return result;
        }

        private string AggregateBudgetTags(IEnumerable<DefBudgetTag> values)
            => values?.OrderBy(x => x.ID).Aggregate("", (a, r) => $"{a}|{r.ID}") ?? "";

        internal class BudgetTagParams : ContextBase
        {
            private DefBudgetTag[] tags;

            public BudgetTagParams(Context context) : base(context)
            { }

            [Caption("Tagi")]
            public DefBudgetTag[] Tags
            {
                get => tags;
                set
                {
                    tags = value;
                    OnChanged();
                }
            }

            public LookupInfo.EnumerableItem GetListTags()
                => new LookupInfo.EnumerableItem("Definicje", BudgetTagHelper.GetBudgetTagDefinitions(Session), new[] { "Name" });
        }
    }
}
