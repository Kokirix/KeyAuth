# KeyAuth

.NET Authentication Library made for KeyAuth. Install the DLL in [Releases](https://github.com/Kokirix/KeyAuth/releases/tag/v1.0.0)
Please let me know if theres any errors.

Creating a new instance
-----------------------
`public static Api Api = new Api("Application Name", "OwnerID", "Secret", "Version", false);`

Please make sure to create this outside of a void and inside of a class,
I didn't test this too much since I want to move on with other things but I will keep updating this
if theres any errors or issues.

Logging in
----------
If you're using a console application try to make the base of your code this:
```C#
Console.Write("Username: ");
var Username = Console.ReadLine();
Console.Write("Password: ");
var Password = Console.ReadLine();

var Result = Api.Login(Username, Password);
if (Result == Results.Login.Success)
    Console.WriteLine("Logged In.");
```
If you're using a WinForms App try to change this up a little to match your case:
```C#
var Result = Api.Login(Username.Text, Password.Text);
if (Result == Results.Login.Success)
    StatusMessage.Text = "Logged In.";
```

Installing a File
-----------------
I've been trying to make this KeyAuth api really simple to dim it down a little bit so it wont be much of an issue to figure out your problem.


But installing a file is ***insanely*** easy to do.
```C#
byte[] FileBytes = Api.Download(. . .);
if(Api.Status == Results.Download.Success);
  File.WriteAllBytes("./example.txt", FileBytes);
```

# After Authentication Only

These snippets will only work properly if a user is already logged in.
If you don't listen than an error will probably occur that I might not want to fix depending on my mood.

Outputting User Variables
-------------------------
If you are making a Console App here is a snippet:
```C#
Console.WriteLine("Username: " + User.Username);
Console.WriteLine("HWID: " + User.Hwid);
Console.WriteLine("Session: " + User.SessionID);
```
On the other hand, if you're making a WinForms application:
```C#
Username.Text = User.Username;
Hwid.Text = User.Hwid;

```

Outputting Subscriptions
------------------------

Sadly I have a snippet for a Console App:
```C#
public static void ShowSubscriptions(){
  if (Api.LoggedIn){ /* Checks if the User is Logged in */
    Console.WriteLine("Subscriptions:\n");
    foreach (var Subscription in User.Subscriptions) {
        Console.WriteLine("Subscription: " + Subscription.Subscription);
        Console.WriteLine("Expiration Date: " + GetTime(long.Parse(Subscription.Expiry))); /* Gets the Expiration Date as a string */
        Console.WriteLine("Key: " + Subscription.Key);
        Console.WriteLine("Time Left [In Seconds]: " + Subscription.TimeLeft);
        Console.WriteLine();
    }
  }
}
public static DateTime GetTime(long unixtime) {
  DateTime Date = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Local);
  return Date.AddSeconds(unixtime).ToLocalTime(); // The Date the Subscription will Expire.
}
```
