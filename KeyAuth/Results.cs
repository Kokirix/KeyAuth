namespace KeyAuth {
    public class Results {
        public enum Registration {
            /// <summary>
            /// It was a success.
            /// </summary>
            Success,
            /// <summary>
            /// The registration failed.
            /// </summary>
            Failure,
            /// <summary>
            /// That name is already in use.
            /// </summary>
            NameTaken,
            /// <summary>
            /// The specified key was invalid.
            /// </summary>
            InvalidKey,
            /// <summary>
            /// The specified key has already been used.
            /// </summary>
            KeyAlreadyUsed,
            /// <summary>
            /// The specified key is restricted from use.
            /// </summary>
            KeyBanned,
            /// <summary>
            /// No subscriptions are available for this user.
            /// </summary>
            NoSubscriptions,
            ResponseWasNull
        }
        public enum Login {
            /// <summary>
            /// Login was a success.
            /// </summary>
            Success,
            /// <summary>
            /// The login was not a success.
            /// </summary>
            Failure,
            /// <summary>
            /// The specified user was not found.
            /// </summary>
            UserNotFound,
            /// <summary>
            /// The password was wrong.
            /// </summary>
            WrongPassword,
            /// <summary>
            /// This user is banned.
            /// </summary>
            Banned,
            /// <summary>
            /// No subscriptions are active for this user.
            /// </summary>
            NoSubscriptions
        }
        public enum Operation {
            Success,
            Failure,
            InvalidVariable,
            ResponseIsNull,
            ParameterWasNull,
            ConnectionFailed,
            RateLimited,
            CantUpdate,
            OutdatedApp,
            InvalidApp,
            ChannelNotFound
        }
        public enum Upgrade {
            UserNotFound,
            KeyAlreadyUsed,
            InvalidKey,
            NoSubscriptions,
            Success,
            NotLoggedIn
        }
        public enum Blacklist {
            NotBlacklisted,
            Blacklisted,
            ResponseWasNull
        }
        public enum WebHook {
            ContentEmpty,
            InvalidWebhook,
            Success,
            ResponseWasNull,
            Failure,
            InvalidJSON
        }
        public enum License {
            KeyWasNull,
            AlreadyUsed,
            Good,
            InvalidKey,
            NoResponse,
            Failure
        }
        public enum Download {
            InvalidFileID,
            Success,
            FileNotFound,
            Failure
        }
    }
}
// Made for KeyAuth | Completed by [Kokiri#8556]