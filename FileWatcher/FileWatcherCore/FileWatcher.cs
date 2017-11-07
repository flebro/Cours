using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatcherCore
{
    public class FileWatcher
    {
        #region Fields

        private string _InputDirectoryPath;
        private string _OutputDirectoryPath;
        private FileSystemWatcher _FileSystemWatcher;

        #endregion

        #region Properties

        /// <summary>
        /// Obtient ou définit le répertoire à écouter.
        /// </summary>
        public string InputDirectoryPath
        {
            get { return _InputDirectoryPath; }
            private set { _InputDirectoryPath = value; }
        }

        /// <summary>
        ///     Obtient ou définit le répertoire de sortie.
        /// </summary>
        public string OutputDirectoryPath
        {
            get { return _OutputDirectoryPath; }
            private set { _OutputDirectoryPath = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="FileWatcher.Test.FileWatcherCore"/>.
        /// </summary>
        /// <param name="inputDirectoryPath">Répertoire à écouter.</param>
        /// <param name="outputDirectoryPath">Répertoire de sortie.</param>
        public FileWatcher(string inputDirectoryPath, string outputDirectoryPath)
        {
            //ExpandEnvironmentVariables permet de résoudre les chemins comme %temp%
            InputDirectoryPath = Environment.ExpandEnvironmentVariables(inputDirectoryPath);
            OutputDirectoryPath = Environment.ExpandEnvironmentVariables(outputDirectoryPath);

            #region Arguments Testing

            if (string.IsNullOrWhiteSpace(InputDirectoryPath))
            {
                //throw new ArgumentException("Le paramètre " + nameof(inputDirectoryPath) 
                //    + " n'est pas défini ou vide."
                //    , nameof(inputDirectoryPath));

                throw new ArgumentException(
                    $"Le paramètre {nameof(inputDirectoryPath)} n'est pas défini ou vide."
                    , nameof(inputDirectoryPath));

                //$ devant une chaîne appel la méthode string.Format
                //string.Format("Le paramètre {0} n'est pas défini ou vide.", nameof(inputDirectoryPath));
            }

            if (string.IsNullOrWhiteSpace(OutputDirectoryPath))
            {
                throw new ArgumentException(
                $"Le paramètre {nameof(outputDirectoryPath)} n'est pas défini ou vide."
                , nameof(outputDirectoryPath));
            }

            #endregion

            #region Directories Check

            //Permet de vérifier si le dossier existe.
            if (!Directory.Exists(InputDirectoryPath))
            {
                //Créer le dossier et les sous-dossier manquant.
                Directory.CreateDirectory(InputDirectoryPath);
            }

            //CreateDirectory ne créé pas le dossier s'il existe déjà.
            Directory.CreateDirectory(OutputDirectoryPath);

            #endregion

            _FileSystemWatcher = new FileSystemWatcher(InputDirectoryPath);
            _FileSystemWatcher.IncludeSubdirectories = true;

            _FileSystemWatcher.Created += _FileSystemWatcher_Created;
            _FileSystemWatcher.Changed += _FileSystemWatcher_Changed;
            _FileSystemWatcher.Deleted += _FileSystemWatcher_Deleted;
            _FileSystemWatcher.Renamed += _FileSystemWatcher_Renamed;
        }

        #endregion

        #region Methods

        #region FileSystemWatcher Events

        private void _FileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            //Console.WriteLine("-------");
            //Console.WriteLine("RENAMED");
            //Console.WriteLine("OldFullPath : " + e.OldFullPath);
            //Console.WriteLine("FullPath : " + e.FullPath);
            //Console.WriteLine("-------");
        }

        private void _FileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            //Console.WriteLine("-------");
            //Console.WriteLine("DELETED");
            //Console.WriteLine("FullPath : " + e.FullPath);
            //Console.WriteLine("-------");
        }

        private void _FileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            //Console.WriteLine("-------");
            //Console.WriteLine("CHANGED");
            //Console.WriteLine("FullPath : " + e.FullPath);
            //Console.WriteLine("-------");
        }

        private void _FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine("-------");
            Console.WriteLine("CREATED");
            Console.WriteLine("FullPath : " + e.Name);
            Console.WriteLine("-------");

            if (File.Exists(e.FullPath))
            {
                FileStream fs = null;
                bool processed = false;
              do
                {
                    
                    try
                    {
                       fs = File.Open(e.FullPath, FileMode.Open, FileAccess.ReadWrite, FileShare.None); //Tentative d'ouverture
                        processed = true;
                    }
                    catch (Exception)
                    {
                        //Si on a une erreur c'est que File.Open n'arrive pas à avoir un accès exclusif au fichier
                        //Dans ce cas on temporise et on essai de nouveau
                        System.Threading.Thread.Sleep(100);
                    }
                    // La fermeture est automatique
                } while (!processed);

                string targetPath = OutputDirectoryPath + "\\" + e.Name;
                string targetDir = Path.GetDirectoryName(targetPath);
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }

                using (FileStream destinationStream = File.Open(targetPath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    // On copie le stream
                    fs.CopyTo(destinationStream);    
                }

                // On supprime
                fs.Close();
                string directoryPath = Path.GetDirectoryName(e.FullPath);
                File.Delete(e.FullPath);
                Clean(directoryPath);
            }
            else if (Directory.Exists(e.FullPath))
            {
                string destinationDir = OutputDirectoryPath + "\\" + e.Name;
                // Si le dossier source n'est pas vide on l'ignore car il sera traité au traitement d'un de ses enfants
                // Si le dossier destination existe, on ne créé pas
                if (!Directory.EnumerateFileSystemEntries(e.FullPath).Any() && !Directory.Exists(destinationDir))
                {
                    //Créer le dossier et les sous-dossier manquant.
                    Directory.CreateDirectory(destinationDir);
                }

                // On clean
                Clean(e.FullPath);
            }
        }

        #endregion

        private void Clean(string path)
        {
            string parent;
            while (!Directory.EnumerateFileSystemEntries(path).Any() && !path.Equals(InputDirectoryPath))
            {
                parent = Directory.GetParent(path).FullName;
                Directory.Delete(path);
                path = parent;
            }
        }

        #region Service Management

        public void Start()
        {
            _FileSystemWatcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            _FileSystemWatcher.EnableRaisingEvents = false;
        }

        public void Pause()
        {
            _FileSystemWatcher.EnableRaisingEvents = false;
        }

        public void Resume()
        {
            _FileSystemWatcher.EnableRaisingEvents = true;
        }

        #endregion

        #endregion
    }
}
