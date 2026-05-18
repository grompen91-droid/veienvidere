using Arket.DTOs;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Arket.Controllers;

[ApiController]
[Route("api/budget")]
[Authorize]
public class BudgetController : ControllerBase
{
    [HttpPost("export")]
    public IActionResult ExportExcel([FromBody] BudgetDto dto)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Budget");

        // ── Styles
        var green     = XLColor.FromHtml("#008774");
        var lightGreen = XLColor.FromHtml("#f0faf8");
        var headerFont = XLColor.White;

        // ── Column widths
        ws.Column(1).Width = 32;
        ws.Column(2).Width = 24;
        ws.Column(3).Width = 18;

        // ── Title
        var title = ws.Cell(1, 1);
        title.Value = "Monthly Budget";
        title.Style.Font.Bold = true;
        title.Style.Font.FontSize = 16;
        title.Style.Font.FontColor = green;

        var month = ws.Cell(2, 1);
        month.Value = DateTime.Now.ToString("MMMM yyyy");
        month.Style.Font.Italic = true;
        month.Style.Font.FontColor = XLColor.Gray;

        int row = 4;

        decimal totalIncome   = 0;
        decimal totalExpenses = 0;

        // ── Sections
        foreach (var section in dto.Sections)
        {
            // Section header row
            var headerCell = ws.Cell(row, 1);
            headerCell.Value = section.Name;
            headerCell.Style.Font.Bold      = true;
            headerCell.Style.Font.FontSize  = 12;
            headerCell.Style.Font.FontColor = headerFont;
            headerCell.Style.Fill.BackgroundColor = green;

            ws.Cell(row, 2).Style.Fill.BackgroundColor = green;

            var sectionTotalCell = ws.Cell(row, 3);
            sectionTotalCell.Style.Fill.BackgroundColor = green;
            sectionTotalCell.Style.Font.Bold      = true;
            sectionTotalCell.Style.Font.FontColor = headerFont;
            sectionTotalCell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            row++;

            // Column sub-headers
            ws.Cell(row, 1).Value = "Item";
            ws.Cell(row, 3).Value = "Amount (kr)";
            foreach (var col in new[] { 1, 2, 3 })
            {
                ws.Cell(row, col).Style.Font.Bold = true;
                ws.Cell(row, col).Style.Font.FontColor = green;
                ws.Cell(row, col).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                ws.Cell(row, col).Style.Border.BottomBorderColor = green;
            }
            row++;

            decimal sectionTotal = 0;

            foreach (var item in section.Items)
            {
                ws.Cell(row, 1).Value = item.Label;
                ws.Cell(row, 3).Value = item.Amount;
                ws.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                ws.Cell(row, 3).Style.NumberFormat.Format = "#,##0";

                // Light stripe on even rows
                if (row % 2 == 0)
                {
                    ws.Cell(row, 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#f9f9f9");
                    ws.Cell(row, 3).Style.Fill.BackgroundColor = XLColor.FromHtml("#f9f9f9");
                }

                sectionTotal += item.Amount;
                row++;
            }

            // Write section total back
            sectionTotalCell.Value = sectionTotal;
            sectionTotalCell.Style.NumberFormat.Format = "#,##0";

            if (section.IsIncome) totalIncome   += sectionTotal;
            else                  totalExpenses += sectionTotal;

            row++; // empty row between sections
        }

        // ── Summary
        row++;

        void SummaryRow(int r, string label, decimal value, bool bold = false)
        {
            var lbl = ws.Cell(r, 1);
            var val = ws.Cell(r, 3);

            lbl.Value = label;
            val.Value = value;
            val.Style.NumberFormat.Format = "#,##0";
            val.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            if (bold)
            {
                lbl.Style.Font.Bold = true;
                val.Style.Font.Bold = true;
                lbl.Style.Fill.BackgroundColor = lightGreen;
                val.Style.Fill.BackgroundColor = lightGreen;
                ws.Cell(r, 2).Style.Fill.BackgroundColor = lightGreen;
            }
        }

        ws.Cell(row, 1).Value = "SUMMARY";
        ws.Cell(row, 1).Style.Font.Bold = true;
        ws.Cell(row, 1).Style.Font.FontColor = green;
        row++;

        SummaryRow(row++, "Total income",   totalIncome);
        SummaryRow(row++, "Total expenses", totalExpenses);

        var balance = totalIncome - totalExpenses;
        SummaryRow(row, balance >= 0 ? "Balance" : "Deficit", Math.Abs(balance), bold: true);
        ws.Cell(row, 3).Style.Font.FontColor = balance >= 0 ? green : XLColor.Red;

        // ── Return file
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        stream.Position = 0;

        var fileName = $"budget-{DateTime.Now:yyyy-MM}.xlsx";
        return File(
            stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName
        );
    }
}