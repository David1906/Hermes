try {
    $hundredDaysAgo = (Get-Date).AddDays(-100)
    $filePath = "C:\Users\Foxconn\RiderProjects\Hermes\Hermes\bin\Debug\net8.0\Backup\100_days_ago5.txt"
    New-Item -ItemType File -Path $filePath
    (Get-Item  $filePath).LastWriteTime =$hundredDaysAgo
    (Get-Item  $filePath).CreationTime =$hundredDaysAgo
    Write-Host "File created successfully."
} catch {
    Write-Warning "An error occurred while creating the file: $($_.Exception.Message)"
}