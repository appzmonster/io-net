using System;
using System.IO;
using Xunit;
using Appzmonster.IO;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;

namespace Appzmonster.IO.FileCompressionTests
{
    public class FileCompressionTests
    {
        private readonly ITestOutputHelper _output;

        public FileCompressionTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData("./assets/file-compression/1.txt", "single-file.zip", true)]
        public void FileCompression_CompressSingleFile(string path, string testValue, bool expectedResult)
        {
            File.Delete(testValue);
            FileCompression.Compress(new string[] { path }, testValue);
            bool ok = File.Exists(testValue);

            Assert.True(ok == expectedResult);
        }

        [Theory]
        [InlineData(new string[] {
            "./assets/file-compression/sample-data.txt.zip",
            "./assets/file-compression/1.txt"
        }, "multi-files.zip", true)]
        public void FileCompression_CompressMultiFiles(string[] paths, string testValue, bool expectedResult)
        {
            File.Delete(testValue);
            FileCompression.Compress(paths, testValue);
            bool ok = File.Exists(testValue);

            Assert.True(ok == expectedResult);
        }

        [Theory]
        [InlineData(new string[] {
            "./assets/file-compression/sample-data.txt.zip",
            "./assets/file-compression/1.txt",
            "./assets/file-compression/3.csv",
            "./assets/file-compression/some-folder"
        }, "multi-mixed-files.zip", true)]
        public void FileCompression_CompressMultiFilesAndDirectories(string[] paths, string testValue, bool expectedResult)
        {
            File.Delete(testValue);
            FileCompression.Compress(paths, testValue);
            bool ok = File.Exists(testValue);

            Assert.True(ok == expectedResult);
        }

        [Theory]
        [InlineData(new string[] {
            "./assets/file-compression/1.txt",
            "./assets/file-compression/3.csv",
        }, "pass1234", "passworded.zip", true)]
        public void FileCompression_CompressWithPassword(string[] paths, string password, string testValue, bool expectedResult)
        {
            File.Delete(testValue);
            FileCompression.Compress(paths, testValue, password);
            bool ok = File.Exists(testValue);

            Assert.True(ok == expectedResult);
        }

        [Theory]
        [InlineData("./assets/file-compression/multi-mixed-files.zip", "multi-mixed-files", 10, true)]
        public void FileCompression_Uncompress(string path, string outputPath, int totalExpectedFiles, bool expectedResult)
        {
            if (Directory.Exists(outputPath))
            {
                Directory.Delete(outputPath, true);
            }

            FileCompression.Uncompress(path, outputPath);
            int totalFiles = 0;
            var files = Directory.GetFileSystemEntries(outputPath, "*.*", SearchOption.AllDirectories);
            if (files != null)
            {
                totalFiles = files.Count();
                _output.WriteLine($"totalFiles: {totalFiles}");

                for (int i = 0; i <= (files.Count() - 1); i++)
                {
                    _output.WriteLine($"{i + 1}: {files[i]}");
                }
            }
            bool isEqual = totalFiles == totalExpectedFiles;

            Assert.True(isEqual == expectedResult);
        }

        [Theory]
        [InlineData("./assets/file-compression/passworded.zip", "passworded", "pass1234", 4, true)]
        public void FileCompression_UncompressWithPassword(string path, string outputPath, string password, int totalExpectedFiles, bool expectedResult)
        {
            if (Directory.Exists(outputPath))
            {
                Directory.Delete(outputPath, true);
            }

            FileCompression.Uncompress(path, outputPath, password);
            int totalFiles = 0;
            var files = Directory.GetFileSystemEntries(outputPath, "*.*", SearchOption.AllDirectories);
            if (files != null)
            {
                totalFiles = files.Count();
                _output.WriteLine($"totalFiles: {totalFiles}");

                for (int i = 0; i <= (files.Count() - 1); i++)
                {
                    _output.WriteLine($"{i + 1}: {files[i]}");
                }
            }
            bool isEqual = totalFiles == totalExpectedFiles;

            Assert.True(isEqual == expectedResult);
        }
    }
}
