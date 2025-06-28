# FluentExceptionResults

A **clean, extensible, and production-ready** exception and result handling package for ASP.NET Core (.NET 8 / .NET 9).
Built to standardize API error responses, integrate with FluentValidation, and support structured logging — with **zero dependency** on a specific logging library.

---

## ✨ Features

* ✅ Unified handling of business, validation, auth, and unexpected exceptions
* ✅ Strongly-typed error codes with `ErrorEnumeration`
* ✅ Automatic `CorrelationId` middleware (RFC-compliant)
* ✅ FluentValidation support with a consistent error model
* ✅ Logger-scope enrichment for `CorrelationId` (Serilog-friendly)
* ✅ Supports both MVC and Minimal API
* ✅ Zero runtime dependency on Serilog or any logging framework

---

## 📆 Installation

```bash
dotnet add package FluentExceptionResults
```

---

## 🔧 Setup

### 1. Register services

In your `Program.cs`:

```csharp
builder.Services.AddFluentExceptionResults();
```

### 2. Add middleware

```csharp
app.UseFluentExceptionResults();
```

---

## 🚀 Usage

### Throw custom exceptions

```csharp
public sealed class UserNotFoundException : BusinessException
{
    public UserNotFoundException(Guid userId)
        : base(UserErrors.NotFound, $"User with ID '{userId}' not found.") { }
}
```

---

### Define error codes using `ErrorEnumeration`

```csharp
public sealed class UserErrors
{
    public static readonly ErrorEnumeration NotFound = new(1001, "User Not Found", "User");
}
```

---

### Wrap results in services or controller actions

```csharp
public async Task<IActionResult> Get(Guid id)
{
    var result = await _service.GetUser(id).ToResultAsync(_logger);
    return result.ToActionResult();
}
```

---

## 📄 Standard JSON Output

### ✅ Success

```json
{
  "success": true,
  "data": {
    "id": "abc-123",
    "name": "John Doe"
  }
}
```

### ❌ Business / Auth error

```json
{
  "success": false,
  "traceId": "00-abc123...",
  "errors": {
    "code": "User-1001",
    "message": "User with ID 'abc-123' not found."
  }
}
```

### ❌ Validation error (FluentValidation compatible)

```json
{
  "success": false,
  "traceId": "00-xyz...",
  "code": "Common-1000",
  "message": "Validation errors occurred.",
  "errors": [
    { "propertyName": "Email", "message": "Invalid email format." },
    { "propertyName": "Password", "message": "Password is required." }
  ]
}
```

---

## ✅ FluentValidation Integration

```csharp
var result = await validator.ValidateAsync(request);
if (!result.IsValid)
    throw result.ToAppValidationException();
```

Or use `ToResult<T>()` directly:

```csharp
var result = await validator.ValidateAsync(request);
return result.ToResult<UserDto>();
```

---

## 📊 Testing

* `AppExceptionHandler` handles all exceptions in a single place
* Logs include `CorrelationId` scope
* Output is always structured and consistent
* Each exception carries its own HTTP status and error code

---

## 📂 Project Structure

```
FluentExceptionResults/
├── Exceptions/
├── Middleware/
├── Results/
├── Extensions/
└── FluentExceptionResultsRegistration.cs
```

---

## 📋 License

[MIT](LICENSE)

---

## 🙌 Contributing

Pull requests and issues are welcome!
For feature suggestions, improvements, or bug reports, please create a GitHub issue.

---

### Created with ❤️ by [Behrouz Goudarzi](https://github.com/Behrouz-Goudarzi)
