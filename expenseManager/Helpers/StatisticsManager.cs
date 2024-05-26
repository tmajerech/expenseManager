using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using expenseManager.Enums;
using expenseManager.Models;
using ScottPlot;
using ScottPlot.TickGenerators;

namespace expenseManager.Helpers;

public static class StatisticsManager
{
    public static void PlotSumPerDay(List<Record> records)
    {
        //get all data
        var pairs = records.GroupBy(r => r.Date)
            .Select(g => new
            {
                Date = g.Key,
                TotalSum = g.Sum(r => r.Type == RecordType.Income ? r.Amount : -r.Amount)
            }).ToList();
        var days = pairs.Select(p => p.Date).ToArray();
        var amounts = pairs.Select(p => p.TotalSum).ToArray();

        ScottPlot.Plot myPlot = new();

        myPlot.Add.Scatter(days, amounts);
        myPlot.Axes.DateTimeTicksBottom();
        
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..", "Statistics", "SumPerDay.png");
        var directory = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }
        
        myPlot.SavePng(filePath, 400, 300);
    }

    public static void PlotSumAmountPerDay(List<Record> records, string filePath)
    {
        ScottPlot.Plot myPlot = new();
        
        //get all data
        var pairs = records.GroupBy(r => r.Date)
            .Select(g => new
            {
                Date = g.Key,
                TotalSum = g.Sum(r => r.Type == RecordType.Income ? r.Amount : -r.Amount)
            }).ToList();

        List<Tick> ticks = [];
        for (var i = 1; i <= pairs.Count; i++)
        {
            myPlot.Add.Bar(position: i, value: pairs[i-1].TotalSum);
            ticks.Add(new Tick(i, pairs[i-1].Date.ToString("yyyy-MM-dd")));
        }
        
        myPlot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.NumericManual(ticks.ToArray());
        myPlot.Axes.Bottom.MajorTickStyle.Length = 0;
        myPlot.HideGrid();

// tell the plot to autoscale with no padding beneath the bars
        myPlot.Axes.Margins(bottom: 0);
        
        myPlot.SavePng(filePath, 400, 300);
    }
    
}