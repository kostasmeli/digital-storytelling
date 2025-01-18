using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;


namespace BlazorApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreateDialogueController(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<UploadController> logger,
        DataContext context)
        : ControllerBase
    {

        public class CreateDialogueRequest
        {
            public string Title { get; set; }
            public string base64YarnFile { get; set; }
            public string base64ZipFile { get; set; }
        }

        private readonly ILogger<UploadController> _logger = logger;

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateNewDialogue([FromBody] CreateDialogueRequest req)
        {
            try
            {

                //Decode the base64 string to get the .yarn file contents
                byte[] yarnFileBytes = Convert.FromBase64String(req.base64YarnFile);
                string yarnFileContents = Encoding.UTF8.GetString(yarnFileBytes);

                //Get Values from Config
                string sourceFolderPath = configuration.GetSection("AppSettings:SourceFolder").Value;
                string buildFolderPath = configuration.GetSection("AppSettings:BuildOutput").Value;
                string GitLabToken = configuration.GetSection("AppSettings:RepoToken").Value;


                string yarnFilePath = Path.Combine(sourceFolderPath, "Assets", "DialogueYarn", "Dialog.yarn");

                System.IO.File.WriteAllText(yarnFilePath, yarnFileContents);


                //Now go and delete all the files inside the Resources and replace them with the new files
                string[] mp3Files = Directory.GetFiles($"{sourceFolderPath}\\Assets\\Resources", "*.mp3");
                foreach (string mp3File in mp3Files)
                {
                    System.IO.File.Delete(mp3File);
                }

                // Convert the base64-encoded ZIP file to a byte array
                byte[] zipBytes = Convert.FromBase64String(req.base64ZipFile);

                // Extract the contents of the ZIP file to the folder
                using (MemoryStream memoryStream = new MemoryStream(zipBytes))
                {
                    using (ZipArchive zipArchive = new ZipArchive(memoryStream))
                    {
                        zipArchive.ExtractToDirectory($"{sourceFolderPath}\\Assets\\Resources");
                    }
                }


                var response = await RunUnityBuildCommand(sourceFolderPath, buildFolderPath);
                if (response != 0)
                {
                    throw new Exception("Could not Build Unity");
                }

                

                string deploymentIdentifier = DateTime.Now.ToString("ddMMyyyyHHmm");

                var publishedUrl = await CreateGitLabRepositoryAndUploadFiles(buildFolderPath, $"Blazor_{deploymentIdentifier}", "TestingIntegration");

                //Create a record on db
                var FinalResponse = await CreateRecordOnDb(req.Title, publishedUrl);

                if (FinalResponse == "Created")
                {
                    return Ok();
                }
                else
                {
                    return Conflict(FinalResponse);
                }
                
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occurred during the process
                return StatusCode(500, ex.Message);
            }
        }

       

        private async Task<string> CreateRecordOnDb(string Title,string url)
        {
            DialogueObject dialogueObject = new DialogueObject();
            dialogueObject.Title = Title;
            dialogueObject.url = url;
            context.DialogueObjects.Add(dialogueObject);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DialogueObjectExists(dialogueObject.Title))
                {
                    return "Title Already Exists";
                }
                else
                {
                    throw;
                }
            }
            return "Created";
        }
        private bool DialogueObjectExists(string id)
        {
            return (context.DialogueObjects?.Any(e => e.Title == id)).GetValueOrDefault();
        }

        private Task<int> RunUnityBuildCommand(string projectPath, string outputFolder)
        {

            string unity_log = configuration.GetSection("AppSettings:UnityLog").Value;
            string unity_exe_path = configuration.GetSection("AppSettings:UnityExePath").Value;
            string log_txt = $"{unity_log}\\LogUnity_{Guid.NewGuid().ToString()}.txt";
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = unity_exe_path,
                Arguments = $"\"{outputFolder}\" -projectPath \"{projectPath}\" -quit -batchmode -executeMethod UnityBuilder.PerformBuild -logFile \"{log_txt}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using (Process process = new Process())
            {
                process.StartInfo = startInfo;

                StringBuilder outputBuilder = new StringBuilder();

                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        outputBuilder.AppendLine(e.Data);
                        Console.WriteLine(e.Data);
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();

                string output = outputBuilder.ToString();
                int exitCode = process.ExitCode;

                return Task.FromResult(exitCode);
            }
        }


        public async Task<string> CreateGitLabRepositoryAndUploadFiles(string folderPath,string RepoName, string RepoDesc)
        {
            var token = configuration.GetSection("AppSettings:RepoToken").Value;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            // Create a new repository
            var repositoryID = await CreateRepository(RepoName, RepoDesc);

            await UploadFile(folderPath,repositoryID);

            return $"https://km13km13km13.gitlab.io/{RepoName}";
        }



        public async Task<string> CreateRepository(string repositoryName, string repositoryDescription)
        {
            
            var requestBody = new { name = repositoryName, description = repositoryDescription, visibility = "public" };
            var response = await httpClient.PostAsJsonAsync("https://gitlab.com/api/v4/projects", requestBody);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            dynamic responseData = JsonConvert.DeserializeObject(responseContent);
            var repositoryUrl = responseData.id;
            return repositoryUrl;
        }

        public async Task UploadFile(string FolderPath, string projectId)
        {
            List<Actions> Action_List = ConvertFolderToObjects(FolderPath);
            //Before Uploading ,Create A Build Folder
            var request_payload = new BodyData();
            request_payload.branch = "main";
            request_payload.commit_message = "Test Batch Integration";
            request_payload.actions = Action_List;

            var json = System.Text.Json.JsonSerializer.Serialize(request_payload);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            //System.IO.File.WriteAllText($"E:\\Temp\\{Guid.NewGuid()}.txt", json);

            var response = await httpClient.PostAsJsonAsync($"https://gitlab.com/api/v4/projects/{projectId}/repository/commits", new {    
                branch = "main",
                commit_message = "Test Integration",
                actions = Action_List
            });
            response.EnsureSuccessStatusCode();
        }

        private static List<Actions> ConvertFolderToObjects(string folderPath)
        {
            List<Actions> fileObjects = new List<Actions>();
            ConvertFilesToObjects(folderPath, "", fileObjects);
            return fileObjects;
        }

        private static void ConvertFilesToObjects(string folderPath, string currentPath, List<Actions> fileObjects)
        {
            string[] files = Directory.GetFiles(folderPath);
            foreach (string file in files)
            {
                string filePath = Path.Combine(currentPath, Path.GetFileName(file));
                byte[] fileBytes = System.IO.File.ReadAllBytes(file);
                string base64String = Convert.ToBase64String(fileBytes);

                var fileObject = new Actions
                {
                    action = "create",
                    file_path = filePath.Replace("\\", "/"),
                    content = base64String,
                    encoding = "base64"
                };

                fileObjects.Add(fileObject);
            }

            string[] subDirectories = Directory.GetDirectories(folderPath);
            foreach (string subDirectory in subDirectories)
            {
                string subDirectoryName = Path.GetFileName(subDirectory);
                string subDirectoryPath = Path.Combine(currentPath, subDirectoryName);
                ConvertFilesToObjects(subDirectory, subDirectoryPath, fileObjects);
            }
        }
    }

}

