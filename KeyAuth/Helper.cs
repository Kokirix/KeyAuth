using KeyAuth.Storage;


using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace KeyAuth {
    internal class Helper {
        /// <summary>
        /// Get the POST values for the request.
        /// </summary>
        /// <param name="Referer">The void this is being called from.</param>
        /// <param name="IV">The IV Key.</param>
        /// <returns></returns>
        internal static NameValueCollection GetValues(string Referer, string IV, int Type, params string[] Values) {
            var Value = new NameValueCollection() {
                ["type"] = Referer,
                ["name"] = App.Name,
                ["ownerid"] = App.OwnerID
            };
            if (Type == 4) {
                Value.Add("ver", App.Version);
                Value.Add("hash", Encryption.MD5(Process.GetCurrentProcess().MainModule.FileName));
                Value.Add("enckey", Encryption.Encrypt(App.EncryptionKey, App.Secret, IV));
            }
            if (Type == 0) Value.Add("ver", App.Version);
            if (Type == 1) {
                Value.Add("username", Values[0]);
                Value.Add("pass", Values[1]);
                Value.Add("hwid", User.Hwid);
                Value.Add("init_iv", IV);
            }
            if (Type == 2) {
                Value.Add("username", Values[0]);
                Value.Add("pass", Values[1]);
                Value.Add("key", Values[2]);
                Value.Add("hwid", User.Hwid);
                Value.Add("init_iv", IV);
            }
            if (Type == 3) {
                Value.Add("username", Values[0]);
                Value.Add("key", Values[1]);
                Value.Add("hwid", User.Hwid);
                Value.Add("init_iv", IV);
            }
            if (Type == 5 || Type == 6) {
                Value.Add("var", Values[0]);
                if (Type == 6) Value.Add("data", Values[1]);
            }
            if (Type > 0 && Type != 4) Value.Add("sessionid", User.SessionID);
            if (Type == 7) Value.Add("channel", Values[0]);
            if (Type == 8) {
                Value.Add("channel", Values[0]);
                Value.Add("message", Values[1]);
            }
            if (Type == 9) Value.Add("hwid", User.Hwid);
            if (Type == 10) {
                Value.Add("webid", Values[0]);
                Value.Add("params", Values[1]);
                Value.Add("body", Values[2]);
                Value.Add("conttype", Values[3]);
            }
            if (Type == 11) Value.Add("key", Values[0]);
            if (Type == 13 || Type == 12) Value.Add("fileid", Values[0]);
            return Value;
        }
        /// <summary>
        /// Handles the program requests.
        /// </summary>
        /// <param name="Referer">The name of the void calling this function.</param>
        /// <param name="Type">The type of void calling this function.</param>
        /// <param name="Resources">The parameters for the request.</param>
        /// <returns></returns>
        internal static dynamic Handler(string Referer, int Type, params string[] Resources) {
            User.Hwid = WindowsIdentity.GetCurrent().User.Value;
            var TempIV = Encryption.SHA256(Encryption.IV());
            var Response = Send(GetValues(Referer, TempIV, Type, Resources));
            //Response = Encryption.Decrypt(Response, App.Secret, EncryptionKey);
            if (!Response.Contains("{")) return Response;
            return new JavaScriptSerializer().Deserialize<dynamic>(Response);
        }
        /// <summary>
        /// Auto updates the program
        /// </summary>
        internal static void AutoUpdater() {
            if (!string.IsNullOrWhiteSpace(App.DownloadLink)) {
                var Client = new WebClient();
                var Path = Application.ExecutablePath + "\\" + GetRandomString(new Random().Next(10, 15)) + ".exe";
                if (!App.DelAndOpen) {
                    Client.DownloadFile(App.DownloadLink, Path);
                    Process.Start(Path);
                    Process.Start(new ProcessStartInfo() {
                        Arguments = "/C choice /C Y /N /D Y /T 5 & Del \"" + Application.ExecutablePath + "\"",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true,
                        FileName = "cmd.exe"
                    });
                    Environment.Exit(0);
                }
                else {
                    Process.Start(App.DownloadLink);
                    Environment.Exit(0);
                }
                Api.Status = Results.Operation.Success;
            }
            else Api.Status = Results.Operation.CantUpdate;
        }
        /// <summary>
        /// Applies a dynamic type to the Status and returns it.
        /// </summary>
        /// <param name="Type">The dynamic option to return</param>
        /// <returns></returns>
        internal static dynamic ApplyStatus(dynamic Type) {
            Api.Status = Type;
            return Type;
        }
        /// <summary>
        /// Produces a random string.
        /// </summary>
        /// <param name="length">The preferable string length.</param>
        /// <returns></returns>
        internal static string GetRandomString(int length) => new string(Enumerable.Range(0, length).Select(n => (char)new Random().Next(32, 127)).ToArray());
        /// <summary>
        /// Returns if a string is a valid parameter.
        /// </summary>
        /// <param name="Text">The text to validate.</param>
        /// <returns></returns>
        internal static bool isNull(string Text) => Text == null || string.IsNullOrEmpty(Text);
        /// <summary>
        /// Shows an error message.
        /// </summary>
        /// <param name="Message">The specified error message.</param>
        internal static void Error(string Message) {
            MessageBox.Show(Message, "KeyAuth", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(0);
        }
        /// <summary>
        /// Send a request to the KeyAuth server.
        /// </summary>
        /// <param name="Values">The POST Parameters.</param>
        /// <returns></returns>
        internal static string Send(NameValueCollection Values, string UserAgent = "KeyAuth") {
            try {
                using (WebClient Client = new WebClient()) {
                    Client.Headers["User-Agent"] = UserAgent;
                    Client.Proxy = null;
                    Api.Status = Results.Operation.Success;
                    return Encoding.Default.GetString(Client.UploadValues("https://keyauth.win/api/1.1/", Values));
                }
            }
            catch {
                if (App.ThrowExceptions) Error("Connection Failed.");
                Api.Status = Results.Operation.ConnectionFailed;
            }
            return "";
        }

    }
}
// Made for KeyAuth | Completed by [Kokiri#8556]
