using System.Collections.Generic;
using System.Linq;
using Soneta.Business.UI;
using Soneta.Tools;
using SonetaBudget.Models.Database.Config;

namespace SonetaBudget.UI.UIElements.Builders
{
    internal class ProjectBudgetTagsBuilder : ProjectBudgetTagsBuilderBase<StackContainer>
    {
        public ProjectBudgetTagsBuilder(StackContainer stack)
            : base(stack)
        { }

        public ProjectBudgetTagsBuilder BlockFilter()
        {
            var flow = new FlowContainer();

            flow.Elements.Add(new FieldElement {
                CaptionHtml = "Tagi".Translate(),
                Width = "20",
                EditValue = "{Params.Tags}"
            });

            Component.Elements.Add(flow);

            return this;
        }

        public ProjectBudgetTagsBuilder BlockGrid(IEnumerable<DefBudgetTag> defBudgetTags)
        {
            var grid = new GridElement
            {
                Width = "*",
                Height = "*",
                IsToolbarVisible = true,
                IsFilterRowVisible = true,
                EditValue = "{Collection}"
            };

            grid.Elements.Add(new FieldElement
            {
                CaptionHtml = "Nazwa projektu".Translate(),
                Width = "20",
                EditValue = "{Projekt.Nazwa}",
                Class = new[] { UIClass.Frozen }
            });

            grid.Elements.Add(new FieldElement
            {
                CaptionHtml = "Symbol budżetu".Translate(),
                Width = "20",
                EditValue = "{Symbol}",
                Class = new[] { UIClass.Frozen }
            });

            grid.Elements.AddRange(CreateTags(defBudgetTags));

            Component.Elements.Add(grid);

            return this;
        }

        private IEnumerable<UIElement> CreateTags(IEnumerable<DefBudgetTag> defBudgetTags)
        {
            if (defBudgetTags is null)
                yield break;

            foreach (var item in defBudgetTags.Select((value, index) => new { index, value }))
            {
                yield return new FieldElement
                {
                    CaptionHtml = item.value.Symbol,
                    Width = "20",
                    EditValue = $"{{Workers.BudgetTagDynamicColumns.Columns[{item.index}]}}"
                };
            }
        }

        public static ProjectBudgetTagsBuilder DefaultComponent(StackContainer stack, IEnumerable<DefBudgetTag> defBudgetTags)
            => new ProjectBudgetTagsBuilder(stack)
                .BlockFilter()
                .BlockGrid(defBudgetTags);
    }
}
