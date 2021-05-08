using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using ARSoft.Tools.Net.Dns;
using System.Net.Sockets;
using ProxyLib.Proxy;
using System.Web;

namespace ALTSecurity.Web.Models.Validation
{
    public class EmailValidation
    {
        const string CRLF = "\r\n";
        /// <summary>
        /// Отримати відповідь
        /// </summary>
        /// <param name="tcpc"></param>
        /// <returns></returns>
        public string GetResult(TcpClient tcpc)
        {
            string returndata = "";
            NetworkStream tps = tcpc.GetStream();
            if (tps.CanRead)
            {
                while (tps.DataAvailable)
                {
                    byte[] bytes = new byte[tcpc.ReceiveBufferSize];
                    tps.Read(bytes, 0, (int)tcpc.ReceiveBufferSize);
                    returndata += Encoding.Default.GetString(bytes).Replace("\0", "");
                }
            }
            return returndata;
        }


        /// <summary>
        /// Розпарсити відповідь
        /// </summary>
        /// <param name="tcpc"></param>
        /// <param name="strCmd"></param>
        /// <returns></returns>
        public bool OperaStream(TcpClient tcpc, string strCmd)
        {
            try
            {
                NetworkStream TcpStream;
                if (strCmd != "") strCmd += CRLF;


                TcpStream = tcpc.GetStream();
                byte[] bWrite = Encoding.Default.GetBytes(strCmd.ToCharArray());
                TcpStream.Write(bWrite, 0, bWrite.Length);
                string sp = "";


                string returndata = GetResult(tcpc);


                sp = returndata.Substring(0, 3);
                if (returndata.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).Length > 1) sp = returndata.Substring(returndata.IndexOf("\r\n") + 2, 3);


                if (sp == "250" || sp == "220") return true;
                return false;
            }
            catch (Exception ex)
            {
                Logger.LogMessage("EmailValidation.OperaStream", "Parsing error");
                return false;
            }
        }


        /// <summary>
        /// Переврірка приналежності email-адреси домену
        /// </summary>
        /// <param name="email"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public bool EmailExists(string email)
        {
            bool rsl = false;
            string strEmail;
            int intPort;
            strEmail = email;
            intPort = 25;
            string domainName;
            domainName = email.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries)[1];

            //Отримати перелык серверів
            DnsStubResolver resolver = new DnsStubResolver();
            var mx = resolver.Resolve<MxRecord>(domainName, RecordType.Mx).OrderBy(x => x.Preference);

            ProxyClientFactory factory = new ProxyClientFactory();
            IProxyClient proxyClient = factory.CreateProxyClient(ProxyType.Http, "102.129.249.120", 8080);

            TcpClient tcpc = null;
            try
            {
                tcpc = proxyClient.CreateConnection(mx.First().ExchangeDomainName.ToString(), intPort);
                string topconet = GetResult(tcpc);
                while (topconet == "") { Thread.Sleep(1000); topconet = GetResult(tcpc); }
                if (topconet.Substring(0, 3) != "220") { throw new Exception("err"); }
                OperaStream(tcpc, "EHLO local.altstudy.com.ua");// here is a test email domain
                OperaStream(tcpc, "NOOP");
                OperaStream(tcpc, "MAIL FROM: <ivan_manager@local.altstudy.com.ua>"); // here is the email address for sending test email.
                OperaStream(tcpc, "NOOP");
                
                if (OperaStream(tcpc, "RCPT TO:<" + strEmail + ">"))
                {
                    rsl = true;
                }

                OperaStream(tcpc, "RSET");
                OperaStream(tcpc, "QUIT");

                tcpc.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException("EmailValidation.EmailExists", ex);

                rsl = false;
                tcpc.Close();
            }

            return rsl;
        }
    }
}