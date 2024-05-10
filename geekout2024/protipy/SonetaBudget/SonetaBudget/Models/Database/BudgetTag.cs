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
        public BudgetTag(RowCreator creator) : base(creator)
        {
        }

        public BudgetTag([Required] BudzetProjektu budget, [Required] DefBudgetTag definition) : base(budget, definition)
        {
        }

        protected override void OnAdded()
        {
            base.OnAdded();

            Owner = Session.Get(Session.Login.Operator);
            CreationDate = Date.Today;
        }

        public override string ToString()
            => Description;
    }
}
