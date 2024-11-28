using Octokit;
using RepoToText;
using System.Text;

class Program
{
    private static async Task Main(string[] args)
    {
        // Taking user input for repository owner, repository name, and output file path
        Console.Write("Enter your GitHub repository token (You can create one from your github account developer setting): ");
        string token = Console.ReadLine();

        Console.Write("Enter the GitHub repository owner (e.g., 'RefatMustaque'): ");
        string owner = Console.ReadLine();

        Console.Write("Enter the GitHub repository name (e.g., 'AIUB-Student-Help-Desk'): ");
        string repoName = Console.ReadLine();

        Console.Write("Enter the output file path (e.g., 'C:\\Users\\Public\\Documents\\GithubScrappedOutput.txt'): ");
        string filePath = Console.ReadLine();

        // GitHub Client Initialization
        var client = new GitHubClient(new ProductHeaderValue("GitHubToText"));
        var tokenAuth = new Credentials(token); // Use your GitHub Token for private repos (omit for public repos)
        client.Credentials = tokenAuth;

        GithubScrapper githubScrapper = new GithubScrapper();
        // Fetch the repository contents recursively
        var contents = await githubScrapper.GetRepositoryContentsRecursively(client, owner, repoName, null);

        Console.WriteLine($"Found {contents.Count} files in the repository.");

        // Process files and write to output.txt
        using (var writer = new StreamWriter(filePath))
        {
            foreach (var content in contents)
            {
                // Only handle files, not directories
                if (content.Type == ContentType.File)
                {
                    try
                    {
                        // Debug: Output file path
                        Console.WriteLine($"Fetching file: {content.Path}");

                        byte[] fileContentBytes = await client.Repository.Content.GetRawContent(owner, repoName, content.Path);
                        string fileContent = Encoding.UTF8.GetString(fileContentBytes); // Convert byte[] to string

                        // Write to output file with clear separation
                        writer.WriteLine($"--- File: {content.Path} ---");
                        writer.WriteLine(fileContent);
                        writer.WriteLine("\n\n");
                    }
                    catch (Exception ex)
                    {
                        // Handle cases where file content cannot be fetched (e.g., binary files)
                        writer.WriteLine($"--- File: {content.Path} ---");
                        writer.WriteLine($"Error fetching file content: {ex.Message}");
                        writer.WriteLine("\n\n");
                    }
                }
            }
        }

        Console.WriteLine($"Repository content has been written to {filePath}");
    }
}