using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.Bot.Sample.LuisBot
{
    // For more information about this template visit http://aka.ms/azurebots-csharp-luis
    [LuisModel("{luis_app_id}", "{subscription_key}")]
    [Serializable]
    public class BasicLuisDialog : LuisDialog<object>
    {
        //[{"name":"XBox","date":"2017-12-13T10:46:59.8092593Z"}]
        private class WishListEntry
        {
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("date")]
            public DateTime Date { get; set; }
        }

        public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(ConfigurationManager.AppSettings["LuisAppId"], ConfigurationManager.AppSettings["LuisAPIKey"])))
        {
        }

        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            const string presentUrl = "https://frbxmashack2017.azurewebsites.net/api/GetWishlist?code=/DnTnrysKWgCtMmdmSdqDmI08SrDiiwavqft9o6mwyWwWPVVfeYNhA==";

            var client = new HttpClient();
            var response = await client.GetAsync(presentUrl);
            var s = "";
            var list = JsonConvert.DeserializeObject<List<WishListEntry>>(await response.Content.ReadAsStringAsync());
            if (list.Count > 1) s = "es";
            var idx = new Random().Next(list.Count);
            var randomItem = list[idx];
            var msg = $"Not sure what you mean. Anyway so far, your list contains {list.Count} wish{s}, among others a {randomItem.Name}";

            await context.SayAsync(msg);

            context.Wait(MessageReceived);
        }

        // Go to https://luis.ai and create a new intent, then train/publish your luis app.
        // Finally replace "MyIntent" with the name of your newly created intent in the following handler
        [LuisIntent("weather")]
        public async Task Weather(IDialogContext context, LuisResult result)
        {
            const string weatherUrl = "https://frbxmashack2017.azurewebsites.net/api/GetWeather?code=TAS3VaGt5RvskcqAgwZdeRUQxkb/C5uckjlEaRXYlvDz2Nvr88Vu6Q==";

            var client = new HttpClient();

            var weatherResponse = await client.GetStringAsync(weatherUrl);
            var msg = $"The weather here is {weatherResponse}";
            await context.SayAsync(msg, speak:msg);
            context.Wait(MessageReceived);
        }


        [LuisIntent("welcome")]
        public async Task Welcome(IDialogContext context, LuisResult result)
        {
            const string presentUrl = "https://frbxmashack2017.azurewebsites.net/api/GetWishlist?code=/DnTnrysKWgCtMmdmSdqDmI08SrDiiwavqft9o6mwyWwWPVVfeYNhA==";

            var client = new HttpClient();
            var response = await client.GetAsync(presentUrl);
            var s = "";
            var list = JsonConvert.DeserializeObject<List<WishListEntry>>(await response.Content.ReadAsStringAsync());
            if (list.Count > 1) s = "es";
            var msg = $"Nice to meet you. I am Santa's helper, so far I know your {list.Count} wish{s}. How about you add some more?";
            await context.SayAsync(msg, speak:msg); //
            context.Wait(MessageReceived);
        }

        [LuisIntent("present")]
        public async Task Present(IDialogContext context, LuisResult result)
        {
            const string presentUrl = "https://frbxmashack2017.azurewebsites.net/api/UpdateWishlist?code=UuKcdc5YTMPp8k0yfBHF/39/e3EvjiUxoikbi1mEQ2eaVuWyKtFJ3w==";

            var client = new HttpClient();
            
            // error handling is left as an exercise for the user ;-)
            var entity = result.Entities[0];
            var presentName = entity.Entity;

            var response = await client.PostAsJsonAsync(presentUrl, new { name = presentName });

            var msg = "Consider it done";
            await context.SayAsync(msg, speak: msg);

            context.Wait(MessageReceived);
        }

        [LuisIntent("tree")]
        public async Task TreeLights(IDialogContext context, LuisResult result)
        {
            const string treeUrl = "https://frbtalktotree.azurewebsites.net/api/TalkToTreeFunction?code=utSazS2dMOcOgXPNRx3AHDyDJHDjbMP1RIp68NqmaQEz8y6BlSJ2DA==";
            
            var client = new HttpClient();
            var response = await client.GetAsync(treeUrl); 

            var msg = "Merry Christmas! ho! ho! ho!";
            await context.SayAsync(msg, speak: msg);
            context.Wait(MessageReceived);
        }
    }
}