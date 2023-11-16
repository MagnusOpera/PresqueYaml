namespace  MagnusOpera.PresqueYaml.Tests.CSharp;

public class TestCSharpCompability
{
    [Test]
    public void ReadAndDeserialize() {
        var yaml = "item1: value1";
        var node = YamlParser.Read(yaml);
        var dic = YamlSerializer.Deserialize<Dictionary<string, string>>(node, Defaults.options);
        Assert.That(dic.Count, Is.EqualTo(1));
        Assert.That(dic["item1"], Is.EqualTo("value1"));
    }
}
