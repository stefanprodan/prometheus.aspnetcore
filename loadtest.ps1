workflow loadtest{
    Param([int]$Iterations, [string]$Url)

    $array = 1..$Iterations

    $startTime = get-date
    $JSON = @'
{
 "Log":"{data:'CF073A515F80A73441854FD15928D2A26A479EAB9DD5438548C670EBD35110C2857A69A834CAE51E36DD05AC669018D80D3C0A9AE5F3DF2C7614387E3BF58BD4'}"
}
'@
    foreach -Parallel -ThrottleLimit 10 ($i in $array){
       Invoke-RestMethod $Url -Method Post -Body $JSON -ContentType "application/json"
    }

    "elapsed time " + ((get-date) - $startTime).TotalSeconds + "sec"

}

# run load test
loadtest -Iterations 100000 -Url 'http://localhost:5100/ingest/event'
