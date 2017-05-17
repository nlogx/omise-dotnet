using System;
namespace Omise.Models
{
    public class ScheduleOnRequest : Request
    {
        public Weekdays[] Weekdays { get; set; }
        public int[] DaysOfMonth { get; set; }
        public String WeekdayOfMonth { get; set; }
    }

    public class CreateScheduleRequest : Request
    {
        public int Every { get; set; }
        public SchedulePeriod Period { get; set; }
        public ScheduleOnRequest On { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ChargeScheduling Charge { get; set; }

        public CreateScheduleRequest()
        {
            On = new ScheduleOnRequest();
        }
    }
}
