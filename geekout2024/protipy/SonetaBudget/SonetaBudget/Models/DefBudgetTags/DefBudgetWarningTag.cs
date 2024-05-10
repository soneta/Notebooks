using Soneta.Business;
using SonetaBudget.Enums;
using SonetaBudget.Models.Database.Config;
using SonetaBudget.Models.DefBudgetTags;

[assembly: BusinessRow(typeof(DefBudgetWarningTag), TagType.Warning)]

namespace SonetaBudget.Models.DefBudgetTags
{
    public class DefBudgetWarningTag : DefBudgetTag
    {
        public DefBudgetWarningTag(RowCreator creator) : base(creator)
        {
        }

        public DefBudgetWarningTag() : base(TagType.Warning)
        {
        }
    }
}
