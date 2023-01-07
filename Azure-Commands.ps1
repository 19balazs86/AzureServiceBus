# Install the Azure PowerShell module
# https://docs.microsoft.com/en-us/powershell/azure/install-az-ps

# Sign in
# Connect-AzAccount

param(
  [string]
  # [Parameter(Mandatory=$true)]
  $resGroupName = "test-ResGroup",

  [string] $location = "North Europe",

  [string] $serviceBusName = "balazs-test-ServBus",

  [string] $sku = "Standard"
)

Get-AzResourceGroup -Name $resGroupName -Location $location -ErrorVariable notPresent -ErrorAction SilentlyContinue | Out-Null

if ($notPresent)
{
    New-AzResourceGroup -Name $resGroupName -Location $location
}

Get-AzServiceBusNamespace `
    -ResourceGroupName $resGroupName `
    -NamespaceName $serviceBusName `
    -ErrorVariable notPresent `
    -ErrorAction SilentlyContinue | Out-Null

if ($notPresent)
{
    New-AzServiceBusNamespace `
        -ResourceGroupName $resGroupName `
        -Name $serviceBusName `
        -Location $location `
        -SkuName $sku `
        -MinimumTlsVersion 1.2
}

$keys = Get-AzServiceBusKey -ResourceGroupName $resGroupName -Namespace $serviceBusName -Name "RootManageSharedAccessKey"

Write-Host "PrimaryConnString = " $keys.PrimaryConnectionString