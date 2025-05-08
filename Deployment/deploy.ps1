param (
    [Parameter(Mandatory=$true)]
    [string]$ResourceGroupName,
    
    [Parameter(Mandatory=$true)]
    [string]$Location,
    
    [Parameter(Mandatory=$true)]
    [string]$JwtSecret
)

# Create resource group if it doesn't exist
$resourceGroup = az group show --name $ResourceGroupName 2>$null
if (!$resourceGroup) {
    Write-Host "Creating resource group '$ResourceGroupName'..."
    az group create --name $ResourceGroupName --location $Location
}

# Deploy the Azure resources
Write-Host "Deploying Azure resources..."
$deploymentOutput = az deployment group create `
    --resource-group $ResourceGroupName `
    --template-file azure-resources.json `
    --parameters location=$Location jwtSecret=$JwtSecret `
    --output json

# Parse the deployment output
$deploymentJson = $deploymentOutput | ConvertFrom-Json
$backendUrl = $deploymentJson.properties.outputs.backendUrl.value
$frontendUrl = $deploymentJson.properties.outputs.frontendUrl.value

Write-Host "Deployment completed successfully!"
Write-Host "Backend API URL: $backendUrl"
Write-Host "Frontend URL: $frontendUrl"