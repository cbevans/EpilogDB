# EpilogDB

A simple utility for clearing orphaned job data and compacting the Epilog Job Database. Reads the Epilog Job database (sqlite), identifies orphaned job data records, deletes them, and then compacts the database.

Run this on a backup copy of your job database, then restore the cleaned copy using the Epilog file manager.

### To use:
- Make a backup copy of your database with the Epilog file manager
- Download and Install .NET Desktop Runtime 5 from https://dotnet.microsoft.com/download/dotnet/5.0
- Download the EpilogDB.zip from the releases, unzip to any directory, and double-click EpilogDB.exe
- Select the backup copy of your database using the "..." button and click "Clean"
- Restore the cleaned copy of your database with the Epilog file manager

#### Requirements to Run:
- Windows 7 or Windows Server 2012 or later
- .Net Core 5.0
