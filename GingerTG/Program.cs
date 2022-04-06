// <copyright file="MainForm.cs" company="PublicDomain.is">
//     CC0 1.0 Universal (CC0 1.0) - Public Domain Dedication
//     https://creativecommons.org/publicdomain/zero/1.0/legalcode
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Fclp;
using Telegraph.Net;
using Telegraph.Net.Models;

namespace GingerTG
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            // Set version for generating semantic version 
            Version version = typeof(MainClass).GetTypeInfo().Assembly.GetName().Version;

            // Info
            Console.WriteLine($"GingerTG v{version.Major}.{version.Minor}.{version.Build}{Environment.NewLine}");

            // Fluent
            var p = new FluentCommandLineParser();

            // Action
            p.Setup<string>('a', "action")
             .Callback(action => ProcessActionAsync(action, args))
             .Required();

            // Parse
            var result = p.Parse(args);

            // Check for errors
            if (result.HasErrors)
            {
                // Advise user
                Console.WriteLine(result.ErrorText);
            }
        }

        /// <summary>
        /// Processes the action.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="args">Arguments.</param>
        private static async void ProcessActionAsync(string action, string[] args)
        {
            // Process implemented methods
            switch (action)
            {
                // Create account
                case "createAccount":

                    /* Parse arguments */

                    // Fluent
                    var createAccountParser = new FluentCommandLineParser<CreateAccountArguments>();

                    // short_name
                    createAccountParser.Setup(arg => arg.ShortName)
                        .As('s', "short-name")
                        .Required();

                    // author_name
                    createAccountParser.Setup(arg => arg.AuthorName)
                        .As('n', "author-name");

                    // author_url
                    createAccountParser.Setup(arg => arg.AuthorUrl)
                        .As('u', "author-url");

                    // Parse
                    var createAccountParserResult = createAccountParser.Parse(args);

                    // Check for errors
                    if (createAccountParserResult.HasErrors)
                    {
                        // Advise user
                        Console.WriteLine(createAccountParserResult.ErrorText);

                        // Halt flow
                        return;
                    }

                    /* Create account */

                    // Advise user
                    Console.WriteLine("Creating account...");

                    try
                    {
                        // Set telegraph client
                        var client = new TelegraphClient();

                        // Create account
                        var account = client.CreateAccountAsync(createAccountParser.Object.ShortName, createAccountParser.Object.AuthorName, createAccountParser.Object.AuthorUrl);

                        // Check if OK
                        if (account.Result.AccessToken.Length > 0)
                        {
                            // Set account data
                            string accountData =
                            $"short_name: {account.Result.ShortName}{Environment.NewLine}" +
                            $"author_name: {account.Result.AuthorName}{Environment.NewLine}" +
                            $"author_url: {account.Result.AuthorUrl}{Environment.NewLine}" +
                            $"access_token: {account.Result.AccessToken}{Environment.NewLine}" +
                            $"auth_url: {account.Result.AuthorizationUrl}";

                            // Set message
                            string createAccountMessage = $"{Environment.NewLine}{Environment.NewLine}{DateTime.Now}{Environment.NewLine}{accountData}";

                            // Save account data to disk
                            File.AppendAllText("GingerTG-Accounts.txt", createAccountMessage);

                            // Advise user
                            Console.WriteLine($"Account created.{createAccountMessage}");
                        }
                        else
                        {
                            // Throw exception with error message
                            throw new Exception("No access token returned.");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Advise user
                        Console.WriteLine($"Create account error: {ex.Message}");

                        // Log to file
                        File.AppendAllText("GingerTG-ErrorLog.txt", $"{Environment.NewLine}{Environment.NewLine}{DateTime.Now}{Environment.NewLine}Create account error: { ex.Message}");

                        // Halt flow
                        return;
                    }

                    break;

                // Create page
                case "createPage":

                    /* Parse arguments */

                    // Fluent
                    var createPageParser = new FluentCommandLineParser<CreatePageArguments>();

                    // short_name
                    createPageParser.Setup(arg => arg.ShortName)
                        .As('s', "short-name");

                    // access_token
                    createPageParser.Setup(arg => arg.AccessToken)
                        .As('k', "access-token")
                        .Required();

                    // title
                    createPageParser.Setup(arg => arg.Title)
                        .As('t', "title")
                        .Required();

                    // author_name
                    createPageParser.Setup(arg => arg.AuthorName)
                        .As('n', "author-name");

                    // author_url
                    createPageParser.Setup(arg => arg.AuthorUrl)
                        .As('u', "author-url");

                    // content
                    createPageParser.Setup(arg => arg.Content)
                        .As('c', "content")
                        .Required();

                    // return_content
                    createPageParser.Setup(arg => arg.ReturnContent)
                        .As('r', "return-content");

                    // Parse
                    var createPageParserResult = createPageParser.Parse(args);

                    // Check for errors
                    if (createPageParserResult.HasErrors)
                    {
                        // Advise user
                        Console.WriteLine(createPageParserResult.ErrorText);

                        // Halt flow
                        return;
                    }

                    /* Create page */

                    // Advise user
                    Console.WriteLine("Creating page...");

                    try
                    {
                        // Set telegraph client
                        var client = new TelegraphClient();

                        // Set token client
                        ITokenClient tokenClient = client.GetTokenClient(createPageParser.Object.AccessToken);

                        // Set nodes
                        var nodes = new List<NodeElement>();
                        nodes.Add(
                            new NodeElement("p",
                                null,
                                createPageParser.Object.Content,
                                null
                            )
                        );

                        // Set content
                        NodeElement[] content = nodes.ToArray();

                        // Create page
                        Page page = await tokenClient.CreatePageAsync(
                          createPageParser.Object.Title,
                          content,
                          createPageParser.Object.AuthorName,
                          createPageParser.Object.AuthorUrl,
                          createPageParser.Object.ReturnContent
                        );

                        // Check if OK
                        if (page.Path.Length > 0)
                        {
                            // Set page data
                            string pageData =
                            $"path: {page.Path}{Environment.NewLine}" +
                            $"url: {page.Url}{Environment.NewLine}" +
                            $"title: {page.Title}{Environment.NewLine}" +
                            $"description: {page.Description}{Environment.NewLine}" +
                            $"author_name: {page.AuthorName}{Environment.NewLine}" +
                            $"author_url: {page.AuthorUrl}{Environment.NewLine}" +
                            $"image_url: {page.ImageUrl}{Environment.NewLine}" +
                            $"content: {page.Content.ToString()}{Environment.NewLine}" +
                            $"views: {page.Views}{Environment.NewLine}" +
                            $"can_edit: {page.CanEdit}{Environment.NewLine}";

                            // Set message
                            string createPageMessage = $"{Environment.NewLine}{Environment.NewLine}{DateTime.Now}{Environment.NewLine}{pageData}";

                            // Save page data to disk
                            File.AppendAllText("GingerTG-Success.txt", createPageMessage);

                            // Advise user
                            Console.WriteLine($"Page created.{pageData}");
                        }
                        else
                        {
                            // Throw exception with error message
                            throw new Exception("No page path returned.");
                        }
                    }
                    catch (Exception ex)
                    {
                        // Advise user
                        Console.WriteLine($"Create page error: {ex.Message}");

                        // Log to file
                        File.AppendAllText("GingerTG-ErrorLog.txt", $"{Environment.NewLine}{Environment.NewLine}{DateTime.Now}{Environment.NewLine}Create page error: { ex.Message}");

                        // Halt flow
                        return;
                    }

                    break;

                // Edit page
                case "editPage":

                    // TODO Edit page

                    break;

                default:

                    // Avise user
                    Console.WriteLine($"Error: {action} is not implemented.");

                    break;
            }
        }
    }
}
