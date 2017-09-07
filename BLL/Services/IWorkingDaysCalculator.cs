using System;

namespace BLL.Services
{
   public  interface IWorkingDaysCalculator
    {

        /// <summary>
        /// Gets the working days.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns></returns>
       int GetWorkingDays( DateTime? start, DateTime? end);
    }
}
