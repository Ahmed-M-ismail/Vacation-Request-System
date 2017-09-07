using System;
using BOL;

namespace BLL.Interfaces
{
    public interface IEmployeeRepository : IDisposable
    {

        Employee Login(Employee emp);
        void ChangePassword(Employee emp, string empid);

        bool   LostPassword( string  email );


        int emptype();

    }


}
