using System.Security.Cryptography;

namespace BLL.Services
{
   public  interface IGetMd5Hash
   {

       string Md5Hash(MD5 md5Hash, string input);
   }
}
