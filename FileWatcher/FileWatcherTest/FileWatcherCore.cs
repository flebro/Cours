using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileWatcherTest
{
    class FileWatcherCore
    {
        #region fields

        private FileSystemWatcher fileSystemWatcher;

        #endregion

        #region Properties

        /// <summary>
        /// Obtient ou défini le chemin du répertoire à écouter
        /// </summary>
        public string InputDirectoryPath { get; private set; }
        /// <summary>
        /// Obtient ou défini le chemin du répertoire de sortie
        /// </summary>
        public string OutputDirectoryPath { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialise une nouvelle instance de la class <see cref="FileWatcherCore"/>
        /// </summary>
        /// <param name="inputDirectoryPath">Répertoire à écouter</param>
        /// <param name="outputDirectoryPath">Répertoire de sortie</param>
        public FileWatcherCore(string inputDirectoryPath, string outputDirectoryPath)
        {
            InputDirectoryPath = inputDirectoryPath;
            OutputDirectoryPath = outputDirectoryPath;

            if (string.IsNullOrWhiteSpace(InputDirectoryPath) || Directory.)
            {
                throw new ArgumentException("Paramètre null ou vide", nameof(inputDirectoryPath));
            }

        }

        #endregion

        #region Methods

        public void Start()
        {
            
        }

        public void Stop()
        {

        }

        public void Pause()
        {

        }

        public void Resume()
        {

        }

        #endregion
    }
}
