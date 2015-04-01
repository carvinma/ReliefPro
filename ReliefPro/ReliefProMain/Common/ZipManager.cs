using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Packaging;

namespace ReliefProMain.Common
{
    public class ZipManager
    {
        public string SourceFolderPath { get; set; }
        public ZipManager()
        {
            
        }
        public void ZipFolder(string sourceFolderPath,string zipFilePath)
        {
            this.SourceFolderPath = sourceFolderPath;
            if (File.Exists(zipFilePath))
                File.Delete(zipFilePath);

            using(Package package=Package.Open(zipFilePath,System.IO.FileMode.Create))
            {
                DirectoryInfo di = new DirectoryInfo(sourceFolderPath);
                ZipDirectory(di, package);
            }
        }
        public bool UnZipFolder(string unZipFilePath,string targetZipFilePath)
        {
            bool result = false;
            try
            {
                if (!File.Exists(unZipFilePath))
                {
                    return result;
                }

                DirectoryInfo directoryInfo = new DirectoryInfo(targetZipFilePath);
                if (directoryInfo.Exists)
                {
                    Directory.Delete(targetZipFilePath, true);
                }
                directoryInfo.Create();

                using (Package package = Package.Open(unZipFilePath, FileMode.Open, FileAccess.Read))
                {
                    foreach (PackagePart packagePart in package.GetParts())
                    {
                        ExtractPart(packagePart, targetZipFilePath, true);
                    }
                }

                result = true;
            }
            catch (Exception e)
            {
                throw new Exception("Error unzipping file " + unZipFilePath, e);
            }
            return result;
        }
        private void ExtractPart(PackagePart packagePart, string targetDirectory, bool overrideExisting)
        {
            string stringPart = targetDirectory +packagePart.Uri.ToString().Replace('\\', '/');

            if (!Directory.Exists(Path.GetDirectoryName(stringPart)))
                Directory.CreateDirectory(Path.GetDirectoryName(stringPart));

            if (!overrideExisting && File.Exists(stringPart))
                return;
            using (FileStream fileStream = new FileStream(stringPart, FileMode.Create))
            {
                packagePart.GetStream().CopyTo(fileStream);
            }
        }
        private void ZipDirectory(DirectoryInfo di, Package package)
        {
            foreach (FileInfo fi in di.GetFiles())
            {
                string relivatePath = fi.FullName.Replace(SourceFolderPath, string.Empty);
                relivatePath = relivatePath.Replace("\\","/");

                PackagePart part = package.CreatePart(new Uri(relivatePath, UriKind.Relative), "application/x-zip-compressed", CompressionOption.Maximum);
                using (FileStream fs = fi.OpenRead())
                {
                    CopyStream(fs, part.GetStream());
                }
            }
            foreach (DirectoryInfo subDi in di.GetDirectories())
            {
                ZipDirectory(subDi, package);
            }
        }
        private void CopyStream(Stream source,Stream target)
        {
            const int bufSize = 0x10000;
            byte[] buf = new byte[bufSize];
            int bytesRead = 0;
            while ((bytesRead = source.Read(buf, 0, bufSize))>0)
            {
                target.Write(buf, 0, bytesRead);
            }
            
        }
    }
}
