
namespace Gameplay.Tag;

public class GameplayTagsSettings
{
    public string JsonDataForTest { get; set; }
    public string JsonSettingFilePath { get; set; } = "";
    
    public GameplayTagsSettings()
    {
        JsonDataForTest = "[\n                { \"tagName\": \"A.B.C\", \"description\": \"Description of A.B.C\" },\n                { \"tagName\": \"A.B.D\", \"description\": \"Description of A.B.D\" },\n                { \"tagName\": \"A.C\", \"description\": \"Description of A.C\" },\n                { \"tagName\": \"D\", \"description\": \"Description of D\" },\n                { \"tagName\": \"D.C\", \"description\": \"Description of D\" },\n                { \"tagName\": \"D.C.B\", \"description\": \"Description of D\" },\n                { \"tagName\": \"A.C.B\", \"description\": \"Description of D\" }\n            ]";
    }
}