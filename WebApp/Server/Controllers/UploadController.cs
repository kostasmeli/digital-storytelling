using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text;


namespace BlazorApp.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController(HttpClient httpClient, IConfiguration configuration, ILogger<UploadController> logger)
        : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UploadFileToRepository([FromBody] RequestBody request)
        {
            try
            {
                byte[] yarnFileBytes = Convert.FromBase64String(request.Base64TextFile);
                string yarnFileContents = Encoding.UTF8.GetString(yarnFileBytes);

                //Get Values from Config
                string sourceFolderPath = configuration.GetSection("AppSettings:SourceFolder").Value;
                string buildFolderPath = configuration.GetSection("AppSettings:BuildOutput").Value;
                string GitLabToken = configuration.GetSection("AppSettings:RepoToken").Value;

                // Set the GitLab API access token
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GitLabToken);

                //Replace the Yarn File in the folder.
                string yarnFilePath = Path.Combine(sourceFolderPath, "Assets", "DialogueYarn", "Dialog.yarn");


                System.IO.File.WriteAllText(yarnFilePath, yarnFileContents);



                //Now go and delete all the files inside the Resources and replace them with the new files
                string[] mp3Files = Directory.GetFiles($"{sourceFolderPath}\\Assets\\Resources", "*.mp3");
                foreach (string mp3File in mp3Files)
                {
                    System.IO.File.Delete(mp3File);
                }


                // Convert the base64-encoded ZIP file to a byte array
                byte[] zipBytes = Convert.FromBase64String(request.Base64ZipFile);

                // Extract the contents of the ZIP file to the folder
                using (MemoryStream memoryStream = new MemoryStream(zipBytes))
                {
                    using (ZipArchive zipArchive = new ZipArchive(memoryStream))
                    {
                        zipArchive.ExtractToDirectory($"{sourceFolderPath}\\Assets\\Resources");
                    }
                }

                var response = await RunUnityBuildCommand(sourceFolderPath,buildFolderPath);
                if (!(response == 0))
                {
                    throw new Exception("Could not build unity");
                    //Return Response, not exception
                }

                //Now that we created a  new build, we upload the contents in the according repository
                //Use the id of the project to update
                //No need to update database , since url remains the same
                var FinalResponse = await UpdateGitLabRepository(buildFolderPath, request.Url);
                if(FinalResponse != "OK")
                {
                    return Conflict();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while uploading the file.");
                return StatusCode(500, "An error occurred while uploading the file.");
            }
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


        public async Task<string> UpdateGitLabRepository(string folderPath,string RepoUrl)
        {
            var token = configuration.GetSection("AppSettings:RepoToken").Value;
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            // Create a new repository

            await UploadFile(folderPath, RepoUrl);

            return "OK";
        }
        public async Task UploadFile(string FolderPath, string projectUrl)
        {
            List<Actions> Action_List = ConvertFolderToObjects(FolderPath);
            //Before Uploading ,Create A Build Folder

            //System.IO.File.WriteAllText($"E:\\Temp\\{Guid.NewGuid()}.txt", json);

            string GitlabName = configuration.GetSection("AppSettings:GitLabName").Value;
            string FormatedProjectName = GitlabName +"%2F"+ projectUrl.Substring(projectUrl.LastIndexOf('/') + 1);
            

            var response = await httpClient.PostAsJsonAsync($"https://gitlab.com/api/v4/projects/{FormatedProjectName}/repository/commits", new
            {
                branch = "main",
                commit_message = $"Update-{DateTime.Now}",
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
                    action = "update",
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


    public class BodyData
    {
        public string branch { get; set; }

        public string commit_message { get; set; }
        public List<Actions> actions { get; set; }

    }

    public class Actions
    {
        public string action { get; set; }
        public string file_path { get; set; }
        public string content { get; set; }
        public string encoding { get; set; }
    }

    public class RequestBody
    {
        public string Base64TextFile { get; set; }
        public string Base64ZipFile { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
    }
}

