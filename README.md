# GZip archiver

The C# console program compresses and decompresses files block-by-block using System.IO.Compression.GzipStream. The program effectively parallelizes and synchronizes the processing of blocks in a multiprocessor environment and processes files that are larger than the amount of available RAM. In case of exceptional situations, it informs the user with a clear message. 
Program parameters, names of source and result files must be specified in the command line as follows:

GZipTest.exe compress / decompress [source filename] [output filename]

Note: When working with threads, it uses only standard classes and libraries from .Net 3.5 (excluding ThreadPool, BackgroundWorker, TPL). That was a technical requirement.

made in 2019
