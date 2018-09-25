public class PlayerResult {

    public string playerName;
    public int playerScore;
    public string resultDate;      // TODO: Make this a proper date format

    public PlayerResult(string playerName, int playerScore, string resultDate) {
        this.playerName = playerName;
        this.playerScore = playerScore;
        this.resultDate = resultDate;
    }    
}