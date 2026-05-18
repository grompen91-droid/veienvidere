namespace Arket.DTOs;

public class BudgetDto
{
    public List<BudgetSectionDto> Sections { get; set; } = [];
}

public class BudgetSectionDto
{
    public string Name     { get; set; } = "";
    public bool IsIncome   { get; set; }
    public List<BudgetItemDto> Items { get; set; } = [];
}

public class BudgetItemDto
{
    public string Label  { get; set; } = "";
    public decimal Amount { get; set; }
}