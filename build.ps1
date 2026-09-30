$OutputDir = "./nupkgs"
$sln = "Shared"
$projects = @("Seamas.Shared")

# 删除指定目录
if (Test-Path $OutputDir) {
	Write-Host "🗑️  正在清理旧包目录: $OutputDir"
	Remove-Item -Path $OutputDir -Recurse -Force
}


# 清理生成的文件
dotnet clean "$sln.sln" --configuration Release

# restore
Write-Host "正在store"
dotnet restore

# 生成
Write-Host "正在build"
dotnet build --configuration Release


# 打包到指定目录
Write-Host "正在pack到 ./nupkgs"
foreach($proj in $projects) {
	dotnet pack "$($proj)/$($proj).csproj" --configuration Release --output ./nupkgs
}


# 获取所有 .nupkg 文件
$nupkgs = Get-ChildItem -Path $OutputDir -Filter "*.nupkg"
$apiKey = ""
$source = "https://api.nuget.org/v3/index.json"

foreach ($pkg in $nupkgs) {
	Write-Host "🚀 正在推送: $($pkg.Name)" -ForegroundColor Cyan
	
	dotnet nuget push $pkg.FullName --api-key $ApiKey --source $Source --skip-duplicate
}

# 删除指定目录
Remove-Item -Path $OutputDir -Recurse -Force