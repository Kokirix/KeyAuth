using KeyAuth.Models;
using System.Collections.Generic;

namespace KeyAuth.Storage {
    /// <summary>
    /// Storage will be widely available after initialization with a login or registration.
    /// </summary>
    public class User {
        /// <summary>
        /// The current user's username.
        /// </summary>
        public static string Username { get; internal set; }
        /// <summary>
        /// The user's session id.
        /// </summary>
        public static string SessionID { get; internal set; }
        /// <summary>
        /// The user's ip address.
        /// </summary>
        public static string IP { get; internal set; }
        /// <summary>
        /// The key the user used if the Register void is called.
        /// </summary>
        public static string Key { get; internal set; }
        /// <summary>
        /// The user's hardware id.
        /// </summary>
        public static string Hwid { get; internal set; }
        /// <summary>
        /// The user's creation date.
        /// </summary>
        public static string CreatedAt { get; internal set; }
        /// <summary>
        /// The last time this user logged in.
        /// </summary>
        public static string LastLogin { get; internal set; }
        /// <summary>
        /// Lists All Subscriptions.
        /// </summary>
        public static List<SubscriptionData> Subscriptions { get; internal set; }
        /// <summary>
        /// Checks if the user is blacklisted
        /// </summary>
        /// <returns>A enumerator</returns>
        public Results.Blacklist IsBlacklisted() {
            var Response = Helper.Handler("checkblacklist", 9);
            if (Response != null) {
                if (Response["success"])
                    return Helper.ApplyStatus(Results.Blacklist.Blacklisted);
                else if (!Response["success"])
                    return Helper.ApplyStatus(Results.Blacklist.NotBlacklisted);
            }
            return Helper.ApplyStatus(Results.Blacklist.ResponseWasNull);
        }
        /// <summary>
        /// Is the session validated?
        /// </summary>
        /// <returns>Boolean</returns>
        public static bool SessionValidated() {
            if (SessionID != null) {
                var Req = Helper.Send(new System.Collections.Specialized.NameValueCollection() { ["type"] = "check", ["Name"] = App.Name, ["sessionid"] = SessionID, ["ownerid"] = App.OwnerID });
                if (Req != "KeyAuth_Invalid" && Req.Contains("Session")) return true;
            }
            return false;
        }
    }
    public class App {
        /// <summary>
        /// The application name.
        /// </summary>
        public static string Name { get; internal set; }
        /// <summary>
        /// The application owner id.
        /// </summary>
        public static string OwnerID { get; internal set; }
        /// <summary>
        /// The application secret.
        /// </summary>
        internal static string Secret { get; set; }
        /// <summary>
        /// If the application will throw an exception on an error.
        /// </summary>
        internal static bool ThrowExceptions { get; set; }
        /// <summary>
        /// The application version, helps AutoUpdater.
        /// </summary>
        public static string Version { get; internal set; }
        /// <summary>
        /// The user count this application has.
        /// </summary>
        public static int Users { get; internal set; }
        /// <summary>
        /// The login count, resets every day.
        /// </summary>
        public static int Logins { get; internal set; }
        /// <summary>
        /// The key count this application has.
        /// </summary>
        public static int Keys { get; internal set; }
        /// <summary>
        /// The Customer Panel Link.
        /// </summary>
        public static string CustomerPanelLink { get; internal set; }
        /// <summary>
        /// File Download Link in this application.
        /// </summary>
        public static string DownloadLink { get; private set; }
        internal static bool AutoUpdate { get; set; }
        internal static bool DelAndOpen { get; set; }
        internal static string EncryptionKey { get; set; }
        internal static bool Initialized { get; set; }
    }
}
// Made for KeyAuth | Completed by [Kokiri#8556]