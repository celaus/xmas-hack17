using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Collections.Generic;

namespace Microsoft.Bot.Sample.LuisBot
{
    // For more information about this template visit http://aka.ms/azurebots-csharp-luis
    [Serializable]
    public class BasicLuisDialog : LuisDialog<object>
    {

        private string skills = "weather, presents";

        public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(ConfigurationManager.AppSettings["LuisAppId"], ConfigurationManager.AppSettings["LuisAPIKey"])))
        {
        }

        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"You have reached the none intent. You said: {result.Query}"); //
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
            await context.PostAsync($"The weather here is {weatherResponse}");
            context.Wait(MessageReceived);
        }

        [LuisIntent("personality")]
        public async Task Personality(IDialogContext context, LuisResult result)
        {
            var myStuff = new List<String>(new[] { "horse", "donkey" });

            await context.PostAsync($"Nice to meet you. I am Santa's helper, so far I know your {myStuff.Count} wishes from you. Do you want more stuff?"); //
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

            await context.PostAsync("Consider it done");

            context.Wait(MessageReceived);
        }
    }
}