using System;
using System.IO;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;

namespace Appzmonster.IO
{
    /// <summary>
    /// Provides static methods to compress and uncompress files, directories with or without password.
    /// </summary>
    public static class FileCompression
    {
        /// <summary>
        /// Compresses the specified paths into a single file, with the specified password and compression level.
        /// </summary>
        /// <param name="paths">The paths of the file system items to compress. Path can be a file or directory.</param>
        /// <param name="outputPath">The output file.</param>
        /// <param name="password">The password to protect the output file.</param>
        /// <param name="compressionLevel">The compression level to use.</param>
        public static void Compress(string[] paths, string outputPath, string password = null, int compressionLevel = 9)
        {
            List<string> sourcePaths = new List<string>();
            
            if (string.IsNullOrEmpty(outputPath))
            {
                throw new Exception("Output path is empty");
            }

            // Process given paths. Expand paths if the path is a directory.
            if ((paths != null) && (paths.Length > 0))
            {
                // Loop through every path and check type.
                for (int i = 0; i <= (paths.Length - 1); i++)
                {
                    if (File.GetAttributes(paths[i]).HasFlag(FileAttributes.Directory))
                    {
                        // For directory path.
                        //
                        // Adds all children file names and directory names of the directory to the 
                        // source path list for later compression processing.
                        var childrenPaths = Directory.GetFileSystemEntries(paths[i], "*.*", SearchOption.AllDirectories);
                        sourcePaths.AddRange(childrenPaths);
                    }
                    else
                    {
                        // For file path.
                        sourcePaths.Add(paths[i]);
                    }
                }
            }

            if (sourcePaths.Count > 0)
            {
                // Creates a zip output stream.
                using (var zipOutputStream = new ZipOutputStream(File.Create(outputPath)))
                {
                    // Sets password if specified.
                    if (string.IsNullOrEmpty(password) == false)
                    {
                        zipOutputStream.Password = password;
                    }

                    // Sets compression level.
                    zipOutputStream.SetLevel(compressionLevel);

                    for (int i = 0; i <= (sourcePaths.Count - 1); i++)
                    {
                        // Creates a zip entry for each path.
                        ZipEntry zipEntry = null;

                        if (File.GetAttributes(sourcePaths[i]).HasFlag(FileAttributes.Directory))
                        {
                            // Creates a zip directory item. Zip directory item must ends with '/'
                            // otherwise is treated as zip file item.
                            zipEntry = new ZipEntry((sourcePaths[i].EndsWith("/") ? sourcePaths[i] : sourcePaths[i] + "/"));
                        }
                        else
                        {
                            // Creates a zip file item.
                            zipEntry = new ZipEntry(sourcePaths[i]);
                        }

                        // Sets zip entry last modification date time.
                        zipEntry.DateTime = DateTime.Now;

                        // Adds the zip entry to the zip output stream. This zip entry becomes current active
                        // entry and all subsequent data write operations to the zip output stream affects this
                        // zip entry. This continues until PutNextEntry method is fired again to add another zip
                        // entry which closes the current active zip entry or the zip output stream closes.
                        zipOutputStream.PutNextEntry(zipEntry);

                        if (zipEntry.IsFile)
                        {
                            // For zip file item type.
                            //
                            // Reads source file item stream into chunks of 4096 bytes and writes the
                            // chunks of data to the zip output stream. Repeat this until end of
                            // source file item stream. These written data belongs to the current active
                            // zip entry.
                            byte[] buffer = new byte[4096];
                            using (FileStream fileStream = File.OpenRead(sourcePaths[i]))
                            {
                                int sourceBytes = 0;

                                do
                                {

                                    sourceBytes = fileStream.Read(buffer, 0, buffer.Length);
                                    zipOutputStream.Write(buffer, 0, sourceBytes);
                                } while (sourceBytes > 0);
                            }
                        }
                    }

                    // Completes the zip output stream.
                    zipOutputStream.Finish();
                    zipOutputStream.Close();

                    // Note:
                    // The above Finish and Close methods are optional to invoke.
                    // Dispose method invokes both Finish and Close methods internally.
                }
            }

            return;
        }

        /// <summary>
        /// Uncompresses the specified file to the specified output path, with the specified password.
        /// </summary>
        /// <param name="path">The file to uncompress.</param>
        /// <param name="outputPath">The output directory for the file to uncompress.</param>
        /// <param name="password">The password to unlock the file.</param>
        public static void Uncompress(string path, string outputPath = null, string password = null)
        {
            ZipFile zipFile = null;

            if (string.IsNullOrEmpty(outputPath))
            {
                outputPath = Path.GetDirectoryName(path);
            }

            try
            {
                if (File.Exists(path))
                {
                    // Opens the specified file for reading.
                    using (FileStream fileStream = File.OpenRead(path))
                    {
                        // Creates a zip file with the above reader stream.
                        zipFile = new ZipFile(fileStream);

                        // Sets password if specified (to unlock the file).
                        if (string.IsNullOrEmpty(password) == false)
                        {
                            zipFile.Password = password;
                        }

                        // Loop through each zip entry in the zip file, each zip entry
                        // can be either a file zip entry or directory zip entry.
                        foreach (ZipEntry zipEntry in zipFile)
                        {
                            // Constructs the zip entry output path (where the zip entry will uncompress).
                            // Formula: outputPath + zip entry name.
                            var zipEntryOutputPath = Path.Combine(outputPath, zipEntry.Name);

                            // Gets the directory path of the zip entry output path.
                            // Note:
                            // If the zip entry is a directory, we need to create the directory inside the output path
                            // directory so the zip entry can uncompress to the directory successfully.
                            var zipEntryOutputDirectoryPath = Path.GetDirectoryName(zipEntryOutputPath);
                            if (Directory.Exists(zipEntryOutputDirectoryPath) == false)
                            {
                                Directory.CreateDirectory(zipEntryOutputDirectoryPath);
                            }

                            if (zipEntry.IsFile)
                            {
                                // For zip file item.
                                //
                                // Reads the zip entry stream into chunks of 4096 bytes and write the chunks to the output file.
                                // Repeat this until zip entry stream reaches end of stream.
                                using (var zipInputStream = zipFile.GetInputStream(zipEntry))
                                {
                                    byte[] buffer = new byte[4096];
                                    using (var outputFileStreamWriter = File.Create(zipEntryOutputPath))
                                    {
                                        int sourceBytes = 0;

                                        do
                                        {
                                            sourceBytes = zipInputStream.Read(buffer, 0, buffer.Length);
                                            outputFileStreamWriter.Write(buffer, 0, sourceBytes);
                                        } while (sourceBytes > 0);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                if (zipFile != null)
                {
                    zipFile.IsStreamOwner = true;
                    zipFile.Close();
                }
            }
        }
    }
}
