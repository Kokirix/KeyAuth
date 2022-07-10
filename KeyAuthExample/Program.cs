using KeyAuth;
using KeyAuth.Storage;
using System;

namespace KeyAuthExample {
    internal class Program {
        public static Api Api = new Api("AppName", "OwnerID", "Secret", "1.0", false, false);
        static void Main(string[] args) {
        Retry:
            Console.Title = "KeyAuth Example";
            Console.Write("Options:\n[1] Login\n[2] Register\n[3] Upgrade\n[4] Check License\n[Choose] -> ");
            dynamic Result = Console.ReadLine();
            if (Result != "1" && Result != "2" && Result != "3" && Result != "4")
                goto Retry;
            Result = int.Parse(Result);
            Console.WriteLine();
            string Username, Password, Key;
            switch (Result) {
                case 1:
                    Console.Write("Username: ");
                    Username = Console.ReadLine();
                    Console.Write("\nPassword: ");
                    Password = Console.ReadLine();
                    Result = Api.Login(Username, Password);
                    switch (Result) {
                        case Results.Login.Success:
                            break;
                        case Results.Login.UserNotFound:
                            Console.WriteLine("That user does not exist.");
                            return;
                        case Results.Login.Banned:
                            Console.WriteLine("This user is banned.");
                            return;
                        case Results.Login.WrongPassword:
                            Console.WriteLine("Wrong Password.");
                            return;
                        default:
                            Console.WriteLine("Could not login.");
                            return;
                    }
                    break;
                case 2:
                    Console.Write("Username: ");
                    Username = Console.ReadLine();
                    Console.Write("Password: ");
                    Password = Console.ReadLine();
                    Console.Write("Key: ");
                    Key = Console.ReadLine();
                    Result = Api.Register(Username, Password, Key);
                    switch (Result) {
                        case Results.Registration.Success:
                            break;
                        case Results.Registration.NameTaken:
                            Console.WriteLine("That name is already taken.");
                            return;
                        case Results.Registration.ResponseWasNull:
                            Console.WriteLine("Try checking your connection.");
                            return;
                        case Results.Registration.NoSubscriptions:
                            Console.WriteLine("This key has no subscriptions on it.");
                            return;
                        case Results.Registration.InvalidKey:
                            Console.WriteLine("This key does not exist.");
                            return;
                        case Results.Registration.KeyAlreadyUsed:
                            Console.WriteLine("This key has already been used.");
                            return;
                    }
                    break;
                case 3:
                    Console.Write("Username: ");
                    Username = Console.ReadLine();
                    Console.Write("Key: ");
                    Key = Console.ReadLine();
                    Result = Api.Upgrade(Key, Username);
                    switch (Result) {
                        case Results.Upgrade.Success:
                            Console.WriteLine("Upgraded Successfully!"); break;
                        case Results.Upgrade.UserNotFound:
                            Console.WriteLine("That username isn't correct."); return;
                        case Results.Upgrade.InvalidKey:
                            Console.WriteLine("The key was null."); return;
                        case Results.Upgrade.KeyAlreadyUsed:
                            Console.WriteLine("The key has already been used."); return;
                        default:
                            Console.WriteLine("Could not upgrade."); return;
                    }
                    break;
                case 4:
                    Console.Write("Key: ");
                    Key = Console.ReadLine();
                    Result = Api.License(Key);
                    switch (Result) {
                        case Results.License.Good:
                            break;
                        case Results.License.NoResponse:
                            Console.WriteLine("Try checking your connection."); return;
                        case Results.License.AlreadyUsed:
                            Console.WriteLine("Key has already been used."); return;
                        case Results.License.InvalidKey:
                            Console.WriteLine("This key is invalid."); return;
                        case Results.License.Failure:
                            Console.WriteLine("Something went wrong."); return;
                        default:
                            Console.WriteLine("Could not verify this key is valid."); return;
                    }
                    Console.WriteLine(Result);
                    break;
            }
            if (Api.LoggedIn)
                ShowSubscriptions();
            Console.Read();
        }
        public static void ShowSubscriptions() {
            if (Api.LoggedIn) {
                Console.WriteLine("\nSubscriptions:\n");
                foreach (var Subscription in User.Subscriptions) {
                    Console.WriteLine("Subscription: " + Subscription.Subscription);
                    Console.WriteLine("Expiration Date: " + GetTime(long.Parse(Subscription.Expiry)));
                    Console.WriteLine("Key: " + Subscription.Key);
                    Console.WriteLine("Time Left [In Seconds]: " + Subscription.TimeLeft);
                    Console.WriteLine();
                }
            }
        }
        public static DateTime GetTime(long Unix) {
            DateTime Date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
            return Date.AddSeconds(Unix).ToLocalTime();
        }
    }
}