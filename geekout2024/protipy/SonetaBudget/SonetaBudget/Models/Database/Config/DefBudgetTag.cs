using Soneta.Business;
using SonetaBudget.Budget;
using SonetaBudget.Enums;

namespace SonetaBudget.Models.Database.Config
{
    public abstract class DefBudgetTag : SonetaBudgetModule.DefBudgetTagRow
    {
        public DefBudgetTag(RowCreator creator) : base(creator)
        {
        }

        public DefBudgetTag([Required] TagType type) : base(type)
        {
        }

        public override string ToString()
            => $"[{Symbol}] {Name}";
    }
}
