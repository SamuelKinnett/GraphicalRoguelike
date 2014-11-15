using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GraphicalRoguelike
{
    /// <summary>
    /// This class handles the saving and loading of files related to maps,
    /// items, player data etc.
    /// </summary>
    class FileManager
    {
        StreamReader streamReader;
        StreamWriter streamWriter;
        string currentDirectory;

        public FileManager()
        {
            currentDirectory = Directory.GetCurrentDirectory();

        }

        public void CreateWorldFolder(string folderName)
        {
            Directory.CreateDirectory(currentDirectory + @"\World\" + folderName);
        }

        public void SaveChunk()
        {

        }

    }
}
