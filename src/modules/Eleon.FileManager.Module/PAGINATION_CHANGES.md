# Pagination Support for GetEntriesByParentId

## Overview

Pagination support has been added to the `GetEntriesByParentId` method throughout the FileManager module stack. This allows clients to retrieve file and folder entries in pages rather than loading all entries at once, improving performance for directories with many items.

## Changes Summary

### 1. Provider Layer
- **New Method**: `IVfsBlobProvider.ListPagedAsync(VfsListPagedArgs args)` - Already implemented in the Storage module
- **Helper Method**: `FilePathHelper.CreateListPagedArgs()` - Creates paged list arguments for the provider

### 2. Repository Layer
- **Interface**: `IFileSystemEntryRepository.GetEntriesByParentIdPaged()` - New paged method signature
- **Implementation**: 
  - `FileStorageProviderRepository.GetEntriesByParentIdPaged()` - Uses `ListPagedAsync` from provider
  - `FileVirtualRepository.GetEntriesByParentIdPaged()` - Uses EF Core paging

### 3. Domain Layer
- **FileManager**: `GetEntriesByParentIdPaged()` - Wraps repository paged method
- **FileDomainService**: `GetEntriesByParentIdPaged()` - Adds permission checks, status filtering, and favorite/external link enrichment

### 4. Application Layer
- **DTO**: `GetFileEntriesByParentPagedInput` - Input DTO inheriting from `PagedAndSortedResultRequestDto`
- **Interface**: `IFileAppService.GetEntriesByParentIdPaged()` - Returns `PagedResultDto<FileSystemEntryDto>`
- **Implementation**: `FileAppService.GetEntriesByParentIdPaged()` - Maps domain entities to DTOs

### 5. HTTP API Layer
- **Endpoint**: `GET /api/file-manager/files/GetEntriesByParentIdPaged`
- **Query Parameters**:
  - `folderId` (string, required) - Parent folder ID
  - `archiveId` (Guid, required) - Archive ID
  - `kind` (EntryKind?, optional) - Filter by File, Folder, or null for both
  - `fileStatuses` (List<FileStatus>, optional) - Filter by file statuses
  - `type` (FileManagerType, required) - File manager type
  - `skipCount` (int, default: 0) - Number of items to skip
  - `maxResultCount` (int, default: 10) - Maximum number of items to return
  - `sorting` (string, optional) - Sort field and direction (e.g., "name", "name desc", "lastModificationTime")

## Backward Compatibility

The existing non-paged `GetEntriesByParentId` methods remain unchanged and continue to work as before. They are not deprecated and can still be used for cases where all entries are needed or when pagination is not required.

## Usage Example

### HTTP Request
```
GET /api/file-manager/files/GetEntriesByParentIdPaged?folderId=folder123&archiveId=guid&skipCount=0&maxResultCount=20&sorting=name&kind=File&fileStatuses=Active&type=Provider
```

### Response
```json
{
  "totalCount": 150,
  "items": [
    {
      "id": "file1",
      "name": "document.pdf",
      "entryKind": "File",
      ...
    },
    ...
  ]
}
```

## Limitations

1. **Recursive Search**: The initial implementation supports paging only for non-recursive queries (direct children of a folder). Recursive paging across multiple folder levels is not yet supported.

2. **Status Filtering**: When filtering by `EntryKind` or `FileStatus`, the `TotalCount` returned may represent the total count of all items in the folder before filtering, not the filtered count. This is a known limitation that can be addressed in future enhancements.

3. **Sorting**: Sorting is applied at the repository/application layer. For `FileStorageProviderRepository`, sorting is done in-memory after retrieving items from the provider. For `FileVirtualRepository`, sorting is done at the database level using EF Core.

## Testing

Comprehensive tests should be added for:
- Repository paged methods (verify correct Items count and TotalCount)
- Domain service paged method (verify paging parameters are honored, stable sorting, and DTO mapping)
- API endpoint (basic integration test for query params → paging result)

Note: Full test implementation requires mocking multiple dependencies in `FileDomainService` (permission checker, favorite service, external link service, etc.).

## Future Enhancements

1. Support recursive paging across folder hierarchies
2. More accurate TotalCount when filtering by EntryKind or FileStatus
3. Provider-level sorting support (if the underlying storage provider supports it)
4. Additional sorting fields beyond name, lastModificationTime, and size


