using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace RepoToText
{
    internal class GithubScrapper
    {
        // Fetch contents recursively from a repository
        internal async Task<IReadOnlyList<RepositoryContent>> GetRepositoryContentsRecursively(GitHubClient client, string owner, string repoName, string path)
        {
            var contents = new List<RepositoryContent>();

            try
            {
                // Fetch the contents of the current directory or subdirectory
                IReadOnlyList<RepositoryContent> repoContents;
                if (string.IsNullOrEmpty(path))
                {
                    repoContents = await client.Repository.Content.GetAllContents(owner, repoName);
                }
                else
                {
                    repoContents = await client.Repository.Content.GetAllContents(owner, repoName, path);
                }


                Console.WriteLine($"Fetching contents from path: {path}");
                Console.WriteLine($"Found {repoContents.Count} items in path {path}");

                foreach (var content in repoContents)
                {
                    if (content.Type == Octokit.ContentType.Dir)
                    {
                        // If the content is a directory, recurse into the directory to fetch its contents
                        var subContents = await GetRepositoryContentsRecursively(client, owner, repoName, content.Path);
                        contents.AddRange(subContents); // Add the subcontent (i.e., contents of subdirectory)
                    }
                    else if (IsCodeFile(content.Path)) // Ensure the file is a valid code file
                    {
                        contents.Add(content);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching contents from {path}: {ex.Message}");
            }

            return contents;
        }

        // Check if the file is a valid code file (based on file extension)
        private bool IsCodeFile(string filePath)
        {
            // List of human-readable code and markup file extensions
            string[] validExtensions = {
                ".cs",      // C#
                ".js",      // JavaScript
                ".java",    // Java
                ".py",      // Python
                ".html",    // HTML
                ".css",     // CSS
                ".cpp",     // C++
                ".h",       // C/C++ Header
                ".ts",      // TypeScript
                ".rb",      // Ruby
                ".go",      // Go
                ".json",    // JSON
                ".xml",     // XML
                ".yaml",    // YAML
                ".yml",     // YAML (alternative extension)
                ".md",      // Markdown
                ".txt",     // Plain text
                ".sql",     // SQL scripts
                ".sh",      // Shell scripts
                ".bat",     // Batch scripts
                ".ini",     // Configuration files
                ".env",     // Environment configuration
                ".php",     // PHP
                ".dart",    // Dart
                ".pl",      // Perl
                ".r",       // R
                ".swift",   // Swift
                ".scala",   // Scala
                ".kt",      // Kotlin
                ".rs",      // Rust
                ".erl",     // Erlang
                ".ex",      // Elixir
                ".exs",     // Elixir scripts
                ".cfg",     // Configuration files
                ".log",     // Log files
                ".gradle",  // Gradle build scripts
                ".properties", // Java properties files
                ".makefile",   // Makefiles
                ".dockerfile", // Dockerfiles
                ".tf",         // Terraform files
                ".toml",       // TOML configuration
                ".jsx",        // React JSX
                ".tsx",        // React TSX
                ".vue",        // Vue.js components
                ".vbs",        // VBScript
                ".asp",        // Classic ASP
                ".jsp",        // JavaServer Pages
                ".ejs",        // Embedded JavaScript templates
                ".handlebars", // Handlebars templates
                ".pug",        // Pug templates (formerly Jade)
            };


            // Check if the file extension matches one of the valid code extensions
            string extension = Path.GetExtension(filePath).ToLower();
            return Array.Exists(validExtensions, ext => ext.Equals(extension));
        }
    }
}
