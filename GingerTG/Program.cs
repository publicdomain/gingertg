// <copyright file="MainForm.cs" company="PublicDomain.is">
//     CC0 1.0 Universal (CC0 1.0) - Public Domain Dedication
//     https://creativecommons.org/publicdomain/zero/1.0/legalcode
// </copyright>

using System;
using System.Reflection;
using Fclp;

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

                    // TODO Create account

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
