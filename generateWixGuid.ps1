$path = "WixInstaller\Product.wxs"
$pattern = 'ProductCode = "\S+"'
$guid = [guid]::NewGuid().ToString()
$newValue = 'ProductCode = "'+$guid+'"'
(Get-Content $path) | 
Foreach-Object {$_ -replace $pattern,$newValue}  | 
Out-File $path
