using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Renci.SshNet;
using System;
using static System.Net.WebRequestMethods;
using System.Diagnostics;

namespace Application
{
    public class SFTPPandamart : ISFTPPandamart
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        public SFTPPandamart(IConfiguration configuration,
            IServiceScopeFactory serviceScopeFactory) 
        {
            _configuration = configuration;
            _scopeFactory = serviceScopeFactory;
        }

        public async Task SendtoPAndamartsftp(string localFilePath = "")
        {
            IServiceScope scope = _scopeFactory.CreateScope();
            IServiceProvider serviceProvider = scope.ServiceProvider;
            var logger = serviceProvider.GetRequiredService<ILogger<SFTPPandamart>>();

            try
            {
                var sftpConnection = _configuration.GetSection("SFTPconnection").Get<SFTPConnection>();

                string host = sftpConnection.SFTPinfo.Host;
                int port = int.Parse(sftpConnection.SFTPinfo.Port);
                string username = sftpConnection.Credential.Username;
                string password = sftpConnection.Credential.Password;
                string remoteDirectory = sftpConnection.Directory.RemoteDirectory;

                //Create client Object
                using (SftpClient sftpClient = new SftpClient(host, port, username, password))
                {
                    //Connect to server"
                    sftpClient.Connect();

                    //Creating FileStream object to stream a file
                    sftpClient.ChangeDirectory(remoteDirectory);
                    using (FileStream fs = new FileStream(localFilePath, FileMode.Open))
                    {
                        //sftpClient.BufferSize = 1024;
                        sftpClient.UploadFile(fs, Path.GetFileName(localFilePath));
                    }

                    ////Creating FileStream object to stream a file
                    //using (SftpClient client = new SftpClient(host, port, username, password))
                    //{
                    //    client.Connect();
                    //    client.ChangeDirectory(remoteDirectory);
                    //    using (FileStream fs = new FileStream(localFilePath, FileMode.Open))
                    //    {
                    //        //client.BufferSize = 1024;
                    //        client.UploadFile(fs, Path.GetFileName(localFilePath));
                    //    }
                    //}

                    sftpClient.Dispose();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(
                    exception: ex,
                    message: "A fatal error occurred ({exceptionType}).",
                    ex.GetType().Name);

            }
        }
        public async Task MoveToNewFileLocation()
        {
            IServiceScope scope = _scopeFactory.CreateScope();
            IServiceProvider serviceProvider = scope.ServiceProvider;
            var logger = serviceProvider.GetRequiredService<ILogger<SFTPPandamart>>();

            try
            {
                //get the data on appsettings.json
                var sftpConnection = _configuration.GetSection("SFTPconnection").Get<SFTPConnection>();

                string host = sftpConnection.SFTPinfo.Host;
                int port = int.Parse(sftpConnection.SFTPinfo.Port);
                string username = sftpConnection.Credential.Username;
                string password = sftpConnection.Credential.Password;
                string remoteDirectory = sftpConnection.Directory.RemoteDirectory;

                DateTime dateToday = DateTime.Now;
                string newDirectory = String.Format("{0}/{1}", remoteDirectory, dateToday.ToString("yyyyMMdd"));

                using (SftpClient sftpClient = new SftpClient(host, port, username, password))
                {
                    //Connect to server"
                    sftpClient.Connect();
                    bool dirExists;

                    //check new Directory if existing
                    dirExists = sftpClient.Exists(newDirectory);
                    if (dirExists == false)
                    {
                        sftpClient.CreateDirectory(newDirectory);     //Creater New Directory

                        //Remove all oldffiles
                        var oldffiles = sftpClient.ListDirectory(remoteDirectory);
                        foreach (var file in oldffiles)
                        {
                            if (file.IsRegularFile)
                            {
                                sftpClient.Delete(String.Format("{0}", file.FullName)); //delete oldffiles in remoteDirectory
                            }
                        }

                    }
                    else
                    {
                        //Remove all file in newDirectory
                        var Existingfiles = sftpClient.ListDirectory(newDirectory);
                        foreach (var file in Existingfiles)
                        {
                            if (file.IsRegularFile)
                            {
                                sftpClient.Delete(String.Format("{0}", file.FullName)); //delete Existingfiles in newDirectory
                            }
                        }

                        var files = sftpClient.ListDirectory(remoteDirectory);  //get the file in existing folder
                        foreach (var file in files)
                        {
                            if (file.IsRegularFile)
                            {
                                sftpClient.RenameFile(file.FullName, String.Format("{0}/{1}", newDirectory, file.Name));//move to new directory
                            }
                        }
                    }

                    //Dispose connection
                    sftpClient.Dispose();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(
                    exception: ex,
                    message: "A fatal error occurred ({exceptionType}).",
                    ex.GetType().Name);

            }
        }
    }
}
