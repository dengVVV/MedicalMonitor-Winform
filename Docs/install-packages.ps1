<#
.SYNOPSIS
  一键安装医疗设备监控上位机系统的所有 NuGet 依赖。

.DESCRIPTION
  本脚本读取同目录下的 packages.config，自动下载并还原所有依赖包。
  使用策略：
  1. 优先调用 dotnet restore（.NET SDK 自带，无需额外安装）
  2. 若 dotnet 不可用，查找已安装的 nuget.exe（PATH / 常见安装路径）
  3. 若 nuget.exe 也未找到，自动从 nuget.org 下载到本地 .tools/nuget.exe
  4. 无论哪种方式，最终在项目根目录执行 restore 还原所有包

.PARAMETER SolutionPath
  解决方案文件路径。默认值为项目根目录的 .sln 文件。
  可指定绝对路径或相对路径。

.PARAMETER NuGetSource
  NuGet 包源 URL。默认使用 nuget.org，也可指定离线源或私有源。

.PARAMETER SkipNuGetCheck
  跳过 nuget.exe 检查/下载步骤，直接执行 restore。

.EXAMPLE
  .\install-packages.ps1
  一键安装所有依赖。

.EXAMPLE
  .\install-packages.ps1 -NuGetSource "https://api.nuget.org/v3/index.json"
  指定包源后安装。

.EXAMPLE
  .\install-packages.ps1 -SolutionPath "D:\Projects\MySolution.sln"
  指定解决方案路径后安装。
#>

[CmdletBinding()]
param(
    [string]$SolutionPath = "",
    [string]$NuGetSource = "https://api.nuget.org/v3/index.json",
    [switch]$SkipNuGetCheck
)

$ErrorActionPreference = "Stop"
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$ProjectRoot = Resolve-Path "$ScriptDir\.."

# 若未指定解决方案路径，自动查找
if (-not $SolutionPath) {
    $slnFiles = Get-ChildItem -Path $ProjectRoot -Filter "*.sln" -Depth 0
    if ($slnFiles.Count -eq 0) {
        Write-Error "未在项目根目录找到 .sln 文件，请通过 -SolutionPath 参数指定。"
        exit 1
    }
    $SolutionPath = $slnFiles[0].FullName
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host " 医疗设备监控上位机系统 - 依赖安装工具" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "[信息] 项目根目录 : $ProjectRoot" -ForegroundColor Gray
Write-Host "[信息] 解决方案文件 : $SolutionPath" -ForegroundColor Gray
Write-Host ""

# ---- 选择还原方式 ----
$solutionDir = Split-Path $SolutionPath -Parent

# 策略 1：优先使用 dotnet restore
$dotnetExe = (Get-Command "dotnet.exe" -ErrorAction SilentlyContinue).Source
if ($dotnetExe) {
    Write-Host "[方式] 使用 dotnet restore 还原包" -ForegroundColor Cyan
    Write-Host "[执行] $dotnetExe restore $SolutionPath --source $NuGetSource" -ForegroundColor Gray

    Push-Location $solutionDir
    try {
        & $dotnetExe restore $SolutionPath --source $NuGetSource 2>&1 | ForEach-Object {
            $line = $_ | Out-String
            if ($line -match "error") {
                Write-Host $line -ForegroundColor Red -NoNewline
            }
            elseif ($line -match "warning") {
                Write-Host $line -ForegroundColor Yellow -NoNewline
            }
            else {
                Write-Host $line -ForegroundColor Gray -NoNewline
            }
        }

        if ($LASTEXITCODE -eq 0) {
            Write-Host ""
            Write-Host "========================================" -ForegroundColor Green
            Write-Host " ✓ 所有依赖包还原成功！(dotnet restore)" -ForegroundColor Green
            Write-Host "========================================" -ForegroundColor Green
            Write-Host ""
            Write-Host "提示：在 Visual Studio 中打开解决方案即可开始开发。" -ForegroundColor Gray
            return
        }
        else {
            Write-Host "[降级] dotnet restore 失败，尝试 nuget.exe..." -ForegroundColor Yellow
        }
    }
    catch {
        Write-Host "[降级] dotnet restore 异常: $_" -ForegroundColor Yellow
        Write-Host "[降级] 尝试 nuget.exe..." -ForegroundColor Yellow
    }
    finally {
        Pop-Location
    }
}
else {
    Write-Host "[信息] 未找到 dotnet.exe，尝试 nuget.exe..." -ForegroundColor Gray
}

# ---- 策略 2：使用 nuget.exe ----
$nugetPath = ""

if (-not $SkipNuGetCheck) {
    if (-not $dotnetExe) {
        # 在 PATH 中查找
        $nugetInPath = Get-Command "nuget.exe" -ErrorAction SilentlyContinue
        if ($nugetInPath) {
            $nugetPath = $nugetInPath.Source
            Write-Host "[OK] 找到 nuget.exe (PATH): $nugetPath" -ForegroundColor Green
        }
        else {
            # 检查常见安装路径
            $commonPaths = @(
                "$env:LOCALAPPDATA\NuGet\nuget.exe",
                "$env:ProgramData\NuGet\nuget.exe",
                "${env:ProgramFiles(x86)}\NuGet\nuget.exe",
                "$env:USERPROFILE\.nuget\nuget.exe"
            )
            $found = $false
            foreach ($p in $commonPaths) {
                if (Test-Path $p) {
                    $nugetPath = $p
                    $found = $true
                    Write-Host "[OK] 找到 nuget.exe: $nugetPath" -ForegroundColor Green
                    break
                }
            }

            if (-not $found) {
                # 下载 nuget.exe
                $nugetPath = "$ScriptDir\.tools\nuget.exe"
                $nugetDir = Split-Path $nugetPath -Parent
                if (-not (Test-Path $nugetDir)) {
                    New-Item -Path $nugetDir -ItemType Directory -Force | Out-Null
                }
                Write-Host "[下载] 正在下载 nuget.exe (v6.12.1)..." -ForegroundColor Yellow
                try {
                    $nugetUrl = "https://dist.nuget.org/win-x86-commandline/v6.12.1/nuget.exe"
                    Invoke-WebRequest -Uri $nugetUrl -OutFile $nugetPath -UseBasicParsing
                    Write-Host "[OK] nuget.exe 已下载到: $nugetPath" -ForegroundColor Green
                }
                catch {
                    Write-Error "下载 nuget.exe 失败: $_"
                    Write-Host "[提示] 请手动下载 https://nuget.org/nuget.exe 并放入 $ScriptDir\.tools\ 目录" -ForegroundColor Yellow
                    Write-Host "[提示] 或者确保已安装 .NET SDK（dotnet restore 优先）" -ForegroundColor Yellow
                    exit 1
                }
            }
        }

        # 验证 nuget.exe
        try {
            $nugetVersion = & $nugetPath help 2>&1 | Select-String -Pattern "NuGet Version" | ForEach-Object { $_ -replace '.*?([\d.]+).*', '$1' }
            Write-Host "[OK] nuget 版本: $nugetVersion" -ForegroundColor Green
        }
        catch {
            Write-Error "nuget.exe 验证失败: $_"
            exit 1
        }
    }
}
else {
    $nugetPath = "nuget.exe"
}

Write-Host ""
Write-Host "[方式] 使用 nuget.exe 还原包" -ForegroundColor Cyan
Write-Host "[执行] $nugetPath restore $SolutionPath -Source $NuGetSource -Verbosity Normal" -ForegroundColor Gray

Push-Location $solutionDir
try {
    & $nugetPath restore $SolutionPath -Source $NuGetSource -Verbosity Normal 2>&1 | ForEach-Object {
        $line = $_ | Out-String
        if ($line -match "error") {
            Write-Host $line -ForegroundColor Red -NoNewline
        }
        elseif ($line -match "warn") {
            Write-Host $line -ForegroundColor Yellow -NoNewline
        }
        else {
            Write-Host $line -ForegroundColor Gray -NoNewline
        }
    }

    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "========================================" -ForegroundColor Green
        Write-Host " ✓ 所有依赖包还原成功！(nuget restore)" -ForegroundColor Green
        Write-Host "========================================" -ForegroundColor Green
        Write-Host ""
        Write-Host "提示：在 Visual Studio 中打开解决方案即可自动生效。" -ForegroundColor Gray
        Write-Host "      如果智能提示未更新，请执行 生成 > 重新生成解决方案。" -ForegroundColor Gray
    }
    else {
        Write-Host ""
        Write-Error "还原失败 (ExitCode: $LASTEXITCODE)，请检查网络连接或包源地址。"
        exit $LASTEXITCODE
    }
}
catch {
    Write-Error "还原过程异常: $_"
    Pop-Location
    exit 1
}
finally {
    Pop-Location
}
