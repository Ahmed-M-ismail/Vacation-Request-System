using System;

namespace BLL.Services
{
    public  class WorkingDaysCalculator :  IWorkingDaysCalculator
    {

    

    
 



        public int GetWorkingDays(DateTime? start, DateTime? end)
        {

            int days = 0;
            while (start <= end)
            {
                if (start.Value.DayOfWeek != DayOfWeek.Friday && start.Value.DayOfWeek != DayOfWeek.Saturday)
                {
                    ++days;
                }
              start = start.Value.AddDays(1);
            }
            return days;
        }
    }
}
