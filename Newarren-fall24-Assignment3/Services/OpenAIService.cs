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
using Azure.Security.KeyVault.Secrets;

using static System.Environment;
using System.Drawing.Text;
using Newarren_fall24_Assignment3.Services;
/*
 * Calls Chat GPT API
 * 
 */

namespace Newarren_fall24_Assignment3.Services
{
    public class OpenAIService
    {
        private readonly ChatClient _chatClient;
        private readonly KeyVaultService _keyVaultService;
        private readonly string openAIKey;
        private readonly string openAIEndpoint;
        public OpenAIService(String openAIKey, String openAIEndpoint)
        {
            // Replace these values with your Key Vault information
            // get keys
            //openAIKey = "8nnDdipUcqXFIBQDKOuQA5MeEQpHQmrAwYzM9kAjN1NLEXjC08cdJQQJ99AJACYeBjFXJ3w3AAABACOG77Tn";  
           // openAIEndpoint = "https://openaiconn.openai.azure.com/";  

            
            

            if (string.IsNullOrEmpty(openAIKey) || string.IsNullOrEmpty(openAIEndpoint))
            {
                throw new ArgumentNullException("OpenAI key or endpoint not provided in configuration.");
            }

            AzureOpenAIClient azureClient = new(
                new Uri(openAIEndpoint),
                new ApiKeyCredential(openAIKey));
            var chatClient = azureClient.GetChatClient("gpt-35-turbo");

            _chatClient = chatClient;
        }

        public async Task<List<string>> GenerateActorTweetsAsync(string reviewee)
        {

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("When given an actor, create 20 tweets about the actor given. Do not Number them. Separate each tweet with '%%%'"),
                new UserChatMessage($"Write tweets about {reviewee}")
            };


            var options = new ChatCompletionOptions
            {
                Temperature = (float)0.8
            };

            try
            {
                List<string> reviews = new List<string>();

                ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, options);

                if (completion.Content != null)
                {
                    string fullResponse = completion.Content.First().Text;

                    var reviewArray = fullResponse.Split(new[] { "%%%" }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < reviewArray.Length; i++)
                    {
                        reviews.Add(reviewArray[i].Trim()); 
                    }


                    Console.WriteLine(reviews.ToArray());
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
                new UserChatMessage($"The movie's name is '{reviewee}'. Please write reviews for this Movie.")
            };


            var options = new ChatCompletionOptions
            {
                Temperature = (float)0.8
            };

            try
            {
                List<string> reviews = new List<string>();

                ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, options);

                if (completion.Content != null)
                {
                    string fullResponse = completion.Content.First().Text;

                    var reviewArray = fullResponse.Split(new[] { "%%%" }, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < reviewArray.Length; i++)
                    {
                        reviews.Add(reviewArray[i].Trim());
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

        public async Task<List<string>> GenerateMovieListAsync(string actor)
        {
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("Give a list of 5 movies just the title with a given actor. Separate each movie with '%%%', do not number the list"),
                new UserChatMessage($"The actor's name is '{actor}'. Give me 5 movie titles ")
            };


            var options = new ChatCompletionOptions
            {
                Temperature = (float)0.2
            };

            try
            {
                List<string> movies = new List<string>();

                ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, options);

                if (completion.Content != null)
                {
                    string fullResponse = completion.Content.First().Text;

                    var reviewArray = fullResponse.Split(new[] { "%%%" }, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < reviewArray.Length; i++)
                    {
                        //removes whitespace
                        movies.Add(reviewArray[i].Trim());
                    }

                    return movies;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return null;
        }

        public async Task<List<string>> GenerateActorListAsync(string movie)
        {
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("Give a list of 5 actors in a given movie. Separate each actor with '%%%', do not number the list"),
                new UserChatMessage($"The movie's name is '{movie}'. Give me 5 actors ")
            };


            var options = new ChatCompletionOptions
            {
                Temperature = (float)0.2
            };

            try
            {
                List<string> movies = new List<string>();

                ChatCompletion completion = await _chatClient.CompleteChatAsync(messages, options);

                if (completion.Content != null)
                {
                    string fullResponse = completion.Content.First().Text;

                    var reviewArray = fullResponse.Split(new[] { "%%%" }, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < reviewArray.Length; i++)
                    {
                        //removes whitespace
                        movies.Add(reviewArray[i].Trim());
                    }

                    return movies;
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