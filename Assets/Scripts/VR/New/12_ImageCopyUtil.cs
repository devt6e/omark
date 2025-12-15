using System.IO;
using UnityEngine;

public static class ImageCopyUtil
{
    public static string CopyToPersistentPath(string srcPath, string fileName)
    {
        string dstPath = Path.Combine(
            Application.persistentDataPath,
            fileName
        );

        File.Copy(srcPath, dstPath, true);
        return dstPath;
    }
}
