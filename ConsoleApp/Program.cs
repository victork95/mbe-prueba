using ConsoleApp.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static HttpClient client = new HttpClient();
        static testContext dbContext = new testContext();
        const string API = "Api";
        const string LINUX = "Linux";
        static void Main(string[] args)
        {
            var timer = new System.Timers.Timer(10000); // se ejecutara cada 10 segundos


            timer.Elapsed += async (sender, e) => {
                List<Schedule> schedules = dbContext.Schedules.Where(x => x.Executiondate.Date == DateTime.Today.Date && x.Executed == false).ToList();

                foreach (var schedule in schedules)
                {
                    switch (schedule.Action)
                    {   
                        case API:
                            await ApiFunction(schedule);
                            break;
                        case LINUX:
                            LinuxFunction(schedule);
                            break;
                        default:
                            break;
                    }
                }
            };

            timer.Start();
            Console.Read();
        }

        public static async Task ApiFunction(Schedule schedule)
        {
            HttpResponseMessage response = await client.GetAsync(schedule.Actiondetail);
            response.EnsureSuccessStatusCode();

            if (response.IsSuccessStatusCode)
            {
                Createschedulelog(true, await response.Content.ReadAsStringAsync(), schedule.Id);
            }
            else
            {
                Createschedulelog(false, await response.Content.ReadAsStringAsync(), schedule.Id);
            }

            SetExecutedAsTrue(schedule);
        }

        public static void LinuxFunction(Schedule schedule)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = "/bin/bash";
            psi.Arguments = schedule.Actiondetail;
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            using var process = Process.Start(psi);

            process.WaitForExit();

            var error = process.StandardError.ReadToEnd().Substring(0, 100);
            var output = process.StandardOutput.ReadToEnd().Substring(0, 100);

            if (error.Length > 0)
            {
                Createschedulelog(false, error, schedule.Id);
            }

            if (output.Length > 0)
            {
                Createschedulelog(true, output, schedule.Id);
            }

            SetExecutedAsTrue(schedule);
        }

        public static void Createschedulelog(bool success, string result, int scheduleId)
        {
            var scheduleLog = new Schedulelog() { Executiondate = DateTime.Now, Success = success, Actionresult = result.Substring(0, 100), Schedulelogid = scheduleId };

            dbContext.Schedulelogs.Add(scheduleLog);
            dbContext.SaveChanges();
        }

        public static void SetExecutedAsTrue(Schedule schedule)
        {
            schedule.Executed = true;

            dbContext.Schedules.Update(schedule);
            dbContext.SaveChanges();
        }
    }
}