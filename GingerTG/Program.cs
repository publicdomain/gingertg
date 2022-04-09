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
                        var account = client.CreateAccountAsync(createAccountParser.Object.ShortName, createAccountParser.Object.AuthorName, createAccountParser.Object.AuthorUrl).Result;

                        // Check if OK
                        if (account.AccessToken.Length > 0)
                        {
                            // Set account data
                            string accountData =
                            $"short_name: {account.ShortName}{Environment.NewLine}" +
                            $"author_name: {account.AuthorName}{Environment.NewLine}" +
                            $"author_url: {account.AuthorUrl}{Environment.NewLine}" +
                            $"access_token: {account.AccessToken}{Environment.NewLine}" +
                            $"auth_url: {account.AuthorizationUrl}";

                            // Set message
                            string createAccountMessage = $"{Environment.NewLine}{Environment.NewLine}Create account / {DateTime.Now}{Environment.NewLine}{accountData}";

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
                        var createPageClient = new TelegraphClient();

                        // Set token client
                        ITokenClient tokenClient = createPageClient.GetTokenClient(createPageParser.Object.AccessToken);

                        // Create page
                        Page createdPage = tokenClient.CreatePageAsync(
                            createPageParser.Object.Title,
                            new List<NodeElement> {
                                new NodeElement("p", null, createPageParser.Object.Content)
                            }.ToArray(),
                            createPageParser.Object.AuthorName,
                            createPageParser.Object.AuthorUrl,
                            createPageParser.Object.ReturnContent
                        ).Result;

                        // Check if OK
                        if (createdPage.Path.Length > 0)
                        {
                            // Set created page data
                            string createdPageData =
                            $"path: {createdPage.Path}{Environment.NewLine}" +
                            $"url: {createdPage.Url}{Environment.NewLine}" +
                            $"title: {createdPage.Title}{Environment.NewLine}" +
                            $"description: {createdPage.Description}{Environment.NewLine}" +
                            $"author_name: {createdPage.AuthorName}{Environment.NewLine}" +
                            $"author_url: {createdPage.AuthorUrl}{Environment.NewLine}" +
                            $"image_url: {createdPage.ImageUrl}{Environment.NewLine}" +
                            $"content: {createdPage.Content}{Environment.NewLine}" +
                            $"views: {createdPage.Views}{Environment.NewLine}" +
                            $"can_edit: {createdPage.CanEdit}{Environment.NewLine}";

                            // Set message
                            string createPageMessage = $"{Environment.NewLine}{Environment.NewLine}createPage / {DateTime.Now}{Environment.NewLine}{createdPageData}";

                            // Save page data to disk
                            File.AppendAllText("GingerTG-Success.txt", createPageMessage);

                            // Advise user
                            Console.WriteLine($"Page created.{createdPageData}");
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

                    /* Parse arguments */

                    // Fluent
                    var editPageParser = new FluentCommandLineParser<EditPageArguments>();

                    // short_name
                    editPageParser.Setup(arg => arg.ShortName)
                        .As('s', "short-name");

                    // access_token
                    editPageParser.Setup(arg => arg.AccessToken)
                        .As('k', "access-token")
                        .Required();

                    // Path
                    editPageParser.Setup(arg => arg.Path)
                        .As('p', "path")
                        .Required();

                    // title
                    editPageParser.Setup(arg => arg.Title)
                        .As('t', "title")
                        .Required();

                    // content
                    editPageParser.Setup(arg => arg.Content)
                        .As('c', "content")
                        .Required();

                    // author_name
                    editPageParser.Setup(arg => arg.AuthorName)
                        .As('n', "author-name");

                    // author_url
                    editPageParser.Setup(arg => arg.AuthorUrl)
                        .As('u', "author-url");

                    // return_content
                    editPageParser.Setup(arg => arg.ReturnContent)
                        .As('r', "return-content");

                    // Parse
                    var editPageParserResult = editPageParser.Parse(args);

                    // Check for errors
                    if (editPageParserResult.HasErrors)
                    {
                        // Advise user
                        Console.WriteLine(editPageParserResult.ErrorText);

                        // Halt flow
                        return;
                    }

                    /* Edit page */

                    // Advise user
                    Console.WriteLine("Editing page...");

                    try
                    {
                        // Set telegraph client
                        var editPageClient = new TelegraphClient();

                        // Set token client
                        ITokenClient tokenClient = editPageClient.GetTokenClient(editPageParser.Object.AccessToken);

                        // Edit page
                        Page editedPage = tokenClient.EditPageAsync(
                            editPageParser.Object.Path,
                            editPageParser.Object.Title,
                            new List<NodeElement> {
                                    new NodeElement("p", null, editPageParser.Object.Content)
                            }.ToArray(),
                            editPageParser.Object.AuthorName,
                            editPageParser.Object.AuthorUrl,
                            editPageParser.Object.ReturnContent
                        ).Result;

                        // Check if OK
                        if (editedPage.Path.Length > 0)
                        {
                            // Set page data
                            string editedPageData =
                            $"path: {editedPage.Path}{Environment.NewLine}" +
                            $"url: {editedPage.Url}{Environment.NewLine}" +
                            $"title: {editedPage.Title}{Environment.NewLine}" +
                            $"description: {editedPage.Description}{Environment.NewLine}" +
                            $"author_name: {editedPage.AuthorName}{Environment.NewLine}" +
                            $"author_url: {editedPage.AuthorUrl}{Environment.NewLine}" +
                            $"image_url: {editedPage.ImageUrl}{Environment.NewLine}" +
                            $"content: {editedPage.Content}{Environment.NewLine}" +
                            $"views: {editedPage.Views}{Environment.NewLine}" +
                            $"can_edit: {editedPage.CanEdit}{Environment.NewLine}";

                            // Set message
                            string editPageMessage = $"{Environment.NewLine}{Environment.NewLine}editPage / {DateTime.Now}{Environment.NewLine}{editedPageData}";

                            // Save page data to disk
                            File.AppendAllText("GingerTG-Success.txt", editPageMessage);

                            // Advise user
                            Console.WriteLine($"Page edited.{editedPageData}");
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
                        Console.WriteLine($"Edit page error: {ex.Message}");

                        // Log to file
                        File.AppendAllText("GingerTG-ErrorLog.txt", $"{Environment.NewLine}{Environment.NewLine}{DateTime.Now}{Environment.NewLine}Edit page error: { ex.Message}");

                        // Halt flow
                        return;
                    }

                    break;

                // Get views
                case "getViews":

                    /* Parse arguments */

                    // Fluent
                    var getViewsParser = new FluentCommandLineParser<GetViewsArguments>();

                    // short_name
                    getViewsParser.Setup(arg => arg.ShortName)
                        .As('s', "short-name");

                    // Path
                    getViewsParser.Setup(arg => arg.Path)
                        .As('p', "path")
                        .Required();

                    /*// Year
                    getViewsParser.Setup(arg => arg.Year)
                       .As('y', "year");

                    // Month
                    getViewsParser.Setup(arg => arg.Month)
                       .As('m', "month");

                    // Day
                    getViewsParser.Setup(arg => arg.Day)
                       .As('d', "day");

                    // Hour
                    getViewsParser.Setup(arg => arg.Hour)
                       .As('h', "hour");*/

                    // Parse
                    var getViewsParserResult = getViewsParser.Parse(args);

                    // Check for errors
                    if (getViewsParserResult.HasErrors)
                    {
                        // Advise user
                        Console.WriteLine(getViewsParserResult.ErrorText);

                        // Halt flow
                        return;
                    }

                    /* Page views */

                    // Advise user
                    Console.WriteLine("Getting page views...");

                    try
                    {
                        // Set telegraph client
                        var getViewsClient = new TelegraphClient();

                        // Get pageViews object
                        var pageViews = getViewsClient.GetViewsAsync(getViewsParser.Object.Path, getViewsParser.Object.Year, getViewsParser.Object.Month, getViewsParser.Object.Day, getViewsParser.Object.Hour).Result;

                        // Get actual views integer
                        int views = pageViews.Views;

                        // Set views data
                        string viewsData =
                        $"views: {views}";

                        // Set message
                        string pageViewsMessage = $"{Environment.NewLine}{Environment.NewLine}Get views / {DateTime.Now}{Environment.NewLine}{viewsData}";

                        // Save account data to disk
                        File.AppendAllText("GingerTG-success.txt", pageViewsMessage);

                        // Advise user
                        Console.WriteLine($"Page views fetched.{pageViewsMessage}");
                    }
                    catch (Exception ex)
                    {
                        // Advise user
                        Console.WriteLine($"Get views error: {ex.Message}");

                        // Log to file
                        File.AppendAllText("GingerTG-ErrorLog.txt", $"{Environment.NewLine}{Environment.NewLine}{DateTime.Now}{Environment.NewLine}Get views error: { ex.Message}");

                        // Halt flow
                        return;
                    }

                    break;

                default:

                    // Avise user
                    Console.WriteLine($"Error: {action} is not implemented.");

                    break;
            }
        }
    }
}
