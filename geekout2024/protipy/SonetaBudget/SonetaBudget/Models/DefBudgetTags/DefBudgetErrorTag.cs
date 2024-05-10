using Soneta.Business;
using SonetaBudget.Enums;
using SonetaBudget.Models.Database.Config;
using SonetaBudget.Models.DefBudgetTags;

[assembly: BusinessRow(typeof(DefBudgetErrorTag), TagType.Error)]

namespace SonetaBudget.Models.DefBudgetTags
{
    public class DefBudgetErrorTag : DefBudgetTag
    {
        public DefBudgetErrorTag(RowCreator creator) : base(creator)
        {
        }

        public DefBudgetErrorTag() : base(TagType.Error)
        {
        }
    }
}
