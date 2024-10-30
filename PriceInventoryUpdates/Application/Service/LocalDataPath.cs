using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class LocalDataPath : ILocalDataPath
    {
        public LocalDataPath()
        {
        }
        public async Task CreateBackUpFile(string LocalPath = "") 
        {
            DirectoryInfo dir = new DirectoryInfo(LocalPath);
            DateTime dateToday = DateTime.Now;
            string BackupPath = string.Format("{0}/{1}", LocalPath, dateToday.ToString("yyyyMMdd"));

            //check if backup folder is existing
            bool exists = Directory.Exists(BackupPath);
            if (!exists)
                Directory.CreateDirectory(BackupPath);  //create new backup folder

            foreach (FileInfo file in dir.GetFiles())
            {
                try
                {
                    //file.Delete(); //delete each file
                    file.MoveTo(string.Format("{0}/{1}", BackupPath, file.Name), true); //move file to backup folder
                }
                catch (Exception ex) { } // Ignore all exceptions
            }

        }
    }
}
