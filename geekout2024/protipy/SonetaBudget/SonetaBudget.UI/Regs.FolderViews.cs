using Soneta.Business.UI;
using SonetaBudzet.UI.ViewInfos;

[assembly: FolderView("Projekty/Budżety",
    Priority = 1,
    Description = "Budżety projektu",
    TableName = "BudzetProjektu",
    ViewType = typeof(ProjectBudgetsViewInfo)
)]
