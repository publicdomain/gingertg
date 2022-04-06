// <copyright file="MainForm.cs" company="PublicDomain.is">
//     CC0 1.0 Universal (CC0 1.0) - Public Domain Dedication
//     https://creativecommons.org/publicdomain/zero/1.0/legalcode
// </copyright>

using System;
using System.IO;
using System.Reflection;
using Fclp;
using Telegraph.Net;

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
             .Callback(action => ProcessAction(action, args))
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
        private static void ProcessAction(string action, string[] args)
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

                    // TODO Create page

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
