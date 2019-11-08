$baseUrl = "http://localhost:7071/api";

$voteInfo = @{
    MinimumInFavor=2
    Voters=@("v1@info.ne", "v2@info.be", "v3@info.be", "v4@info.be")
};

$vote = Invoke-RestMethod "$baseUrl/voting" -Method Post -Body (ConvertTo-Json $voteInfo -Compress) -ContentType "application/json";
$voteId = $vote.votingId;

Invoke-RestMethod "$baseUrl/voting/$($vote.votingId)" -Method Get;

Invoke-RestMethod "$baseUrl/voting/$voteId/voters/$($voteInfo.Voters[0])/yea" -Method Post;

Invoke-RestMethod "$baseUrl/voting/$($vote.votingId)" -Method Get;

Invoke-RestMethod "$baseUrl/voting/$voteId/voters/$($voteInfo.Voters[1])/nay" -Method Post;

Invoke-RestMethod "$baseUrl/voting/$($vote.votingId)" -Method Get;

Invoke-RestMethod "$baseUrl/voting/$voteId/voters/$($voteInfo.Voters[2])/yea" -Method Post;

Invoke-RestMethod "$baseUrl/voting/$($vote.votingId)" -Method Get;

Invoke-RestMethod "$baseUrl/voting/$voteId/voters/$($voteInfo.Voters[3])/nay" -Method Post;
