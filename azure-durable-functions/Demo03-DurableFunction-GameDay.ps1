$baseUrl = "http://localhost:7071/api";

$gameInfo = @{
    MinimumOfPlayers=1
    Players=@("p1@info.ne", "p2@info.be")
};

$game = Invoke-RestMethod "$baseUrl/Games" -Method Post -Body (ConvertTo-Json $gameInfo -Compress) -ContentType "application/json";
$gameId = $game.gameId;

Invoke-RestMethod "$baseUrl/Games/$($game.gameId)" -Method Get;


foreach($player in $gameInfo.Players)
{
    $status = "Available";

    if((Read-Host -Prompt "$player") -eq "n")
    {
        $status = "Unavailable"
    }

    Invoke-RestMethod "$baseUrl/Games/$gameId/Players/$player/$status" -Method Post;    
}
