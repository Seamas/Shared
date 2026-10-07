# Wang.Seamas.Shared.AspNetCore

ASP.NET Core 共享组件包：全局过滤器、中间件、可扩展的异常映射机制。

## 依赖

| 依赖项                     | 类型               | 说明                                                       |
| -------------------------- | ------------------ | ---------------------------------------------------------- |
| `Wang.Seamas.Shared`       | NuGet 包           | 基础包：ApiResult、异常类型、CurrentUserContext            |
| `Microsoft.AspNetCore.App` | FrameworkReference | 不产生 NuGet 传递依赖，消费方（ASP.NET Core 应用）天然已有 |

目标框架为 `net10.0`（FrameworkReference 要求 `netcoreapp3.0+`，如需兼容旧版本请调整多目标）。

## 快速接入

### 1. 注册服务（必须在 AddControllers 之前）

```csharp
builder.Services.AddSharedAspNetCore();  // 必须放在builder.Services.AddControllers(options) 之前）

builder.Services.AddControllers(options =>
{
    options.AddSharedFilters();
});
```

### 2. 配置中间件管道

```csharp
app.UseGlobalErrorHandler();   // 必须放在管道最外层（UseRouting 之前）
app.UseRouting();

// ... 项目自己的认证中间件（如 TokenValidationMiddleware）...

app.UseCurrentUser();          // 必须在认证完成之后
app.MapControllers();
```

## 包内容

### 过滤器（由 `AddSharedFilters()` 挂载）

| 过滤器                  | 职责                                            |
| ----------------------- | ----------------------------------------------- |
| `ModelValidationFilter` | ModelState 无效时短路，返回 400 统一错误结构    |
| `GlobalExceptionFilter` | 兜底 Controller/Action 内异常，走异常映射       |
| `ApiResultFilter`       | 将 ObjectResult 自动包装为强类型 `ApiResult<T>` |

### 中间件

| 中间件                         | 职责                                                                                      |
| ------------------------------ | ----------------------------------------------------------------------------------------- |
| `GlobalErrorHandlerMiddleware` | 捕获 MVC 之外的异常（如认证中间件抛出的）；对非 200 状态码（404/403/401）补充统一错误结构 |
| `CurrentUserMiddleware`        | 从 ClaimsPrincipal 解析 `NameIdentifier` 写入 `CurrentUserContext`，请求结束自动清理      |

两层异常处理分工明确：Filter 管 Controller 内部，Middleware 管其余所有环节。两者共用同一 `IExceptionMapper`，行为一致。

### 响应约定

所有错误统一返回 **HTTP 200**，真实错误类型由 `ApiResult.Code` 表达，方便链路追踪与前端统一处理。

## 异常映射扩展（重点）

公共模块不知道各项目的具体异常类型，通过选项模式扩展。

### 内置默认映射

| 异常类型                      | Code      | 日志级别 |
| ----------------------------- | --------- | -------- |
| `LoginLockoutException`       | 429       | Warning  |
| `UnauthorizedAccessException` | 401       | Warning  |
| `AuthException`               | 401       | Warning  |
| `ValidateException`           | 400       | Warning  |
| `BizException`                | `ex.Code` | Warning  |
| `Exception`（兜底）           | 500       | Error    |

### 注册项目自己的异常

```csharp
builder.Services.AddSharedAspNetCore(options =>
{
    // 新增项目自定义异常
    options.Map<OrderNotFoundException>(ex =>
        new ExceptionMapping(404, ex.Message, LogLevel.Warning));

    // 覆盖内置映射（同类型后注册者生效）
    options.Map<Exception>(ex =>
        new ExceptionMapping(500, "服务器内部错误", LogLevel.Error));
});
```

### 映射查找规则

- 按异常的**运行时类型**查找，未命中则沿 `BaseType` 继承链向上匹配，最近的已注册基类生效。
- 因此可以只注册一个基类异常，其所有派生类自动适用；`Map<Exception>` 天然兜底。
- 类型 → 映射工厂的查找结果被缓存；工厂每次调用都会执行，映射里可以安全使用 `ex.Message`、`ex.Code` 等实例数据。

## 注意事项

1. **注册顺序**
   - `AddSharedAspNetCore()` 必须先于 `AddControllers(options => options.AddSharedFilters())`，否则过滤器拿不到 `IExceptionMapper`。
   - `UseGlobalErrorHandler()` 必须放在管道最外层，才能捕获所有后续中间件的异常。
   - `UseCurrentUser()` 依赖认证已完成的 `context.User`，必须放在认证中间件之后。

2. **系统异常消息外泄风险**
   默认兜底 `Map<Exception>` 将 `ex.Message` 直接返回给前端。生产环境如需隐藏内部细节（空引用堆栈信息等），请覆盖默认映射，返回固定文案。

3. **业务异常 vs 系统异常**
   - 可预期的业务校验失败：抛 `BizException`（或映射过的自定义异常），日志级别 Warning。
   - 不可预期的系统故障（空引用、DB 断连）：不要包装成业务异常，让它走到兜底映射，日志级别 Error 并带完整堆栈。
   - 服务层避免 `catch (Exception)` 后重新包装抛出，会吞掉原始异常类型导致映射失效。

4. **强业务耦合的组件不在包内**
   依赖项目自身抽象的中间件（如依赖 `ITokenService` 的认证中间件、依赖 `IPermissionChecker` 的权限中间件）仍留在各业务项目中。

5. **打包**
   已加入 `build.ps1` 的项目列表，执行脚本即随其他 Shared 包一起打包发布。

## 目录结构

```
Seamas.Shared.AspNetCore/
├── ExceptionHandling/          # 异常映射核心
│   ├── ExceptionMapping.cs         # 映射结果（Code/Message/LogLevel）
│   ├── IExceptionMapper.cs         # 映射器抽象
│   ├── ExceptionMapper.cs          # 继承链查找 + 缓存
│   ├── SharedAspNetCoreOptions.cs  # 选项（Map<TException> 入口）
│   └── DefaultExceptionMappings.cs # 内置默认映射
├── Filters/
│   ├── GlobalExceptionFilter.cs
│   ├── ModelValidationFilter.cs
│   └── ApiResultFilter.cs
├── Middlewares/
│   ├── GlobalErrorHandlerMiddleware.cs
│   └── CurrentUserMiddleware.cs
└── Extensions/
    ├── SharedAspNetCoreServiceCollectionExtensions.cs  # AddSharedAspNetCore
    ├── MvcOptionsExtensions.cs                          # AddSharedFilters
    └── SharedApplicationBuilderExtensions.cs            # Use* 扩展
```
