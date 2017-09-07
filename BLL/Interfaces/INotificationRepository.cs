using System.Collections.Generic;
using BOL;

namespace BLL.Interfaces
{
   
   public  interface INotificationRepository
   {

        /// <summary>
        /// Managers the shownotifications.
        /// </summary>
        /// <param name="employeeId">The employee identifier.</param>
        /// <returns></returns>
        IEnumerable<Notifications> ManagerShownotifications(string employeeId);

        /// <summary>
        /// Adds the notification.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="requestID">The request identifier.</param>
        void AddNotification( string text , string requestID);





 

        /// <summary>
        /// Numbers the of notifications.
        /// </summary>
        /// <param name="employeeId">The employee identifier.</param>
        /// <returns></returns>
        int NumOfNotifications(string employeeId);
    }
}
