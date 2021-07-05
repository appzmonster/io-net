# FileCompression

Provides static methods to compress and uncompress files, directories with or without password.

Example syntax:
```
string[] paths = new string[] { "./file-1.txt", "./directory-abc" };
string output = "output.zip";
FileCompression.Compress(paths, output);
```

## Usage
`FileCompression` provides functions to compress and uncompress with optional password and compression level configuration.

### Compress Single or Multiple Files / Directories
`FileCompression.Compress`
- Compresses the specified paths into a single file, with the specified password and compression level.

| Argument | Type | Optional | Description |
| ---      | ---  | :---:    | ---         |
| `paths`  | Array of string | No | The paths of the file system items to compress. Path can be a file or directory. |
| `outputPath` | String | No | The output file. |
| `password` | String | Yes | The password to protect the output file. |
| `compressionLevel` | Int | Yes | The compression level to use. Default is 9 (higher number indicates higher compression). |

Example:
```
...

// Multiple files and directories to compress into a single file.
string[] paths = new string[]
{
    "./assets/file-compression/sample-data.txt.zip",
    "./assets/file-compression/1.txt",
    "./assets/file-compression/3.csv",
    "./assets/file-compression/some-folder"
};

// Output zip file.
string outputFile = "my-compressed-file.zip";

// Protect the zip file with this password.
string password = "112233";

// Compress with password.
FileCompression.Compress(paths, outputFile, password);
```

### Uncompress File
`FileCompression.Uncompress`
- Uncompresses the specified file to the specified output path, with the specified password.

| Argument | Type | Optional | Description |
| ---      | ---  | :---:    | ---         |
| `path`  | Sstring | No | The file to uncompress. |
| `outputPath` | String | No | The output directory for the file to uncompress. |
| `password` | String | Yes | The password to unlock the file. |

Example:
```
...

// Zip file to uncompress.
string passwordedZipFile = "./assets/file-compression/passworded.zip";

// Uncompress to.
string destinationDirectory = "unzip";

// Password to unlock the zip file.
string password = "98279hlOJ";

// Uncompress with password.
FileCompression.Uncompress(passwordedZipFile, destinationDirectory, password);
```

## License
Copyright (c) 2021 Jimmy Leong (Github: appzmonster). Licensed under the MIT License.

[Back to main](./README.md#top)