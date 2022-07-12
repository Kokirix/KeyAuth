using KeyAuth.Models;
using KeyAuth.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Script.Serialization;

namespace KeyAuth {
    public class Api {
        public static bool LoggedIn { get; private set; }
        /// <summary>
        /// Gets the status of the KeyAuth Response.
        /// </summary>
        public static dynamic Status { get; internal set; }
        /// <summary>
        /// Create your KeyAuth Instance.
        /// </summary>
        /// <param name="Name">Your appliaction name</param>
        /// <param name="OwnerID">Your OwnerID, it can be found in your account settings.</param>
        /// <param name="Secret">Your Application Secret.</param>
        /// <param name="Version">Application Version,</param>
        /// <param name="ThrowExceptions">Do you want to throw an exception on an error.</param>
        /// <param name="AutoUpdate">Toggle on AutoUpdates if the program version isn't up-to date.</param>
        /// <param name="SaveAndRun">When theres a need to auto-update, delete the current file and automatically install then open the new one.</param>
        public Api(string Name, string OwnerID, string Secret, string Version, bool ThrowExceptions, bool AutoUpdate = true, bool SaveAndRun = true) {
            if (Helper.isNull(Name) || Helper.isNull(OwnerID) || Helper.isNull(Secret) || Helper.isNull(Version)) {
                if (ThrowExceptions) Helper.Error("One or more arguments were null.");
                else Status = Results.Operation.Failure;
            }

            App.Name = Name;
            App.OwnerID = OwnerID;
            App.Secret = Secret;
            App.Version = Version;
            App.AutoUpdate = AutoUpdate;
            App.DelAndOpen = SaveAndRun;
            App.ThrowExceptions = ThrowExceptions;
            if (Status != Results.Operation.Failure)
                Initialize();
        }
        private void Initialize() {
            App.EncryptionKey = Encryption.SHA256(Encryption.IV());
            var Request = Helper.Send(Helper.GetValues("init", Encryption.IV(), 0));
            if (Request != null) {
                if (!Request.Contains("{")) {
                    if (Request.Contains("KeyAuth_Invalid")) {
                        if (App.ThrowExceptions) Helper.Error("Application Not Found.");
                        Status = Results.Operation.InvalidApp;
                    }
                    else if (Request.Contains("invalidver")) {
                        if (App.AutoUpdate) Helper.AutoUpdater();
                        else if (App.ThrowExceptions) Helper.Error("Outdated Application Version.");
                        Status = Results.Operation.OutdatedApp;
                    }
                    return;
                }
                else if (Status != Results.Operation.Failure) {
                    var Response = new JavaScriptSerializer().Deserialize<dynamic>(Request);
                    if (Response["success"]) {
                        App.Users = Convert.ToInt32(Response["appinfo"]["numUsers"]);
                        User.SessionID = Response["sessionid"];
                        App.Logins = Convert.ToInt32(Response["appinfo"]["numOnlineUsers"]);
                        App.Keys = Convert.ToInt32(Response["appinfo"]["numKeys"]);
                        App.Version = Response["appinfo"]["version"];
                        App.CustomerPanelLink = Response["appinfo"]["customerPanelLink"];
                        Status = Results.Operation.Success;
                        App.Initialized = true;
                        return;
                    }
                }
            }
            Status = Results.Operation.Failure;

        }
        /// <summary>
        /// Register a user to your application.
        /// </summary>
        /// <param name="Username">The specified username.</param>
        /// <param name="Password">The user's password.</param>
        /// <param name="Key">The key the user will use.</param>
        public Results.Registration Register(string Username, string Password, string Key) {
            var Response = Helper.Handler("register", 2, Username, Password, Key);
            if (Response != null) {
                if (Helper.Contains(Response, "KeyAuth_Invalid"))
                    return Results.Registration.Failure;
                else if (Response["success"]) {
                    User.Username = Username;
                    User.Hwid = Response["info"]["hwid"];
                    User.IP = Response["info"]["ip"];
                    User.CreatedAt = Response["info"]["createdate"];
                    User.LastLogin = Response["info"]["lastlogin"];
                    var Subscriptions = new List<SubscriptionData>();
                    foreach (var Subscription in Response["info"]["subscriptions"]) {
                        Subscriptions.Add(new SubscriptionData() {
                            Subscription = Subscription["subscription"],
                            Expiry = Subscription["expiry"],
                            Key = Key,
                            TimeLeft = Subscription["timeleft"]
                        });
                    }
                    User.Subscriptions = Subscriptions;
                    LoggedIn = true;
                    return Helper.ApplyStatus(Results.Registration.Success);
                }
                else {
                    if      (Response["message"] == "invalidkey") return Helper.ApplyStatus(Results.Registration.InvalidKey);
                    else if (Response["message"].Contains("Username")) return Helper.ApplyStatus(Results.Registration.NameTaken);
                    else if (Response["message"] == "Key Not Found.") return Helper.ApplyStatus(Results.Registration.InvalidKey);
                    else if (Response["message"] == "Key Already Used.") return Helper.ApplyStatus(Results.Registration.KeyAlreadyUsed);
                    else if (Response["message"] == "Your license is banned.") return Helper.ApplyStatus(Results.Registration.KeyBanned);
                    else if (Response["message"].Contains("no subscription")) return Helper.ApplyStatus(Results.Registration.NoSubscriptions);
                    else return Helper.ApplyStatus(Results.Registration.Failure);
                }
            }
            return Helper.ApplyStatus(Results.Registration.ResponseWasNull);

        }
        /// <summary>
        /// Login to your application.
        /// </summary>
        /// <param name="Username">The username to login with.</param>
        /// <param name="Password">The password that corresponds to the user.</param>
        public Results.Login Login(string Username, string Password) {
            var Response = Helper.Handler("login", 1, Username, Password);
            if (Response != null) {
                if (((Type)Response.GetType()).GetProperties().Any(x => x.Name.Equals("KeyAuth_Invalid")))
                    return Results.Login.Failure;
                else if (Response["success"]) {
                    User.Username = Username;
                    User.IP = Response["info"]["ip"];
                    User.CreatedAt = Response["info"]["createdate"];
                    User.LastLogin = Response["info"]["lastlogin"];
                    User.Hwid = Response["info"]["hwid"];
                    User.CreatedAt = Response["info"]["createdate"];
                    User.LastLogin = Response["info"]["lastlogin"];
                    var Subscriptions = new List<Models.SubscriptionData>();
                    foreach (var Subscription in Response["info"]["subscriptions"]) {
                        Subscriptions.Add(new Models.SubscriptionData() {
                            Subscription = Subscription["subscription"],
                            Expiry = Subscription["expiry"],
                            Key = Subscription["key"],
                            TimeLeft = Subscription["timeleft"]
                        });
                    }
                    User.Subscriptions = Subscriptions;
                    LoggedIn = true;
                    return Helper.ApplyStatus(Results.Login.Success);
                }
                else {
                    if (Response["message"] == "Username not found.") return Helper.ApplyStatus(Results.Login.UserNotFound);
                    else if (Response["message"] == "Password does not match.") return Helper.ApplyStatus(Results.Login.WrongPassword);
                    else if (Response["message"] == "The user is banned") return Helper.ApplyStatus(Results.Login.Banned);
                    else if (Response["message"] == "No active subscriptions found.") return Helper.ApplyStatus(Results.Login.NoSubscriptions);
                    else return Helper.ApplyStatus(Results.Login.Failure);
                }
            }
            return Helper.ApplyStatus(Results.Login.Failure);
        }
        /// <summary>
        /// [Must be Authenticated to use] Upgrade a key.
        /// </summary>
        /// <param name="Key">The key to upgrade.</param>
        public Results.Upgrade Upgrade(string Key, string Username = "") {
            if (!LoggedIn) {
                dynamic Response = "";
                if (Helper.isNull(Username))
                    Response = Helper.Handler("upgrade", 3, User.Username, Key);
                else Response = Helper.Handler("upgrade", 3, Username, Key);
                if (Response["success"])
                    return Helper.ApplyStatus(Results.Upgrade.Success);
                else {
                    if (Response["message"] == "Key Not Found.") return Helper.ApplyStatus(Results.Upgrade.UserNotFound);
                    else if (Response["message"] == "Key Already Used.") return Helper.ApplyStatus(Results.Upgrade.KeyAlreadyUsed);
                    else if (Response["message"] == "Username not found.") return Helper.ApplyStatus(Results.Upgrade.UserNotFound);
                }
            }
            else if (App.ThrowExceptions) Helper.Error("Not logged in!");
            return Helper.ApplyStatus(Results.Upgrade.NotLoggedIn);
        }
        /// <summary>
        /// Login with license
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public Results.License License(string Key) {
            if (Key == null) return Helper.ApplyStatus(Results.License.KeyWasNull);
            else {
                var Response = Helper.Handler("license", 11, Key);
                if (Response != null) {
                    if (Response["success"])
                        return Helper.ApplyStatus(Results.License.Good);
                    else if (Response["message"].Contains("Key Already Used.")) {
                        return Helper.ApplyStatus(Results.License.AlreadyUsed);
                    }
                    else return Helper.ApplyStatus(Results.License.InvalidKey);
                }
                else return Helper.ApplyStatus(Results.License.NoResponse);
            }
        }
        /// <summary>
        /// Get a user variable.
        /// </summary>
        /// <param name="Variable">The variable you wish to return.</param>
        public string GetUserVariable(string Variable) {
            var Response = Helper.Handler("getvar", 5, Variable);
            if (!Helper.isNull(Response)) {
                Status = Results.Operation.Success;
                return Response["response"];
            }
            else if (((Type)Response.GetType()).GetProperties().Any(x => x.Name.Equals("Variable not found for user")))
                Status = Results.Operation.InvalidVariable;
            return null;
        }
        /// <summary>
        /// Returns a global variable.
        /// </summary>
        /// <param name="VariableID">The variable id.</param>
        /// <returns></returns>
        public string GetGlobalVariable(string VariableID) {
            var Response = Helper.Handler("var", 5, VariableID);
            if (!Helper.isNull(Response)) {
                if (Response["success"]) {
                    Helper.ApplyStatus(Results.Operation.Success);
                    return Response["message"];
                }
                else if (((Type)Response.GetType()).GetProperties().Any(x => x.Name.Contains("not found")))
                    return Helper.ApplyStatus(Results.Operation.InvalidVariable);
            }
            Helper.ApplyStatus(Results.Operation.ResponseIsNull);
            return null;
        }
        /// <summary>
        /// Set a user variable.
        /// </summary>
        /// <param name="Variable">The variable you want to set, if it doesn't exist then it will be created.</param>
        /// <param name="Content">The content of the variable to change.</param>
        /// <returns></returns>
        public Results.Operation SetVariable(string Variable, string Content) {
            if (Content == null) return Helper.ApplyStatus(Results.Operation.ParameterWasNull);
            else {
                var Response = Helper.Handler("setvar", 6, Variable, Content);
                if (Response != null) {
                    if (Response["success"])
                        return Helper.ApplyStatus(Results.Operation.Success);
                }
                else return Helper.ApplyStatus(Results.Operation.ResponseIsNull);
            }
            return Helper.ApplyStatus(Results.Operation.Failure);
        }
        /// <summary>
        /// [Must be Authenticated to use] Bans the logged in user
        /// </summary>
        public Results.Operation Ban() {
            var Response = Helper.Handler("ban", 7);
            if (Response["success"]) return Helper.ApplyStatus(Results.Operation.Success);
            return Helper.ApplyStatus(Results.Operation.Failure);
        }
        /// <summary>
        /// Download a file.
        /// </summary>
        /// <param name="FileID">The file id.</param>
        /// <param name="NeedsAuthentication">If the file needs authentication before downloading.</param>
        /// <returns></returns>
        public byte[] Download(string FileID) {
            if (FileID == null) Status = Results.Download.InvalidFileID;
            else {
                dynamic Response = Helper.Handler("file", 12, FileID);
                
                if (Response != null) {
                    if (Response["success"]) {
                        Status = Results.Download.Success;
                        return Encryption.StringToByteArray(Response["contents"]);
                    }
                    else if (Response["message"] == "File not Found")
                        Status = Results.Download.FileNotFound;
                }
                else Status = Results.Operation.ResponseIsNull;
            }
            return null;
        }
    }
}
// Made for KeyAuth | Completed by [Kokiri#8556]
