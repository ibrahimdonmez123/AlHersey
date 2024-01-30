using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Net;
using System.Net.Mail;
using System.Text;
using XSystem.Security.Cryptography;

namespace AlHersey.Models
{
    public interface IUserRepository
    {
        Task<List<User>> UserSelectAsync();
        Task<User?> UserDetailsAsync(int? id);
        Task<User?> LoginControlAsync(User user);
        string MD5Sifrele(string value);
        Task<User?> SelectMemberInfoAsync(string email);
        Task<bool> LoginEmailControlAsync(User user);
        Task<bool> AddUserAsync(User user);
        Task<string> MemberControlAsync(User user);
        Task<bool> SendSmsAsync(string OrderGroupGUID);
        Task<string> XmlPostAsync(string url, string ss);
        Task SendEmailAsync(string OrderGroupGUID);
    }

    public class Cls_User : IUserRepository
    {
        private readonly AlHerseyContext context;
        public Cls_User(AlHerseyContext _context)
        {
            context = _context;
        }

        public async Task<List<User>> UserSelectAsync()
        {
            List<User> users = await context.Users.ToListAsync();
            return users;
        }

        public async Task<User?> UserDetailsAsync(int? id)
        {
            User? user = await context.Users.FindAsync(id);
            return user;
        }

        public async Task<User?> LoginControlAsync(User user)
        {
            string md5sifrele = MD5Sifrele(user.Password);

            User? usr = await context.Users.FirstOrDefaultAsync(u => u.Email == user.Email && u.Password == md5sifrele && u.IsAdmin == true && u.Active == true);

            return usr;
        }

        public string MD5Sifrele(string value)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] btr = Encoding.UTF8.GetBytes(value);
            btr = md5.ComputeHash(btr);

            StringBuilder sb = new StringBuilder();
            foreach (byte item in btr)
            {
                sb.Append(item.ToString("x2").ToLower());
            }
            return sb.ToString();
        }

        public async Task<User?> SelectMemberInfoAsync(string email)
        {
            User? user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }

        public async Task<bool> LoginEmailControlAsync(User user)
        {
            User? usr = await context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);

            if (usr == null)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> AddUserAsync(User user)
        {
            try
            {
                user.Active = true;
                user.IsAdmin = false;
                user.Password = MD5Sifrele(user.Password);
                context.Users.Add(user);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<string> MemberControlAsync(User user)
        {
            string answer = "";

            try
            {
                string md5Sifre = MD5Sifrele(user.Password);
                User? usr = await context.Users.FirstOrDefaultAsync(u => u.Email == user.Email && u.Password == md5Sifre);

                if (usr == null)
                {
                    answer = "error";
                }
                else
                {
                    if (usr.IsAdmin == true)
                    {
                        answer = "admin";
                    }
                    else
                    {
                        answer = usr.Email;
                    }
                }
            }
            catch (Exception)
            {
                return "HATA";
            }
            return answer;
        }

        public async Task<bool> SendSmsAsync(string OrderGroupGUID)
        {
            try
            {
                string ss = "<?xml version='1.0' encoding='UTF-8'?> ";
                ss += "<mainbody>";
                ss += "<header>";
                ss += "<company dil='TR'> üyelikte size verilen şirket ismi buraya yazılacak </company>";
                ss += "<usercode> size verilen usercode </usercode>";
                ss += "<password> size verilen şifre </password>";
                ss += "<startdate></startdate>";
                ss += "<stopdate></stopdate>";
                ss += "<msgheader></msgheader>";
                ss += "</header>";
                ss += "<body>";

                int userID = (await context.Orders.FirstOrDefaultAsync(o => o.OrderGroupGUID == OrderGroupGUID)).UserID;
                User user = await context.Users.FirstOrDefaultAsync(u => u.UserID == userID);

                string content = "Sayın" + user.NameSurname + "," + DateTime.Now + " tarihinde " + OrderGroupGUID + " nolu siparişiniz alınmıştır.";
                ss += "<mp><msg><!CDATA[" + content + "]></msg><no>" + user.Telephone + "</no></mp>";
                ss += "</body>";
                ss += "</mainbody>";

                string result = await XmlPostAsync("https://api.netgsm.com.tr/xmlbulpost.asp", ss);

                if (result != "-1")
                {
                    //Sms gitti, Order tablosunda sms koluna true bas
                }
                else
                {
                    //Sms gitmedi, Order tablosunda sms koluna false bas
                    //ilgili admin personeline email gönder.
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<string> XmlPostAsync(string url, string ss)
        {
            try
            {
                WebClient wUploud = new WebClient();
                HttpWebRequest request = WebRequest.Create(new Uri(url)) as HttpWebRequest;

                request.Method = "POST";
                request.ContentType = "application/x-xxx-form-urlencoded";

                Byte[] bPostArray = Encoding.UTF8.GetBytes(ss);
                Byte[] bResponse = await wUploud.UploadDataTaskAsync(request.RequestUri, "POST", bPostArray);
                Char[] sReturnChars = Encoding.UTF8.GetChars(bResponse);

                string sWebpage = new string(sReturnChars);
                return sWebpage;
            }
            catch (Exception)
            {
                return "-1";
            }
        }



        public async Task SendEmailAsync(string OrderGroupGUID)
        {
            Order order = await context.Orders.FirstOrDefaultAsync(o => o.OrderGroupGUID == OrderGroupGUID);
            User user = await context.Users.FirstOrDefaultAsync(u => u.UserID == order.UserID);

            string mail = "gonderen email buraya info@ibrahim.com";
            string _mail = user.Email;
            string subject = "";
            string content = "";

            content = "Sayın " + user.NameSurname + "," + DateTime.Now + " tarihinde " + OrderGroupGUID + " nolu siparişiniz alınmıştır.";

            subject = "Sayın " + user.NameSurname + " siparişiniz alınmıştır.";

            string host = "smtp.iakademi.com";
            int port = 587;
            string login = "mailserver a baglanılan login buraya";
            string password = "mailserver a baglanılan şifre buraya";

            MailMessage e_posta = new MailMessage();
            e_posta.From = new MailAddress(mail, "inci bilgi"); //gönderen
            e_posta.To.Add(_mail); //alıcı
            e_posta.Subject = subject;
            e_posta.IsBodyHtml = true;
            e_posta.Body = content;

            SmtpClient smtp = new SmtpClient();
            smtp.Credentials = new NetworkCredential(login, password);
            smtp.Port = port;
            smtp.Host = host;

            try
            {
                await smtp.SendMailAsync(e_posta);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
