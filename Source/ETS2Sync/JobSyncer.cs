using log4net;
using Microsoft.Win32;
using Palow.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace ETS2Sync
{
    public class JobSyncer
    {
        public String SaveFolderPath { get; set; }
        public String SaveFolderPathGameSiiZero { get { return Path.Combine(this.SaveFolderPath, "game.sii.0"); } }
        public String SaveFolderPathGameSii
        {
            get
            {
                try
                {
                    return Path.Combine(this.SaveFolderPath, "game.sii");
                }
                catch (Exception e)
                {
                    //C:\Users\janni\Documents\Euro Truck Simulator 2\profiles\5B626C61636B44445D2064616E7A61\save\8
                    return "";
                }

            }
        }

        public Boolean DlcScandinavia { get; set; }
        public Boolean DlcGoingEast { get; set; }
        public Boolean DlcHighPowerCargoes { get; set; }


        public String Ets2Folder { get; set; }

        public FileSystemWatcher FileSystemWatcher { get; set; }

        public String LastMd5 { get; set; }

        private Boolean syncInProgress = false;

        public JobSyncer()
        {
            //C: \Users\janni\Documents\Euro Truck Simulator 2\profiles\5B626C61636B44445D2064616E7A61\save\8
            //this.SaveFolderPath = @"C:\Users\janni\Documents\Euro Truck Simulator 2\profiles\5B626C61636B44445D2064616E7A61\save\8";

            try
            {
                String installPath = (String)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", null);
                if (installPath != null)
                {
                    this.Ets2Folder = Path.Combine(installPath, @"steamapps\common\Euro Truck Simulator 2");


                    if (File.Exists(Path.Combine(this.Ets2Folder, "dlc_east.scs")))
                    {
                        this.DlcGoingEast = true;
                    }


                    if (File.Exists(Path.Combine(this.Ets2Folder, "dlc_north.scs")))
                    {
                        this.DlcScandinavia = true;
                    }

                    if (File.Exists(Path.Combine(this.Ets2Folder, "dlc_trailers.scs")))
                    {
                        this.DlcHighPowerCargoes = true;
                    }

                    // http://www.ets2sync.com/




                    // Do stuff
                }
            }
            catch (Exception e)
            {

            }

            String docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            String etsSettingsPath = Path.Combine(docPath, "Euro Truck Simulator 2");


            String etsSettingsFile = Path.Combine(etsSettingsPath, "config.cfg");




            try
            {
                String content = Functions.ReadFile(etsSettingsFile);
                //uset g_save_format "0"
                content = content.Replace("uset g_save_format \"0\"", "uset g_save_format \"3\"");

                Functions.WriteFile(etsSettingsFile, content);

            }
            catch (Exception e)
            {
            }

            try
            {
                String tmp = Path.Combine(etsSettingsPath, "profiles");
                if (Directory.Exists(tmp))
                {
                    this.SaveFolderPath = tmp;
                }
            }
            catch (Exception e)
            {

            }

        }

        public void SyncJobs()
        {
            SyncJobs(this.SaveFolderPathGameSiiZero);
        }



        public void SyncJobs(String fileName)
        {
            if (syncInProgress)
            {
                return;
            }
            syncInProgress = true;


            ILog lg = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
            lg.Debug("Start syncing");
            try
            {
                String folderPathGameSiiZero = fileName;
                String folderPathSave = Path.GetDirectoryName(fileName);
                String folderPathGameSii = Path.Combine(folderPathSave, "game.sii");


                try
                {
                    if (Rn.Config.General.PlaySound)
                    {
                        SoundPlayer sp = new SoundPlayer(@"sound\sound_send.wav");
                        sp.Play();
                    }


                    if (Rn.Config.General.BackupSaveGame)
                    {
                        File.Move(folderPathGameSii, folderPathGameSii + "." + DateTime.Now.ToFileTime() + ".bak");
                    }

                }
                catch (Exception e)
                {
                }


                // Get PHP Session ID
                HTTPRequest http = new HTTPRequest();
                http.BaseUrl = "http://www.ets2sync.com/de/";
                http.Request();


                http.CreateFullUrl("file_upload.php?uploadfile=game.sii.0");
                http.UploadFile(folderPathGameSiiZero, "uploadfile", "application/octet-stream");

                Response.UploadFile resp = fastJSON.JSON.ToObject<Response.UploadFile>(http.Response);

                List<String> dlcs = new List<String>();
                if (this.DlcGoingEast)
                {
                    dlcs.Add("east");
                }
                if (this.DlcHighPowerCargoes)
                {
                    dlcs.Add("hpower");
                }
                if (this.DlcScandinavia)
                {
                    dlcs.Add("north");
                }

                String dlcPost = String.Join(",", dlcs);

                http.CreateFullUrl("engine.php");
                http.Post = "dlcs=" + dlcPost + "&f_path_orig=" + resp.file_path + "&f_path_synced=" + resp.f_path_synced;
                http.Request();
                //http://www.ets2sync.com/de/downloader.php?file=synced

                http.CreateFullUrl("file_list.php");
                http.Post = "file=synced";
                http.Request();


                //http://www.ets2sync.com/de/downloader.php?file=synced
                http.CreateFullUrl("downloader.php?file=synced");
                http.DownloadFile(folderPathGameSii);


                try
                {
                    if (Rn.Config.General.PlaySound)
                    {
                        SoundPlayer sp = new SoundPlayer(@"sound\sound_receive.wav");
                        sp.Play();
                    }
                }
                catch (Exception e)
                {
                }
            }
            catch (Exception e)
            {
                if (Rn.Config.General.PlaySound)
                {
                    SoundPlayer sp = new SoundPlayer(@"sound\sound_error.wav");
                    sp.Play();
                }
            }


            lg.Debug("End syncing");
            syncInProgress = false;


        }


        public void StartFileWatcher()
        {
            if (!String.IsNullOrEmpty(this.SaveFolderPath) && System.IO.Directory.Exists(this.SaveFolderPath))
            {
                if (this.FileSystemWatcher != null)
                {
                    this.FileSystemWatcher.EnableRaisingEvents = false;
                }



                this.FileSystemWatcher = new FileSystemWatcher();
                this.FileSystemWatcher.Path = this.SaveFolderPath;
                this.FileSystemWatcher.IncludeSubdirectories = true;
                this.FileSystemWatcher.Filter = "*";


                this.FileSystemWatcher.Changed += new FileSystemEventHandler(FileWatcherOnChangedCreate);
                this.FileSystemWatcher.Created += new FileSystemEventHandler(FileWatcherOnChangedCreate);

                this.FileSystemWatcher.EnableRaisingEvents = true;
            }
        }

        private void FileWatcherOnChangedCreate(object sender, FileSystemEventArgs e)
        {
            ILog lg = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            try
            {
                #region *** PreCheck - Unlock, Double Process


                if (File.Exists(e.FullPath))
                {
                    #region *** Wait until FileUnLocked
                    FileInfo fInfo = new FileInfo(e.FullPath);
                    int unlockTries = 0;
                    while (FileWatcherIsFileLocked(fInfo))
                    {
                        Thread.Sleep(100);
                        unlockTries++;
                        if (unlockTries > 7)
                        {
                            //lg.Error(String.Format("No Unlock after {1} tries with 100ms Error for:{0}, Cancel Event", e.Name, unlockTries));
                            return;
                        }
                    }
                    #endregion
                    #region *** CheckSum // Detect Double Run

                    string md5 = this.FileWatcherMd5Hash(e.FullPath);
                    if (this.LastMd5 == md5)
                    {
                        //lg.Warn("Detected Double Run, cancel:" + e.Name);
                        return;
                    }
                    this.LastMd5 = md5;
                    #endregion
                }






                #endregion


                if (Path.GetFileName(e.FullPath) == "game.sii.0~" && !e.FullPath.Contains("autosave") && e.ChangeType == WatcherChangeTypes.Created) // (File.Exists(e.FullPath) && Path.GetFileName(e.FullPath) == "game.sii.0") ||
                {
                    lg.Debug("Filename: " + e.FullPath + " " + e.ChangeType);

                    try
                    {
                        this.SyncJobs(e.FullPath.Replace("~", ""));
                    }
                    catch (Exception exc)
                    {
                    }

                }
            }
            catch (Exception exce)
            {
            }


        }


        public bool FileWatcherIsFileLocked(FileInfo file)
        {
            FileStream stream = null;
            Boolean res = true;
            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                res = false;
            }
            catch (IOException)
            {
                //
            }
            finally
            {
                if (stream != null) stream.Close();
            }
            return res;
        }

        public String FileWatcherMd5Hash(String filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    return Functions.ByteArrayToString(md5.ComputeHash(stream));
                }
            }
        }
    }
}
