using Soneta.Business;
using Soneta.Types;
using Soneta.Zadania.Budzetowanie;
using SonetaBudget.Budget;
using SonetaBudget.Models.Database;
using SonetaBudget.Models.Database.Config;

[assembly: NewRow(typeof(BudgetTag))]

namespace SonetaBudget.Models.Database
{
    public class BudgetTag : SonetaBudgetModule.BudgetTagRow
    {
        //public BudgetTag(BudzetProjektu budget, DefBudgetTag definition)
        //{
        //    if (budget is null)
        //        throw new ArgumentNullException(nameof(budget));

        //    if (definition is null)
        //        throw new ArgumentNullException(nameof(definition));

        //    Budget = budget;
        //    Definition = definition;
        //}

        //public BudgetTag()
        //{ }

        [Context]
        public new BudzetProjektu Budget { get; set; }

        [Context]
        public new DefBudgetTag Definition
        {
            get => base.Definition;
            set => base.Definition = value;
        }

        protected override void OnAdded()
        {
            base.OnAdded();

            base.Budget = Budget;

            Owner = Session.Get(Session.Login.Operator);
            CreationDate = Date.Today;
        }

        public override string ToString()
            => Description;
    }
}
