using Soneta.Business.UI;
using SonetaBudget.UI.Models;
using SonetaBudzet.UI.ViewInfos;

[assembly: FolderView("Projekty/Budżety",
    Priority = 1,
    Description = "Budżety projektu",
    TableName = "BudzetProjektu",
    ViewType = typeof(ProjectBudgetsViewInfo)
)]

[assembly: FolderView("Projekty/Budżety z tagami",
    Priority = 2,
    Description = "Budżety projektu z tagami",
    ObjectType = typeof(ProjectBudgetTags),
    ObjectPage = "ProjectBudgetTags.Main.pageform.xml"
)]
