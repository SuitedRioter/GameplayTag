
namespace Gameplay.Tag;

public class GameplayTagsSettings
{
    public string JsonDataForTest { get; set; }
    public string JsonSettingFilePath { get; set; } = "";
    
    public GameplayTagsSettings()
    {
        JsonDataForTest = "[\n                { \"tag_name\": \"A.B.C\", \"description\": \"Description of A.B.C\" },\n                { \"tag_name\": \"A.B.D\", \"description\": \"Description of A.B.D\" },\n                { \"tag_name\": \"A.C\", \"description\": \"Description of A.C\" },\n                { \"tag_name\": \"D\", \"description\": \"Description of D\" },\n                { \"tag_name\": \"D.C\", \"description\": \"Description of D\" },\n                { \"tag_name\": \"D.C.B\", \"description\": \"Description of D\" },\n                { \"tag_name\": \"A.C.B\", \"description\": \"Description of D\" }\n            ]";
    }
}