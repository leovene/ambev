# Ensure the script stops on errors
$ErrorActionPreference = "Stop"

# Step 1: Clean the solution
Write-Host "Step 1: Cleaning the solution..." -ForegroundColor Green
dotnet clean "Ambev.sln"
if ($LASTEXITCODE -ne 0) {
    Write-Error "Error cleaning the solution. Exiting..."
    exit $LASTEXITCODE
}

# Step 2: Restore dependencies
Write-Host "Step 2: Restoring dependencies..." -ForegroundColor Green
dotnet restore "Ambev.sln"
if ($LASTEXITCODE -ne 0) {
    Write-Error "Error restoring dependencies. Exiting..."
    exit $LASTEXITCODE
}

# Step 3: Build the solution
Write-Host "Step 3: Building the solution..." -ForegroundColor Green
dotnet build "Ambev.sln" --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Error "Error building the solution. Exiting..."
    exit $LASTEXITCODE
}

# Step 4: Run the Web API with the HTTPS launch profile in the background
Write-Host "Step 4: Running the Web API with HTTPS profile in the background..." -ForegroundColor Green
Start-Process -NoNewWindow -FilePath "dotnet" -ArgumentList "run --project src\Ambev.WebApi.Sales\Ambev.WebApi.Sales.csproj --launch-profile https"
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error running the Web API. Exiting..." -ForegroundColor Red
    exit $LASTEXITCODE
}

# Step 5: Run the Worker with its specific launch profile in the background
Write-Host "Step 5: Running the Worker with its profile in the background..." -ForegroundColor Green
Start-Process -NoNewWindow -FilePath "dotnet" -ArgumentList "run --project src\Ambev.Worker.Sales\Ambev.Worker.Sales.csproj --launch-profile Ambev.Worker.Sales"
if ($LASTEXITCODE -ne 0) {
    Write-Host "Error running the Worker. Exiting..." -ForegroundColor Red
    exit $LASTEXITCODE
}

# Step 6: Navigate to the Angular project directory
Write-Host "Step 6: Navigating to Angular project directory..." -ForegroundColor Green
Set-Location "src\sales-documents-app"

# Step 7: Clean the Angular project
Write-Host "Step 7: Cleaning the Angular project..." -ForegroundColor Green
if (Test-Path "dist") { Remove-Item -Recurse -Force "dist" }
if (Test-Path "node_modules") { Remove-Item -Recurse -Force "node_modules" }

# Step 8: Install Angular dependencies
Write-Host "Step 8: Installing Angular dependencies..." -ForegroundColor Green
$npmCmd = (Get-Command npm.cmd).Source
& $npmCmd install
if ($LASTEXITCODE -ne 0) {
    Write-Warning "Warnings occurred during npm install, but continuing..."
}

# Step 9: Run the Angular application
Write-Host "Step 9: Running the Angular application..." -ForegroundColor Green
Start-Process -NoNewWindow -FilePath $npmCmd -ArgumentList "start"

# Final message
Write-Host "All services are running. The Web API, Worker, and Angular application should now be active." -ForegroundColor Cyan
