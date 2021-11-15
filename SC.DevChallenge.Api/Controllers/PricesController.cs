using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace SC.DevChallenge.Api.Controllers
{
    class Record
    {
        public string portfolio { get; set; }
        public string owner { get; set; }
        public string instrument { get; set; }
        public DateTime datetime { get; set; }
        public double timestamp { get; set; }
        public double price { get; set; }

        public static Record FromCsv(string csvLine)
        {
            string[] values = csvLine.Split(',');

            Record record = new Record();
            record.portfolio = Convert.ToString(values[0]).ToLower();
            record.owner = Convert.ToString(values[1]).ToLower();
            record.instrument = Convert.ToString(values[2]).ToLower();
            record.price = Convert.ToDouble(values[4]);

            string datetime = Convert.ToString(values[3]);
            record.datetime = DateTime.ParseExact(datetime, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            record.timestamp = ConvertToUnixTimestamp(record.datetime);

            return record;
        }

        public override string ToString()
        {
            return $"{portfolio} - {owner} - {instrument} - {datetime} ({timestamp}) - {price}";
        }

        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }

        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

    }

    [ApiController]
    [Route("api/[controller]")]
    public class PricesController : ControllerBase
    {
        [HttpGet("average")]
        public string Average(string portfolio, string owner, string instrument, string date)
        {
            // normalize input data
            portfolio = portfolio.ToLower();
            owner = owner.ToLower();
            instrument = instrument.ToLower();
            System.DateTime datetime = DateTime.ParseExact(date, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

            // read csv file
            List<Record> records = System.IO.File.ReadAllLines(@"Input/data.csv")
                                       .Skip(1)
                                       .Select(v => Record.FromCsv(v))
                                       .ToList();
            // list of records timestamp
            List<double> recordTimeStamps = new List<double>();

            foreach (Record record in records)
            {
                recordTimeStamps.Add(record.timestamp);
            }
            recordTimeStamps.Sort();

            // get start and stop timestamp of slots
            double startTimeStampsSlot = recordTimeStamps.First();
            double stopTimeStampsSlot = recordTimeStamps.Last();

            // make list of timestamp slots
            List<double> timeStampsSlots = new List<double>();

            do
            {
                startTimeStampsSlot += 10000;
                timeStampsSlots.Add(startTimeStampsSlot);
            } while (startTimeStampsSlot < stopTimeStampsSlot);

            // search for the date position in the list of timestamp slots
            // TODO: convert to time zone
            double datetimeToTameStamp = Record.ConvertToUnixTimestamp(datetime);

            for (var i = 0; i < timeStampsSlots.Count; i++)
            {
                if (datetimeToTameStamp > timeStampsSlots[i] && datetimeToTameStamp < timeStampsSlots[i + 1])
                {
                    startTimeStampsSlot = timeStampsSlots[i];
                    stopTimeStampsSlot = timeStampsSlots[i + 1];
                    break;
                }

            }

            // find records in the time slot 
            foreach (Record record in records)
            {
                if (
                    record.timestamp > startTimeStampsSlot && record.timestamp < stopTimeStampsSlot &&
                    record.portfolio == portfolio &&
                    record.owner == owner &&
                    record.instrument == instrument
                )
                {
                    // found record
                    Console.WriteLine(record.ToString());
                }
            }

            // TODO: calculate average price for same PIIT
            // TODO: generate and return result

            return $"{portfolio}:{owner}:{instrument}:{datetime}";
        }
    }
}
