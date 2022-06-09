using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class LocalFileIOHandler
{
    private static string RootPath => $"{Application.dataPath}/";

    public static bool Save(in string text, in string directory, in string file, in string ext)
    {
        string path = Path.Combine(RootPath, directory, $"{file}.{ext}");

        FileInfo fi = new FileInfo(path);
        DirectoryInfo di = fi.Directory;

        if (!di.Exists)
            di.Create();

        File.WriteAllText(path, text);

        return true;
    }

    public static string Load(in string dir, in string file, in string ext)
    {
        string path = Path.Combine(RootPath, dir, $"{file}.{ext}");

        FileInfo fi = new FileInfo(path);

        if (fi.Exists)
            return null;

        return File.ReadAllText(path);
    }
}
