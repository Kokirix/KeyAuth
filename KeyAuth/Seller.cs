using KeyAuth.Models;
using System.Collections.Generic;

namespace KeyAuth {
    public class Seller {
        /// <summary>
        /// Get all the messages from a channel.
        /// </summary>
        /// <param name="Channel">Channel name.</param>
        /// <returns></returns>
        public static List<ChatMessage> GetChats(string Channel) {
            var Response = Helper.Handler("chatget", 7, Channel);
            if (Response != null) {
                if (Response["success"]) {
                    Api.Status = Results.Operation.Success;
                    var Messages = new List<ChatMessage>();
                    foreach (var Message in Response["messages"]) {
                        Message.Add(new ChatMessage() {
                            Message = Message["message"],
                            Author = Message["author"],
                            Timestamp = Message["timestamp"]
                        });
                    }
                    return Messages;
                }
            }
            else Api.Status = Results.Operation.ResponseIsNull;
            return null;
        }
        /// <summary>
        /// [Must be Authenticated to use] Send a chat to a channel.
        /// </summary>
        /// <param name="Channel">The channel name.</param>
        /// <param name="Message">The message you want to send.</param>
        /// <returns></returns>
        public static Results.Operation ChatSend(string Channel, string Message) {
            var Response = Helper.Handler("chatsend", 8, Channel, Message);
            if (Response != null) {
                if (Response["success"])
                    return Helper.ApplyStatus(Results.Operation.Success);
                else if (Response.Contains("not found"))
                    return Helper.ApplyStatus(Results.Operation.ChannelNotFound);
                return Helper.ApplyStatus(Results.Operation.Failure);
            }
            return Helper.ApplyStatus(Results.Operation.ResponseIsNull);
        }
        /// <summary>
        /// Send a webhook.
        /// </summary>
        /// <param name="WebID">The webhookid. Can be retrieved on-site.</param>
        /// <param name="Parameters">The parameters of your webhook. [Ex: &currency=btc&amount=5]</param>
        /// <param name="Body">The body of the webhook request.</param>
        /// <param name="ContentType">The content type of the request. [Ex: application/json]</param>
        /// <returns></returns>
        public static Results.WebHook SendWebhook(string WebID, string Parameters, string Body = "", string ContentType = "") {
            if (Helper.isNull(Body)) return Helper.ApplyStatus(Results.WebHook.Failure);
            var Response = Helper.Handler("webhook", 10, WebID, Parameters, Body, ContentType);
            if (Response != null) {
                if (Response["success"] && !Response.Contains("Invalid Webhook Token"))
                    return Helper.ApplyStatus(Results.WebHook.Success);
                else if (Response.Contains("50109"))
                    return Helper.ApplyStatus(Results.WebHook.InvalidJSON);
                return Helper.ApplyStatus(Results.WebHook.Failure);
            }
            return Helper.ApplyStatus(Results.WebHook.ResponseWasNull);
        }
    }
}
// Made for KeyAuth | Completed by [Kokiri#8556]