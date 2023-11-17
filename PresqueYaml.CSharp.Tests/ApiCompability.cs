namespace  MagnusOpera.PresqueYaml.Tests.CSharp;

public class Person {
    public Person(string firstname, string? lastname) {
        FirstName = firstname;
        LastName = lastname;
    }

    public string FirstName { get; init; }
    public string? LastName { get; init; }
}


public class TestCSharpCompability
{
    [Test]
    public void ReadAndDeserialize() {
        var yaml = "item1: value1";
        var node = YamlParser.Read(yaml);
        var dic = YamlSerializer.Deserialize<Dictionary<string, string>>(node);
        Assert.That(dic.Count, Is.EqualTo(1));
        Assert.That(dic["item1"], Is.EqualTo("value1"));
    }

    [Test]
    public void CheckNRT() {
        var yaml = "firstName: John";
        var node = YamlParser.Read(yaml);
        var person = YamlSerializer.Deserialize<Person>(node);
        Assert.That(person.FirstName, Is.EqualTo("John"));
        Assert.That(person.LastName, Is.Null);
    }

    [Test]
    public void CheckNRTRequired() {
        var yaml = "lastName: Doe";
        var node = YamlParser.Read(yaml);
        Assert.That(() => YamlSerializer.Deserialize<Person>(node), Throws.Exception);
    }
}
