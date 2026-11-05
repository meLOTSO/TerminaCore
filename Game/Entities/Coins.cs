using TerminaCore.Graphics;

public class Coins
{
    private int _sum = 0;
    private int _intention = 0;
    private bool _isIntention = false;

    public int Sum => _sum;
    public bool IsIntention => _isIntention;
    public Texture Texture { get; }
    public Texture IntentionTexture = new("       =     ");
    public ConsoleColor? PositiveIntentionColor { get; set; } = ConsoleColor.Green;
    public ConsoleColor? NegativeIntentionColor { get; set; } = ConsoleColor.DarkYellow;
    public ConsoleColor? NegativeSumColor { get; set; } = ConsoleColor.Red;

    public Coins()
    {
        this.Texture = new Texture(string.Format($"COINS: {_sum.ToString().PadLeft(4)}"));
    }

    public void IntentionToSummarize(int sum)
    {
        _isIntention = true;
        _intention += sum;
        UpdateIntention();
    }

    public void ApplyIntention()
    {
        _isIntention = false;
        if (_intention < 0)
        {
            _sum = Math.Max(-999, _sum + _intention);
        }
        else
        {
            _sum = Math.Min(999, _sum + _intention);
        }
        _intention = 0;
        _sum.ToString().PadLeft(4).AsSpan().CopyTo(this.Texture.Buffer.AsSpan(7));
    }

    private void UpdateIntention()
    {
        if (_intention + _sum < _sum)
        {
            IntentionTexture.ForegroundColor = NegativeIntentionColor;
            IntentionTexture.Buffer[1] = '-';
        }
        else
        {
            IntentionTexture.ForegroundColor = PositiveIntentionColor;
            IntentionTexture.Buffer[1] = '+';
        }
        if (_intention + _sum < 0)
        {
            IntentionTexture.ForegroundColor = NegativeSumColor;
        }
        Math.Abs(_intention).ToString().PadLeft(3).AsSpan().CopyTo(IntentionTexture.Buffer.AsSpan(3));
        (_intention < 0 ? Math.Max(-999, _sum + _intention) : Math.Min(999, _sum + _intention)).ToString().PadLeft(3).AsSpan().CopyTo(IntentionTexture.Buffer.AsSpan(9));
    }
}