using Microsoft.AspNetCore.Mvc;
using Azure.AI.OpenAI;
using Microsoft.AspNetCore.DataProtection;
using OpenAI;
using Azure;
using static System.Net.WebRequestMethods;
using OpenAI.Chat;
using System.ClientModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Identity;

using static System.Environment;

namespace Newarren_fall24_Assignment3.Services
{
    public class OpenAIService
    {
        private readonly ChatClient _chatClient;
        private readonly string openAIKey;
        private readonly string openAIEndpoint;
        public OpenAIService()
        {
            // Fetch key and endpoint from configuration
            openAIKey = "8nnDdipUcqXFIBQDKOuQA5MeEQpHQmrAwYzM9kAjN1NLEXjC08cdJQQJ99AJACYeBjFXJ3w3AAABACOG77Tn";  // Must match key in appsettings.json
            openAIEndpoint = "https://openaiconn.openai.azure.com/openai/deployments/gpt-35-turbo/chat/completions?api-version=2024-08-01-preview";  // Must match endpoint in appsettings.json

            // Check if values were properly retrieved, throw an exception if not
            if (string.IsNullOrEmpty(openAIKey) || string.IsNullOrEmpty(openAIEndpoint))
            {
                throw new ArgumentNullException("OpenAI key or endpoint not provided in configuration.");
            }

            // Initialize the Azure OpenAI client
            AzureOpenAIClient azureClient = new(
                new Uri(openAIEndpoint),
                new ApiKeyCredential(openAIKey));
            var chatClient = azureClient.GetChatClient("gpt-35-turbo");

            // _chatClient is initialized correctly for later use
            _chatClient = chatClient;
        }

        public async Task<List<string>> GenerateActorTweetsAsync(string reviewee)
        {

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("When given an actors, create 20 tweets about the actor given. Separate each tweet with '%%%'"),
                new UserChatMessage($"Write tweets about {reviewee}")
            };


            var options = new ChatCompletionOptions
            {
                Temperature = (float)0.8
            };

            try
            {
                List<string> reviews = new List<string>();

                // Make the API request for chat completion
                ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, options);

                if (completion.Content != null)
                {
                    // Assuming completion.Content contains the full response as text
                    string fullResponse = completion.Content.First().Text;

                    // Split the response based on the '###' separator
                    var reviewArray = fullResponse.Split(new[] { "%%%" }, StringSplitOptions.RemoveEmptyEntries);

                    // Add each split review to the reviews list
                    for (int i = 0; i < reviewArray.Length; i++)
                    {
                        reviews.Add(reviewArray[i].Trim());  // Trim any extra whitespace
                    }

                    return reviews;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        public async Task<List<string>> GenerateMovieReviewsAsync(string reviewee)
        {
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("Write 10 short reviews (3-5 sentences) for the given movie, and give a rating out of 10. Separate each review with '%%%'"),
                new UserChatMessage($"The actor's name is '{reviewee}'. Please write reviews for this actor.")
            };


            var options = new ChatCompletionOptions
            {
                Temperature = (float)0.8
            };

            try
            {
                List<string> reviews = new List<string>();

                // Make the API request for chat completion
                ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, options);

                if (completion.Content != null)
                {
                    // Assuming completion.Content contains the full response as text
                    string fullResponse = completion.Content.First().Text;

                    // Split the response based on the '###' separator
                    var reviewArray = fullResponse.Split(new[] { "%%%" }, StringSplitOptions.RemoveEmptyEntries);

                    // Add each split review to the reviews list
                    for (int i = 0; i < reviewArray.Length; i++)
                    {
                        reviews.Add(reviewArray[i].Trim());  // Trim any extra whitespace
                    }

                    return reviews;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }
    }
}