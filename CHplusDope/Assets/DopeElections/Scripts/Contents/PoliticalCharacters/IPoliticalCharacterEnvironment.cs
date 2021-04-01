using Localizator;
using SpeechBubbles;

namespace DopeElections.PoliticalCharacters
{
    public interface IPoliticalCharacterEnvironment
    {
        ISpeechBubbleSource SpeechBubbleSource { get; }
        ILocalization Localization { get; }
    }
}