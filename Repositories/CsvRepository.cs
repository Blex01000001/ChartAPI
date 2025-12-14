using ChartAPI.Interfaces;
using ChartAPI.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace ChartAPI.Repositories
{
    //public class CsvRepository 
    //{
    //    string repositoryDirPath = "D:\\Code\\C#\\ManHour Analysis\\bin\\Debug\\repository\\";

    //    public IEnumerable<ManHourModel> GetByName(string name)
    //    {
    //        ConcurrentBag<List<ManHourModel>> tempMHModel = new ConcurrentBag<List<ManHourModel>>();

    //        List<ManHourModel> list = new List<ManHourModel>()
    //        {
    //            new ManHourModel()
    //            {
    //                Name = "LIN",
    //                CostCode = "003",
    //                Year = 2025,
    //                Date = new DateTime(2025,8,5),
    //                Hours = 1
    //            },
    //            new ManHourModel()
    //            {
    //                Name = "LIN",
    //                CostCode = "003",
    //                Year = 2025,
    //                Date = new DateTime(2025,3,6),
    //                Hours = 1.5
    //            },
    //            new ManHourModel()
    //            {
    //                Name = "LIN",
    //                CostCode = "053",
    //                Year = 2025,
    //                Date = new DateTime(2025,6,6),
    //                Hours = 3
    //            },
    //            new ManHourModel()
    //            {
    //                Name = "LIN",
    //                CostCode = "003",
    //                Year = 2024,
    //                Date = new DateTime(2024,1,8),
    //                Hours = 5
    //            },
    //            new ManHourModel()
    //            {
    //                Name = "LIN",
    //                CostCode = "003",
    //                Year = 2024,
    //                Date = new DateTime(2024,5,15),
    //                Hours = 2
    //            },
    //            new ManHourModel()
    //            {
    //                Name = "LIN",
    //                CostCode = "053",
    //                Year = 2024,
    //                Date = new DateTime(2024,6,7),
    //                Hours = 8
    //            },
    //            new ManHourModel()
    //            {
    //                Name = "LIN",
    //                CostCode = "003",
    //                Year = 2023,
    //                Date = new DateTime(2023,9,12),
    //                Hours = 1
    //            },
    //            new ManHourModel()
    //            {
    //                Name = "LIN",
    //                CostCode = "003",
    //                Year = 2023,
    //                Date = new DateTime(2023,12,8),
    //                Hours = 1.5
    //            },
    //            new ManHourModel()
    //            {
    //                Name = "LIN",
    //                CostCode = "053",
    //                Year = 2023,
    //                Date = new DateTime(2023,7,29),
    //                Hours = 3
    //            }


    //        };

    //        List<string> years = new List<string>() {
    //            "2025", "2024", "2023"
    //        };
    //        List<Task> tasks = new List<Task>();

    //        foreach (var year in years)
    //        {
    //            //tasks.Add(Task.Run(() =>
    //            //{
    //            string CsvPath = repositoryDirPath + year + ".csv";
    //            Console.WriteLine($"CsvPath: {CsvPath}");
    //            var reader = new StreamReader(CsvPath, Encoding.GetEncoding("Big5"));
    //            var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture);
    //            List<ManHourModel> TempMHModel = csv.GetRecords<ManHourModel>().ToList();
    //            tempMHModel.Add(TempMHModel);
    //            Console.WriteLine($"Count: {TempMHModel.Count}");
    //            //}));
    //        }
    //        Task.WhenAll(tasks);

    //        List<ManHourModel> readManHourModel = tempMHModel.SelectMany(r => r).ToList();//merge model

    //        return readManHourModel.Where(x => x.Name == name);
    //    }
    //    public IEnumerable<ManHourModel> GetByID(string id)
    //    {
    //        throw new NotImplementedException();
    //    }

    //}
}
