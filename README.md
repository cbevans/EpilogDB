# EpilogDB

A simple utility for clearing orphaned job data and compacting the Epilog Job Database. 
Read the Epilog Job database (sqlite), identifies orphaned job data records, deletes them, and then compacts the database.

Run this on a backup copy of your job database, then restore the cleaned copy using the Epilog file manager.

To use:
Download and Install .NET Core 5 from https://dotnet.microsoft.com/download/dotnet/5.0
Download the EpilogDB.zip from the releases, unzip to any directory, and double-click EpilogDB.exe
