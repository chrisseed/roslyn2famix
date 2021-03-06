﻿using FAMIX;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Model;
using RoslynMonoFamix.VB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Roslyn2Famix
{
    class MainClass
    {

        public static void Main(params string[] args)
        {

            string solutionPath = args[0]; 
            string msePath = args[1];
            ExportToMSE(solutionPath, msePath);
            /* var rootCommand = new RootCommand(
                 description: "Outputs a mse file from a given .NET Solution");

             rootCommand.Add(new Option<string>(
                  aliases: new string[] { "--input", "-i" },
                  description: "The path to the .NET Solution file"));
             rootCommand.Add(new Option<string>(
                 aliases: new string[] { "--output", "-o" },
                 description: "The path to the .mse file"));

             rootCommand.Handler = CommandHandler.Create<string, string>(ExportToMSE);
             rootCommand.InvokeAsync(args).Wait();*/
        }

        private static void ExportToMSE(string input, string output)
        {
            try
            {
                string path = Assembly.GetAssembly(typeof(MainClass)).Location;
                Console.WriteLine("Current executable location " + path);
                path = path.Replace("RoslynMonoFamix.exe", "");

                var metamodel = FamixModel.Metamodel();

                var msWorkspace = MSBuildWorkspace.Create();

                var solution = msWorkspace.OpenSolutionAsync(input).Result;
                Uri uri = null;
                try
                {
                    uri = new Uri(input); ;
                }
                catch (UriFormatException e)
                {
                    var currentFolder = new Uri(Environment.CurrentDirectory + "\\");
                    uri = new Uri(currentFolder, input.Replace("\\", "/"));
                    Console.WriteLine(e.StackTrace);
                }

                var ignoreFolder = Path.GetDirectoryName(uri.AbsolutePath);
                var importer = new InCSharp.InCSharpImporter(metamodel, ignoreFolder);
                var documents = new List<Document>();
                Console.WriteLine("Solution with id " + solution.Id.Id + " opened and contains " + solution.Projects.Count<Project>() + " projects.");

                var diagnostics = msWorkspace.Diagnostics;
                foreach (var diagnostic in diagnostics)
                {
                    Console.WriteLine(diagnostic.Message);
                }

                for (int i = 0; i < solution.Projects.Count<Project>(); i++)
                {
                    var project = solution.Projects.ElementAt<Project>(i);

                    System.Console.WriteLine("(project " + (i + 1) + " / " + solution.Projects.Count<Project>() + ") contains " + project.Documents.Count<Document>() + " documents.");

                    var projectCompilation = project.GetCompilationAsync().Result;
                    for (int j = 0; j < project.Documents.Count<Document>(); j++)
                    {
                        var document = project.Documents.ElementAt<Document>(j);
                        if (document.SupportsSyntaxTree)
                        {
                            System.Console.Write("(project " + (i + 1) + " / " + solution.Projects.Count<Project>() + ")");
                            System.Console.WriteLine("(document " + (j + 1) + " / " + project.Documents.Count<Document>() + " " + document.FilePath + ")");
                            var syntaxTree = document.GetSyntaxTreeAsync().Result;

                            var compilationAsync = project.GetCompilationAsync().Result;
                            var semanticModel = compilationAsync.GetSemanticModel(syntaxTree);

                            semanticModel.ToString();

                            if (semanticModel.Language == "C#")
                            {
                                var visitor = new ASTVisitor(semanticModel, importer);
                                visitor.Visit(syntaxTree.GetRoot());
                            }
                            else
                            {
                                var visitor = new VBASTVisitor(semanticModel, importer);
                                visitor.Visit(syntaxTree.GetRoot());
                            }
                        }
                    }
                }
                metamodel.ExportMSEFile(output);
            }
            catch (ReflectionTypeLoadException ex)
            {
                StringBuilder sb = new StringBuilder();
                foreach (System.Exception exSub in ex.LoaderExceptions)
                {
                    sb.AppendLine(exSub.Message);
                    FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                    if (exFileNotFound != null)
                    {
                        if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                        {
                            sb.AppendLine("Fusion Log:");
                            sb.AppendLine(exFileNotFound.FusionLog);
                        }
                    }
                    sb.AppendLine();
                }
                string errorMessage = sb.ToString();
                Console.WriteLine(errorMessage);
                //Display or log the error based on your application.
            }
        }
    }
}
