# TsundokuApp Test Suite

This directory contains comprehensive unit tests for the TsundokuApp project using xUnit.

## Test Organization

Tests are organized by namespace to mirror the main project structure:

- **Models/** - Tests for data models (`Book`, `BookStatus`)
- **Services/** - Tests for service classes (`BookRepository`, `OpenLibraryBookLookupService`, `ImageService`)
- **ViewModels/** - Tests for MVVM view models (`LibraryViewModel`, `BookEditViewModel`)
- **Converters/** - Tests for value converters used in XAML bindings
- **Data/** - Tests for database initialization (`TsundokuDb`)

## Test Naming Conventions

All test methods follow a consistent naming pattern:
```
MethodName_StateUnderTest_ExpectedBehavior
```

Examples:
- `LookupByIsbnAsync_WithValidIsbn_ReturnsBookLookupResult`
- `Convert_WithNull_ReturnsFalse`
- `SaveAsync_WithNewBook_InsertsBook`

## Running Tests

### Run all tests:
```bash
dotnet test TsundokuAppTests/TsundokuAppTests.csproj
```

### Run with detailed output:
```bash
dotnet test TsundokuAppTests/TsundokuAppTests.csproj --logger "console;verbosity=detailed"
```

### Run tests for a specific class:
```bash
dotnet test TsundokuAppTests/TsundokuAppTests.csproj --filter FullyQualifiedName~BookTests
```

## Platform Limitations

Since the main app targets iOS and Mac Catalyst, some tests that depend on platform-specific MAUI APIs (like `FileSystem.AppDataDirectory` and `MainThread`) may not run on non-Apple platforms. These tests are:

- **TsundokuDbTests** - Database initialization tests (require FileSystem API)
- **BookRepositoryTests** - Repository integration tests (require database)
- **LibraryViewModelTests** (some tests) - Tests that use MainThread for UI updates
- **ImageServiceTests** - Tests that use SkiaSharp for image processing

These tests will pass when run on macOS with the appropriate workloads installed.

## Test Coverage

The test suite covers:

1. **Model Tests** (100% coverage)
   - Property getters and setters
   - Default values
   - Enum values

2. **Service Tests**
   - Book lookup service (ISBN normalization, API interaction)
   - Image service (download and storage)
   - Book repository (CRUD operations, sorting)

3. **ViewModel Tests**
   - Property changes and notifications
   - Command execution
   - Data loading and validation
   - Integration with services

4. **Converter Tests**
   - Value conversion logic
   - Null handling
   - Localization

## Dependencies

- **xUnit** - Test framework
- **Moq** - Mocking framework for dependencies
- **Microsoft.NET.Test.Sdk** - Test SDK
- **coverlet.collector** - Code coverage

## Contributing

When adding new functionality to the main app, please add corresponding tests:

1. Each test should test one thing
2. Use descriptive test names following the convention above
3. Organize tests in the appropriate namespace
4. Use Moq to mock dependencies
5. Keep tests isolated and independent
