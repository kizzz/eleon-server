using System;

namespace VPortal.FileManager.Module.Tests.TestHelpers;

public static class FileManagerTestConstants
{
    public static readonly Guid TestArchiveId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid TestStorageProviderId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid TestTenantId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    public static readonly Guid TestUserId = Guid.Parse("44444444-4444-4444-4444-444444444444");
    
    public const string TestFileName = "testfile.txt";
    public const string TestFolderName = "TestFolder";
    public const string TestArchiveName = "TestArchive";
    public const string TestFileExtension = ".txt";
    public const string TestFilePath = "/test/path/file.txt";
    
    public static readonly string TestFileId = Guid.NewGuid().ToString();
    public static readonly string TestFolderId = Guid.NewGuid().ToString();
    public static readonly string TestRootFolderId = Guid.NewGuid().ToString();
}
