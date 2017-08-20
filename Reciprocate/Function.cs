using Amazon.Lambda.Core;
using Newtonsoft.Json;
using Slight.Alexa.Framework.Models.Requests;
using Slight.Alexa.Framework.Models.Responses;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Reciprocate
{
    public class Function
    {
        public SkillResponse FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            LambdaLogger.Log($"SkillRequest input: {JsonConvert.SerializeObject(input)}");

            Response response = new Response();
            IOutputSpeech innerResponse = null;

            if (input.GetRequestType() == typeof(Slight.Alexa.Framework.Models.Requests.RequestTypes.ILaunchRequest))
            {
                LambdaLogger.Log($"Default LaunchRequest made");

                innerResponse = new PlainTextOutputSpeech();
                (innerResponse as PlainTextOutputSpeech).Text = "What's For Dinner tells you what you should cook for dinner tonight. What ingredient would you like to include?";
                
                response.ShouldEndSession = false;
            }
            else if (input.GetRequestType() == typeof(Slight.Alexa.Framework.Models.Requests.RequestTypes.IIntentRequest))
            {
                var intent = input.Request.Intent.Name;

                LambdaLogger.Log($"Intent Requested {intent}");

                var responseText = "";
                
                if (intent == "RecipeIntent")
                {
                    var ingredient = "";

                    if (input.Request.Intent.Slots.ContainsKey("ingredient") && !string.IsNullOrWhiteSpace(input.Request.Intent.Slots["ingredient"].Value))
                    {
                        ingredient = input.Request.Intent.Slots["ingredient"].Value;
                        LambdaLogger.Log($"Ingredient requested: {ingredient}");
                    }

                    if (ingredient.ToLower()  == "version")
                    {
                        responseText = "This is What's For Dinner version 1.0";
                    }
                    else
                    {
                        Task.Run(async () => responseText = await RecipeGetter.Get(ingredient)).Wait();
                    }
                
                    response.ShouldEndSession = true;
                }
                else if (intent == "AMAZON.HelpIntent")
                {
                    responseText = "You can ask me what you should cook for dinner. What ingredient would you like to include?";
                    response.ShouldEndSession = false;
                }
                else if (intent == "AMAZON.StopIntent")
                {
                    response.ShouldEndSession = true;
                }
                else if (intent == "AMAZON.CancelIntent")
                {
                    response.ShouldEndSession = true;
                }

                LambdaLogger.Log($"Response: {responseText} - Session should end: {response.ShouldEndSession}");

                innerResponse = new PlainTextOutputSpeech();
                (innerResponse as PlainTextOutputSpeech).Text = responseText;
            }

            response.OutputSpeech = innerResponse;

            var skillResponse = new SkillResponse();
            skillResponse.Response = response;
            skillResponse.Version = "1.0";

            return skillResponse;
        }
    }
}
