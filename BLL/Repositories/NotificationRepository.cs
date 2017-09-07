using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text.RegularExpressions;
using BLL.Interfaces;
using BOL;

namespace BLL.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly YouxelVacation _context;


        public NotificationRepository(YouxelVacation context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;

        }

        public IEnumerable<Notifications> ManagerShownotifications(string employeeId)
        {


            var notifications =
                _context.Notifications.Where(x => x.seen == false && x.EmployeeID == employeeId).ToList();



            foreach (var notify in notifications)
            {

                notify.seen = true;

                _context.Entry(notify).State = EntityState.Modified;



                _context.Entry(notify).Property(x => x.CreatedAT).IsModified = false;
                _context.Entry(notify).Property(x => x.LastUpdated).IsModified = false;
                _context.Entry(notify).Property(x => x.text).IsModified = false;
                _context.Entry(notify).Property(x => x.EmployeeID).IsModified = false;
                _context.SaveChanges();

            }


            return notifications;

        }





        public int NumOfNotifications(string employeeId)
        {

            return _context.Notifications.Count(x => x.EmployeeID.Equals(employeeId) && x.seen.Equals(false));

        }


        public void AddNotification(string text, string requestID)
        {

            var IDs = _context.RequestApproval.Where(g => g.RequestID == requestID).Select(x => x.ApprovalBy).ToList();

            foreach (string id in IDs)
            {
                Notifications notify = new Notifications();
                var listofvc = _context.Notifications.ToList().OrderByDescending(x => x.ID);




                if (!listofvc.Any())
                {
                    notify.ID = "notify-000001";
                }
                else
                {
                    var originalValue = listofvc.Select(x => x.ID).FirstOrDefault();

                    // Get the numeric part (0002)
                    var stringValue = Regex.Match(originalValue, @"\d+").Value;
                    // Convert to int
                    var intValue = Int32.Parse(stringValue);
                    // Increase
                    intValue++;
                    // Convert back to string
                    originalValue = "notify-" + intValue.ToString("D6");




                    notify.ID = originalValue;

                }

                notify.EmployeeID = id;
                notify.CreatedAT = DateTime.Now;
                notify.LastUpdated = DateTime.Now;
                notify.seen = false;
                notify.text = text;

                _context.Notifications.Add(notify);

                _context.SaveChanges();

            }



        }
    }
}
